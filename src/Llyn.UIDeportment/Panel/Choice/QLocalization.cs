using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QLocalization
{
    private readonly FrameworkElement _qLocalizationSettings;

    private CLedger _qLocalizationLedger = null!;

    private CEnvoy _qLocalizationEnvoy = null!;

    internal QLocalization(FrameworkElement settings)
    {
        _qLocalizationSettings = settings;
        QLocalizationChoice.SelectedValuePath = "Tag";
        QLocalizationChoice.SelectionChanged += QLocalizationObserve;
    }

    private ComboBox QLocalizationChoice =>
        QContract.QContractFind<ComboBox>(_qLocalizationSettings, "PLocalization");

    internal void QLocalizationIntroduce(CLedger ledger, CEnvoy envoy)
    {
        _qLocalizationLedger = ledger;
        _qLocalizationEnvoy = envoy;
    }

    internal void QLocalizationRefine(IReadOnlyList<KeyValuePair<string, string>> languages, string localization)
    {
        QLocalizationChoice.SelectionChanged -= QLocalizationObserve;
        try
        {
            QLocalizationChoiceRefine(languages);
            QLocalizationChoice.SelectedValue = localization;
        }
        finally
        {
            QLocalizationChoice.SelectionChanged += QLocalizationObserve;
        }
    }

    private void QLocalizationChoiceRefine(IReadOnlyList<KeyValuePair<string, string>> languages)
    {
        bool matched = QLocalizationChoice.Items.Count == languages.Count;
        for (int index = 0; matched && index < languages.Count; index++)
        {
            matched = QLocalizationChoice.Items[index] is ComboBoxItem { Content: string shown, Tag: string tag }
                && string.Equals(shown, languages[index].Value, StringComparison.Ordinal)
                && string.Equals(tag, languages[index].Key, StringComparison.Ordinal);
        }

        if (matched)
        {
            return;
        }

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

        _qLocalizationLedger.CLedgerLocalizationSave(language, _qLocalizationEnvoy);
    }
}
