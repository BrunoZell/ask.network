using System.CommandLine;
using System.CommandLine.Invocation;

namespace AskFi.Cli;

public class StrategyBacktest : Command
{
    public class CommandOptions
    {
        public FileInfo Project { get; set; } = null!;
        public string Strategy { get; set; } = null!;

        public string ContextId { get; set; } = null!;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    public StrategyBacktest(string name, string? description = null)
        : base(name, description)
    {
        Add(new Argument<FileInfo>("project", "Path to the csproj or fsproj containing the strategy."));
        Add(new Argument<string>("strategy", ".NET full type name identifying the strategy module. Must contain a static function 'decide' with a signature of 'Reflection -> Context -> Decision'."));
        Add(new Argument<string>("context-id", "Base32 encoded hash of the initial node of the context sequence to evaluate the strategy to."));
        Add(new Argument<DateTime>("start", "Earliest timestamp of the first context to evaluate the strategy on."));
        Add(new Argument<DateTime>("end", "Latest timestamp before which to gracefully complete the backtest."));

        Handler = CommandHandler.Create<CommandOptions>(Execute);
    }

    private static async Task Execute(CommandOptions options)
    {
        // 1: Compile project, identify dll. Abort on error.

        // CliWrap.exec("dotnet build -p project -c Debug")
        // Locate bin/net7.0/project.dll
        // Load dll as bytes[], combine with strategy name, print strategy-id (CID)

        // 2: Load dll into AssemblyLoadContext, reference strategy decision function. Print strategy id. Abort on error.

        // var assembly = new AssemblyLoadContext("path/project.dll");
        // var strategy = assembly.Load(options.Strategy);

        // 3: Load latest context sequence head, identify first context and last context from --start and --end. Print details.

        // Query state server for latest available context sequence head under --context-id.
        // var query = QueryContext.Load(tradeContextSequenceHead, _persistence); // Reads whole context sequence (w/o observations) and builds in-memory index of timestamps.
        // Run time-index range query from --start to --end
        // Print first context, last context.

        // 4: 

        Console.WriteLine($"Downloading historic Bybit trades from {options.From} to {options.To} for instrument {options.Instrument}...");
    }
}
