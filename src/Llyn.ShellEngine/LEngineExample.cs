using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LExample LEngineExampleCreate(LExample example)
    {
        ArgumentNullException.ThrowIfNull(example);
        return new LExampleArchive(_lEngineDatabase).LExampleCreate(example);
    }

    public LExample? LEngineExampleRead(string id)
    {
        return new LExampleArchive(_lEngineDatabase).LExampleRead(id);
    }

    public IReadOnlyList<LExample> LEngineExampleRead()
    {
        return new LExampleArchive(_lEngineDatabase).LExampleRead();
    }

    public IReadOnlyList<LExample> LEngineExampleRead(string ownerId, LOwner owner)
    {
        LExampleLink examples = new(_lEngineDatabase);
        return owner switch
        {
            LOwner.LOwnerEntry => examples.LExampleEntryRead(ownerId),
            LOwner.LOwnerMeaning or LOwner.LOwnerCollocation =>
                [.. LEngineSentenceRead(ownerId, owner).Select(sentence => sentence.LSentenceExample)],
            _ => throw LEngineOwnerRaise(owner),
        };
    }

    public IReadOnlyList<LSentence> LEngineSentenceRead(string meaningId)
    {
        return new LSentenceArchive(_lEngineDatabase).LSentenceMeaningRead(meaningId);
    }

    public IReadOnlyList<LSentence> LEngineSentenceRead(string ownerId, LOwner owner)
    {
        LSentenceArchive sentences = new(_lEngineDatabase);
        return owner switch
        {
            LOwner.LOwnerMeaning => sentences.LSentenceMeaningRead(ownerId),
            LOwner.LOwnerCollocation => sentences.LSentenceCollocationRead(ownerId),
            _ => throw LEngineOwnerRaise(owner),
        };
    }

    public void LEngineExampleUpdate(LExample example)
    {
        new LExampleArchive(_lEngineDatabase).LExampleUpdate(example);
    }

    public void LEngineExampleUpdate(string exampleId, LStateValue reference)
    {
        new LExampleArchive(_lEngineDatabase).LExampleSourceUpdate(exampleId, reference);
    }

    public void LEngineExampleAttach(string ownerId, string exampleId, int position, LOwner owner)
    {
        LExampleLink examples = new(_lEngineDatabase);
        switch (owner)
        {
            case LOwner.LOwnerEntry:
                examples.LExampleEntryAttach(ownerId, exampleId, position);
                return;
            case LOwner.LOwnerMeaning:
                new LSentenceArchive(_lEngineDatabase).LSentenceMeaningAttach(ownerId, exampleId, position);
                return;
            case LOwner.LOwnerCollocation:
                new LSentenceArchive(_lEngineDatabase).LSentenceCollocationAttach(ownerId, exampleId, position);
                return;
            default:
                throw LEngineOwnerRaise(owner);
        }
    }

    public void LEngineExampleDetach(string ownerId, string exampleId, LOwner owner)
    {
        LExampleLink examples = new(_lEngineDatabase);
        switch (owner)
        {
            case LOwner.LOwnerEntry:
                examples.LExampleEntryDetach(ownerId, exampleId);
                return;
            case LOwner.LOwnerMeaning:
                new LSentenceArchive(_lEngineDatabase).LSentenceMeaningDetach(ownerId, exampleId);
                return;
            case LOwner.LOwnerCollocation:
                new LSentenceArchive(_lEngineDatabase).LSentenceCollocationDetach(ownerId, exampleId);
                return;
            default:
                throw LEngineOwnerRaise(owner);
        }
    }

    public void LEngineExampleRemove(string ownerId, string exampleId, LOwner owner)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(exampleId);

        using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();

        LEngineExampleDetach(ownerId, exampleId, owner);

        LExampleArchive examples = new(_lEngineDatabase);
        if (examples.LExampleReferenceRead(exampleId) == 0)
        {
            examples.LExampleDelete(exampleId);
        }

        session.LDatabaseSessionCommit();
    }

    public void LEngineExampleDelete(string id)
    {
        new LExampleArchive(_lEngineDatabase).LExampleDelete(id);
    }

    public void LEngineExampleDelete(string id, bool detach)
    {
        new LExampleArchive(_lEngineDatabase).LExampleDelete(id, detach);
    }
}
