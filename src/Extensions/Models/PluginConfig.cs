using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace PowerMatt.SKFromConfig.Extensions.Models;

public class PluginConfig
{
    private static IDeserializer deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

    [YamlMember(Alias = "name")]
    public string? Name { get; set; }

    [YamlMember(Alias = "description")]
    public string? Description { get; set; }

    [YamlMember(Alias = "Functions")]
    public FunctionsConfig? Functions { get; set; }

    public class FunctionsConfig
    {
        [YamlMember(Alias = "semanticFunctions")]
        public string[]? SemanticFunctions { get; set; }

        [YamlMember(Alias = "cSharpFunctions")]
        public cSharpFunctionsConfig[]? CSharpFunctions { get; set; }

        public class cSharpFunctionsConfig
        {
            [YamlMember(Alias = "dll")]
            public string? dll { get; set; }

            [YamlMember(Alias = "className")]
            public string? ClassName { get; set; }
        }
    }

    public static PluginConfig FromFile(string file)
    {
        return deserializer.Deserialize<PluginConfig>(System.IO.File.ReadAllText(file));
    }

    public static PluginConfig FromYaml(string yaml)
    {
        return deserializer.Deserialize<PluginConfig>(yaml);
    }
}