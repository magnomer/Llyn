using System;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QLecternFrequency
{
    private readonly CDisplay _qLecternFrequencyArea;

    private readonly StackPanel _qLecternFrequencySection;

    private readonly Border _qLecternChip;

    private readonly TextBlock _qLecternName;

    private readonly TextBlock _qLecternBand;

    public QLecternFrequency(FrameworkElement surface, CDisplay display)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(display);

        _qLecternFrequencyArea = display;
        _qLecternFrequencySection = QContract.QContractFind<StackPanel>(surface, "PDisplayFrequencySection");
        _qLecternChip = QContract.QContractFind<Border>(surface, "PDisplayFrequencyChip");
        _qLecternName = QContract.QContractFind<TextBlock>(surface, "PDisplayFrequency");
        _qLecternBand = QContract.QContractFind<TextBlock>(surface, "PDisplayFrequencyBand");

        display.CDisplayOpened += QLecternFrequencyRefine;
        display.CDisplayClosed += QLecternFrequencyRefine;
        display.CDisplayFrequencyChanged += QObserver.QObserverCreate<CBulletin>(surface, QLecternFrequencyRefine);
    }

    private void QLecternFrequencyRefine()
    {
        QFrequencyLabel.QFrequencyChipRefine(
            _qLecternFrequencySection,
            _qLecternChip,
            _qLecternName,
            _qLecternBand,
            _qLecternFrequencyArea.CDisplayFrequencyRead(QLocalizationCatalog.QLocalizationTextRead));
    }
}
