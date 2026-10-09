using System;
using System.Windows;
using System.Windows.Controls;

namespace Llyn.UIDeportment;

internal sealed class QDial
{
    private readonly FrameworkElement _qDialSettings;

    internal QDial(FrameworkElement settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        _qDialSettings = settings;
    }

    private StackPanel QDialWorkspace => QContract.QContractFind<StackPanel>(_qDialSettings, "PDialWorkspace");

    private StackPanel QDialLanguage => QContract.QContractFind<StackPanel>(_qDialSettings, "PDialLanguage");

    private StackPanel QDialTranscription =>
        QContract.QContractFind<StackPanel>(_qDialSettings, "PDialTranscription");

    private StackPanel QDialListing => QContract.QContractFind<StackPanel>(_qDialSettings, "PDialListing");

    private StackPanel QDialInflection => QContract.QContractFind<StackPanel>(_qDialSettings, "PDialInflection");

    private StackPanel QDialWeb => QContract.QContractFind<StackPanel>(_qDialSettings, "PDialWeb");

    private StackPanel QDialLayout => QContract.QContractFind<StackPanel>(_qDialSettings, "PDialLayout");

    internal void QDialRefine(string child)
    {
        foreach ((string name, StackPanel page) in QDialTableRead())
        {
            page.Visibility = string.Equals(name, child, StringComparison.Ordinal)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }

    private (string QDialChild, StackPanel QDialPage)[] QDialTableRead()
    {
        return
        [
            ("Workspace", QDialWorkspace),
            ("Language", QDialLanguage),
            ("Transcription", QDialTranscription),
            ("Listing", QDialListing),
            ("Inflection", QDialInflection),
            ("Web", QDialWeb),
            ("Layout", QDialLayout)
        ];
    }
}
