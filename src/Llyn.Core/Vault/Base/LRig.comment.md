# LRig.cs
Hash: `4e35c7434bc0c4c8`

## `public sealed record LRig(LVault LRigVault, LRigKeeping LRigKeeping, LSettingsVault LRigSettings, LAuditVault LRigAudit, LPostureVault LRigPosture, LRigAsset LRigAsset, LEntryVault LRigEntries, LEntryQueryVault LRigEntryQuery, LRigLexicon LRigLexicon, LRigDraft LRigDraft, LRigCitation LRigCitation, LRigSound LRigSound, LRigSentence LRigSentence, LRigContext LRigContext, LRigSource LRigSource, LOutpost LRigOutpost, LLanguageVault LRigLanguages, LPress LRigPress, LWarrant LRigWarrant, LUsher LRigUsher, LPhonograph LRigPhonograph, LTrail LRigTrail, LClock LRigClock, int LRigProcess, string LRigWorkspace)`

The bundle of ports the engine is built over.
It is one record and one constructor argument.
The composition root assembles it through `LRigFactory` in Infrastructure, and a test assembles it from fakes.
The engine copies each port into a field of its own and never learns which adapter stands behind it.
A workspace change hands the engine a whole new rig, so every port swaps at once.
There is no identity port.
The engine builds `LIdentity` over `LRigKeeping.LRigKeepingWorkspaces` itself, since it is a use case.
`LEnsign` is likewise built engine-side over `LRigUsher`, since it is a cache and not an adapter.
`LRigPress` is the printing surface, handed in by the root beside the usher since both are media.
`LRigWarrant` is the token protector, handed in by the root since protection depends on the operating system.
`LRigPhonograph` is the player, handed in by the root beside them for the same reason.
Every property is named `LRig{Base}`, the base being the port's own, so a port and its slot read alike.
`LRigOutpost` is the Joplin push port, set beside the fetch ports since it too speaks HTTP.
`LRigAsset.LRigAssetManifest` remembers what was last pushed to Joplin and is kept with the other workspace files.
`LRigAsset.LRigAssetLivery` answers the style note Joplin imports, set beside the portrait port since both wear the theme.
`LRigTrail` and `LRigClock` are the two ambient facts the engine may not read itself, the path rules and the time.
`LRigEntryQuery` is the read-only entry query port, set beside `LRigEntries` since both cover entries.
`LRigLexicon` groups the word-level ports, so a lexicon consumer reads them through one slot.
`LRigSentence` groups the sentence-level ports, so a sentence consumer reads them through one slot.
`LRigContext` groups the usage-context ports, so a register, situation or tag consumer reads them through one slot.
`LRigSound` groups the pronunciation-data ports, so a sound consumer reads them through one slot.
`LRigSource` groups the fetch ports and the recordings, so a source consumer reads them through one slot.
`LRigCitation` groups the source-citation ports, so a citation consumer reads them through one slot.
`LRigKeeping` groups the workspace-keeping ports, so a keeping consumer reads them through one slot.
`LRigDraft` groups the draft, claim and court ports, so a draft consumer reads them through one slot.
`LRigAsset` groups the manifest, localization, markup, portrait and livery ports, so an asset consumer reads them through one slot.
`LRigProcess` is the id of the process the rig was built in.
A claim on a draft is compared against it.
`LRigWorkspace` is the root every root-bound adapter above was built over.
It is kept so the engine can say where it stands.
