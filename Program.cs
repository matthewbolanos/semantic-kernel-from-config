using PowerMatt.SKFromConfig.Extensions.Agent;

var agentDirectory = Path.Combine(
    System.IO.Directory.GetCurrentDirectory(),
    "Configuration/Agents/DocumentExpert"
);
ConsoleAgent agent = new ConsoleAgent(agentDirectory);

await agent.SendMessageAsync("This is a test");


