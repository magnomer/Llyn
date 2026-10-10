using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LLiveryPage(
    LEntryDraft LLiveryPageDraft,
    bool LLiveryPageFavorite,
    int LLiveryPageGrasp,
    string LLiveryPageCreated,
    string LLiveryPageUpdated,
    LAccentSheet LLiveryPageAccent,
    IReadOnlyList<LReflexGuise> LLiveryPageGuise,
    IReadOnlyList<string> LLiveryPageFolded,
    IReadOnlySet<long> LLiveryPageFold,
    IReadOnlyList<LTranscriptionDraft> LLiveryPageTranscription,
    LGlyph? LLiveryPageGlyph,
    IReadOnlyList<LGlyphCell> LLiveryPageCell,
    IReadOnlyList<LFrequency> LLiveryPageFrequency,
    IReadOnlyList<LParadigmRow> LLiveryPageParadigm,
    LParadigmView? LLiveryPageInflection,
    IReadOnlyList<LFanqieGroup> LLiveryPageFanqie,
    IReadOnlyList<LScriptGroup> LLiveryPageScript,
    IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> LLiveryPageTarget,
    IReadOnlyDictionary<long, string> LLiveryPageSource,
    IReadOnlyList<LUsage> LLiveryPageIncoming,
    IReadOnlyList<LTranslationTarget> LLiveryPageEtymon,
    IReadOnlyDictionary<string, string> LLiveryPageBanner,
    IReadOnlyDictionary<string, string> LLiveryPageEnsign);
