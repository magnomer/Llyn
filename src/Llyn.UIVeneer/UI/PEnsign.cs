using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace Llyn.UIVeneer;

internal static class PEnsign
{
    private static readonly Dictionary<string, ImageSource?> PEnsignStore = new(StringComparer.Ordinal);

    private static readonly SemaphoreSlim PEnsignGate = new(1, 1);

    internal static void PEnsignAttach(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);

        engine.LEngineObserverAttach(new PObserver(PEnsignBulletinHandle));
    }

    private static void PEnsignBulletinHandle(LBulletin bulletin)
    {
        if (!bulletin.LBulletinMatch(LSubject.LSubjectWorkspace))
        {
            return;
        }

        lock (PEnsignStore)
        {
            PEnsignStore.Clear();
        }
    }

    internal static DrawingImage? PEnsignResolve(LEngine engine, string path)
    {
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
            engine.LEngineEnsignDelete(path);
            return null;
        }
    }

    internal static async Task PEnsignLoad(LEngine engine)
    {
        await PEnsignGate.WaitAsync().ConfigureAwait(true);
        try
        {
            IReadOnlyList<LEnsignRow> kept = await engine.LEngineEnsignLoad();
            PEnsignStoreAdd(engine, kept);
        }
        finally
        {
            PEnsignGate.Release();
        }
    }

    internal static Task PEnsignVarietyLoad(LEngine engine, LEditor editor)
    {
        return PEnsignVarietyLoad(engine, editor.LEditorLanguage, editor.LEditorVarietyNames);
    }

    internal static async Task PEnsignVarietyLoad(LEngine engine, string language, IEnumerable<string> varieties)
    {
        ArgumentNullException.ThrowIfNull(engine);

        await PEnsignGate.WaitAsync().ConfigureAwait(true);
        try
        {
            IReadOnlyList<LEnsignRow> kept = await engine.LEngineEnsignLoad(language, varieties);
            PEnsignStoreAdd(engine, kept);
        }
        finally
        {
            PEnsignGate.Release();
        }
    }

    private static void PEnsignStoreAdd(LEngine engine, IReadOnlyList<LEnsignRow> rows)
    {
        lock (PEnsignStore)
        {
            foreach (LEnsignRow row in rows)
            {
                PEnsignStore[row.LEnsignRowKey] = PEnsignResolve(engine, row.LEnsignRowPath);
            }
        }
    }

    internal static string PEnsignVarietyFormat(string language, string variety)
    {
        return string.Concat(language, "/", variety);
    }

    internal static void PEnsignFlagShow(Image flag, UIElement globe, string language)
    {
        ImageSource? found = PEnsignFind(language);
        flag.Source = found;
        flag.Visibility = PLook.PLookVisibleRead(found is not null);
        globe.Visibility = PLook.PLookVisibleRead(found is null);
    }

    internal static ImageSource? PEnsignFind(string language)
    {
        lock (PEnsignStore)
        {
            return PEnsignStore.TryGetValue(language, out ImageSource? flag) ? flag : null;
        }
    }
}
