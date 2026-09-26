using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.UIDeportment;

public class PAutograph : UserControl
{
    private LGuild _lGuild = null!;

    public PAutograph()
    {
        UserControl surface = (UserControl)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Panel/Guild/PAutograph.xaml", UriKind.Relative));
        Content = surface;
        NameScope.SetNameScope(this, NameScope.GetNameScope(surface));

        PAutographName.TextChanged += PAutographNameHandle;
        PAutographUnion.TextChanged += PAutographUnionHandle;
        PAutographUnionList.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(PAutographUnionSelect));

        PLookItem.PLookItemAttach(PAutographUnionList, PRollItem.PRollItemApply);
    }

    private TextBox PAutographName => (TextBox)FindName(nameof(PAutographName));

    private TextBlock PAutographWork => (TextBlock)FindName(nameof(PAutographWork));

    private TextBlock PAutographTally => (TextBlock)FindName(nameof(PAutographTally));

    private StackPanel PAutographUnionBody => (StackPanel)FindName(nameof(PAutographUnionBody));

    private TextBox PAutographUnion => (TextBox)FindName(nameof(PAutographUnion));

    private ItemsControl PAutographUnionList => (ItemsControl)FindName(nameof(PAutographUnionList));

    private TextBlock PAutographUnionNotice => (TextBlock)FindName(nameof(PAutographUnionNotice));

    internal void PAutographAttach(LGuild guild)
    {
        _lGuild = guild;
        _lGuild.LGuildAutograph.LDeskStarted += PAutographStartUpdate;
        PAutographObserverAttach(_lGuild.LGuildAutograph);
        _lGuild.LGuildAutograph.LDeskDraftChanged += PAutographDraftUpdate;
    }

    internal void PAutographTallyShow(LVita vita)
    {
        ArgumentNullException.ThrowIfNull(vita);

        PAutographWork.Text = vita.LVitaWork;
        PAutographTally.Text = vita.LVitaTally;
    }

    internal void PAutographModeUpdate()
    {
        PAutographUnionBody.Visibility = PLook.PLookVisibleRead(_lGuild.LGuildUnionShown);
        PAutographUnionNotice.Visibility = PLook.PLookVisibleRead(!_lGuild.LGuildUnionShown);
    }

    private void PAutographObserverAttach(LDesk desk)
    {
        desk.LDeskDraftAttach(LSubject.LSubjectDraft, LObserver.LObserverCreate(this, desk.LDeskDraftUpdate));
        desk.LDeskDraftAttach(LSubject.LSubjectTenure, LObserver.LObserverCreate(this, desk.LDeskStateUpdate));
    }

    private void PAutographStartUpdate()
    {
        PAutographUnion.Text = string.Empty;
        PAutographUnionList.ItemsSource = null;
        PAutographName.Focus();
    }

    private void PAutographDraftUpdate(LDraft draft)
    {
        PAutographName.Text = draft.LDraftAuthorName;
    }

    private void PAutographNameHandle(object sender, TextChangedEventArgs e)
    {
        _lGuild.LGuildAutograph.LDeskDefer(
            new LRequestAuthorName(_lGuild.LGuildAutograph.LDeskId, PAutographName.Text));
    }

    private void PAutographUnionHandle(object sender, TextChangedEventArgs e)
    {
        PAutographUnionList.ItemsSource = PRollItem.PRollItemBuild(_lGuild.LGuildUnionRead(PAutographUnion.Text));
    }

    private void PAutographUnionSelect(object sender, RoutedEventArgs e)
    {
        _lGuild.LGuildUnionSelect(PSender.PSenderSourceRead<PRollItem>(e)?.PRollItemId);
    }
}
