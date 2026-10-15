namespace Llyn.Core;

public sealed record LRigRehearsal(
    LDeckVault LRigRehearsalDecks,
    LDeckFolderVault LRigRehearsalFolders,
    LCueVault LRigRehearsalCues,
    LStencilVault LRigRehearsalStencils,
    LDepot LRigRehearsalDepot,
    LDeckManifestVault LRigRehearsalManifest);
