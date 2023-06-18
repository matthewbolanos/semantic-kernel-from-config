using PowerMatt.SKFromConfig.Extensions.Agent;

string agentDirectory = Path.Combine(
            System.IO.Directory.GetCurrentDirectory(),
            "Configuration/Agents/DocumentExpert"
        );
ConsoleAgent agent = new ConsoleAgent(agentDirectory);
agent.Start();