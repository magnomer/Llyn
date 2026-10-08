using System;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QLecternEntry
{
    private readonly CDisplay _qLecternEntryArea;

    private readonly CAtelier _qLecternAtelier;

    private readonly StackPanel _qLecternStamp;

    private readonly TextBlock _qLecternAdded;

    private readonly TextBlock _qLecternUpdated;

    private readonly StackPanel _qLecternSpeechSection;

    private readonly ItemsControl _qLecternSpeech;

    private readonly TextBlock _qLecternUnit;

    private readonly StackPanel _qLecternNoteSection;

    private readonly StackPanel _qLecternNote;

    public QLecternEntry(FrameworkElement surface, CDisplay display, CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(atelier);

        _qLecternEntryArea = display;
        _qLecternAtelier = atelier;
        _qLecternStamp = QContract.QContractFind<StackPanel>(surface, "PDisplayStampSection");
        _qLecternAdded = QContract.QContractFind<TextBlock>(surface, "PDisplayStampAdded");
        _qLecternUpdated = QContract.QContractFind<TextBlock>(surface, "PDisplayStampUpdated");
        _qLecternSpeechSection = QContract.QContractFind<StackPanel>(surface, "PDisplaySpeechSection");
        _qLecternSpeech = QContract.QContractFind<ItemsControl>(surface, "PDisplaySpeech");
        _qLecternUnit = QContract.QContractFind<TextBlock>(surface, "PDisplayUnit");
        _qLecternNoteSection = QContract.QContractFind<StackPanel>(surface, "PDisplayNoteSection");
        _qLecternNote = QContract.QContractFind<StackPanel>(surface, "PDisplayNote");

        display.CDisplayOpened += QLecternEntryRefine;
        display.CDisplayClosed += QLecternEntryRefine;
        display.CDisplayOpened += QLecternNoteRefine;
    }

    private void QLecternEntryRefine()
    {
        CLectern shown = _qLecternEntryArea.CDisplayShown;
        _qLecternSpeech.ItemsSource = shown.CLecternSpeeches;
        QLecternUnitRefine(shown.CLecternUnit);
        _qLecternSpeechSection.Visibility = QLook.QLookVisibleRead(shown.CLecternMarked);
        _qLecternNoteSection.Visibility = QLook.QLookVisibleRead(shown.CLecternNoted);
        _qLecternAdded.Text = shown.CLecternAdded;
        _qLecternUpdated.Text = shown.CLecternUpdated;
        _qLecternStamp.Visibility = QLook.QLookVisibleRead(shown.CLecternStamped);
    }

    private void QLecternUnitRefine(string key)
    {
        _qLecternUnit.Visibility = QLook.QLookVisibleRead(key.Length > 0);
        if (key.Length > 0)
        {
            _qLecternUnit.SetResourceReference(TextBlock.TextProperty, key);
        }
    }

    private void QLecternNoteRefine()
    {
        QMarkdownFace.QMarkdownRefine(_qLecternNote, _qLecternEntryArea.CDisplayShown.CLecternNote, _qLecternAtelier);
    }
}
