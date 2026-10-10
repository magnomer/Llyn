using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QStem
{
    private readonly UserControl _qStemSurface;

    private CXiesheng _cXiesheng = null!;

    internal QStem(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qStemSurface = surface;
        surface.CommandBindings.Add(new CommandBinding(QStemCommand.QStemCommandEntry, QStemEntryObserve));
        QLookItem.QLookItemAttach(QStemList, QStemItem.QStemItemRefine);
        QStemList.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(QStemHingeObserve));
    }

    private TextBlock QStemHeadword => QContract.QContractFind<TextBlock>(_qStemSurface, "PStemHeadword");

    private Image QStemFlag => QContract.QContractFind<Image>(_qStemSurface, "PStemFlag");

    private TextBlock QStemLanguage => QContract.QContractFind<TextBlock>(_qStemSurface, "PStemLanguage");

    private TextBlock QStemEmpty => QContract.QContractFind<TextBlock>(_qStemSurface, "PStemEmpty");

    private ItemsControl QStemList => QContract.QContractFind<ItemsControl>(_qStemSurface, "PStemList");

    internal void QStemIntroduce(CXiesheng xiesheng)
    {
        _cXiesheng = xiesheng;
    }

    internal void QStemRefine(CStemPage page)
    {
        QFontFace.QFontRefine(page.CStemPageFont, QStemHeadword);
        QFontFace.QFontRefine(page.CStemPageGlyph, QStemList);
        QFontFace.QFontBaselineRefine(QStemHeadword);
        QStemHeadword.Text = page.CStemPageKey;
        QStemLanguage.Text = page.CStemPageLanguage;
        QStemFlag.Source = QEnsignImage.QEnsignRead(page.CStemPageLanguage);
        QStemList.ItemsSource = QStemItem.QStemItemBuild(page.CStemPageMembers);
        QStemEmpty.Visibility = QLook.QLookVisibleRead(page.CStemPageEmpty);
    }

    private void QStemEntryObserve(object sender, ExecutedRoutedEventArgs e)
    {
        _cXiesheng.CXieshengGlyphSelect(QSender.QSenderTextRead(e));
    }

    private void QStemHingeObserve(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is ToggleButton { DataContext: QStemItem member } hinge)
        {
            QLook.QLookCheckedRefine(
                hinge,
                _cXiesheng.CXieshengFoldToggle(member.QStemItemCharacter, QLook.QLookCheckedRead(hinge.IsChecked)));
        }
    }
}
