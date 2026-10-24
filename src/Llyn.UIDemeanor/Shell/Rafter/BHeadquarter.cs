using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal static class BHeadquarter
{
    internal static BRoofMenu BHeadquarterBuild(
        BLocalizationCatalog catalog,
        CCourier? courier,
        CWorkspace? workspace,
        CEnvoy envoy,
        Func<CCourierState?> heard,
        Action about,
        Action help,
        Action keys,
        Action quit)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(heard);

        Func<string, ConsoleKey?> letter = key => catalog.BLocalizationTextRead(key) is [char single]
            && char.IsAsciiLetter(single)
                ? ConsoleKey.A + (char.ToUpperInvariant(single) - 'A')
                : null;
        Func<string, string, ConsoleKey?, Action, BRoofItem> item = (key, hotkey, shortcut, action) =>
            new(catalog.BLocalizationTextRead(key), shortcut, letter(hotkey), action, static () => true);
        Func<bool> free = () => heard() is { CCourierStateAllowed: true };
        Func<string, Func<CEnvoy, Task>, BRoofItem> courierItem = (key, request) => new(
            catalog.BLocalizationTextRead(key),
            null,
            letter($"{key}.Hotkey"),
            async () => await request(envoy),
            free);
        List<BRoofItem?> joplin = [];
        if (courier is CCourier messenger && heard() is CCourierState state)
        {
            BRoofItem[] attach = [courierItem("Roof.Llyn.Attach", messenger.CCourierAttach)];
            joplin.AddRange(attach.Where(_ => !state.CCourierStateAttached));
            joplin.Add(courierItem("Roof.Llyn.Send", messenger.CCourierSend));
        }

        if (workspace is CWorkspace held)
        {
            joplin.Add(item(
                "Roof.Llyn.Workspace", "Roof.Llyn.Workspace.Hotkey", null, () => _ = held.CWorkspaceChange(envoy)));
        }

        if (joplin.Count > 0)
        {
            joplin.Add(null);
        }

        return new BRoofMenu(
            catalog.BLocalizationTextRead("Roof.Llyn"),
            letter("Roof.Llyn.Hotkey"),
            [
                .. joplin,
                item("Roof.Llyn.About", "Roof.Llyn.About.Hotkey", null, about),
                item("Roof.Llyn.Help", "Roof.Llyn.Help.Hotkey", ConsoleKey.F1, help),
                item("Roof.Llyn.Keys", "Roof.Llyn.Keys.Hotkey", null, keys),
                null,
                item("Roof.Llyn.Quit", "Roof.Llyn.Quit.Hotkey", ConsoleKey.F10, quit),
            ]);
    }

    internal static void BHeadquarterAboutShow(
        BLocalizationCatalog catalog, BBlueprintPart blueprint, Func<BSParley, int?> consult)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(blueprint);
        ArgumentNullException.ThrowIfNull(consult);

        IReadOnlyList<string> about = CAtelier.CAtelierAboutRead(catalog.BLocalizationTextRead);
        BSParley parley = new(
            blueprint,
            about[0],
            "dialog",
            [.. about.Select(static line => new BSParleyLine(line, false))],
            [new BSParleyButton(catalog.BLocalizationTextRead("Parley.Ok"))],
            0);
        _ = consult(parley);
    }
}
