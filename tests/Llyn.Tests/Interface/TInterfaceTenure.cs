using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LTenure TEngineTenureStart(this LEngine engine, string origin, LSubject subject, long? id) =>
        engine.LEngineTenureStart(origin, subject, id);

    internal static void TEngineDelaySet(this LEngine engine, int delay) =>
        engine.LEngineTenureDelay = delay;

    internal static LDraft? TTenureRead(this LTenure tenure) =>
        tenure.LTenureRead();

    internal static LTenureState TTenureStateRead(this LTenure tenure) =>
        tenure.LTenureStateRead();

    internal static void TTenureRequestDefer(this LTenure tenure, LRequest request)
    {
        tenure.LTenureRequestDefer(request);
    }

    internal static void TTenureRequestApply(this LTenure tenure, LRequest request)
    {
        tenure.LTenureRequestApply(request);
    }

    internal static void TTenurePersist(this LTenure tenure)
    {
        tenure.LTenurePersist();
    }

    internal static LDraft? TTenureUndo(this LTenure tenure) =>
        tenure.LTenureUndo();

    internal static LDraft? TTenureRedo(this LTenure tenure) =>
        tenure.LTenureRedo();

    internal static void TTenureSweep(this LTenure tenure)
    {
        tenure.LTenureSweep();
    }

    internal static void TTenureCancel(this LTenure tenure)
    {
        tenure.LTenureCancel();
    }

    internal static long? TTenureFinish(this LTenure tenure, bool store) =>
        tenure.LTenureFinish(store);
}
