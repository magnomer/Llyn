using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QRepertoire
{
    private static string? QVignetteTextRead(CStateValue value)
    {
        return value.CStateValueUncertain
            ? QLocalizationCatalog.QLocalizationTextRead("Display.Unknown")
            : value.CStateValueShown;
    }

    private void QVignetteShow(CSituationDraft situation)
    {
        QVignetteTitleShow(situation.CSituationDraftTitle);
        QVignetteKindShow(situation.CSituationDraftKind);
        QVignetteDescriptionShow(situation.CSituationDraftDescription);
        QVignetteMediaShow(
            QVignetteImageRead(situation.CSituationDraftImage), QVignetteVideoRead(situation.CSituationDraftVideo));
        QVignetteTally.Text = QRepertoireTallyRead(_lRepertoire.LRepertoireAtlas.LAtlasChosen);
    }

    private void QVignetteClear()
    {
        QVignetteMediaShow(null, null);
    }

    private void QVignetteTitleShow(CStateValue value)
    {
        string? text = QVignetteTextRead(value);

        QVignetteTitle.Text = text ?? QLocalizationCatalog.QLocalizationTextRead("Situation.Untitled");
        QVignetteTitle.SetResourceReference(
            TextBlock.ForegroundProperty,
            text is null ? "Theme.Muted" : "Theme.Ink");
    }

    private void QVignetteKindShow(CStateValue value)
    {
        string? text = QVignetteTextRead(value);

        QVignetteKind.Text = text ?? string.Empty;
        QVignetteChip.Visibility = QLook.QLookVisibleRead(text is not null);
    }

    private void QVignetteDescriptionShow(CStateValue value)
    {
        string? text = QVignetteTextRead(value);

        LMarkdownFace.LMarkdownShow(QVignetteDescription, text, _qRepertoireHost.PWindowAtelier);
        QVignetteDescriptionSection.Visibility = QLook.QLookVisibleRead(text is not null);
    }

    private void QVignetteMediaShow(IReadOnlyList<PImage>? pictures, IReadOnlyList<PVideo>? videos)
    {
        QVignettePicture.ItemsSource = pictures;
        QVignetteVideo.ItemsSource = videos;
    }

    private List<PImage> QVignetteImageRead(IReadOnlyList<CImageDraft> rows)
    {
        List<PImage> pictures = [];
        foreach (CImageDraft row in rows)
        {
            if (row.CImageDraftLocation.CStateValueUncertain || row.CImageDraftLocation.CStateValueLegible)
            {
                pictures.Add(QScenarioImageCreate(row));
            }
        }

        return pictures;
    }

    private List<PVideo> QVignetteVideoRead(IReadOnlyList<CVideoDraft> rows)
    {
        List<PVideo> videos = [];
        foreach (CVideoDraft row in rows)
        {
            if (row.CVideoDraftLocation.CStateValueUncertain || row.CVideoDraftLocation.CStateValueLegible)
            {
                videos.Add(QScenarioVideoCreate(row));
            }
        }

        return videos;
    }

    private string QRepertoireTallyRead(long? id)
    {
        int count = id is long stored && _qAtlasCount.TryGetValue(stored, out int usage) ? usage : 0;

        return count switch
        {
            0 => QLocalizationCatalog.QLocalizationTextRead("Situation.UsageNone"),
            1 => QLocalizationCatalog.QLocalizationTextRead("Situation.UsageOne"),
            _ => string.Concat(
                count.ToString(CultureInfo.CurrentCulture),
                " ",
                QLocalizationCatalog.QLocalizationTextRead("Situation.UsageMany")),
        };
    }
}
