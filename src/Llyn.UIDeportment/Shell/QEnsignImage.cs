using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public static class QEnsignImage
{
    private static readonly Dictionary<string, ImageSource?> QEnsignStore = new(StringComparer.Ordinal);

    public static void QEnsignIntroduce(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        atelier.CAtelierWorkspace.CWorkspaceOpened += QEnsignOpenRefine;
    }

    private static void QEnsignOpenRefine()
    {
        lock (QEnsignStore)
        {
            QEnsignStore.Clear();
        }
    }

    private static DrawingImage? QEnsignDraw(string path, Action<string, Exception> delete)
    {
        ArgumentNullException.ThrowIfNull(delete);

        try
        {
            FileSvgReader reader = new(new WpfDrawingSettings { IncludeRuntime = false, TextAsGeometry = true });
            DrawingGroup drawing = reader.Read(path);
            if (drawing is null)
            {
                return null;
            }

            DrawingImage image = new(drawing);
            image.Freeze();
            return image;
        }
        catch (Exception exception)
        {
            delete(path, exception);
            return null;
        }
    }

    public static Action QEnsignDraw(IReadOnlyList<CEnsignRow> rows, Action<string, Exception> delete)
    {
        ArgumentNullException.ThrowIfNull(rows);

        List<KeyValuePair<string, ImageSource?>> resolved = new(rows.Count);
        foreach (CEnsignRow row in rows)
        {
            resolved.Add(new(row.CEnsignRowKey, QEnsignDraw(row.CEnsignRowPath, delete)));
        }

        return () => QEnsignStoreRefine(resolved);
    }

    private static void QEnsignStoreRefine(List<KeyValuePair<string, ImageSource?>> resolved)
    {
        lock (QEnsignStore)
        {
            foreach (KeyValuePair<string, ImageSource?> drawing in resolved)
            {
                QEnsignStore[drawing.Key] = drawing.Value;
            }
        }
    }

    public static void QEnsignFlagRefine(Image flag, UIElement globe, string language)
    {
        ArgumentNullException.ThrowIfNull(flag);
        ArgumentNullException.ThrowIfNull(globe);

        ImageSource? found = QEnsignRead(language);
        flag.Source = found;
        flag.Visibility = found is not null ? Visibility.Visible : Visibility.Collapsed;
        globe.Visibility = found is null ? Visibility.Visible : Visibility.Collapsed;
    }

    public static ImageSource? QEnsignRead(string language)
    {
        lock (QEnsignStore)
        {
            return QEnsignStore.TryGetValue(language, out ImageSource? flag) ? flag : null;
        }
    }
}
