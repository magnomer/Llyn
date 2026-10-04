using System;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LAuthorCitation
{
    private readonly LVault _lAuthorCitationVault;
    private readonly LClaimClerk _lAuthorCitationClaims;
    private readonly LAuthorClerk _lAuthorCitationAuthors;
    private readonly LEntryClerk _lAuthorCitationEntries;

    public LAuthorCitation(LRig rig, LClaimClerk claims, LAuthorClerk authors, LEntryClerk entries)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(claims);
        ArgumentNullException.ThrowIfNull(authors);
        ArgumentNullException.ThrowIfNull(entries);
        _lAuthorCitationVault = rig.LRigVault;
        _lAuthorCitationClaims = claims;
        _lAuthorCitationAuthors = authors;
        _lAuthorCitationEntries = entries;
    }

    public LDraft LAuthorCitationStart(string origin, long? authorId)
    {
        long author = authorId is null or <= 0 ? 0 : authorId.Value;
        LAuthor content = author == 0
            ? new LAuthor(0, string.Empty)
            : _lAuthorCitationAuthors.LAuthorClerkRead(author) ?? throw new LRefusal(LRefusal.LRefusalLink);

        LDraft draft = _lAuthorCitationClaims.LDraftCreate(origin, author, LClaimClerk.LDraftBlank) with
        {
            LDraftAuthorHeld = content,
        };

        return _lAuthorCitationClaims.LClaimClerkStart(draft);
    }

    public LAuthor LAuthorCitationCommit(long id)
    {
        LDraft draft = _lAuthorCitationClaims.LDraftLoad(id);
        LAuthor held = draft.LDraftAuthorHeld ?? throw new LRefusal(LRefusal.LRefusalLink);
        string name = held.LAuthorName.Trim();
        if (!held.LAuthorNamed)
        {
            throw new LRefusal(LRefusal.LRefusalName);
        }

        LAuthor stored;
        using (LVaultSession session = _lAuthorCitationVault.LVaultSessionStart())
        {
            bool fresh = draft.LDraftEntryId == 0
                || _lAuthorCitationAuthors.LAuthorClerkRead(draft.LDraftEntryId) is null;
            if (fresh)
            {
                stored = _lAuthorCitationAuthors.LAuthorClerkCreate(new LAuthor(0, name));
            }
            else
            {
                stored = new LAuthor(draft.LDraftEntryId, name);
                _lAuthorCitationAuthors.LAuthorClerkUpdate(stored);
            }

            LCitationClerk.LRevisionRecord(
                _lAuthorCitationEntries, stored.LAuthorId, "author", fresh, stored.LAuthorName);
            session.LVaultSessionCommit();
        }

        _lAuthorCitationClaims.LDraftSave(draft with { LDraftEntryId = stored.LAuthorId, LDraftAuthorHeld = stored });
        _lAuthorCitationClaims.LClaimClerkFinish(id);
        return stored;
    }

    public bool LAuthorCitationCheck(LDraft draft, LAuthor held)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(held);

        string origin = draft.LDraftEntryId == 0
            ? string.Empty
            : _lAuthorCitationAuthors.LAuthorClerkRead(draft.LDraftEntryId)?.LAuthorName ?? string.Empty;

        return !string.Equals(origin.Trim(), held.LAuthorName.Trim(), StringComparison.Ordinal);
    }
}
