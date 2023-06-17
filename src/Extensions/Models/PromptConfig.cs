using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace PowerMatt.SKFromConfig.Extensions.Models;

public class PromptConfig
{
    private static IDeserializer deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

    [YamlMember(Alias = "name")]
    public string? Name { get; set; }

    [YamlMember(Alias = "prompt")]
    public string? Prompt { get; set; }

    [YamlMember(Alias = "model")]
    public string? Model { get; set; }

    [YamlMember(Alias = "type")]
    public string? Type { get; set; }

    [YamlMember(Alias = "description")]
    public string? Description { get; set; }

    [YamlMember(Alias = "completion")]
    public CompletionConfig? Completion { get; set; }

    [YamlMember(Alias = "defaultServices")]
    public List<string>? DefaultServices { get; set; }

    [YamlMember(Alias = "input")]
    public InputConfig? Input { get; set; }

    [YamlMember(Alias = "isSensitive")]
    public bool? IsSensitive { get; set; }

    public class CompletionConfig
    {
        [YamlMember(Alias = "temperature")]
        public double? Temperature { get; set; }

        [YamlMember(Alias = "topP")]
        public double? TopP { get; set; }

        [YamlMember(Alias = "presencePenalty")]
        public double? PresencePenalty { get; set; }

        [YamlMember(Alias = "frequencyPenalty")]
        public double? FrequencyPenalty { get; set; }

        [YamlMember(Alias = "maxTokens")]
        public int? MaxTokens { get; set; }

        [YamlMember(Alias = "stopSequences")]
        public List<string>? StopSequences { get; set; }
    }
    public class InputParameter
    {
        [YamlMember(Alias = "name")]
        public string? Name { get; set; }

        [YamlMember(Alias = "description")]
        public string? Description { get; set; }

        [YamlMember(Alias = "defaultValue")]
        public string? DefaultValue { get; set; }
    }

    public class InputConfig
    {
        [YamlMember(Alias = "parameters")]
        public List<InputParameter>? Parameters { get; set; }
    }

    public static PromptConfig FromYaml(string yaml)
    {
        return deserializer.Deserialize<PromptConfig>(yaml);
    }

    public static PromptConfig FromFile(string file)
    {
        return deserializer.Deserialize<PromptConfig>(System.IO.File.ReadAllText(file));
    }
}