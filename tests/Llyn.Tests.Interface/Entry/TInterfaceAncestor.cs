using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LRequest TAncestorTextCreate(long draftId, long ancestorId, string text) =>
        new LRequestAncestorText(draftId, ancestorId, text);

    internal static LRequest TAncestorTargetCreate(long draftId, long ancestorId, long entryId) =>
        new LRequestAncestorTarget(draftId, ancestorId, entryId);

    internal static LRequest TAncestorLanguageCreate(long draftId, long ancestorId, string text, long tongueId) =>
        new LRequestAncestorLanguage(draftId, ancestorId, text, tongueId);

    internal static LRequest TAncestorGlossCreate(long draftId, long ancestorId, string text) =>
        new LRequestAncestorGloss(draftId, ancestorId, text);

    internal static LRequest TAncestorRelationCreate(long draftId, long ancestorId, long relationId) =>
        new LRequestAncestorRelation(draftId, ancestorId, relationId);

    internal static LRequest TAncestorInsertionCreate(long draftId, long ancestorId) =>
        new LRequestAncestorInsertion(draftId, ancestorId);

    internal static LRequest TAncestorAdditionCreate(long draftId, long ancestorId) =>
        new LRequestAncestorAddition(draftId, ancestorId);

    internal static LRequest TAncestorRemovalCreate(long draftId, long ancestorId) =>
        new LRequestAncestorRemoval(draftId, ancestorId);

    internal static LRequest TAncestorRestorationCreate(long draftId, long ancestorId) =>
        new LRequestAncestorRestoration(draftId, ancestorId);

    internal static LAncestorClerk TAncestorClerkCreate(LRig rig) =>
        new(rig, new LLanguageClerk(rig, new LLanguageCache(rig.LRigLanguages), new LTrailClerk(rig)));

    internal static IReadOnlyList<LAncestorRelation> TAncestorRelationRead(this LAncestorClerk clerk) =>
        clerk.LAncestorRelationRead();

    internal static LAncestorRelation TAncestorRelationAdd(this LAncestorClerk clerk, string name) =>
        clerk.LAncestorRelationCreate(name);

    internal static int? TAncestorRelationDelete(this LAncestorClerk clerk, long relationId) =>
        clerk.LAncestorRelationDelete(relationId);

    internal static IReadOnlyList<LAncestorLanguage> TAncestorLanguageRead(this LAncestorClerk clerk) =>
        clerk.LAncestorLanguageRead();

    internal static LAncestorLanguage TAncestorTongueAdd(this LAncestorClerk clerk, string name) =>
        clerk.LAncestorTongueCreate(name);

    internal static int? TAncestorTongueDelete(this LAncestorClerk clerk, long tongueId) =>
        clerk.LAncestorTongueDelete(tongueId);

    internal static IReadOnlyList<LAncestorRelation> TEngineRelationRead(this LEngine engine) =>
        engine.LEngineAncestor.LEngineRelationRead();

    internal static LAncestorRelation TEngineRelationCreate(this LEngine engine, string name) =>
        engine.LEngineAncestor.LEngineRelationCreate(name);

    internal static LAncestorLanguage TEngineTongueCreate(this LEngine engine, string name) =>
        engine.LEngineAncestor.LEngineTongueCreate(name);

    internal static LLineageClerk TLineageClerkCreate(LRig rig) => new(rig);

    internal static IReadOnlyList<LLineage> TLineageClerkRead(
        this LLineageClerk clerk, long entryId, IReadOnlyList<LAncestorDraft> map) =>
        clerk.LLineageClerkRead(entryId, map);

    internal static LAncestor TAncestorBoxCreate(long id, long? child, string text, long? target = null) =>
        new(id, child, 0, target, text);


    internal static void TAncestorMapSave(this LRig rig, long entryId, params LAncestor[] boxes) =>
        rig.LRigLexicon.LRigLexiconAncestors.LAncestorSave(entryId, boxes);

    internal static IReadOnlyList<LAncestor> TAncestorMapRead(this LRig rig, long entryId) =>
        rig.LRigLexicon.LRigLexiconAncestors.LAncestorRead(entryId);
}
