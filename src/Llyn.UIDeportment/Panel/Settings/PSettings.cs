using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class PSettings : UserControl
{
    private PWindow _pSettingsHost = null!;

    private CLedgerState _pSettingsState = null!;

    private CAtelier PSettingsAtelier => _pSettingsHost.PWindowAtelier;

    private QPosture PSettingsPosture => _pSettingsHost.PWindowPosture;

    public PSettings()
    {
        UserControl surface = (UserControl)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Panel/Settings/PSettings.xaml", UriKind.Relative));
        Content = surface;
        NameScope.SetNameScope(this, NameScope.GetNameScope(surface));

        PLocalization.SelectedValuePath = "Tag";

        PWorkspaceDialogIcon.QIconSource = QIcon.QIconResolve("folder", 24);
        PWinnow.SetResourceReference(QField.QFieldHintProperty, "Settings.Search");
        PDialFolder.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("folder", 24));
        PDialWidth.SetValue(QLook.QLookIconProperty, QIcon.QIconResolve("sort", 24));

        PWinnow.TextChanged += PWinnowHandle;
        PWorkspacePath.KeyDown += PWorkspaceEscapeRefine;
        PWorkspacePath.KeyDown += PWorkspacePathObserve;
        PWorkspacePath.LostKeyboardFocus += PWorkspaceFocusRefine;
        PWorkspaceDialog.Click += PWorkspaceDialogObserve;
        PDialFolder.Click += PDialFolderHandle;
        PLocalization.SelectionChanged += PLocalizationObserve;
        PRespelling.Click += PRespellingObserve;
        PSettingsEpithet.Click += PSettingsEpithetHandle;
        PFrequency.Click += PFrequencyObserve;
        PMorphology.Click += PMorphologyObserve;
        PLayoutLinked.Click += PLayoutLinkedObserve;
        PDialWidth.Click += PDialWidthHandle;
    }

    private TextBox PWinnow => (TextBox)FindName(nameof(PWinnow));

    private ItemsControl PLedger => (ItemsControl)FindName(nameof(PLedger));

    private TextBlock PLedgerEmpty => (TextBlock)FindName(nameof(PLedgerEmpty));

    private StackPanel PDialWorkspace => (StackPanel)FindName(nameof(PDialWorkspace));

    private TextBox PWorkspacePath => (TextBox)FindName(nameof(PWorkspacePath));

    private Button PWorkspaceDialog => (Button)FindName(nameof(PWorkspaceDialog));

    private QIconImage PWorkspaceDialogIcon => (QIconImage)FindName(nameof(PWorkspaceDialogIcon));

    private Button PDialFolder => (Button)FindName(nameof(PDialFolder));

    private StackPanel PDialLanguage => (StackPanel)FindName(nameof(PDialLanguage));

    private ComboBox PLocalization => (ComboBox)FindName(nameof(PLocalization));

    private StackPanel PDialTranscription => (StackPanel)FindName(nameof(PDialTranscription));

    private ToggleButton PRespelling => (ToggleButton)FindName(nameof(PRespelling));

    private StackPanel PDialListing => (StackPanel)FindName(nameof(PDialListing));

    private ToggleButton PSettingsEpithet => (ToggleButton)FindName(nameof(PSettingsEpithet));

    private StackPanel PDialWeb => (StackPanel)FindName(nameof(PDialWeb));

    private ToggleButton PFrequency => (ToggleButton)FindName(nameof(PFrequency));

    private ToggleButton PMorphology => (ToggleButton)FindName(nameof(PMorphology));

    private StackPanel PDialLayout => (StackPanel)FindName(nameof(PDialLayout));

    private ToggleButton PLayoutLinked => (ToggleButton)FindName(nameof(PLayoutLinked));

    private Button PDialWidth => (Button)FindName(nameof(PDialWidth));

    internal void PSettingsAttach(PWindow host)
    {
        _pSettingsHost = host;
        QLookItem.QLookItemAttach(PLedger, PLedgerApply);
        PSettingsPosture.QPostureLinkedChanged += PLedgerMetaApply;
        PSettingsAtelier.CAtelierLedger.CLedgerChanged += LObserver.LObserverCreate<CLedgerState>(this, PSettingsShow);
        PDialShow("Workspace");
    }

    private void PLocalizationBuild(IReadOnlyList<KeyValuePair<string, string>> languages)
    {
        PLocalization.Items.Clear();
        foreach (KeyValuePair<string, string> language in languages)
        {
            PLocalization.Items.Add(new ComboBoxItem
            {
                Content = language.Value,
                Tag = language.Key,
            });
        }
    }

    private void PSettingsShow(CLedgerState state)
    {
        _pSettingsState = state;
        QLocalizationCatalog.QLocalizationCatalogApply(
            System.Windows.Application.Current.Resources, state.CLedgerStateTexts);
        if (PLocalization.Items.Count == 0)
        {
            PLocalizationBuild(state.CLedgerStateLanguages);
        }

        CSettings settings = state.CLedgerStateSettings;
        PWorkspacePath.Text = state.CLedgerStatePath;
        PLocalization.SelectedValue = state.CLedgerStateLocalization;
        PRespelling.IsChecked = settings.CSettingsRespelled;
        PSettingsEpithet.IsChecked = settings.CSettingsEpithet;
        PFrequency.IsChecked = settings.CSettingsFrequency;
        PMorphology.IsChecked = settings.CSettingsMorphology;
        PLayoutLinked.IsChecked = PSettingsPosture.QPostureRead().LCapsuleContentLinked;
        PLedgerShow(state.CLedgerStatePages);
    }
}
