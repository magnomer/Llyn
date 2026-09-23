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
        engine.LEngineAuthor.LEngineAuthorAttach(referenceId, authorId, position);
    }

    internal static LAuthor TEngineAuthorCreate(this LEngine engine, LAuthor author) =>
        engine.LEngineAuthor.LEngineAuthorCreate(author);

    internal static void TEngineAuthorDelete(this LEngine engine, long id)
    {
        engine.LEngineAuthor.LEngineAuthorDelete(id);
    }

    internal static void TEngineAuthorDetach(this LEngine engine, long referenceId, long authorId)
    {
        engine.LEngineAuthor.LEngineAuthorDetach(referenceId, authorId);
    }

    internal static LAuthor? TEngineAuthorRead(this LEngine engine, long id) =>
        engine.LEngineAuthor.LEngineAuthorRead(id);

    internal static IReadOnlyList<LAuthor> TEngineAuthorRead(this LEngine engine) =>
        engine.LEngineAuthor.LEngineAuthorRead();

    internal static IReadOnlyList<LAuthor> TEngineAuthorFind(this LEngine engine, string query) =>
        engine.LEngineAuthor.LEngineAuthorFind(query);

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
        engine.LEngineAuthor.LEngineAuthorUpdate(author);
    }

    internal static void TEngineReferenceAttach(
        this LEngine engine,
        long ownerId,
        long referenceId,
        int position,
        LOwner owner)
    {
        engine.LEngineReference.LEngineReferenceAttach(ownerId, referenceId, position, owner);
    }

    internal static LReference TEngineReferenceCommit(this LEngine engine, long id) =>
        engine.LEngineReference.LEngineReferenceCommit(id);

    internal static LReference TEngineReferenceCreate(this LEngine engine, LReference reference) =>
        engine.LEngineReference.LEngineReferenceCreate(reference);

    internal static LDraft TEngineReferenceStart(this LEngine engine, string origin, long? referenceId) =>
        engine.LEngineReference.LEngineReferenceStart(origin, referenceId);

    internal static void TEngineReferenceDelete(this LEngine engine, long id)
    {
        engine.LEngineReference.LEngineReferenceDelete(id);
    }

    internal static void TEngineReferenceDetach(
        this LEngine engine,
        long ownerId,
        long referenceId,
        LOwner owner)
    {
        engine.LEngineReference.LEngineReferenceDetach(ownerId, referenceId, owner);
    }

    internal static IReadOnlyList<LReference> TEngineReferenceRead(this LEngine engine) =>
        engine.LEngineReference.LEngineReferenceRead();

    internal static LReference? TEngineReferenceRead(this LEngine engine, long id) =>
        engine.LEngineReference.LEngineReferenceRead(id);

    internal static IReadOnlyList<LReference> TEngineReferenceRead(
        this LEngine engine,
        long ownerId,
        LOwner owner) =>
        engine.LEngineReference.LEngineReferenceRead(ownerId, owner);
}
