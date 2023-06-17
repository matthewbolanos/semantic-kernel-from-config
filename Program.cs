using Microsoft.SemanticKernel;
using PowerMatt.SKFromConfig.Extensions;

var agentDirectory = Path.Combine(
    System.IO.Directory.GetCurrentDirectory(),
    "Configuration/Agents/DocumentExpert"
);
IKernel kernel = new KernelBuilder()
    .BuildWithAgentConfig(agentDirectory);

var f = kernel.Skills.GetFunction("DocumentExpert", "HelloWorld");
var result = await f.InvokeAsync();

Console.WriteLine(result);

