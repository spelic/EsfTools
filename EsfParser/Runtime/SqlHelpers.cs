// EsfParser/Runtime/SqlHelpers.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using IBM.Data.Db2;


#region ESF value bridge (stub) ------------------------------------------------
public static class EsfValueProvider
{
    /// <summary>Resolve a host variable value by ESF name (e.g. "LETODN").</summary>
    public static object? Get(string name)
    {
        // TODO: Read from your generated Items/Records/Workstor by symbolic name.
        // Example pseudo:
        // if (GlobalItems.TryGet(name, out var v)) return v;
        // if (GlobalRecords.TryGetField(name, out var v2)) return v2;
        return null;
    }

    /// <summary>Assign a value (e.g. INTO target) back into Items/Records.</summary>
    public static void Set(string name, object? value)
    {
        // TODO: Write to your generated Items/Records by symbolic name.
        // Example pseudo:
        // GlobalItems.Set(name, value);
        // GlobalRecords.SetField(name, value);
    }
}
#endregion

#region Host variable normalizer ----------------------------------------------
public enum SqlParamStyle
{
    NamedAt,     // @NAME
    NamedColon,  // :NAME
    PositionalQ  // ? (DB2 positional)
}

public static class SqlHostVarNormalizer
{
    /// <summary>
    /// Convert ESF-style host markers (?NAME or @NAME) to the chosen ADO style.
    /// Returns the normalized SQL and the ordered list of parameter names.
    /// </summary>
    public static (string sql, List<string> orderedNames) Normalize(string sql, SqlParamStyle style)
    {
        var names = new List<string>();
        var sb = new StringBuilder(sql.Length);
        char? q = null;

        for (int i = 0; i < sql.Length; i++)
        {
            var c = sql[i];

            if (q != null) { sb.Append(c); if (c == q) q = null; continue; }
            if (c == '\'' || c == '"') { q = c; sb.Append(c); continue; }

            if (c == '?' || c == '@')
            {
                int j = i + 1;
                var name = new StringBuilder();
                while (j < sql.Length)
                {
                    var d = sql[j];
                    if (char.IsLetterOrDigit(d) || d == '_' || d == '$' || d == '#') { name.Append(d); j++; }
                    else break;
                }

                if (name.Length > 0)
                {
                    var n = name.ToString();
                    if (!names.Contains(n, StringComparer.OrdinalIgnoreCase))
                        names.Add(n);

                    switch (style)
                    {
                        case SqlParamStyle.NamedAt: sb.Append('@').Append(n); break;
                        case SqlParamStyle.NamedColon: sb.Append(':').Append(n); break;
                        case SqlParamStyle.PositionalQ: sb.Append('?'); break;
                    }

                    i = j - 1;
                    continue;
                }
            }

            sb.Append(c);
        }

        return (sb.ToString(), names);
    }
}
#endregion

#region DataAccess (DB2) -------------------------------------------------------
public static class DataAccess
{
    /// <summary>
    /// Set once at app start, or leave null to use env var DB2_CONN_STR.
    /// Example: "Server=hostname:50001;Database=MYDB;UID=db2inst1;PWD=secret;Pooling=true;"
    /// </summary>
    public static string? ConnectionString { get; private set; }

    /// <summary>Configure the connection string (optional). If omitted, reads env var DB2_CONN_STR.</summary>
    public static void Configure(string? connectionString = null)
    {
        ConnectionString = string.IsNullOrWhiteSpace(connectionString)
            ? Environment.GetEnvironmentVariable("DB2_CONN_STR")
            : connectionString;

        if (string.IsNullOrWhiteSpace(ConnectionString))
            throw new InvalidOperationException(
                "No DB2 connection string configured. Call DataAccess.Configure(...) or set env var DB2_CONN_STR.");
    }


    public static IDbConnection GetConnection()
    {
        if (string.IsNullOrWhiteSpace(ConnectionString))
            Configure(); // try env var

        var conn = new DB2Connection(ConnectionString);
        conn.Open();
        return conn;
    }
}
#endregion
