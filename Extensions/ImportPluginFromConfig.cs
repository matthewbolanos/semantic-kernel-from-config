using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Security;
using Microsoft.SemanticKernel.SkillDefinition;
using PowerMatt.SKFromConfig.Extensions.Models;

namespace PowerMatt.SKFromConfig.Extensions;

public class Plugin
{
    public string? Name { get; set; }
    public List<ISKFunction>? Functions { get; set; }
}

public static class ImportPluginFromConfigExtension
{
    public static Plugin ImportPluginFromConfig(
        this IKernel kernel,
        string pluginConfigFile,
        ITrustService? trustService = null)
    {

        var promptConfig = PluginConfig.FromFile(pluginConfigFile);
        List<ISKFunction> Functions = new List<ISKFunction> { };

        var pluginDirectory = new FileInfo(pluginConfigFile).Directory!.FullName;

        if (promptConfig.Functions?.SemanticFunctions != null)
        {
            foreach (var semanticFunctionFile in promptConfig.Functions.SemanticFunctions)
            {
                var f = kernel.ImportSemanticFunctionFromConfig(
                    Path.Combine(pluginDirectory, semanticFunctionFile),
                    pluginName: promptConfig.Name,
                    trustService: trustService
                );
                Functions.Add(f);
            }
        }

        return new Plugin
        {
            Name = promptConfig.Name,
            Functions = Functions
        };
    }


}