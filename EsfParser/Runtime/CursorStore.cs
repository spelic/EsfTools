using System;
using System.Collections.Generic;
using System.Data;

public static class CursorStore
{
    private sealed class CursorCtx : IDisposable
    {
        public IDbConnection Conn { get; }
        public IDataReader Reader { get; }
        public List<string> IntoTargets { get; }

        public CursorCtx(IDbConnection conn, IDataReader reader, List<string> intoTargets)
        {
            Conn = conn; Reader = reader; IntoTargets = intoTargets;
        }

        public void Dispose()
        {
            try { Reader?.Dispose(); } catch { }
            try { Conn?.Dispose(); } catch { }
        }
    }

    private static readonly Dictionary<string, CursorCtx> _cursors =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Opens and stores a live DB2 cursor (connection + reader) keyed by function/object name.
    /// </summary>
    public static void Open(string key, string sql, Dapper.DynamicParameters param, List<string> intoTargets)
    {
        Close(key);
        var conn = DataAccess.GetConnection(); // your factory
        var reader = Dapper.SqlMapper.ExecuteReader(conn, sql, param);
        _cursors[key] = new CursorCtx(conn, reader, intoTargets);
    }

    /// <summary>
    /// Fetch next row and assign by ordinal into remembered INTO targets.
    /// </summary>
    public static bool FetchAssign(string key)
    {
        if (!_cursors.TryGetValue(key, out var ctx)) return false;
        if (!ctx.Reader.Read()) return false;

        int cnt = Math.Min(ctx.Reader.FieldCount, ctx.IntoTargets.Count);
        for (int i = 0; i < cnt; i++)
        {
            object? val = ctx.Reader.IsDBNull(i) ? null : ctx.Reader.GetValue(i);
            EsfValueProvider.Set(ctx.IntoTargets[i], val);
        }
        return true;
    }

    public static void Close(string key)
    {
        if (_cursors.TryGetValue(key, out var ctx))
        {
            try { ctx.Dispose(); } catch { }
            _cursors.Remove(key);
        }
    }
}
