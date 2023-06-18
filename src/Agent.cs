using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using PowerMatt.SKFromConfig.Extensions.Kernel;
using PowerMatt.SKFromConfig.Extensions.Models;
using PowerMatt.Gui.Views;
using Terminal.Gui;
using Microsoft.SemanticKernel.SkillDefinition;
using System.Collections.Concurrent;

namespace PowerMatt.SKFromConfig.Extensions.Agent;

public class ConsoleAgent
{
    private IKernel kernel;
    private ISKFunction mainFunction;
    private ChatView? chatView;
    private ConcurrentDictionary<string, ContextThread> contextThreads = new ConcurrentDictionary<string, ContextThread> { };
    private ILogger<ConsoleAgent>? logger;


    public ConsoleAgent(string agentDirectory)
    {
        // Create kernel builder
        var kernelBuilder = new KernelBuilder();

        // Get agent config
        var agentConfig = AgentConfig.FromDirectory(agentDirectory);

        // Check if there are any models
        if (agentConfig.Models == null || agentConfig.Models.Count == 0)
        {
            throw new ArgumentException("No models found in agent config.");
        }

        // Check if there are any connections
        if (agentConfig.Connections == null || agentConfig.Connections.Count == 0)
        {
            throw new ArgumentException("No connections found in agent config.");
        }

        // Add models to kernel
        foreach (var model in agentConfig.Models)
        {
            // Check if there is a model ID
            if (model.Value.ModelId == null)
            {
                throw new ArgumentException($"Model {model.Key} does not have a model ID.");
            }

            // Check if model has a connection
            if (model.Value.Connection == null)
            {
                throw new ArgumentException($"Model {model.Key} does not have a connection.");
            }

            // Check if connection exists
            if (!agentConfig.Connections.ContainsKey(model.Value.Connection))
            {
                throw new ArgumentException($"Connection {model.Value.Connection} not found in agent config.");
            }


            // Get connection
            var connection = agentConfig.Connections[model.Value.Connection];

            // Check if there is a service type
            if (connection.ServiceType == null)
            {
                throw new ArgumentException($"Connection {model.Value.Connection} does not have a service type.");
            }

            // Check if there is an API key
            if (connection.ApiKey == null)
            {
                throw new ArgumentException($"Connection {model.Value.Connection} does not have an API key.");
            }


            // Add service
            switch (connection.ServiceType)
            {
                case ServiceType.AzureOpenAI:
                    // Check if there is an endpoint
                    if (connection.Endpoint == null)
                    {
                        throw new ArgumentException($"Connection {model.Value.Connection} does not have an endpoint.");
                    }

                    if (model.Value.EndpointType == EndpointType.TextCompletion)
                    {
                        kernelBuilder.WithAzureTextCompletionService(deploymentName: model.Value.ModelId, endpoint: connection.Endpoint, apiKey: connection.ApiKey, serviceId: model.Key);
                    }
                    else if (model.Value.EndpointType == EndpointType.ChatCompletion)
                    {
                        kernelBuilder.WithAzureChatCompletionService(deploymentName: model.Value.ModelId, endpoint: connection.Endpoint, apiKey: connection.ApiKey, serviceId: model.Key);
                    }
                    break;

                case ServiceType.OpenAI:
                    if (model.Value.EndpointType == EndpointType.TextCompletion)
                    {
                        kernelBuilder.WithOpenAITextCompletionService(modelId: model.Value.ModelId, apiKey: connection.ApiKey, orgId: connection.OrgId, serviceId: model.Key);
                    }
                    else if (model.Value.EndpointType == EndpointType.ChatCompletion)
                    {
                        kernelBuilder.WithOpenAIChatCompletionService(modelId: model.Value.ModelId, apiKey: connection.ApiKey, orgId: connection.OrgId, serviceId: model.Key);
                    }
                    break;

                default:
                    throw new ArgumentException($"Invalid service type value: {connection.ServiceType}");
            }
        }

        // Add logger
        if (agentConfig.LogLevel != null)
        {
            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .SetMinimumLevel(agentConfig.LogLevel ?? LogLevel.Warning)
                    .AddDebug();
            });
            logger = loggerFactory.CreateLogger<ConsoleAgent>();
            kernelBuilder.WithLogger(loggerFactory.CreateLogger<ConsoleAgent>());
        }

        // Build kernel
        kernel = kernelBuilder.Build();

        // Add plugins
        if (agentConfig.Plugins != null)
        {
            foreach (var pluginFile in agentConfig.Plugins)
            {
                kernel.ImportPluginFromConfig(
                    Path.Combine(agentDirectory, pluginFile)
                );
            }
        }

        // Check if there is a planner type
        if (agentConfig.MainFunction == null)
        {
            throw new ArgumentException("No main function found in agent config.");
        }

        // Set main function
        string[] mainFunctionParts = agentConfig.MainFunction.Split('.');
        mainFunction = kernel.Skills.GetFunction(mainFunctionParts[0], mainFunctionParts[1])!;
    }

    public void Start()
    {
        Action<string> onInput = async (string input) =>
        {
            await SendMessageAsync(input);
        };

        Application.Init();

        chatView = new ChatView(onInput);

        Application.Run(chatView);
        Application.Shutdown();
    }

    public async Task SendMessageAsync(string message)
    {
        // Check if any context threads should receive the message
        bool messageReceived = false;
        foreach (var ct in contextThreads)
        {
            if (ct.Value.IsWaitingForUserInput())
            {
                ct.Value.ReceiveMessage(message);
                messageReceived = true;
            }
        }

        // If not, send to main function
        if (!messageReceived)
        {
            // create new context thread
            var context = kernel.CreateNewContext();
            context["input"] = message;
            context["goalAchieved"] = "FALSE";

            var contextThread = new ContextThread(
                kernel,
                context,
                mainFunction,
                chatView!.Respond);

            // Create random ID
            string id = Guid.NewGuid().ToString();

            // add context to concurrent dictionary
            contextThreads.TryAdd(id, contextThread);

            await contextThread.StartAsync();

            // remove context from concurrent dictionary
            contextThreads.TryRemove(id, out _);
        }
    }
}
