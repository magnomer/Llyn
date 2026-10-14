using System;
using System.Collections.Generic;
using Llyn.Conduct;

namespace Llyn.Tests;

internal static class TEnvoyFake
{
    internal static CEnvoy TEnvoyCreate(bool? answer, List<string> asked) =>
        TEngineFake.TEngineCreate<CEnvoy>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["CEnvoyConfirm"] = args =>
            {
                asked.Add((string)args![0]!);
                return answer ?? false;
            },
            ["CEnvoyFailureShow"] = args =>
            {
                asked.Add((string)args![0]!);
                return null;
            },
            ["CEnvoyLeaveConfirm"] = _ =>
            {
                asked.Add("Leave");
                return answer;
            },
            ["CEnvoyUnionConfirm"] = args =>
            {
                asked.Add(string.Join(">", args!));
                return answer ?? false;
            },
        });

    internal static CEnvoy TEnvoyTallyCreate(bool answer, List<string> asked) =>
        TEngineFake.TEngineCreate<CEnvoy>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["CEnvoyConfirm"] = args =>
            {
                asked.Add(string.Join(">", args!));
                return answer;
            },
            ["CEnvoyFailureShow"] = args =>
            {
                asked.Add((string)args![0]!);
                return null;
            },
        });

    internal static CEnvoy TEnvoyFileCreate(string? path, CPortraitMedium medium, List<string> asked) =>
        TEngineFake.TEngineCreate<CEnvoy>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["CEnvoyFileRead"] = args =>
            {
                asked.Add("File:" + (string)args![0]!);
                return (path, medium);
            },
            ["CEnvoyFailureShow"] = args =>
            {
                asked.Add((string)args![0]!);
                return null;
            },
        });

    internal static CEnvoy TEnvoyPressCreate(Func<CSPress, bool> answer, List<string> asked) =>
        TEnvoyPressCreate("press.pdf", answer, asked);

    internal static CEnvoy TEnvoyPressCreate(string? path, Func<CSPress, bool> answer, List<string> asked) =>
        TEngineFake.TEngineCreate<CEnvoy>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["CEnvoyPressRead"] = args => answer((CSPress)args![0]!),
            ["CEnvoyFileRead"] = args =>
            {
                asked.Add("File:" + (string)args![0]!);
                return (path, CPortraitMedium.CPortraitMediumPdf);
            },
            ["CEnvoyFailureShow"] = args =>
            {
                asked.Add((string)args![0]!);
                return null;
            },
        });

    internal static CEnvoy TEnvoyCoinageCreate(string? wording, bool? leave, List<string> asked) =>
        TEngineFake.TEngineCreate<CEnvoy>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["CEnvoyCoinageRead"] = args =>
            {
                asked.Add("Coinage:" + (string)args![0]!);
                return wording;
            },
            ["CEnvoyLeaveConfirm"] = _ =>
            {
                asked.Add("Leave");
                return leave;
            },
            ["CEnvoyFailureShow"] = args =>
            {
                asked.Add((string)args![0]!);
                return null;
            },
        });

    internal static CEnvoy TEnvoyFolderCreate(string? folder, List<string> asked) =>
        TEngineFake.TEngineCreate<CEnvoy>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["CEnvoyFolderRead"] = args =>
            {
                asked.Add("Folder:" + (string)args![0]!);
                return folder;
            },
            ["CEnvoyFailureShow"] = args =>
            {
                asked.Add(args!.Length > 1 && args[1] is CLedgerNotice notice
                    ? (string)args[0]! + ">" + notice.CLedgerNoticeKey
                    : (string)args[0]!);
                return null;
            },
        });

    internal static CEnvoy TEnvoyMediaCreate(string? path, List<string> asked) =>
        TEngineFake.TEngineCreate<CEnvoy>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["CEnvoyMediaRead"] = args =>
            {
                asked.Add("Media:" + (string)args![0]!);
                return path;
            },
            ["CEnvoyFailureShow"] = args =>
            {
                asked.Add((string)args![0]!);
                return null;
            },
        });

    internal static CEnvoy TEnvoyMarkupCreate(string? path, List<string> asked) =>
        TEngineFake.TEngineCreate<CEnvoy>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["CEnvoyMarkupRead"] = _ => path,
            ["CEnvoyFailureShow"] = args =>
            {
                asked.Add((string)args![0]!);
                return null;
            },
        });
}
