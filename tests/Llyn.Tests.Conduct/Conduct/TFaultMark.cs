using System;
using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

public sealed partial class TFault
{
    private static IReadOnlyList<TFaultWrite> TFaultMarkRows =>
    [
        new(
            "CEsteem.CEsteemFavoriteSet",
            "LFavoritePort.LEngineFavoriteSave",
            "Favorite.MarkFailed",
            static stage =>
            {
                CEditor editor = TFaultDeskOpen(stage);
                return (
                    () => editor.CEditorEsteem.CEsteemFavoriteSet(true),
                    () => editor.CEditorEsteem.CEsteemFavorite);
            }),
        new(
            "CEsteem.CEsteemGraspSet",
            "LGraspPort.LEngineGraspSave",
            "Grasp.MarkFailed",
            static stage =>
            {
                CEditor editor = TFaultDeskOpen(stage);
                return (() => editor.CEditorEsteem.CEsteemGraspSet(4), () => editor.CEditorEsteem.CEsteemGrasp);
            }),
        new(
            "CDisplay.CDisplayFavoriteToggle",
            "LFavoritePort.LEngineFavoriteSave",
            "Favorite.MarkFailed",
            static stage =>
            {
                CDisplay display = TFaultWingOpen(stage).CWingDisplay;
                return (() => display.CDisplayFavoriteToggle(true), () => display.CDisplayFavoriteRead());
            }),
        new(
            "CDisplayGrasp.CDisplayGraspSet",
            "LGraspPort.LEngineGraspSave",
            "Grasp.MarkFailed",
            static stage =>
            {
                CDisplayGrasp grasp = TFaultWingOpen(stage).CWingDisplay.CDisplayGrasp;
                return (() => grasp.CDisplayGraspSet(4), () => grasp.CDisplayGraspRead());
            }),
        new(
            "CDisplaySound.CDisplayFanqieSet",
            "LFanqiePort.LEngineFanqieSet",
            "Display.FanqieRepresentativeFailed",
            static stage =>
            {
                CDisplay display = TFaultWingOpen(stage).CWingDisplay;
                return (() => display.CDisplaySound.CDisplayFanqieSet(1, 0, true), null);
            }),
        new(
            "CDisplay.CDisplayEntryResonate",
            "LLanguagePort.LEngineSoundStart",
            "Sound.StartFailed",
            static stage => (TFaultWingOpen(stage).CWingDisplay.CDisplayEntryResonate, null)),
        new(
            "CWing.CWingEntryOpen",
            "LLanguagePort.LEngineSoundStart",
            "Sound.StartFailed",
            static stage =>
            {
                LEngine engine = TFaultEngineStart(stage);
                CWing wing = CWing.CWingCreate(
                    TFaultAtelierCreate(stage, engine), TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard), true);
                long water = TFaultEntrySave(engine);
                return (() => wing.CWingEntryOpen(water), null);
            }),
        new(
            "CPanel.CPanelRowOpen",
            "LLanguagePort.LEngineSoundStart",
            "Sound.StartFailed",
            static stage =>
            {
                LEngine engine = TFaultEngineStart(stage);
                CLibrary library = CLibrary.CLibraryCreate(
                    TFaultAtelierCreate(stage, engine),
                    static () => true,
                    TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard),
                    static run => run());
                library.CLibraryEditor.CEditorDisplay.CDisplayPanelAttach(library.CLibraryPanel);
                library.TLibraryVistaRestore();
                long water = TFaultEntrySave(engine);
                return (() => library.CLibraryPanel.CPanelRowOpen(water), null);
            }),
        new(
            "CPanel.CPanelDraftResonate",
            "LLanguagePort.LEngineSoundStart",
            "Sound.StartFailed",
            static stage =>
            {
                LEngine engine = TFaultEngineStart(stage);
                CLibrary library = CLibrary.CLibraryCreate(
                    TFaultAtelierCreate(stage, engine),
                    static () => true,
                    TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard),
                    static run => run());
                library.CLibraryEditor.CEditorDisplay.CDisplayPanelAttach(library.CLibraryPanel);
                library.TLibraryVistaRestore();
                library.CLibraryPanel.CPanelRowOpen(TFaultEntrySave(engine));
                return (library.CLibraryPanel.CPanelDraftResonate, null);
            }),
        new(
            "CKindred.CKindredRebuild",
            "LReflexPort.LEngineReflexRebuild",
            "Display.ReflexRebuildFailed",
            static stage => (TFaultDeskOpen(stage).CEditorKindred.CKindredRebuild, null)),
        new(
            "CSounding.CSoundingFanqieSet",
            "LFanqiePort.LEngineFanqieSet",
            "Display.FanqieRepresentativeFailed",
            static stage =>
            {
                CSounding sounding = TFaultDeskOpen(stage).CEditorSounding;
                return (() => sounding.CSoundingFanqieSet(1, 0, true), null);
            }),
        new(
            "CSounding.CSoundingFanqieResolve",
            "LFanqiePort.LEngineFanqieRebuild",
            "Display.FanqieRebuildFailed",
            static stage => (TFaultDeskOpen(stage).CEditorSounding.CSoundingFanqieResolve, null)),
        new(
            "CSounding.CSoundingScriptResolve",
            "LScriptPort.LEngineScriptRebuild",
            "Display.ScriptRebuildFailed",
            static stage => (TFaultDeskOpen(stage).CEditorSounding.CSoundingScriptResolve, null)),
        new(
            "CFold.CFoldFanqieToggle",
            "LSettingsVault.LSettingsSave",
            "Settings.SaveFailed",
            static stage =>
            {
                LEngine engine = TFaultVaultStart(stage);
                CFold fold = CEditor.CEditorCreate(
                    TFaultAtelierCreate(stage, engine),
                    TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard)).CEditorFold;
                bool chosen = !engine.TEngineSettingsRead().LSettingsFanqieOpened;
                return (
                    () => fold.CFoldFanqieToggle(chosen),
                    () => engine.TEngineSettingsRead().LSettingsFanqieOpened);
            }),
        new(
            "CFold.CFoldScriptToggle",
            "LSettingsVault.LSettingsSave",
            "Settings.SaveFailed",
            static stage =>
            {
                LEngine engine = TFaultVaultStart(stage);
                CFold fold = CEditor.CEditorCreate(
                    TFaultAtelierCreate(stage, engine),
                    TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard)).CEditorFold;
                bool chosen = !engine.TEngineSettingsRead().LSettingsScriptOpened;
                return (
                    () => fold.CFoldScriptToggle(chosen),
                    () => engine.TEngineSettingsRead().LSettingsScriptOpened);
            }),
        new(
            "CTaxonomy.CTaxonomyEntryCreate",
            "LTagPort.LEngineTagCreate",
            "Tag.CreateFailed",
            static stage =>
            {
                CTaxonomy taxonomy = TTaxonomy.TTaxonomyPrepare(
                    TFaultAtelierStart(stage), TFaultConsentCreate(stage, "motion"));
                return (taxonomy.CTaxonomyEntryCreate, () => taxonomy.CTaxonomyRowsRead().Count);
            }),
        new(
            "CTenor.CTenorEntryCreate",
            "LRegisterPort.LEngineRegisterCreate",
            "Register.CreateFailed",
            static stage =>
            {
                CTenor tenor = TTenor.TTenorPrepare(TFaultAtelierStart(stage), TFaultConsentCreate(stage, "formal"));
                return (tenor.CTenorEntryCreate, () => tenor.CTenorRowsRead().Count);
            }),
        new(
            "CGuildUnion.CGuildUnionSelect",
            "LAuthorPort.LEngineAuthorAbsorb",
            "Guild.MergeFailed",
            static stage =>
            {
                LEngine engine = TFaultEngineStart(stage);
                CGuild guild = TGuild.TGuildPrepare(
                    TFaultAtelierCreate(stage, engine), TFaultConsentCreate(stage, null));
                LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
                LAuthor adam = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Adam"));
                guild.CGuildAuthorSelect(ada.LAuthorId);
                guild.CGuildScribeToggle(true);
                return (
                    () => guild.CGuildUnion.CGuildUnionSelect(adam.LAuthorId),
                    () => guild.CGuildDiptych.CDiptychParentEditing);
            }),
    ];

    private static long TFaultEntrySave(LEngine engine) =>
        engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", "ˈwɔːtə", string.Empty, [TInterface.TCardCreate("a liquid", 1)], [])).LEntryId;

    private static CEditor TFaultDeskOpen(TFaultStage stage)
    {
        LEngine engine = TFaultEngineStart(stage);
        CEditor editor = CEditor.CEditorCreate(
            TFaultAtelierCreate(stage, engine), TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard));
        editor.TEditorVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        editor.CEditorEntryOpen(TFaultEntrySave(engine));
        return editor;
    }

    private static CWing TFaultWingOpen(TFaultStage stage)
    {
        LEngine engine = TFaultEngineStart(stage);
        CWing wing = CWing.CWingCreate(
            TFaultAtelierCreate(stage, engine), TEnvoyFake.TEnvoyCreate(false, stage.TFaultStageHeard), true);
        wing.CWingEntryOpen(TFaultEntrySave(engine));
        return wing;
    }

    private static CEnvoy TFaultConsentCreate(TFaultStage stage, string? wording) =>
        TEngineFake.TEngineCreate<CEnvoy>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["CEnvoyCoinageRead"] = _ => wording,
            ["CEnvoyConfirm"] = static _ => true,
            ["CEnvoyLeaveConfirm"] = static _ => (bool?)true,
            ["CEnvoyUnionConfirm"] = static _ => true,
            ["CEnvoyFailureShow"] = args =>
            {
                stage.TFaultStageHeard.Add((string)args![0]!);
                return null;
            },
        });
}
