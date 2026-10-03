using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static class TInterfaceEnginePortrait
{
    internal static Task TEnginePortraitExport(
        this LEngine engine,
        long entryId,
        string path,
        LPortraitMedium format,
        LPortraitLabel label) =>
        engine.LEnginePortrait.LEnginePortraitExport(entryId, path, format, label);

    internal static LPortraitPage TEnginePortraitRead(this LEngine engine, long entryId, LPortraitLabel label) =>
        engine.LEnginePortrait.LEnginePortraitRead(entryId, label);

    internal static LPortraitPage TEnginePortraitRead(
        this LEngine engine, long id, LOwner owner, LPortraitLegend legend) =>
        engine.LEnginePortrait.LEnginePortraitRead(id, owner, legend);

    internal static Task TEnginePortraitPrint(
        this LEngine engine, long entryId, LPortraitLabel label, LPressTicket ticket) =>
        engine.LEnginePortrait.LEnginePortraitPrint(entryId, label, ticket);

    internal static Task TEnginePortraitPrint(
        this LEngine engine, long id, LOwner owner, LPortraitLegend legend, LPressTicket ticket) =>
        engine.LEnginePortrait.LEnginePortraitPrint(id, owner, legend, ticket);
}
