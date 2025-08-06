using Xunit;
using EsfParser.Parser.Logic;
using EsfParser.Parser.Logic.Statements;
using System.Collections.Generic;
using System.Linq;

public class VisualAgeParserTests
{

    private static IEnumerable<IStatement> FlattenStatements(IEnumerable<IStatement> root)
    {
        foreach (var stmt in root)
        {
            yield return stmt;

            switch (stmt)
            {
                case IfStatement ifs:
                    foreach (var child in FlattenStatements(ifs.TrueStatements))
                        yield return child;
                    foreach (var child in FlattenStatements(ifs.ElseStatements))
                        yield return child;
                    break;
                case WhileStatement wh:
                    foreach (var child in FlattenStatements(wh.BodyStatements))
                        yield return child;
                    break;
            }
        }
    }

    [Fact]
    public void Parse_BasicMoveAndIfStructure_WorksCorrectly()
    {
        var input = new List<string>
        {
            "MOVE 1 TO A;",
            "IF A > 0;",
            "  MOVE 2 TO B;",
            "ELSE;",
            "  MOVE 3 TO B;",
            "END;"
        };

        var preprocessed = EsfLogicPreprocessor.Preprocess(input);
        var parser = new VisualAgeLogicParser(preprocessed);
        var parsed = parser.Parse();
        
        Assert.Equal(2, parsed.Count); // MOVE + IF

        var move = parsed[0] as MoveStatement;
        Assert.NotNull(move);
        Assert.Equal("1", move.Source);
        Assert.Equal("A", move.Destination);

        var ifStmt = parsed[1] as IfStatement;
        Assert.NotNull(ifStmt);
        Assert.Equal("A > 0", ifStmt.Condition);
        Assert.Single(ifStmt.TrueStatements);
        Assert.Single(ifStmt.ElseStatements);
    }

    [Fact]
    public void LogicParser_ParsesFullFunction_WithNestedBlocks()
    {
        var input = new List<string>
        {
            "MOVE 1 TO EZEFEC;",
            "MOVE 1 TO EZESQISL;",
            "MOVE 'BO01' TO EZESEGTR;",
            "MOVE \"Vnesite izbor   \" TO BO01M01.EZEMSG;",
            "MOVE EZEDTELC TO BO01M01.DTELC;",
            "MOVE 1 TO STEVAPPL;",
            "MOVE 'BO01A' TO BO01W01.APPL[STEVAPPL];",
            "; /* BO01P07();                       /* mapa",
            "DELA1 = 0;",
            "WHILE DELA1 = 0;",
            "BO01P02();                     /* mapa",
            "IF EZEAID IS PF3;",
            "IF STEVAPPL > 1;",
            "STEVAPPL = STEVAPPL - 1;",
            "END;",
            "MOVE 'IMPOA' TO EZEAPP;",
            "DXFR EZEAPP BO01W01;",
            "END;",
            "TEST EZEAID PA2 EZECLOS;",
            "TEST EZEAID PF12 EZECLOS;",
            "IF EZEAID IS ENTER;",
            "MOVE BO01M01.IZBORM TO BO01R01.ZFUNKC;",
            "MOVE BO01W01.ZASIFRA TO BO01R01.ZSIFRA;",
            "MOVE 'BO01A' TO BO01R01.ZAPLIK;",
            "BO01P03();                   /* inq. tab.Zascita",
            "IF BO01R01 IS ERR;",
            "IF BO01R01 IS NRF;",
            "MOVE \"Šifra nima avtorizacije\" TO BO01M01.EZEMSG;",
            "ELSE;",
            "MOVE \"Napaka I/O\" TO BO01M01.EZEMSG;",
            "END;",
            "SET BO01M01.IZBORM CURSOR,MODIFIED;",
            "ELSE;",
            "STEVAPPL = STEVAPPL + 1;",
            "IF BO01M01.IZBORM = 1;",
            "MOVE 'BO02A' TO APPL[STEVAPPL]; /* prijava",
            "END;",
            "IF BO01M01.IZBORM = 2;",
            "MOVE 'BO03A' TO APPL[STEVAPPL]; /* pregled za kupca",
            "END;",
            "IF BO01M01.IZBORM = 3;",
            "MOVE 'BO04A' TO APPL[STEVAPPL]; /* indikator",
            "END;",
            "IF BO01M01.IZBORM = 4;",
            "MOVE 'BO05A' TO APPL[STEVAPPL]; /* pregled",
            "END;",
            "IF BO01M01.IZBORM = 5;",
            "MOVE 'BO07A' TO APPL[STEVAPPL]; /* pregled",
            "END;",
            "MOVE APPL[STEVAPPL] TO EZEAPP;",
            "DXFR EZEAPP BO01W01;",
            "END;", // za err
            "END;", // za sifro
            "END;"  // za while
        };

        var preprocessed = EsfLogicPreprocessor.Preprocess(input);
        var parser = new VisualAgeLogicParser(preprocessed);
        var tree = parser.Parse(); 
        var statements = FlattenStatements(tree).ToList();



        // High-level checks
        Assert.NotEmpty(statements);
        Assert.Contains(statements, s => s is MoveStatement m && m.Destination == "EZEFEC");
        Assert.Contains(statements, s => s is WhileStatement);
        Assert.Contains(statements, s => s is IfStatement);
        var unk = statements.Where(s => s is UnknownStatement).ToList();
      //  Assert.Empty(unk); // No unknown statements should be present
        var topWhile = statements.OfType<WhileStatement>().FirstOrDefault();
        Assert.NotNull(topWhile);
        Assert.Contains(topWhile.BodyStatements, s => s is CallStatement c && c.ProgramName.StartsWith("BO01P02"));

        var nestedIf = topWhile.BodyStatements.OfType<IfStatement>().FirstOrDefault();
        Assert.NotNull(nestedIf);
        Assert.Equal("EZEAID IS PF3", nestedIf.Condition);
        Assert.Contains(nestedIf.TrueStatements, s => s is IfStatement);
    }

    [Fact]
    public void LogicParser_ParsesFullFunction_WithNestedBlock2s()
    {
        var input = new List<string>
        {
            "WHILE DELA1 = 0;",
                "IF EZEAID IS ENTER;",
                    "MOVE BO01M01.IZBORM TO BO01R01.ZFUNKC;",
                    "IF BO01R01 IS ERR;",
                        "IF BO01R01 IS NRF;",
                            "MOVE \"Šifra nima avtorizacije\" TO BO01M01.EZEMSG;",
                        "ELSE;",
                            "MOVE \"Napaka I/O\" TO BO01M01.EZEMSG;",
                        "END;",
                        "SET BO01M01.IZBORM CURSOR,MODIFIED;",
                    "ELSE;",
                        "STEVAPPL = STEVAPPL + 1;",
                        "IF BO01M01.IZBORM = 1;",
                            "MOVE 'BO02A' TO APPL[STEVAPPL]; /* prijava",
                        "END;",
                        "IF BO01M01.IZBORM = 2;",
                            "MOVE 'BO03A' TO APPL[STEVAPPL]; /* pregled za kupca",
                        "END;",
                        "IF BO01M01.IZBORM = 3;",
                            "MOVE 'BO04A' TO APPL[STEVAPPL]; /* indikator",
                        "END;",
                        "IF BO01M01.IZBORM = 4;",
                            "MOVE 'BO05A' TO APPL[STEVAPPL]; /* pregled",
                        "END;",
                        "IF BO01M01.IZBORM = 5;",
                            "MOVE 'BO07A' TO APPL[STEVAPPL]; /* pregled",
                        "END;",
                        "MOVE APPL[STEVAPPL] TO EZEAPP;",
                        "DXFR EZEAPP BO01W01;",
                    "END;", // za err
                "END;", // za sifro
            "END;"  // za while
        };

        var preprocessed = EsfLogicPreprocessor.Preprocess(input);
        var parser = new VisualAgeLogicParser(preprocessed);
        var tree = parser.Parse();
        var statements = FlattenStatements(tree).ToList();



        // High-level checks
        Assert.NotEmpty(statements);
        Assert.Contains(statements, s => s is WhileStatement);
        Assert.Contains(statements, s => s is IfStatement);
        var unk = statements.Where(s => s is UnknownStatement).ToList();
        Assert.Empty(unk); // No unknown statements should be present
    }
}
