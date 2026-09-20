using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LCitationClerk
{
    private readonly LVault _lCitationClerkVault;
    private readonly LIdentity _lCitationClerkIdentity;
    private readonly LClaimClerk _lCitationClerkClaims;
    private readonly LAuthorClerk _lCitationClerkAuthors;
    private readonly LExampleClerk _lCitationClerkExamples;
    private readonly LReferenceClerk _lCitationClerkReferences;
    private readonly LSituationClerk _lCitationClerkSituations;
    private readonly LEntryClerk _lCitationClerkEntries;

    public LCitationClerk(
        LRig rig,
        LIdentity identity,
        LClaimClerk claims,
        LAuthorClerk authors,
        LExampleClerk examples,
        LReferenceClerk references,
        LSituationClerk situations,
        LEntryClerk entries)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(identity);
        ArgumentNullException.ThrowIfNull(claims);
        ArgumentNullException.ThrowIfNull(authors);
        ArgumentNullException.ThrowIfNull(examples);
        ArgumentNullException.ThrowIfNull(references);
        ArgumentNullException.ThrowIfNull(situations);
        ArgumentNullException.ThrowIfNull(entries);
        _lCitationClerkVault = rig.LRigVault;
        _lCitationClerkIdentity = identity;
        _lCitationClerkClaims = claims;
        _lCitationClerkAuthors = authors;
        _lCitationClerkExamples = examples;
        _lCitationClerkReferences = references;
        _lCitationClerkSituations = situations;
        _lCitationClerkEntries = entries;
    }

    public LDraft LAuthorStart(string origin, long? authorId)
    {
        long author = authorId is null or <= 0 ? 0 : authorId.Value;
        LAuthor content = author == 0
            ? new LAuthor(0, string.Empty)
            : _lCitationClerkAuthors.LAuthorClerkRead(author) ?? throw new LRefusal(LRefusal.LRefusalLink);

        LDraft draft = _lCitationClerkClaims.LDraftCreate(origin, author, LClaimClerk.LDraftBlank) with
        {
            LDraftAuthorHeld = content,
        };

        return _lCitationClerkClaims.LClaimClerkStart(draft);
    }

    public LAuthor LAuthorCommit(long id)
    {
        LDraft draft = _lCitationClerkClaims.LDraftLoad(id);
        LAuthor held = draft.LDraftAuthorHeld ?? throw new LRefusal(LRefusal.LRefusalLink);
        string name = held.LAuthorName.Trim();
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        LAuthor stored;
        using (LVaultSession session = _lCitationClerkVault.LVaultSessionStart())
        {
            bool fresh = draft.LDraftEntryId == 0
                || _lCitationClerkAuthors.LAuthorClerkRead(draft.LDraftEntryId) is null;
            if (fresh)
            {
                stored = _lCitationClerkAuthors.LAuthorClerkCreate(new LAuthor(0, name));
            }
            else
            {
                stored = new LAuthor(draft.LDraftEntryId, name);
                _lCitationClerkAuthors.LAuthorClerkUpdate(stored);
            }

            LRevisionRecord(stored.LAuthorId, "author", fresh, stored.LAuthorName);
            session.LVaultSessionCommit();
        }

        _lCitationClerkClaims.LDraftSave(draft with { LDraftEntryId = stored.LAuthorId, LDraftAuthorHeld = stored });
        _lCitationClerkClaims.LClaimClerkFinish(id);
        return stored;
    }

    public bool LAuthorCheck(LDraft draft, LAuthor held)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(held);

        string origin = draft.LDraftEntryId == 0
            ? string.Empty
            : _lCitationClerkAuthors.LAuthorClerkRead(draft.LDraftEntryId)?.LAuthorName ?? string.Empty;

        return !string.Equals(origin.Trim(), held.LAuthorName.Trim(), StringComparison.Ordinal);
    }

    public LDraft LExampleStart(string origin, long? exampleId)
    {
        long example = exampleId is null or <= 0 ? 0 : exampleId.Value;
        LExample content = example == 0
            ? LExampleClerk.LExampleClerkBlank with { LExampleId = _lCitationClerkIdentity.LIdentityCreate() }
            : _lCitationClerkExamples.LExampleClerkRead(example) ?? throw new LRefusal(LRefusal.LRefusalExample);

        LDraft draft = _lCitationClerkClaims.LDraftCreate(origin, example, LClaimClerk.LDraftBlank) with
        {
            LDraftExample = content,
        };

        return _lCitationClerkClaims.LClaimClerkStart(draft);
    }

    public LExample LExampleCommit(long id)
    {
        LDraft draft = _lCitationClerkClaims.LDraftLoad(id);
        LExample sending = LExampleNormalize(
            draft.LDraftExample ?? throw new LRefusal(LRefusal.LRefusalExample));

        LExample stored;
        using (LVaultSession session = _lCitationClerkVault.LVaultSessionStart())
        {
            bool fresh = draft.LDraftEntryId == 0
                || _lCitationClerkExamples.LExampleClerkRead(draft.LDraftEntryId) is null;
            if (fresh)
            {
                stored = _lCitationClerkExamples.LExampleClerkCreate(sending);
            }
            else
            {
                LExample written = sending with { LExampleId = draft.LDraftEntryId };
                _lCitationClerkExamples.LExampleClerkUpdate(written);
                stored = _lCitationClerkExamples.LExampleClerkRead(draft.LDraftEntryId) ?? written;
            }

            LRevisionRecord(stored.LExampleId, "example", fresh, stored.LExampleText.LStateValueShow());
            session.LVaultSessionCommit();
        }

        _lCitationClerkClaims.LDraftSave(draft with { LDraftEntryId = stored.LExampleId, LDraftExample = stored });
        _lCitationClerkClaims.LClaimClerkFinish(id);
        return stored;
    }

    public bool LExampleCheck(LDraft draft, LExample held)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(held);

        LExample blank = LExampleClerk.LExampleClerkBlank with { LExampleLanguage = held.LExampleLanguage };
        LExample origin = draft.LDraftEntryId == 0
            ? blank
            : _lCitationClerkExamples.LExampleClerkRead(draft.LDraftEntryId) ?? blank;

        return !LExampleClerk.LExampleClerkMatch(origin, held);
    }

    public LDraft LReferenceStart(string origin, long? referenceId)
    {
        long reference = referenceId is null or <= 0 ? 0 : referenceId.Value;
        LReference content = reference == 0
            ? LReferenceClerk.LReferenceClerkBlank with { LReferenceId = _lCitationClerkIdentity.LIdentityCreate() }
            : _lCitationClerkReferences.LReferenceClerkRead(reference)
                ?? throw new LRefusal(LRefusal.LRefusalReference);

        LDraft draft = _lCitationClerkClaims.LDraftCreate(origin, reference, LClaimClerk.LDraftBlank) with
        {
            LDraftReference = content,
            LDraftAuthor = reference == 0 ? [] : _lCitationClerkAuthors.LAuthorReferenceRead(reference),
        };

        return _lCitationClerkClaims.LClaimClerkStart(draft);
    }

    public LReference LReferenceCommit(long id)
    {
        LDraft draft = _lCitationClerkClaims.LDraftLoad(id);
        LReference sending = LReferenceNormalize(
            draft.LDraftReference ?? throw new LRefusal(LRefusal.LRefusalReference));

        LReference stored;
        using (LVaultSession session = _lCitationClerkVault.LVaultSessionStart())
        {
            bool fresh = draft.LDraftEntryId == 0
                || _lCitationClerkReferences.LReferenceClerkRead(draft.LDraftEntryId) is null;
            if (fresh)
            {
                stored = _lCitationClerkReferences.LReferenceClerkCreate(sending);
            }
            else
            {
                LReference written = sending with { LReferenceId = draft.LDraftEntryId };
                _lCitationClerkReferences.LReferenceClerkUpdate(written);
                stored = _lCitationClerkReferences.LReferenceClerkRead(draft.LDraftEntryId) ?? written;
            }

            _lCitationClerkAuthors.LAuthorReferenceSave(stored.LReferenceId, draft.LDraftAuthor);
            LRevisionRecord(stored.LReferenceId, "reference", fresh, stored.LReferenceTitle.LStateValueShow());
            session.LVaultSessionCommit();
        }

        _lCitationClerkClaims.LDraftSave(
            draft with { LDraftEntryId = stored.LReferenceId, LDraftReference = stored });
        _lCitationClerkClaims.LClaimClerkFinish(id);
        return stored;
    }

    public bool LReferenceCheck(LDraft draft, LReference held)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(held);

        LReference blank = LReferenceClerk.LReferenceClerkBlank;
        LReference origin = draft.LDraftEntryId == 0
            ? blank
            : _lCitationClerkReferences.LReferenceClerkRead(draft.LDraftEntryId) ?? blank;

        IReadOnlyList<LAuthor> credited = draft.LDraftEntryId == 0
            ? []
            : _lCitationClerkAuthors.LAuthorReferenceRead(draft.LDraftEntryId);

        return !LReferenceClerk.LReferenceClerkMatch(origin, held)
            || !LAuthorClerk.LAuthorClerkMatch(credited, draft.LDraftAuthor);
    }

    public LDraft LSituationStart(string origin, long? situationId)
    {
        long situation = situationId is null or <= 0 ? 0 : situationId.Value;
        LSituation content = situation == 0
            ? LSituationClerk.LSituationClerkBlank with { LSituationId = _lCitationClerkIdentity.LIdentityCreate() }
            : _lCitationClerkSituations.LSituationClerkRead(situation)
                ?? throw new LRefusal(LRefusal.LRefusalSituation);

        LDraft draft = _lCitationClerkClaims.LDraftCreate(origin, situation, LClaimClerk.LDraftBlank) with
        {
            LDraftSituation = content,
        };

        return _lCitationClerkClaims.LClaimClerkStart(draft);
    }

    public LSituation LSituationCommit(long id)
    {
        LDraft draft = _lCitationClerkClaims.LDraftLoad(id);
        LSituation sending = LSituationNormalize(
            draft.LDraftSituation ?? throw new LRefusal(LRefusal.LRefusalSituation));

        LSituation stored;
        using (LVaultSession session = _lCitationClerkVault.LVaultSessionStart())
        {
            bool fresh = draft.LDraftEntryId == 0
                || _lCitationClerkSituations.LSituationClerkRead(draft.LDraftEntryId) is null;
            if (fresh)
            {
                stored = _lCitationClerkSituations.LSituationClerkCreate(sending);
            }
            else
            {
                LSituation written = sending with { LSituationId = draft.LDraftEntryId };
                _lCitationClerkSituations.LSituationClerkUpdate(written);
                stored = _lCitationClerkSituations.LSituationClerkRead(draft.LDraftEntryId) ?? written;
            }

            LRevisionRecord(stored.LSituationId, "situation", fresh, stored.LSituationTitle.LStateValueShow());
            session.LVaultSessionCommit();
        }

        _lCitationClerkClaims.LDraftSave(
            draft with { LDraftEntryId = stored.LSituationId, LDraftSituation = stored });
        _lCitationClerkClaims.LClaimClerkFinish(id);
        return stored;
    }

    public bool LSituationCheck(LDraft draft, LSituation held)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(held);

        LSituation blank = LSituationClerk.LSituationClerkBlank;
        LSituation origin = draft.LDraftEntryId == 0
            ? blank
            : _lCitationClerkSituations.LSituationClerkRead(draft.LDraftEntryId) ?? blank;

        return !LSituationClerk.LSituationClerkMatch(origin, held);
    }

    public LDraft LEntryStart(string origin, long? entryId, string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(origin);
        ArgumentNullException.ThrowIfNull(language);

        long entry = entryId is null or <= 0 ? 0 : entryId.Value;
        LEntryDraft content = entry == 0
            ? LClaimClerk.LDraftBlank with { LEntryDraftLanguage = language }
            : _lCitationClerkEntries.LEntryClerkLoad(entry) ?? throw new LRefusal(LRefusal.LRefusalEntry);

        return _lCitationClerkClaims.LClaimClerkStart(_lCitationClerkClaims.LDraftCreate(origin, entry, content));
    }

    public bool LCitationDraftCheck(LDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        if (draft.LDraftExample is LExample sentence)
        {
            return LExampleCheck(draft, sentence);
        }

        if (draft.LDraftSituation is LSituation situation)
        {
            return LSituationCheck(draft, situation);
        }

        if (draft.LDraftReference is LReference reference)
        {
            return LReferenceCheck(draft, reference);
        }

        if (draft.LDraftAuthorHeld is LAuthor author)
        {
            return LAuthorCheck(draft, author);
        }

        LEntryDraft origin = draft.LDraftEntryId <= 0
            ? LClaimClerk.LDraftBlank with { LEntryDraftLanguage = draft.LDraftContent.LEntryDraftLanguage }
            : _lCitationClerkEntries.LEntryClerkLoad(draft.LDraftEntryId) ?? LClaimClerk.LDraftBlank;

        return !LDraftClerkEquality.LDraftMatch(origin, draft.LDraftContent);
    }

    public bool LCitationLeftoverCheck(LDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        if (draft.LDraftExample is LExample sentence)
        {
            return _lCitationClerkExamples.LExampleClerkRead(draft.LDraftEntryId) is LExample kept
                && LExampleClerk.LExampleClerkMatch(kept, sentence);
        }

        if (draft.LDraftSituation is LSituation situation)
        {
            return _lCitationClerkSituations.LSituationClerkRead(draft.LDraftEntryId) is LSituation standing
                && LSituationClerk.LSituationClerkMatch(standing, situation);
        }

        if (draft.LDraftReference is LReference reference)
        {
            return _lCitationClerkReferences.LReferenceClerkRead(draft.LDraftEntryId) is LReference cited
                && LReferenceClerk.LReferenceClerkMatch(cited, reference);
        }

        if (draft.LDraftAuthorHeld is LAuthor author)
        {
            return !LAuthorCheck(draft, author);
        }

        LEntryDraft? stored = _lCitationClerkEntries.LEntryClerkLoad(draft.LDraftEntryId);
        return stored is not null && LDraftClerkEquality.LDraftMatch(stored, draft.LDraftContent);
    }

    private void LRevisionRecord(long target, string subject, bool fresh, string? summary)
    {
        _lCitationClerkEntries.LRevisionRecord(
            [new LRevisionChange(0, target, subject, fresh ? "create" : "update", summary)]);
    }

    private LExample LExampleNormalize(LExample content)
    {
        return content.LExampleId == 0
            ? content with { LExampleId = _lCitationClerkIdentity.LIdentityCreate() }
            : content;
    }

    private LReference LReferenceNormalize(LReference content)
    {
        return content.LReferenceId == 0
            ? content with { LReferenceId = _lCitationClerkIdentity.LIdentityCreate() }
            : content;
    }

    private LSituation LSituationNormalize(LSituation content)
    {
        IReadOnlyList<LImageDraft> images = LDraftClerkList.LDraftListNormalize(
            content.LSituationImage,
            static row => row.LImageDraftEmpty,
            row => row.LImageDraftId == 0
                ? row with { LImageDraftId = _lCitationClerkIdentity.LIdentityCreate() }
                : row);
        IReadOnlyList<LVideoDraft> videos = LDraftClerkList.LDraftListNormalize(
            content.LSituationVideo,
            static row => row.LVideoDraftEmpty,
            row => row.LVideoDraftId == 0
                ? row with { LVideoDraftId = _lCitationClerkIdentity.LIdentityCreate() }
                : row);

        if (content.LSituationId != 0
            && ReferenceEquals(images, content.LSituationImage)
            && ReferenceEquals(videos, content.LSituationVideo))
        {
            return content;
        }

        return content with
        {
            LSituationId = content.LSituationId == 0 ? _lCitationClerkIdentity.LIdentityCreate() : content.LSituationId,
            LSituationImage = images,
            LSituationVideo = videos,
        };
    }
}
