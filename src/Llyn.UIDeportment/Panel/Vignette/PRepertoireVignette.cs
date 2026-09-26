using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIDeportment;

public partial class PRepertoire
{
    private string? PVignetteTextRead(LStateValue value)
    {
        return value.LStateValueUncertain
            ? PLocalizationCatalog.PLocalizationTextRead("Display.Unknown")
            : value.LStateValueShown;
    }

    private void PVignetteShow(LSituation situation)
    {
        PVignetteTitleShow(situation.LSituationTitle);
        PVignetteKindShow(situation.LSituationKind);
        PVignetteDescriptionShow(situation.LSituationDescription);
        PVignetteMediaShow(situation);
        PVignetteTally.Text = PRepertoireTallyRead(_lRepertoire.LRepertoireAtlas.LAtlasChosen);
    }

    private void PVignetteClear()
    {
        PVignetteMediaShow(null);
    }

    private void PVignetteTitleShow(LStateValue value)
    {
        string? text = PVignetteTextRead(value);

        PVignetteTitle.Text = text ?? PLocalizationCatalog.PLocalizationTextRead("Situation.Untitled");
        PVignetteTitle.SetResourceReference(
            TextBlock.ForegroundProperty,
            text is null ? "Theme.Muted" : "Theme.Ink");
    }

    private void PVignetteKindShow(LStateValue value)
    {
        string? text = PVignetteTextRead(value);

        PVignetteKind.Text = text ?? string.Empty;
        PVignetteChip.Visibility = text is null ? Visibility.Collapsed : Visibility.Visible;
    }

    private void PVignetteDescriptionShow(LStateValue value)
    {
        string? text = PVignetteTextRead(value);

        LMarkdownFace.LMarkdownShow(PVignetteDescription, text, _pRepertoireHost.PWindowDeportment);
        PVignetteDescriptionSection.Visibility = text is null ? Visibility.Collapsed : Visibility.Visible;
    }

    private void PVignetteMediaShow(LSituation? situation)
    {
        PVignettePicture.ItemsSource = situation?.LSituationImage;
        PVignetteVideo.ItemsSource = situation?.LSituationVideo;
    }

    private string PRepertoireTallyRead(long? id)
    {
        int count = id is long stored && _pAtlasCount.TryGetValue(stored, out int usage) ? usage : 0;

        return count switch
        {
            0 => PLocalizationCatalog.PLocalizationTextRead("Situation.UsageNone"),
            1 => PLocalizationCatalog.PLocalizationTextRead("Situation.UsageOne"),
            _ => string.Concat(
                count.ToString(CultureInfo.CurrentCulture),
                " ",
                PLocalizationCatalog.PLocalizationTextRead("Situation.UsageMany")),
        };
    }
}
