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

    private readonly ToggleButton _qLecternSoundHinge;

    public QLecternReflex(FrameworkElement surface, CDisplaySound area)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(area);

        _qLecternReflexArea = area;
        ItemsControl reflex = QContract.QContractFind<ItemsControl>(surface, "PDisplayReflex");
        _qLecternSoundLoading = QContract.QContractFind<TextBlock>(surface, "PDisplayReflexLoading");
        _qLecternSoundHinge = QContract.QContractFind<ToggleButton>(surface, "PDisplayReflexHinge");
        _qLecternReflexList = new QReflexList(reflex, _qLecternSoundHinge);

        _qLecternSoundHinge.Click += QLecternHingeObserve;
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

    private void QLecternHingeObserve(object sender, RoutedEventArgs e)
    {
        QLecternHingeRefine(
            _qLecternReflexArea.CDisplayReflexToggle(QLook.QLookCheckedRead(_qLecternSoundHinge.IsChecked)));
    }

    private void QLecternHingeRefine(bool stored)
    {
        if (!stored)
        {
            _qLecternSoundHinge.IsChecked = !QLook.QLookCheckedRead(_qLecternSoundHinge.IsChecked);
        }
    }

    private void QLecternReflexRefine(CLecternReflex reflex)
    {
        _qLecternReflexList.QReflexListShow(reflex.CLecternReflexRows, reflex.CLecternReflexFoldable);
        _qLecternReflexList.QReflexAnchorRefine(reflex.CLecternReflexAnchor);
        _qLecternSoundLoading.Visibility = QLook.QLookVisibleRead(reflex.CLecternReflexPending);
    }
}
