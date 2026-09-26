using System;
using System.Collections.Generic;
using System.Reflection;

namespace Llyn.Tests;

internal static class TEngineFake
{
    internal static TEngineKind TEngineCreate<TEngineKind>(Dictionary<string, Func<object?[]?, object?>> answers)
        where TEngineKind : class
    {
        TEngineKind port = DispatchProxy.Create<TEngineKind, TEngineProxy>();
        ((TEngineProxy)(object)port).TEngineAnswer = answers;
        return port;
    }

    internal static TEngineKind TEngineStubCreate<TEngineKind>() where TEngineKind : class
    {
        return TEngineCreate<TEngineKind>([]);
    }

    public class TEngineProxy : DispatchProxy
    {
        internal Dictionary<string, Func<object?[]?, object?>> TEngineAnswer { get; set; } = [];

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            string name = targetMethod?.Name ?? string.Empty;
            return TEngineAnswer.TryGetValue(name, out Func<object?[]?, object?>? answer)
                ? answer(args)
                : throw new NotSupportedException($"The fake port answers no {name}.");
        }
    }
}
