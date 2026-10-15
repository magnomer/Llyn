namespace Llyn.Core;

public sealed record LRigKeeping(
    LDoctorVault LRigKeepingDoctor,
    LRevisionVault LRigKeepingRevisions,
    LWorkspaceVault LRigKeepingWorkspaces,
    LTombstoneVault LRigKeepingTombstones,
    LFavoriteVault LRigKeepingFavorites,
    LFoldVault LRigKeepingFolds,
    LNoteVault LRigKeepingNotes);
