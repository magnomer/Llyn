using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BCard
{
    private static readonly string[] BCardHint = ["Display.Meaning", "Display.CollocationSingle"];

    private readonly BLocalizationCatalog _bCardCatalog;

    private readonly (Action, Action, Action<bool, long>, Action<bool, long, int>) _bCardGate;

    private readonly Func<int, (IReadOnlyList<CStep>, IReadOnlyList<int>)> _bCardWalk;

    private readonly Func<(int, int, int)?> _bCardLit;

    private readonly Action<int> _bCardLight;

    private readonly BKeymapRow _bCardTitleRow;

    private readonly BKeymapRow _bCardWordingRow;

    private readonly BCardField _bCardTitle;

    private readonly BCardBadge _bCardBadge;

    internal BCard(
        BLocalizationCatalog catalog,
        (Action, Action, Action<bool, long>, Action<bool, long, int>, CCardField) gates,
        (Func<bool, long, bool>, Func<bool, long, string, bool>, Action<bool, long>) position,
        (Func<int, (IReadOnlyList<CStep>, IReadOnlyList<int>)>, Func<(int, int, int)?>) sight,
        Func<Func<bool>, Func<BKeymapLive>> when,
        (Action<int>, Action, Action) host)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(gates.Item1);
        ArgumentNullException.ThrowIfNull(gates.Item2);
        ArgumentNullException.ThrowIfNull(gates.Item3);
        ArgumentNullException.ThrowIfNull(gates.Item4);
        ArgumentNullException.ThrowIfNull(gates.Item5);
        ArgumentNullException.ThrowIfNull(sight.Item1);
        ArgumentNullException.ThrowIfNull(sight.Item2);
        ArgumentNullException.ThrowIfNull(when);
        ArgumentNullException.ThrowIfNull(host.Item1);
        ArgumentNullException.ThrowIfNull(host.Item2);
        ArgumentNullException.ThrowIfNull(host.Item3);

        _bCardCatalog = catalog;
        _bCardGate = (gates.Item1, gates.Item2, gates.Item3, gates.Item4);
        (_bCardWalk, _bCardLit) = sight;
        Action changed;
        Action keeper;
        (_bCardLight, changed, keeper) = host;
        CCardField field = gates.Item5;
        _bCardTitle = new BCardField(catalog, field.CCardTitleSet, keeper);
        BCardWording = new BCardField(
            catalog,
            (collocated, id, text) =>
                (collocated ? (Action<bool, long, string>)field.CCardExpressionSet : field.CCardMeaningSet)(
                    collocated, id, text),
            keeper);
        _bCardBadge = new BCardBadge(position);

        const BKeymapFocus side = BKeymapFocus.BKeymapFocusEditor;
        Func<Action, Action<ConsoleKeyInfo>> pressed = action => _ =>
        {
            action();
            changed();
        };
        Func<bool> shut = () => _bCardTitle.BCardFieldCard is null && BCardWording.BCardFieldCard is null
            && _bCardBadge.BCardBadgeCard is null;
        Func<bool> idle = () => BCardHeadRead() is not null && shut();
        Func<BKeymapLive> header = when(idle);
        _bCardTitleRow = new(ConsoleKey.Enter, 0, side, "Editor.Open", _ => BCardTitleOpen(), header);
        _bCardWordingRow = new(ConsoleKey.Enter, 0, side, "Editor.Open", pressed(BCardWordingOpen),
            when(() => BCardHeadRead(1) is not null && shut()));
        BCardBond = new BBond(
            catalog,
            (field.CCardSenseRead, field.CCardSenseAttach, field.CCardSenseDetach),
            () => shut() && BCardHeadRead(2) is (true, { CStepCard: CCardDraft card }) ? card : null,
            when,
            changed);
        Func<BKeymapLive> folding = when(() => idle() && BCardHeadRead() is (_, { CStepFold: not null }));
        Func<BKeymapLive> several = when(() => idle() && BCardListRead().Count > 1);
        Func<int, Func<BKeymapLive>> adding = stack => when(() => idle() && _bCardLit()?.Item2 == stack);
        Func<int, Func<BKeymapLive>> adder = stack => when(() => _bCardLit() is (int row, int at, _)
            && at == stack && row - BEditor.BEditorHeadSpan == _bCardWalk(at).Item1.Count);
        Func<BKeymapLive> titled = when(() => _bCardTitle.BCardFieldCard is not null
            || BCardWording.BCardFieldCard is not null);
        Func<BKeymapLive> badged = when(() => _bCardBadge.BCardBadgeCard is not null);
        Func<int, Func<BKeymapLive>> seeking = way => when(() => BCardSeekRead(way) is not null);
        Action<ConsoleKeyInfo> meaning = pressed(() => BCardAdd(_bCardGate.Item1));
        Action<ConsoleKeyInfo> collocation = pressed(() => BCardAdd(_bCardGate.Item2));
        Action<ConsoleKeyInfo> settle = pressed(BCardBadgeSettle);
        Action<ConsoleKeyInfo> close = pressed(() =>
        {
            _bCardTitle.BCardFieldClose();
            BCardWording.BCardFieldClose();
        });
        Func<int, Action<ConsoleKeyInfo>> move = way => pressed(() => BCardMove(way));
        Func<int, Action<ConsoleKeyInfo>> seek = way => pressed(() => BCardSeek(way));
        BCardKeymap =
        [
            new(ConsoleKey.Enter, 0, side, "Card.Fold", pressed(BCardFoldToggle), folding),
            new(ConsoleKey.Insert, 0, side, "Input.MeaningAdd", meaning, adding(0)),
            new(ConsoleKey.Insert, 0, side, "Input.CollocationAdd", collocation, adding(1)),
            new(ConsoleKey.Enter, 0, side, "Input.MeaningAdd", meaning, adder(0)),
            new(ConsoleKey.Enter, 0, side, "Input.CollocationAdd", collocation, adder(1)),
            new(ConsoleKey.Delete, 0, side, "Card.Remove", pressed(BCardRemove), header),
            new(ConsoleKey.UpArrow, ConsoleModifiers.Control, side, "Card.Move", move(-1), several),
            new(ConsoleKey.DownArrow, ConsoleModifiers.Control, side, "Card.Move", move(1), several),
            new(ConsoleKey.NoName, 0, side, "Card.Position", pressed(BCardBadgeOpen), several)
            {
                BKeymapRowSign = '#',
            },
            new(ConsoleKey.Enter, 0, side, "Card.PositionSettle", settle, badged),
            new(ConsoleKey.Escape, 0, side, "Card.PositionClose", pressed(_bCardBadge.BCardBadgeClose), badged),
            new(ConsoleKey.UpArrow, 0, side, "Card.PositionSettle", settle, badged),
            new(ConsoleKey.DownArrow, 0, side, "Card.PositionSettle", settle, badged),
            new(ConsoleKey.Enter, 0, side, "Editor.Close", close, titled),
            new(ConsoleKey.Escape, 0, side, "Editor.Close", close, titled),
            new(ConsoleKey.UpArrow, 0, side, "Editor.Close", close, titled),
            new(ConsoleKey.DownArrow, 0, side, "Editor.Close", close, titled),
            new(ConsoleKey.PageUp, ConsoleModifiers.Control, side, "Editor.Card", seek(-1), seeking(-1)),
            new(ConsoleKey.PageDown, ConsoleModifiers.Control, side, "Editor.Card", seek(1), seeking(1)),
            _bCardWordingRow,
            .. BCardBond.BBondKeymap,
        ];
    }

    internal IReadOnlyList<BKeymapRow> BCardKeymap { get; }

    internal BCardField BCardWording { get; }

    internal BBond BCardBond { get; }

    internal bool BCardObserve(ConsoleKeyInfo key)
    {
        if (_bCardBadge.BCardBadgeCard is not null)
        {
            return _bCardBadge.BCardBadgeObserve(key);
        }

        if (BCardBond.BBondObserve(key))
        {
            return true;
        }

        if (!char.IsControl(key.KeyChar))
        {
            _bCardTitleRow.BKeymapRowAction(key);
            _bCardWordingRow.BKeymapRowAction(key);
        }

        return _bCardTitle.BCardFieldObserve(key) || BCardWording.BCardFieldObserve(key);
    }

    internal void BCardClose()
    {
        _bCardTitle.BCardFieldClose();
        BCardWording.BCardFieldClose();
        BCardBond.BBondReset();
        _bCardBadge.BCardBadgeSettle();
    }

    internal void BCardReset()
    {
        _bCardTitle.BCardFieldReset();
        BCardWording.BCardFieldReset();
        BCardBond.BBondReset();
        _bCardBadge.BCardBadgeClose();
    }

    internal static int BCardLeadRead(bool collocated)
    {
        return collocated ? 2 : 1;
    }


    internal IReadOnlyList<BCanvasText> BCardDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        BCanvasWidth measure,
        (int, int, int) place,
        (bool, CStep) card,
        (string?, bool) look)
    {
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(measure);
        ArgumentNullException.ThrowIfNull(card.Item2);

        (int line, int column, int width) = place;
        (bool collocated, CStep step) = card;
        (string? tone, bool focused) = look;
        CCardDraft draft = step.CStepCard ?? throw new ArgumentException("The step carries no card.", nameof(card));
        string rule = BLecternHeader.BLecternGlyphRead(glyphs, "Rule", 1, "-");
        string ellipsis = BLecternHeader.BLecternGlyphRead(glyphs, "Ellipsis", 0, "~");
        bool folded = draft.CCardDraftFolded;
        CStateWording peek = collocated ? draft.CCardDraftExpression : draft.CCardDraftMeaning;
        CStateWording shown = folded && draft.CCardDraftTitle.CStateWordingMuted && !peek.CStateWordingMuted
            ? peek
            : draft.CCardDraftTitle;
        string caption = shown.CStateWordingMuted ? _bCardCatalog.BLocalizationTextRead(BCardHint[collocated ? 1 : 0])
            : shown.CStateWordingKey is string key ? _bCardCatalog.BLocalizationTextRead(key)
            : shown.CStateWordingText;
        string role = tone ?? (shown.CStateWordingMuted ? "muted" : "heading");
        string hinge = step.CStepFold is not null
            ? " " + BLecternHeader.BLecternGlyphRead(glyphs, "Fold", folded ? 1 : 0, folded ? ">" : "v")
            : string.Empty;
        int hingeCells = measure.BCanvasWidthResolve(hinge);
        string rank = draft.CCardDraftPosition.ToString(CultureInfo.CurrentCulture);
        (bool, long) self = (collocated, draft.CCardDraftId);
        bool badged = _bCardBadge.BCardBadgeCard == self;
        bool titled = _bCardTitle.BCardFieldCard == self;
        if (!badged && !titled)
        {
            string opening = measure.BCanvasWidthShorten(
                $"{rule}{rule} {rank} {rule} {caption} ", Math.Max(width - hingeCells - 1, 0), ellipsis);
            int fill = Math.Max(width - measure.BCanvasWidthResolve(opening) - hingeCells, 0);
            string whole = measure.BCanvasWidthClamp(
                opening + string.Concat(Enumerable.Repeat(rule, fill)) + hinge, width);
            return [new BCanvasText(line, column, measure.BCanvasWidthResolve(whole), whole, role)];
        }

        List<BCanvasText> texts = [];
        int edge = column;
        Action<string> put = text =>
        {
            int cells = Math.Min(measure.BCanvasWidthResolve(text), Math.Max(column + width - edge, 0));
            if (cells > 0)
            {
                texts.Add(new BCanvasText(line, edge, cells, measure.BCanvasWidthClamp(text, cells), role));
            }

            edge += cells;
        };
        put($"{rule}{rule} ");
        if (badged)
        {
            List<BCanvasText> badge = [.. _bCardBadge.BCardBadgeDraw(glyphs, measure, (line, edge, role), focused)];
            texts.AddRange(badge);
            edge = badge.Count > 0 ? badge[^1].BCanvasTextColumn + badge[^1].BCanvasTextSpan : edge;
        }
        else
        {
            put(rank);
        }

        put($" {rule} ");
        int room = Math.Max(column + width - edge - hingeCells - 2, 1);
        if (titled)
        {
            texts.AddRange(_bCardTitle.BCardFieldDraw(glyphs, measure, (line, edge, room), focused));
            edge += room;
        }
        else
        {
            put(measure.BCanvasWidthShorten(caption, room, ellipsis));
        }

        put(" ");
        put(string.Concat(Enumerable.Repeat(rule, Math.Max(column + width - edge - hingeCells, 0))));
        put(hinge);
        return texts;
    }

    private (bool, CStep)? BCardHeadRead(int part = 0)
    {
        if (_bCardLit() is not (int row, int stack, int at) || at != part || row <= BEditor.BEditorHeadSpan)
        {
            return null;
        }

        CStep? step = _bCardWalk(stack).Item1.ElementAtOrDefault(row - BEditor.BEditorHeadSpan);
        return step is { CStepCard: not null } ? (stack == 1, step) : null;
    }

    private IReadOnlyList<int> BCardListRead()
    {
        return _bCardLit() is (_, int stack, _) ? _bCardWalk(stack).Item2 : [];
    }

    private int? BCardSeekRead(int way)
    {
        (int row, _, int part) = _bCardLit() ?? (0, 0, 0);
        IEnumerable<int?> cards = BCardListRead().Select(static at => (int?)(at + BEditor.BEditorHeadSpan));
        return way > 0
            ? cards.FirstOrDefault(at => at > row)
            : cards.LastOrDefault(at => at < row || (at == row && part > 0));
    }

    private void BCardSeek(int way)
    {
        _bCardLight(BCardSeekRead(way).GetValueOrDefault() - (_bCardLit()?.Item1 ?? 0));
    }

    private void BCardTitleOpen()
    {
        (bool collocated, CStep step) = BCardHeadRead().GetValueOrDefault();
        BCardTitleOpen(collocated, step.CStepCard!);
    }

    private void BCardTitleOpen(bool collocated, CCardDraft card)
    {
        _bCardTitle.BCardFieldOpen(
            (collocated, card.CCardDraftId), card.CCardDraftTitle, BCardHint[collocated ? 1 : 0]);
    }

    private void BCardWordingOpen()
    {
        (bool collocated, CStep step) = BCardHeadRead(1).GetValueOrDefault();
        CCardDraft card = step.CStepCard!;
        CStateWording wording = collocated ? card.CCardDraftExpression : card.CCardDraftMeaning;
        BCardWording.BCardFieldOpen((collocated, card.CCardDraftId), wording);
    }

    private void BCardFoldToggle()
    {
        CStep step = BCardHeadRead().GetValueOrDefault().Item2;
        _ = step.CStepFold!(!step.CStepCard!.CCardDraftFolded);
    }

    private void BCardAdd(Action add)
    {
        int before = BCardListRead().Count;
        add();
        BCardWalkRefine(before, null);
    }

    private void BCardRemove()
    {
        (bool collocated, CStep step) = BCardHeadRead().GetValueOrDefault();
        _bCardGate.Item3(collocated, step.CStepCard!.CCardDraftId);
    }

    private void BCardMove(int way)
    {
        (bool collocated, CStep step) = BCardHeadRead().GetValueOrDefault();
        int place = BCardListRead().ToList().IndexOf((_bCardLit()?.Item1 ?? 0) - BEditor.BEditorHeadSpan);
        long card = step.CStepCard!.CCardDraftId;
        _bCardGate.Item4(collocated, card, place + way);
        BCardWalkRefine(null, card);
    }

    private void BCardBadgeOpen()
    {
        (bool collocated, CStep step) = BCardHeadRead().GetValueOrDefault();
        CCardDraft card = step.CStepCard!;
        _bCardBadge.BCardBadgeOpen((collocated, card.CCardDraftId), card.CCardDraftPosition);
    }

    private void BCardBadgeSettle()
    {
        long? card = _bCardBadge.BCardBadgeCard?.Item2;
        _bCardBadge.BCardBadgeSettle();
        BCardWalkRefine(null, card);
    }

    private void BCardWalkRefine(int? arrival, long? follow)
    {
        if (_bCardLit() is not (int row, int stack, _))
        {
            return;
        }

        (IReadOnlyList<CStep> walk, IReadOnlyList<int> cards) = _bCardWalk(stack);
        if (arrival is int before && cards.Count > before && walk[cards[^1]] is { CStepCard: CCardDraft fresh })
        {
            _bCardLight(cards[^1] + BEditor.BEditorHeadSpan - row);
            BCardTitleOpen(stack == 1, fresh);
            return;
        }

        int found = cards.ToList().FindIndex(at => walk[at].CStepCard?.CCardDraftId == follow);
        if (follow is not null && found >= 0)
        {
            _bCardLight(cards[found] + BEditor.BEditorHeadSpan - row);
        }
    }
}
