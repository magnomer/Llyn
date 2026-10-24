using Llyn.Conduct;
using Llyn.UIDemeanor;
using Llyn.UITerminal;

namespace Llyn.Tests;

internal static class TEstablishmentFixture
{
    internal static IReadOnlyDictionary<string, IReadOnlyList<string>> TEstablishmentGlyphs =>
        new ABootstrap().BBlueprintGlyphs["single"];

    internal static BEstablishment TEstablishmentCreate()
    {
        ABootstrap bootstrap = new();
        BBlueprintPart window = BContract.BContractBlueprintFind(bootstrap, "AWindow");
        BBlueprint.BBlueprintPlace(window, 0, 0, 80, 24);
        return new BEstablishment(window, TEstablishmentCatalogRead());
    }

    internal static BLocalizationCatalog TEstablishmentCatalogRead()
    {
        BLocalizationCatalog catalog = new();
        catalog.BLocalizationCatalogApply(TInterface.TLocalizationLoad("en"));
        return catalog;
    }

    internal static BRoofMenu TEstablishmentMenuBuild(CCourier? courier)
    {
        return BHeadquarter.BHeadquarterBuild(
            TEstablishmentCatalogRead(),
            courier,
            null,
            TEnvoyFake.TEnvoyCreate(false, []),
            () => courier?.CCourierRead(),
            static () => { },
            static () => { },
            static () => { },
            static () => { });
    }

    internal static BMirror TEstablishmentMirrorCreate(CAtelier atelier, CCourier courier)
    {
        ABootstrap bootstrap = new();
        BBlueprintPart window = BContract.BContractBlueprintFind(bootstrap, "AWindow");
        BBlueprint.BBlueprintPlace(window, 0, 0, 80, 24);
        BLocalizationCatalog catalog = TEstablishmentCatalogRead();
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, []);
        BTheme spare = new(BTheme.BThemeSpareRead);
        return new BMirror(
            catalog,
            atelier,
            envoy,
            new BWardrobe(
                (BTheme.BThemeDefault, spare),
                _ => spare,
                static _ => { },
                bootstrap.BBlueprintGlyphs,
                string.Empty,
                new(),
                null),
            window,
            new BNavigation(bootstrap, atelier),
            heard => BHeadquarter.BHeadquarterBuild(
                catalog,
                courier,
                null,
                envoy,
                heard,
                static () => { },
                static () => { },
                static () => { },
                static () => { }),
            static () => [],
            static () => { },
            static () => { },
            static () => { });
    }

    internal static string TEstablishmentDraw(BEstablishment establishment, CCourierState courier)
    {
        return TEstablishmentRowBuild(
            establishment.BEstablishmentDraw(courier, TEstablishmentGlyphs, new BCanvasWidth(false)));
    }

    internal static string TEstablishmentRowBuild(IReadOnlyList<BCanvasText> texts)
    {
        char[] row = new string(' ', 80).ToCharArray();
        foreach (BCanvasText text in texts)
        {
            for (int at = 0; at < text.BCanvasTextSpan && at < text.BCanvasTextValue.Length; at++)
            {
                row[text.BCanvasTextColumn + at] = text.BCanvasTextValue[at];
            }

            for (int at = text.BCanvasTextValue.Length; at < text.BCanvasTextSpan; at++)
            {
                row[text.BCanvasTextColumn + at] = ' ';
            }
        }

        return new string(row);
    }
}
