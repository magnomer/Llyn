using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LAuthor TEngineAuthorCreate(this LEngine engine, LAuthor author)
    {
        LAuthor created = engine.LEngineStaffHeld.LEngineStaffAuthor.LAuthorClerkCreate(author);
        engine.LEngineBulletinRaise(LSubject.LSubjectAuthor, created.LAuthorId);
        return created;
    }

    internal static LAuthor? TEngineAuthorRead(this LEngine engine, long id) =>
        engine.LEngineAuthor.LEngineAuthorRead(id);

    internal static IReadOnlyList<LAuthor> TEngineAuthorFind(this LEngine engine, string query) =>
        engine.LEngineStaffHeld.LEngineStaffAuthor.LAuthorClerkFind(query);

    internal static IReadOnlyList<LCatalogAuthor> TEngineAuthorFind(
        this LEngine engine,
        string query,
        LCatalogOrder order) =>
        engine.LEngineAuthor.LEngineAuthorFind(query, order);

    internal static IReadOnlyList<LFellow> TEngineFellowFind(this LEngine engine, long authorId) =>
        engine.LEngineAuthor.LEngineFellowFind(authorId);

    internal static void TEngineAuthorAbsorb(this LEngine engine, long kept, long dropped)
    {
        engine.LEngineAuthor.LEngineAuthorAbsorb(kept, dropped);
    }

    internal static IReadOnlyList<LCatalogReference> TEngineOeuvreFind(
        this LEngine engine,
        long? author,
        string query,
        LCatalogFilter kind,
        LCatalogOrder order) =>
        engine.LEngineAuthor.LEngineOeuvreFind(author, query, kind, order);

    internal static IReadOnlyList<LAuthor> TEngineAuthorRead(
        this LEngine engine,
        long ownerId,
        LOwner owner) =>
        engine.LEngineAuthor.LEngineAuthorRead(ownerId, owner);

    internal static void TEngineAuthorUpdate(this LEngine engine, LAuthor author)
    {
        engine.LEngineStaffHeld.LEngineStaffAuthor.LAuthorClerkUpdate(author);
        engine.LEngineBulletinRaise(LSubject.LSubjectAuthor, author.LAuthorId);
    }

    internal static void TEngineAuthorDelete(this LEngine engine, long id, bool detach)
    {
        engine.LEngineAuthor.LEngineAuthorDelete(id, detach);
    }

    internal static LReference TEngineReferenceCommit(this LEngine engine, long id) =>
        engine.LEngineReference.LEngineReferenceCommit(id);

    internal static LReference TEngineReferenceCreate(this LEngine engine, LReference reference) =>
        engine.LEngineStaffHeld.LEngineStaffReference.LReferenceClerkCreate(reference);

    internal static LDraft TEngineReferenceStart(this LEngine engine, string origin, long? referenceId) =>
        engine.LEngineReference.LEngineReferenceStart(origin, referenceId);

    internal static IReadOnlyList<LReference> TEngineReferenceRead(this LEngine engine) =>
        engine.LEngineStaffHeld.LEngineStaffReference.LReferenceClerkRead();

    internal static LReference? TEngineReferenceRead(this LEngine engine, long id) =>
        engine.LEngineReference.LEngineReferenceRead(id);

    internal static void TEngineReferenceDelete(this LEngine engine, long id, bool detach)
    {
        engine.LEngineReference.LEngineReferenceDelete(id, detach);
    }

}
