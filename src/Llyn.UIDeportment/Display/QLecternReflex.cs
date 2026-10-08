using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QLecternReflex
{
    private readonly CDisplaySound _qLecternReflexArea;

    private readonly QReflexList _qLecternReflexList;

    private readonly TextBlock _qLecternSoundLoading;

    private readonly ToggleButton _qLecternSoundFold;

    public QLecternReflex(FrameworkElement surface, CDisplaySound area)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(area);

        _qLecternReflexArea = area;
        ItemsControl reflex = QContract.QContractFind<ItemsControl>(surface, "PDisplayReflex");
        _qLecternSoundLoading = QContract.QContractFind<TextBlock>(surface, "PDisplayReflexLoading");
        _qLecternSoundFold = QContract.QContractFind<ToggleButton>(surface, "PDisplayReflexFold");
        _qLecternReflexList = new QReflexList(reflex, _qLecternSoundFold);

        _qLecternSoundFold.Checked += QLecternFoldObserve;
        _qLecternSoundFold.Unchecked += QLecternFoldObserve;
        _qLecternReflexArea.CDisplayFoldChanged += QLecternFoldRefine;
    }

    public void QLecternReflexRefine()
    {
        QLecternReflexRefine(_qLecternReflexArea.CDisplayReflexRead());
    }

    public void QLecternRenewalRefine()
    {
        QLecternReflexRefine(_qLecternReflexArea.CDisplayReflexResonate());
    }

    public void QLecternFoldRefine()
    {
        _qLecternReflexList.QReflexFoldRefine(_qLecternReflexArea.CDisplayFoldOpened);
    }

    public void QLecternAnchorRefine()
    {
        _qLecternReflexList.QReflexAnchorRefine(_qLecternReflexArea.CDisplayReflexRead().CLecternReflexAnchor);
    }

    private void QLecternFoldObserve(object sender, RoutedEventArgs e)
    {
        _qLecternReflexArea.CDisplayReflexToggle(QLook.QLookCheckedRead(_qLecternSoundFold.IsChecked));
    }

    private void QLecternReflexRefine(CLecternReflex reflex)
    {
        _qLecternReflexList.QReflexListShow(reflex.CLecternReflexRows);
        _qLecternReflexList.QReflexAnchorRefine(reflex.CLecternReflexAnchor);
        _qLecternSoundLoading.Visibility = QLook.QLookVisibleRead(reflex.CLecternReflexPending);
    }
}
