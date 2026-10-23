using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BSCustoms : BSParleyBody
{
    internal const int BSCustomsWidth = 118;
    internal const int BSCustomsHeight = 11;
    private const int BSCustomsWidthWide = 112;
    private const int BSCustomsPage = 7;
    private const int BSCustomsDropPage = 5;

    private static readonly IReadOnlyDictionary<CSCustomsMode, string> BSCustomsChoice =
        new Dictionary<CSCustomsMode, string>
        {
            [CSCustomsMode.CSCustomsModeFresh] = "Customs.Create",
            [CSCustomsMode.CSCustomsModeMerge] = "Customs.Append",
            [CSCustomsMode.CSCustomsModeReplace] = "Customs.Replace",
        };

    private readonly CSCustoms _csCustoms;
    private readonly BLocalizationCatalog _bsCustomsCatalog;
    private readonly BSParleyButton _bsCustomsAdmit;
    private readonly BTable _bsCustomsTable;

    internal BSCustoms(CSCustoms customs, BLocalizationCatalog catalog, BSParleyButton admit)
    {
        ArgumentNullException.ThrowIfNull(customs);
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(admit);

        _csCustoms = customs;
        _bsCustomsCatalog = catalog;
        _bsCustomsAdmit = admit;
        _bsCustomsTable = new BTable(
            customs.CSCustomsEntry.Count,
            BSCustomsPage,
            [(BSCustomsChoiceBuild, BSCustomsModeObserve), (BSCustomsCandidateBuild, BSCustomsTargetObserve)]);
    }

    public int BSParleyBodyWidth => BSCustomsWidth;

    public int BSParleyBodyHeight => BSCustomsHeight;

    public string BSParleyBodyPosition => string.Format(
        CultureInfo.CurrentCulture,
        _bsCustomsCatalog.BLocalizationTextRead("Customs.Position"),
        _bsCustomsTable.BTableRow + 1,
        _csCustoms.CSCustomsEntry.Count);

    public bool BSParleyBodyObserve(ConsoleKeyInfo key)
    {
        return _bsCustomsTable.BTableObserve(key);
    }

    public IEnumerable<BCanvasText> BSParleyBodyDraw(
        IReadOnlyDictionary<string, IReadOnlyList<string>> glyphs,
        int row,
        int column,
        int width,
        bool focused,
        string role,
        BCanvasWidth measure)
    {
        Func<string, int, string> fit = (text, cells) =>
        {
            StringBuilder kept = new();
            int used = 0;
            for (int index = 0, length; index < text.Length; index += length)
            {
                length = StringInfo.GetNextTextElementLength(text, index);
                int size = measure.BCanvasWidthResolve(text.AsSpan(index, length));
                if (used + size > cells)
                {
                    break;
                }

                kept.Append(text, index, length);
                used += size;
            }

            return kept.Append(' ', Math.Max(cells - used, 0)).ToString();
        };

        Func<CSCustomsRow, string> lossText = state => state.CSCustomsRowLoss is string key
            ? string.Format(
                CultureInfo.CurrentCulture,
                _bsCustomsCatalog.BLocalizationTextRead(key),
                state.CSCustomsRowMeaning,
                state.CSCustomsRowCollocation)
            : string.Empty;

        string arrow = glyphs.GetValueOrDefault("Order") is [_, string down] ? down : string.Empty;
        string rule = glyphs.GetValueOrDefault("Rule") is [_, string stroke, _] ? stroke : " ";
        int label = BSCustomsChoice.Values.Max(key => measure.BCanvasWidthResolve(
            _bsCustomsCatalog.BLocalizationTextRead(key)));
        bool wide = width >= BSCustomsWidthWide;
        (int number, int headword, int language, int mode) = (column, column + 5, column + 23, column + 35);
        int target = mode + label + 6;
        int span = wide ? 24 : Math.Max(column + width - target, 4);
        int loss = target + span + 2;

        BLocalizationCatalog catalog = _bsCustomsCatalog;
        yield return new BCanvasText(row, headword, 16, catalog.BLocalizationTextRead("Customs.Headword"), role);
        yield return new BCanvasText(row, mode, label + 4, catalog.BLocalizationTextRead("Customs.Mode"), role);
        yield return new BCanvasText(row, target, span, catalog.BLocalizationTextRead("Customs.Target"), role);
        string ruled = string.Concat(Enumerable.Repeat(rule, width));
        yield return new BCanvasText(row + 1, column, width, ruled, role);
        yield return new BCanvasText(row + 2 + BSCustomsPage, column, width, ruled, role);

        IReadOnlyList<CMarkupEntry> entries = _csCustoms.CSCustomsEntry;
        CSCustomsRow? chosenRow = null;
        int cell = _bsCustomsTable.BTableCell;
        foreach (int index in _bsCustomsTable.BTableRowShown)
        {
            int at = row + 2 + index - _bsCustomsTable.BTableTop;
            CMarkupEntry entry = entries[index];
            CSCustomsRow state = _csCustoms.CSCustomsRowRead(index);
            chosenRow = index == _bsCustomsTable.BTableRow ? state : chosenRow;
            bool lit = focused && index == _bsCustomsTable.BTableRow;
            string shown = state.CSCustomsRowTargeted && entry.CMarkupEntryTarget.Contains(state.CSCustomsRowTarget)
                ? BSCustomsTargetFormat(entry.CMarkupEntryName, state.CSCustomsRowTarget)
                : string.Empty;
            string chosen = _bsCustomsCatalog.BLocalizationTextRead(BSCustomsChoice[state.CSCustomsRowMode]);
            yield return new BCanvasText(
                at, number, 3, $"{(index + 1).ToString(CultureInfo.CurrentCulture),3}", role);
            yield return new BCanvasText(at, headword, 16, fit(entry.CMarkupEntryName, 16), role);
            yield return new BCanvasText(at, language, 10, fit(entry.CMarkupEntryLanguage, 10), role);
            yield return new BCanvasText(
                at, mode, label + 4, $"[{fit(chosen, label)} {arrow}]", lit && cell == 0 ? "lit" : role);
            yield return new BCanvasText(
                at,
                target,
                span,
                $"[{fit(shown, span - 4)} {arrow}]",
                !state.CSCustomsRowTargeted ? "disabled" : lit && cell == 1 ? "lit" : role);
            if (wide)
            {
                yield return new BCanvasText(at, loss, column + width - loss, lossText(state), role);
            }
        }

        if (!wide && chosenRow is CSCustomsRow kept)
        {
            yield return new BCanvasText(row + 3 + BSCustomsPage, column, width, lossText(kept), role);
        }

        if (_bsCustomsTable.BTableDrop is not BDrop drop)
        {
            yield break;
        }

        (int left, int cells) = cell == 0 ? (mode, label + 4) : (target, span);
        int first = row + 2 + _bsCustomsTable.BTableRow - _bsCustomsTable.BTableTop;
        bool above = first - row + drop.BDropLabelShown.Count() >= BSCustomsHeight;
        first += above ? -drop.BDropLabelShown.Count() : 1;
        foreach (int choice in drop.BDropLabelShown)
        {
            yield return new BCanvasText(
                first + choice - drop.BDropTop,
                left,
                cells,
                $" {fit(drop.BDropLabel[choice], cells - 2)} ",
                choice == drop.BDropLit ? "lit" : "menu");
        }
    }

    private static string BSCustomsTargetFormat(string headword, long id)
    {
        return string.Concat(headword, " #", id.ToString(CultureInfo.CurrentCulture));
    }

    private BDrop BSCustomsChoiceBuild(int row)
    {
        CSCustomsRow state = _csCustoms.CSCustomsRowRead(row);
        CSCustomsMode[] modes = [.. BSCustomsChoice.Keys];
        return new BDrop(
            [.. modes.Select(mode => _bsCustomsCatalog.BLocalizationTextRead(BSCustomsChoice[mode]))],
            Array.IndexOf(modes, state.CSCustomsRowMode),
            true,
            BSCustomsDropPage);
    }

    private BDrop BSCustomsCandidateBuild(int row)
    {
        CSCustomsRow state = _csCustoms.CSCustomsRowRead(row);
        CMarkupEntry entry = _csCustoms.CSCustomsEntry[row];
        IReadOnlyList<long> ids = entry.CMarkupEntryTarget;
        return new BDrop(
            [.. ids.Select(id => BSCustomsTargetFormat(entry.CMarkupEntryName, id))],
            ids.ToList().IndexOf(state.CSCustomsRowTarget),
            state.CSCustomsRowTargeted,
            BSCustomsDropPage);
    }

    private void BSCustomsModeObserve(int row, int choice)
    {
        _bsCustomsAdmit.BSParleyButtonEnabled =
            _csCustoms.CSCustomsModeSet(row, BSCustomsChoice.Keys.ElementAt(choice));
    }

    private void BSCustomsTargetObserve(int row, int choice)
    {
        _bsCustomsAdmit.BSParleyButtonEnabled =
            _csCustoms.CSCustomsTargetSet(row, _csCustoms.CSCustomsEntry[row].CMarkupEntryTarget[choice]);
    }
}
