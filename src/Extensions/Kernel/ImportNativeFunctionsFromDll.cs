using System.Reflection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Security;
using Microsoft.SemanticKernel.SkillDefinition;
using static PowerMatt.SKFromConfig.Extensions.Models.PluginConfig.FunctionsConfig;

namespace PowerMatt.SKFromConfig.Extensions.Kernel;

public static class ImportNativeFunctionsFromDllExtension
{
    public static IDictionary<string, ISKFunction> ImportNativeFunctionsFromDll(
        this IKernel kernel,
        cSharpFunctionsConfig cSharpFunctionFile,
        string? pluginName = null,
        ITrustService? trustService = null)
    {
        Assembly assembly;

        if (cSharpFunctionFile.dll == null)
        {
            assembly = Assembly.GetExecutingAssembly();
        }
        else
        {
            // Get the path of the currently executing assembly
            string assemblyPath = Assembly.GetExecutingAssembly().Location;

            // Get the directory containing the assembly
            string assemblyDirectory = Path.GetDirectoryName(assemblyPath)!;

            assembly = Assembly.LoadFrom(Path.Combine(assemblyDirectory, cSharpFunctionFile.dll));
        }

        Type? classType = assembly.GetType(cSharpFunctionFile.ClassName!);

        // Check if the class exists
        if (classType == null)
        {
            throw new Exception("Failed to load the class " + cSharpFunctionFile.ClassName);
        }

        // Add the functions to the kernel
        var functions = kernel.ImportSkill(Activator.CreateInstance(classType)!, pluginName);

        return functions;
    }


}