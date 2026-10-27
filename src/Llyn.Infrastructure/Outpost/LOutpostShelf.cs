using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal sealed class LOutpostShelf
{
    internal static readonly IReadOnlyList<string> LOutpostShelfFolder = ["xiesheng", "yunjing", "shengmu", "yunmu"];

    private readonly LOutpostHttp _lOutpostShelfHttp;
    private readonly LEngraverEntrance _lOutpostShelfEngraver;
    private readonly Action<Exception> _lOutpostShelfFault;

    public LOutpostShelf(LOutpostHttp http, LEngraverEntrance engraver, Action<Exception> fault)
    {
        ArgumentNullException.ThrowIfNull(http);
        ArgumentNullException.ThrowIfNull(engraver);
        ArgumentNullException.ThrowIfNull(fault);
        _lOutpostShelfHttp = http;
        _lOutpostShelfEngraver = engraver;
        _lOutpostShelfFault = fault;
    }

    public static Func<string, string, string, string> LOutpostShelfBuild(
        string stamp, IReadOnlyList<LLiveryLanguage> languages)
    {
        ArgumentNullException.ThrowIfNull(stamp);
        ArgumentNullException.ThrowIfNull(languages);

        Dictionary<(string, string, string), string> links = [];
        foreach (LLiveryLanguage language in languages)
        {
            string name = language.LLiveryLanguageName;
            List<(string LOutpostShelfKind, string LOutpostShelfKey)> held =
            [
                .. language.LLiveryLanguageStem
                    .Select(static stem => (LLiveryStem.LLiveryStemKind, stem.LLiveryStemPage.LStemPageKey)),
                .. language.LLiveryLanguageDiwei
                    .Select(static diwei => (diwei.LLiveryDiweiKind, diwei.LLiveryDiweiPage.LDiweiPageKey)),
            ];
            held.RemoveAll(static row => row.LOutpostShelfKey.Length == 0);

            foreach ((string kind, string key) in held)
            {
                links.TryAdd(
                    (name, kind, key),
                    LOutpostSeal.LOutpostSealFormat("llyn:" + kind + ":" + stamp + ":" + name + ":" + key));
            }
        }

        return (language, kind, key) => links.GetValueOrDefault((language, kind, key), string.Empty);
    }

    public async Task<int> LOutpostShelfSend(
        int port,
        string token,
        string shelf,
        string mark,
        LLiveryLanguage language,
        Func<long, string> note,
        Func<string, string, string, string> link,
        Func<string, string> lookup,
        Func<LOutpostNote, IReadOnlyList<LParcel>, Task<bool>> send,
        ISet<string> current,
        IList<string> failed,
        int stalled,
        CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(note);
        ArgumentNullException.ThrowIfNull(link);
        ArgumentNullException.ThrowIfNull(lookup);
        ArgumentNullException.ThrowIfNull(send);
        ArgumentNullException.ThrowIfNull(current);
        ArgumentNullException.ThrowIfNull(failed);

        LEngraverEntrance engraver = _lOutpostShelfEngraver;
        string name = language.LLiveryLanguageName;
        string[] notebooks =
            [.. LOutpostShelfFolder.Select(seed => LOutpostSeal.LOutpostSealFormat("llyn:" + seed + ":" + name))];
        string xiesheng = notebooks[0];
        string yunjing = notebooks[1];
        string shengmu = notebooks[2];
        string yunmu = notebooks[3];

        List<(string LOutpostShelfId, string LOutpostShelfNotebook, Func<string> LOutpostShelfTitle,
            Func<LLiveryNote> LOutpostShelfSheet)> notes = [];
        foreach (LLiveryStem stem in language.LLiveryLanguageStem)
        {
            string key = stem.LLiveryStemPage.LStemPageKey;
            notes.Add((link(name, LLiveryStem.LLiveryStemKind, key), xiesheng, () => key,
                () => engraver.LEngraverEntranceFormat(stem, mark, note, lookup)));
        }

        foreach (LLiveryDiwei diwei in language.LLiveryLanguageDiwei)
        {
            string key = diwei.LLiveryDiweiPage.LDiweiPageKey;
            (string folder, Func<string> title) = diwei.LLiveryDiweiKind switch
            {
                LDiwei.LDiweiInitial => (shengmu, () => key),
                LDiwei.LDiweiRime => (yunmu, () => key),
                _ => (yunjing, (Func<string>)(() =>
                    string.Format(CultureInfo.CurrentCulture, lookup("Display.FanqieTone"), key))),
            };
            notes.Add((link(name, diwei.LLiveryDiweiKind, key), folder, title,
                () => engraver.LEngraverEntranceFormat(diwei, mark, note, lookup)));
        }

        notes.RemoveAll(static row => row.LOutpostShelfId.Length == 0);

        HashSet<string> held = [.. notes.Select(static row => row.LOutpostShelfNotebook)];
        try
        {
            if (held.Contains(xiesheng))
            {
                await _lOutpostShelfHttp
                    .LOutpostFolderSave(port, token, xiesheng, shelf, lookup("Navigation.Xiesheng"), cancellation)
                    .ConfigureAwait(false);
            }

            if (held.Contains(yunjing) || held.Contains(shengmu) || held.Contains(yunmu))
            {
                await _lOutpostShelfHttp
                    .LOutpostFolderSave(port, token, yunjing, shelf, lookup("Navigation.Yunjing"), cancellation)
                    .ConfigureAwait(false);
            }

            if (held.Contains(shengmu))
            {
                await _lOutpostShelfHttp
                    .LOutpostFolderSave(port, token, shengmu, yunjing, lookup("Yunjing.Shengmu"), cancellation)
                    .ConfigureAwait(false);
            }

            if (held.Contains(yunmu))
            {
                await _lOutpostShelfHttp
                    .LOutpostFolderSave(port, token, yunmu, yunjing, lookup("Yunjing.Yunmu"), cancellation)
                    .ConfigureAwait(false);
            }
        }
        catch (TimeoutException exception)
        {
            _lOutpostShelfFault(exception);
            throw new LRefusal(LRefusal.LRefusalOutpost);
        }

        foreach ((string id, string folder, Func<string> title, Func<LLiveryNote> sheet) in notes)
        {
            cancellation.ThrowIfCancellationRequested();
            current.Add(id);
            string label = name;
            try
            {
                label = title();
                LLiveryNote rendered = sheet();
                await send(new LOutpostNote(id, folder, label, rendered.LLiveryNoteBody), rendered.LLiveryNoteParcel)
                    .ConfigureAwait(false);
                stalled = 0;
            }
            catch (Exception exception) when (exception is not LRefusal and not OperationCanceledException)
            {
                _lOutpostShelfFault(exception);
                failed.Add(label);
                stalled = exception is TimeoutException ? stalled + 1 : 0;
                if (stalled >= LOutpostBatch.LOutpostBatchStall)
                {
                    throw new LRefusal(LRefusal.LRefusalOutpost);
                }
            }
        }

        return stalled;
    }
}
