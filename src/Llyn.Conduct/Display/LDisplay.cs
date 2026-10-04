using System;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

internal sealed class LDisplay
{
    private readonly LEntryPort _lEntryPort;

    private readonly CEnvoy _lDisplayEnvoy;

    private readonly LSettingsPort _lDisplaySettings;

    private LVista? _lDisplayVista;

    private (long, int)? _lDisplayGrasp;

    internal LDisplay(
        LDraftPort drafts,
        LEntryPort entries,
        LPhonologyPort phonology,
        LSettingsPort settings,
        LMediaPort media,
        CEnvoy envoy,
        CLedgerNoticed noticed)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(noticed);

        _lEntryPort = entries;
        _lDisplayEnvoy = envoy;
        _lDisplaySettings = settings;
        LDisplayNoticed = noticed;
        LDisplayMediaPort = media;
        LDisplaySound = new LDisplaySound(entries, phonology, media, settings);
        LDisplaySound.LDisplaySoundFailed +=
            (key, exception) => noticed.LLedgerRepaintShow(envoy, settings, key, exception);
        LDisplaySound.LDisplayMarkFailed +=
            (key, exception) => CLedger.LLedgerFailureShow(envoy, settings, key, exception);
    }

    internal LDisplaySound LDisplaySound { get; }

    internal CLedgerNoticed LDisplayNoticed { get; }

    internal LMediaPort LDisplayMediaPort { get; }

    internal long? LDisplayChosen => _lDisplayVista?.LVistaChosen;

    internal void LDisplayVistaRestore(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        _lDisplayVista = vista;
    }

    internal void LDisplayChosenAttach(CSubject subject, Action<CBulletin> observer)
    {
        _lDisplayVista?.LVistaChosenAttach(
            CCatalog.LCatalogSubjectRead(subject), bulletin => observer(CAtelier.CAtelierBulletinRead(bulletin)));
    }

    internal void LDisplayObserverAttach(CSubject subject, Action<CBulletin> observer)
    {
        _lDisplayVista?.LVistaObserverAttach(
            CCatalog.LCatalogSubjectRead(subject), bulletin => observer(CAtelier.CAtelierBulletinRead(bulletin)));
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
            LDisplayNoticed.LLedgerRepaintShow(_lDisplayEnvoy, _lDisplaySettings, "Sound.LoadFailed", exception);
            return;
        }

        show(draft);
    }

    internal LEntryDraft? LDisplayEntryLoad(long id)
    {
        LEntryDraft? loaded = _lEntryPort.LEngineEntryLoad(id);
        _lDisplayVista?.LVistaSelect(loaded is null ? null : id);
        return loaded;
    }

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
        catch (Exception exception)
        {
            LDisplayNoticed.LLedgerRepaintShow(_lDisplayEnvoy, _lDisplaySettings, "Favorite.ReadFailed", exception);
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
            CLedger.LLedgerFailureShow(_lDisplayEnvoy, _lDisplaySettings, "Favorite.MarkFailed", exception);
        }
    }

    internal int LDisplayGraspStep => Math.Max(0, _lEntryPort.LEngineGraspStep);

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
            int step = Math.Clamp(_lEntryPort.LEngineGraspRead(id), 0, LDisplayGraspStep);
            _lDisplayGrasp = (id, step);
            return step;
        }
        catch (Exception exception)
        {
            LDisplayNoticed.LLedgerRepaintShow(_lDisplayEnvoy, _lDisplaySettings, "Grasp.ReadFailed", exception);
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
            CLedger.LLedgerFailureShow(_lDisplayEnvoy, _lDisplaySettings, "Grasp.MarkFailed", exception);
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
        catch (Exception exception)
        {
            LDisplayNoticed.LLedgerRepaintShow(_lDisplayEnvoy, _lDisplaySettings, "Frequency.ReadFailed", exception);
            return null;
        }

        return gauge is null
            ? null
            : new CFrequency(
                Math.Max(0, gauge.LFrequencyGaugeBand),
                gauge.LFrequencyGaugeSource,
                LDisplayTierRead(gauge.LFrequencyGaugeRank),
                Math.Max(0, gauge.LFrequencyGaugeSpare),
                gauge.LFrequencyGaugeRanked);
    }

    private static CFrequencyTier LDisplayTierRead(string rank)
    {
        return rank switch
        {
            "Core" => CFrequencyTier.CFrequencyTierCore,
            "Everyday" => CFrequencyTier.CFrequencyTierEveryday,
            "Advanced" => CFrequencyTier.CFrequencyTierAdvanced,
            "Rare" => CFrequencyTier.CFrequencyTierRare,
            _ => CFrequencyTier.CFrequencyTierUnknown,
        };
    }
}
