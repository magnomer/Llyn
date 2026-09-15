using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static void TEngineAuthorAttach(
        this LEngine engine,
        long referenceId,
        long authorId,
        int position)
    {
        engine.LEngineAuthorAttach(referenceId, authorId, position);
    }

    internal static LAuthor TEngineAuthorCreate(this LEngine engine, LAuthor author) =>
        engine.LEngineAuthorCreate(author);

    internal static void TEngineAuthorDelete(this LEngine engine, long id)
    {
        engine.LEngineAuthorDelete(id);
    }

    internal static void TEngineAuthorDetach(this LEngine engine, long referenceId, long authorId)
    {
        engine.LEngineAuthorDetach(referenceId, authorId);
    }

    internal static LAuthor? TEngineAuthorRead(this LEngine engine, long id) =>
        engine.LEngineAuthorRead(id);

    internal static IReadOnlyList<LAuthor> TEngineAuthorRead(this LEngine engine) =>
        engine.LEngineAuthorRead();

    internal static IReadOnlyList<LAuthor> TEngineAuthorFind(this LEngine engine, string query) =>
        engine.LEngineAuthorFind(query);

    internal static IReadOnlyList<LCatalogAuthor> TEngineAuthorFind(
        this LEngine engine,
        string query,
        LCatalogOrder order) =>
        engine.LEngineAuthorFind(query, order);

    internal static void TEngineAuthorAbsorb(this LEngine engine, long kept, long dropped)
    {
        engine.LEngineAuthorAbsorb(kept, dropped);
    }

    internal static IReadOnlyList<LCatalogReference> TEngineOeuvreFind(
        this LEngine engine,
        long? author,
        string query,
        LCatalogFilter kind,
        LCatalogOrder order) =>
        engine.LEngineOeuvreFind(author, query, kind, order);

    internal static IReadOnlyList<LAuthor> TEngineAuthorRead(
        this LEngine engine,
        long ownerId,
        LOwner owner) =>
        engine.LEngineAuthorRead(ownerId, owner);

    internal static void TEngineAuthorUpdate(this LEngine engine, LAuthor author)
    {
        engine.LEngineAuthorUpdate(author);
    }

    internal static void TEngineReferenceAttach(
        this LEngine engine,
        long ownerId,
        long referenceId,
        int position,
        LOwner owner)
    {
        engine.LEngineReferenceAttach(ownerId, referenceId, position, owner);
    }

    internal static LReference TEngineReferenceCommit(this LEngine engine, long id) =>
        engine.LEngineReferenceCommit(id);

    internal static LReference TEngineReferenceCreate(this LEngine engine, LReference reference) =>
        engine.LEngineReferenceCreate(reference);

    internal static LDraft TEngineReferenceStart(this LEngine engine, string origin, long? referenceId) =>
        engine.LEngineReferenceStart(origin, referenceId);

    internal static void TEngineReferenceDelete(this LEngine engine, long id)
    {
        engine.LEngineReferenceDelete(id);
    }

    internal static void TEngineReferenceDetach(
        this LEngine engine,
        long ownerId,
        long referenceId,
        LOwner owner)
    {
        engine.LEngineReferenceDetach(ownerId, referenceId, owner);
    }

    internal static IReadOnlyList<LReference> TEngineReferenceRead(this LEngine engine) =>
        engine.LEngineReferenceRead();

    internal static LReference? TEngineReferenceRead(this LEngine engine, long id) =>
        engine.LEngineReferenceRead(id);

    internal static IReadOnlyList<LReference> TEngineReferenceRead(
        this LEngine engine,
        long ownerId,
        LOwner owner) =>
        engine.LEngineReferenceRead(ownerId, owner);
}
