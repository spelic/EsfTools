using System;
using System.Collections.Generic;
using System.Text;
using EsfParser.CodeGen;                     // ConditionBuilder + CSharpUtils
using static EsfParser.CodeGen.CSharpUtils; // Indent(...)

namespace EsfParser.Parser.Logic.Statements
{
    // === Statements ===
    public class IfStatement : IStatement
    {
        public StatementType Type => StatementType.If;
        public string OriginalCode { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public List<IStatement> TrueStatements { get; set; } = new();
        public List<IStatement> ElseStatements { get; set; } = new();
        public int LineNumber { get; set; }
        public int NestingLevel { get; set; } = 0;

        public string ToCSharp()
        {
            // Render starting at the current nesting level.
            return Render(NestingLevel, inlineElseIf: false);
        }

        public override string ToString()
        {
            var trueNumberOfStatemets = TrueStatements.Count;
            var elseNumberOfStatemets = ElseStatements.Count;
            return $"If: {Condition} (Line: {LineNumber}, Nesting: {NestingLevel}) If:{trueNumberOfStatemets} E:{elseNumberOfStatemets}";
        }

        // --- helpers ---

        private string Render(int indentLevel, bool inlineElseIf)
        {
            var sb = new StringBuilder();
            var indent = Indent(indentLevel);
            var childIndentLevel = indentLevel + 1;

            // Translate ESF condition → C# using the new builder (which uses CSharpUtils internally).
            var cond = string.IsNullOrWhiteSpace(Condition)
                ? "true"
                : ConditionBuilder.ToCSharp(Condition);

            // if ( ... )
            if (!inlineElseIf)
                sb.Append(indent);

            sb.Append("if (").Append(cond).AppendLine(")").AppendLine($" // Org: {Condition}");
            sb.Append(indent).AppendLine("{");

            // true block
            AppendStatements(sb, TrueStatements, childIndentLevel);

            sb.Append(indent).Append("}");

            // else / else if
            if (ElseStatements is { Count: > 0 })
            {
                // else if ( ... )  — chain when the else has exactly one nested if
                if (ElseStatements.Count == 1 && ElseStatements[0] is IfStatement elseIf)
                {
                    sb.Append(' ').Append("else ").Append(elseIf.Render(indentLevel, inlineElseIf: true));
                }
                else
                {
                    sb.AppendLine()
                      .Append(indent).AppendLine("else")
                      .Append(indent).AppendLine("{");

                    AppendStatements(sb, ElseStatements, childIndentLevel);

                    sb.Append(indent).Append("}");
                }
            }

            return sb.ToString();
        }

        private void AppendStatements(StringBuilder sb, IEnumerable<IStatement> statements, int indentLevel)
        {
            if (statements == null) return;

            foreach (var st in statements)
            {
                if (st == null) continue;

                // Temporarily bump the child's nesting so its own generator indents correctly.
                var old = st.NestingLevel;
                st.NestingLevel = indentLevel;
                var text = st.ToCSharp();
                st.NestingLevel = old;

                if (!string.IsNullOrEmpty(text))
                {
                    sb.Append(text);
                    // Ensure a trailing newline between statements for tidy output
                    if (!text.EndsWith("\n") && !text.EndsWith("\r"))
                        sb.AppendLine();
                }
            }
        }
    }
}
