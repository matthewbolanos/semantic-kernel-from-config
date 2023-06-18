using Microsoft.Extensions.Logging;
using PowerMatt.SKFromConfig.Extensions.Agent;


using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .SetMinimumLevel(0)
        .AddDebug();
});
var logger = loggerFactory.CreateLogger<ConsoleAgent>();


string agentDirectory = Path.Combine(
            System.IO.Directory.GetCurrentDirectory(),
            "Configuration/Agents/DocumentExpert"
        );
ConsoleAgent agent = new ConsoleAgent(agentDirectory, new GoalOrchestrator(logger));
agent.Start();