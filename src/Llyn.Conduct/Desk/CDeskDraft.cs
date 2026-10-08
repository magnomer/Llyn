using System;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CDeskDraft
{
    private readonly LDraftPort _cDeskDraftPort;

    private LTenure? _cDeskDraftTenure;

    private LErrand? _cDeskDraftErrand;

    private LEasel? _cDeskDraftEasel;

    private LQuillChip? _cDeskDraftChip;

    private LQuillSpeech? _cDeskDraftSpeech;

    private readonly LSettingsPort _cDeskDraftSettings;

    private readonly string _cDeskDraftScope;

    private readonly CEnvoy _cDeskDraftEnvoy;

    private bool _cDeskDraftFilling;

    internal CDeskDraft(LDraftPort drafts, LSettingsPort settings, string scope, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentException.ThrowIfNullOrWhiteSpace(scope);
        ArgumentNullException.ThrowIfNull(envoy);

        _cDeskDraftPort = drafts;
        _cDeskDraftSettings = settings;
        _cDeskDraftScope = scope;
        _cDeskDraftEnvoy = envoy;
    }

    internal event Action<LDraft>? CDeskDraftPrepared;

    public event Action<CDraft>? CDeskDraftChanged;

    internal LErrand? CDeskDraftErrand => CDeskDraftFilling ? null : _cDeskDraftErrand;

    internal LQuillReference? CDeskDraftReference =>
        !CDeskDraftFilling && _cDeskDraftTenure is LTenure held ? new LQuillReference(held) : null;

    internal LQuillAuthor? CDeskDraftAuthor =>
        !CDeskDraftFilling && _cDeskDraftTenure is LTenure held ? new LQuillAuthor(held) : null;

    internal LQuillExample? CDeskDraftExample =>
        !CDeskDraftFilling && _cDeskDraftTenure is LTenure held ? new LQuillExample(held) : null;

    internal LQuillSentence? CDeskDraftSentence =>
        !CDeskDraftFilling && _cDeskDraftTenure is LTenure held ? new LQuillSentence(held) : null;

    internal LQuillMention? CDeskDraftMention =>
        !CDeskDraftFilling && _cDeskDraftTenure is LTenure held ? new LQuillMention(held) : null;

    internal LQuillTranscription? CDeskDraftTranscription =>
        !CDeskDraftFilling && _cDeskDraftTenure is LTenure held ? new LQuillTranscription(held) : null;

    internal LQuillEtymology? CDeskDraftEtymology =>
        !CDeskDraftFilling && _cDeskDraftTenure is LTenure held ? new LQuillEtymology(held) : null;

    internal LEasel? CDeskDraftEasel => CDeskDraftFilling ? null : _cDeskDraftEasel;

    internal LQuillChip? CDeskDraftChip => CDeskDraftFilling ? null : _cDeskDraftChip;

    internal LQuillSpeech? CDeskDraftSpeech => _cDeskDraftSpeech;

    public bool CDeskDraftFilling => _cDeskDraftFilling;

    internal LTenure? CDeskDraftTenure => _cDeskDraftTenure;

    public bool CDeskDraftAltered =>
        _cDeskDraftTenure?.LTenureGauge.LTenureGaugeRead() is { LTenureStateChanged: true };

    public bool CDeskDraftStorable => _cDeskDraftTenure?.LTenureGauge.LTenureGaugeStorable ?? false;

    internal void LDeskDraftSet(LTenure started)
    {
        ArgumentNullException.ThrowIfNull(started);

        _cDeskDraftTenure = started;
        _cDeskDraftErrand = started.LTenureErrand;
        _cDeskDraftEasel = new LEasel(started);
        _cDeskDraftChip = new LQuillChip(started, _cDeskDraftPort);
        _cDeskDraftSpeech = new LQuillSpeech(started);
    }

    internal void LDeskDraftClear()
    {
        _cDeskDraftTenure = null;
        _cDeskDraftErrand = null;
        _cDeskDraftEasel = null;
        _cDeskDraftChip = null;
        _cDeskDraftSpeech = null;
    }

    internal LDraft? CDeskDraftRead()
    {
        if (_cDeskDraftTenure is not LTenure held)
        {
            return null;
        }

        held.LTenurePersist();
        return held.LTenureRead();
    }

    public void CDeskDraftResonate()
    {
        if (CDeskDraftFilling)
        {
            return;
        }

        try
        {
            CDeskDraftShow(CDeskDraftPrepare());
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(
                _cDeskDraftEnvoy, _cDeskDraftSettings, _cDeskDraftScope + ".LoadFailed", exception);
        }
    }

    private LDraft? CDeskDraftPrepare()
    {
        if (_cDeskDraftTenure is not LTenure held)
        {
            return null;
        }

        held.LTenurePersist();
        return held.LTenurePrepare();
    }

    public void CDeskDraftPersist()
    {
        if (CDeskDraftFilling)
        {
            return;
        }

        _cDeskDraftTenure?.LTenurePersist();
    }

    private void CDeskDraftShow(LDraft? draft)
    {
        if (draft is null)
        {
            return;
        }

        _cDeskDraftFilling = true;
        try
        {
            CDeskDraftPrepared?.Invoke(draft);
            CDeskDraftChanged?.Invoke(new CDraft(draft.LDraftAuthorName));
        }
        finally
        {
            _cDeskDraftFilling = false;
        }
    }
}
