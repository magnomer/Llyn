using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml;
using Llyn.Core;
using Llyn.ShellEngine;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace Llyn.UIShell;

internal static class PEnsign
{
    private static readonly Dictionary<string, ImageSource?> PEnsignStore = [];

    private static readonly SemaphoreSlim PEnsignGate = new(1, 1);

    private static int _pEnsignAge;

    internal static void PEnsignAttach(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);

        engine.LEngineObserverAttach(new PObserver(PEnsignBulletinHandle));
    }

    private static void PEnsignBulletinHandle(LBulletin bulletin)
    {
        if (bulletin.LBulletinSubject != LSubject.LSubjectWorkspace)
        {
            return;
        }

        lock (PEnsignStore)
        {
            _pEnsignAge++;
            PEnsignStore.Clear();
        }
    }

    internal static DrawingImage? PEnsignResolve(string path)
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
        catch (Exception exception) when (exception is IOException or XmlException or NotSupportedException)
        {
            return null;
        }
    }

    internal static async Task PEnsignLoad(LEngine engine)
    {
        await PEnsignGate.WaitAsync().ConfigureAwait(true);
        try
        {
            int age;
            string[] missing;
            lock (PEnsignStore)
            {
                age = _pEnsignAge;
                missing = engine.LEngineLanguageRead()
                    .Where(language => !PEnsignStore.ContainsKey(language))
                    .ToArray();
            }

            if (missing.Length == 0)
            {
                return;
            }

            string?[] paths = await Task.WhenAll(missing.Select(language => PEnsignRead(engine, language)));

            lock (PEnsignStore)
            {
                if (age != _pEnsignAge)
                {
                    return;
                }

                for (int index = 0; index < missing.Length; index++)
                {
                    string? path = paths[index];
                    PEnsignStore[missing[index]] = path is not null && File.Exists(path)
                        ? PEnsignResolve(path)
                        : null;
                }
            }
        }
        finally
        {
            PEnsignGate.Release();
        }
    }

    private static async Task<string?> PEnsignRead(LEngine engine, string language)
    {
        try
        {
            return await engine.LEngineFlagRead(language, CancellationToken.None);
        }
        catch (Exception)
        {
            return null;
        }
    }

    internal static ImageSource? PEnsignFind(string language)
    {
        lock (PEnsignStore)
        {
            return PEnsignStore.TryGetValue(language, out ImageSource? flag) ? flag : null;
        }
    }
}
