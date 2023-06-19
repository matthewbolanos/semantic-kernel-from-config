using Microsoft.Extensions.Logging;
using PowerMatt.Sample;
using PowerMatt.SKFromConfig.Extensions.Agent;

// Create logger
using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .SetMinimumLevel(0)
        .AddDebug();
});
var logger = loggerFactory.CreateLogger<ConsoleAgent>();

// Instantiate custom AI orchestrator
var orchestrator = new SimpleOrchestrator(logger);

// Build agent with configuration files and custom AI orchestrator
string agentDirectory = Path.Combine(
            System.IO.Directory.GetCurrentDirectory(),
            "Configuration/Agents/ChatAgent"
        );
ConsoleAgent agent = new ConsoleAgent(agentDirectory, orchestrator);
agent.Start();