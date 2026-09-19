using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LEntryVault TEntryVaultCreate(LDatabase database) =>
        new LEntryArchive(database);

    internal static LDraftVault TDraftVaultCreate(string root) =>
        new LDraftArchive(root);

    internal static LLanguageVault TLanguageVaultCreate() =>
        new LLanguageLoader();

    internal static LSettingsVault TSettingsVaultCreate(string root) =>
        new LSettingsLoader(root);

    internal static LWorkspaceVault TWorkspaceVaultCreate(LDatabase database) =>
        new LWorkspaceArchive(database);

    internal static LIdentity TIdentityCreate(LWorkspaceVault workspaces) =>
        new(workspaces);

    internal static long TIdentityCreate(this LIdentity identity) =>
        identity.LIdentityCreate();

    internal static IReadOnlyList<string> TLanguageScan(this LLanguageVault languageVault) =>
        languageVault.LLanguageScan();

    internal static LLanguage TLanguageRead(this LLanguageVault languageVault, string language) =>
        languageVault.LLanguageRead(language);

    internal static bool TLanguageNameValidate(this LLanguageVault languageVault, string? language) =>
        languageVault.LLanguageNameValidate(language);

    internal static bool TSettingsExist(this LSettingsVault settingsVault) =>
        settingsVault.LSettingsExist();

    internal static LSettings TSettingsRead(this LSettingsVault settingsVault) =>
        settingsVault.LSettingsRead();

    internal static void TSettingsSave(this LSettingsVault settingsVault, LSettings settings)
    {
        settingsVault.LSettingsSave(settings);
    }

    internal static void TDraftSave(this LDraftVault draftVault, LDraft draft)
    {
        draftVault.LDraftSave(draft);
    }

    internal static LDraft? TDraftRead(this LDraftVault draftVault, long id) =>
        draftVault.LDraftRead(id);

    internal static IReadOnlyList<LDraft> TDraftScan(this LDraftVault draftVault) =>
        draftVault.LDraftScan();

    internal static void TDraftDelete(this LDraftVault draftVault, long id)
    {
        draftVault.LDraftDelete(id);
    }
}
