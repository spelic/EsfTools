using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System.Text;


namespace EsfParseDemo;

public static class Program
{
    public static void Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        string path = args.Length > 0 ? args[0] : "M000A-V22.esf";
        //path = "D123A-V54.esf";
        if (!File.Exists(path))
        {
            Console.WriteLine($"File not found: {path}");
            return;
        }

        // ---- create lexer / parser ----------------------------------------------------
        using var reader = File.OpenText(path);
        var input = CharStreams.fromTextReader(reader);
        var lexer = new ESFLexer(input);
        var tokens = new CommonTokenStream(lexer);
        var parser = new ESFParser(tokens) { BuildParseTree = true };

        var tree = parser.file();          // "file" is the grammar's root rule

        // ---- 1) one-liner (classic ANTLR view) ---------------------------------------
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n=== ToStringTree view ===\n");
        Console.ResetColor();
        Console.WriteLine(tree.ToStringTree(parser));

        // ---- 2) pretty, indented outline --------------------------------------------
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n=== Indented view ===\n");
        Console.ResetColor();
        ParseTreeWalker.Default.Walk(new IndentedPrinter(parser), tree);
    }

    /// Listener that prints each rule as we enter/exit, indenting by depth.
    private sealed class IndentedPrinter(ESFParser parser) : ESFBaseListener
    {
        private readonly string[] _ruleNames = parser.RuleNames;
        private int _depth;

        public override void EnterEveryRule(ParserRuleContext ctx)
        {
            Console.WriteLine($"{new string(' ', _depth * 2)}{_ruleNames[ctx.RuleIndex]}");
            _depth++;
        }
        public override void ExitEveryRule(ParserRuleContext ctx) => _depth--;
    }
}
