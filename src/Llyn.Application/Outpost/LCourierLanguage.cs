using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LCourierLanguage
{
    internal static readonly IReadOnlyList<string> LCourierLanguageFolder = ["xiesheng", "yunjing", "shengmu", "yunmu"];

    private readonly LOutpost _lCourierLanguageOutpost;
    private readonly LLivery _lCourierLanguageLivery;
    private readonly Action<Exception> _lCourierLanguageFault;

    public LCourierLanguage(LOutpost outpost, LLivery livery, Action<Exception> fault)
    {
        ArgumentNullException.ThrowIfNull(outpost);
        ArgumentNullException.ThrowIfNull(livery);
        ArgumentNullException.ThrowIfNull(fault);
        _lCourierLanguageOutpost = outpost;
        _lCourierLanguageLivery = livery;
        _lCourierLanguageFault = fault;
    }

    public async Task<int> LCourierLanguageSend(
        int port,
        string token,
        string shelf,
        string style,
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

        LLivery livery = _lCourierLanguageLivery;
        string name = language.LLiveryLanguageName;
        string[] notebooks =
            [.. LCourierLanguageFolder.Select(seed => livery.LLiveryIdFormat("llyn:" + seed + ":" + name))];
        string xiesheng = notebooks[0];
        string yunjing = notebooks[1];
        string shengmu = notebooks[2];
        string yunmu = notebooks[3];

        List<(string LCourierNoteId, string LCourierNoteFolder, Func<string> LCourierNoteTitle,
            Func<LLiveryNote> LCourierNoteSheet)> notes = [];
        foreach (LLiveryStem stem in language.LLiveryLanguageStem)
        {
            string key = stem.LLiveryStemPage.LStemPageKey;
            notes.Add((link(name, LLiveryStem.LLiveryStemKind, key), xiesheng, () => key,
                () => livery.LLiveryFormat(stem, style, note, lookup)));
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
                () => livery.LLiveryFormat(diwei, style, note, lookup)));
        }

        notes.Add((link(name, LCourierClerk.LCourierPhonology, string.Empty), shelf,
            () => lookup("Navigation.Phonology"), () => livery.LLiveryFormat(language, style, note, lookup)));
        notes.RemoveAll(static row => row.LCourierNoteId.Length == 0);

        HashSet<string> held = [.. notes.Select(static row => row.LCourierNoteFolder)];
        try
        {
            if (held.Contains(xiesheng))
            {
                await _lCourierLanguageOutpost
                    .LOutpostFolderSave(port, token, xiesheng, shelf, lookup("Navigation.Xiesheng"), cancellation)
                    .ConfigureAwait(false);
            }

            if (held.Contains(yunjing) || held.Contains(shengmu) || held.Contains(yunmu))
            {
                await _lCourierLanguageOutpost
                    .LOutpostFolderSave(port, token, yunjing, shelf, lookup("Navigation.Yunjing"), cancellation)
                    .ConfigureAwait(false);
            }

            if (held.Contains(shengmu))
            {
                await _lCourierLanguageOutpost
                    .LOutpostFolderSave(port, token, shengmu, yunjing, lookup("Yunjing.Shengmu"), cancellation)
                    .ConfigureAwait(false);
            }

            if (held.Contains(yunmu))
            {
                await _lCourierLanguageOutpost
                    .LOutpostFolderSave(port, token, yunmu, yunjing, lookup("Yunjing.Yunmu"), cancellation)
                    .ConfigureAwait(false);
            }
        }
        catch (TimeoutException exception)
        {
            _lCourierLanguageFault(exception);
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
                _lCourierLanguageFault(exception);
                failed.Add(label);
                stalled = exception is TimeoutException ? stalled + 1 : 0;
                if (stalled >= LCourierClerk.LCourierStall)
                {
                    throw new LRefusal(LRefusal.LRefusalOutpost);
                }
            }
        }

        return stalled;
    }
}
