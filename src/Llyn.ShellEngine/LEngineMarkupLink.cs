using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private IReadOnlyList<LEntry> LEngineMarkupFind(string headword, string language)
    {
        return _lEngineEntries.LEntryHeadwordFind(language.Trim(), headword.Trim());
    }

    private LExampleDraft LEngineMarkupResolve(
        LMarkupExample example,
        int line,
        IReadOnlyDictionary<(string, string), long> prepared,
        List<LMarkupOmission> omissions,
        List<(LMarkupMention, int)> held)
    {
        int length = LMentionClerk.LMentionRuneRead(example.LMarkupExampleText.LStateValueShow()).Count;
        List<LMentionDraft> mentions = new(example.LMarkupExampleMention.Count);
        foreach (LMarkupMention mention in example.LMarkupExampleMention)
        {
            if (!LEngineMarkupCheck(mention, length, mentions))
            {
                omissions.Add(new LMarkupOmission(0, $"mention at {mention.LMarkupMentionOffset}"));
                continue;
            }

            long target = 0;
            if (mention.LMarkupMentionHeadword.Length > 0)
            {
                target = LEngineMarkupResolve(
                    mention.LMarkupMentionHeadword, mention.LMarkupMentionLanguage, prepared);
                if (target == 0)
                {
                    omissions.Add(new LMarkupOmission(0, $"mention \"{mention.LMarkupMentionHeadword}\""));
                    continue;
                }
            }

            long draftId = 0;
            if (target != 0 && mention.LMarkupMentionSense.Length > 0)
            {
                held.Add((mention, line));
                draftId = -held.Count;
            }

            mentions.Add(new LMentionDraft(
                draftId, mention.LMarkupMentionOffset, mention.LMarkupMentionLength, target));
        }

        return new LExampleDraft(
            example.LMarkupExampleText,
            0,
            example.LMarkupExampleReference is LMarkupReference reference
                ? LStateAnchor.LStateAnchorCreate(LEngineMarkupResolve(reference))
                : LStateAnchor.LStateAnchorUnspecified,
            example.LMarkupExampleLanguage,
            example.LMarkupExampleGloss,
            mentions);
    }

    private static bool LEngineMarkupCheck(LMarkupMention mention, int length, IReadOnlyList<LMentionDraft> kept)
    {
        long start = mention.LMarkupMentionOffset;
        long end = start + mention.LMarkupMentionLength;
        if (start < 0 || mention.LMarkupMentionLength <= 0 || end > length)
        {
            return false;
        }

        foreach (LMentionDraft other in kept)
        {
            if (start < other.LMentionDraftOffset + (long)other.LMentionDraftLength && other.LMentionDraftOffset < end)
            {
                return false;
            }
        }

        return true;
    }

    private void LEngineMarkupSettle(
        IReadOnlyList<(LMarkupMention, int)> held,
        IReadOnlyDictionary<long, long> identity,
        IReadOnlyDictionary<(string, string), long> prepared,
        List<LMarkupOmission> omissions)
    {
        LMentionVault mentions = _lEngineMentions;
        for (int index = 0; index < held.Count; index++)
        {
            if (!identity.TryGetValue(-(index + 1), out long mentionId))
            {
                continue;
            }

            (LMarkupMention mention, int line) = held[index];
            long target = LEngineMarkupResolve(
                mention.LMarkupMentionHeadword, mention.LMarkupMentionLanguage, prepared);
            long sense = target == 0 ? 0 : LEngineMarkupResolve(target, mention.LMarkupMentionSense);
            if (sense == 0)
            {
                omissions.Add(new LMarkupOmission(
                    line, $"sense \"{mention.LMarkupMentionSense}\" of \"{mention.LMarkupMentionHeadword}\""));
                continue;
            }

            mentions.LMentionSenseSet(mentionId, sense);
        }
    }

    private long LEngineMarkupResolve(
        string headword, string language, IReadOnlyDictionary<(string, string), long> prepared)
    {
        (string, string) key = (LCatalog.LCatalogTextNormalize(headword), LCatalog.LCatalogTextNormalize(language));
        if (prepared.TryGetValue(key, out long id))
        {
            return id;
        }

        IReadOnlyList<LEntry> found = LEngineMarkupFind(headword, language);
        return found.Count == 1 ? found[0].LEntryId : 0;
    }

    private long LEngineMarkupResolve(long entryId, string sense)
    {
        IReadOnlyList<LMeaning> meanings = _lEngineMeanings.LMeaningRead(entryId);
        long? parent = null;
        long current = 0;
        foreach (string step in sense.Split('.'))
        {
            if (!int.TryParse(step, out int position))
            {
                return 0;
            }

            current = 0;
            foreach (LMeaning meaning in meanings)
            {
                if (meaning.LMeaningParentId == parent && meaning.LMeaningPosition + 1 == position)
                {
                    current = meaning.LMeaningId;
                    break;
                }
            }

            if (current == 0)
            {
                return 0;
            }

            parent = current;
        }

        return current;
    }

    private long LEngineMarkupResolve(LMarkupReference reference)
    {
        string title = LCatalog.LCatalogTextNormalize(reference.LMarkupReferenceTitle.LStateValueShow());
        string year = LCatalog.LCatalogTextNormalize(reference.LMarkupReferenceYear.LStateValueShow());
        string url = LCatalog.LCatalogTextNormalize(reference.LMarkupReferenceUrl.LStateValueShow());

        LReferenceVault references = _lEngineReferences;
        if (title.Length > 0 || url.Length > 0)
        {
            foreach (LReference stored in references.LReferenceAllRead())
            {
                bool same = title.Length > 0
                    ? LCatalog.LCatalogTextNormalize(stored.LReferenceTitle.LStateValueShow()) == title
                        && LCatalog.LCatalogTextNormalize(stored.LReferenceYear.LStateValueShow()) == year
                    : LCatalog.LCatalogTextNormalize(stored.LReferenceUrl.LStateValueShow()) == url;
                if (same)
                {
                    return stored.LReferenceId;
                }
            }
        }

        LReference created = references.LReferenceCreate(new LReference(
            0,
            reference.LMarkupReferenceTitle,
            reference.LMarkupReferenceYear,
            reference.LMarkupReferenceKind,
            reference.LMarkupReferenceNote,
            reference.LMarkupReferenceUrl,
            reference.LMarkupReferenceAuthor.Count == 0
                ? LStateMark.LStateMarkUnspecified
                : LStateMark.LStateMarkSpecified));

        List<LAuthor> authors = new(reference.LMarkupReferenceAuthor.Count);
        foreach (string name in reference.LMarkupReferenceAuthor)
        {
            authors.Add(new LAuthor(0, name));
        }

        LEngineCreditSave(created.LReferenceId, authors);
        return created.LReferenceId;
    }

    private bool LEngineMarkupResolve(string location, List<LMarkupOmission> omissions)
    {
        if (location.Length == 0)
        {
            return true;
        }

        Uri? resolved = LEngineLocationResolve(location);
        if (resolved is null)
        {
            omissions.Add(new LMarkupOmission(0, $"location \"{location}\""));
            return false;
        }

        if (resolved.IsFile && !_lEngineUsher.LUsherPathExist(resolved.LocalPath))
        {
            omissions.Add(new LMarkupOmission(0, $"file \"{location}\""));
        }

        return true;
    }
}
