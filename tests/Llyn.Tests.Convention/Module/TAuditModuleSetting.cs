namespace Convention.Tests;

internal static class TAuditModuleSetting
{
    public const int TAuditGeneration = 27;
    public const bool TAuditModuleEnforced = true;
    public const string TAuditModulePath = "temp/audit/Module-{0}.md";

    public static IReadOnlyList<TAuditModuleRow> TAuditModules =>
    [
        new(
            "Llyn.Infrastructure",
            "LLockerModule",
            "Llyn.Infrastructure.LLockerEntrance",
            [
                "src/Llyn.Infrastructure/Rehearsal/LDeckManifest.cs",
                "src/Llyn.Infrastructure/Rehearsal/LDeckManifestFile.cs",
                "src/Llyn.Infrastructure/Rehearsal/LDeckSheet.cs",
                "src/Llyn.Infrastructure/Rehearsal/LDepotFolder.cs",
                "src/Llyn.Infrastructure/Rehearsal/LLockerEntrance.cs",
            ],
            []),
        new(
            "Llyn.Infrastructure",
            "LDossierModule",
            "Llyn.Infrastructure.LDossierEntrance",
            [
                "src/Llyn.Infrastructure/Draft/LClaim.cs",
                "src/Llyn.Infrastructure/Draft/LClaimArchive.cs",
                "src/Llyn.Infrastructure/Draft/LCourtArchive.cs",
                "src/Llyn.Infrastructure/Draft/LDossierEntrance.cs",
                "src/Llyn.Infrastructure/Draft/LDraftArchive.cs",
            ],
            []),
        new(
            "Llyn.Infrastructure",
            "LEmbassyModule",
            "Llyn.Infrastructure.LEmbassyEntrance",
            [
                "src/Llyn.Infrastructure/Outpost/LEmbassyEntrance.cs",
                "src/Llyn.Infrastructure/Outpost/LManifest.cs",
                "src/Llyn.Infrastructure/Outpost/LManifestFile.cs",
                "src/Llyn.Infrastructure/Outpost/LOutpostBatch.cs",
                "src/Llyn.Infrastructure/Outpost/LOutpostHttp.cs",
                "src/Llyn.Infrastructure/Outpost/LOutpostPairing.cs",
                "src/Llyn.Infrastructure/Outpost/LOutpostSeal.cs",
                "src/Llyn.Infrastructure/Outpost/LOutpostShelf.cs",
                "src/Llyn.Infrastructure/Outpost/LOutpostWire.cs",
            ],
            []),
        new(
            "Llyn.Infrastructure",
            "LEngraverModule",
            "Llyn.Infrastructure.LEngraverEntrance",
            [
                "src/Llyn.Infrastructure/Outpost/Livery/Entry/LLiveryCard.cs",
                "src/Llyn.Infrastructure/Outpost/Livery/Entry/LLiveryCollocation.cs",
                "src/Llyn.Infrastructure/Outpost/Livery/Entry/LLiveryEtymology.cs",
                "src/Llyn.Infrastructure/Outpost/Livery/Entry/LLiveryHeader.cs",
                "src/Llyn.Infrastructure/Outpost/Livery/Entry/LLiveryInflection.cs",
                "src/Llyn.Infrastructure/Outpost/Livery/Entry/LLiveryLink.cs",
                "src/Llyn.Infrastructure/Outpost/Livery/Entry/LLiveryRime.cs",
                "src/Llyn.Infrastructure/Outpost/Livery/Entry/LLiveryScript.cs",
                "src/Llyn.Infrastructure/Outpost/Livery/Entry/LLiverySound.cs",
                "src/Llyn.Infrastructure/Outpost/Livery/LEngraverEntrance.cs",
                "src/Llyn.Infrastructure/Outpost/Livery/LLiveryStyle.cs",
                "src/Llyn.Infrastructure/Outpost/Livery/LLiveryXiesheng.cs",
                "src/Llyn.Infrastructure/Outpost/Livery/LLiveryYunjing.cs",
            ],
            []),
    ];

    public static readonly IReadOnlyDictionary<string, int> TAuditModuleCeiling =
        new Dictionary<string, int>(StringComparer.Ordinal);

    public static readonly string[] TAuditModuleInclude =
    [
        "src/*.cs",
    ];
}
