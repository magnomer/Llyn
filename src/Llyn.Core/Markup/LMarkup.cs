using System;
using System.Collections.Generic;

namespace Llyn.Core;

public static partial class LMarkup
{
    public static IReadOnlyList<LEntryDraft> LMarkupRead(string text)
    {
        List<LEntryDraft> drafts = new List<LEntryDraft>();

        foreach (LMarkupEntry entry in LMarkupEntryRead(text).LMarkupDocumentEntry)
        {
            drafts.Add(entry.LMarkupEntryDraft);
        }

        return drafts;
    }

    internal static LCardDraft LMarkupCardRead(
        LMarkupCatalog catalog,
        IReadOnlyList<LMarkupToken> tokens,
        int first,
        int last,
        int place,
        bool collocation)
    {
        LMarkupToken? title = null;
        LMarkupToken? expression = null;
        LMarkupToken? meaning = null;
        LMarkupToken? gloss = null;
        LMarkupToken? labels = null;
        List<string> tags = new List<string>();
        List<string> translations = new List<string>();
        List<LImageDraft> images = new List<LImageDraft>();
        List<LVideoDraft> videos = new List<LVideoDraft>();
        List<LSentenceDraft> sentences = new List<LSentenceDraft>();
        List<LSituationDraft> situations = new List<LSituationDraft>();
        List<LRegisterDraft> registers = new List<LRegisterDraft>();
        List<LCardDraft> children = new List<LCardDraft>();

        for (int position = first + 1; position < last; position++)
        {
            LMarkupToken token = tokens[position];

            if (token.LMarkupTokenKind == LMarkupTokenKind.LMarkupTokenEnter)
            {
                int close = LMarkupBlockFind(tokens, position);

                switch (token.LMarkupTokenName)
                {
                    case "sense":
                        if (!collocation)
                        {
                            children.Add(LMarkupCardRead(
                                catalog, tokens, position, close, children.Count + 1, false));
                        }

                        break;
                    case "situation":
                        LMarkupSituationAdd(catalog, situations, token);
                        break;
                    case "register":
                        LMarkupRegisterAdd(catalog, registers, token);
                        break;
                    case "video":
                        LMarkupVideoAdd(catalog, videos, token);
                        break;
                    default:
                        break;
                }

                position = close;
                continue;
            }

            if (token.LMarkupTokenKind != LMarkupTokenKind.LMarkupTokenText)
            {
                continue;
            }

            switch (token.LMarkupTokenName)
            {
                case "title":
                    title = token;
                    break;
                case "expression":
                    expression = token;
                    break;
                case "meaning":
                    meaning = token;
                    break;
                case "gloss":
                    gloss = token;
                    break;
                case "labels":
                    labels = token;
                    break;
                case "tag":
                    LMarkupTagRead(tags, token);
                    break;
                case "translation":
                    LMarkupTranslationRead(translations, token);
                    break;
                case "use":
                    sentences.Add(LMarkupUseRead(catalog, token, place, sentences.Count + 1));
                    break;
                case "image":
                    LMarkupImageAdd(catalog, images, token);
                    break;
                default:
                    break;
            }
        }

        return new LCardDraft(
            LMarkupStateRead(title),
            LMarkupStateRead(expression),
            LMarkupStateRead(meaning),
            sentences,
            situations,
            registers,
            translations,
            string.Empty,
            tags,
            images,
            videos,
            place,
            string.Empty,
            children,
            LMarkupPlainRead(gloss),
            meaning?.LMarkupTokenRead("lang"),
            LMarkupPlainRead(labels) ?? string.Empty);
    }

    private static LSentenceDraft LMarkupUseRead(
        LMarkupCatalog catalog, LMarkupToken token, int card, int place)
    {
        LSentenceDraft written = new LSentenceDraft(
            LMarkupQuoteRead(catalog, token.LMarkupTokenRead("ref")),
            LMarkupStateRead(token.LMarkupTokenRead("par")),
            LMarkupStateRead(token.LMarkupTokenRead("dep")));

        if (written.LSentenceDraftEmpty)
        {
            throw new FormatException(
                $"Use {place} of card {card} carries neither a reference nor a frame.");
        }

        return written;
    }

    private static LExampleDraft? LMarkupQuoteRead(LMarkupCatalog catalog, string? cited)
    {
        if (cited is null)
        {
            return null;
        }

        if (!catalog.LMarkupCatalogExample.TryGetValue(cited, out LExample? held))
        {
            return new LExampleDraft(
                LStateValue.LStateValueUnknown,
                string.Empty,
                LStateValue.LStateValueUnspecified,
                LStateValue.LStateValueUnspecified);
        }

        return new LExampleDraft(
            held.LExampleText,
            cited,
            held.LExampleSource,
            held.LExampleTranslation,
            held.LExampleLanguage);
    }

    private static void LMarkupSituationAdd(
        LMarkupCatalog catalog, List<LSituationDraft> situations, LMarkupToken token)
    {
        if (token.LMarkupTokenRead("ref") is not string cited
            || !catalog.LMarkupCatalogSituation.TryGetValue(cited, out LSituation? held))
        {
            return;
        }

        situations.Add(new LSituationDraft(
            held.LSituationTitle, cited, held.LSituationDescription, held.LSituationKind));
    }

    private static void LMarkupRegisterAdd(
        LMarkupCatalog catalog, List<LRegisterDraft> registers, LMarkupToken token)
    {
        if (token.LMarkupTokenRead("ref") is not string cited
            || !catalog.LMarkupCatalogRegister.TryGetValue(cited, out LRegister? held))
        {
            return;
        }

        registers.Add(new LRegisterDraft(
            held.LRegisterName, cited, held.LRegisterLanguage, held.LRegisterBuiltin));
    }

    private static void LMarkupImageAdd(
        LMarkupCatalog catalog, List<LImageDraft> images, LMarkupToken token)
    {
        if (token.LMarkupTokenRead("ref") is not string cited
            || !catalog.LMarkupCatalogImage.TryGetValue(cited, out LImage? held))
        {
            return;
        }

        images.Add(new LImageDraft(held.LImageLocation, cited));
    }

    private static void LMarkupVideoAdd(
        LMarkupCatalog catalog, List<LVideoDraft> videos, LMarkupToken token)
    {
        if (token.LMarkupTokenRead("ref") is not string cited
            || !catalog.LMarkupCatalogVideo.TryGetValue(cited, out LVideo? held))
        {
            return;
        }

        videos.Add(new LVideoDraft(held.LVideoLocation, held.LVideoSpan, cited));
    }

    private static void LMarkupTagRead(List<string> tags, LMarkupToken token)
    {
        if (token.LMarkupTokenEmpty)
        {
            return;
        }

        string named = token.LMarkupTokenText.Trim();
        if (named.Length == 0 || tags.Contains(named))
        {
            return;
        }

        tags.Add(named);
    }

    private static void LMarkupTranslationRead(List<string> translations, LMarkupToken token)
    {
        if (token.LMarkupTokenRead("entry") is not string cited
            || cited.Length == 0
            || translations.Contains(cited))
        {
            return;
        }

        translations.Add(cited);
    }

    private static IEnumerable<LMarkupToken> LMarkupLeafRead(IReadOnlyList<LMarkupToken> tokens)
    {
        int depth = 0;

        foreach (LMarkupToken token in tokens)
        {
            switch (token.LMarkupTokenKind)
            {
                case LMarkupTokenKind.LMarkupTokenEnter:
                    depth++;
                    break;
                case LMarkupTokenKind.LMarkupTokenLeave:
                    depth--;
                    break;
                default:
                    if (depth == 1)
                    {
                        yield return token;
                    }

                    break;
            }
        }
    }

    private static string? LMarkupPlainRead(LMarkupToken? token)
    {
        if (token is null || token.Value.LMarkupTokenEmpty)
        {
            return null;
        }

        return token.Value.LMarkupTokenText;
    }

    private static LStateValue LMarkupStateRead(LMarkupToken? token)
    {
        if (token is null)
        {
            return LStateValue.LStateValueUnspecified;
        }

        return token.Value.LMarkupTokenEmpty
            ? LStateValue.LStateValueUnknown
            : LStateValue.LStateValueCreate(token.Value.LMarkupTokenText);
    }

    private static LStateValue LMarkupStateRead(string? source)
    {
        if (source is null)
        {
            return LStateValue.LStateValueUnspecified;
        }

        return source.Length == 0
            ? LStateValue.LStateValueUnknown
            : LStateValue.LStateValueCreate(source);
    }
}
