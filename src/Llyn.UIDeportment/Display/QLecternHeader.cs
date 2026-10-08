using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QLecternHeader
{
    private readonly CDisplay _qLecternHeaderArea;

    private readonly CAtelier _qLecternAtelier;

    private readonly CEnvoy _qLecternEnvoy;

    private readonly TextBlock _qLecternHeadword;

    private readonly TextBlock _qLecternLanguage;

    private readonly Image _qLecternFlag;

    private readonly Ellipse _qLecternGlobe;

    private readonly ToggleButton _qLecternFavorite;

    public QLecternHeader(FrameworkElement surface, CDisplay display, CAtelier atelier, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(envoy);

        _qLecternHeaderArea = display;
        _qLecternAtelier = atelier;
        _qLecternEnvoy = envoy;
        _qLecternHeadword = QContract.QContractFind<TextBlock>(surface, "PDisplayHeadword");
        _qLecternLanguage = QContract.QContractFind<TextBlock>(surface, "PDisplayLanguage");
        _qLecternFlag = QContract.QContractFind<Image>(surface, "PDisplayLanguageFlag");
        _qLecternGlobe = QContract.QContractFind<Ellipse>(surface, "PDisplayLanguageGlobe");
        _qLecternFavorite = QContract.QContractFind<ToggleButton>(surface, "PDisplayFavorite");

        _qLecternFavorite.Click += QLecternFavoriteObserve;
        display.CDisplayOpened += QLecternHeaderRefine;
        display.CDisplayOpened += QLecternFontRefine;
        display.CDisplayOpened += QLecternFlagRefine;
        display.CDisplayOpened += QLecternFavoriteRefine;
        display.CDisplayClosed += QLecternHeaderRefine;
        display.CDisplayClosed += QLecternFontRefine;
        display.CDisplayClosed += QLecternFlagRefine;
        display.CDisplayClosed += QLecternFavoriteRefine;
        display.CDisplayFavoriteChanged += QObserver.QObserverCreate<CBulletin>(surface, QLecternFavoriteRefine);
    }

    private void QLecternFavoriteObserve(object sender, RoutedEventArgs e)
    {
        QLecternFavoriteRefine(
            _qLecternHeaderArea.CDisplayFavoriteToggle(QLook.QLookCheckedRead(_qLecternFavorite.IsChecked)));
    }

    private void QLecternHeaderRefine()
    {
        CLectern shown = _qLecternHeaderArea.CDisplayShown;
        _qLecternHeadword.Text = shown.CLecternHeadword;
        _qLecternLanguage.Text = shown.CLecternLanguage;
    }

    private void QLecternFontRefine()
    {
        QFontFace.QFontRefine(
            _qLecternHeaderArea.CDisplaySound.CDisplayFontRead(CFontRole.CFontRoleHeadword), _qLecternHeadword);
        QFontFace.QFontBaselineRefine(_qLecternHeadword);
    }

    private async void QLecternFlagRefine()
    {
        QEnsignImage.QEnsignFlagRefine(_qLecternFlag, _qLecternGlobe, string.Empty);

        await _qLecternAtelier.CAtelierCatalog.CCatalogEnsignLoad(_qLecternEnvoy, QEnsignImage.QEnsignDraw);

        QEnsignImage.QEnsignFlagRefine(
            _qLecternFlag, _qLecternGlobe, _qLecternHeaderArea.CDisplayShown.CLecternLanguage);
    }

    private void QLecternFavoriteRefine()
    {
        QLecternFavoriteRefine(_qLecternHeaderArea.CDisplayFavoriteRead());
    }

    private void QLecternFavoriteRefine(bool marked)
    {
        _qLecternFavorite.IsChecked = marked;
    }
}
