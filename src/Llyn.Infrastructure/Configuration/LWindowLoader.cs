using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LWindowLoader
{
    private const string LWindowLoaderLeft = "left";
    private const string LWindowLoaderTop = "top";
    private const string LWindowLoaderWidth = "width";
    private const string LWindowLoaderHeight = "height";
    private const string LWindowLoaderMaximized = "maximized";

    public static LWindowState? LWindowLoaderRead(JsonElement window)
    {
        if (window.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        double? left = LWindowLoaderResolve(window, LWindowLoaderLeft);
        double? top = LWindowLoaderResolve(window, LWindowLoaderTop);
        double? width = LWindowLoaderResolve(window, LWindowLoaderWidth);
        double? height = LWindowLoaderResolve(window, LWindowLoaderHeight);

        if (left is null || top is null || width is null || height is null)
        {
            return null;
        }

        bool maximized =
            window.TryGetProperty(LWindowLoaderMaximized, out JsonElement flag) &&
            flag.ValueKind == JsonValueKind.True;

        return new LWindowState(left.Value, top.Value, width.Value, height.Value, maximized);
    }

    public static Dictionary<string, object> LWindowLoaderCreate(LWindowState window)
    {
        ArgumentNullException.ThrowIfNull(window);

        return new Dictionary<string, object>(StringComparer.Ordinal)
        {
            [LWindowLoaderLeft] = window.LWindowStateLeft,
            [LWindowLoaderTop] = window.LWindowStateTop,
            [LWindowLoaderWidth] = window.LWindowStateWidth,
            [LWindowLoaderHeight] = window.LWindowStateHeight,
            [LWindowLoaderMaximized] = window.LWindowStateMaximized
        };
    }

    private static double? LWindowLoaderResolve(JsonElement window, string name)
    {
        if (!window.TryGetProperty(name, out JsonElement value))
        {
            return null;
        }

        if (value.ValueKind == JsonValueKind.Number && value.TryGetDouble(out double number))
        {
            return double.IsFinite(number) ? number : null;
        }

        if (value.ValueKind == JsonValueKind.String &&
            double.TryParse(value.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out double parsed))
        {
            return double.IsFinite(parsed) ? parsed : null;
        }

        return null;
    }
}
