namespace Llyn.Core;

/// <summary>
/// The user's persisted application settings. These live as <c>settings.json</c> inside the user's
/// workspace folder — never elsewhere — so a workspace carries its own preferences. The workspace
/// folder path itself is not stored here: it is the bootstrap locator that tells the program where
/// to find this file, and so is kept in a small fixed pointer outside the workspace.
/// </summary>
/// <param name="LSettingsLocalization">The chosen interface-language code, for example <c>"en"</c>.</param>
public sealed record LSettings(
    string LSettingsLocalization);
