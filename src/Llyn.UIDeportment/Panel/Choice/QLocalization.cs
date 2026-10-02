using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QLocalization
{
    private readonly FrameworkElement _qLocalizationSettings;

    private CLedger _qLocalizationLedger = null!;

    internal QLocalization(FrameworkElement settings)
    {
        _qLocalizationSettings = settings;
        QLocalizationChoice.SelectedValuePath = "Tag";
        QLocalizationChoice.SelectionChanged += QLocalizationObserve;
    }

    private ComboBox QLocalizationChoice =>
        QContract.QContractFind<ComboBox>(_qLocalizationSettings, "PLocalization");

    internal void QLocalizationIntroduce(CLedger ledger)
    {
        _qLocalizationLedger = ledger;
    }

    internal void QLocalizationRefine(IReadOnlyList<KeyValuePair<string, string>> languages, string localization)
    {
        if (QLocalizationChoice.Items.Count == 0)
        {
            QLocalizationChoiceRefine(languages);
        }

        QLocalizationChoice.SelectedValue = localization;
    }

    private void QLocalizationChoiceRefine(IReadOnlyList<KeyValuePair<string, string>> languages)
    {
        QLocalizationChoice.Items.Clear();
        foreach (KeyValuePair<string, string> language in languages)
        {
            QLocalizationChoice.Items.Add(new ComboBoxItem
            {
                Content = language.Value,
                Tag = language.Key,
            });
        }
    }

    private void QLocalizationObserve(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox { SelectedValue: string language })
        {
            return;
        }

        _qLocalizationLedger.CLedgerLocalizationSave(language);
    }
}
