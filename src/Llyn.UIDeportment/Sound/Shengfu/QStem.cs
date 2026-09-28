using System;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QStem
{
    private readonly UserControl _qStemSurface;

    private CAtelier _qStemAtelier = null!;

    private LXiesheng _lXiesheng = null!;

    internal QStem(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qStemSurface = surface;
        surface.CommandBindings.Add(new CommandBinding(QStemCommand.QStemCommandEntry, QStemEntryHandle));
    }

    private TextBlock QStemHeadword => QContract.QContractFind<TextBlock>(_qStemSurface, "PStemHeadword");

    private Image QStemFlag => QContract.QContractFind<Image>(_qStemSurface, "PStemFlag");

    private TextBlock QStemLanguage => QContract.QContractFind<TextBlock>(_qStemSurface, "PStemLanguage");

    private TextBlock QStemEmpty => QContract.QContractFind<TextBlock>(_qStemSurface, "PStemEmpty");

    private ItemsControl QStemList => QContract.QContractFind<ItemsControl>(_qStemSurface, "PStemList");

    internal void QStemAttach(CAtelier atelier, LXiesheng xiesheng)
    {
        _qStemAtelier = atelier;
        _lXiesheng = xiesheng;
    }

    internal void QStemShow(CStemPage page)
    {
        LFontFace.LFontRefine(
            _qStemAtelier,
            page.CStemPageLanguage,
            [CFontRole.CFontRoleHeadword, CFontRole.CFontRoleGlyph],
            [QStemHeadword, QStemList]);
        LFontFace.LFontPlace(QStemHeadword);
        QStemHeadword.Text = page.CStemPageKey;
        QStemLanguage.Text = page.CStemPageLanguage;
        QStemFlag.Source = LEnsignImage.LEnsignFind(page.CStemPageLanguage);
        QStemList.ItemsSource = page.CStemPageCharacters;
        QStemEmpty.Visibility = QLook.QLookVisibleRead(page.CStemPageEmpty);
    }

    private void QStemEntryHandle(object sender, ExecutedRoutedEventArgs e)
    {
        _lXiesheng.LXieshengGlyphSelect(QSender.QSenderTextRead(e));
    }
}
