using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Application;
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

        PWorkspaceDialogIcon.PIconSource = PIcon.PIconResolve("folder", 24);
        PDialFolder.Tag = PIcon.PIconResolve("folder", 24);
        PDialWidth.Tag = PIcon.PIconResolve("sort", 24);

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

    private PIconImage PWorkspaceDialogIcon => (PIconImage)FindName(nameof(PWorkspaceDialogIcon));

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
        PSettingsWindow.LWindowObserverAttach(LObserver.LObserverCreate(this, PSettingsBulletinHandle));
        PLookItem.PLookItemAttach(PLedger, PLedgerApply);

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
        LSettings settings = PSettingsWindow.LWindowSettingsRead();
        PWorkspacePath.Text = PSettingsWindow.LWindowWorkspaceRead();
        PLocalization.SelectedValue =
            PSettingsWindow.LWindowLocalizationRead();
        PRespelling.IsChecked = settings.LSettingsRespelled;
        PSettingsEpithet.IsChecked = settings.LSettingsEpithet;
        PFrequency.IsChecked = settings.LSettingsFrequency;
        PMorphology.IsChecked = settings.LSettingsMorphology;
        PLayoutLinked.IsChecked = PSettingsWindow.LWindowPostureRead().LPostureStateLinked;
    }

    private void PSettingsBulletinHandle(LBulletin bulletin)
    {
        if (!bulletin.LBulletinMatch(LSubject.LSubjectSettings))
        {
            return;
        }

        PLocalizationApply(PSettingsWindow.LWindowLocalizationRead());
        PLedgerMetaApply();
    }
}
