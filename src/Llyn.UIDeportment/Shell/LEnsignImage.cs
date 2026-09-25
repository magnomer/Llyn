using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Llyn.Application;
using Llyn.Core;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace Llyn.UIDeportment;

public static class LEnsignImage
{
    private static readonly Dictionary<string, ImageSource?> LEnsignStore = new(StringComparer.Ordinal);

    private static readonly SemaphoreSlim LEnsignGate = new(1, 1);

    public static void LEnsignAttach(LWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        window.LWindowObserverAttach(LEnsignBulletinHandle);
    }

    private static void LEnsignBulletinHandle(LBulletin bulletin)
    {
        if (!bulletin.LBulletinMatch(LSubject.LSubjectWorkspace))
        {
            return;
        }

        lock (LEnsignStore)
        {
            LEnsignStore.Clear();
        }
    }

    public static DrawingImage? LEnsignResolve(LWindow window, string path)
    {
        ArgumentNullException.ThrowIfNull(window);

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
        catch (Exception)
        {
            window.LWindowEnsignDelete(path);
            return null;
        }
    }

    public static async Task LEnsignLoad(LWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        await LEnsignGate.WaitAsync().ConfigureAwait(true);
        try
        {
            IReadOnlyList<LEnsignRow> kept = await window.LWindowEnsignLoad();
            LEnsignStoreAdd(window, kept);
        }
        finally
        {
            LEnsignGate.Release();
        }
    }

    public static Task LEnsignVarietyLoad(LWindow window, LEditor editor)
    {
        ArgumentNullException.ThrowIfNull(editor);

        return LEnsignVarietyLoad(window, editor.LEditorLanguage, editor.LEditorVarietyNames);
    }

    public static async Task LEnsignVarietyLoad(LWindow window, string language, IEnumerable<string> varieties)
    {
        ArgumentNullException.ThrowIfNull(window);

        await LEnsignGate.WaitAsync().ConfigureAwait(true);
        try
        {
            IReadOnlyList<LEnsignRow> kept = await window.LWindowEnsignLoad(language, varieties);
            LEnsignStoreAdd(window, kept);
        }
        finally
        {
            LEnsignGate.Release();
        }
    }

    private static void LEnsignStoreAdd(LWindow window, IReadOnlyList<LEnsignRow> rows)
    {
        lock (LEnsignStore)
        {
            foreach (LEnsignRow row in rows)
            {
                LEnsignStore[row.LEnsignRowKey] = LEnsignResolve(window, row.LEnsignRowPath);
            }
        }
    }

    public static void LEnsignFlagShow(Image flag, UIElement globe, string language)
    {
        ArgumentNullException.ThrowIfNull(flag);
        ArgumentNullException.ThrowIfNull(globe);

        ImageSource? found = LEnsignFind(language);
        flag.Source = found;
        flag.Visibility = found is not null ? Visibility.Visible : Visibility.Collapsed;
        globe.Visibility = found is null ? Visibility.Visible : Visibility.Collapsed;
    }

    public static ImageSource? LEnsignFind(string language)
    {
        lock (LEnsignStore)
        {
            return LEnsignStore.TryGetValue(language, out ImageSource? flag) ? flag : null;
        }
    }
}
