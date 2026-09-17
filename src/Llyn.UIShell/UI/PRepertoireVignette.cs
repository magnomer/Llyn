using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PRepertoire
{
    private long? _pVignetteSituation;

    private string? PVignetteTextRead(LStateValue value)
    {
        return PStateConverter.PStateConverterCheck(value)
            ? _pRepertoireHost.PLocalizationTextRead("Display.Unknown")
            : value.LStateValueShow() is { Length: > 0 } shown ? shown : null;
    }

    private void PVignetteTitleShow(LStateValue value)
    {
        string? text = PVignetteTextRead(value);

        PVignetteTitle.Text = text ?? _pRepertoireHost.PLocalizationTextRead("Situation.Untitled");
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

        PMarkdown.PMarkdownShow(PVignetteDescription, text);
        PVignetteDescriptionSection.Visibility = text is null ? Visibility.Collapsed : Visibility.Visible;
    }

    private void PVignetteMediaShow(LSituation? situation)
    {
        PVignettePicture.DataContext = situation;
        PVignetteVideo.DataContext = situation;
    }

    private string PRepertoireTallyRead(long? id)
    {
        int count = id is long stored && _pAtlasCount.TryGetValue(stored, out int usage) ? usage : 0;

        return count switch
        {
            0 => _pRepertoireHost.PLocalizationTextRead("Situation.UsageNone"),
            1 => _pRepertoireHost.PLocalizationTextRead("Situation.UsageOne"),
            _ => $"{count.ToString(CultureInfo.CurrentCulture)} "
                + _pRepertoireHost.PLocalizationTextRead("Situation.UsageMany"),
        };
    }
}
