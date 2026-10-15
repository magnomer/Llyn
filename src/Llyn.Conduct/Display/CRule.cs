using System;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

internal sealed class CRule
{
    private readonly LEntryPort _cRuleEntryPort;

    private readonly LFavoritePort _cRuleFavoritePort;

    private readonly LVistaPort _cRuleVistaPort;

    private readonly LGraspPort _cRuleGraspPort;

    private readonly LPronunciationPort _cRulePronunciationPort;

    private readonly CEnvoy _cRuleEnvoy;

    private readonly LSettingsPort _cRuleSettings;

    private LVista? _cRuleVista;

    private (long, int)? _cRuleGrasp;

    private LGraspStar? _cRuleGraspStar;

    internal CRule(
        LDraftPort drafts,
        LEntryPort entries,
        LFavoritePort favorites,
        LVistaPort vistas,
        LGraspPort grasps,
        LPronunciationPort pronunciations,
        LLanguagePort languages,
        LReflexPort reflexes,
        LFanqiePort fanqies,
        LScriptPort scripts,
        LParadigmPort paradigms,
        LSettingsPort settings,
        LMediaPort media,
        CEnvoy envoy,
        CLedgerNoticed noticed)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(favorites);
        ArgumentNullException.ThrowIfNull(vistas);
        ArgumentNullException.ThrowIfNull(grasps);
        ArgumentNullException.ThrowIfNull(pronunciations);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(noticed);

        _cRuleEntryPort = entries;
        _cRuleFavoritePort = favorites;
        _cRuleVistaPort = vistas;
        _cRuleGraspPort = grasps;
        _cRulePronunciationPort = pronunciations;
        _cRuleEnvoy = envoy;
        _cRuleSettings = settings;
        CRuleNoticed = noticed;
        CRuleMediaPort = media;
        CVoice = new CVoice(
            entries, languages, reflexes, fanqies, scripts, paradigms, media, settings);
        CVoice.CVoiceFailed +=
            (key, exception) => noticed.LLedgerRepaintShow(envoy, settings, key, exception);
        CVoice.CVoiceMarkFailed +=
            (key, exception) => CLedger.LLedgerFailureShow(envoy, settings, key, exception);
    }

    internal CVoice CVoice { get; }

    internal CLedgerNoticed CRuleNoticed { get; }

    internal LMediaPort CRuleMediaPort { get; }

    internal long? CRuleChosen => _cRuleVista?.LVistaChosen;

    internal void CRuleVistaRestore(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        _cRuleVista = vista;
    }

    internal void CRuleChosenAttach(CSubject subject, Action<CBulletin> observer)
    {
        _cRuleVista?.LVistaChosenAttach(
            CCatalog.LCatalogSubjectRead(subject), bulletin => observer(CAtelier.CAtelierBulletinRead(bulletin)));
    }

    internal void CRuleObserverAttach(CSubject subject, Action<CBulletin> observer)
    {
        _cRuleVista?.LVistaObserverAttach(
            CCatalog.LCatalogSubjectRead(subject), bulletin => observer(CAtelier.CAtelierBulletinRead(bulletin)));
    }

    internal void CRuleDraftLoad(Action<LEntryDraft?> show)
    {
        ArgumentNullException.ThrowIfNull(show);

        LEntryDraft? draft;
        try
        {
            draft = _cRuleVista is null
                ? null
                : _cRuleVistaPort.LEngineVistaLoad(_cRuleVista)?.LDraftContent;
        }
        catch (Exception exception)
        {
            CRuleNoticed.LLedgerRepaintShow(_cRuleEnvoy, _cRuleSettings, "Sound.LoadFailed", exception);
            return;
        }

        show(draft);
    }

    internal LEntryDraft? CRuleEntryLoad(long id)
    {
        LEntryDraft? loaded = _cRuleEntryPort.LEngineEntryLoad(id);
        _cRuleVista?.LVistaSelect(loaded is null ? null : id);
        return loaded;
    }

    internal bool CRuleFavoriteRead(long? entry)
    {
        if (entry is not long id)
        {
            return false;
        }

        try
        {
            return _cRuleFavoritePort.LEngineFavoriteCheck(id);
        }
        catch (Exception exception)
        {
            CRuleNoticed.LLedgerRepaintShow(_cRuleEnvoy, _cRuleSettings, "Favorite.ReadFailed", exception);
            return false;
        }
    }

    internal void CRuleFavoriteSave(long? entry, bool marked)
    {
        if (entry is not long id)
        {
            return;
        }

        try
        {
            if (marked)
            {
                _cRuleFavoritePort.LEngineFavoriteSave(id);
            }
            else
            {
                _cRuleFavoritePort.LEngineFavoriteDelete(id);
            }
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cRuleEnvoy, _cRuleSettings, "Favorite.MarkFailed", exception);
        }
    }

    internal int CRuleGraspStep => Math.Max(0, _cRuleGraspPort.LEngineGraspStep);

    internal LGraspStar CRuleGraspStar => _cRuleGraspStar ??= LGraspPort.LEngineStarScan(CRuleGraspStep);

    internal string CRuleGraspFormat(long? entry, int step)
    {
        return entry is null
            ? string.Empty
            : _cRuleGraspPort.LEngineGraspFormat(step);
    }

    internal int CRuleGraspRead(long? entry)
    {
        if (entry is not long id)
        {
            return 0;
        }

        try
        {
            int step = Math.Clamp(_cRuleGraspPort.LEngineGraspRead(id), 0, CRuleGraspStep);
            _cRuleGrasp = (id, step);
            return step;
        }
        catch (Exception exception)
        {
            CRuleNoticed.LLedgerRepaintShow(_cRuleEnvoy, _cRuleSettings, "Grasp.ReadFailed", exception);
            return 0;
        }
    }

    internal void CRuleGraspSave(long? entry, int step)
    {
        if (entry is not long id)
        {
            return;
        }

        int kept = _cRuleGrasp == (id, step) ? 0 : step;
        try
        {
            _cRuleGraspPort.LEngineGraspSave(id, kept);
            _cRuleGrasp = (id, kept);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cRuleEnvoy, _cRuleSettings, "Grasp.MarkFailed", exception);
        }
    }

    internal CFrequency? CRuleFrequencyRead(long? entry, string once)
    {
        if (entry is not long id)
        {
            return null;
        }

        LFrequencyGauge? gauge;
        try
        {
            gauge = _cRulePronunciationPort.LEngineFrequencyResolve(id, once);
        }
        catch (Exception exception)
        {
            CRuleNoticed.LLedgerRepaintShow(_cRuleEnvoy, _cRuleSettings, "Frequency.ReadFailed", exception);
            return null;
        }

        return gauge is null
            ? null
            : new CFrequency(
                Math.Max(0, gauge.LFrequencyGaugeBand),
                gauge.LFrequencyGaugeSource,
                CRuleTierRead(gauge.LFrequencyGaugeRank),
                Math.Max(0, gauge.LFrequencyGaugeSpare),
                gauge.LFrequencyGaugeRanked);
    }

    private static CFrequencyTier CRuleTierRead(string rank)
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
