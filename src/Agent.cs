using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using PowerMatt.SKFromConfig.Extensions.Kernel;
using PowerMatt.SKFromConfig.Extensions.Models;
using PowerMatt.SKFromConfig.Extensions.Planner;
using PowerMatt.Gui.Views;
using Terminal.Gui;

namespace PowerMatt.SKFromConfig.Extensions.Agent;

public class ConsoleAgent
{
    private IKernel kernel;
    private IPlanner planner;
    private Microsoft.SemanticKernel.Planning.Plan? currentPlan;
    private ChatView? chatView;

    private ILogger<ConsoleAgent> logger;

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
        if (agentConfig.Planner?.Type == null)
        {
            throw new ArgumentException("No planner type found in agent config.");
        }

        // Create planner
        switch (agentConfig.Planner.Type)
        {
            case PlannerType.MockPlanner:
                planner = new MockPlanner(kernel);
                break;

            case PlannerType.ActionPlanner:
                planner = new ActionPlanner(kernel);
                break;

            case PlannerType.SequentialPlanner:
                planner = new SequentialPlanner(kernel);
                break;

            default:
                throw new ArgumentException($"Invalid planner type value: {agentConfig.Planner.Type}");
        }
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
        currentPlan = await planner.CreatePlanAsync(message);
        logger.LogTrace("Plan created: {plan}", currentPlan.ToJson(true));

        var result = await currentPlan.InvokeAsync();
        chatView!.Respond(result.ToString());
    }
}
