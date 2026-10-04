namespace Convention.Tests;

internal static class TAuditEngineSetting
{
    public static readonly IReadOnlyDictionary<string, string[]> TAuditEngineOffer =
        new Dictionary<string, string[]>
        {
            ["Llyn.Conduct>Llyn.ShellEngine"] =
            [
                "LDraftPort",
                "LEntryPort",
                "LPhonologyPort",
                "LSettingsPort",
                "LMediaPort",
                "LPortraitPort",
                "LTenure",
                "LVista",
                "LForay",
                "LPosture",
                "LQuill",
                "LEasel",
                "LQuillChip",
                "LQuillCard",
                "LQuillSituation",
                "LQuillReflex",
            ],
        };
}
