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
}
