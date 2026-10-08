using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QLecternEtymology
{
    private readonly CDisplayCard _qLecternEtymologyArea;

    private readonly CDisplayRoute _qLecternEtymologyRoute;

    private readonly QEtymology _qLecternCardEtymology;

    private readonly UIElement _qLecternCardOrigin;

    public QLecternEtymology(FrameworkElement surface, CDisplayCard area, CDisplayRoute route)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(area);
        ArgumentNullException.ThrowIfNull(route);

        _qLecternEtymologyArea = area;
        _qLecternEtymologyRoute = route;
        _qLecternCardEtymology = QContract.QContractFind<QEtymology>(surface, "PDisplayEtymology");
        _qLecternCardOrigin = QContract.QContractFind<StackPanel>(surface, "PDisplayEtymologySection");

        surface.CommandBindings.Add(new CommandBinding(
            PEtymologyCommand.PEtymologyCommandEntry, QLecternEtymonObserve));
        _qLecternCardEtymology.AddHandler(
            PMention.PMentionClickEvent, new EventHandler<PMentionArgument>(QLecternEtymologyObserve));
    }

    internal event Action<PMention, CMentionOffer?>? QLecternEtymologyNotice;

    public void QLecternEtymologyRefine()
    {
        CLecternEtymology etymology = _qLecternEtymologyArea.CDisplayEtymologyRead();
        _qLecternCardEtymology.QEtymologyText = etymology.CLecternEtymologyText;
        _qLecternCardEtymology.QEtymologyNarrated = etymology.CLecternEtymologyNarrated;
        _qLecternCardEtymology.QEtymologySourceShow(
            etymology.CLecternEtymologyTargets
                .Select(static target => new PEtymon(
                    target.CTranslationTargetId, target.CTranslationTargetHeadword, target.CTranslationTargetLanguage))
                .ToList(),
            etymology.CLecternEtymologyLinked);
        _qLecternCardEtymology.Visibility = QLook.QLookVisibleRead(etymology.CLecternEtymologyShown);
        _qLecternCardOrigin.Visibility = QLook.QLookVisibleRead(etymology.CLecternEtymologyDerived);
    }

    private void QLecternEtymonObserve(object sender, ExecutedRoutedEventArgs e)
    {
        _qLecternEtymologyRoute.CDisplayChipOpen(null, e.Parameter as long?);
    }

    private void QLecternEtymologyObserve(object? sender, PMentionArgument e)
    {
        ArgumentNullException.ThrowIfNull(e);

        QLecternEtymologyNotice?.Invoke(
            e.PMentionArgumentOrigin,
            _qLecternEtymologyRoute.CDisplayEtymologyFind(e.PMentionArgumentText, e.PMentionArgumentUnit));
    }
}
