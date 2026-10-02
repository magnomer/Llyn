using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QVolume
{
    private readonly List<Slider> _qVolumeSlider = [];

    private readonly List<MediaPlayer> _qVolumePlayer = [];

    private readonly CAtelier _cAtelier;

    internal QVolume(CAtelier atelier)
    {
        _cAtelier = atelier;
        PVolumeCatalog.PVolumeCatalogCurrent.PropertyChanged += QVolumeLevelRefine;
        atelier.CAtelierWorkspace.CWorkspaceOpened += QVolumeRefine;
    }

    internal void QVolumeSliderAttach(FrameworkElement surface)
    {
        Slider slider = QContract.QContractFind<Slider>(surface, "PVolume");
        slider.Value = PVolumeCatalog.PVolumeCatalogCurrent.PVolumeCatalogLevel;
        slider.ValueChanged += QVolumeObserve;
        slider.AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler(QVolumeSaveObserve));
        slider.AddHandler(UIElement.MouseUpEvent, new MouseButtonEventHandler(QVolumeSaveObserve), true);
        slider.AddHandler(UIElement.KeyUpEvent, new KeyEventHandler(QVolumeSaveObserve), true);
        _qVolumeSlider.Add(slider);
    }

    internal void QVolumePlayerAttach(MediaPlayer player)
    {
        _qVolumePlayer.Add(player);
    }

    private void QVolumeObserve(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        PVolumeCatalog.PVolumeCatalogCurrent.PVolumeCatalogLevel = e.NewValue;
        _cAtelier.CAtelierVolumeSet(e.NewValue, false);
    }

    private void QVolumeLevelRefine(object? sender, PropertyChangedEventArgs e)
    {
        QVolumeShow(PVolumeCatalog.PVolumeCatalogCurrent.PVolumeCatalogLevel);
    }

    private void QVolumeSaveObserve(object sender, RoutedEventArgs e)
    {
        _cAtelier.CAtelierVolumeSet(((RangeBase)sender).Value, true);
    }

    private void QVolumeRefine()
    {
        double level = _cAtelier.CAtelierVolumeRead();
        PVolumeCatalog.PVolumeCatalogCurrent.PVolumeCatalogLevel = level;
        QVolumeShow(level);
    }

    private void QVolumeShow(double level)
    {
        foreach (Slider slider in _qVolumeSlider)
        {
            slider.ValueChanged -= QVolumeObserve;
            slider.Value = level;
            slider.ValueChanged += QVolumeObserve;
        }

        foreach (MediaPlayer player in _qVolumePlayer)
        {
            player.Volume = level;
        }
    }
}
