using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Llyn.Tests;

internal static class TEngineFault
{
    internal static TEngineKind TEngineFaultCreate<TEngineKind>(TEngineKind real, string member, bool thrown)
        where TEngineKind : class
    {
        ArgumentNullException.ThrowIfNull(real);
        ArgumentException.ThrowIfNullOrWhiteSpace(member);

        TEngineKind port = DispatchProxy.Create<TEngineKind, TEngineFaultProxy>();
        TEngineFaultProxy proxy = (TEngineFaultProxy)(object)port;
        proxy.TEngineFaultReal = real;
        proxy.TEngineFaultMember = member;
        proxy.TEngineFaultThrown = thrown;
        return port;
    }

    public class TEngineFaultProxy : DispatchProxy
    {
        internal object? TEngineFaultReal { get; set; }

        internal string TEngineFaultMember { get; set; } = string.Empty;

        internal bool TEngineFaultThrown { get; set; }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            ArgumentNullException.ThrowIfNull(targetMethod);

            if (!string.Equals(
                    targetMethod.DeclaringType?.Name + "." + targetMethod.Name,
                    TEngineFaultMember,
                    StringComparison.Ordinal))
            {
                return targetMethod.Invoke(TEngineFaultReal, BindingFlags.DoNotWrapExceptions, null, args, null);
            }

            InvalidOperationException fault = new($"The fault port faults {TEngineFaultMember}.");
            Type answer = targetMethod.ReturnType;
            if (TEngineFaultThrown || !typeof(Task).IsAssignableFrom(answer))
            {
                throw fault;
            }

            return answer.IsGenericType
                ? typeof(Task)
                    .GetMethod(nameof(Task.FromException), 1, [typeof(Exception)])!
                    .MakeGenericMethod(answer.GetGenericArguments()[0])
                    .Invoke(null, [fault])
                : Task.FromException(fault);
        }
    }
}
