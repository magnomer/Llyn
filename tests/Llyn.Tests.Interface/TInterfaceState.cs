using Llyn.Core;

namespace Llyn.Tests;

internal static class TInterfaceState
{
    internal static LStateValue TStateValueCreate(string text) =>
        LStateValue.LStateValueCreate(text);

    internal static LStateValue TStateValueResolve(string? text, bool unknown) =>
        new LStateWritten(text, unknown).LStateWrittenResolve();

    internal static LStateWritten TStateWrittenRead(this LStateValue value) =>
        value is null
            ? LStateWritten.LStateWrittenEmpty
            : new LStateWritten(value.LStateValueShow(), value.LStateValueState == LState.LStateUnknown);

    internal static string TStateValueShow(this LStateValue stateValue) =>
        stateValue.LStateValueShow();

    internal static long TStateAnchorShow(this LStateAnchor stateAnchor) =>
        stateAnchor.LStateAnchorShow();

    internal static LStateAnchor TStateAnchorRead(long? id) =>
        LStateAnchor.LStateAnchorRead(id);
}
