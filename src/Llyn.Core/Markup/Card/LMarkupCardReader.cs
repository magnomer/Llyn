using System.Collections.Generic;

namespace Llyn.Core;

internal static class LMarkupCardReader
{
    internal static LMarkupCard LMarkupCardParse(LMarkupNode element, bool nested, List<LMarkupOmission> omissions)
    {
        LStateValue title = LStateValue.LStateValueUnspecified;
        LStateValue expression = LStateValue.LStateValueUnspecified;
        LStateValue meaning = LStateValue.LStateValueUnspecified;
        List<LMarkupSentence> sentences = [];
        List<LSituationDraft> situations = [];
        List<LRegisterDraft> registers = [];
        List<LMarkupTranslation> translations = [];
        List<LTagDraft> tags = [];
        List<LImageDraft> images = [];
        List<LVideoDraft> videos = [];
        List<LMarkupCard> children = [];

        foreach (LMarkupNode child in element.LMarkupNodeChild)
        {
            switch (child.LMarkupNodeName)
            {
                case "title":
                    title = LMarkup.LMarkupValueParse(child);
                    break;
                case "expression":
                    expression = LMarkup.LMarkupValueParse(child);
                    break;
                case "definition":
                    meaning = LMarkup.LMarkupValueParse(child);
                    break;
                case "sentence":
                    sentences.Add(LMarkupSentenceParse(child, omissions));
                    break;
                case "situation":
                    situations.Add(LMarkupSituationParse(child, omissions));
                    break;
                case "register":
                    registers.Add(new LRegisterDraft(LMarkup.LMarkupValueParse(child), 0));
                    break;
                case "translation":
                    translations.Add(LMarkupTranslationParse(child, omissions));
                    break;
                case "tag":
                    tags.Add(LTagDraft.LTagDraftCreate(LMarkup.LMarkupTextParse(child)));
                    break;
                case "image":
                    images.Add(LMarkupImageParse(child, omissions));
                    break;
                case "video":
                    videos.Add(LMarkupVideoParse(child, omissions));
                    break;
                case "meaning" when nested:
                    children.Add(LMarkupCardParse(child, true, omissions));
                    break;
                default:
                    LMarkupReader.LMarkupOmissionAdd(omissions, child);
                    break;
            }
        }

        return new LMarkupCard(
            title,
            expression,
            meaning,
            sentences,
            situations,
            registers,
            translations,
            tags,
            images,
            videos,
            children);
    }

    private static LMarkupSentence LMarkupSentenceParse(LMarkupNode element, List<LMarkupOmission> omissions)
    {
        LMarkupExample? example = null;
        LStateValue particle = LStateValue.LStateValueUnspecified;
        LStateValue dependence = LStateValue.LStateValueUnspecified;

        foreach (LMarkupNode child in element.LMarkupNodeChild)
        {
            switch (child.LMarkupNodeName)
            {
                case "particle":
                    particle = LMarkup.LMarkupValueParse(child);
                    break;
                case "dependence":
                    dependence = LMarkup.LMarkupValueParse(child);
                    break;
                case "example":
                    example = LMarkupExampleParse(child, omissions);
                    break;
                default:
                    LMarkupReader.LMarkupOmissionAdd(omissions, child);
                    break;
            }
        }

        return new LMarkupSentence(example, particle, dependence);
    }

    private static LMarkupExample LMarkupExampleParse(LMarkupNode element, List<LMarkupOmission> omissions)
    {
        LStateValue text = LStateValue.LStateValueUnspecified;
        string language = string.Empty;
        List<LGlossDraft> glosses = [];
        List<LMarkupMention> mentions = [];
        LMarkupReference? reference = null;

        foreach (LMarkupNode child in element.LMarkupNodeChild)
        {
            switch (child.LMarkupNodeName)
            {
                case "text":
                    text = LMarkup.LMarkupValueParse(child);
                    break;
                case "language":
                    language = LMarkup.LMarkupTextParse(child);
                    break;
                case "gloss":
                    glosses.Add(LMarkupGlossParse(child, omissions));
                    break;
                case "mention":
                    mentions.Add(LMarkupMentionParse(child, omissions));
                    break;
                case "reference":
                    reference = LMarkupReferenceParse(child, omissions);
                    break;
                default:
                    LMarkupReader.LMarkupOmissionAdd(omissions, child);
                    break;
            }
        }

        return new LMarkupExample(text, language, glosses, mentions, reference);
    }

    private static LGlossDraft LMarkupGlossParse(LMarkupNode element, List<LMarkupOmission> omissions)
    {
        string language = string.Empty;
        LStateValue text = LStateValue.LStateValueUnspecified;

        foreach (LMarkupNode child in element.LMarkupNodeChild)
        {
            switch (child.LMarkupNodeName)
            {
                case "language":
                    language = LMarkup.LMarkupTextParse(child);
                    break;
                case "text":
                    text = LMarkup.LMarkupValueParse(child);
                    break;
                default:
                    LMarkupReader.LMarkupOmissionAdd(omissions, child);
                    break;
            }
        }

        return new LGlossDraft(0, language, text);
    }

    private static LMarkupMention LMarkupMentionParse(LMarkupNode element, List<LMarkupOmission> omissions)
    {
        int offset = -1;
        int length = 0;
        string headword = string.Empty;
        string language = string.Empty;
        string sense = string.Empty;

        foreach (LMarkupNode child in element.LMarkupNodeChild)
        {
            switch (child.LMarkupNodeName)
            {
                case "offset":
                    offset = LMarkupReader.LMarkupNumberParse(child) ?? -1;
                    break;
                case "length":
                    length = LMarkupReader.LMarkupNumberParse(child) ?? 0;
                    break;
                case "headword":
                    headword = LMarkup.LMarkupTextParse(child);
                    break;
                case "language":
                    language = LMarkup.LMarkupTextParse(child);
                    break;
                case "sense":
                    sense = LMarkup.LMarkupTextParse(child);
                    break;
                default:
                    LMarkupReader.LMarkupOmissionAdd(omissions, child);
                    break;
            }
        }

        return new LMarkupMention(offset, length, headword, language, sense);
    }

    private static LMarkupReference LMarkupReferenceParse(LMarkupNode element, List<LMarkupOmission> omissions)
    {
        LStateValue title = LStateValue.LStateValueUnspecified;
        LStateValue year = LStateValue.LStateValueUnspecified;
        LReferenceKind kind = LReferenceKind.LReferenceKindUnspecified;
        LStateValue url = LStateValue.LStateValueUnspecified;
        LStateValue note = LStateValue.LStateValueUnspecified;
        List<string> authors = [];

        foreach (LMarkupNode child in element.LMarkupNodeChild)
        {
            switch (child.LMarkupNodeName)
            {
                case "title":
                    title = LMarkup.LMarkupValueParse(child);
                    break;
                case "year":
                    year = LMarkup.LMarkupValueParse(child);
                    break;
                case "kind":
                    kind = LMarkupKindParse(child);
                    break;
                case "url":
                    url = LMarkup.LMarkupValueParse(child);
                    break;
                case "note":
                    note = LMarkup.LMarkupValueParse(child);
                    break;
                case "author":
                    authors.Add(LMarkup.LMarkupTextParse(child));
                    break;
                default:
                    LMarkupReader.LMarkupOmissionAdd(omissions, child);
                    break;
            }
        }

        return new LMarkupReference(title, year, kind, url, note, authors);
    }

    private static LReferenceKind LMarkupKindParse(LMarkupNode element)
    {
        LStateValue value = LMarkup.LMarkupValueParse(element);
        return value.LStateValueState switch
        {
            LState.LStateUnknown => LReferenceKind.LReferenceKindUnknown,
            LState.LStateSpecified => LReference.LReferenceKindParse(value.LStateValueText?.Trim()),
            _ => LReferenceKind.LReferenceKindUnspecified,
        };
    }

    private static LMarkupTranslation LMarkupTranslationParse(LMarkupNode element, List<LMarkupOmission> omissions)
    {
        string headword = string.Empty;
        string language = string.Empty;

        foreach (LMarkupNode child in element.LMarkupNodeChild)
        {
            switch (child.LMarkupNodeName)
            {
                case "headword":
                    headword = LMarkup.LMarkupTextParse(child);
                    break;
                case "language":
                    language = LMarkup.LMarkupTextParse(child);
                    break;
                default:
                    LMarkupReader.LMarkupOmissionAdd(omissions, child);
                    break;
            }
        }

        return new LMarkupTranslation(headword, language);
    }

    private static LSituationDraft LMarkupSituationParse(LMarkupNode element, List<LMarkupOmission> omissions)
    {
        LStateValue title = LStateValue.LStateValueUnspecified;
        LStateValue description = LStateValue.LStateValueUnspecified;
        LStateValue kind = LStateValue.LStateValueUnspecified;

        foreach (LMarkupNode child in element.LMarkupNodeChild)
        {
            switch (child.LMarkupNodeName)
            {
                case "title":
                    title = LMarkup.LMarkupValueParse(child);
                    break;
                case "description":
                    description = LMarkup.LMarkupValueParse(child);
                    break;
                case "kind":
                    kind = LMarkup.LMarkupValueParse(child);
                    break;
                default:
                    LMarkupReader.LMarkupOmissionAdd(omissions, child);
                    break;
            }
        }

        return new LSituationDraft(title, 0, description, kind);
    }

    private static LImageDraft LMarkupImageParse(LMarkupNode element, List<LMarkupOmission> omissions)
    {
        LStateValue location = LStateValue.LStateValueUnspecified;

        foreach (LMarkupNode child in element.LMarkupNodeChild)
        {
            if (child.LMarkupNodeName == "location")
            {
                location = LMarkup.LMarkupValueParse(child);
            }
            else
            {
                LMarkupReader.LMarkupOmissionAdd(omissions, child);
            }
        }

        return new LImageDraft(location);
    }

    private static LVideoDraft LMarkupVideoParse(LMarkupNode element, List<LMarkupOmission> omissions)
    {
        LStateValue location = LStateValue.LStateValueUnspecified;
        LStateValue span = LStateValue.LStateValueUnspecified;

        foreach (LMarkupNode child in element.LMarkupNodeChild)
        {
            switch (child.LMarkupNodeName)
            {
                case "location":
                    location = LMarkup.LMarkupValueParse(child);
                    break;
                case "span":
                    span = LMarkup.LMarkupValueParse(child);
                    break;
                default:
                    LMarkupReader.LMarkupOmissionAdd(omissions, child);
                    break;
            }
        }

        return new LVideoDraft(location, span);
    }
}
