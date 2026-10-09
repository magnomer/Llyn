namespace Llyn.Core;

public sealed record LRigCitation(
    LAuthorVault LRigCitationAuthors,
    LImageVault LRigCitationImages,
    LReferenceVault LRigCitationReferences,
    LVideoVault LRigCitationVideos);
