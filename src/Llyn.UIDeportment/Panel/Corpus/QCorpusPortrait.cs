using System;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QCorpusPortrait
{
    private readonly UserControl _qCorpusPortraitScope;

    private CCorpus _cCorpus = null!;

    internal QCorpusPortrait(UserControl scope)
    {
        ArgumentNullException.ThrowIfNull(scope);

        _qCorpusPortraitScope = scope;

        _qCorpusPortraitScope.CommandBindings.Add(
            new CommandBinding(ApplicationCommands.Print, QCorpusPressObserve, QCorpusPressCheck));
        _qCorpusPortraitScope.CommandBindings.Add(new CommandBinding(
            PDisplayCommand.PDisplayCommandPortrait, QCorpusPortraitObserve, QCorpusPortraitCheck));
    }

    internal void QCorpusPortraitIntroduce(CCorpus corpus)
    {
        ArgumentNullException.ThrowIfNull(corpus);

        _cCorpus = corpus;
    }

    private void QCorpusPressCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cCorpus?.CCorpusPressAllowed ?? false;
    }

    private async void QCorpusPressObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cCorpus.CCorpusPortraitPrint();
    }

    private void QCorpusPortraitCheck(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = _cCorpus?.CCorpusPortraitAllowed ?? false;
    }

    private async void QCorpusPortraitObserve(object sender, ExecutedRoutedEventArgs e)
    {
        await _cCorpus.CCorpusQuotation.CQuotationPortraitExport();
    }
}
