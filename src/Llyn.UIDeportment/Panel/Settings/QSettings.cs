using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QSettings
{
    private readonly FrameworkElement _qSettingsSurface;

    private readonly QWorkspace _qSettingsWorkspace;

    private readonly QLocalization _qSettingsLocalization;

    private readonly QRespelling _qSettingsRespelling;

    private readonly QFrequency _qSettingsFrequency;

    private readonly QCourier _qSettingsCourier;

    private readonly QMorphology _qSettingsMorphology;

    private readonly QLayout _qSettingsLayout;

    private readonly List<QLedgerItem> _qSettingsList = [];

    private QWindow _qSettingsHost = null!;

    internal QSettings(FrameworkElement surface, QLayout layout)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(layout);

        _qSettingsSurface = surface;
        _qSettingsWorkspace = new QWorkspace(surface);
        _qSettingsLocalization = new QLocalization(surface);
        _qSettingsRespelling = new QRespelling(surface);
        _qSettingsFrequency = new QFrequency(surface);
        _qSettingsCourier = new QCourier(surface);
        _qSettingsMorphology = new QMorphology(surface);
        _qSettingsLayout = layout;
    }

    private CAtelier QSettingsAtelier => _qSettingsHost.QWindowAtelier;

    private QPosture QSettingsPosture => _qSettingsHost.QWindowPosture;

    private TextBox QSettingsWinnow => QContract.QContractFind<TextBox>(_qSettingsSurface, "PWinnow");

    private ItemsControl QSettingsLedger => QContract.QContractFind<ItemsControl>(_qSettingsSurface, "PLedger");

    private TextBlock QSettingsEmpty => QContract.QContractFind<TextBlock>(_qSettingsSurface, "PLedgerEmpty");

    private StackPanel QSettingsWorkspacePage =>
        QContract.QContractFind<StackPanel>(_qSettingsSurface, "PDialWorkspace");

    private Button QSettingsFolder => QContract.QContractFind<Button>(_qSettingsSurface, "PDialFolder");

    private StackPanel QSettingsLanguagePage =>
        QContract.QContractFind<StackPanel>(_qSettingsSurface, "PDialLanguage");

    private StackPanel QSettingsTranscriptionPage =>
        QContract.QContractFind<StackPanel>(_qSettingsSurface, "PDialTranscription");

    private StackPanel QSettingsListingPage =>
        QContract.QContractFind<StackPanel>(_qSettingsSurface, "PDialListing");

    private ToggleButton QSettingsEpithet =>
        QContract.QContractFind<ToggleButton>(_qSettingsSurface, "PSettingsEpithet");

    private StackPanel QSettingsWebPage => QContract.QContractFind<StackPanel>(_qSettingsSurface, "PDialWeb");

    private StackPanel QSettingsLayoutPage => QContract.QContractFind<StackPanel>(_qSettingsSurface, "PDialLayout");

    private Button QSettingsWidth => QContract.QContractFind<Button>(_qSettingsSurface, "PDialWidth");

    internal void QSettingsIntroduce(QWindow host)
    {
        _qSettingsHost = host;
        QSettingsWinnow.SetResourceReference(QField.QFieldHintProperty, "Settings.Search");
        QSettingsFolder.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("folder", 24));
        QSettingsWidth.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("sort", 24));
        QSettingsWinnow.TextChanged += QSettingsWinnowObserve;
        QSettingsFolder.Click += QSettingsFolderObserve;
        QSettingsEpithet.Click += QSettingsEpithetObserve;
        QSettingsWidth.Click += QSettingsWidthObserve;

        CLedger ledger = QSettingsAtelier.CAtelierLedger;
        _qSettingsWorkspace.QWorkspaceIntroduce(QSettingsAtelier, host.QWindowEnvoy);
        _qSettingsLocalization.QLocalizationIntroduce(ledger, host.QWindowEnvoy);
        _qSettingsRespelling.QRespellingIntroduce(ledger, host.QWindowEnvoy);
        _qSettingsFrequency.QFrequencyIntroduce(ledger, host.QWindowEnvoy);
        _qSettingsCourier.QCourierIntroduce(ledger, host.QWindowAtelier, host.QWindowEnvoy);
        _qSettingsMorphology.QMorphologyIntroduce(ledger, host.QWindowEnvoy);
        _qSettingsLayout.QLayoutIntroduce(_qSettingsSurface);
        QLookItem.QLookItemAttach(QSettingsLedger, QSettingsItemRefine);
        QSettingsPosture.QPostureLinkedChanged += QSettingsMetaRefine;
        ledger.CLedgerChanged += QObserver.QObserverCreate<CLedgerState>(_qSettingsSurface, QSettingsRefine);
        QSettingsDialRefine("Workspace");
    }

    private void QSettingsRefine(CLedgerState state)
    {
        QLocalizationCatalog.QLocalizationCatalogApply(
            System.Windows.Application.Current.Resources, state.CLedgerStateTexts);
        CSettings settings = state.CLedgerStateSettings;
        _qSettingsWorkspace.QWorkspacePathRefine(state.CLedgerStatePath);
        _qSettingsLocalization.QLocalizationRefine(state.CLedgerStateLanguages, state.CLedgerStateLocalization);
        _qSettingsRespelling.QRespellingRefine(settings.CSettingsRespelled);
        QSettingsEpithet.IsChecked = settings.CSettingsEpithet;
        _qSettingsFrequency.QFrequencyRefine(settings.CSettingsFrequency);
        _qSettingsCourier.QCourierOutpostRefine(settings.CSettingsOutpost);
        _qSettingsMorphology.QMorphologyRefine(settings.CSettingsMorphology);
        _qSettingsLayout.QLayoutLinkedRefine();
        QSettingsLedgerRefine(state);
    }

    private void QSettingsEpithetObserve(object sender, RoutedEventArgs e)
    {
        QSettingsAtelier.CAtelierLedger.CLedgerEpithetSave(
            QSettingsEpithet.IsChecked == true, _qSettingsHost.QWindowEnvoy);
    }

    private (string QSettingsChild, StackPanel QSettingsPage)[] QSettingsTableRead()
    {
        return
        [
            ("Workspace", QSettingsWorkspacePage),
            ("Language", QSettingsLanguagePage),
            ("Transcription", QSettingsTranscriptionPage),
            ("Listing", QSettingsListingPage),
            ("Web", QSettingsWebPage),
            ("Layout", QSettingsLayoutPage)
        ];
    }

    private void QSettingsDialRefine(string child)
    {
        foreach (QLedgerItem item in _qSettingsList)
        {
            item.QLedgerItemChosen = string.Equals(item.QLedgerItemChild, child, StringComparison.Ordinal);
        }

        foreach ((string name, StackPanel page) in QSettingsTableRead())
        {
            page.Visibility = string.Equals(name, child, StringComparison.Ordinal)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }

    private void QSettingsFolderObserve(object sender, RoutedEventArgs e)
    {
        QSettingsAtelier.CAtelierLedger.CLedgerFolderOpen(_qSettingsHost.QWindowEnvoy);
    }

    private void QSettingsWidthObserve(object sender, RoutedEventArgs e)
    {
        QSettingsPosture.QPostureLayoutReset();
    }

    private void QSettingsLedgerRefine(CLedgerState state)
    {
        foreach (CLedgerPage page in state.CLedgerStatePages)
        {
            QSettingsPageRefine(page);
        }

        QSettingsFindRefine(state.CLedgerStateShown);
        QSettingsMetaRefine();
    }

    private void QSettingsPageRefine(CLedgerPage page)
    {
        QLedgerItem? item = _qSettingsList.Find(
            item => string.Equals(item.QLedgerItemChild, page.CLedgerPageChild, StringComparison.Ordinal));
        if (item is null)
        {
            item = new QLedgerItem(page.CLedgerPageChild, page.CLedgerPageTitle);
            _qSettingsList.Add(item);
        }

        item.QLedgerItemTitle = page.CLedgerPageTitle;
        item.QLedgerItemMeta = page.CLedgerPageMeta;
    }

    private void QSettingsMetaRefine()
    {
        QSettingsPageRefine(QSettingsAtelier.CAtelierLedger.CLedgerMetaRead(
            QSettingsPosture.QPostureRead().LCapsuleContentLinked));
    }

    private void QSettingsFindRefine(CLedgerShown shown)
    {
        QSettingsLedger.ItemsSource = _qSettingsList
            .Where(item => shown.CLedgerShownChildren.Contains(item.QLedgerItemChild, StringComparer.Ordinal))
            .ToList();
        QSettingsEmpty.Visibility = shown.CLedgerShownEmpty ? Visibility.Visible : Visibility.Collapsed;
    }

    private void QSettingsRowRefine(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement row || row.DataContext is not QLedgerItem item)
        {
            return;
        }

        QSettingsDialRefine(item.QLedgerItemChild);
    }

    private void QSettingsItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QLedgerItem ledger)
        {
            return;
        }

        if (QLook.QLookPartFind<Button>(container, "PLedgerRow") is Button row)
        {
            if (ledger.QLedgerItemChosen)
            {
                row.SetValue(QLook.QLookCueProperty, QLookCue.QLookCueChosen);
            }
            else
            {
                row.ClearValue(QLook.QLookCueProperty);
            }

            row.Click -= QSettingsRowRefine;
            row.Click += QSettingsRowRefine;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PLedgerTitle") is TextBlock title)
        {
            title.Text = ledger.QLedgerItemTitle;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PLedgerMeta") is TextBlock meta)
        {
            meta.Text = ledger.QLedgerItemMeta;
        }
    }

    private void QSettingsWinnowObserve(object sender, TextChangedEventArgs e)
    {
        QSettingsFindRefine(QSettingsAtelier.CAtelierLedger.CLedgerFind(QSettingsWinnow.Text));
    }
}
