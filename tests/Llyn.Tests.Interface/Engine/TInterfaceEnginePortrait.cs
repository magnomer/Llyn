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

    internal static Task<LGalleyRelay?> TEnginePressOpen(
        this LEngine engine, long entryId, LPortraitLabel label) =>
        engine.LEnginePortrait.LEnginePressOpen(entryId, label)!;

    internal static Task<LGalleyRelay?> TEnginePressOpen(
        this LEngine engine, long id, LOwner owner, LPortraitLegend legend) =>
        engine.LEnginePortrait.LEnginePressOpen(id, owner, legend)!;

    internal static LLiveryPage? TLiveryRead(this LEngine engine, long entryId) =>
        engine.LEngineLivery.LEngineLiveryRead(entryId, false);

    internal static LLiveryLanguage TLiveryRead(this LEngine engine, string language) =>
        engine.LEngineLivery.LEngineLiveryRead(language, static key => key);
}
