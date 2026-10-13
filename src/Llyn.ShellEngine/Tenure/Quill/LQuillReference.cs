using System;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LQuillReference
{
    private readonly LTenure _lQuillReferenceTenure;

    public LQuillReference(LTenure tenure)
    {
        ArgumentNullException.ThrowIfNull(tenure);

        _lQuillReferenceTenure = tenure;
    }

    public void LReferenceTitleSet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillReferenceTenure.LTenureRequestDefer(
            new LRequestReferenceTitle(_lQuillReferenceTenure.LTenureId, new LStateWritten(text)));
    }

    public void LReferenceYearSet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillReferenceTenure.LTenureRequestDefer(
            new LRequestReferenceYear(_lQuillReferenceTenure.LTenureId, new LStateWritten(text)));
    }

    public void LReferenceUrlSet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillReferenceTenure.LTenureRequestDefer(
            new LRequestReferenceUrl(_lQuillReferenceTenure.LTenureId, new LStateWritten(text)));
    }

    public void LReferenceNoteSet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillReferenceTenure.LTenureRequestDefer(
            new LRequestReferenceNote(_lQuillReferenceTenure.LTenureId, new LStateWritten(text)));
    }

    public void LReferenceKindSet(string? tag)
    {
        if (LReferenceClerk.LReferenceKindRead(_lQuillReferenceTenure.LTenureRead(), tag) is not LReferenceKind kind)
        {
            return;
        }

        _lQuillReferenceTenure.LTenureRequestApply(new LRequestReferenceKind(_lQuillReferenceTenure.LTenureId, kind));
    }

    public bool LReferenceAuthorAdd(string? name, int position, long former)
    {
        if (!LDraftClerkPanel.LAuthorNameCheck(name))
        {
            return false;
        }

        _lQuillReferenceTenure.LTenureRequestApply(
            new LRequestAuthorAddition(_lQuillReferenceTenure.LTenureId, name, position, former));
        return true;
    }

    public bool LReferenceAuthorInsert(long author, int position, long former)
    {
        if (!LDraftClerkList.LDraftFormerCheck(author, former))
        {
            return false;
        }

        _lQuillReferenceTenure.LTenureRequestApply(
            new LRequestAuthorPick(_lQuillReferenceTenure.LTenureId, author, position, former));
        return true;
    }

    public void LReferenceAuthorRemove(long author)
    {
        _lQuillReferenceTenure.LTenureRequestApply(
            new LRequestAuthorRemoval(_lQuillReferenceTenure.LTenureId, author));
    }

    public void LReferenceAuthorMove(long author, int position)
    {
        _lQuillReferenceTenure.LTenureRequestApply(
            new LRequestAuthorShift(_lQuillReferenceTenure.LTenureId, author, position));
    }
}
