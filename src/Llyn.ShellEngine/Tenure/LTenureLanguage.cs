using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LTenure
{
    public bool LTenureFlaggedCheck()
    {
        string language = LTenureLanguageRead();
        return language.Length > 0 && _lEngine.LEngineLanguage.LEngineFlaggedCheck(language);
    }

    public IReadOnlyList<LVariety> LTenureVarietyRead()
    {
        string language = LTenureLanguageRead();
        return language.Length == 0 ? [] : _lEngine.LEngineLanguage.LEngineVarietyRead(language);
    }

    public IReadOnlyList<string> LTenureVarietyNames =>
        LTenureVarietyRead().Select(static variety => variety.LVarietyName).ToList();

    public bool LTenureReflexCheck()
    {
        return _lEngine.LEngineReflex.LEngineReflexRead(LTenureLanguageRead()).Count > 0
            || LTenureRead()?.LDraftContent.LEntryDraftReflected == true;
    }

    public string LTenurePronunciationRead()
    {
        return LTenureRead()?.LDraftContent is LEntryDraft draft
            ? _lEngine.LEngineSettings.LEnginePronunciationRead(draft)
            : string.Empty;
    }

    public void LTenurePronunciationSet(string text)
    {
        if (_lEngine.LEngineSettings.LEngineRespellingCheck(LTenureLanguageRead()))
        {
            LTenureRespellingSet(text);
            return;
        }

        LTenureIpaSet(text);
    }

    public void LTenureIpaSet(string text)
    {
        LTenureRequestDefer(new LRequestIpa(LTenureId, text));
    }

    public void LTenureRespellingSet(string text)
    {
        LTenureRequestDefer(new LRequestRespelling(LTenureId, text));
    }

    public void LTenureVarietySet(long pronunciation, string variety)
    {
        ArgumentNullException.ThrowIfNull(variety);

        if (pronunciation == 0 || variety.Length == 0)
        {
            return;
        }

        LTenureRequestApply(new LRequestPronunciationVariety(LTenureId, pronunciation, variety));
    }

    public IReadOnlyList<LTranslationTarget> LTenureEtymonRead()
    {
        if (LTenureRead()?.LDraftContent is not LEntryDraft draft)
        {
            return [];
        }

        try
        {
            return _lEngine.LEngineCard.LEngineEtymonRead(draft);
        }
        catch (Exception)
        {
            return [];
        }
    }

    public IReadOnlyDictionary<long, LTranslationTarget> LTenureTargetRead()
    {
        try
        {
            return _lEngine.LEngineCard.LEngineTargetFind(LTenureId);
        }
        catch (Exception)
        {
            return new Dictionary<long, LTranslationTarget>();
        }
    }
}
