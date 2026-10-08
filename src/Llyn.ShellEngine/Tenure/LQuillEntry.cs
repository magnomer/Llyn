using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LQuillEntry
{
    private readonly LTenure _lQuillEntryTenure;

    public LQuillEntry(LTenure tenure)
    {
        ArgumentNullException.ThrowIfNull(tenure);

        _lQuillEntryTenure = tenure;
    }

    public void LQuillHeadwordSet(string text)
    {
        _lQuillEntryTenure.LTenureRequestDefer(new LRequestHeadword(_lQuillEntryTenure.LTenureId, text));
    }

    public void LQuillNoteSet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillEntryTenure.LTenureRequestDefer(
            new LRequestNote(_lQuillEntryTenure.LTenureId, LDraftClerkEntry.LNoteResolve(text)));
    }

    public static bool LQuillNoteCheck(string text, string note)
    {
        ArgumentNullException.ThrowIfNull(text);

        return LDraftClerkEntry.LNoteCheck(text, note);
    }

    public void LQuillLanguageSet(string language)
    {
        ArgumentNullException.ThrowIfNull(language);

        if (language.Length > 0)
        {
            _lQuillEntryTenure.LTenureRequestApply(new LRequestLanguage(_lQuillEntryTenure.LTenureId, language));
        }
    }

    public LUnit LQuillUnitRead()
    {
        return _lQuillEntryTenure.LTenureRead()?.LDraftContent.LEntryDraftUnit ?? LUnit.LUnitEmpty;
    }

    public IReadOnlyList<LUnit> LQuillUnitScan()
    {
        return LUnitClerk.LUnitScan(
            _lQuillEntryTenure.LTenureEngine.LEngineLanguage.LEngineSpacedCheck(
                _lQuillEntryTenure.LTenureLanguageRead()));
    }

    public void LQuillUnitSet(LUnit unit)
    {
        _lQuillEntryTenure.LTenureRequestApply(new LRequestUnit(
            _lQuillEntryTenure.LTenureId, unit == LQuillUnitRead() ? LUnit.LUnitEmpty : unit));
    }
}
