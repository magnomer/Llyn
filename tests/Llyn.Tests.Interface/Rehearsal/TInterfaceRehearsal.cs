using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static IReadOnlyList<LStencilToken> TStencilTokenParse(string body) =>
        LStencilToken.LStencilTokenParse(body);

    internal static string TCueMarkupResolve(string? markdown) =>
        LCueMarkup.LCueMarkupResolve(markdown);

    internal static string TCueMarkupFormat(string text) =>
        LCueMarkup.LCueMarkupFormat(text);

    internal static string TDeckCodeResolve(string normalized, IReadOnlyCollection<string> taken) =>
        LDeckCode.LDeckCodeResolve(normalized, taken);

    internal static bool TDeckCodeCheck(string code, IReadOnlyCollection<string> taken) =>
        LDeckCode.LDeckCodeCheck(code, taken);

    internal static LCue TCueCreate(
        LGrain grain,
        long? entryId,
        long? meaningId,
        long? collocationId,
        long? exampleId,
        long? targetId) =>
        new(
            1, 1, 0, grain, entryId, meaningId, collocationId, exampleId, targetId,
            0, false, string.Empty, string.Empty, "2026-01-01 00:00:00");

    internal static LDeckArchive TDeckArchiveCreate(LDatabase database) =>
        new(database);

    internal static LCueArchive TCueArchiveCreate(LDatabase database) =>
        new(database);

    internal static LStencilArchive TStencilArchiveCreate(LDatabase database) =>
        new(database);

    internal static LDeck TDeckFixtureCreate(string name, long stencilId) =>
        new(
            0, null, 0, name, LGrain.LGrainMeaning, stencilId, LBearingLead.LBearingLeadOne,
            new LDeckSchedule(), name, false, null, string.Empty, string.Empty);

    internal static IReadOnlyList<LCueCell> TCueFixtureCreate(LCueColumn column, int count) =>
        Enumerable.Range(1, count).Select(index => new LCueCell(column, index, $"{column} {index}")).ToList();

    internal static LStencil TStencilFixtureCreate(string name, bool saved) =>
        new(0, name, saved, [new LStencilSlot(LCueColumn.LCueColumnText, 1, "{Headword}")]);

    internal static IReadOnlyList<LStencilSlot> TStencilDefaultRead(LGrain grain) =>
        LStencilDefault.LStencilDefaultRead(grain);

    internal static LStencil TStencilSlotCreate(LCueColumn column, string body) =>
        new(0, "Slot", false, [new LStencilSlot(column, 1, body)]);

    internal static LStencilSlot TStencilSlotCreate(LCueColumn column, int index, string body) =>
        new(column, index, body);

    internal static LCueCell TCueCellCreate(LCueColumn column, int index, string value) =>
        new(column, index, value);

    internal static LStencilClerk TStencilClerkCreate(LRig rig) =>
        new(
            rig,
            TEntryClerkCreate(rig),
            TTranslationClerkCreate(rig),
            new LReferenceClerk(rig),
            new LTrailClerk(rig));

    internal static LCueFace TStencilClerkResolve(
        this LStencilClerk clerk, LCue cue, LStencil stencil, LPortraitLabel label) =>
        clerk.LStencilClerkResolve(cue, stencil, label);

    internal static LCueFace TStencilClerkResolve(this LStencilClerk clerk, long cueId, LPortraitLabel label) =>
        clerk.LStencilClerkResolve(cueId, label);

    internal static LBearing TBearingFixtureCreate(int direction, int front, int back, int statistics) =>
        new(direction, front, back, statistics);

    internal static LDeck TDeckCreate(this LDeckVault deckVault, LDeck deck) =>
        deckVault.LDeckCreate(deck);

    internal static IReadOnlyList<LDeck> TDeckRead(this LDeckVault deckVault) =>
        deckVault.LDeckRead();

    internal static LDeck? TDeckRead(this LDeckVault deckVault, long deckId) =>
        deckVault.LDeckRead(deckId);

    internal static void TDeckUpdate(this LDeckVault deckVault, LDeck deck)
    {
        deckVault.LDeckUpdate(deck);
    }

    internal static void TDeckOrderSet(this LDeckVault deckVault, long? folderId, IReadOnlyList<long> order)
    {
        deckVault.LDeckOrderSet(folderId, order);
    }

    internal static void TDeckDelete(this LDeckVault deckVault, long deckId)
    {
        deckVault.LDeckDelete(deckId);
    }

    internal static IReadOnlyList<LBearing> TBearingRead(this LDeckVault deckVault, long deckId) =>
        deckVault.LBearingRead(deckId);

    internal static void TBearingSave(this LDeckVault deckVault, long deckId, IReadOnlyList<LBearing> bearings)
    {
        deckVault.LBearingSave(deckId, bearings);
    }

    internal static IReadOnlyList<LCue> TCueCreate(this LCueVault cueVault, long deckId, IReadOnlyList<LCue> cues) =>
        cueVault.LCueCreate(deckId, cues);

    internal static IReadOnlyList<LCue> TCueRead(this LCueVault cueVault, long deckId) =>
        cueVault.LCueRead(deckId);

    internal static IReadOnlyList<LCueCell> TCueCellRead(this LCueVault cueVault, long cueId) =>
        cueVault.LCueCellRead(cueId);

    internal static void TCueCellSave(this LCueVault cueVault, long cueId, IReadOnlyList<LCueCell> cells)
    {
        cueVault.LCueCellSave(cueId, cells);
    }

    internal static LStencil TStencilCreate(this LStencilVault stencilVault, LStencil stencil) =>
        stencilVault.LStencilCreate(stencil);

    internal static IReadOnlyList<LStencil> TStencilRead(this LStencilVault stencilVault) =>
        stencilVault.LStencilRead();

    internal static LStencil? TStencilRead(this LStencilVault stencilVault, long stencilId) =>
        stencilVault.LStencilRead(stencilId);

    internal static LDeckClerk TDeckClerkCreate(LRig rig) =>
        new(rig, new LTrailClerk(rig));

    internal static LCueClerk TCueClerkCreate(LRig rig) =>
        new(rig, TEntryClerkCreate(rig));

    internal static IReadOnlyList<LCue> TCueClerkScan(LEntryDraft draft, LGrain grain) =>
        LCueClerk.LCueClerkScan(draft, grain);

    internal static LDeck TDeckClerkCreate(this LDeckClerk clerk, string name, long? folderId, LGrain grain) =>
        clerk.LDeckClerkCreate(name, folderId, grain);

    internal static IReadOnlyList<LDeck> TDeckClerkRead(this LDeckClerk clerk) =>
        clerk.LDeckClerkRead();

    internal static (LDeck LDeckStored, IReadOnlyList<LBearing> LDeckBearings)? TDeckClerkRead(
        this LDeckClerk clerk, long deckId) =>
        clerk.LDeckClerkRead(deckId);

    internal static void TDeckNameSet(this LDeckClerk clerk, long deckId, string name)
    {
        clerk.LDeckNameSet(deckId, name);
    }

    internal static void TDeckClerkUpdate(this LDeckClerk clerk, LDeck deck, IReadOnlyList<LBearing> bearings)
    {
        clerk.LDeckClerkUpdate(deck, bearings);
    }

    internal static void TDeckOrderSet(this LDeckClerk clerk, long? folderId, IReadOnlyList<long> order)
    {
        clerk.LDeckOrderSet(folderId, order);
    }

    internal static IReadOnlyList<LDeckFolder> TDeckFolderRead(this LDeckClerk clerk) =>
        clerk.LDeckFolderRead();

    internal static LDeckFolder TDeckFolderCreate(this LDeckClerk clerk, string name) =>
        clerk.LDeckFolderCreate(name);

    internal static void TDeckFolderUpdate(this LDeckClerk clerk, long folderId, string name)
    {
        clerk.LDeckFolderUpdate(folderId, name);
    }

    internal static void TDeckFolderDelete(this LDeckClerk clerk, long folderId)
    {
        clerk.LDeckFolderDelete(folderId);
    }

    internal static IReadOnlyList<LCue> TCueClerkRead(this LCueClerk clerk, long deckId) =>
        clerk.LCueClerkRead(deckId);

    internal static IReadOnlyList<LCue> TCueClerkAdd(
        this LCueClerk clerk, long deckId, IReadOnlyList<long> entryIds) =>
        clerk.LCueClerkAdd(deckId, entryIds);

    internal static LCue TCueClerkCreate(this LCueClerk clerk, long deckId, IReadOnlyList<LCueCell> cells) =>
        clerk.LCueClerkCreate(deckId, cells);

    internal static IReadOnlyList<LCueCell> TCueCellRead(this LCueClerk clerk, long cueId) =>
        clerk.LCueCellRead(cueId);

    internal static void TCueCellSave(this LCueClerk clerk, long cueId, IReadOnlyList<LCueCell> cells)
    {
        clerk.LCueCellSave(cueId, cells);
    }

    internal static LStencil TStencilClerkRead(this LStencilClerk clerk, long deckId) =>
        clerk.LStencilClerkRead(deckId);

    internal static void TStencilSlotSet(this LStencilClerk clerk, long deckId, LStencilSlot slot)
    {
        clerk.LStencilSlotSet(deckId, slot);
    }

    internal static void TStencilClerkSave(this LStencilClerk clerk, long deckId, string name)
    {
        clerk.LStencilClerkSave(deckId, name);
    }

    internal static void TStencilClerkApply(this LStencilClerk clerk, long deckId, long stencilId)
    {
        clerk.LStencilClerkApply(deckId, stencilId);
    }

    internal static LDepotFolder TDepotFolderCreate() => new();

    internal static LDeckManifestFile TDeckManifestCreate(string root) => new(new LKeepFile(root));

    internal static LDeckManifest TDeckManifestRead(this LDeckManifestFile file) => file.LDeckManifestRead();

    internal static void TDeckManifestSave(this LDeckManifestFile file, LDeckManifest manifest)
    {
        file.LDeckManifestSave(manifest);
    }

    internal static LDeckManifest TDeckManifestCreate(
        IReadOnlyDictionary<long, (string, IReadOnlyList<string>, string)> decks) =>
        new(decks);

    internal static void TDeckManifestSave(this LDeckManifestVault vault, LDeckManifest manifest)
    {
        vault.LDeckManifestSave(manifest);
    }

    internal static LRig TDepotRigCreate(this LRig rig, LDepot depot, LDeckManifestVault manifest) =>
        rig with
        {
            LRigRehearsal = rig.LRigRehearsal with { LRigRehearsalDepot = depot, LRigRehearsalManifest = manifest },
        };

    internal static LDepotClerk TDepotClerkCreate(LRig rig, LDepotLatch latch) =>
        TDepotClerkCreate(rig, latch, "Phone");

    internal static LDepotClerk TDepotClerkCreate(LRig rig, LDepotLatch latch, string depot) =>
        new(
            rig,
            TStencilClerkCreate(rig),
            new LTrailClerk(rig),
            latch,
            new object(),
            () => TSettingsCreate("en") with { LSettingsDepot = depot },
            static _ => { });

    internal static Task<LDeckReceipt> TDepotClerkExport(this LDepotClerk clerk, long deckId) =>
        clerk.LDepotClerkExport(deckId, TPortraitLabelRead(), CancellationToken.None);

    internal static LDepotLatch TDepotLatchCreate() => new();

    internal static LDepotLatch TDepotLatchRead(this LEngine engine) =>
        engine.LEngineHearth.LEngineStaffHeld.LEngineStaffWorkspace.LWorkspaceStaffLatch;

    internal static bool TDepotLatchStart(this LDepotLatch latch) => latch.LDepotLatchStart();

    internal static void TDepotLatchFinish(this LDepotLatch latch)
    {
        latch.LDepotLatchFinish();
    }

    internal static bool TDepotLatchCheck(this LDepotLatch latch) => latch.LDepotLatchCheck();

    internal static LDeckSheet TDeckSheetCreate(
        IReadOnlyList<(string, string)> settings, IReadOnlyList<string> header, params IReadOnlyList<string>[] rows) =>
        new(settings, header, rows);

    internal static Task TDepotSave(this LDepot depot, string folder, string code, LDeckSheet sheet, bool basic) =>
        depot.LDepotSave(folder, code, sheet, basic, CancellationToken.None);

    internal static LDeckSheet? TDepotRead(this LDepot depot, string folder, string code) =>
        depot.LDepotRead(folder, code);
}
