using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CSounding
{
    private readonly CDesk _cSoundingDesk;

    private readonly LFanqiePort _cSoundingFanqiePort;

    private readonly LDiweiPort _cSoundingDiweiPort;

    private readonly LScriptPort _cSoundingScriptPort;

    private readonly LParadigmPort _cSoundingParadigmPort;

    private readonly CEnvoy _cSoundingEnvoy;

    private readonly LSettingsPort _cSoundingSettingsPort;

    private readonly LDisplaySound _cSoundingVoice;

    private readonly CLedgerNoticed _cSoundingNoticed;

    internal CSounding(
        CDesk desk,
        LFanqiePort fanqies,
        LDiweiPort diweis,
        LScriptPort scripts,
        LParadigmPort paradigms,
        LSettingsPort settings,
        LDisplay display,
        CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(fanqies);
        ArgumentNullException.ThrowIfNull(diweis);
        ArgumentNullException.ThrowIfNull(scripts);
        ArgumentNullException.ThrowIfNull(paradigms);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(envoy);

        _cSoundingDesk = desk;
        _cSoundingFanqiePort = fanqies;
        _cSoundingDiweiPort = diweis;
        _cSoundingScriptPort = scripts;
        _cSoundingParadigmPort = paradigms;
        _cSoundingEnvoy = envoy;
        _cSoundingSettingsPort = settings;
        _cSoundingVoice = display.LDisplaySound;
        _cSoundingNoticed = display.LDisplayNoticed;
    }

    public event Action? CSoundingChanged;

    internal event Action<string, string, string>? LSoundingDiweiChosen;

    internal void LSoundingObserverAttach(Action<Action> marshal)
    {
        _cSoundingDesk.CDeskVigil.LVigilObserverAttach(
            CSubject.CSubjectFanqie, _ => marshal(() => CSoundingChanged?.Invoke()));
    }

    private long? LSoundingEntry => _cSoundingDesk.CDeskStoredRead();

    private string LSoundingLanguage =>
        _cSoundingDesk.CDeskDraft.CDeskDraftTenure?.LTenureLanguageRead() ?? string.Empty;

    public CSoundingFanqie CSoundingFanqieRead()
    {
        long? entry = LSoundingEntry;
        return new CSoundingFanqie(
            CSoundingFanqieRead(LSoundingListRead(
                _cSoundingFanqiePort.LEngineFanqieRead, "Display.FanqieReadFailed")),
            _cSoundingVoice.LDisplayFanqieCheck(entry),
            entry is not null
            && _cSoundingNoticed.LLedgerRepaintRead(_cSoundingEnvoy, _cSoundingSettingsPort,
                () => _cSoundingFanqiePort.LEngineBookCheck(LSoundingLanguage), false, "Display.BookFailed"),
            CFont.CFontRead(_cSoundingSettingsPort, LSoundingLanguage, CFontRole.CFontRoleGlyph));
    }

    public string CSoundingReadingRead(string headword)
    {
        return LSoundingEntry is long id
            ? _cSoundingNoticed.LLedgerRepaintRead(_cSoundingEnvoy, _cSoundingSettingsPort,
                () => _cSoundingFanqiePort.LEngineReadingRead(id, headword), string.Empty, "Display.ReadingFailed")
            : string.Empty;
    }

    public void CSoundingFanqieResolve()
    {
        LSoundingMarkSend(_cSoundingFanqiePort.LEngineFanqieRebuild, "Display.FanqieRebuildFailed");
    }

    public void CSoundingFanqieSet(long fanqieId, int rank, bool raise)
    {
        LSoundingMarkSend(
            id => _cSoundingFanqiePort.LEngineFanqieSet(id, fanqieId, rank, raise),
            "Display.FanqieRepresentativeFailed");
    }

    public void CSoundingDiweiOpen(bool initial, string key)
    {
        if (_cSoundingDiweiPort.LEngineDiweiRead(initial, key) is string kind)
        {
            LSoundingDiweiChosen?.Invoke(LSoundingLanguage, kind, key);
        }
    }

    public CSoundingScript CSoundingScriptRead()
    {
        long? entry = LSoundingEntry;
        return new CSoundingScript(
            CSoundingScriptRead(
                LSoundingListRead(_cSoundingScriptPort.LEngineScriptRead, "Display.ScriptReadFailed"),
                _cSoundingSettingsPort),
            _cSoundingVoice.LDisplayScriptCheck(entry),
            entry is not null
            && _cSoundingNoticed.LLedgerRepaintRead(_cSoundingEnvoy, _cSoundingSettingsPort,
                () => _cSoundingScriptPort.LEngineStyleCheck(LSoundingLanguage), false, "Display.StyleFailed"),
            CFont.CFontRead(_cSoundingSettingsPort, LSoundingLanguage, CFontRole.CFontRoleGlyph));
    }

    public void CSoundingScriptResolve()
    {
        LSoundingMarkSend(_cSoundingScriptPort.LEngineScriptRebuild, "Display.ScriptRebuildFailed");
    }

    public CLecternParadigm CSoundingParadigmRead()
    {
        string language = LSoundingEntry is long id
            ? _cSoundingNoticed.LLedgerRepaintRead(_cSoundingEnvoy, _cSoundingSettingsPort,
                () => _cSoundingParadigmPort.LEngineLanguageResolve(id), string.Empty, "Display.LanguageFailed")
            : string.Empty;
        IReadOnlyList<LParadigmRow> rows =
            LSoundingListRead(_cSoundingParadigmPort.LEngineParadigmScan, "Display.ParadigmReadFailed");
        bool pending = _cSoundingVoice.LDisplayParadigmCheck(LSoundingEntry);
        bool enabled = _cSoundingVoice.LDisplayMorphologyRead();
        LParadigmView? view = LSoundingEntry is long entry
            ? _cSoundingNoticed.LLedgerRepaintRead<LParadigmView?>(_cSoundingEnvoy, _cSoundingSettingsPort,
                () => _cSoundingParadigmPort.LEngineInflectionRead(entry, pending, enabled),
                null, "Display.ParadigmReadFailed")
            : null;
        return new CLecternParadigm(
            CSoundingParadigmRead(_cSoundingParadigmPort, rows, pending, enabled, true),
            CFont.CFontRead(_cSoundingSettingsPort, language, CFontRole.CFontRoleHeadword),
            CParadigmView.CParadigmViewCreate(view, true));
    }

    private IReadOnlyList<LSoundingItem> LSoundingListRead<LSoundingItem>(
        Func<long, IReadOnlyList<LSoundingItem>> read, string key)
    {
        return LSoundingEntry is long id
            ? _cSoundingNoticed.LLedgerRepaintRead(_cSoundingEnvoy, _cSoundingSettingsPort, () => read(id), [], key)
            : [];
    }

    private void LSoundingMarkSend(Action<long> mark, string key)
    {
        if (LSoundingEntry is not long id)
        {
            return;
        }

        try
        {
            mark(id);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cSoundingEnvoy, _cSoundingSettingsPort, key, exception);
            return;
        }

        CSoundingChanged?.Invoke();
    }

    internal static IReadOnlyList<CFanqieGroup> CSoundingFanqieRead(IReadOnlyList<LFanqieGroup> groups)
    {
        ArgumentNullException.ThrowIfNull(groups);

        return groups
            .Select(static group => new CFanqieGroup(
                group.LFanqieGroupHeading,
                group.LFanqieGroupLabel,
                group.LFanqieGroupSource,
                group.LFanqieGroupStems,
                group.LFanqieGroupRows.Select(LSoundingRowRead).ToList()))
            .ToList();
    }

    private static CFanqieRow LSoundingRowRead(LFanqieRow row)
    {
        return new CFanqieRow(
            row.LFanqieRowId,
            row.LFanqieRowRepresentative,
            row.LFanqieRowMarked,
            row.LFanqieRowPrimary,
            row.LFanqieRowOrder,
            row.LFanqieRowClosed,
            row.LFanqieRowSlashed,
            row.LFanqieRowLabel,
            row.LFanqieRowInitial,
            row.LFanqieRowCell,
            row.LFanqieRowBracketed,
            row.LFanqieRowKnotted,
            row.LFanqieRowMedial,
            row.LFanqieRowGraded,
            row.LFanqieRowTone,
            row.LFanqieRowSpelling,
            row.LFanqieRowRemainder);
    }

    internal static IReadOnlyList<CScriptGroup> CSoundingScriptRead(
        IReadOnlyList<LScriptGroup> groups, LSettingsPort settings)
    {
        ArgumentNullException.ThrowIfNull(groups);
        ArgumentNullException.ThrowIfNull(settings);

        return groups
            .Select(group => new CScriptGroup(
                group.LScriptGroupHeading,
                group.LScriptGroupStyle,
                group.LScriptGroupGloss,
                group.LScriptGroupImages.Select(image => LSoundingImageRead(image, settings)).ToList()))
            .ToList();
    }

    private static CScriptImage LSoundingImageRead(LScriptImage image, LSettingsPort settings)
    {
        string epoch = "Epoch." + image.LScriptImageEpoch;
        return new CScriptImage(
            image.LScriptImageData,
            image.LScriptImageCaption,
            image.LScriptImageEpoch.Length > 0 && settings.LEngineTextFind(epoch) is not null ? epoch : string.Empty);
    }

    internal static IReadOnlyList<CParadigmSlot> CSoundingParadigmRead(
        LParadigmPort paradigms, IReadOnlyList<LParadigmRow> rows, bool pending, bool enabled, bool held)
    {
        ArgumentNullException.ThrowIfNull(paradigms);
        ArgumentNullException.ThrowIfNull(rows);

        return rows
            .Select(row => LSoundingSlotRead(row, paradigms.LEngineParadigmCheck(row, pending, enabled), held))
            .ToList();
    }

    private static CParadigmSlot LSoundingSlotRead(LParadigmRow row, LParadigmStatus status, bool held)
    {
        string text = row.LParadigmRowFirst.LParadigmSlotInflection?.LInflectionText ?? string.Empty;
        CParadigmForm shown = CParadigmForm.CParadigmFormResolve(status, text, held);
        return new CParadigmSlot(
            row.LParadigmRowPart, row.LParadigmRowName, shown.CParadigmFormText, shown.CParadigmFormTip);
    }
}
