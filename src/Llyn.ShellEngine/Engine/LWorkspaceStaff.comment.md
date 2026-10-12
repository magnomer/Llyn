# LWorkspaceStaff.cs
Hash: `2224ad986d49a941`

## `internal sealed record LWorkspaceStaff(LWorkspaceClerk LWorkspaceStaffWorkspace, LMarkupClerk LWorkspaceStaffMarkup, LMarkupClerkIntake LWorkspaceStaffIntake, LPortraitClerk LWorkspaceStaffPortrait, LPortraitClerkPress LWorkspaceStaffPress, LPressClerk LWorkspaceStaffGalley, LCourierClerk LWorkspaceStaffCourier)`

The group of clerks for workspace state, markup, portraits, print settings and the Joplin push.
It is built last, since it reads every other group.

**Parameters**

- `LWorkspaceStaffWorkspace` manages workspace state.
- `LWorkspaceStaffMarkup` translates an entry to and from markup.
- `LWorkspaceStaffIntake` imports markup.
- `LWorkspaceStaffPortrait` builds portraits.
- `LWorkspaceStaffPress` exports a composed page, opens its print session and lists the printers.
- `LWorkspaceStaffGalley` runs the print setting rules and keeps the setting for the next print session.
- `LWorkspaceStaffCourier` pushes entries into Joplin.

## `internal static LWorkspaceStaff LWorkspaceStaffBuild(LRig rig, LLanguageCache cache, LRevisionClerk revision, object gate, Func<LSettings> settings, LCatalogStaff catalog, LClaimStaff claim, LLanguageStaff language, LEntryStaff entry)`

Builds the workspace clerks over one rig after every other group.
The markup entry clerk receives the reflex clerk, so an exported entry lists its reflexes in the pack's order.
One markup example clerk serves both the card and the entry clerk, so both name entries alike.
The portrait clerk receives the entry group's fold clerk, so a portrait reads each card's stored fold.
The markup link clerk is built here and shared by the intake and its draft clerk.
The courier records its failures through the workspace clerk, so a failed push leaves a trace.
The courier takes no portrait clerk, since `LCourierFacade` hands it each Livery page at send time.
The courier searches through the query clerk, never through the entry lifecycle.
