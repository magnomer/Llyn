using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QEnvoy : CEnvoy
{
    private const double QEnvoyInch = 96.0;

    private readonly Window _qEnvoySurface;

    internal QEnvoy(Window surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qEnvoySurface = surface;
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

    public void CEnvoyFailureShow(string key, CLedgerNotice notice)
    {
        ArgumentNullException.ThrowIfNull(notice);

        string detail = QLocalizationCatalog.QLocalizationTextRead(notice.CLedgerNoticeKey);
        if (notice is { CLedgerNoticeLabel: string label, CLedgerNoticePath: string path })
        {
            detail = $"{detail}\n\n{QLocalizationCatalog.QLocalizationTextRead(label)}\n{path}";
        }

        MessageBox.Show(
            _qEnvoySurface,
            $"{QLocalizationCatalog.QLocalizationTextRead(key)}\n\n{detail}",
            QLocalizationCatalog.QLocalizationTextRead("Terms.Product"),
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
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

    public bool? CEnvoyLeaveConfirm()
    {
        return QSLeave.QSLeaveShow(_qEnvoySurface) switch
        {
            QSLeaveAnswer.QSLeaveAnswerStore => true,
            QSLeaveAnswer.QSLeaveAnswerDiscard => false,
            _ => null,
        };
    }

    public string? CEnvoyMarkupRead()
    {
        return QMarkup.QMarkupConsult(_qEnvoySurface);
    }

    public bool CEnvoyCustomsRead(CSCustoms customs)
    {
        return QSCustoms.QSCustomsConsult(_qEnvoySurface, customs);
    }

    public void CEnvoyOmissionShow(IReadOnlyList<CMarkupOmission> omissions)
    {
        QSCustoms.QSCustomsOmissionConsult(_qEnvoySurface, omissions);
    }

    public (string? CEnvoyFile, CPortraitMedium CEnvoyMedium) CEnvoyFileRead(
        string file, IReadOnlyList<CPortraitChoice> choices)
    {
        ArgumentNullException.ThrowIfNull(choices);

        Microsoft.Win32.SaveFileDialog dialog = new()
        {
            Title = QLocalizationCatalog.QLocalizationTextRead("Export.Title"),
            Filter = string.Join(
                "|",
                choices.Select(static choice => string.Concat(
                    QLocalizationCatalog.QLocalizationTextRead(choice.CPortraitChoiceKey),
                    "|*",
                    choice.CPortraitChoiceSuffix))),
            FilterIndex = choices.ToList().FindIndex(static choice => choice.CPortraitChoiceChosen) + 1,
            AddExtension = true,
            FileName = file,
        };

        bool chosen = dialog.ShowDialog(_qEnvoySurface) == true;
        CPortraitMedium medium = choices[Math.Clamp(dialog.FilterIndex - 1, 0, choices.Count - 1)]
            .CPortraitChoiceMedium;

        return (chosen ? dialog.FileName : null, medium);
    }

    public CPressTicket? CEnvoyTicketRead()
    {
        PrintDialog dialog = new()
        {
            UserPageRangeEnabled = false,
        };

        if (dialog.ShowDialog() != true)
        {
            return null;
        }

        PrintTicket chosen = dialog.PrintTicket;

        return new CPressTicket(
            dialog.PrintQueue.FullName,
            chosen.PageMediaSize?.Width / QEnvoyInch,
            chosen.PageMediaSize?.Height / QEnvoyInch,
            chosen.PageOrientation is PageOrientation.Landscape or PageOrientation.ReverseLandscape,
            chosen.CopyCount ?? 1,
            chosen.Collation != Collation.Uncollated,
            chosen.Duplexing switch
            {
                Duplexing.OneSided => CPressSide.CPressSideSingle,
                Duplexing.TwoSidedLongEdge => CPressSide.CPressSideLong,
                Duplexing.TwoSidedShortEdge => CPressSide.CPressSideShort,
                _ => CPressSide.CPressSideDefault,
            },
            chosen.OutputColor switch
            {
                OutputColor.Color => CPressInk.CPressInkColor,
                OutputColor.Grayscale or OutputColor.Monochrome => CPressInk.CPressInkGray,
                _ => CPressInk.CPressInkDefault,
            });
    }

    public string? CEnvoyWorkspaceRead(string workspace)
    {
        Microsoft.Win32.OpenFolderDialog dialog = new()
        {
            Title = QLocalizationCatalog.QLocalizationTextRead("Settings.Workspace"),
            InitialDirectory = workspace,
        };

        return dialog.ShowDialog(_qEnvoySurface) == true ? dialog.FolderName : null;
    }

    public string? CEnvoyCoinageRead(string key)
    {
        return QSCoinage.QSCoinageShow(_qEnvoySurface, key);
    }
}
