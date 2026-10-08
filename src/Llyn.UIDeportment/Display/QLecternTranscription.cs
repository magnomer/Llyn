using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QLecternTranscription
{
    private readonly CDisplaySound _qLecternTranscriptionArea;

    private readonly ObservableCollection<QTranscriptionItem> _qLecternSoundTranscription = [];

    public QLecternTranscription(FrameworkElement surface, CDisplaySound area)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(area);

        _qLecternTranscriptionArea = area;
        ItemsControl transcriptions = QContract.QContractFind<ItemsControl>(surface, "PDisplayTranscription");

        transcriptions.ItemsSource = _qLecternSoundTranscription;
        QLookItem.QLookItemAttach(transcriptions, QTranscriptionItem.QTranscriptionItemRefine);
    }

    public void QLecternTranscriptionRefine()
    {
        _qLecternSoundTranscription.Clear();
        foreach (CTranscriptionDraft spelled in _qLecternTranscriptionArea.CDisplayTranscriptionRead())
        {
            _qLecternSoundTranscription.Add(QTranscriptionItem.QTranscriptionRowRefine(spelled));
        }
    }
}
