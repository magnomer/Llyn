using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
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
