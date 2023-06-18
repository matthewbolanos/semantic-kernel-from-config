using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Security;
using Microsoft.SemanticKernel.SkillDefinition;
using PowerMatt.SKFromConfig.Extensions.Models;

namespace PowerMatt.SKFromConfig.Extensions.Kernel;

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

        var pluginConfig = PluginConfig.FromFile(pluginConfigFile);
        List<ISKFunction> Functions = new List<ISKFunction> { };

        var pluginDirectory = new FileInfo(pluginConfigFile).Directory!.FullName;

        if (pluginConfig.Functions?.SemanticFunctions != null)
        {
            foreach (var semanticFunctionFile in pluginConfig.Functions.SemanticFunctions)
            {
                var f = kernel.ImportSemanticFunctionFromConfig(
                    Path.Combine(pluginDirectory, semanticFunctionFile.path!),
                    pluginName: pluginConfig.Name,
                    trustService: trustService
                );
                Functions.Add(f);
            }
        }

        if (pluginConfig.Functions?.CSharpFunctions != null)
        {
            foreach (var cSharpFunctionFile in pluginConfig.Functions.CSharpFunctions)
            {
                var functions = kernel.ImportNativeFunctionsFromDll(
                    cSharpFunctionFile,
                    pluginName: pluginConfig.Name,
                    trustService: trustService
                );

                // loop over the functions and add them to the list
                foreach (var f in functions)
                {
                    Functions.Add(f.Value);
                }
            }
        }

        return new Plugin
        {
            Name = pluginConfig.Name,
            Functions = Functions
        };
    }


}