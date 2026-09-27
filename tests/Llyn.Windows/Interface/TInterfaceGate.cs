using Llyn.Conduct;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.Tests;

internal static class TInterfaceGate
{
    internal static void TDeskSituationStart(this LDesk desk, long? id) =>
        desk.LDeskStart("Repertoire", CSubject.CSubjectSituation, id);

    internal static void TQuillSituationChange(this LDesk desk, string title, string description, string kind) =>
        desk.LDeskQuill.QQuillSituationChange(title, description, kind);

    internal static void TEaselImageAdd(this LDesk desk, int position) => desk.LDeskEasel.QEaselImageAdd(0, position);

    internal static void TEaselImageRemove(this LDesk desk, long image) =>
        desk.LDeskEasel.QEaselImageRemove(0, image);

    internal static void TEaselImageSet(this LDesk desk, long image, string location, bool deferred) =>
        desk.LDeskEasel.QEaselImageSet(image, location, deferred);

    internal static void TEaselVideoAdd(this LDesk desk, int position) => desk.LDeskEasel.QEaselVideoAdd(0, position);

    internal static void TEaselVideoRemove(this LDesk desk, long video) =>
        desk.LDeskEasel.QEaselVideoRemove(0, video);

    internal static void TEaselVideoSet(this LDesk desk, long video, string location, bool deferred) =>
        desk.LDeskEasel.QEaselVideoSet(video, location, deferred);

    internal static void TEaselSpanSet(this LDesk desk, long video, string span) =>
        desk.LDeskEasel.QEaselSpanSet(video, span);

    internal static LPortraitLegend TAtlasLegendRead(CPortraitLegend legend) => LAtlas.LAtlasLegendRead(legend);

    internal static CSituationDraft? TAtlasSituationRead(LSituation? situation) =>
        LAtlas.LAtlasSituationRead(situation);
}
