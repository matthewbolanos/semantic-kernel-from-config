using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.TextCompletion;
using Microsoft.SemanticKernel.Security;
using Microsoft.SemanticKernel.SemanticFunctions;
using Microsoft.SemanticKernel.SkillDefinition;
using PowerMatt.SKFromConfig.Extensions.Models;

namespace PowerMatt.SKFromConfig.Extensions.Kernel;

public static class ImportPromptFromConfigExtension
{
    public static ISKFunction ImportSemanticFunctionFromConfig(
        this IKernel kernel,
        string promptConfigFile,
        string? pluginName = null,
        ITrustService? trustService = null)
    {
        // Check if the file exists
        if (!File.Exists(promptConfigFile))
        {
            throw new Exception("File at path " + promptConfigFile + " not found");
        }

        var promptConfig = PromptConfig.FromFile(promptConfigFile);

        var config = new PromptTemplateConfig
        {
            Description = promptConfig.Description ?? "Generic function, unknown purpose",
            Type = "completion",
            IsSensitive = promptConfig.IsSensitive ?? false,
            Completion = new PromptTemplateConfig.CompletionConfig
            {
                Temperature = promptConfig.Completion?.Temperature ?? 0.0,
                TopP = promptConfig.Completion?.TopP ?? 1.0,
                PresencePenalty = promptConfig.Completion?.PresencePenalty ?? 0.0,
                FrequencyPenalty = promptConfig.Completion?.FrequencyPenalty ?? 0.0,
                MaxTokens = promptConfig.Completion?.MaxTokens ?? 100,
                StopSequences = promptConfig.Completion?.StopSequences ?? new List<string>()
            }
        };

        var func = kernel.CreateSemanticFunction(
            promptTemplate: promptConfig.Prompt ?? "",
            config: config,
            functionName: promptConfig.Name,
            skillName: pluginName,
            trustService: trustService);


        if (promptConfig.Model != null)
        {
            func.SetAIService(() => kernel.GetService<ITextCompletion>(promptConfig.Model));
        }

        return func;
    }


}