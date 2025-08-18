// SystemFunctionStatement.cs ─────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using EsfParser.CodeGen;

namespace EsfParser.Parser.Logic.Statements
{
    public class SystemFunctionStatement : IStatement
    {
        public StatementType Type => StatementType.SystemFunction;

        public string OriginalCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<string> Parameters { get; set; } = new();

        public int LineNumber { get; set; }
        public int NestingLevel { get; set; } = 0;

        public string ToCSharp()
        {
            var t = typeof(EzFunctions);

            // Build reflection cache once (per AppDomain)
            EnsureCache(t);

            // Normalize + candidate names (case-insensitive)
            var raw = (Name ?? string.Empty).Trim();
            var up = raw.ToUpperInvariant();
            var candidates = new List<string>();

            // Try: raw, CleanName(raw), UPPER, CleanName(UPPER)
            candidates.Add(raw);
            candidates.Add(CSharpUtils.CleanName(raw));
            candidates.Add(up);
            candidates.Add(CSharpUtils.CleanName(up));

            // If it starts with EY, also try EZE + rest (compatibility)
            if (up.StartsWith("EY"))
            {
                var ezAlt = "EZE" + up.Substring(2);
                candidates.Add(ezAlt);
                candidates.Add(CSharpUtils.CleanName(ezAlt));
            }

            // Convert arguments using your operand converter
            var args = Parameters.Select(p => CSharpUtils.ConvertOperand(p)).ToArray();

            // 1) Nested static type with Execute(...) (e.g., EZECOMIT.Execute())
            foreach (var cand in candidates)
            {
                if (_nestedExec.TryGetValue(cand, out var execMethod))
                {
                    var call = BuildMethodCall("EzFunctions." + execMethod.DeclaringType!.Name + ".Execute", execMethod, args);
                    return call + $"  // Org: {OriginalCode}";
                }
            }

            // 2) Static method on EzFunctions (e.g., ExternalCallProgram(...))
            foreach (var cand in candidates)
            {
                if (_methods.TryGetValue(cand, out var overloads))
                {
                    var method = PickBestOverload(overloads, args.Length);
                    if (method != null)
                    {
                        var call = BuildMethodCall("EzFunctions." + method.Name, method, args);
                        return call + $"  // Org: {OriginalCode}";
                    }
                }
            }

            // 3) Static property (single-arg → setter)
            foreach (var cand in candidates)
            {
                if (_props.TryGetValue(cand, out var prop))
                {
                    // If exactly one parameter → treat as setter: EzFunctions.Prop = arg;
                    if (args.Length == 1 && prop.CanWrite)
                    {
                        var rhs = CoerceForProperty(prop.PropertyType, args[0]);
                        return $"EzFunctions.{prop.Name} = {rhs};  // Org: {OriginalCode}";
                    }
                    // If zero parameters and has getter: call getter but ignore (no-op)
                    if (args.Length == 0 && prop.CanRead)
                    {
                        return $"_ = EzFunctions.{prop.Name};  // Org: {OriginalCode}";
                    }
                    // Otherwise: unhandled arity for property
                    return $"// SystemFunction (property arity mismatch): {OriginalCode}";
                }
            }

            // 4) Static field (single-arg → assignment)
            foreach (var cand in candidates)
            {
                if (_fields.TryGetValue(cand, out var field))
                {
                    if (args.Length == 1)
                    {
                        var rhs = CoerceForField(field.FieldType, args[0]);
                        return $"EzFunctions.{field.Name} = {rhs};  // Org: {OriginalCode}";
                    }
                    if (args.Length == 0)
                    {
                        return $"_ = EzFunctions.{field.Name};  // Org: {OriginalCode}";
                    }
                    return $"// SystemFunction (field arity mismatch): {OriginalCode}";
                }
            }

            // Fallback: not found
            return $"// SystemFunction: unknown '{Name}' → {OriginalCode}";
        }

        public override string ToString()
            => $"SystemFunctionStatement: {Name}({string.Join(", ", Parameters)}) (Line: {LineNumber}, Nesting: {NestingLevel})";

        // ───────────────────────────
        // Reflection cache
        // ───────────────────────────
        private static bool _cacheReady = false;
        private static Dictionary<string, PropertyInfo> _props = default!;
        private static Dictionary<string, FieldInfo> _fields = default!;
        private static Dictionary<string, List<MethodInfo>> _methods = default!;
        private static Dictionary<string, MethodInfo> _nestedExec = default!;

        private static void EnsureCache(Type t)
        {
            if (_cacheReady) return;

            var cmp = StringComparer.OrdinalIgnoreCase;
            _props = t.GetProperties(BindingFlags.Public | BindingFlags.Static)
                      .ToDictionary(p => p.Name, p => p, cmp);

            _fields = t.GetFields(BindingFlags.Public | BindingFlags.Static)
                       .ToDictionary(f => f.Name, f => f, cmp);

            _methods = t.GetMethods(BindingFlags.Public | BindingFlags.Static)
                        .Where(m => !m.IsSpecialName) // skip get_/set_
                        .GroupBy(m => m.Name, cmp)
                        .ToDictionary(g => g.Key, g => g.ToList(), cmp);

            _nestedExec = new Dictionary<string, MethodInfo>(cmp);
            foreach (var nt in t.GetNestedTypes(BindingFlags.Public))
            {
                var exec = nt.GetMethod("Execute", BindingFlags.Public | BindingFlags.Static);
                if (exec != null)
                    _nestedExec[nt.Name] = exec;
            }

            _cacheReady = true;
        }

        // ───────────────────────────
        // Emission helpers
        // ───────────────────────────
        private static MethodInfo? PickBestOverload(List<MethodInfo> overloads, int argCount)
        {
            // 1) Exact arity match
            var exact = overloads.FirstOrDefault(m =>
            {
                var ps = m.GetParameters();
                return ps.Length == argCount && !ps.Any(p => p.IsOut);
            });
            if (exact != null) return exact;

            // 2) Supports params array (ParamArray)
            foreach (var m in overloads)
            {
                var ps = m.GetParameters();
                if (ps.Length == 0) continue;
                var last = ps[^1];
                var hasParams = last.GetCustomAttribute<ParamArrayAttribute>() != null;
                if (!hasParams) continue;
                // If params[] exists, allow any argCount >= ps.Length-1
                if (argCount >= ps.Length - 1)
                    return m;
            }

            // 3) Fallback: any
            return overloads.FirstOrDefault();
        }

        private static string BuildMethodCall(string qualifiedName, MethodInfo method, string[] args)
        {
            var ps = method.GetParameters();
            if (ps.Length == 0)
                return qualifiedName + "();";

            // If method uses params T[] on last parameter, pack the tail
            var last = ps[^1];
            var isParams = last.GetCustomAttribute<ParamArrayAttribute>() != null;

            if (isParams)
            {
                // Head args are the fixed ones; the rest go into a new array
                int fixedCount = ps.Length - 1;
                var head = args.Take(Math.Min(fixedCount, args.Length)).ToArray();
                var tail = args.Skip(fixedCount).ToArray();

                string arrayCtor;
                var elemType = last.ParameterType.GetElementType() ?? typeof(object);
                var elemTypeName = elemType.FullName == "System.String" ? "string" :
                                   elemType.FullName == "System.Object" ? "object" :
                                   elemType.Name;

                if (tail.Length == 0)
                {
                    arrayCtor = $"new {elemTypeName}[0]";
                }
                else
                {
                    arrayCtor = $"new {elemTypeName}[] {{ {string.Join(", ", tail)} }}";
                }

                var finalArgs = head.Concat(new[] { arrayCtor });
                return $"{qualifiedName}({string.Join(", ", finalArgs)});";
            }

            // Plain call
            return $"{qualifiedName}({string.Join(", ", args)});";
        }

        private static string CoerceForProperty(Type targetType, string rhs)
        {
            // Keep it simple and inline casts for common types we have in EzFunctions
            if (targetType == typeof(int))
                return $"(int)({WrapIfLiteral(rhs, isString: false)})";
            if (targetType == typeof(decimal))
                return $"(decimal)({WrapIfLiteral(rhs, isString: false)})";
            if (targetType == typeof(string))
                return WrapIfLiteral(rhs, isString: true);
            if (targetType == typeof(DateTime))
                return rhs; // assume caller passed a DateTime expr

            // Fallback: as-is
            return rhs;
        }

        private static string CoerceForField(Type targetType, string rhs) => CoerceForProperty(targetType, rhs);

        private static string WrapIfLiteral(string expr, bool isString)
        {
            var t = expr.Trim();
            if (isString)
            {
                // If already a string literal "..." just return it
                if (t.Length >= 2 && t[0] == '"' && t[^1] == '"') return expr;
                // Otherwise pass through; ConvertOperand should have produced a proper string expr
                return expr;
            }
            else
            {
                // Numeric literal? Leave as-is
                if (Regex.IsMatch(t, @"^[+-]?(\d+(\.\d*)?|\.\d+)$")) return expr;
                return expr;
            }
        }
    }
}
