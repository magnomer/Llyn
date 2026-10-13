using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QLecternAccent
{
    private readonly CDisplayAccent _qLecternAccentArea;

    private readonly ObservableCollection<QAccentItem> _qLecternAccentRow = [];

    private readonly QLecternLead _qLecternAccentLead;

    private readonly Image _qLecternAccentFlag;

    private readonly TextBlock _qLecternAccentLabel;

    private readonly TextBlock _qLecternAccentOpener;

    private readonly TextBlock _qLecternAccentPronunciation;

    private readonly TextBlock _qLecternAccentCloser;

    private readonly PContour _qLecternAccentContour;

    public QLecternAccent(FrameworkElement surface, CDisplayAccent area)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(area);

        _qLecternAccentArea = area;
        _qLecternAccentLead = new QLecternLead(
            QContract.QContractFind<Border>(surface, "PDisplayPronunciationSurface"),
            QContract.QContractFind<ColumnDefinition>(surface, "PDisplayPronunciationLead"));
        _qLecternAccentFlag = QContract.QContractFind<Image>(surface, "PDisplayPronunciationFlag");
        _qLecternAccentLabel = QContract.QContractFind<TextBlock>(surface, "PDisplayPronunciationLabel");
        _qLecternAccentOpener = QContract.QContractFind<TextBlock>(surface, "PDisplayPronunciationOpener");
        _qLecternAccentPronunciation = QContract.QContractFind<TextBlock>(surface, "PDisplayPronunciation");
        _qLecternAccentCloser = QContract.QContractFind<TextBlock>(surface, "PDisplayPronunciationCloser");
        _qLecternAccentContour = QContract.QContractFind<PContour>(surface, "PDisplayContour");
        ItemsControl accents = QContract.QContractFind<ItemsControl>(surface, "PDisplayAccent");

        _qLecternAccentContour.PContourScale = _qLecternAccentArea.CDisplayAccentScale;
        accents.ItemsSource = _qLecternAccentRow;
        QLookItem.QLookItemAttach(accents, QAccentItem.QAccentItemRefine);
    }

    public void QLecternAccentRefine()
    {
        CLecternAccent accent = _qLecternAccentArea.CDisplayAccentRead();
        _qLecternAccentOpener.Text = accent.CLecternAccentMark.CRespellingMarkOpener;
        _qLecternAccentCloser.Text = accent.CLecternAccentMark.CRespellingMarkCloser;
        _qLecternAccentContour.PContourSyllables =
            QContourInk.QContourInkBuild(accent.CLecternAccentContour, _qLecternAccentContour);
        _qLecternAccentPronunciation.Text = accent.CLecternAccentText;
        _qLecternAccentLead.QLecternLeadRefine(accent.CLecternAccentSpoken);

        _qLecternAccentRow.Clear();
        foreach (CAccent row in accent.CLecternAccentRows)
        {
            _qLecternAccentRow.Add(
                QAccentItem.QAccentItemBuild(row, accent.CLecternAccentFlagged, accent.CLecternAccentMark));
        }

        QLecternPrimaryRefine(accent);
    }

    public async void QLecternEnsignRefine()
    {
        QLecternFlagRefine(await _qLecternAccentArea.CDisplayAccentLoad(QEnsignImage.QEnsignDraw));
    }

    private void QLecternFlagRefine(CLecternAccent? accent)
    {
        if (accent is null)
        {
            return;
        }

        foreach (QAccentItem row in _qLecternAccentRow)
        {
            row.QAccentFlagRefine(accent.CLecternAccentFlagged);
        }

        QLecternPrimaryRefine(accent);
    }

    private void QLecternPrimaryRefine(CLecternAccent accent)
    {
        _qLecternAccentFlag.Source =
            QAccentItem.QAccentEnsignRefine(accent.CLecternAccentPrimary, accent.CLecternAccentFlagged);
        _qLecternAccentLabel.Text = _qLecternAccentFlag.Source is null
            ? QAccentItem.QAccentLabelRefine(accent.CLecternAccentPrimary)
            : string.Empty;
    }
}
