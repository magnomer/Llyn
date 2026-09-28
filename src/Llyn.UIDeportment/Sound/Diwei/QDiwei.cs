using System;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QDiwei
{
    private readonly UserControl _qDiweiSurface;

    private CAtelier _qDiweiAtelier = null!;

    private CYunjing _cYunjing = null!;

    internal QDiwei(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qDiweiSurface = surface;
        surface.CommandBindings.Add(new CommandBinding(QDiweiCommand.QDiweiCommandEntry, QDiweiEntryHandle));
        surface.CommandBindings.Add(new CommandBinding(QDiweiCommand.QDiweiCommandSwitch, QDiweiSwitchHandle));

        QLookItem.QLookItemAttach(QDiweiList, QDiweiItem.QDiweiItemApply);
    }

    private TextBlock QDiweiHeadword => QContract.QContractFind<TextBlock>(_qDiweiSurface, "PDiweiHeadword");

    private TextBlock QDiweiKind => QContract.QContractFind<TextBlock>(_qDiweiSurface, "PDiweiKind");

    private Image QDiweiFlag => QContract.QContractFind<Image>(_qDiweiSurface, "PDiweiFlag");

    private TextBlock QDiweiLanguage => QContract.QContractFind<TextBlock>(_qDiweiSurface, "PDiweiLanguage");

    private TextBlock QDiweiEmpty => QContract.QContractFind<TextBlock>(_qDiweiSurface, "PDiweiEmpty");

    private ItemsControl QDiweiList => QContract.QContractFind<ItemsControl>(_qDiweiSurface, "PDiweiList");

    internal void QDiweiAttach(CAtelier atelier, CYunjing yunjing)
    {
        _qDiweiAtelier = atelier;
        _cYunjing = yunjing;
    }

    internal void QDiweiShow(CDiweiPage page, string kind)
    {
        LFontFace.LFontRefine(
            _qDiweiAtelier,
            page.CDiweiPageLanguage,
            [CFontRole.CFontRoleHeadword, CFontRole.CFontRoleGlyph],
            [QDiweiHeadword, QDiweiList]);
        LFontFace.LFontPlace(QDiweiHeadword);
        QDiweiHeadword.Text = page.CDiweiPageKey;
        QDiweiKind.SetResourceReference(TextBlock.TextProperty, kind);
        QDiweiLanguage.Text = page.CDiweiPageLanguage;
        QDiweiFlag.Source = LEnsignImage.LEnsignFind(page.CDiweiPageLanguage);
        QDiweiList.ItemsSource = QDiweiItem.QDiweiItemBuild(page.CDiweiPageSections);
        QDiweiEmpty.Visibility = QLook.QLookVisibleRead(page.CDiweiPageEmpty);
    }

    private void QDiweiSwitchHandle(object sender, ExecutedRoutedEventArgs e)
    {
        _cYunjing.CYunjingTallyToggle(QSender.QSenderFlagRead(e));
    }

    private void QDiweiEntryHandle(object sender, ExecutedRoutedEventArgs e)
    {
        _cYunjing.CYunjingGlyphSelect(QSender.QSenderTextRead(e));
    }
}
