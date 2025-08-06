using Xunit;
using System.Collections.Generic;
using System.Linq;

public class EsfPreprocessorTests
{
    [Fact]
    public void MergeMultilineStatements_HandlesSemicolonAndComments()
    {
        var input = new List<string>
        {
            ";", // should be ignored
            "MOVE A TO B; comment after",
            "; orphan comment after",
            "/* standalone comment */",
            "IF A = 1;",
            "MOVE X TO Y;",
            "END;"
        };

        var processed = EsfLogicPreprocessor.Preprocess(input);

        Assert.Equal(6, processed.Count); // 3 statements, 2 comments, 1 orphan comment

        Assert.Contains(processed, p => p.CleanLine.StartsWith("MOVE A TO B"));
        Assert.Contains(processed, p => p.FullLineComment?.Contains("orphan") ?? false);
        Assert.Contains(processed, p => p.FullLineComment?.Contains("standalone") ?? false);
    }

    [Fact]
    public void Preprocessor_ParsesCommentsAndSemicolonsCorrectly()
    {
        var raw = new List<string>
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

        var processed = EsfLogicPreprocessor.Preprocess(raw);

        Assert.Contains(processed, p => p.CleanLine.StartsWith("MOVE 'BO01A' TO BO01W01"));
        Assert.Contains(processed, p => p.InlineComment?.Contains("inq. tab.Zascita") ?? false);
        Assert.Contains(processed, p => p.CleanLine.StartsWith("WHILE"));
        Assert.Equal("STEVAPPL = STEVAPPL + 1;", processed.First(p => p.CleanLine.Contains("STEVAPPL +")).CleanLine);
    }
}
