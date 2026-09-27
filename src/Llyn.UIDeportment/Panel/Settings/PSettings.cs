using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;

namespace Llyn.UIDeportment;

public partial class PSettings : UserControl
{
    private PWindow _pSettingsHost = null!;

    private LWindow PSettingsWindow => _pSettingsHost.PWindowDeportment;

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
        PWorkspacePath.KeyDown += PWorkspacePathHandle;
        PWorkspacePath.LostKeyboardFocus += PWorkspaceFocusHandle;
        PWorkspaceDialog.Click += PWorkspaceDialogHandle;
        PDialFolder.Click += PDialFolderHandle;
        PLocalization.SelectionChanged += PLocalizationHandle;
        PRespelling.Click += PRespellingHandle;
        PSettingsEpithet.Click += PSettingsEpithetHandle;
        PFrequency.Click += PFrequencyHandle;
        PMorphology.Click += PMorphologyHandle;
        PLayoutLinked.Click += PLayoutLinkedHandle;
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
        PLocalizationBuild();
        PSettingsWindow.LWindowObserverAttach(
            CSubject.CSubjectSettings, LObserver.LObserverCreate<CBulletin>(this, PSettingsBulletinHandle));
        QLookItem.QLookItemAttach(PLedger, PLedgerApply);

        PSettingsSync();
        PLedgerBuild();
        PDialShow("Workspace");
    }

    private void PLocalizationBuild()
    {
        PLocalization.Items.Clear();
        foreach (string language in PSettingsWindow.LWindowLocalizationScan())
        {
            PLocalization.Items.Add(new ComboBoxItem
            {
                Content = CultureInfo.GetCultureInfo(language).NativeName,
                Tag = language,
            });
        }
    }

    internal void PSettingsSync()
    {
        CSettings settings = PSettingsWindow.LWindowWorkspace.QWorkspaceSettingsRead();
        PWorkspacePath.Text = PSettingsWindow.LWindowWorkspace.QWorkspacePathRead();
        PLocalization.SelectedValue =
            PSettingsWindow.LWindowWorkspace.QWorkspaceLocalizationRead();
        PRespelling.IsChecked = settings.CSettingsRespelled;
        PSettingsEpithet.IsChecked = settings.CSettingsEpithet;
        PFrequency.IsChecked = settings.CSettingsFrequency;
        PMorphology.IsChecked = settings.CSettingsMorphology;
        PLayoutLinked.IsChecked = PSettingsWindow.LWindowPostureRead().CPostureStateLinked;
    }

    private void PSettingsBulletinHandle()
    {
        PLocalizationApply(PSettingsWindow.LWindowWorkspace.QWorkspaceLocalizationRead());
        PLedgerMetaApply();
    }
}
