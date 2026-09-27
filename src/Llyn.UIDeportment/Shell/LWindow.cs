using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LWindow : IDisposable
{
    private readonly LDraftPort _lDraftPort;

    private readonly LSettingsPort _lSettingsPort;

    private readonly LPhonologyPort _lPhonologyPort;

    private readonly LMediaPort _lMediaPort;

    private readonly LEntryPort _lEntryPort;

    private readonly LPortraitPort _lPortraitPort;

    private readonly LPosture _lPosture;

    private readonly List<Action> _lWindowVistas = [];

    internal LWindow(
        LPosture posture,
        LDraftPort drafts,
        LEntryPort entries,
        LSettingsPort settings,
        LPhonologyPort phonology,
        LMediaPort media,
        LPortraitPort portraits)
    {
        ArgumentNullException.ThrowIfNull(posture);
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(media);
        ArgumentNullException.ThrowIfNull(portraits);

        _lPosture = posture;
        _lDraftPort = drafts;
        _lEntryPort = entries;
        _lSettingsPort = settings;
        _lPhonologyPort = phonology;
        _lMediaPort = media;
        _lPortraitPort = portraits;
        LWindowWorkspace = new QWorkspace(this);
        LWindowForge = new QForge(this);
    }

    public QWorkspace LWindowWorkspace { get; }

    public QForge LWindowForge { get; }

    internal LDraftPort LWindowDraftPort => _lDraftPort;

    internal LEntryPort LWindowEntryPort => _lEntryPort;

    internal LPhonologyPort LWindowPhonologyPort => _lPhonologyPort;

    internal LSettingsPort LWindowSettingsPort => _lSettingsPort;

    internal LMediaPort LWindowMediaPort => _lMediaPort;

    internal LPortraitPort LWindowPortraitPort => _lPortraitPort;

    public void LWindowVistaRestore()
    {
        foreach (Action restore in _lWindowVistas)
        {
            restore();
        }
    }

    internal LWindowHeld LWindowVistaAdd<LWindowHeld>(LWindowHeld held, Action<LWindowHeld, LWindow> restore)
    {
        restore(held, this);
        _lWindowVistas.Add(() => restore(held, this));
        return held;
    }

    public CPostureState LWindowPostureRead()
    {
        return LFootprint.LFootprintPostureRead(_lPosture.LPostureRead());
    }

    public CLayout? LWindowLayoutRead(string tab)
    {
        return LFootprint.LFootprintLayoutRead(_lPosture.LPostureRead().LPostureStateLayout ?? [], tab);
    }

    public bool LWindowModeMatch(string? mode)
    {
        return _lPosture.LPostureModeMatch(mode);
    }

    internal LVista LWindowVistaStart(string tab, LSubject? subject, LCatalogOrder fallback, bool blank = false)
    {
        return _lPosture.LPostureVistaStart(tab, subject, fallback, blank);
    }

    public void LWindowStateDefer(CWindowState window, bool minimized, int delay)
    {
        _lPosture.LPostureWindowDefer(LFootprint.LFootprintStateRead(window), minimized, delay);
    }

    public void LWindowVolumeSet(double volume)
    {
        _lPosture.LPostureVolumeSet(volume);
        _lMediaPort.LEngineVolumeSet(volume);
    }

    public void LWindowVolumeSave()
    {
        _lPosture.LPostureVolumeSave();
    }

    public void LWindowModeSave(string mode)
    {
        _lPosture.LPostureModeSave(mode);
    }

    public bool LWindowLinkedSave(bool linked)
    {
        return _lPosture.LPostureLinkedSave(linked);
    }

    public void LWindowLayoutSave(IEnumerable<CLayout> layout)
    {
        _lPosture.LPostureLayoutSave(LFootprint.LFootprintLayoutRead(layout));
    }

    public void LWindowLayoutReset()
    {
        _lPosture.LPostureLayoutReset();
    }

    public void LWindowLeftoverSweep()
    {
        _lDraftPort.LEngineLeftoverSweep();
    }

    public void Dispose()
    {
        _lPosture.Dispose();
    }

    public Action LWindowObserverAttach(Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        return LWindowObserverAdd(LWindowBulletinSend);

        void LWindowBulletinSend(LBulletin bulletin)
        {
            observer(LWindowBulletinRead(bulletin));
        }
    }

    public Action LWindowObserverAttach(CSubject subject, Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        return LWindowObserverAdd(LWindowBulletinSend);

        void LWindowBulletinSend(LBulletin bulletin)
        {
            if (bulletin.LBulletinMatch(LPanel.LPanelSubjectRead(subject)))
            {
                observer(LWindowBulletinRead(bulletin));
            }
        }
    }

    private Action LWindowObserverAdd(Action<LBulletin> sent)
    {
        _lDraftPort.LEngineObserverAttach(sent);
        return () => _lDraftPort.LEngineObserverDetach(sent);
    }

    public CFont LWindowFontRead(string language, CFontRole role)
    {
        return LSounding.LSoundingFontRead(_lSettingsPort.LEngineFontRead(language, (LFontRole)role));
    }

    public bool LWindowRespellingCheck(string language)
    {
        return _lPhonologyPort.LEngineRespellingCheck(language);
    }

    public bool LWindowPhonemicCheck(string language)
    {
        return _lPhonologyPort.LEnginePhonemicCheck(language);
    }

    public IReadOnlyList<CReflexRule> LWindowReflexRead(string language)
    {
        return LSounding.LSoundingRuleRead(_lPhonologyPort.LEngineReflexRead(language));
    }

    public CSentenceOrder LWindowOrderRead(string language)
    {
        return LCard.LCardOrderRead(_lPhonologyPort.LEngineOrderRead(language));
    }

    public IReadOnlyList<CMentionLabel> LWindowMentionResolve(
        string text, IReadOnlyList<CMentionDraft> mentions, string silent)
    {
        ArgumentNullException.ThrowIfNull(mentions);

        List<LMentionDraft> drafts = new(mentions.Count);
        foreach (CMentionDraft mention in mentions)
        {
            drafts.Add(new LMentionDraft(
                mention.CMentionDraftId,
                mention.CMentionDraftOffset,
                mention.CMentionDraftLength,
                mention.CMentionDraftEntry,
                mention.CMentionDraftSense));
        }

        return LWindowLabelRead(_lEntryPort.LEngineMentionResolve(text, drafts), silent);
    }

    public IReadOnlyList<CMentionPiece> LWindowMentionDivide(string text, IReadOnlyList<CMention> mentions)
    {
        return LSplice.LSpliceBuild(
            _lDraftPort.LEngineMentionDivide(text, LWindowMentionRead(mentions)),
            static piece => new CMentionPiece(
                piece.LMentionPieceOffset,
                piece.LMentionPieceEnd,
                piece.LMentionPieceText,
                piece.LMentionPieceStored?.LMentionLinked));
    }

    public int LWindowUnitRead(string text, int offset)
    {
        return _lDraftPort.LEngineUnitRead(text, offset);
    }

    public int LWindowOffsetRead(string text, int unit)
    {
        return _lDraftPort.LEngineOffsetRead(text, unit);
    }

    public (int LWindowSpanOffset, int LWindowSpanLength) LWindowSpanRead(string text, int start, int length)
    {
        LMentionDraft span = _lDraftPort.LEngineSpanRead(text, start, length);
        return (span.LMentionDraftOffset, span.LMentionDraftLength);
    }

    public bool LWindowSpanCheck(string text, int start, int length)
    {
        return _lDraftPort.LEngineSpanCheck(text, start, length);
    }

    public bool LWindowAnchorMatch(IReadOnlyList<long> one, IReadOnlyList<long> other)
    {
        return _lDraftPort.LEngineAnchorMatch(one, other);
    }

    internal bool LWindowAnchorCheck(IReadOnlyList<LFanqieRow> rows, string headword)
    {
        return _lDraftPort.LEngineAnchorCheck(rows, headword);
    }

    internal string LWindowAnchorFormat(
        IReadOnlyList<LFanqieRow> rows, IReadOnlyList<long> anchors, string headword, string separator)
    {
        return _lDraftPort.LEngineAnchorFormat(rows, anchors, headword, separator);
    }

    public IReadOnlyList<CMarkdownBlock> LWindowMarkdownParse(string? text)
    {
        return LSplice.LSpliceBuild(_lEntryPort.LEngineMarkdownParse(text), LWindowMarkdownRead);
    }

    public IReadOnlyList<string> LWindowSchemeRead(string language)
    {
        return _lPhonologyPort.LEngineSchemeRead(language);
    }

    public IReadOnlyList<CSpeechValue> LWindowSpeechRead(string language)
    {
        return LSplice.LSpliceBuild(
            _lPhonologyPort.LEngineSpeechRead(language),
            static value => new CSpeechValue(value.LSpeechValueId, value.LSpeechValueName));
    }

    public CSpeechValue? LWindowSpeechAdd(string language, string name)
    {
        return LSounding.LSoundingSpeechRead(_lPhonologyPort.LEngineSpeechAdd(language, name));
    }

    public CGlyph? LWindowGlyphRead(string language)
    {
        return LSounding.LSoundingGlyphRead(_lEntryPort.LEngineGlyphRead(language));
    }

    public IReadOnlyList<string> LWindowLocalizationScan()
    {
        return _lSettingsPort.LEngineLocalizationScan();
    }

    private static CBulletin LWindowBulletinRead(LBulletin bulletin)
    {
        return new CBulletin(bulletin.LBulletinId, bulletin.LBulletinStored);
    }

    private static IReadOnlyList<CMentionLabel> LWindowLabelRead(IReadOnlyList<LMentionLabel> labels, string silent)
    {
        List<CMentionLabel> read = new(labels.Count);
        foreach (LMentionLabel label in labels)
        {
            read.Add(new CMentionLabel(
                label.LMentionLabelId,
                label.LMentionLabelWord,
                label.LMentionLabelLinked ? label.LMentionLabelName : silent,
                label.LMentionLabelSense));
        }

        return read;
    }

    internal static CMarkdownBlock LWindowMarkdownRead(LMarkdownBlock block)
    {
        return new CMarkdownBlock(
            block.LMarkdownBlockHeaded,
            block.LMarkdownBlockListed,
            block.LMarkdownBlockQuoted,
            block.LMarkdownBlockFenced,
            block.LMarkdownBlockRuled,
            block.LMarkdownBlockLevel,
            block.LMarkdownBlockMark,
            block.LMarkdownBlockText,
            LSplice.LSpliceBuild(
                block.LMarkdownBlockSpan,
                static span => new CMarkdownSpan(
                    span.LMarkdownSpanText,
                    span.LMarkdownSpanBold,
                    span.LMarkdownSpanItalic,
                    span.LMarkdownSpanCode,
                    span.LMarkdownSpanAddress)));
    }

    internal static IReadOnlyList<CMention> LWindowMentionRead(IReadOnlyList<LMention> mentions)
    {
        return LSplice.LSpliceBuild(
            mentions,
            static mention => new CMention(
                mention.LMentionId,
                mention.LMentionOffset,
                mention.LMentionLength,
                mention.LMentionEntryId,
                mention.LMentionSenseId));
    }

    internal static IReadOnlyList<LMention> LWindowMentionRead(IReadOnlyList<CMention> mentions)
    {
        return LSplice.LSpliceBuild(
            mentions,
            static mention => new LMention(
                mention.CMentionId,
                mention.CMentionOffset,
                mention.CMentionLength,
                mention.CMentionEntry,
                mention.CMentionSense));
    }
}
