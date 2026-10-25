using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CDisplayRoute
{
    private readonly CVoice _cDisplayVoice;

    private readonly LMentionPort _cDisplayMentionPort;

    private readonly LSettingsPort _cDisplaySettings;

    private readonly CEnvoy _cDisplayEnvoy;

    private CMention? _cDisplayMention;

    private LEntryDraft? _cDisplayWordShown;

    private long? _cDisplayWordSentence;

    private IReadOnlyList<string> _cDisplayWordPiece = [];

    private int _cDisplayWordUnit;

    private int _cDisplayWordHead;

    private readonly CDisplayCard _cDisplayCard;

    internal CDisplayRoute(
        CRule display, CDisplayCard card, LMentionPort mentions, LSettingsPort settings, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(card);
        ArgumentNullException.ThrowIfNull(mentions);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(envoy);

        _cDisplayVoice = display.CVoice;
        _cDisplayCard = card;
        _cDisplayMentionPort = mentions;
        _cDisplaySettings = settings;
        _cDisplayEnvoy = envoy;
    }

    internal event Action<string, long>? CDisplayRowChosen;

    public int CDisplayWordUnit => _cDisplayWordUnit;

    public int CDisplayWordLength
    {
        get
        {
            string text = LDisplayWordRead();
            return _cDisplayWordUnit < text.Length ? StringInfo.GetNextTextElementLength(text, _cDisplayWordUnit) : 0;
        }
    }

    public string CDisplayWordLead
    {
        get
        {
            string text = LDisplayWordRead();
            int head = Math.Clamp(_cDisplayWordHead, 0, text.Length);
            return text[head..Math.Clamp(_cDisplayWordUnit, head, text.Length)];
        }
    }

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

        return CDisplayChipOpen(link);
    }

    public bool CDisplayChipOpen(long? link)
    {
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
        return LDisplayMentionOpen(_cDisplayCard.CDisplayCardShown, sentence, text, unit);
    }

    public void CDisplayWordStart(long sentence)
    {
        CLecternCard card = _cDisplayCard.CDisplayCardRead();
        CLeafLine? line = card.CLecternCardMeanings.Concat(card.CLecternCardOther)
            .SelectMany(static leaf => leaf.CLeafSentence.Concat(
                leaf.CLeafCollocation.SelectMany(static row => row.CLeafSentence)))
            .FirstOrDefault(found => found.CLeafLineSentence == sentence);
        _cDisplayWordShown = _cDisplayCard.CDisplayCardShown;
        _cDisplayWordSentence = line?.CLeafLineSentence;
        _cDisplayWordPiece = [.. line?.CLeafLinePiece.Select(static piece => piece.CMentionPieceText) ?? []];
        _cDisplayWordUnit = 0;
        _cDisplayWordHead = 0;
    }

    public void CDisplayWordMove(int step)
    {
        int[] starts = StringInfo.ParseCombiningCharacters(LDisplayWordRead());
        if (starts.Length == 0)
        {
            return;
        }

        int index = Math.Max(Array.IndexOf(starts, _cDisplayWordUnit), 0);
        _cDisplayWordUnit = starts[Math.Clamp(index + Math.Sign(step), 0, starts.Length - 1)];
        _cDisplayWordHead = LDisplayStopRead().LastOrDefault(stop => stop <= _cDisplayWordUnit);
    }

    public void CDisplayWordSeek(int step)
    {
        IReadOnlyList<int> stops = LDisplayStopRead();
        int unit = _cDisplayWordUnit;
        _cDisplayWordUnit = step > 0
            ? stops.FirstOrDefault(stop => stop > unit, unit)
            : stops.LastOrDefault(stop => stop < unit, unit);
        _cDisplayWordHead = _cDisplayWordUnit;
    }

    public CMentionOffer? CDisplayWordFind()
    {
        if (_cDisplayWordSentence is not long sentence)
        {
            return null;
        }

        CMentionOffer? offer = LDisplayMentionOpen(_cDisplayWordShown, sentence, LDisplayWordRead(), _cDisplayWordUnit);
        _cDisplayWordHead = Math.Min(offer?.CMentionOfferUnit ?? _cDisplayWordHead, _cDisplayWordUnit);
        return offer;
    }

    private string LDisplayWordRead()
    {
        return string.Concat(_cDisplayWordPiece);
    }

    private List<int> LDisplayStopRead()
    {
        string text = LDisplayWordRead();
        SortedSet<int> stops = [0];
        int at = 0;
        foreach (string piece in _cDisplayWordPiece)
        {
            stops.Add(at);
            at += piece.Length;
        }

        for (int index = 1; index < text.Length; index++)
        {
            if (text[index] != ' ' && text[index - 1] == ' ')
            {
                stops.Add(index);
            }
        }

        return [.. stops.Where(stop => stop < Math.Max(text.Length, 1))];
    }

    private CMentionOffer? LDisplayMentionOpen(LEntryDraft? minted, long sentence, string text, int unit)
    {
        if (_cDisplayMention is not CMention mention
            || _cDisplayVoice.CVoiceShown is not LEntryDraft shown
            || !ReferenceEquals(shown, minted))
        {
            return null;
        }

        try
        {
            int offset = mention.LMentionOffsetRead(text, unit);
            return mention.LMentionResultOpen(
                CMention.CMentionResultRead(_cDisplayMentionPort.LEngineMentionFind(shown, sentence, offset)), text);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cDisplayEnvoy, _cDisplaySettings, "Mention.FindFailed", exception);
            return null;
        }
    }
}
