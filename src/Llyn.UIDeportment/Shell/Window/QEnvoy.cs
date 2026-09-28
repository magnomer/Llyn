using System;
using System.Globalization;
using System.Windows;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QEnvoy : CEnvoy
{
    private readonly Window _qEnvoySurface;

    private readonly PWindow _qEnvoyHost;

    internal QEnvoy(Window surface, PWindow host)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(host);

        _qEnvoySurface = surface;
        _qEnvoyHost = host;
    }

    public bool CEnvoyConfirm(string key)
    {
        return MessageBox.Show(
            _qEnvoySurface,
            QLocalizationCatalog.QLocalizationTextRead(key),
            QLocalizationCatalog.QLocalizationTextRead("Terms.Product"),
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning) == MessageBoxResult.Yes;
    }

    public bool CEnvoyConfirm(string key, string tallyKey, int tally)
    {
        string count = string.Concat(
            QLocalizationCatalog.QLocalizationTextRead(tallyKey), " ", tally.ToString(CultureInfo.CurrentCulture));

        return MessageBox.Show(
            _qEnvoySurface,
            $"{QLocalizationCatalog.QLocalizationTextRead(key)}\n\n{count}",
            QLocalizationCatalog.QLocalizationTextRead("Terms.Product"),
            MessageBoxButton.YesNo,
            MessageBoxImage.Question) == MessageBoxResult.Yes;
    }

    public bool CEnvoyUnionConfirm(string key, string dropped, string kept)
    {
        string confirm = QLocalizationCatalog.QLocalizationTextRead(key);
        string question = $"{confirm}\n\n{dropped} \u2192 {kept}";

        return MessageBox.Show(
            _qEnvoySurface,
            question,
            QLocalizationCatalog.QLocalizationTextRead("Terms.Product"),
            MessageBoxButton.YesNo,
            MessageBoxImage.Question) == MessageBoxResult.Yes;
    }

    public void CEnvoyFailureShow(string key, Exception exception)
    {
        _qEnvoyHost.PWindowFailureShow(key, exception);
    }

    public void CEnvoyFailureShow(string key)
    {
        MessageBox.Show(
            _qEnvoySurface,
            QLocalizationCatalog.QLocalizationTextRead(key),
            QLocalizationCatalog.QLocalizationTextRead("Terms.Product"),
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }

    public bool CEnvoyDiscardConfirm()
    {
        return _qEnvoyHost.PWindowDiscardConfirm();
    }

    public bool? CEnvoyLeaveConfirm()
    {
        return QSLeave.QSLeaveShow(_qEnvoySurface) switch
        {
            QSLeaveAnswer.QSLeaveAnswerStore => true,
            QSLeaveAnswer.QSLeaveAnswerDiscard => false,
            _ => null,
        };
    }
}
