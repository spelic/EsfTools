namespace EsfCore.Tags.Logic
{
    public interface ITokenQualifier
    {
        string Qualify(string text);
    }

    public interface IExpressionConverter
    {
        string Convert(string expr);
    }

    public interface IConditionConverter
    {
        string Convert(string condition);
    }

    public interface IStatementTranslator
    {
        bool CanTranslate(string line);
        string Translate(string line, out string? originalComment);
    }

    public interface ILogicProcessor
    {
        string Process(IEnumerable<string> lines, int indentSpaces = 6);
    }
}
