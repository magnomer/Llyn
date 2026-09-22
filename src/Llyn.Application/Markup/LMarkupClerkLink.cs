using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LMarkupClerkLink
{
    private readonly LEntryVault _lMarkupLinkEntries;
    private readonly LMeaningVault _lMarkupLinkMeanings;
    private readonly LReferenceClerk _lMarkupLinkReferences;
    private readonly LAuthorClerk _lMarkupLinkAuthors;
    private readonly LTrailClerk _lMarkupLinkTrail;

    public LMarkupClerkLink(LRig rig, LReferenceClerk references, LAuthorClerk authors, LTrailClerk trail)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(references);
        ArgumentNullException.ThrowIfNull(authors);
        ArgumentNullException.ThrowIfNull(trail);
        _lMarkupLinkEntries = rig.LRigEntries;
        _lMarkupLinkMeanings = rig.LRigMeanings;
        _lMarkupLinkReferences = references;
        _lMarkupLinkAuthors = authors;
        _lMarkupLinkTrail = trail;
    }

    public IReadOnlyList<LEntry> LMarkupEntryFind(string headword, string language)
    {
        ArgumentNullException.ThrowIfNull(headword);
        ArgumentNullException.ThrowIfNull(language);
        return _lMarkupLinkEntries.LEntryHeadwordFind(language.Trim(), headword.Trim());
    }

    public LExampleDraft LMarkupExampleResolve(
        LMarkupExample example,
        int line,
        IReadOnlyDictionary<(string, string), long> prepared,
        List<LMarkupOmission> omissions,
        List<(LMarkupMention, int)> held)
    {
        ArgumentNullException.ThrowIfNull(example);
        ArgumentNullException.ThrowIfNull(prepared);
        ArgumentNullException.ThrowIfNull(omissions);
        ArgumentNullException.ThrowIfNull(held);

        int length = LMentionClerk.LMentionRuneRead(example.LMarkupExampleText.LStateValueShow()).Count;
        List<LMentionDraft> mentions = new(example.LMarkupExampleMention.Count);
        foreach (LMarkupMention mention in example.LMarkupExampleMention)
        {
            if (!LMarkupMentionCheck(mention, length, mentions))
            {
                omissions.Add(new LMarkupOmission(0, $"mention at {mention.LMarkupMentionOffset}"));
                continue;
            }

            long target = 0;
            if (mention.LMarkupMentionHeadword.Length > 0)
            {
                target = LMarkupEntryResolve(
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
                ? LStateAnchor.LStateAnchorCreate(LMarkupReferenceResolve(reference))
                : LStateAnchor.LStateAnchorUnspecified,
            example.LMarkupExampleLanguage,
            example.LMarkupExampleGloss,
            mentions);
    }

    public LEtymologyDraft LMarkupEtymologyResolve(
        LMarkupEntry entry,
        IReadOnlyDictionary<(string, string), long> prepared,
        List<LMarkupOmission> omissions)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(prepared);
        ArgumentNullException.ThrowIfNull(omissions);

        List<long> etymons = [];
        foreach (LMarkupEtymon etymon in entry.LMarkupEntryEtymon)
        {
            long target = LMarkupEntryResolve(
                etymon.LMarkupEtymonHeadword, etymon.LMarkupEtymonLanguage, prepared);
            if (target == 0)
            {
                omissions.Add(new LMarkupOmission(0, $"etymon \"{etymon.LMarkupEtymonHeadword}\""));
                continue;
            }

            if (!etymons.Contains(target))
            {
                etymons.Add(target);
            }
        }

        if (entry.LMarkupEntryEtymology is not LMarkupEtymology etymology)
        {
            return new LEtymologyDraft(string.Empty, [], etymons);
        }

        string text = etymology.LMarkupEtymologyText;
        int length = LMentionClerk.LMentionRuneRead(text).Count;
        List<LMentionDraft> mentions = [];
        foreach (LMarkupMention mention in etymology.LMarkupEtymologyMention)
        {
            if (mention.LMarkupMentionHeadword.Length == 0 || !LMarkupMentionCheck(mention, length, mentions))
            {
                omissions.Add(new LMarkupOmission(0, $"mention at {mention.LMarkupMentionOffset}"));
                continue;
            }

            long target = LMarkupEntryResolve(
                mention.LMarkupMentionHeadword, mention.LMarkupMentionLanguage, prepared);
            if (target == 0)
            {
                omissions.Add(new LMarkupOmission(0, $"mention \"{mention.LMarkupMentionHeadword}\""));
                continue;
            }

            mentions.Add(new LMentionDraft(
                0, mention.LMarkupMentionOffset, mention.LMarkupMentionLength, target));
        }

        return new LEtymologyDraft(text, mentions, etymons);
    }

    public long LMarkupEntryResolve(
        string headword, string language, IReadOnlyDictionary<(string, string), long> prepared)
    {
        ArgumentNullException.ThrowIfNull(prepared);

        (string, string) key = (LCatalog.LCatalogTextNormalize(headword), LCatalog.LCatalogTextNormalize(language));
        if (prepared.TryGetValue(key, out long id))
        {
            return id;
        }

        IReadOnlyList<LEntry> found = LMarkupEntryFind(headword, language);
        return found.Count == 1 ? found[0].LEntryId : 0;
    }

    public long LMarkupSenseResolve(long entryId, string sense)
    {
        ArgumentNullException.ThrowIfNull(sense);

        IReadOnlyList<LMeaning> meanings = _lMarkupLinkMeanings.LMeaningRead(entryId);
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

    public bool LMarkupLocationResolve(string location, List<LMarkupOmission> omissions)
    {
        ArgumentNullException.ThrowIfNull(location);
        ArgumentNullException.ThrowIfNull(omissions);

        if (location.Length == 0)
        {
            return true;
        }

        Uri? resolved = _lMarkupLinkTrail.LTrailClerkResolve(location);
        if (resolved is null)
        {
            omissions.Add(new LMarkupOmission(0, $"location \"{location}\""));
            return false;
        }

        if (resolved.IsFile && !_lMarkupLinkTrail.LTrailPathCheck(resolved.LocalPath))
        {
            omissions.Add(new LMarkupOmission(0, $"file \"{location}\""));
        }

        return true;
    }

    private static bool LMarkupMentionCheck(LMarkupMention mention, int length, IReadOnlyList<LMentionDraft> kept)
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

    private long LMarkupReferenceResolve(LMarkupReference reference)
    {
        string title = LCatalog.LCatalogTextNormalize(reference.LMarkupReferenceTitle.LStateValueShow());
        string year = LCatalog.LCatalogTextNormalize(reference.LMarkupReferenceYear.LStateValueShow());
        string url = LCatalog.LCatalogTextNormalize(reference.LMarkupReferenceUrl.LStateValueShow());

        if (title.Length > 0 || url.Length > 0)
        {
            foreach (LReference stored in _lMarkupLinkReferences.LReferenceClerkRead())
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

        LReference created = _lMarkupLinkReferences.LReferenceClerkCreate(new LReference(
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

        _lMarkupLinkAuthors.LAuthorReferenceSave(created.LReferenceId, authors);
        return created.LReferenceId;
    }
}
