using EsfParser.Parser.Logic.Statements;
using System.Text;
using System.Text.RegularExpressions;

namespace EsfParser.Parser.Logic
{
    // === Visualizer ===
    public static class StatementTreeVisualizer
    {
        public static void ExportToGraphviz(IEnumerable<IStatement> statements, string outputPath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("digraph LogicTree {");
            sb.AppendLine("    node [shape=box, style=rounded, fontname=Arial];");

            int nodeId = 0;
            var nodeStack = new Stack<(int parentId, List<IStatement> children)>();
            nodeStack.Push((-1, statements.ToList()));

            while (nodeStack.Count > 0)
            {
                var (parentId, children) = nodeStack.Pop();
                foreach (var stmt in children)
                {
                    int currentId = nodeId++;
                    string label = stmt.Type.ToString();
                    string details = stmt is IfStatement ifs ? ifs.Condition :
                                    stmt is WhileStatement wh ? wh.Condition :
                                    stmt is AssignStatement a ? $"{a.Left} = {a.Right}" :
                                    stmt is MoveStatement m ? $"{m.Destination} = {m.Source}" :
                                    stmt is CallStatement c ? $"{c.ProgramName}" :
                                    stmt is TestStatement t ? t.Expression :
                                    stmt is DxfrStatement d ? $"{d.TargetApp}->{d.TargetScreen}" :
                                    stmt is CommentStatement cm ? cm.Text :
                                    stmt is SetStatement s ? $"{s.Target} ({string.Join(", ", s.Attributes)})" :
                                    stmt is SystemFunctionStatement sf ? $"{sf.Name}({string.Join(", ", sf.Parameters)})" :
                                    stmt is UnknownStatement u ? u.OriginalCode :
                                    stmt is MarkerStatement mk ? mk.OriginalCode.TrimEnd(';').ToUpper() :
                                    stmt is MoveAStatement ma ? $"{ma.Target} = {ma.Source} FOR {ma.ForClause}" :
                                    stmt is RetrStatement r ? $"{r.SourceItem} {r.SearchColumn} → {r.TargetItem} ← {r.ReturnColumn}" :
                                    stmt.OriginalCode;

                    label = label.Replace("\"", "'");
                    details = details.Replace("\"", "'");
                    // Replace this line:
                    // sb.AppendLine($"    node{currentId} [label="[{ label}] { details}"];");

                    // With this corrected line:
                    sb.AppendLine($"    node{currentId} [label=\"[{label}] {details}\"];");
                    

                    if (parentId >= 0)
                        sb.AppendLine($"    node{parentId} -> node{currentId};");

                    switch (stmt)
                    {
                        case IfStatement ifs2:
                            nodeStack.Push((currentId, ifs2.ElseStatements));
                            nodeStack.Push((currentId, ifs2.TrueStatements));
                            break;
                        case WhileStatement wh2:
                            nodeStack.Push((currentId, wh2.BodyStatements));
                            break;

                    }
                }
            }

            sb.AppendLine("}");
            File.WriteAllText(outputPath, sb.ToString());
        }
        private static string Collapse(string input, int maxLength = 80)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";

            // Collapse all internal whitespace and newlines to a single space
            string collapsed = Regex.Replace(input, @"\s+", " ").Trim();

            // Truncate long expressions
            if (collapsed.Length > maxLength)
                collapsed = collapsed.Substring(0, maxLength - 3) + "...";

            return collapsed;
        }
        public static void Print(IEnumerable<IStatement> statements, int indent = 0)
        {
            foreach (var stmt in statements)
            {                
                string meta = $"[L:{stmt.LineNumber:D4}][{stmt.NestingLevel:D2}]".PadRight(10);
                string indentPad = new string(' ', indent * 4);
                string output = stmt switch
                {
                    IfStatement ifs2 => $"[IF] {ifs2.Condition}",
                    WhileStatement wh => $"[WHILE] {wh.Condition}",
                    MoveStatement m => $"[MOVE] {m.Destination} = {m.Source}",
                    AssignStatement a => $"[ASSIGN] {a.Left} = {Collapse(a.Right)}",
                    CallStatement c => $"[CALL] {c.ProgramName} USING {string.Join(", ", c.Parameters)}",
                    TestStatement t => $"[TEST] {t.Expression}",
                    DxfrStatement d => $"[DXFR] {d.TargetApp} -> {d.TargetScreen}",
                    CommentStatement cm => $"[COMMENT] {cm.Text}",
                    SetStatement s => $"[SET] {s.Target}" + (s.Attributes.Any() ? $" ({string.Join(", ", s.Attributes)})" : ""),
                    UnknownStatement u => $"[?] {u.OriginalCode}",
                    MarkerStatement mk => $"[MARKER!!!] {mk.OriginalCode.TrimEnd(';').ToUpper()}",
                    ElseStatement els => $"[ELSE] {els.InlineComent}",
                    EndStatement end => $"[END] {end.InlineComent}",
                    SystemFunctionStatement sf => $"[SYSFUNC] {sf.Name}({string.Join(", ", sf.Parameters)})",
                    MoveAStatement ma => $"[MOVEA] {ma.Target} = {ma.Source} FOR {ma.ForClause}",
                    RetrStatement r => $"[RETR] {r.SourceItem} {r.SearchColumn} → {r.TargetItem} ← {r.ReturnColumn}\";",
                    _ => $"[{stmt.Type}] {stmt.OriginalCode}"
                };

                string full = meta + indentPad + output;
                full = full.PadRight(100);

                string original = $"Org: {stmt.OriginalCode}";

                Console.ForegroundColor = stmt.Type switch
                {
                    StatementType.Unknown => ConsoleColor.Red,
                    StatementType.Comment => ConsoleColor.DarkGray,
                    _ => ConsoleColor.Green
                };

                Console.WriteLine($"{full}{original}");
                Console.ResetColor();

                // Handle IF
                if (stmt is IfStatement ifs)
                {
                    Print(ifs.TrueStatements, indent + 1);
                    if (ifs.ElseStatements.Any())
                    {
                        Print(ifs.ElseStatements, indent + 1);
                    }
                }
                // Handle WHILE
                else if (stmt is WhileStatement wh)
                {
                    Print(wh.BodyStatements, indent + 1);
                }
            }
        }
    }
}