using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class LDisplay
{
    private readonly LEntryPort _lEntryPort;

    private readonly CEnvoy _lDisplayEnvoy;

    private LVista? _lDisplayVista;

    private (long, int)? _lDisplayGrasp;

    internal LDisplay(
        LDraftPort drafts,
        LEntryPort entries,
        LPhonologyPort phonology,
        LSettingsPort settings,
        LMediaPort media,
        CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(envoy);

        _lEntryPort = entries;
        _lDisplayEnvoy = envoy;
        LDisplaySound = new LDisplaySound(entries, phonology, media, settings);
        LDisplaySound.LDisplaySoundFailed += envoy.CEnvoyFailureShow;
        CDisplayArea = new CDisplay(this, entries, phonology, envoy);
        CDisplaySound = new CDisplaySound(
            LDisplaySound, CDisplayArea, drafts, entries, phonology, media, settings, envoy);
    }

    public LDisplaySound LDisplaySound { get; }

    internal void LDisplayNavigationAttach(CNavigation navigation)
    {
        ArgumentNullException.ThrowIfNull(navigation);

        CDisplayArea.CDisplayRowChosen += (tab, id) => navigation.LNavigationRowOpen(tab, id);
        CDisplaySound.CDisplayRowChosen += (tab, id) => navigation.LNavigationRowOpen(tab, id);
        CDisplaySound.CDisplayDiweiChosen +=
            (language, kind, key) => navigation.CNavigationDiweiOpen(language, kind, key);
        CDisplaySound.CDisplayStemChosen += (language, key) => navigation.LNavigationStemOpen(language, key);
    }

    public CDisplay CDisplayArea { get; }

    public CDisplaySound CDisplaySound { get; }

    internal long? LDisplayChosen => _lDisplayVista?.LVistaChosen;

    public void LDisplayVistaRestore(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        _lDisplayVista = vista;
        CDisplayArea.LDisplayVistaAttach();
    }

    internal void LDisplayChosenAttach(CSubject subject, Action<CBulletin> observer)
    {
        _lDisplayVista?.LVistaChosenAttach(
            CPanel.CPanelSubjectRead(subject), bulletin => observer(CAtelier.CAtelierBulletinRead(bulletin)));
    }

    internal void LDisplayObserverAttach(CSubject subject, Action<CBulletin> observer)
    {
        _lDisplayVista?.LVistaObserverAttach(
            CPanel.CPanelSubjectRead(subject), bulletin => observer(CAtelier.CAtelierBulletinRead(bulletin)));
    }

    internal void LDisplayDraftLoad(Action<LEntryDraft?> show)
    {
        ArgumentNullException.ThrowIfNull(show);

        LEntryDraft? draft;
        try
        {
            draft = _lDisplayVista?.LVistaLoad()?.LDraftContent;
        }
        catch (Exception exception)
        {
            _lDisplayEnvoy.CEnvoyFailureShow("Sound.LoadFailed", exception);
            return;
        }

        show(draft);
    }

    internal void LDisplayEntryLoad(long id)
    {
        LEntryDraft? loaded = _lEntryPort.LEngineEntryLoad(id);
        _lDisplayVista?.LVistaSelect(loaded is null ? null : id);
        CDisplayArea.LDisplayEntryOpen(loaded);
    }

    internal bool LDisplayFanqieCheck(long? id) => LDisplaySound.LDisplayFanqieCheck(id);

    internal bool LDisplayScriptCheck(long? id) => LDisplaySound.LDisplayScriptCheck(id);

    internal bool LDisplayParadigmCheck(long? id) => LDisplaySound.LDisplayParadigmCheck(id);

    internal bool LDisplayFavoriteRead(long? entry)
    {
        if (entry is not long id)
        {
            return false;
        }

        try
        {
            return _lEntryPort.LEngineFavoriteCheck(id);
        }
        catch (Exception)
        {
            return false;
        }
    }

    internal void LDisplayFavoriteSave(long? entry, bool marked)
    {
        if (entry is not long id)
        {
            return;
        }

        try
        {
            if (marked)
            {
                _lEntryPort.LEngineFavoriteSave(id);
            }
            else
            {
                _lEntryPort.LEngineFavoriteDelete(id);
            }
        }
        catch (Exception exception)
        {
            _lDisplayEnvoy.CEnvoyFailureShow("Favorite.MarkFailed", exception);
        }
    }

    internal int LDisplayGraspStep => _lEntryPort.LEngineGraspStep;

    public IReadOnlyList<CCompassRow> CDisplayCompassRead(
        IReadOnlyList<CCompassPart> parts, Func<string, string> lookup)
    {
        ArgumentNullException.ThrowIfNull(parts);
        ArgumentNullException.ThrowIfNull(lookup);

        LEntryDraft? shown = LDisplaySound.LDisplayShown;
        List<CCompassRow> rows = [];
        List<string> labels = [];
        foreach (CCompassPart part in parts)
        {
            rows.Add(new CCompassRow(part, null, string.Empty, string.Empty, 0));
            labels.Add(lookup(LDisplayKeyRead(part)));
            if (shown is null
                || part is not (CCompassPart.CCompassPartMeaning or CCompassPart.CCompassPartCollocation))
            {
                continue;
            }

            bool collocated = part == CCompassPart.CCompassPartCollocation;
            IReadOnlyList<CCardDraft> cards = CFolio.CFolioSheetRead(
                collocated ? shown.LEntryDraftCollocations : shown.LEntryDraftMeanings);
            string kind = lookup(collocated ? "Display.CollocationSingle" : "Display.MeaningSingle");
            string unknown = lookup("Display.Unknown");
            for (int index = 0; index < cards.Count; index++)
            {
                CStateValue title = cards[index].CCardDraftTitle;
                rows.Add(new CCompassRow(
                    part,
                    index,
                    string.Empty,
                    cards[index].CCardDraftPosition.ToString(CultureInfo.CurrentCulture),
                    1));
                labels.Add(title.CStateValueUncertain ? unknown : title.CStateValueShown ?? kind);
            }
        }

        IReadOnlyList<string> names = LDisplayNameResolve(labels);
        return rows.Select((row, index) => row with { CCompassRowName = names[index] }).ToList();
    }

    private static string LDisplayKeyRead(CCompassPart part)
    {
        return part switch
        {
            CCompassPart.CCompassPartSpeech => "Speech.Title",
            CCompassPart.CCompassPartFrequency => "Frequency.Title",
            CCompassPart.CCompassPartMeaning => "Display.MeaningPlural",
            CCompassPart.CCompassPartCollocation => "Display.Collocation",
            CCompassPart.CCompassPartIncoming => "Display.Translated",
            CCompassPart.CCompassPartNote => "Display.Note",
            _ => throw new ArgumentOutOfRangeException(nameof(part), part, null),
        };
    }

    private IReadOnlyList<string> LDisplayNameResolve(IReadOnlyList<string> labels)
    {
        try
        {
            return _lEntryPort.LEngineNameResolve(labels);
        }
        catch (Exception exception)
        {
            _lDisplayEnvoy.CEnvoyFailureShow("Display.NameFailed", exception);
            return labels;
        }
    }

    internal string LDisplayGraspFormat(long? entry, int step)
    {
        return entry is null
            ? string.Empty
            : _lEntryPort.LEngineGraspFormat(step);
    }

    internal int LDisplayGraspRead(long? entry)
    {
        if (entry is not long id)
        {
            return 0;
        }

        try
        {
            int step = _lEntryPort.LEngineGraspRead(id);
            _lDisplayGrasp = (id, step);
            return step;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    internal void LDisplayGraspSave(long? entry, int step)
    {
        if (entry is not long id)
        {
            return;
        }

        int kept = _lDisplayGrasp == (id, step) ? 0 : step;
        try
        {
            _lEntryPort.LEngineGraspSave(id, kept);
            _lDisplayGrasp = (id, kept);
        }
        catch (Exception exception)
        {
            _lDisplayEnvoy.CEnvoyFailureShow("Grasp.MarkFailed", exception);
        }
    }

    internal CFrequency? LDisplayFrequencyRead(long? entry, string once)
    {
        if (entry is not long id)
        {
            return null;
        }

        LFrequencyGauge? gauge;
        try
        {
            gauge = _lEntryPort.LEngineFrequencyResolve(id, once);
        }
        catch (Exception)
        {
            return null;
        }

        return gauge is null
            ? null
            : new CFrequency(
                gauge.LFrequencyGaugeBand,
                gauge.LFrequencyGaugeSource,
                gauge.LFrequencyGaugeRank,
                gauge.LFrequencyGaugeSpare,
                gauge.LFrequencyGaugeRanked);
    }
}
