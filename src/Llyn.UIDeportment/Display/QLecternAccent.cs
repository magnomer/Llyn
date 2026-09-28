using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QLecternAccent
{
    private readonly CDisplay _qLecternAccentArea;

    private readonly ObservableCollection<QAccentItem> _qLecternAccentRow = [];

    private UIElement _qLecternAccentSurface = null!;

    private ColumnDefinition _qLecternAccentLead = null!;

    private Image _qLecternAccentFlag = null!;

    private TextBlock _qLecternAccentLabel = null!;

    private TextBlock _qLecternAccentOpener = null!;

    private TextBlock _qLecternAccentPronunciation = null!;

    private TextBlock _qLecternAccentCloser = null!;

    private DependencyObject _qLecternAccentContour = null!;

    private DependencyProperty _qLecternAccentTonal = null!;

    public QLecternAccent(CDisplay area)
    {
        ArgumentNullException.ThrowIfNull(area);

        _qLecternAccentArea = area;
    }

    public void QLecternAccentIntroduce(
        UIElement surface,
        ColumnDefinition lead,
        Image flag,
        TextBlock label,
        TextBlock opener,
        TextBlock pronunciation,
        TextBlock closer,
        ItemsControl accents,
        DependencyObject contour,
        DependencyProperty tonal)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(lead);
        ArgumentNullException.ThrowIfNull(flag);
        ArgumentNullException.ThrowIfNull(label);
        ArgumentNullException.ThrowIfNull(opener);
        ArgumentNullException.ThrowIfNull(pronunciation);
        ArgumentNullException.ThrowIfNull(closer);
        ArgumentNullException.ThrowIfNull(accents);
        ArgumentNullException.ThrowIfNull(contour);
        ArgumentNullException.ThrowIfNull(tonal);

        _qLecternAccentSurface = surface;
        _qLecternAccentLead = lead;
        _qLecternAccentFlag = flag;
        _qLecternAccentLabel = label;
        _qLecternAccentOpener = opener;
        _qLecternAccentPronunciation = pronunciation;
        _qLecternAccentCloser = closer;
        _qLecternAccentContour = contour;
        _qLecternAccentTonal = tonal;
        accents.ItemsSource = _qLecternAccentRow;
        QLookItem.QLookItemAttach(accents, QAccentItem.QAccentItemRefine);
    }

    public void QLecternAccentRefine()
    {
        QLecternAccentRefine(_qLecternAccentArea.CDisplayAccentRead());
    }

    public async void QLecternEnsignRefine()
    {
        QLecternFlagRefine(await LEnsignImage.LEnsignLoad(_qLecternAccentArea.CDisplayEnsignLoad));
    }

    public void QLecternMuteRefine()
    {
        _qLecternAccentSurface.Visibility = Visibility.Collapsed;
        _qLecternAccentLead.SharedSizeGroup = null;
        _qLecternAccentRow.Clear();
        _qLecternAccentFlag.Source = null;
        _qLecternAccentLabel.Text = string.Empty;
    }

    private void QLecternAccentRefine(CLecternAccent accent)
    {
        _qLecternAccentOpener.Text = accent.CLecternAccentMark.CRespellingMarkOpener;
        _qLecternAccentCloser.Text = accent.CLecternAccentMark.CRespellingMarkCloser;
        _qLecternAccentContour.SetValue(_qLecternAccentTonal, accent.CLecternAccentTonal);
        _qLecternAccentPronunciation.Text = accent.CLecternAccentText;
        _qLecternAccentSurface.Visibility = QLook.QLookVisibleRead(accent.CLecternAccentSpoken);
        _qLecternAccentLead.SharedSizeGroup = accent.CLecternAccentSpoken ? "PReadingLabel" : null;

        _qLecternAccentRow.Clear();
        foreach (CAccent row in accent.CLecternAccentRows)
        {
            _qLecternAccentRow.Add(
                QAccentItem.QAccentItemBuild(row, accent.CLecternAccentFlagged, accent.CLecternAccentMark));
        }

        QLecternPrimaryRefine(accent);
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
