using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LCourtLink LEngineCourtSave(
        long ownerId, long targetId, string headword, string language)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ownerId);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(targetId);
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
        long ownerId, string origin, string headword, string language)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ownerId);
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

    public void LEngineCourtDelete(long linkId)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(linkId);
            LCourtArchive.LCourtArchiveDelete(_lEngineWorkspace, linkId);
        }
    }

    public LCourtLink? LEngineCourtFind(long ownerId, long targetId)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ownerId);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(targetId);

            foreach (LCourtLink link in LEngineCourtScan(ownerId))
            {
                if (link.LCourtLinkTarget == targetId)
                {
                    return link;
                }
            }

            return null;
        }
    }

    private void LEngineCourtRemove(long id)
    {
        IReadOnlyList<LCourtLink> court = LCourtArchive.LCourtArchiveScan(_lEngineWorkspace);

        foreach (LCourtLink link in court)
        {
            if (link.LCourtLinkOwner != id)
            {
                if (LDraftArchive.LDraftArchiveRead(_lEngineWorkspace, link.LCourtLinkOwner) is null)
                {
                    LCourtArchive.LCourtArchiveDelete(_lEngineWorkspace, link.LCourtLinkId);
                }

                continue;
            }

            LCourtArchive.LCourtArchiveDelete(_lEngineWorkspace, link.LCourtLinkId);

            if (link.LCourtLinkTarget == id)
            {
                continue;
            }

            bool claimed = false;
            foreach (LCourtLink other in court)
            {
                if (other.LCourtLinkOwner != id
                    && other.LCourtLinkTarget == link.LCourtLinkTarget)
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

    private IReadOnlyList<LCourtLink> LEngineCourtScan(long ownerId)
    {
        List<LCourtLink> owned = [];
        foreach (LCourtLink link in LCourtArchive.LCourtArchiveScan(_lEngineWorkspace))
        {
            if (link.LCourtLinkOwner == ownerId)
            {
                owned.Add(link);
            }
        }

        return owned;
    }

    private void LEngineCourtUpdate(LCourtLink link, long realId)
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
