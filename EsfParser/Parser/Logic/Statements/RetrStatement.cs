// RetrStatement.cs
using System.Text;
using EsfParser.CodeGen;
using EsfParser.Parser.Logic.Statements;

public class RetrStatement : IStatement
{
    public StatementType Type => StatementType.Retr;
    public string OriginalCode { get; set; } = string.Empty;

    // RETR ITEM  INFO.STATE  AMOUNT  AREA;
    public string SourceItem { get; set; } = string.Empty;      // ITEM (can be literal)
    public string TableName { get; set; } = string.Empty;      // INFO
    public string SearchColumn { get; set; } = string.Empty;    // INFO.STATE  or STATE
    public string TargetItem { get; set; } = string.Empty;      // AMOUNT
    public string ReturnColumn { get; set; } = string.Empty;    // AREA  (or INFO.AREA)

    public int LineNumber { get; set; }
    public int NestingLevel { get; set; } = 0;

    public string ToCSharp()
    {
        // Normalize pieces
        string tableId = string.IsNullOrWhiteSpace(TableName)
                           ? GuessTableFrom(SearchColumn)
                           : CSharpUtils.CleanName(TableName);

        string searchCol = ColOnly(SearchColumn);
        string returnCol = ColOnly(ReturnColumn);

        // Fallbacks (only if caller left them empty)
        if (string.IsNullOrWhiteSpace(searchCol))
            searchCol = "COL1";     // TODO: replace with real first column, if needed
        if (string.IsNullOrWhiteSpace(returnCol))
            returnCol = "COL2";     // TODO: replace with real second column, if needed

        string srcExpr = CSharpUtils.ConvertOperand(SourceItem);
        string targetExpr = CSharpUtils.ConvertOperand(TargetItem);

        var ind = CSharpUtils.Indent(NestingLevel);
        var sb = new StringBuilder();

        // Emit code
        sb.AppendLine($"{ind}{{  // RETR  {OriginalCode}".TrimEnd());
        sb.AppendLine($"{ind}    var __src = {srcExpr};");
        sb.AppendLine($"{ind}    int __row = 0;"); // 1-based as per EZETST semantics
        sb.AppendLine($"{ind}    var __tbl = GlobalTables.{tableId};");
        sb.AppendLine($"{ind}    int __cnt = System.Linq.Enumerable.Count(__tbl.{searchCol});");
        sb.AppendLine($"{ind}    for (int __i = 1; __i <= __cnt; __i++)");
        sb.AppendLine($"{ind}    {{");
        sb.AppendLine($"{ind}        if (object.Equals(__tbl.{searchCol}[__i], __src))");
        sb.AppendLine($"{ind}        {{");
        sb.AppendLine($"{ind}            __row = __i;"); // first match, 1-based
        sb.AppendLine($"{ind}            {targetExpr} = __tbl.{returnCol}[__row];");
        sb.AppendLine($"{ind}            break;");
        sb.AppendLine($"{ind}        }}");
        sb.AppendLine($"{ind}    }}");
        sb.AppendLine($"{ind}    EzFunctions.EZETST = __row;");
        sb.AppendLine($"{ind}}}");

        return sb.ToString();
    }

    // Pretty print
    public override string ToString() =>
        $"RetrStatement: {SourceItem} -> {TargetItem} (Table: {TableName}, Search: {SearchColumn}, Return: {ReturnColumn}) (Line: {LineNumber}, Nesting: {NestingLevel})";

    // Helpers --------------------------------------------------------------

    // Accepts "INFO.STATE" or "STATE"; returns cleaned "STATE"
    private static string ColOnly(string col)
    {
        if (string.IsNullOrWhiteSpace(col)) return col;
        int dot = col.IndexOf('.');
        var name = dot >= 0 ? col[(dot + 1)..] : col;
        return CSharpUtils.CleanName(name);
    }

    // If table name is omitted, try to grab it from "INFO.STATE"
    private static string GuessTableFrom(string searchColumn)
    {
        if (string.IsNullOrWhiteSpace(searchColumn)) return string.Empty;
        int dot = searchColumn.IndexOf('.');
        var t = dot > 0 ? searchColumn[..dot] : searchColumn;
        return CSharpUtils.CleanName(t);
    }
}
