using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;

namespace Llyn.UIDeportment;

public sealed class QWorkspace
{
    private readonly LWindow _lWindow;

    internal QWorkspace(LWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        _lWindow = window;
    }

    public CSettings QWorkspaceSettingsRead()
    {
        return QWorkspaceSettingsRead(_lWindow.LWindowSettingsPort.LEngineSettingsRead());
    }

    public string QWorkspacePathRead()
    {
        return _lWindow.LWindowSettingsPort.LEngineWorkspaceRead();
    }

    public string QWorkspacePathFormat()
    {
        return _lWindow.LWindowSettingsPort.LEngineWorkspaceFormat();
    }

    public CWorkspaceState QWorkspaceStateRead()
    {
        return QWorkspaceStateRead(_lWindow.LWindowSettingsPort.LEngineStateRead());
    }

    public string QWorkspaceLocalizationRead()
    {
        return _lWindow.LWindowSettingsPort.LEngineLocalizationRead();
    }

    public IReadOnlyDictionary<string, string> QWorkspaceLocalizationLoad(string language)
    {
        return _lWindow.LWindowSettingsPort.LEngineLocalizationLoad(language);
    }

    public void QWorkspaceLocalizationSave(string language)
    {
        _lWindow.LWindowSettingsPort.LEngineLocalizationSave(language);
    }

    public void QWorkspaceEpithetSave(bool epithet)
    {
        _lWindow.LWindowSettingsPort.LEngineEpithetSave(epithet);
    }

    public void QWorkspaceFrequencySave(bool frequency)
    {
        _lWindow.LWindowSettingsPort.LEngineFrequencySave(frequency);
    }

    public void QWorkspaceMorphologySave(bool morphology)
    {
        _lWindow.LWindowSettingsPort.LEngineMorphologySave(morphology);
    }

    public void QWorkspaceRespellingSave(bool respelled)
    {
        _lWindow.LWindowSettingsPort.LEngineRespellingSave(respelled);
    }

    public CEstablishment QWorkspaceEstablishmentRead()
    {
        return QWorkspaceEstablishmentRead(_lWindow.LWindowSettingsPort.LEngineEstablishmentRead());
    }

    public string? QWorkspaceAuditRecord(Exception exception)
    {
        return _lWindow.LWindowSettingsPort.LEngineAuditRecord(exception);
    }

    public string? QWorkspaceNoticeRead(Exception exception)
    {
        return _lWindow.LWindowSettingsPort.LEngineNoticeRead(exception);
    }

    public void QWorkspaceRecordingSweep()
    {
        _lWindow.LWindowMediaPort.LEngineRecordingSweep();
    }

    public bool QWorkspaceRecordingExist(string? file)
    {
        return _lWindow.LWindowMediaPort.LEngineRecordingExist(file);
    }

    public Task<string> QWorkspaceRecordingPrepare(CRecording recording, CancellationToken cancellation)
    {
        return _lWindow.LWindowMediaPort.LEngineRecordingPrepare(LDesk.LDeskRecordingRead(recording), cancellation);
    }

    public Task QWorkspaceEnsignLoad(Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)
    {
        ArgumentNullException.ThrowIfNull(store);

        return _lWindow.LWindowSettingsPort.LEngineEnsignLoad(QWorkspaceEnsignSend);

        Action QWorkspaceEnsignSend(IReadOnlyList<LEnsignRow> rows, Action<string, Exception> delete)
        {
            return store(QWorkspaceEnsignRead(rows), delete);
        }
    }

    public Task QWorkspaceEnsignLoad(
        string language,
        IEnumerable<string> varieties,
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)
    {
        ArgumentNullException.ThrowIfNull(store);

        return _lWindow.LWindowSettingsPort.LEngineEnsignLoad(language, varieties, QWorkspaceEnsignSend);

        Action QWorkspaceEnsignSend(IReadOnlyList<LEnsignRow> rows, Action<string, Exception> delete)
        {
            return store(QWorkspaceEnsignRead(rows), delete);
        }
    }

    public Uri? QWorkspaceLocationRead(string? location)
    {
        return _lWindow.LWindowMediaPort.LEngineLocationRead(location);
    }

    public void QWorkspaceLocationOpen(string target)
    {
        _lWindow.LWindowMediaPort.LEngineLocationOpen(target);
    }

    public IReadOnlyList<string> QWorkspaceLanguageRead()
    {
        return _lWindow.LWindowSettingsPort.LEngineLanguageRead();
    }

    public CEntryDraft? QWorkspaceEntryLoad(long id)
    {
        return QWorkspaceEntryRead(_lWindow.LWindowEntryPort.LEngineEntryLoad(id));
    }

    public IReadOnlyList<long> QWorkspaceMarkupFind(string headword, string language)
    {
        return LSplice.LSpliceBuild(
            _lWindow.LWindowEntryPort.LEngineMarkupFind(new LMarkupEntry(headword, language)),
            static entry => entry.LEntryId);
    }

    public long QWorkspaceGlyphResolve(string character, string language)
    {
        return _lWindow.LWindowEntryPort.LEngineGlyphResolve(character, language).LEntryId;
    }

    public IReadOnlyList<CMeaning> QWorkspaceMeaningRead(long entryId, string unknown)
    {
        return QWorkspaceMeaningSort(
            _lWindow.LWindowEntryPort.LEngineMeaningRead(entryId, LOwner.LOwnerEntry), unknown);
    }

    internal static IReadOnlyList<CMeaning> QWorkspaceMeaningSort(IReadOnlyList<LMeaning> meanings, string unknown)
    {
        ArgumentNullException.ThrowIfNull(meanings);
        ArgumentNullException.ThrowIfNull(unknown);

        List<CMeaning> rows = [];
        QWorkspaceMeaningAppend(rows, meanings, 0, 0, unknown);
        return rows;
    }

    private static void QWorkspaceMeaningAppend(
        List<CMeaning> rows,
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
            rows.Add(new CMeaning(
                meaning.LMeaningId, meaning.LMeaningName.Length > 0 ? meaning.LMeaningName : unknown, depth));
            QWorkspaceMeaningAppend(rows, meanings, meaning.LMeaningId, depth + 1, unknown);
        }
    }

    private static CEntryDraft? QWorkspaceEntryRead(LEntryDraft? draft)
    {
        return draft is null ? null : LCard.LCardEntryRead(draft);
    }

    private static IReadOnlyList<CEnsignRow> QWorkspaceEnsignRead(IReadOnlyList<LEnsignRow> rows)
    {
        return LSplice.LSpliceBuild(rows, static row => new CEnsignRow(row.LEnsignRowKey, row.LEnsignRowPath));
    }

    internal static CSettings QWorkspaceSettingsRead(LSettings settings)
    {
        return new CSettings(
            settings.LSettingsLocalization,
            settings.LSettingsRespelled,
            settings.LSettingsFrequency,
            settings.LSettingsMorphology,
            settings.LSettingsEpithet,
            settings.LSettingsOnline);
    }

    private static CWorkspaceState QWorkspaceStateRead(LWorkspaceState state)
    {
        return new CWorkspaceState(state.LWorkspaceStateLeft, state.LWorkspaceStateRight);
    }

    private static CEstablishment QWorkspaceEstablishmentRead(LEstablishment establishment)
    {
        return new CEstablishment(
            establishment.LEstablishmentUnsaved,
            establishment.LEstablishmentEntry,
            establishment.LEstablishmentSize,
            establishment.LEstablishmentPending,
            establishment.LEstablishmentSingle);
    }
}
