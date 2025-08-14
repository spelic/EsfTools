// DebugFuncTagInjector.cs (updated)
using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using EsfParser.Tags;

public static class DebugFuncTagInjector
{
    /// <summary>Create a FuncTag that contains only BeforeLogic lines.</summary>
    public static FuncTag MakeFuncTag(string funcName, IEnumerable<string> beforeLogicLines)
    {
        if (beforeLogicLines == null) beforeLogicLines = Array.Empty<string>();

        var tag = new FuncTag();

        // Name / cosmetics (set whichever exist)
        SetFirstExistingProp(tag, new[] { ("Name", (object)funcName), ("FunctionName", (object)funcName) });
        SetFirstExistingProp(tag, new[] { ("GrpName", (object)"DEBUG"), ("Group", (object)"DEBUG") });
        SetFirstExistingProp(tag, new[] { ("Description",(object)"Auto-inserted DEBUG function"),
                                          ("Comment",(object)"Auto-inserted DEBUG function") });

        // Clear other blocks if present
        TryClearList(tag, "MainLogic");
        TryClearList(tag, "AfterLogic");
        TryClearList(tag, "Logic");
        TryClearList(tag, "Body");

        // Populate BeforeLogic with lines
        if (!TrySetStringList(tag, "BeforeLogic", beforeLogicLines))
            if (!TrySetStringList(tag, "Before", beforeLogicLines) &&
                !TrySetStringList(tag, "Lines", beforeLogicLines))
                TrySetIList(tag, "BeforeLogic", beforeLogicLines);

        return tag;
    }

    /// <summary>Insert at index 0 (non-generic overload, fully qualified).</summary>
    public static void InsertDebugFuncAtTop(System.Collections.IList nodes, IEnumerable<string> beforeLogicLines, string funcName = "__DEBUG_ONLY__")
    {
        if (nodes == null) throw new ArgumentNullException(nameof(nodes));
        var funcTag = MakeFuncTag(funcName, beforeLogicLines);
        nodes.Insert(0, funcTag);
    }

   

    // ── helpers ─────────────────────────────────────────────────────────

    private static bool TrySetStringList(object obj, string listPropName, IEnumerable<string> lines)
    {
        var p = obj.GetType().GetProperty(listPropName, BindingFlags.Public | BindingFlags.Instance);
        if (p == null) return false;

        if (typeof(IEnumerable<string>).IsAssignableFrom(p.PropertyType))
        {
            p.SetValue(obj, lines.ToList());
            return true;
        }

        if (typeof(IList).IsAssignableFrom(p.PropertyType))
        {
            var list = p.GetValue(obj) as IList ?? (IList)Activator.CreateInstance(p.PropertyType)!;
            foreach (var s in lines) list.Add(s);
            p.SetValue(obj, list);
            return true;
        }

        return false;
    }

    private static bool TrySetIList(object obj, string listPropName, IEnumerable<string> lines)
    {
        var p = obj.GetType().GetProperty(listPropName, BindingFlags.Public | BindingFlags.Instance);
        if (p == null || !typeof(IList).IsAssignableFrom(p.PropertyType)) return false;

        var list = p.GetValue(obj) as IList ?? (IList)Activator.CreateInstance(p.PropertyType)!;
        foreach (var s in lines) list.Add(s);
        p.SetValue(obj, list);
        return true;
    }

    private static void TryClearList(object obj, string listPropName)
    {
        var p = obj.GetType().GetProperty(listPropName, BindingFlags.Public | BindingFlags.Instance);
        if (p == null) return;

        var val = p.GetValue(obj);
        if (val is IList il) il.Clear();
        else if (val != null && val.GetType().IsGenericType)
            val.GetType().GetMethod("Clear", BindingFlags.Public | BindingFlags.Instance)?.Invoke(val, null);
    }

    private static void SetFirstExistingProp(object obj, IEnumerable<(string name, object value)> candidates)
    {
        foreach (var (name, value) in candidates)
        {
            var p = obj.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (p != null && p.CanWrite) { p.SetValue(obj, value); break; }
        }
    }
}
