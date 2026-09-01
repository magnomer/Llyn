namespace Llyn.Core;

/// <summary>
/// One revision — a stamped point in the workspace's history under which a batch of changes was
/// recorded. A revision owns its <see cref="LRevisionChange"/> rows and the tombstones written under
/// it; it owns no lexical data, and deleting lexical data never deletes a revision.
/// <see cref="LRevisionId"/> is the identity: an opaque, program-generated stable id.
/// </summary>
/// <param name="LRevisionId">Opaque, program-generated stable id.</param>
/// <param name="LRevisionCreatedUtc">Round-trip UTC timestamp of when the revision was opened.</param>
public sealed record LRevision(
    string LRevisionId,
    string LRevisionCreatedUtc);
