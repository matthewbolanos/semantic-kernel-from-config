using PowerMatt.SKFromConfig.Extensions.Agent;

var agentDirectory = Path.Combine(
    System.IO.Directory.GetCurrentDirectory(),
    "Configuration/Agents/DocumentExpert"
);
ConsoleAgent agent = new ConsoleAgent(agentDirectory);

await agent.SendMessageAsync("Summarize this document: https://learn.microsoft.com/en-us/semantic-kernel/");


