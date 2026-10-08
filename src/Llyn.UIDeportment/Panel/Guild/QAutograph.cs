using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QAutograph
{
    private readonly UserControl _qAutographSurface;

    private CGuild _cGuild = null!;

    internal QAutograph(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qAutographSurface = surface;

        QAutographName.SetResourceReference(QField.QFieldHintProperty, "Autograph.Name");
        QAutographUnion.SetResourceReference(QField.QFieldHintProperty, "Autograph.UnionSearch");
        QAutographName.TextChanged += QAutographNameObserve;
        QAutographUnion.TextChanged += QAutographUnionObserve;
        QAutographUnionList.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QAutographChoiceObserve));

        QLookItem.QLookItemAttach(QAutographUnionList, QRollItem.QRollItemApply);
    }

    private TextBox QAutographName => QContract.QContractFind<TextBox>(_qAutographSurface, "PAutographName");

    private TextBlock QAutographWork => QContract.QContractFind<TextBlock>(_qAutographSurface, "PAutographWork");

    private TextBlock QAutographTally => QContract.QContractFind<TextBlock>(_qAutographSurface, "PAutographTally");

    private StackPanel QAutographUnionBody =>
        QContract.QContractFind<StackPanel>(_qAutographSurface, "PAutographUnionBody");

    private TextBox QAutographUnion => QContract.QContractFind<TextBox>(_qAutographSurface, "PAutographUnion");

    private ItemsControl QAutographUnionList =>
        QContract.QContractFind<ItemsControl>(_qAutographSurface, "PAutographUnionList");

    private TextBlock QAutographUnionNotice =>
        QContract.QContractFind<TextBlock>(_qAutographSurface, "PAutographUnionNotice");

    internal void QAutographIntroduce(CGuild guild)
    {
        _cGuild = guild;
        _cGuild.CGuildUnion.CGuildUnionCleared += QAutographStartRefine;
        _cGuild.CGuildAutograph.CDeskDraft.CDeskDraftChanged += QAutographDraftUpdate;
    }

    internal void QAutographTallyShow(CVita vita)
    {
        ArgumentNullException.ThrowIfNull(vita);

        QAutographWork.Text = vita.CVitaWork;
        QAutographTally.Text = vita.CVitaTally;
    }

    internal void QAutographModeUpdate()
    {
        QAutographUnionBody.Visibility = QLook.QLookVisibleRead(_cGuild.CGuildUnion.CGuildUnionShown);
        QAutographUnionNotice.Visibility = QLook.QLookVisibleRead(!_cGuild.CGuildUnion.CGuildUnionShown);
    }

    private void QAutographStartRefine()
    {
        QAutographUnion.Text = string.Empty;
        QAutographUnionList.ItemsSource = null;
        QAutographName.Focus();
    }

    private void QAutographDraftUpdate(CDraft draft)
    {
        QAutographName.Text = draft.CDraftAuthorName;
    }

    private void QAutographNameObserve(object sender, TextChangedEventArgs e)
    {
        _cGuild.CGuildNameSet(QAutographName.Text);
    }

    private void QAutographUnionObserve(object sender, TextChangedEventArgs e)
    {
        QAutographUnionRefine(_cGuild.CGuildUnion.CGuildUnionRead(QAutographUnion.Text));
    }

    private void QAutographUnionRefine(IReadOnlyList<CCatalogAuthor> rows)
    {
        QAutographUnionList.ItemsSource = QRollItem.QRollItemBuild(rows);
    }

    private void QAutographChoiceObserve(object sender, RoutedEventArgs e)
    {
        _cGuild.CGuildUnion.CGuildUnionSelect(QSender.QSenderSourceRead<QRollItem>(e)?.QRollItemId);
    }
}
