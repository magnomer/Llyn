using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
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

    public LWindow(
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
    }

    public LEditor LWindowEditorCreate(Func<bool> unreadableSeam)
    {
        return new LEditor(_lDraftPort, _lEntryPort, _lPhonologyPort, _lSettingsPort, _lMediaPort, unreadableSeam);
    }

    public LCorpus LWindowCorpusCreate(
        LEditor editor, Func<bool> shownSeam, Func<Func<bool, bool>, bool> leaveSeam,
        Func<int, bool> removalSeam, Func<bool> unreadableSeam)
    {
        return new LCorpus(
            _lDraftPort, _lEntryPort, _lPortraitPort, _lSettingsPort, editor,
            shownSeam, leaveSeam, removalSeam, unreadableSeam);
    }

    public LFavorite LWindowFavoriteCreate(
        LEditor editor, Func<bool> shownSeam, Func<bool> leaveSeam, Func<bool> deleteSeam)
    {
        return new LFavorite(
            _lEntryPort, _lPortraitPort, _lSettingsPort, editor, shownSeam, leaveSeam, deleteSeam);
    }

    public LGuild LWindowGuildCreate(
        Func<bool> shownSeam,
        Func<bool> leaveSeam,
        Func<int, bool> removalSeam,
        Func<string, string, bool> unionSeam,
        Func<bool> unreadableSeam)
    {
        return new LGuild(
            _lDraftPort, _lEntryPort, _lPortraitPort, _lSettingsPort,
            shownSeam, leaveSeam, removalSeam, unionSeam, unreadableSeam);
    }

    public LLibrary LWindowLibraryCreate(
        LEditor editor, Func<bool> shownSeam, Func<bool> leaveSeam, Func<bool> deleteSeam)
    {
        return new LLibrary(_lEntryPort, _lPortraitPort, editor, shownSeam, leaveSeam, deleteSeam);
    }

    public LPhonology LWindowPhonologyCreate(
        LEditor editor, Func<bool> shownSeam, Func<bool> leaveSeam, Func<bool> deleteSeam)
    {
        return new LPhonology(_lPhonologyPort, _lPortraitPort, editor, shownSeam, leaveSeam, deleteSeam);
    }

    public LShelf LWindowShelfCreate(
        LEditor editor,
        Func<bool> shownSeam,
        Func<bool> leaveSeam,
        Func<int, bool> removalSeam,
        Func<bool> unreadableSeam)
    {
        return new LShelf(
            _lDraftPort, _lEntryPort, _lPortraitPort, _lSettingsPort,
            editor, shownSeam, leaveSeam, removalSeam, unreadableSeam);
    }

    public LRepertoire LWindowRepertoireCreate(
        LEditor editor, Func<bool> shownSeam, Func<Func<bool, bool>, bool> leaveSeam,
        Func<int, bool> removalSeam, Func<bool> unreadableSeam)
    {
        return new LRepertoire(
            _lDraftPort, _lEntryPort, _lPortraitPort, _lSettingsPort, editor,
            shownSeam, leaveSeam, removalSeam, unreadableSeam);
    }

    public LTaxonomy LWindowTaxonomyCreate(
        LEditor editor, Func<bool> shownSeam, Func<bool> leaveSeam, Func<bool> deleteSeam)
    {
        return new LTaxonomy(
            _lEntryPort, _lPortraitPort, _lSettingsPort, editor, shownSeam, leaveSeam, deleteSeam);
    }

    public LTenor LWindowTenorCreate(
        LEditor editor, Func<bool> shownSeam, Func<bool> leaveSeam, Func<bool> deleteSeam)
    {
        return new LTenor(
            _lEntryPort, _lPortraitPort, _lSettingsPort, editor, shownSeam, leaveSeam, deleteSeam);
    }

    public LWing LWindowWingCreate()
    {
        return new LWing(_lEntryPort, _lPhonologyPort, _lSettingsPort, _lMediaPort);
    }

    public LXiesheng LWindowXieshengCreate(
        LEditor editor, Func<bool> shownSeam, Func<bool> leaveSeam, Func<bool> deleteSeam)
    {
        return new LXiesheng(_lPhonologyPort, _lPortraitPort, editor, shownSeam, leaveSeam, deleteSeam);
    }

    public LYunjing LWindowYunjingCreate(
        LEditor editor, Func<bool> shownSeam, Func<bool> leaveSeam, Func<bool> deleteSeam)
    {
        return new LYunjing(
            _lPhonologyPort, _lPortraitPort, _lSettingsPort, editor, shownSeam, leaveSeam, deleteSeam);
    }

    public LPostureState LWindowPostureRead()
    {
        return _lPosture.LPostureRead();
    }

    public bool LWindowModeMatch(string? mode)
    {
        return _lPosture.LPostureModeMatch(mode);
    }

    public bool LWindowVolumeMatch(double volume)
    {
        return _lPosture.LPostureVolumeMatch(volume);
    }

    public LVista LWindowVistaStart(string tab, LSubject? subject, LCatalogOrder fallback, bool blank = false)
    {
        return _lPosture.LPostureVistaStart(tab, subject, fallback, blank);
    }

    public void LWindowStateDefer(LWindowState window, bool minimized, int delay)
    {
        _lPosture.LPostureWindowDefer(window, minimized, delay);
    }

    public void LWindowVolumeSave(double volume)
    {
        _lPosture.LPostureVolumeSave(volume);
    }

    public void LWindowModeSave(string mode)
    {
        _lPosture.LPostureModeSave(mode);
    }

    public bool LWindowLinkedSave(bool linked)
    {
        return _lPosture.LPostureLinkedSave(linked);
    }

    public void LWindowLayoutSave(IEnumerable<LLayout> layout)
    {
        _lPosture.LPostureLayoutSave(layout);
    }

    public void LWindowLayoutReset()
    {
        _lPosture.LPostureLayoutReset();
    }

    public void Dispose()
    {
        _lPosture.Dispose();
    }

    public void LWindowObserverAttach(Action<LBulletin> observer)
    {
        _lDraftPort.LEngineObserverAttach(observer);
    }

    public void LWindowObserverDetach(Action<LBulletin> observer)
    {
        _lDraftPort.LEngineObserverDetach(observer);
    }

    public LFont LWindowFontRead(string language, LFontRole role)
    {
        return _lSettingsPort.LEngineFontRead(language, role);
    }

    public Task<IReadOnlyList<LEnsignRow>> LWindowEnsignLoad()
    {
        return _lSettingsPort.LEngineEnsignLoad();
    }

    public Task<IReadOnlyList<LEnsignRow>> LWindowEnsignLoad(string language, IEnumerable<string> varieties)
    {
        return _lSettingsPort.LEngineEnsignLoad(language, varieties);
    }

    public void LWindowEnsignDelete(string path)
    {
        _lSettingsPort.LEngineEnsignDelete(path);
    }

    public bool LWindowRespellingCheck(string language)
    {
        return _lPhonologyPort.LEngineRespellingCheck(language);
    }

    public bool LWindowPhonemicCheck(string language)
    {
        return _lPhonologyPort.LEnginePhonemicCheck(language);
    }

    public IReadOnlyList<LReflexRule> LWindowReflexRead(string language)
    {
        return _lPhonologyPort.LEngineReflexRead(language);
    }

    public Uri? LWindowLocationRead(string? location)
    {
        return _lMediaPort.LEngineLocationRead(location);
    }

    public bool LWindowRecordingExist(string? file)
    {
        return _lMediaPort.LEngineRecordingExist(file);
    }

    public LSentenceOrder LWindowOrderRead(string language)
    {
        return _lPhonologyPort.LEngineOrderRead(language);
    }

    public IReadOnlyList<LMentionLabel> LWindowMentionResolve(string text, IReadOnlyList<LMentionDraft> mentions)
    {
        return _lEntryPort.LEngineMentionResolve(text, mentions);
    }

    public IReadOnlyList<LMentionPiece> LWindowMentionDivide(string text, IReadOnlyList<LMention> mentions)
    {
        return _lDraftPort.LEngineMentionDivide(text, mentions);
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

    public bool LWindowAnchorMatch(IReadOnlyList<long> one, IReadOnlyList<long> other)
    {
        return _lDraftPort.LEngineAnchorMatch(one, other);
    }

    public IReadOnlyList<LAnchorRow> LWindowAnchorScan(
        IReadOnlyList<LFanqieRow> rows, IReadOnlyList<long> anchors, string language, string reflex, string tone)
    {
        return _lDraftPort.LEngineAnchorScan(rows, anchors, language, reflex, tone);
    }

    public bool LWindowAnchorCheck(IReadOnlyList<LFanqieRow> rows, string headword)
    {
        return _lDraftPort.LEngineAnchorCheck(rows, headword);
    }

    public string LWindowAnchorFormat(
        IReadOnlyList<LFanqieRow> rows, IReadOnlyList<long> anchors, string headword, string separator)
    {
        return _lDraftPort.LEngineAnchorFormat(rows, anchors, headword, separator);
    }

    public IReadOnlyList<LMarkdownBlock> LWindowMarkdownParse(string? text)
    {
        return _lEntryPort.LEngineMarkdownParse(text);
    }

    public void LWindowLocationOpen(string target)
    {
        _lMediaPort.LEngineLocationOpen(target);
    }

    public IReadOnlyList<string> LWindowLanguageRead()
    {
        return _lSettingsPort.LEngineLanguageRead();
    }

    public IReadOnlyList<string> LWindowSchemeRead(string language)
    {
        return _lPhonologyPort.LEngineSchemeRead(language);
    }

    public IReadOnlyList<LSpeechValue> LWindowSpeechRead(string language)
    {
        return _lPhonologyPort.LEngineSpeechRead(language);
    }

    public LSpeechValue? LWindowSpeechAdd(string language, string name)
    {
        return _lPhonologyPort.LEngineSpeechAdd(language, name);
    }

    public LGlyph? LWindowGlyphRead(string language)
    {
        return _lEntryPort.LEngineGlyphRead(language);
    }

    public LEntryDraft? LWindowEntryLoad(long id)
    {
        return _lEntryPort.LEngineEntryLoad(id);
    }

    public IReadOnlyList<LEntry> LWindowMarkupFind(LMarkupEntry entry)
    {
        return _lEntryPort.LEngineMarkupFind(entry);
    }

    public Task<string> LWindowRecordingPrepare(LRecording recording, CancellationToken cancellation)
    {
        return _lMediaPort.LEngineRecordingPrepare(recording, cancellation);
    }

    public LSettings LWindowSettingsRead()
    {
        return _lSettingsPort.LEngineSettingsRead();
    }

    public string LWindowWorkspaceRead()
    {
        return _lSettingsPort.LEngineWorkspaceRead();
    }

    public string LWindowWorkspaceFormat()
    {
        return _lSettingsPort.LEngineWorkspaceFormat();
    }

    public LWorkspaceState LWindowStateRead()
    {
        return _lSettingsPort.LEngineStateRead();
    }

    public string LWindowLocalizationRead()
    {
        return _lSettingsPort.LEngineLocalizationRead();
    }

    public IReadOnlyDictionary<string, string> LWindowLocalizationLoad(string language)
    {
        return _lSettingsPort.LEngineLocalizationLoad(language);
    }

    public void LWindowLocalizationSave(string language)
    {
        _lSettingsPort.LEngineLocalizationSave(language);
    }

    public void LWindowEpithetSave(bool epithet)
    {
        _lSettingsPort.LEngineEpithetSave(epithet);
    }

    public void LWindowFrequencySave(bool frequency)
    {
        _lSettingsPort.LEngineFrequencySave(frequency);
    }

    public void LWindowMorphologySave(bool morphology)
    {
        _lSettingsPort.LEngineMorphologySave(morphology);
    }

    public void LWindowRespellingSave(bool respelled)
    {
        _lSettingsPort.LEngineRespellingSave(respelled);
    }

    public LEstablishment LWindowEstablishmentRead()
    {
        return _lSettingsPort.LEngineEstablishmentRead();
    }

    public string? LWindowAuditRecord(Exception exception)
    {
        return _lSettingsPort.LEngineAuditRecord(exception);
    }

    public string? LWindowNoticeRead(Exception exception)
    {
        return _lSettingsPort.LEngineNoticeRead(exception);
    }

    public void LWindowLeftoverSweep()
    {
        _lDraftPort.LEngineLeftoverSweep();
    }

    public void LWindowRecordingSweep()
    {
        _lMediaPort.LEngineRecordingSweep();
    }

    public LEntry LWindowGlyphResolve(string character, string language)
    {
        return _lEntryPort.LEngineGlyphResolve(character, language);
    }

    public IReadOnlyList<LMeaning> LWindowMeaningRead(long entryId)
    {
        return _lEntryPort.LEngineMeaningRead(entryId, LOwner.LOwnerEntry);
    }

    public static IReadOnlyList<(long LMeaningId, string LMeaningName, int LMeaningDepth)> LWindowMeaningSort(
        IReadOnlyList<LMeaning> meanings, string unknown)
    {
        ArgumentNullException.ThrowIfNull(meanings);
        ArgumentNullException.ThrowIfNull(unknown);

        List<(long LMeaningId, string LMeaningName, int LMeaningDepth)> rows = [];
        LWindowMeaningAppend(rows, meanings, 0, 0, unknown);
        return rows;
    }

    private static void LWindowMeaningAppend(
        List<(long LMeaningId, string LMeaningName, int LMeaningDepth)> rows,
        IReadOnlyList<LMeaning> meanings,
        long parent,
        int depth,
        string unknown)
    {
        List<LMeaning> children = [];
        foreach (LMeaning meaning in meanings)
        {
            if ((meaning.LMeaningParentId ?? 0) == parent && meaning.LMeaningId != parent)
            {
                children.Add(meaning);
            }
        }

        children.Sort((left, right) => left.LMeaningPosition.CompareTo(right.LMeaningPosition));
        foreach (LMeaning meaning in children)
        {
            rows.Add((meaning.LMeaningId, meaning.LMeaningName.Length > 0 ? meaning.LMeaningName : unknown, depth));
            LWindowMeaningAppend(rows, meanings, meaning.LMeaningId, depth + 1, unknown);
        }
    }
}
