using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LQuillReflex
{
    private readonly LTenure _lQuillReflexTenure;

    private readonly LReflexPort _lQuillReflexPort;

    public LQuillReflex(LTenure tenure, LReflexPort reflexes)
    {
        ArgumentNullException.ThrowIfNull(tenure);
        ArgumentNullException.ThrowIfNull(reflexes);

        _lQuillReflexTenure = tenure;
        _lQuillReflexPort = reflexes;
    }

    public void LQuillReflexAdd(long reflex)
    {
        if (_lQuillReflexTenure.LTenureRead()?.LDraftContent is not LEntryDraft content)
        {
            return;
        }

        LReflexDraft blank = LDraftClerkReflex.LReflexDefaultRead(content, reflex);
        _lQuillReflexTenure.LTenureRequestApply(new LRequestReflexAddition(
            _lQuillReflexTenure.LTenureId,
            blank.LReflexDraftLanguage,
            blank.LReflexDraftKind,
            LDraftClerkReflex.LReflexPositionRead(content, reflex)));
    }

    public void LQuillReflexRemove(long reflex)
    {
        _lQuillReflexTenure.LTenureRequestApply(new LRequestReflexRemoval(_lQuillReflexTenure.LTenureId, reflex));
    }

    public void LQuillReflexToggle(long reflex)
    {
        if (_lQuillReflexTenure.LTenureRead()?.LDraftContent is LEntryDraft content
            && LDraftClerkReflex.LReflexFind(content, reflex) is LReflexDraft row)
        {
            _lQuillReflexTenure.LTenureRequestApply(
                new LRequestReflexMain(_lQuillReflexTenure.LTenureId, reflex, !row.LReflexDraftMain));
        }
    }

    public IReadOnlyList<LReflexDraft> LQuillLanguageSet(long reflex, string language)
    {
        ArgumentNullException.ThrowIfNull(language);

        IReadOnlyList<LReflexDraft> held = _lQuillReflexTenure.LTenureRead()?.LDraftContent.LEntryDraftReflexes ?? [];
        _lQuillReflexTenure.LTenureRequestDefer(
            new LRequestReflexLanguage(_lQuillReflexTenure.LTenureId, reflex, language));
        return held
            .Select(row => row.LReflexDraftId == reflex ? row with { LReflexDraftLanguage = language } : row)
            .ToList();
    }

    public void LQuillKindSet(long reflex, string kind)
    {
        ArgumentNullException.ThrowIfNull(kind);

        _lQuillReflexTenure.LTenureRequestDefer(new LRequestReflexKind(_lQuillReflexTenure.LTenureId, reflex, kind));
    }

    public void LQuillTextSet(long reflex, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        long id = _lQuillReflexTenure.LTenureId;
        _lQuillReflexTenure.LTenureRequestDefer(LQuillRespellingCheck(reflex)
            ? new LRequestReflexRespelling(id, reflex, text)
            : new LRequestReflexText(id, reflex, text));
    }

    public void LQuillRomanizationSet(long reflex, string romanization)
    {
        ArgumentNullException.ThrowIfNull(romanization);

        _lQuillReflexTenure.LTenureRequestDefer(
            new LRequestReflexRomanization(_lQuillReflexTenure.LTenureId, reflex, romanization));
    }

    public void LQuillMeaningSet(long reflex, string meaning)
    {
        ArgumentNullException.ThrowIfNull(meaning);

        _lQuillReflexTenure.LTenureRequestDefer(
            new LRequestReflexMeaning(_lQuillReflexTenure.LTenureId, reflex, meaning));
    }

    public void LQuillNoteSet(long reflex, string note)
    {
        ArgumentNullException.ThrowIfNull(note);

        _lQuillReflexTenure.LTenureRequestDefer(new LRequestReflexNote(_lQuillReflexTenure.LTenureId, reflex, note));
    }

    public void LReflexAnchorSet(long reflex, long fanqie, bool anchored)
    {
        _lQuillReflexTenure.LTenureRequestApply(
            new LRequestReflexAnchor(_lQuillReflexTenure.LTenureId, reflex, fanqie, anchored));
    }

    public bool LQuillReflexCheck()
    {
        return _lQuillReflexTenure.LTenureEngine.LEngineReflex.LEngineReflexRead(
                _lQuillReflexTenure.LTenureLanguageRead()).Count > 0
            || _lQuillReflexTenure.LTenureRead()?.LDraftContent.LEntryDraftReflected == true;
    }

    public void LQuillReflexStart()
    {
        if (_lQuillReflexTenure.LTenureRead() is { LDraftStored: long stored } held
            && !held.LDraftContent.LEntryDraftReflected)
        {
            _lQuillReflexTenure.LTenureEngine.LEngineReflex.LEngineReflexStart(stored);
        }
    }

    public IReadOnlyList<LAnchorRow> LQuillAnchorScan(long reflex)
    {
        LDraft? held = _lQuillReflexTenure.LTenureRead();
        if (held?.LDraftStored is not long stored)
        {
            return [];
        }

        try
        {
            return LDraftClerkReflex.LReflexFind(held.LDraftContent, reflex) is LReflexDraft row
                ? _lQuillReflexTenure.LTenureEngine.LEngineReflex.LEngineAnchorScan(
                    stored,
                    row.LReflexDraftAnchors,
                    held.LDraftContent.LEntryDraftLanguage,
                    row.LReflexDraftLanguage,
                    row.LReflexDraftAnatomy.LAnatomyToneIpa)
                : [];
        }
        catch (Exception exception) when (LWorkspaceClerk.LWorkspaceRefusedCheck(exception))
        {
            return [];
        }
    }

    private bool LQuillRespellingCheck(long reflex)
    {
        return _lQuillReflexTenure.LTenureRead()?.LDraftContent is LEntryDraft content
            && LDraftClerkReflex.LReflexFind(content, reflex) is LReflexDraft row
            && _lQuillReflexPort
                .LEngineGuiseRead(content.LEntryDraftLanguage, [row.LReflexDraftLanguage])[0]
                .LReflexGuiseRespelled;
    }
}
