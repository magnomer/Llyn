using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LQuillReflex
{
    private readonly LTenure _lQuillReflexTenure;

    private readonly LPhonologyPort _lQuillReflexPhonology;

    public LQuillReflex(LTenure tenure, LPhonologyPort phonology)
    {
        ArgumentNullException.ThrowIfNull(tenure);
        ArgumentNullException.ThrowIfNull(phonology);

        _lQuillReflexTenure = tenure;
        _lQuillReflexPhonology = phonology;
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

    private bool LQuillRespellingCheck(long reflex)
    {
        return _lQuillReflexTenure.LTenureRead()?.LDraftContent is LEntryDraft content
            && LDraftClerkReflex.LReflexFind(content, reflex) is LReflexDraft row
            && _lQuillReflexPhonology
                .LEngineGuiseRead(content.LEntryDraftLanguage, [row.LReflexDraftLanguage])[0]
                .LReflexGuiseRespelled;
    }
}
