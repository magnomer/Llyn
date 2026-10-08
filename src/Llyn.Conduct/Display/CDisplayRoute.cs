using System;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CDisplayRoute
{
    private readonly LDisplaySound _cDisplayVoice;

    private readonly LMentionPort _cDisplayMentionPort;

    private readonly LSettingsPort _cDisplaySettings;

    private readonly CEnvoy _cDisplayEnvoy;

    private CMention? _cDisplayMention;

    internal CDisplayRoute(LDisplay display, LMentionPort mentions, LSettingsPort settings, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(mentions);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(envoy);

        _cDisplayVoice = display.LDisplaySound;
        _cDisplayMentionPort = mentions;
        _cDisplaySettings = settings;
        _cDisplayEnvoy = envoy;
    }

    internal event Action<string, long>? CDisplayRowChosen;

    internal void LDisplayMentionAttach(CMention mention)
    {
        ArgumentNullException.ThrowIfNull(mention);

        _cDisplayMention = mention;
    }

    public bool CDisplayChipOpen(CLeafChip? chip, long? link)
    {
        if (chip is { CLeafChipStored: true })
        {
            CDisplayRowChosen?.Invoke(LDisplayTabRead(chip.CLeafChipSubject), chip.CLeafChipId);
            return true;
        }

        if (_cDisplayMentionPort.LEngineLinkRead(link) is not long id)
        {
            return false;
        }

        CDisplayRowChosen?.Invoke("Library", id);
        return true;
    }

    private static string LDisplayTabRead(CSubject subject)
    {
        return subject switch
        {
            CSubject.CSubjectSituation => "Repertoire",
            CSubject.CSubjectRegister => "Tenor",
            CSubject.CSubjectTag => "Taxonomy",
            _ => throw new ArgumentOutOfRangeException(nameof(subject), subject, null),
        };
    }

    public CMentionOffer? CDisplayMentionFind(long sentence, string text, int unit)
    {
        return LDisplayMentionOpen(
            text, unit, (shown, offset) => _cDisplayMentionPort.LEngineMentionFind(shown, sentence, offset));
    }

    public CMentionOffer? CDisplayEtymologyFind(string text, int unit)
    {
        return LDisplayMentionOpen(
            text, unit, (shown, offset) => _cDisplayMentionPort.LEngineEtymologyFind(shown, offset));
    }

    private CMentionOffer? LDisplayMentionOpen(
        string text, int unit, Func<LEntryDraft, int, LMentionResult> find)
    {
        if (_cDisplayMention is not CMention mention || _cDisplayVoice.LDisplayShown is not LEntryDraft shown)
        {
            return null;
        }

        try
        {
            int offset = mention.LMentionOffsetRead(text, unit);
            return mention.LMentionResultOpen(CMention.CMentionResultRead(find(shown, offset)), text);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cDisplayEnvoy, _cDisplaySettings, "Mention.FindFailed", exception);
            return null;
        }
    }
}
