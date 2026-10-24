using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.UIDemeanor.Capsule;

namespace Llyn.UIDemeanor;

internal sealed class BEnvoyPathfinder
{
    private readonly BLocalizationCatalog _bEnvoyPathfinderCatalog;
    private readonly BBlueprintPart _bEnvoyPathfinderParley;
    private readonly BBlueprintPart _bEnvoyPathfinderBlueprint;
    private readonly BBlueprintPart _bEnvoyPathfinderDrive;
    private readonly Func<string, bool> _bEnvoyPathfinderAssent;
    private BWindow? _bEnvoyPathfinderWindow;
    private CAtelier? _bEnvoyPathfinderAtelier;
    private Func<string, bool>? _bEnvoyPathfinderPresence;
    private Func<string, (IReadOnlyList<string>, IReadOnlyList<string>)>? _bEnvoyPathfinderFolder;
    private Func<string[]>? _bEnvoyPathfinderDrives;
    private LCapsuleConsole? _bEnvoyPathfinderCapsule;

    internal BEnvoyPathfinder(BLocalizationCatalog catalog, BBlueprint blueprint, Func<string, bool> confirm)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(blueprint);
        ArgumentNullException.ThrowIfNull(confirm);

        _bEnvoyPathfinderCatalog = catalog;
        _bEnvoyPathfinderParley = BContract.BContractBlueprintFind(blueprint, "ASParley");
        _bEnvoyPathfinderBlueprint = BContract.BContractBlueprintFind(blueprint, "ASPathfinder");
        _bEnvoyPathfinderDrive = BContract.BContractBlueprintFind(blueprint, "ASDrive");
        _bEnvoyPathfinderAssent = confirm;
    }

    internal void BEnvoyPathfinderAttach(
        BWindow window,
        CAtelier? atelier,
        Func<string, bool>? presence,
        Func<string, (IReadOnlyList<string>, IReadOnlyList<string>)>? folder,
        Func<string[]>? drives,
        LCapsuleConsole capsule)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(capsule);

        _bEnvoyPathfinderWindow = window;
        _bEnvoyPathfinderCapsule = capsule;
        _bEnvoyPathfinderAtelier = atelier;
        _bEnvoyPathfinderPresence = presence;
        _bEnvoyPathfinderFolder = folder;
        _bEnvoyPathfinderDrives = drives;
    }

    internal string? BEnvoyPathfinderConsult(string title, string folder, IReadOnlyList<string>? suffixes)
    {
        return BEnvoyPathfinderConsult(title, folder, suffixes, null, string.Empty);
    }

    internal (string?, CPortraitMedium) BEnvoyPathfinderConsult(
        string file, IReadOnlyList<CPortraitChoice> choices)
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(choices);

        if (choices.Count == 0)
        {
            return (null, default);
        }

        BSPathfinderMedium medium = new(choices);
        int cut = file.LastIndexOfAny(['\\', '/']) + 1;
        string? chosen = BEnvoyPathfinderConsult("Export.Title", file[..cut], null, medium, file[cut..]);
        return (chosen, medium.BSPathfinderMediumChoice.CPortraitChoiceMedium);
    }

    private string? BEnvoyPathfinderConsult(
        string title, string folder, IReadOnlyList<string>? suffixes, BSPathfinderMedium? medium, string name)
    {
        if (_bEnvoyPathfinderWindow is not BWindow window
            || _bEnvoyPathfinderPresence is not Func<string, bool> presence
            || _bEnvoyPathfinderFolder is not Func<string, (IReadOnlyList<string>, IReadOnlyList<string>)> listing
            || _bEnvoyPathfinderDrives is not Func<string[]> drives
            || _bEnvoyPathfinderCapsule is not LCapsuleConsole capsule)
        {
            return null;
        }

        string? root = _bEnvoyPathfinderAtelier?.CAtelierPathRead();
        string? stored = capsule.LCapsuleConsoleRead(root).LCapsuleSettingFolder;

        string admitKey = medium is not null ? "Pathfinder.Save"
            : suffixes is null ? "Pathfinder.Choose"
            : "Pathfinder.Open";
        BSParleyButton admit = new(_bEnvoyPathfinderCatalog.BLocalizationTextRead(admitKey));
        BSParleyButton cancel = new(_bEnvoyPathfinderCatalog.BLocalizationTextRead("Pathfinder.Cancel"));
        BSParleyButton[] buttons = [admit, cancel];
        TaskCompletionSource<string> chosen = new();
        Func<string?> drive = () =>
        {
            string[] roots;
            try
            {
                roots = drives();
            }
            catch (Exception)
            {
                return null;
            }

            TaskCompletionSource<string> picked = new();
            BSDrive list = new(_bEnvoyPathfinderDrive, roots, root => picked.TrySetResult(root));
            BSParley nested = new(
                _bEnvoyPathfinderParley,
                _bEnvoyPathfinderCatalog.BLocalizationTextRead("Drive.Title"),
                "dialog",
                [],
                [cancel],
                0,
                list);
            _ = window.BWindowParleyConsult(nested, until: picked.Task);
            return picked.Task.IsCompleted ? picked.Task.Result : null;
        };
        BSPathfinder body = new(
            _bEnvoyPathfinderBlueprint,
            _bEnvoyPathfinderCatalog,
            !string.IsNullOrEmpty(folder) ? folder
                : !string.IsNullOrWhiteSpace(stored) ? stored
                : root ?? string.Empty,
            suffixes,
            listing,
            presence,
            path =>
            {
                if (medium is null || !presence(path) || _bEnvoyPathfinderAssent("Pathfinder.Replace"))
                {
                    chosen.TrySetResult(path);
                }
            },
            drive,
            medium,
            name);
        string heading = _bEnvoyPathfinderCatalog.BLocalizationTextRead(title);
        while (!chosen.Task.IsCompleted)
        {
            BSParley parley = new(_bEnvoyPathfinderParley, heading, "dialog", [], buttons, buttons.Length - 1, body);
            if (window.BWindowParleyConsult(parley, until: chosen.Task) is not 0)
            {
                break;
            }

            body.BSPathfinderAccept();
        }

        if (!chosen.Task.IsCompleted)
        {
            return null;
        }

        string answer = chosen.Task.Result;
        int cut = suffixes is null && medium is null ? answer.Length : answer.LastIndexOfAny(['\\', '/']) + 1;
        BEnvoyPathfinderSave(capsule, root, answer[..cut]);
        return answer;
    }

    private static void BEnvoyPathfinderSave(LCapsuleConsole capsule, string? root, string folder)
    {
        LCapsuleSetting stored = capsule.LCapsuleConsoleRead(root);
        if (folder.Length == 0 || folder == stored.LCapsuleSettingFolder)
        {
            return;
        }

        try
        {
            capsule.LCapsuleConsoleSave(root, stored with { LCapsuleSettingFolder = folder });
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
