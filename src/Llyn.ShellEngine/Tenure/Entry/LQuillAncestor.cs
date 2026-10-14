using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LQuillAncestor
{
    private readonly LTenure _lQuillAncestorTenure;

    private readonly LAncestorPort _lQuillAncestorPort;

    public LQuillAncestor(LTenure tenure, LAncestorPort ancestors)
    {
        ArgumentNullException.ThrowIfNull(tenure);
        ArgumentNullException.ThrowIfNull(ancestors);

        _lQuillAncestorTenure = tenure;
        _lQuillAncestorPort = ancestors;
    }

    public IReadOnlyList<LLineage> LQuillLineageRead()
    {
        if (_lQuillAncestorTenure.LTenureRead() is not LDraft held)
        {
            return [];
        }

        return _lQuillAncestorPort.LEngineLineageRead(
            held.LDraftStored ?? 0, held.LDraftContent.LEntryDraftAncestors);
    }

    public void LQuillTextSet(long ancestor, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        LRequest request = new LRequestAncestorText(_lQuillAncestorTenure.LTenureId, ancestor, text);
        if (LQuillAncestorFind(ancestor)?.LAncestorDraftTarget is not null)
        {
            _lQuillAncestorTenure.LTenureRequestApply(request);
            return;
        }

        _lQuillAncestorTenure.LTenureRequestDefer(request);
    }

    public bool LQuillTargetSet(long ancestor, long entry)
    {
        bool sourced = LQuillSourceCheck(ancestor);
        _lQuillAncestorTenure.LTenureRequestApply(
            new LRequestAncestorTarget(_lQuillAncestorTenure.LTenureId, ancestor, entry));
        return sourced && LQuillAncestorFind(ancestor)?.LAncestorDraftTarget == entry;
    }

    public void LQuillLanguageSet(long ancestor, string language)
    {
        ArgumentNullException.ThrowIfNull(language);

        string written = language.Trim();
        long tongue = _lQuillAncestorPort.LEngineLanguageRead()
            .FirstOrDefault(held => string.Equals(
                held.LAncestorLanguageName, written, StringComparison.OrdinalIgnoreCase))
            ?.LAncestorLanguageTongue ?? 0;
        _lQuillAncestorTenure.LTenureRequestApply(new LRequestAncestorLanguage(
            _lQuillAncestorTenure.LTenureId, ancestor, tongue == 0 ? written : string.Empty, tongue));
    }

    public void LQuillGlossSet(long ancestor, string gloss)
    {
        ArgumentNullException.ThrowIfNull(gloss);

        _lQuillAncestorTenure.LTenureRequestDefer(
            new LRequestAncestorGloss(_lQuillAncestorTenure.LTenureId, ancestor, gloss));
    }

    public void LQuillRelationSet(long ancestor, long relation)
    {
        _lQuillAncestorTenure.LTenureRequestApply(
            new LRequestAncestorRelation(_lQuillAncestorTenure.LTenureId, ancestor, relation));
    }

    public void LQuillAncestorInsert(long ancestor)
    {
        _lQuillAncestorTenure.LTenureRequestApply(
            new LRequestAncestorInsertion(_lQuillAncestorTenure.LTenureId, ancestor));
    }

    public void LQuillAncestorAdd(long ancestor)
    {
        _lQuillAncestorTenure.LTenureRequestApply(
            new LRequestAncestorAddition(_lQuillAncestorTenure.LTenureId, ancestor));
    }

    public void LQuillAncestorRemove(long ancestor)
    {
        _lQuillAncestorTenure.LTenureRequestApply(
            new LRequestAncestorRemoval(_lQuillAncestorTenure.LTenureId, ancestor));
    }

    public void LQuillAncestorRestore(long ancestor)
    {
        _lQuillAncestorTenure.LTenureRequestApply(
            new LRequestAncestorRestoration(_lQuillAncestorTenure.LTenureId, ancestor));
    }

    private bool LQuillSourceCheck(long ancestor)
    {
        return _lQuillAncestorTenure.LTenureRead()?.LDraftContent.LEntryDraftAncestors
            .Any(box => box.LAncestorDraftChild == ancestor) == true;
    }

    private LAncestorDraft? LQuillAncestorFind(long ancestor)
    {
        return _lQuillAncestorTenure.LTenureRead()?.LDraftContent.LEntryDraftAncestors
            .FirstOrDefault(box => box.LAncestorDraftId == ancestor);
    }
}
