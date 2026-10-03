using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static class TInterfaceConduct
{
    internal static CAtelier TAtelierCreate(LEngine engine) => new(
        new LPosture(engine),
        new LDraftOutlet(engine),
        new LEntryOutlet(engine),
        new LSettingsOutlet(engine),
        new LPhonologyOutlet(engine),
        TEngineFake.TEngineCreate<LMediaPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineRecordingStop"] = _ => null,
            ["LEngineLocationRead"] = _ => null,
            ["LEngineScreenRead"] = _ => null,
        }),
        new LPortraitOutlet(engine));

    internal static CAtelier TAtelierCreate(LEngine engine, LMediaPort media) => new(
        new LPosture(engine),
        TEngineFake.TEngineCreate<LDraftPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineLeftoverSweep"] = _ => null,
        }),
        new LEntryOutlet(engine),
        new LSettingsOutlet(engine),
        new LPhonologyOutlet(engine),
        media,
        new LPortraitOutlet(engine));

    internal static CAtelier TAtelierMediaCreate(LEngine engine, LMediaPort media) => new(
        new LPosture(engine),
        new LDraftOutlet(engine),
        new LEntryOutlet(engine),
        new LSettingsOutlet(engine),
        new LPhonologyOutlet(engine),
        media,
        new LPortraitOutlet(engine));

    internal static CAtelier TAtelierCreate(LEngine engine, LSettingsPort settings) => new(
        new LPosture(engine),
        TEngineFake.TEngineCreate<LDraftPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineLeftoverSweep"] = _ => null,
            ["LEngineObserverAttach"] = _ => null,
            ["LEngineObserverDetach"] = _ => null,
        }),
        new LEntryOutlet(engine),
        settings,
        new LPhonologyOutlet(engine),
        TEngineFake.TEngineStubCreate<LMediaPort>(),
        new LPortraitOutlet(engine));

    internal static CAtelier TAtelierCreate(LEngine engine, Dictionary<string, Func<object?[]?, object?>> answers)
    {
        answers["LEngineLeftoverSweep"] = _ => null;
        answers["LEngineRecordingStop"] = _ => null;
        return new CAtelier(
            new LPosture(engine),
            TEngineFake.TEngineCreate<LDraftPort>(answers),
            TEngineFake.TEngineCreate<LEntryPort>(answers),
            new LSettingsOutlet(engine),
            TEngineFake.TEngineCreate<LPhonologyPort>(answers),
            TEngineFake.TEngineCreate<LMediaPort>(answers),
            new LPortraitOutlet(engine));
    }

    internal static LMediaPort TMediaCreate(LEngine engine) => new LMediaOutlet(engine);

    internal static LMediaPort TMediaCreate() =>
        TEngineFake.TEngineCreate<LMediaPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineLocationRead"] = _ => null,
            ["LEngineScreenRead"] = _ => null,
        });

    internal static LSettingsPort TSettingsCreate() =>
        TEngineFake.TEngineCreate<LSettingsPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineFailureRead"] = args => ((string)args![1]!, (string?)null, (string?)null),
            ["LEngineTextRead"] = args => (string)args![0]!,
        });

    internal static CVoyageState TVoyageRead(this CVoyage voyage) => voyage.LVoyageRead();

    internal static void TVoyageStationAdd(this CVoyage voyage, string tab, long id) =>
        voyage.LVoyageStationAdd(tab, id);

    internal static bool TVoyageUndo(this CVoyage voyage, string tab, long id, Func<string, long, bool> show) =>
        voyage.LVoyageUndo(tab, id, show);

    internal static bool TVoyageRedo(this CVoyage voyage, string tab, long id, Func<string, long, bool> show) =>
        voyage.LVoyageRedo(tab, id, show);

    internal static bool TAtelierSplitRead(CAtelier atelier) => atelier.LAtelierSplitRead();

    internal static CEstablishment TAtelierEstablishmentRead(this CAtelier atelier) =>
        atelier.LAtelierEstablishmentRead();

    internal static void TWorkspaceDraftAdd(this CWorkspace workspace, Func<bool> pending, Func<bool, bool> closure) =>
        workspace.LWorkspaceDraftAdd(pending, closure);

    internal static void TWorkspaceStateAdd(this CWorkspace workspace, Action<CWorkspaceState> heard) =>
        workspace.LWorkspaceStateOpened += heard;

    internal static CWorkspaceState? TAtelierStateOpen(this CAtelier atelier)
    {
        CWorkspaceState? opened = null;
        Action<CWorkspaceState> heard = state => opened = state;
        atelier.CAtelierWorkspace.LWorkspaceStateOpened += heard;
        atelier.CAtelierOpen();
        atelier.CAtelierWorkspace.LWorkspaceStateOpened -= heard;
        return opened;
    }
}
