using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LCourt LEngineCourtSave(
        long ownerId, long targetId, string headword, string language)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(ownerId);
            ArgumentOutOfRangeException.ThrowIfZero(targetId);
            ArgumentException.ThrowIfNullOrWhiteSpace(headword);

            LCourt link = new(
                LEngineIdentityCreate(),
                ownerId,
                targetId,
                headword,
                language ?? string.Empty);

            LCourtArchive.LCourtArchiveSave(_lEngineWorkspace, link);
            return link;
        }
    }

    public LCourt LEngineCourtStart(
        long ownerId, string origin, string headword, string language)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(ownerId);
            ArgumentException.ThrowIfNullOrWhiteSpace(headword);
            LEngineDraftValidate(ownerId);

            string named = language ?? string.Empty;
            LDraft target = LEngineDraftStart(origin, null);

            try
            {
                _lEngineDrafts.LDraftSave(target with
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

    public void LEngineCourtDelete(long linkId)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(linkId);
            LCourtArchive.LCourtArchiveDelete(_lEngineWorkspace, linkId);
        }
    }

    public LCourt? LEngineCourtFind(long ownerId, long targetId)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(ownerId);
            ArgumentOutOfRangeException.ThrowIfZero(targetId);

            foreach (LCourt link in LEngineCourtScan(ownerId))
            {
                if (link.LCourtTargetId == targetId)
                {
                    return link;
                }
            }

            return null;
        }
    }

    private void LEngineCourtRemove(long id)
    {
        IReadOnlyList<LCourt> court = LCourtArchive.LCourtArchiveScan(_lEngineWorkspace);

        foreach (LCourt link in court)
        {
            if (link.LCourtOwnerId != id)
            {
                if (_lEngineDrafts.LDraftRead(link.LCourtOwnerId) is null)
                {
                    LCourtArchive.LCourtArchiveDelete(_lEngineWorkspace, link.LCourtId);
                }

                continue;
            }

            LCourtArchive.LCourtArchiveDelete(_lEngineWorkspace, link.LCourtId);

            if (link.LCourtTargetId == id)
            {
                continue;
            }

            bool claimed = false;
            foreach (LCourt other in court)
            {
                if (other.LCourtOwnerId != id
                    && other.LCourtTargetId == link.LCourtTargetId)
                {
                    claimed = true;
                    break;
                }
            }

            LDraft? target = _lEngineDrafts.LDraftRead(link.LCourtTargetId);

            if (claimed || target is null || !LEngineHoldCheck(link.LCourtTargetId))
            {
                continue;
            }

            _lEngineDraftHeld.Remove(link.LCourtTargetId);
            LClaimArchive.LClaimArchiveDelete(_lEngineWorkspace, link.LCourtTargetId);
            _lEngineDrafts.LDraftDelete(link.LCourtTargetId);
        }
    }

    private IReadOnlyList<LCourt> LEngineCourtScan(long ownerId)
    {
        List<LCourt> owned = [];
        foreach (LCourt link in LCourtArchive.LCourtArchiveScan(_lEngineWorkspace))
        {
            if (link.LCourtOwnerId == ownerId)
            {
                owned.Add(link);
            }
        }

        return owned;
    }

    private void LEngineCourtUpdate(LCourt link, long realId)
    {
        LDraft? owner = _lEngineDrafts.LDraftRead(link.LCourtOwnerId);
        if (owner is null)
        {
            return;
        }

        LEntryDraft content = owner.LDraftContent with
        {
            LEntryDraftMeanings =
                LEngineTranslationUpdate(owner.LDraftContent.LEntryDraftMeanings, link.LCourtTargetId, realId),
            LEntryDraftCollocations =
                LEngineTranslationUpdate(owner.LDraftContent.LEntryDraftCollocations, link.LCourtTargetId, realId),
        };

        LEngineChronicleClear(owner.LDraftId);
        _lEngineDrafts.LDraftSave(owner with { LDraftContent = content });
    }

    private static IReadOnlyList<LCardDraft> LEngineTranslationUpdate(
        IReadOnlyList<LCardDraft> cards, long draftId, long realId)
    {
        List<LCardDraft> written = new(cards.Count);
        foreach (LCardDraft card in cards)
        {
            List<long> translations = new(card.LCardDraftTranslation.Count);
            foreach (long translation in card.LCardDraftTranslation)
            {
                if (translation != draftId)
                {
                    translations.Add(translation);
                    continue;
                }

                if (realId != 0)
                {
                    translations.Add(realId);
                }
            }

            written.Add(card with { LCardDraftTranslation = translations });
        }

        return written;
    }
}
