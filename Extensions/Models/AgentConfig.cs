using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace PowerMatt.SKFromConfig.Extensions.Models;

public class AgentConfig
{
    private static IDeserializer deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

    [YamlMember(Alias = "models")]
    public Dictionary<string, ModelConfig>? Models { get; set; }

    [YamlMember(Alias = "connections")]
    public Dictionary<string, ConnectionConfig>? Connections { get; set; }

    [YamlMember(Alias = "plugins")]
    public List<string>? Plugins { get; set; }

    [YamlMember(Alias = "logLevel")]
    public LogLevel? LogLevel { get; set; }

    public class ModelConfig
    {
        [YamlMember(Alias = "modelId")]
        public string? ModelId { get; set; }

        [YamlMember(Alias = "endpointType")]
        public string? EndpointType { get; set; }

        [YamlMember(Alias = "connection")]
        public string? Connection { get; set; }
    }

    public class ConnectionConfig
    {
        [YamlMember(Alias = "serviceType")]
        public string? ServiceType { get; set; }

        [YamlMember(Alias = "endPoint")]
        public string? Endpoint { get; set; }

        [YamlMember(Alias = "apiKey")]
        public string? ApiKey { get; set; }

        [YamlMember(Alias = "orgId")]
        public string? OrgId { get; set; }
    }

    public static AgentConfig FromDirectory(string directory, string environment = "dev")
    {
        // Load the default yaml file for the agent
        var defaultConfigFile = Path.Combine(directory, $"agent.yaml");
        var defaultConfig = FromFile(defaultConfigFile);

        // Load the environment specific yaml file for the agent if it exists
        var envConfigFile = Path.Combine(directory, $"agent.{environment}.yaml");
        var envConfig = File.Exists(envConfigFile) ? FromFile(envConfigFile) : null;

        // Merge the default and environment configurations into one object
        var mergedConfig = MergeUtilities.MergeObjects(defaultConfig, envConfig);

        // Return or store the merged configuration object
        return mergedConfig;
    }

    public static AgentConfig FromFile(string file)
    {
        return FromYaml(File.ReadAllText(file));
    }

    public static AgentConfig FromYaml(string yaml)
    {
        // Load the default yaml file for the agent
        var defaultConfig = deserializer.Deserialize<AgentConfig>(yaml);

        // Return or store the merged configuration object
        return defaultConfig;
    }
}

internal static class ServiceTypes
{
    internal const string OpenAI = "OPENAI";
    internal const string AzureOpenAI = "AZUREOPENAI";
}

internal static class EndpointTypes
{
    internal const string TextCompletion = "text-completion";
    internal const string ChatCompletion = "chat-completion";
}
