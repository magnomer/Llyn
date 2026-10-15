using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QDial
{
    private readonly FrameworkElement _qDialSettings;

    private readonly QPosture _qDialPosture;

    private CLedger _qDialLedger = null!;

    private CEnvoy _qDialEnvoy = null!;

    internal QDial(FrameworkElement settings, QPosture posture)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(posture);

        _qDialSettings = settings;
        _qDialPosture = posture;
    }

    private StackPanel QDialWorkspace => QContract.QContractFind<StackPanel>(_qDialSettings, "PDialWorkspace");

    private StackPanel QDialLanguage => QContract.QContractFind<StackPanel>(_qDialSettings, "PDialLanguage");

    private StackPanel QDialTranscription =>
        QContract.QContractFind<StackPanel>(_qDialSettings, "PDialTranscription");

    private StackPanel QDialListing => QContract.QContractFind<StackPanel>(_qDialSettings, "PDialListing");

    private StackPanel QDialInflection => QContract.QContractFind<StackPanel>(_qDialSettings, "PDialInflection");

    private StackPanel QDialWeb => QContract.QContractFind<StackPanel>(_qDialSettings, "PDialWeb");

    private StackPanel QDialOutpost => QContract.QContractFind<StackPanel>(_qDialSettings, "PDialOutpost");

    private StackPanel QDialLayout => QContract.QContractFind<StackPanel>(_qDialSettings, "PDialLayout");

    private Button QDialFolder => QContract.QContractFind<Button>(_qDialSettings, "PDialFolder");

    private ToggleButton QDialEpithet => QContract.QContractFind<ToggleButton>(_qDialSettings, "PSettingsEpithet");

    private Button QDialWidth => QContract.QContractFind<Button>(_qDialSettings, "PDialWidth");

    private TextBox QDialNotebook => QContract.QContractFind<TextBox>(_qDialSettings, "PSettingsNotebook");

    private TextBox QDialDepot => QContract.QContractFind<TextBox>(_qDialSettings, "PSettingsDepot");

    private Button QDialDepotDialog => QContract.QContractFind<Button>(_qDialSettings, "PSettingsDepotDialog");

    private QIconImage QDialDepotIcon => QContract.QContractFind<QIconImage>(_qDialSettings, "PSettingsDepotIcon");

    internal void QDialIntroduce(CLedger ledger, CEnvoy envoy)
    {
        _qDialLedger = ledger;
        _qDialEnvoy = envoy;
        QDialFolder.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("folder", 24));
        QDialWidth.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("sort", 24));
        QDialFolder.Click += QDialFolderObserve;
        QDialEpithet.Click += QDialEpithetObserve;
        QDialWidth.Click += QDialWidthObserve;
        QDialNotebook.KeyDown += QDialCommitObserve;
        QDialNotebook.LostKeyboardFocus += QDialNotebookObserve;
        QDialDepotIcon.QIconSource = QIcon.QIconResolve("folder", 24);
        QDialDepot.KeyDown += QDialDepotCommit;
        QDialDepot.LostKeyboardFocus += QDialDepotObserve;
        QDialDepotDialog.Click += QDialDepotOpen;
    }

    internal void QDialRefine(string child)
    {
        foreach ((string name, StackPanel page) in QDialTableRead())
        {
            page.Visibility = string.Equals(name, child, StringComparison.Ordinal)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }

    internal void QDialSettingsRefine(CSettings settings)
    {
        QDialEpithet.IsChecked = settings.CSettingsEpithet;
        QDialNotebookRefine(settings.CSettingsNotebook);
        QDialDepotRefine(settings.CSettingsDepot);
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
            ("Sync", QDialOutpost),
            ("Layout", QDialLayout)
        ];
    }

    private void QDialEpithetObserve(object sender, RoutedEventArgs e)
    {
        _qDialLedger.CLedgerEpithetSave(QDialEpithet.IsChecked == true, _qDialEnvoy);
    }

    private void QDialCommitObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        e.Handled = true;
        QDialNotebookRefine(_qDialLedger.CLedgerNotebookSave(QDialNotebook.Text, _qDialEnvoy));
    }

    private void QDialNotebookObserve(object sender, KeyboardFocusChangedEventArgs e)
    {
        QDialNotebookRefine(_qDialLedger.CLedgerNotebookSave(QDialNotebook.Text, _qDialEnvoy));
    }

    private void QDialNotebookRefine(string? notebook)
    {
        if (notebook is not null)
        {
            QDialNotebook.Text = notebook;
        }
    }

    private void QDialDepotCommit(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        e.Handled = true;
        QDialDepotRefine(_qDialLedger.CLedgerDepotSave(QDialDepot.Text, _qDialEnvoy));
    }

    private void QDialDepotObserve(object sender, KeyboardFocusChangedEventArgs e)
    {
        QDialDepotRefine(_qDialLedger.CLedgerDepotSave(QDialDepot.Text, _qDialEnvoy));
    }

    private void QDialDepotOpen(object sender, RoutedEventArgs e)
    {
        QDialDepotRefine(_qDialLedger.CLedgerDepotChange(_qDialEnvoy));
    }

    private void QDialDepotRefine(string? depot)
    {
        if (depot is not null)
        {
            QDialDepot.Text = depot;
        }
    }

    private void QDialFolderObserve(object sender, RoutedEventArgs e)
    {
        _qDialLedger.CLedgerFolderOpen(_qDialEnvoy);
    }

    private void QDialWidthObserve(object sender, RoutedEventArgs e)
    {
        _qDialPosture.QPostureLayoutReset();
    }
}
