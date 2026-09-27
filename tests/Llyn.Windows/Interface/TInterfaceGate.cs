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

    internal static void TQuillAuthorSet(this LDesk desk, string name) => desk.LDeskQuill.QQuillAuthorSet(name);

    internal static void TDeskExampleStart(this LDesk desk, long? id) =>
        desk.LDeskStart("Corpus", CSubject.CSubjectExample, id);

    internal static void TQuillExampleChange(this LDesk desk, CExampleField field, string value) =>
        desk.LDeskQuill.QQuillExampleChange(field, value);

    internal static void TQuillCitationSet(this LDesk desk, long reference) =>
        desk.LDeskQuill.QQuillCitationSet(reference);

    internal static void TQuillGlossAdd(this LDesk desk, string language, int position) =>
        desk.LDeskQuill.QQuillGlossAdd(0, 0, language, position);

    internal static void TQuillGlossRemove(this LDesk desk, long gloss) =>
        desk.LDeskQuill.QQuillGlossRemove(0, 0, gloss);

    internal static void TQuillGlossChange(this LDesk desk, long gloss, CGlossField field, string value) =>
        desk.LDeskQuill.QQuillGlossChange(0, 0, gloss, field, value);

    internal static void TQuillMentionAdd(this LDesk desk, int offset, int length, long entry) =>
        desk.LDeskQuill.QQuillMentionAdd(0, 0, offset, length, entry);

    internal static void TQuillMentionRemove(this LDesk desk, long mention) =>
        desk.LDeskQuill.QQuillMentionRemove(0, 0, mention);

    internal static void TQuillMentionChange(this LDesk desk, long mention, long sense) =>
        desk.LDeskQuill.QQuillMentionChange(0, 0, mention, sense);

    internal static CExample? TAnthologyExampleRead(LExample? example) => LAnthology.LAnthologyExampleRead(example);

    internal static LExample TExampleMentionAdd(this LExample example, LMention mention) =>
        example with { LExampleMention = [mention] };

    internal static CMentionResult TAnthologyMentionRead(int offset, LMention? stored) =>
        LAnthology.LAnthologyMentionRead(new LMentionResult(offset, stored, []));

    internal static LPortraitLegend TAtlasLegendRead(CPortraitLegend legend) => LAtlas.LAtlasLegendRead(legend);

    internal static CSituationDraft? TAtlasSituationRead(LSituation? situation) =>
        LAtlas.LAtlasSituationRead(situation);
}
