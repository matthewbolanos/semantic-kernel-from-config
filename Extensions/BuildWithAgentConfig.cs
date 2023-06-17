using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using PowerMatt.SKFromConfig.Extensions.Models;

namespace PowerMatt.SKFromConfig.Extensions;

internal static class WithAgentConfigExtensions
{
    /// <summary>
    /// Adds a text completion service to the list. It can be either an OpenAI or Azure OpenAI backend service.
    /// </summary>
    /// <param name="kernelBuilder"></param>
    /// <param name="agentDirectory"></param>
    /// <exception cref="ArgumentException"></exception>
    internal static IKernel BuildWithAgentConfig(this KernelBuilder kernelBuilder, string agentDirectory)
    {
        // Get agent config
        var agentConfig = AgentConfig.FromDirectory(agentDirectory);

        // Add models
        foreach (var model in agentConfig.Models)
        {
            // Get connection
            var connection = agentConfig.Connections[model.Value.Connection];
            string serviceType = connection.ServiceType.ToUpper();

            // Add service
            switch (serviceType)
            {
                case ServiceTypes.AzureOpenAI:
                    if (model.Value.EndpointType == EndpointTypes.TextCompletion)
                    {
                        kernelBuilder.WithAzureTextCompletionService(deploymentName: model.Value.ModelId, endpoint: connection.Endpoint, apiKey: connection.ApiKey, serviceId: model.Key);
                    }
                    else if (model.Value.EndpointType == EndpointTypes.ChatCompletion)
                    {
                        kernelBuilder.WithAzureChatCompletionService(deploymentName: model.Value.ModelId, endpoint: connection.Endpoint, apiKey: connection.ApiKey, serviceId: model.Key);
                    }
                    break;

                case ServiceTypes.OpenAI:
                    if (model.Value.EndpointType == EndpointTypes.TextCompletion)
                    {
                        kernelBuilder.WithOpenAITextCompletionService(modelId: model.Value.ModelId, apiKey: connection.ApiKey, orgId: connection.OrgId, serviceId: model.Key);
                    }
                    else if (model.Value.EndpointType == EndpointTypes.ChatCompletion)
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
                    .AddConsole()
                    .AddDebug();
            });
            kernelBuilder.WithLogger(loggerFactory.CreateLogger<IKernel>());
        }

        IKernel kernel = kernelBuilder.Build();

        // Add plugins
        foreach (var pluginFile in agentConfig.Plugins)
        {
            kernel.ImportPluginFromConfig(
                Path.Combine(agentDirectory, pluginFile)
            );
        }

        return kernel;
    }
}
