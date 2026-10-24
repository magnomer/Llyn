using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QEnvoy : CEnvoy
{
    private readonly Window _qEnvoySurface;

    private readonly QPosture _qEnvoyPosture;

    internal QEnvoy(Window surface, QPosture posture)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(posture);

        _qEnvoySurface = surface;
        _qEnvoyPosture = posture;
    }

    private Window QEnvoyOwner =>
        System.Windows.Application.Current is { } application && application.CheckAccess()
            ? application.Windows.OfType<Window>().FirstOrDefault(static window => window.IsActive)
                ?? _qEnvoySurface
            : _qEnvoySurface;

    public bool CEnvoyConfirm(string key)
    {
        return MessageBox.Show(
            QEnvoyOwner,
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
            QEnvoyOwner,
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
            QEnvoyOwner,
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
            QEnvoyOwner,
            $"{QLocalizationCatalog.QLocalizationTextRead(key)}\n\n{detail}",
            QLocalizationCatalog.QLocalizationTextRead("Terms.Product"),
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }

    public void CEnvoyFailureShow(string key)
    {
        MessageBox.Show(
            QEnvoyOwner,
            QLocalizationCatalog.QLocalizationTextRead(key),
            QLocalizationCatalog.QLocalizationTextRead("Terms.Product"),
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }

    public bool? CEnvoyLeaveConfirm()
    {
        return QSLeave.QSLeaveShow(QEnvoyOwner) switch
        {
            QSLeaveAnswer.QSLeaveAnswerStore => true,
            QSLeaveAnswer.QSLeaveAnswerDiscard => false,
            _ => null,
        };
    }

    public string? CEnvoyMarkupRead(string key, IReadOnlyList<string> suffixes)
    {
        return QMarkup.QMarkupConsult(QEnvoyOwner, key, suffixes);
    }

    public bool CEnvoyCustomsRead(CSCustoms customs)
    {
        return QSCustoms.QSCustomsConsult(QEnvoyOwner, customs);
    }

    public bool CEnvoyPressRead(CSPress press)
    {
        return QSPress.QSPressConsult(QEnvoyOwner, press, _qEnvoyPosture);
    }

    public void CEnvoyOmissionShow(IReadOnlyList<CMarkupOmission> omissions)
    {
        QSCustoms.QSCustomsOmissionConsult(QEnvoyOwner, omissions);
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

        bool chosen = dialog.ShowDialog(QEnvoyOwner) == true;
        CPortraitMedium medium = choices[Math.Clamp(dialog.FilterIndex - 1, 0, choices.Count - 1)]
            .CPortraitChoiceMedium;

        return (chosen ? dialog.FileName : null, medium);
    }

    public string? CEnvoyMediaRead(string key, IReadOnlyList<string> suffixes)
    {
        ArgumentNullException.ThrowIfNull(suffixes);

        string title = QLocalizationCatalog.QLocalizationTextRead(key);
        Microsoft.Win32.OpenFileDialog dialog = new()
        {
            Title = title,
            Filter = string.Concat(title, "|", string.Join(";", suffixes.Select(static suffix => "*" + suffix))),
            CheckFileExists = true,
        };

        return dialog.ShowDialog(QEnvoyOwner) == true ? dialog.FileName : null;
    }

    public string? CEnvoyWorkspaceRead(string workspace)
    {
        Microsoft.Win32.OpenFolderDialog dialog = new()
        {
            Title = QLocalizationCatalog.QLocalizationTextRead("Settings.Workspace"),
            InitialDirectory = workspace,
        };

        return dialog.ShowDialog(QEnvoyOwner) == true ? dialog.FolderName : null;
    }

    public string? CEnvoyFolderRead(string folder)
    {
        Microsoft.Win32.OpenFolderDialog dialog = new()
        {
            Title = QLocalizationCatalog.QLocalizationTextRead("Settings.Depot"),
            InitialDirectory = folder,
        };

        return dialog.ShowDialog(QEnvoyOwner) == true ? dialog.FolderName : null;
    }

    public string? CEnvoyCoinageRead(string key)
    {
        return QSCoinage.QSCoinageShow(QEnvoyOwner, key);
    }
}
