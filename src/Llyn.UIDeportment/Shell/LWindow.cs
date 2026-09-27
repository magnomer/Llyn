using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LWindow
{
    private readonly List<Action> _lWindowVistas = [];

    internal LWindow(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        LWindowAtelier = atelier;
        LWindowWorkspace = new QWorkspace(this);
        LWindowForge = new QForge(this);
        LWindowPosture = new QPosture(LWindowWorkspace.QWorkspacePathRead);
    }

    public QWorkspace LWindowWorkspace { get; }

    public QForge LWindowForge { get; }

    public CAtelier LWindowAtelier { get; }

    public QPosture LWindowPosture { get; }

    public void LWindowVistaRestore()
    {
        foreach (Action restore in _lWindowVistas)
        {
            restore();
        }
    }

    internal LWindowHeld LWindowVistaAdd<LWindowHeld>(LWindowHeld held, Action<LWindowHeld, LWindow> restore)
    {
        restore(held, this);
        _lWindowVistas.Add(() => restore(held, this));
        return held;
    }

    public CFont LWindowFontRead(string language, CFontRole role)
    {
        return LSounding.LSoundingFontRead(
            LWindowAtelier.CAtelierSettingsPort.LEngineFontRead(language, (LFontRole)role));
    }

    public bool LWindowRespellingCheck(string language)
    {
        return LWindowAtelier.CAtelierPhonologyPort.LEngineRespellingCheck(language);
    }

    public bool LWindowPhonemicCheck(string language)
    {
        return LWindowAtelier.CAtelierPhonologyPort.LEnginePhonemicCheck(language);
    }

    public IReadOnlyList<CReflexRule> LWindowReflexRead(string language)
    {
        return LSounding.LSoundingRuleRead(LWindowAtelier.CAtelierPhonologyPort.LEngineReflexRead(language));
    }

    public CSentenceOrder LWindowOrderRead(string language)
    {
        return LCard.LCardOrderRead(LWindowAtelier.CAtelierPhonologyPort.LEngineOrderRead(language));
    }

    public IReadOnlyList<CMentionLabel> LWindowMentionResolve(
        string text, IReadOnlyList<CMentionDraft> mentions, string silent)
    {
        ArgumentNullException.ThrowIfNull(mentions);

        List<LMentionDraft> drafts = new(mentions.Count);
        foreach (CMentionDraft mention in mentions)
        {
            drafts.Add(new LMentionDraft(
                mention.CMentionDraftId,
                mention.CMentionDraftOffset,
                mention.CMentionDraftLength,
                mention.CMentionDraftEntry,
                mention.CMentionDraftSense));
        }

        return LWindowLabelRead(LWindowAtelier.CAtelierEntryPort.LEngineMentionResolve(text, drafts), silent);
    }

    public IReadOnlyList<CMentionPiece> LWindowMentionDivide(string text, IReadOnlyList<CMention> mentions)
    {
        return LSplice.LSpliceBuild(
            LWindowAtelier.CAtelierDraftPort.LEngineMentionDivide(text, LWindowMentionRead(mentions)),
            static piece => new CMentionPiece(
                piece.LMentionPieceOffset,
                piece.LMentionPieceEnd,
                piece.LMentionPieceText,
                piece.LMentionPieceStored?.LMentionLinked));
    }

    public int LWindowUnitRead(string text, int offset)
    {
        return LWindowAtelier.CAtelierDraftPort.LEngineUnitRead(text, offset);
    }

    public int LWindowOffsetRead(string text, int unit)
    {
        return LWindowAtelier.CAtelierDraftPort.LEngineOffsetRead(text, unit);
    }

    public (int LWindowSpanOffset, int LWindowSpanLength) LWindowSpanRead(string text, int start, int length)
    {
        LMentionDraft span = LWindowAtelier.CAtelierDraftPort.LEngineSpanRead(text, start, length);
        return (span.LMentionDraftOffset, span.LMentionDraftLength);
    }

    public bool LWindowSpanCheck(string text, int start, int length)
    {
        return LWindowAtelier.CAtelierDraftPort.LEngineSpanCheck(text, start, length);
    }

    public bool LWindowAnchorMatch(IReadOnlyList<long> one, IReadOnlyList<long> other)
    {
        return LWindowAtelier.CAtelierDraftPort.LEngineAnchorMatch(one, other);
    }

    internal bool LWindowAnchorCheck(IReadOnlyList<LFanqieRow> rows, string headword)
    {
        return LWindowAtelier.CAtelierDraftPort.LEngineAnchorCheck(rows, headword);
    }

    internal string LWindowAnchorFormat(
        IReadOnlyList<LFanqieRow> rows, IReadOnlyList<long> anchors, string headword, string separator)
    {
        return LWindowAtelier.CAtelierDraftPort.LEngineAnchorFormat(rows, anchors, headword, separator);
    }

    public IReadOnlyList<CMarkdownBlock> LWindowMarkdownParse(string? text)
    {
        return LSplice.LSpliceBuild(LWindowAtelier.CAtelierEntryPort.LEngineMarkdownParse(text), LWindowMarkdownRead);
    }

    public IReadOnlyList<string> LWindowSchemeRead(string language)
    {
        return LWindowAtelier.CAtelierPhonologyPort.LEngineSchemeRead(language);
    }

    public IReadOnlyList<CSpeechValue> LWindowSpeechRead(string language)
    {
        return LSplice.LSpliceBuild(
            LWindowAtelier.CAtelierPhonologyPort.LEngineSpeechRead(language),
            static value => new CSpeechValue(value.LSpeechValueId, value.LSpeechValueName));
    }

    public CSpeechValue? LWindowSpeechAdd(string language, string name)
    {
        return LSounding.LSoundingSpeechRead(LWindowAtelier.CAtelierPhonologyPort.LEngineSpeechAdd(language, name));
    }

    public CGlyph? LWindowGlyphRead(string language)
    {
        return LSounding.LSoundingGlyphRead(LWindowAtelier.CAtelierEntryPort.LEngineGlyphRead(language));
    }

    public IReadOnlyList<string> LWindowLocalizationScan()
    {
        return LWindowAtelier.CAtelierSettingsPort.LEngineLocalizationScan();
    }

    private static IReadOnlyList<CMentionLabel> LWindowLabelRead(IReadOnlyList<LMentionLabel> labels, string silent)
    {
        List<CMentionLabel> read = new(labels.Count);
        foreach (LMentionLabel label in labels)
        {
            read.Add(new CMentionLabel(
                label.LMentionLabelId,
                label.LMentionLabelWord,
                label.LMentionLabelLinked ? label.LMentionLabelName : silent,
                label.LMentionLabelSense));
        }

        return read;
    }

    internal static CMarkdownBlock LWindowMarkdownRead(LMarkdownBlock block)
    {
        return new CMarkdownBlock(
            block.LMarkdownBlockHeaded,
            block.LMarkdownBlockListed,
            block.LMarkdownBlockQuoted,
            block.LMarkdownBlockFenced,
            block.LMarkdownBlockRuled,
            block.LMarkdownBlockLevel,
            block.LMarkdownBlockMark,
            block.LMarkdownBlockText,
            LSplice.LSpliceBuild(
                block.LMarkdownBlockSpan,
                static span => new CMarkdownSpan(
                    span.LMarkdownSpanText,
                    span.LMarkdownSpanBold,
                    span.LMarkdownSpanItalic,
                    span.LMarkdownSpanCode,
                    span.LMarkdownSpanAddress)));
    }

    internal static IReadOnlyList<CMention> LWindowMentionRead(IReadOnlyList<LMention> mentions)
    {
        return LSplice.LSpliceBuild(
            mentions,
            static mention => new CMention(
                mention.LMentionId,
                mention.LMentionOffset,
                mention.LMentionLength,
                mention.LMentionEntryId,
                mention.LMentionSenseId));
    }

    internal static IReadOnlyList<LMention> LWindowMentionRead(IReadOnlyList<CMention> mentions)
    {
        return LSplice.LSpliceBuild(
            mentions,
            static mention => new LMention(
                mention.CMentionId,
                mention.CMentionOffset,
                mention.CMentionLength,
                mention.CMentionEntry,
                mention.CMentionSense));
    }
}
