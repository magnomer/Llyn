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

    public static DrawingImage? LEnsignResolve(string path, Action<string, Exception> delete)
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

    public static async Task LEnsignLoad(LWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        await LEnsignGate.WaitAsync().ConfigureAwait(true);
        try
        {
            await window.LWindowEnsignLoad(LEnsignStoreAdd);
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
            await window.LWindowEnsignLoad(language, varieties, LEnsignStoreAdd);
        }
        finally
        {
            LEnsignGate.Release();
        }
    }

    private static Action LEnsignStoreAdd(IReadOnlyList<LEnsignRow> rows, Action<string, Exception> delete)
    {
        List<KeyValuePair<string, ImageSource?>> resolved = new(rows.Count);
        foreach (LEnsignRow row in rows)
        {
            resolved.Add(new(row.LEnsignRowKey, LEnsignResolve(row.LEnsignRowPath, delete)));
        }

        return () => LEnsignStoreCommit(resolved);
    }

    private static void LEnsignStoreCommit(List<KeyValuePair<string, ImageSource?>> resolved)
    {
        lock (LEnsignStore)
        {
            foreach (KeyValuePair<string, ImageSource?> drawing in resolved)
            {
                LEnsignStore[drawing.Key] = drawing.Value;
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
