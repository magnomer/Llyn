using System;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CEntryBundle
{
    internal CEntryBundle(
        LEntryPort entries,
        LGraspPort grasps,
        LFavoritePort favorites,
        LVistaPort vistas,
        LCardPort cards,
        LTagPort tags,
        LRegisterPort registers,
        LMentionPort mentions,
        LGlyphPort glyphs,
        LSituationPort situations,
        LExamplePort examples,
        LReferencePort references,
        LAuthorPort authors,
        LMarkdownPort markdown,
        LPronunciationPort pronunciations)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(grasps);
        ArgumentNullException.ThrowIfNull(favorites);
        ArgumentNullException.ThrowIfNull(vistas);
        ArgumentNullException.ThrowIfNull(cards);
        ArgumentNullException.ThrowIfNull(tags);
        ArgumentNullException.ThrowIfNull(registers);
        ArgumentNullException.ThrowIfNull(mentions);
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(situations);
        ArgumentNullException.ThrowIfNull(examples);
        ArgumentNullException.ThrowIfNull(references);
        ArgumentNullException.ThrowIfNull(authors);
        ArgumentNullException.ThrowIfNull(markdown);
        ArgumentNullException.ThrowIfNull(pronunciations);

        CEntryBundleEntry = entries;
        CEntryBundleGrasp = grasps;
        CEntryBundleFavorite = favorites;
        CEntryBundleVista = vistas;
        CEntryBundleCard = cards;
        CEntryBundleTag = tags;
        CEntryBundleRegister = registers;
        CEntryBundleMention = mentions;
        CEntryBundleGlyph = glyphs;
        CEntryBundleSituation = situations;
        CEntryBundleExample = examples;
        CEntryBundleReference = references;
        CEntryBundleAuthor = authors;
        CEntryBundleMarkdown = markdown;
        CEntryBundlePronunciation = pronunciations;
    }

    public LEntryPort CEntryBundleEntry { get; }

    public LGraspPort CEntryBundleGrasp { get; }

    public LFavoritePort CEntryBundleFavorite { get; }

    public LVistaPort CEntryBundleVista { get; }

    public LCardPort CEntryBundleCard { get; }

    public LTagPort CEntryBundleTag { get; }

    public LRegisterPort CEntryBundleRegister { get; }

    public LMentionPort CEntryBundleMention { get; }

    public LGlyphPort CEntryBundleGlyph { get; }

    public LSituationPort CEntryBundleSituation { get; }

    public LExamplePort CEntryBundleExample { get; }

    public LReferencePort CEntryBundleReference { get; }

    public LAuthorPort CEntryBundleAuthor { get; }

    public LMarkdownPort CEntryBundleMarkdown { get; }

    public LPronunciationPort CEntryBundlePronunciation { get; }
}
