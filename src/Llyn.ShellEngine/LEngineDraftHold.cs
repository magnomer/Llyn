using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private readonly HashSet<string> _lEngineDraftHeld = new(StringComparer.Ordinal);

    public LDraft LEngineDraftStart(string origin, string? entryId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(origin);

        string entry = string.IsNullOrWhiteSpace(entryId) ? string.Empty : entryId;
        LEntryDraft content = entry.Length == 0
            ? new LEntryDraft(string.Empty, string.Empty, string.Empty, string.Empty, [], [])
            : LEngineEntryLoad(entry) ?? throw new LRefusal(LRefusal.LRefusalEntry);

        LDraft draft = new(
            LIdentity.LIdentityCreate(),
            origin,
            entry,
            content,
            DateTimeOffset.UtcNow);

        LDraftArchive.LDraftArchiveSave(_lEngineWorkspace, draft);
        _lEngineDraftHeld.Add(draft.LDraftId);
        return draft;
    }

    public void LEngineDraftSave(LDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);
        LDraftArchive.LDraftArchiveSave(_lEngineWorkspace, draft);
    }

    public LDraft? LEngineDraftRead(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        return LDraftArchive.LDraftArchiveRead(_lEngineWorkspace, id);
    }

    public IReadOnlyList<LDraft> LEngineDraftScan()
    {
        return LDraftArchive.LDraftArchiveScan(_lEngineWorkspace);
    }

    public IReadOnlyList<LDraft> LEngineLeftoverRead()
    {
        List<LDraft> leftovers = [];
        foreach (LDraft draft in LDraftArchive.LDraftArchiveScan(_lEngineWorkspace))
        {
            if (_lEngineDraftHeld.Contains(draft.LDraftId) || !LEngineDraftCheck(draft.LDraftId))
            {
                continue;
            }

            leftovers.Add(draft);
        }

        return leftovers;
    }

    public void LEngineDraftDelete(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        _lEngineDraftHeld.Remove(id);
        LDraftArchive.LDraftArchiveDelete(_lEngineWorkspace, id);
    }

    public IReadOnlyList<LCardDraft> LEngineDraftMove(string id, bool collocation, int from, int target)
    {
        LDraft draft = LEngineDraftLoad(id);
        List<LCardDraft> cards = new(
            collocation
                ? draft.LDraftContent.LEntryDraftCollocations
                : draft.LDraftContent.LEntryDraftSenses);

        if (cards.Count != 0)
        {
            int origin = Math.Clamp(from, 0, cards.Count - 1);
            int landing = Math.Clamp(target, 0, cards.Count - 1);

            LCardDraft moved = cards[origin];
            cards.RemoveAt(origin);
            cards.Insert(landing, moved);

            for (int index = 0; index < cards.Count; index++)
            {
                cards[index] = cards[index] with { LCardDraftPosition = index + 1 };
            }
        }

        LEntryDraft content = collocation
            ? draft.LDraftContent with { LEntryDraftCollocations = cards }
            : draft.LDraftContent with { LEntryDraftSenses = cards };

        LDraftArchive.LDraftArchiveSave(_lEngineWorkspace, draft with { LDraftContent = content });
        return cards;
    }

    public LCourtLink LEngineCourtSave(
        string ownerId, string targetId, string headword, string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ownerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetId);
        ArgumentException.ThrowIfNullOrWhiteSpace(headword);

        LCourtLink link = new(
            LIdentity.LIdentityCreate(),
            ownerId,
            targetId,
            headword,
            language ?? string.Empty);

        LCourtArchive.LCourtArchiveSave(_lEngineWorkspace, link);
        return link;
    }

    public void LEngineCourtDelete(string linkId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(linkId);
        LCourtArchive.LCourtArchiveDelete(_lEngineWorkspace, linkId);
    }

    public LCourtLink? LEngineCourtFind(string ownerId, string targetId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ownerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetId);

        foreach (LCourtLink link in LEngineCourtScan(ownerId))
        {
            if (string.Equals(link.LCourtLinkTarget, targetId, StringComparison.Ordinal))
            {
                return link;
            }
        }

        return null;
    }

    public bool LEngineDraftCheck(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return false;
        }

        LDraft? draft = LDraftArchive.LDraftArchiveRead(_lEngineWorkspace, id);
        if (draft is null)
        {
            return false;
        }

        LEntryDraft origin = string.IsNullOrWhiteSpace(draft.LDraftEntry)
            ? LEngineDraftBlank
            : LEngineEntryLoad(draft.LDraftEntry) ?? LEngineDraftBlank;

        return !LEngineDraftMatch(origin, draft.LDraftContent);
    }

    public LEntry LEngineDraftCommit(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        return LEngineDraftCommit(id, []);
    }

    private LEntry LEngineDraftCommit(string id, HashSet<string> entered)
    {
        entered.Add(id);

        foreach (LCourtLink link in LEngineCourtScan(id))
        {
            if (entered.Contains(link.LCourtLinkTarget) ||
                LDraftArchive.LDraftArchiveRead(_lEngineWorkspace, link.LCourtLinkTarget) is null)
            {
                continue;
            }

            LEngineDraftCommit(link.LCourtLinkTarget, entered);
        }

        LDraft draft = LEngineDraftLoad(id);

        LEntry entry = string.IsNullOrWhiteSpace(draft.LDraftEntry)
            ? LEngineEntrySave(draft.LDraftContent)
            : LEngineEntryUpdate(draft.LDraftEntry, draft.LDraftContent);

        LDraftArchive.LDraftArchiveSave(_lEngineWorkspace, draft with { LDraftEntry = entry.LEntryId });

        foreach (LCourtLink link in
            LCourtArchive.LCourtArchiveResolve(_lEngineWorkspace, id, entry.LEntryId))
        {
            LEngineCourtUpdate(link, entry.LEntryId);
        }

        _lEngineDraftHeld.Remove(id);
        LDraftArchive.LDraftArchiveDelete(_lEngineWorkspace, id);
        return entry;
    }

    public void LEngineDraftCancel(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        LDraft? cancelled = LDraftArchive.LDraftArchiveRead(_lEngineWorkspace, id);
        IReadOnlyList<LCourtLink> court = LCourtArchive.LCourtArchiveScan(_lEngineWorkspace);

        foreach (LCourtLink link in court)
        {
            if (!string.Equals(link.LCourtLinkOwner, id, StringComparison.Ordinal))
            {
                continue;
            }

            LCourtArchive.LCourtArchiveDelete(_lEngineWorkspace, link.LCourtLinkId);

            if (string.Equals(link.LCourtLinkTarget, id, StringComparison.Ordinal))
            {
                continue;
            }

            bool claimed = false;
            foreach (LCourtLink other in court)
            {
                if (!string.Equals(other.LCourtLinkOwner, id, StringComparison.Ordinal)
                    && string.Equals(
                        other.LCourtLinkTarget, link.LCourtLinkTarget, StringComparison.Ordinal))
                {
                    claimed = true;
                    break;
                }
            }

            LDraft? target = LDraftArchive.LDraftArchiveRead(_lEngineWorkspace, link.LCourtLinkTarget);

            if (claimed
                || target is null
                || cancelled is null
                || !string.Equals(target.LDraftOrigin, cancelled.LDraftOrigin, StringComparison.Ordinal))
            {
                continue;
            }

            LDraftArchive.LDraftArchiveDelete(_lEngineWorkspace, link.LCourtLinkTarget);
        }

        LCourtArchive.LCourtArchiveCancel(_lEngineWorkspace, id);
        _lEngineDraftHeld.Remove(id);
        LDraftArchive.LDraftArchiveDelete(_lEngineWorkspace, id);
    }

    private static LEntryDraft LEngineDraftBlank =>
        new(string.Empty, string.Empty, string.Empty, string.Empty, [], []);

    private IReadOnlyList<LCourtLink> LEngineCourtScan(string ownerId)
    {
        List<LCourtLink> owned = [];
        foreach (LCourtLink link in LCourtArchive.LCourtArchiveScan(_lEngineWorkspace))
        {
            if (string.Equals(link.LCourtLinkOwner, ownerId, StringComparison.Ordinal))
            {
                owned.Add(link);
            }
        }

        return owned;
    }

    private static bool LEngineDraftMatch(LEntryDraft one, LEntryDraft other)
    {
        return string.Equals(one.LEntryDraftHeadword, other.LEntryDraftHeadword, StringComparison.Ordinal)
            && string.Equals(one.LEntryDraftLanguage, other.LEntryDraftLanguage, StringComparison.Ordinal)
            && string.Equals(
                one.LEntryDraftPronunciation, other.LEntryDraftPronunciation, StringComparison.Ordinal)
            && string.Equals(one.LEntryDraftNote, other.LEntryDraftNote, StringComparison.Ordinal)
            && LEngineTextMatch(one.LEntryDraftSpeeches ?? [], other.LEntryDraftSpeeches ?? [])
            && string.Equals(one.LEntryDraftAudio, other.LEntryDraftAudio, StringComparison.Ordinal)
            && string.Equals(one.LEntryDraftSource, other.LEntryDraftSource, StringComparison.Ordinal)
            && LEngineCardMatch(one.LEntryDraftSenses, other.LEntryDraftSenses)
            && LEngineCardMatch(one.LEntryDraftCollocations, other.LEntryDraftCollocations);
    }

    private static bool LEngineCardMatch(IReadOnlyList<LCardDraft> one, IReadOnlyList<LCardDraft> other)
    {
        IReadOnlyList<LCardDraft> first = LEngineCardScan(one);
        IReadOnlyList<LCardDraft> second = LEngineCardScan(other);

        if (first.Count != second.Count)
        {
            return false;
        }

        for (int index = 0; index < first.Count; index++)
        {
            LCardDraft written = first[index];
            LCardDraft held = second[index];
            if (written.LCardDraftPosition != held.LCardDraftPosition
                || written.LCardDraftTitle != held.LCardDraftTitle
                || written.LCardDraftExpression != held.LCardDraftExpression
                || written.LCardDraftMeaning != held.LCardDraftMeaning
                || !string.Equals(written.LCardDraftId, held.LCardDraftId, StringComparison.Ordinal)
                || !LEngineExampleMatch(written.LCardDraftExample, held.LCardDraftExample)
                || !LEngineSituationMatch(written.LCardDraftSituation, held.LCardDraftSituation)
                || !LEngineTextMatch(written.LCardDraftTranslation, held.LCardDraftTranslation)
                || !LEngineTextMatch(written.LCardDraftTag, held.LCardDraftTag)
                || !LEngineValueMatch(written.LCardDraftImage, held.LCardDraftImage))
            {
                return false;
            }
        }

        return true;
    }

    private static IReadOnlyList<LCardDraft> LEngineCardScan(IReadOnlyList<LCardDraft> cards)
    {
        List<LCardDraft> filled = [];
        foreach (LCardDraft card in cards)
        {
            if (!LEngineCardCheck(card))
            {
                filled.Add(card);
            }
        }

        return filled;
    }

    private static bool LEngineCardCheck(LCardDraft card)
    {
        return card.LCardDraftTitle.LStateValueEmpty
            && card.LCardDraftExpression.LStateValueEmpty
            && card.LCardDraftMeaning.LStateValueEmpty
            && card.LCardDraftExample.Count == 0
            && card.LCardDraftSituation.Count == 0
            && card.LCardDraftTranslation.Count == 0
            && card.LCardDraftTag.Count == 0
            && card.LCardDraftImage.Count == 0;
    }

    private static bool LEngineExampleMatch(
        IReadOnlyList<LExampleDraft> one, IReadOnlyList<LExampleDraft> other)
    {
        if (one.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < one.Count; index++)
        {
            if (one[index].LExampleDraftText != other[index].LExampleDraftText
                || !string.Equals(
                    one[index].LExampleDraftId, other[index].LExampleDraftId, StringComparison.Ordinal)
                || one[index].LExampleDraftReference != other[index].LExampleDraftReference)
            {
                return false;
            }
        }

        return true;
    }

    private static bool LEngineSituationMatch(
        IReadOnlyList<LSituationDraft> one, IReadOnlyList<LSituationDraft> other)
    {
        if (one.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < one.Count; index++)
        {
            if (one[index].LSituationDraftText != other[index].LSituationDraftText
                || !string.Equals(
                    one[index].LSituationDraftId, other[index].LSituationDraftId, StringComparison.Ordinal)
                || one[index].LSituationDraftReference != other[index].LSituationDraftReference)
            {
                return false;
            }
        }

        return true;
    }

    private static bool LEngineValueMatch(IReadOnlyList<LStateValue> one, IReadOnlyList<LStateValue> other)
    {
        if (one.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < one.Count; index++)
        {
            if (one[index] != other[index])
            {
                return false;
            }
        }

        return true;
    }

    private static bool LEngineTextMatch(IReadOnlyList<string> one, IReadOnlyList<string> other)
    {
        if (one.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < one.Count; index++)
        {
            if (!string.Equals(one[index], other[index], StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    private LDraft LEngineDraftLoad(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        return LDraftArchive.LDraftArchiveRead(_lEngineWorkspace, id)
            ?? throw new LRefusal(LRefusal.LRefusalDraft);
    }

    private void LEngineCourtUpdate(LCourtLink link, string realId)
    {
        LDraft? owner = LDraftArchive.LDraftArchiveRead(_lEngineWorkspace, link.LCourtLinkOwner);
        if (owner is null)
        {
            return;
        }

        LEntryDraft content = owner.LDraftContent with
        {
            LEntryDraftSenses =
                LEngineTranslationUpdate(owner.LDraftContent.LEntryDraftSenses, link.LCourtLinkTarget, realId),
            LEntryDraftCollocations =
                LEngineTranslationUpdate(owner.LDraftContent.LEntryDraftCollocations, link.LCourtLinkTarget, realId),
        };

        LDraftArchive.LDraftArchiveSave(_lEngineWorkspace, owner with { LDraftContent = content });
    }

    private static IReadOnlyList<LCardDraft> LEngineTranslationUpdate(
        IReadOnlyList<LCardDraft> cards, string draftId, string realId)
    {
        List<LCardDraft> written = new(cards.Count);
        foreach (LCardDraft card in cards)
        {
            List<string> translations = new(card.LCardDraftTranslation.Count);
            foreach (string translation in card.LCardDraftTranslation)
            {
                translations.Add(
                    string.Equals(translation, draftId, StringComparison.Ordinal) ? realId : translation);
            }

            written.Add(card with { LCardDraftTranslation = translations });
        }

        return written;
    }
}
