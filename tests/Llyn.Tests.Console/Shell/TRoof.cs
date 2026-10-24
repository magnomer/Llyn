using Llyn.Conduct;
using Llyn.ShellEngine;
using Llyn.UIDemeanor;
using Llyn.UITerminal;
using Xunit;

namespace Llyn.Tests;

public sealed class TRoof
{
    private static readonly ConsoleKeyInfo TRoofRight = new('\0', ConsoleKey.RightArrow, false, false, false);
    private static readonly ConsoleKeyInfo TRoofDown = new('\0', ConsoleKey.DownArrow, false, false, false);
    private static readonly ConsoleKeyInfo TRoofEnter = new('\r', ConsoleKey.Enter, false, false, false);

    [Fact]
    public void RoofApply_MiddleSubmenuItemVanishes_LightsNextItem()
    {
        List<string> ran = [];
        BRoof roof = TRoofPrepare(["Amber", "Birch", "Cedar"], ran, TRoofDown);

        roof.BRoofApply(TRoofMenuBuild(["Amber", "Cedar"], ran));
        roof.BRoofKeyObserve(TRoofEnter);

        Assert.Equal(["Cedar"], ran);
    }

    [Fact]
    public void RoofApply_LastSubmenuItemVanishes_LightsFirstLiveItem()
    {
        List<string> ran = [];
        BRoof roof = TRoofPrepare(["Amber", "Birch", "Cedar"], ran, TRoofDown, TRoofDown);

        roof.BRoofApply(TRoofMenuBuild(["Amber", "Birch"], ran));
        roof.BRoofKeyObserve(TRoofEnter);

        Assert.Equal(["Amber"], ran);
    }

    [Fact]
    public void RoofApply_SubmenuItemVanishes_KeepsSubmenuOpen()
    {
        List<string> ran = [];
        BRoof roof = TRoofPrepare(["Amber", "Birch", "Cedar"], ran, TRoofDown);

        roof.BRoofApply(TRoofMenuBuild(["Amber", "Cedar"], ran));

        IReadOnlyList<BCanvasText> texts = roof.BRoofDraw(
            new ABootstrap().BBlueprintGlyphs["single"], new BCanvasWidth(false));
        Assert.Equal(BKeymapFocus.BKeymapFocusRoof, roof.BRoofKeymapFocus);
        Assert.Contains(texts, static text => text.BCanvasTextValue.Contains("Cedar", StringComparison.Ordinal));
        Assert.DoesNotContain(texts, static text => text.BCanvasTextValue.Contains("Birch", StringComparison.Ordinal));
    }

    [Fact]
    public void HeadquarterBuild_Workspace_JoinsJoplinAboveRule()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CCourier courier = CCourier.CCourierCreate(atelier);

        BRoofMenu menu = TRoofHeadquarterBuild(courier, atelier.CAtelierWorkspace, TEnvoyFake.TEnvoyCreate(false, []));

        Assert.Equal(
            ["Connect to Joplin", "Sync to Joplin", "Workspace…", null, "About Llyn"],
            menu.BRoofMenuItems.Take(5).Select(static item => item?.BRoofItemLabel));
        Assert.Equal(ConsoleKey.W, menu.BRoofMenuItems[2]?.BRoofItemHotkey);
    }

    [Fact]
    public void HeadquarterBuild_WorkspaceChosen_AsksForWorkspace()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CEnvoy envoy = TEngineFake.TEngineCreate<CEnvoy>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["CEnvoyWorkspaceRead"] = args =>
            {
                asked.Add((string)args![0]!);
                return null;
            },
        });

        BRoofMenu menu = TRoofHeadquarterBuild(null, atelier.CAtelierWorkspace, envoy);
        menu.BRoofMenuItems[0]!.BRoofItemAction();

        Assert.Equal("Workspace…", menu.BRoofMenuItems[0]!.BRoofItemLabel);
        Assert.Null(menu.BRoofMenuItems[1]);
        Assert.Equal([atelier.CAtelierPathRead()], asked);
    }

    private static BRoof TRoofPrepare(string[] labels, List<string> ran, params ConsoleKeyInfo[] keys)
    {
        ABootstrap bootstrap = new();
        BBlueprintPart window = BContract.BContractBlueprintFind(bootstrap, "AWindow");
        BBlueprint.BBlueprintPlace(window, 0, 0, 80, 24);
        BRoof roof = new(window, TRoofMenuBuild(labels, ran));
        roof.BRoofOpen(0);
        roof.BRoofKeyObserve(TRoofRight);
        foreach (ConsoleKeyInfo key in keys)
        {
            roof.BRoofKeyObserve(key);
        }

        return roof;
    }

    private static IReadOnlyList<BRoofMenu> TRoofMenuBuild(string[] labels, List<string> ran)
    {
        BRoofItem[] submenu =
        [
            .. labels.Select(label => new BRoofItem(label, null, null, () => ran.Add(label), static () => true)),
        ];
        BRoofItem parent = new("Shelf", null, null, static () => { }, static () => true, submenu);
        return [new BRoofMenu("Menu", null, [parent])];
    }

    private static BRoofMenu TRoofHeadquarterBuild(CCourier? courier, CWorkspace? workspace, CEnvoy envoy)
    {
        BLocalizationCatalog catalog = new();
        catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
        return BHeadquarter.BHeadquarterBuild(
            catalog,
            courier,
            workspace,
            envoy,
            () => courier?.CCourierRead(),
            static () => { },
            static () => { },
            static () => { },
            static () => { });
    }
}
