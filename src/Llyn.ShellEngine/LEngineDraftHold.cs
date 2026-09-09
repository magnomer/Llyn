using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private readonly HashSet<string> _lEngineDraftHeld = new(StringComparer.Ordinal);

    private readonly HashSet<string> _lEngineDraftStale = new(StringComparer.Ordinal);

    public LDraft LEngineDraftStart(string origin, string? entryId)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(origin);

            string entry = string.IsNullOrWhiteSpace(entryId) ? string.Empty : entryId;
            LEntryDraft content = entry.Length == 0
                ? new LEntryDraft(string.Empty, string.Empty, null, string.Empty, [], [])
                : LEngineEntryLoad(entry) ?? throw new LRefusal(LRefusal.LRefusalEntry);

            LDraft draft = new(
                LIdentity.LIdentityCreate(),
                origin,
                entry,
                content,
                DateTimeOffset.UtcNow);

            LDraftArchive.LDraftArchiveSave(_lEngineWorkspace, draft);
            LClaimArchive.LClaimArchiveSave(
                _lEngineWorkspace, LClaimArchive.LClaimArchiveCreate(draft.LDraftId));
            _lEngineDraftHeld.Add(draft.LDraftId);
            return draft;
        }
    }

    public LEntryDraft LEngineDraftSave(LDraft draft)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(draft);
            LEngineDraftValidate(draft.LDraftId);

            LEntryDraft content = LEngineDraftNormalize(draft.LDraftContent);
            LDraftArchive.LDraftArchiveSave(_lEngineWorkspace, draft with { LDraftContent = content });
            return content;
        }
    }

    public LDraft? LEngineDraftRead(string id)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);
            LEngineDraftValidate(id);
            return LDraftArchive.LDraftArchiveRead(_lEngineWorkspace, id);
        }
    }

    public IReadOnlyList<LDraft> LEngineDraftScan()
    {
        lock (_lEngineGate)
        {
            return LDraftArchive.LDraftArchiveScan(_lEngineWorkspace);
        }
    }

    public void LEngineLeftoverSweep()
    {
        lock (_lEngineGate)
        {
            LDraftArchive.LDraftArchiveSweep(_lEngineWorkspace);
            LCourtArchive.LCourtArchiveSweep(_lEngineWorkspace);

            foreach (LDraft draft in LDraftArchive.LDraftArchiveScan(_lEngineWorkspace))
            {
                if (_lEngineDraftHeld.Contains(draft.LDraftId)
                    || LEngineClaimCheck(draft.LDraftId)
                    || string.IsNullOrWhiteSpace(draft.LDraftEntry))
                {
                    continue;
                }

                if (draft.LDraftExample is LExample sentence)
                {
                    if (LEngineExampleRead(draft.LDraftEntry) is LExample kept
                        && LEngineExampleMatch(kept, sentence))
                    {
                        LEngineDraftCancel(draft.LDraftId);
                    }

                    continue;
                }

                if (draft.LDraftSituation is LSituation situation)
                {
                    if (LEngineSituationRead(draft.LDraftEntry) is LSituation standing
                        && LEngineSituationMatch(standing, situation))
                    {
                        LEngineDraftCancel(draft.LDraftId);
                    }

                    continue;
                }

                if (draft.LDraftReference is LReference reference)
                {
                    if (LEngineReferenceRead(draft.LDraftEntry) is LReference cited
                        && LEngineReferenceMatch(cited, reference))
                    {
                        LEngineDraftCancel(draft.LDraftId);
                    }

                    continue;
                }

                LEntryDraft? stored = LEngineEntryLoad(draft.LDraftEntry);
                if (stored is null || !LEngineDraftMatch(stored, draft.LDraftContent))
                {
                    continue;
                }

                LEngineDraftCancel(draft.LDraftId);
            }
        }
    }

    public IReadOnlyList<LDraft> LEngineLeftoverRead()
    {
        lock (_lEngineGate)
        {
            List<LDraft> leftovers = [];
            foreach (LDraft draft in LDraftArchive.LDraftArchiveScan(_lEngineWorkspace))
            {
                if (_lEngineDraftHeld.Contains(draft.LDraftId)
                    || !LEngineDraftCheck(draft.LDraftId)
                    || LEngineClaimCheck(draft.LDraftId))
                {
                    continue;
                }

                leftovers.Add(draft);
            }

            return leftovers;
        }
    }

    public void LEngineDraftDelete(string id)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);
            LEngineDraftValidate(id);
            LEngineCourtRemove(id);
            _lEngineDraftHeld.Remove(id);
            _lEngineTrove.LTroveClear(id);
            LClaimArchive.LClaimArchiveDelete(_lEngineWorkspace, id);
            LDraftArchive.LDraftArchiveDelete(_lEngineWorkspace, id);
        }
    }

    public IReadOnlyList<LCardDraft> LEngineDraftMove(string id, bool collocation, int from, int target)
    {
        lock (_lEngineGate)
        {
            LEngineDraftValidate(id);
            LDraft draft = LEngineDraftLoad(id);
            List<LCardDraft> cards = new(
                collocation
                    ? draft.LDraftContent.LEntryDraftCollocations
                    : draft.LDraftContent.LEntryDraftMeanings);

            if (cards.Count != 0)
            {
                int origin = Math.Clamp(from, 0, cards.Count - 1);
                int landing = Math.Clamp(target, 0, cards.Count - 1);

                LCardDraft moved = cards[origin];
                cards.RemoveAt(origin);
                cards.Insert(landing, moved);
            }

            return LEngineCardApply(draft, collocation, cards);
        }
    }

    public IReadOnlyList<LCardDraft> LEngineDraftNormalize(string id, bool collocation)
    {
        lock (_lEngineGate)
        {
            LEngineDraftValidate(id);
            LDraft draft = LEngineDraftLoad(id);
            List<LCardDraft> cards = new(
                collocation
                    ? draft.LDraftContent.LEntryDraftCollocations
                    : draft.LDraftContent.LEntryDraftMeanings);

            return LEngineCardApply(draft, collocation, cards);
        }
    }

    public string LEngineCardCreate()
    {
        lock (_lEngineGate)
        {
            return LIdentity.LIdentityCreate();
        }
    }

    private IReadOnlyList<LCardDraft> LEngineCardApply(
        LDraft draft, bool collocation, List<LCardDraft> cards)
    {
        for (int index = 0; index < cards.Count; index++)
        {
            LCardDraft card = cards[index];
            cards[index] = card with
            {
                LCardDraftPosition = index + 1,
                LCardDraftId = card.LCardDraftId.Length == 0
                    ? LIdentity.LIdentityCreate()
                    : card.LCardDraftId,
            };
        }

        LEntryDraft content = collocation
            ? draft.LDraftContent with { LEntryDraftCollocations = cards }
            : draft.LDraftContent with { LEntryDraftMeanings = cards };

        LDraftArchive.LDraftArchiveSave(_lEngineWorkspace, draft with { LDraftContent = content });
        return cards;
    }

    public LCourtLink LEngineCourtSave(
        string ownerId, string targetId, string headword, string language)
    {
        lock (_lEngineGate)
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
    }

    public LCourtLink LEngineCourtStart(
        string ownerId, string origin, string headword, string language)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(ownerId);
            ArgumentException.ThrowIfNullOrWhiteSpace(headword);
            LEngineDraftValidate(ownerId);

            string named = language ?? string.Empty;
            LDraft target = LEngineDraftStart(origin, null);

            try
            {
                LEngineDraftSave(target with
                {
                    LDraftContent = target.LDraftContent with
                    {
                        LEntryDraftHeadword = headword,
                        LEntryDraftLanguage = named,
                    },
                });

                return LEngineCourtSave(ownerId, target.LDraftId, headword, named);
            }
            catch (Exception)
            {
                LEngineDraftCancel(target.LDraftId);
                throw;
            }
        }
    }

    public void LEngineCourtDelete(string linkId)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(linkId);
            LCourtArchive.LCourtArchiveDelete(_lEngineWorkspace, linkId);
        }
    }

    public LCourtLink? LEngineCourtFind(string ownerId, string targetId)
    {
        lock (_lEngineGate)
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
    }

    public bool LEngineDraftCheck(string id)
    {
        lock (_lEngineGate)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return false;
            }

            LEngineDraftValidate(id);

            LDraft? draft = LDraftArchive.LDraftArchiveRead(_lEngineWorkspace, id);
            if (draft is null)
            {
                return false;
            }

            if (draft.LDraftExample is LExample sentence)
            {
                return LEngineExampleCheck(draft, sentence);
            }

            if (draft.LDraftSituation is LSituation situation)
            {
                return LEngineSituationCheck(draft, situation);
            }

            if (draft.LDraftReference is LReference reference)
            {
                return LEngineReferenceCheck(draft, reference);
            }

            LEntryDraft origin = string.IsNullOrWhiteSpace(draft.LDraftEntry)
                ? LEngineDraftBlank with { LEntryDraftLanguage = draft.LDraftContent.LEntryDraftLanguage }
                : LEngineEntryLoad(draft.LDraftEntry) ?? LEngineDraftBlank;

            return !LEngineDraftMatch(origin, draft.LDraftContent);
        }
    }

    public LEntry LEngineDraftCommit(string id)
    {
        LEntry stored;
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);
            LEngineDraftValidate(id);
            stored = LEngineDraftCommit(id, [], true);
            _lEngineTrove.LTroveClear(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectEntry, stored.LEntryId);
        return stored;
    }

    private LEntry LEngineDraftCommit(string id, HashSet<string> entered, bool held)
    {
        entered.Add(id);

        foreach (LCourtLink link in LEngineCourtScan(id))
        {
            if (entered.Contains(link.LCourtLinkTarget) ||
                LDraftArchive.LDraftArchiveRead(_lEngineWorkspace, link.LCourtLinkTarget) is null)
            {
                continue;
            }

            LEngineDraftCommit(
                link.LCourtLinkTarget, entered, LEngineHoldCheck(link.LCourtLinkTarget));
        }

        LDraft draft = LEngineDraftLoad(id);

        LEntryDraft sending = LEngineTranslationSettle(draft.LDraftContent);

        LEntry entry = string.IsNullOrWhiteSpace(draft.LDraftEntry)
            || LEngineEntryLoad(draft.LDraftEntry) is null
            ? LEngineEntrySave(sending)
            : LEngineEntryUpdate(draft.LDraftEntry, sending);

        LDraft settled = draft with { LDraftEntry = entry.LEntryId };
        if (LEngineEntryLoad(entry.LEntryId) is LEntryDraft written)
        {
            settled = settled with { LDraftContent = written };
        }

        LDraftArchive.LDraftArchiveSave(_lEngineWorkspace, settled);

        foreach (LCourtLink link in
            LCourtArchive.LCourtArchiveSettle(_lEngineWorkspace, id))
        {
            LEngineCourtUpdate(link, entry.LEntryId);
        }

        if (!held)
        {
            return entry;
        }

        _lEngineDraftHeld.Remove(id);
        LClaimArchive.LClaimArchiveDelete(_lEngineWorkspace, id);
        LDraftArchive.LDraftArchiveDelete(_lEngineWorkspace, id);
        return entry;
    }

    public void LEngineDraftCancel(string id)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);
            LEngineDraftValidate(id);

            LEngineCourtRemove(id);

            foreach (LCourtLink link in LCourtArchive.LCourtArchiveSettle(_lEngineWorkspace, id))
            {
                LEngineCourtUpdate(link, string.Empty);
            }

            _lEngineDraftHeld.Remove(id);
            _lEngineTrove.LTroveClear(id);
            LClaimArchive.LClaimArchiveDelete(_lEngineWorkspace, id);
            LDraftArchive.LDraftArchiveDelete(_lEngineWorkspace, id);
        }
    }

    private void LEngineCourtRemove(string id)
    {
        IReadOnlyList<LCourtLink> court = LCourtArchive.LCourtArchiveScan(_lEngineWorkspace);

        foreach (LCourtLink link in court)
        {
            if (!string.Equals(link.LCourtLinkOwner, id, StringComparison.Ordinal))
            {
                if (LDraftArchive.LDraftArchiveRead(_lEngineWorkspace, link.LCourtLinkOwner) is null)
                {
                    LCourtArchive.LCourtArchiveDelete(_lEngineWorkspace, link.LCourtLinkId);
                }

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

            if (claimed || target is null || !LEngineHoldCheck(link.LCourtLinkTarget))
            {
                continue;
            }

            _lEngineDraftHeld.Remove(link.LCourtLinkTarget);
            LClaimArchive.LClaimArchiveDelete(_lEngineWorkspace, link.LCourtLinkTarget);
            LDraftArchive.LDraftArchiveDelete(_lEngineWorkspace, link.LCourtLinkTarget);
        }
    }

    private bool LEngineHoldCheck(string id)
    {
        LClaim? claim = LClaimArchive.LClaimArchiveCheck(_lEngineWorkspace, id)
            ? LClaimArchive.LClaimArchiveRead(_lEngineWorkspace, id)
            : null;

        return claim is not null
            && claim.LClaimProcess == Environment.ProcessId
            && _lEngineDraftHeld.Contains(id);
    }

    private bool LEngineClaimCheck(string id)
    {
        if (!LClaimArchive.LClaimArchiveCheck(_lEngineWorkspace, id))
        {
            return false;
        }

        LClaim? claim = LClaimArchive.LClaimArchiveRead(_lEngineWorkspace, id);
        return claim is not null && claim.LClaimProcess != Environment.ProcessId;
    }

    private static LEntryDraft LEngineDraftNormalize(LEntryDraft content)
    {
        IReadOnlyList<LCardDraft> meanings = LEngineCardNormalize(content.LEntryDraftMeanings);
        IReadOnlyList<LCardDraft> collocations = LEngineCardNormalize(content.LEntryDraftCollocations);

        return ReferenceEquals(meanings, content.LEntryDraftMeanings)
            && ReferenceEquals(collocations, content.LEntryDraftCollocations)
            ? content
            : content with
            {
                LEntryDraftMeanings = meanings,
                LEntryDraftCollocations = collocations,
            };
    }

    private static IReadOnlyList<LCardDraft> LEngineCardNormalize(IReadOnlyList<LCardDraft> cards)
    {
        List<LCardDraft>? named = null;
        for (int index = 0; index < cards.Count; index++)
        {
            if (cards[index].LCardDraftId.Length != 0)
            {
                named?.Add(cards[index]);
                continue;
            }

            if (named is null)
            {
                named = new List<LCardDraft>(cards.Count);
                for (int earlier = 0; earlier < index; earlier++)
                {
                    named.Add(cards[earlier]);
                }
            }

            named.Add(cards[index] with { LCardDraftId = LIdentity.LIdentityCreate() });
        }

        return named ?? cards;
    }

    private static LEntryDraft LEngineDraftBlank =>
        new(string.Empty, string.Empty, null, string.Empty, [], []);

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
            && LEngineSoundMatch(one.LEntryDraftPronunciation, other.LEntryDraftPronunciation)
            && string.Equals(one.LEntryDraftNote, other.LEntryDraftNote, StringComparison.Ordinal)
            && LEngineSpeechMatch(one.LEntryDraftSpeeches, other.LEntryDraftSpeeches)
            && LEngineFormMatch(one.LEntryDraftForms, other.LEntryDraftForms)
            && LEngineInflectionMatch(one.LEntryDraftInflections, other.LEntryDraftInflections)
            && LEngineCardMatch(one.LEntryDraftMeanings, other.LEntryDraftMeanings)
            && LEngineCardMatch(one.LEntryDraftCollocations, other.LEntryDraftCollocations);
    }

    private static bool LEngineSoundMatch(LPronunciationDraft? one, LPronunciationDraft? other)
    {
        if (one is null || other is null)
        {
            return one is null && other is null;
        }

        if (!string.Equals(one.LPronunciationDraftIpa, other.LPronunciationDraftIpa, StringComparison.Ordinal)
            || !string.Equals(
                one.LPronunciationDraftLevel, other.LPronunciationDraftLevel, StringComparison.Ordinal)
            || !string.Equals(
                one.LPronunciationDraftAudio, other.LPronunciationDraftAudio, StringComparison.Ordinal)
            || !string.Equals(
                one.LPronunciationDraftSource, other.LPronunciationDraftSource, StringComparison.Ordinal)
            || one.LPronunciationDraftSyllables.Count != other.LPronunciationDraftSyllables.Count
            || one.LPronunciationDraftRepresentations.Count
                != other.LPronunciationDraftRepresentations.Count)
        {
            return false;
        }

        for (int index = 0; index < one.LPronunciationDraftSyllables.Count; index++)
        {
            if (one.LPronunciationDraftSyllables[index] != other.LPronunciationDraftSyllables[index])
            {
                return false;
            }
        }

        for (int index = 0; index < one.LPronunciationDraftRepresentations.Count; index++)
        {
            if (one.LPronunciationDraftRepresentations[index]
                != other.LPronunciationDraftRepresentations[index])
            {
                return false;
            }
        }

        return true;
    }

    private static bool LEngineSpeechMatch(
        IReadOnlyList<LSpeechDraft> one, IReadOnlyList<LSpeechDraft> other)
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
                || !string.Equals(written.LCardDraftGloss, held.LCardDraftGloss, StringComparison.Ordinal)
                || !string.Equals(
                    written.LCardDraftLanguage, held.LCardDraftLanguage, StringComparison.Ordinal)
                || !string.Equals(written.LCardDraftLabels, held.LCardDraftLabels, StringComparison.Ordinal)
                || !LEngineSentenceMatch(written.LCardDraftSentence, held.LCardDraftSentence)
                || !LEngineSituationMatch(written.LCardDraftSituation, held.LCardDraftSituation)
                || !LEngineRegisterMatch(written.LCardDraftRegister, held.LCardDraftRegister)
                || !LEngineTextMatch(written.LCardDraftTranslation, held.LCardDraftTranslation)
                || !LEngineTextMatch(written.LCardDraftTag, held.LCardDraftTag)
                || !LEngineImageMatch(written.LCardDraftImage, held.LCardDraftImage)
                || !LEngineVideoMatch(written.LCardDraftVideo, held.LCardDraftVideo)
                || !LEngineCardMatch(written.LCardDraftChild, held.LCardDraftChild))
            {
                return false;
            }
        }

        return true;
    }

    private static bool LEngineVideoMatch(IReadOnlyList<LVideoDraft> one, IReadOnlyList<LVideoDraft> other)
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
        return string.IsNullOrEmpty(card.LCardDraftGloss)
            && card.LCardDraftTitle.LStateValueEmpty
            && card.LCardDraftExpression.LStateValueEmpty
            && card.LCardDraftMeaning.LStateValueEmpty
            && card.LCardDraftSentence.Count == 0
            && card.LCardDraftSituation.Count == 0
            && card.LCardDraftRegister.Count == 0
            && card.LCardDraftTranslation.Count == 0
            && card.LCardDraftTag.Count == 0
            && card.LCardDraftImage.Count == 0
            && card.LCardDraftVideo.Count == 0
            && card.LCardDraftChild.Count == 0;
    }

    private static bool LEngineSentenceMatch(
        IReadOnlyList<LSentenceDraft> one, IReadOnlyList<LSentenceDraft> other)
    {
        if (one.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < one.Count; index++)
        {
            if (!string.Equals(
                    one[index].LSentenceDraftId, other[index].LSentenceDraftId, StringComparison.Ordinal)
                || one[index].LSentenceDraftParticle != other[index].LSentenceDraftParticle
                || one[index].LSentenceDraftDependence != other[index].LSentenceDraftDependence
                || !LEngineExampleMatch(
                    one[index].LSentenceDraftExample, other[index].LSentenceDraftExample))
            {
                return false;
            }
        }

        return true;
    }

    private static bool LEngineExampleMatch(LExampleDraft? one, LExampleDraft? other)
    {
        if (one is null || other is null)
        {
            return one is null && other is null;
        }

        return one.LExampleDraftText == other.LExampleDraftText
            && string.Equals(one.LExampleDraftId, other.LExampleDraftId, StringComparison.Ordinal)
            && one.LExampleDraftReference == other.LExampleDraftReference
            && one.LExampleDraftTranslation == other.LExampleDraftTranslation
            && string.Equals(
                one.LExampleDraftLanguage, other.LExampleDraftLanguage, StringComparison.Ordinal);
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
            if (one[index].LSituationDraftTitle != other[index].LSituationDraftTitle
                || one[index].LSituationDraftDescription != other[index].LSituationDraftDescription
                || one[index].LSituationDraftKind != other[index].LSituationDraftKind
                || !string.Equals(
                    one[index].LSituationDraftId, other[index].LSituationDraftId, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    private static bool LEngineImageMatch(IReadOnlyList<LImageDraft> one, IReadOnlyList<LImageDraft> other)
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

    private void LEngineDraftValidate(string id)
    {
        if (_lEngineDraftStale.Contains(id))
        {
            throw new LRefusal(LRefusal.LRefusalStale);
        }
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
            LEntryDraftMeanings =
                LEngineTranslationUpdate(owner.LDraftContent.LEntryDraftMeanings, link.LCourtLinkTarget, realId),
            LEntryDraftCollocations =
                LEngineTranslationUpdate(owner.LDraftContent.LEntryDraftCollocations, link.LCourtLinkTarget, realId),
        };

        LDraftArchive.LDraftArchiveSave(_lEngineWorkspace, owner with { LDraftContent = content });
    }

    private LEntryDraft LEngineTranslationSettle(LEntryDraft content)
    {
        return content with
        {
            LEntryDraftMeanings = LEngineTranslationSettle(content.LEntryDraftMeanings),
            LEntryDraftCollocations = LEngineTranslationSettle(content.LEntryDraftCollocations),
        };
    }

    private IReadOnlyList<LCardDraft> LEngineTranslationSettle(IReadOnlyList<LCardDraft> cards)
    {
        List<LCardDraft> written = new(cards.Count);
        foreach (LCardDraft card in cards)
        {
            List<string> translations = new(card.LCardDraftTranslation.Count);
            foreach (string translation in card.LCardDraftTranslation)
            {
                if (!string.IsNullOrWhiteSpace(translation) && LEngineEntryLoad(translation) is not null)
                {
                    translations.Add(translation);
                }
            }

            written.Add(card with { LCardDraftTranslation = translations });
        }

        return written;
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
                if (!string.Equals(translation, draftId, StringComparison.Ordinal))
                {
                    translations.Add(translation);
                    continue;
                }

                if (realId.Length != 0)
                {
                    translations.Add(realId);
                }
            }

            written.Add(card with { LCardDraftTranslation = translations });
        }

        return written;
    }
}
