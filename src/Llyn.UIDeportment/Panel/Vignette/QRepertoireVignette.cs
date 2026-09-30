using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QRepertoire
{
    private void QVignetteRefine(CSituation situation)
    {
        QVignetteTitleRefine(situation.CSituationTitle);
        QVignetteKindRefine(situation.CSituationKind);
        QVignetteDescriptionRefine(situation.CSituationDescription);
        QVignetteMediaRefine(
            [.. situation.CSituationImage.Select(QScenarioImageCreate)],
            [.. situation.CSituationVideo.Select(QScenarioVideoCreate)]);
        QRepertoireTallyRefine();
    }

    private void QVignetteClearRefine()
    {
        QVignetteMediaRefine(null, null);
    }

    private void QVignetteTitleRefine(CStateWording title)
    {
        QVignetteTitle.Text = title.CStateWordingKey is string key
            ? QLocalizationCatalog.QLocalizationTextRead(key)
            : title.CStateWordingText;
        QVignetteTitle.SetResourceReference(
            TextBlock.ForegroundProperty,
            title.CStateWordingMuted ? "Theme.Muted" : "Theme.Ink");
    }

    private void QVignetteKindRefine(CStateWording kind)
    {
        QVignetteKind.Text = kind.CStateWordingKey is string key
            ? QLocalizationCatalog.QLocalizationTextRead(key)
            : kind.CStateWordingText;
        QVignetteChip.Visibility = QLook.QLookVisibleRead(!kind.CStateWordingMuted);
    }

    private void QVignetteDescriptionRefine(CStateWording description)
    {
        LMarkdownFace.LMarkdownRefine(
            QVignetteDescription,
            description.CStateWordingKey is null
                ? description.CStateWordingText
                : QLocalizationCatalog.QLocalizationTextRead(description.CStateWordingKey),
            _qRepertoireHost.PWindowAtelier);
        QVignetteDescriptionSection.Visibility = QLook.QLookVisibleRead(!description.CStateWordingMuted);
    }

    private void QVignetteMediaRefine(IReadOnlyList<PImage>? pictures, IReadOnlyList<PVideo>? videos)
    {
        QVignettePicture.ItemsSource = pictures;
        QVignetteVideo.ItemsSource = videos;
    }
}
