using System;
using System.Windows;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class PWindow
{
    internal void PWindowFailureRefine(string key, Exception exception)
    {
        CLedgerNotice notice = PWindowAtelier.CAtelierLedger.CLedgerNoticeRead(exception);
        string detail = QLocalizationCatalog.QLocalizationTextRead(notice.CLedgerNoticeKey);
        if (notice is { CLedgerNoticeLabel: string label, CLedgerNoticePath: string path })
        {
            detail = $"{detail}\n\n{QLocalizationCatalog.QLocalizationTextRead(label)}\n{path}";
        }

        MessageBox.Show(
            _pWindowSurface,
            $"{QLocalizationCatalog.QLocalizationTextRead(key)}\n\n{detail}",
            QLocalizationCatalog.QLocalizationTextRead("Terms.Product"),
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }
}
