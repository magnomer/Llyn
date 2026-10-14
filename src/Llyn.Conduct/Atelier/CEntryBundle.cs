using System;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

internal sealed class CEntryBundle
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
        LPronunciationPort pronunciations,
        LAncestorPort ancestors)
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
        ArgumentNullException.ThrowIfNull(ancestors);

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
        CEntryBundleAncestor = ancestors;
    }

    internal LEntryPort CEntryBundleEntry { get; }

    internal LGraspPort CEntryBundleGrasp { get; }

    internal LFavoritePort CEntryBundleFavorite { get; }

    internal LVistaPort CEntryBundleVista { get; }

    internal LCardPort CEntryBundleCard { get; }

    internal LTagPort CEntryBundleTag { get; }

    internal LRegisterPort CEntryBundleRegister { get; }

    internal LMentionPort CEntryBundleMention { get; }

    internal LGlyphPort CEntryBundleGlyph { get; }

    internal LSituationPort CEntryBundleSituation { get; }

    internal LExamplePort CEntryBundleExample { get; }

    internal LReferencePort CEntryBundleReference { get; }

    internal LAuthorPort CEntryBundleAuthor { get; }

    internal LMarkdownPort CEntryBundleMarkdown { get; }

    internal LPronunciationPort CEntryBundlePronunciation { get; }

    internal LAncestorPort CEntryBundleAncestor { get; }
}
