using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LExample LEngineExampleCreate(LExample example)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(example);
            return _lEngineExamples.LExampleCreate(example);
        }
    }

    internal LExample? LEngineExampleRead(long id)
    {
        lock (_lEngineGate)
        {
            return _lEngineExamples.LExampleRead(id);
        }
    }

    internal IReadOnlyList<LExample> LEngineExampleRead()
    {
        lock (_lEngineGate)
        {
            return _lEngineExamples.LExampleRead();
        }
    }

    public IReadOnlyList<LCatalogExample> LEngineExampleFind(string query, LCatalogOrder order)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(query);
            query = query.Trim();

            IReadOnlyList<LExample> read = _lEngineExamples.LExampleRead();
            IReadOnlyDictionary<long, int> usage =
                _lEngineExamples.LExampleReferenceRead();
            IReadOnlyDictionary<long, string> cited = LEngineCitationRead();

            List<LCatalogExample> rows = [];
            foreach (LExample example in read)
            {
                usage.TryGetValue(example.LExampleId, out int counted);

                LCatalogExample row = LCatalogExample.LCatalogExampleCreate(
                    example,
                    LEngineCitationRead(cited, example.LExampleSource),
                    counted);
                if (row.LCatalogExampleMatch(query))
                {
                    rows.Add(row);
                }
            }

            return LCatalogExample.LCatalogExampleSort(rows, order);
        }
    }

    public IReadOnlyList<LCatalogExample> LEngineExampleFind(LVista vista, string unknown = "", string unwritten = "")
    {
        ArgumentNullException.ThrowIfNull(vista);
        IReadOnlyList<LCatalogExample> found = LEngineExampleFind(vista.LVistaQuery, vista.LVistaOrder);
        List<LCatalogExample> rows = new(found.Count);
        string[] names = LEngineTwinRead(
            found,
            row => LEngineNameRead(row.LCatalogExampleStored.LExampleText, unknown, unwritten),
            row => row.LCatalogExampleStored.LExampleId);
        for (int index = 0; index < found.Count; index++)
        {
            LCatalogExample row = found[index];
            rows.Add(row with
            {
                LCatalogExampleName = names[index],
                LCatalogExampleChosen = row.LCatalogExampleStored.LExampleId == vista.LVistaChosen,
            });
        }

        return rows;
    }

    public IReadOnlyDictionary<long, string> LEngineCitationRead()
    {
        lock (_lEngineGate)
        {
            IReadOnlyDictionary<long, IReadOnlyList<LAuthor>> credits =
                _lEngineAuthors.LAuthorReferenceRead();

            Dictionary<long, string> named = [];
            foreach (LReference reference in _lEngineReferences.LReferenceAllRead())
            {
                credits.TryGetValue(reference.LReferenceId, out IReadOnlyList<LAuthor>? credited);
                named[reference.LReferenceId] = reference.LReferenceBylineRead(credited ?? []);
            }

            return named;
        }
    }

    private static string LEngineCitationRead(IReadOnlyDictionary<long, string> named, LStateAnchor source)
    {
        long id = source.LStateAnchorShow();
        if (id == 0)
        {
            return string.Empty;
        }

        return named.TryGetValue(id, out string? name)
            ? name
            : id.ToString(CultureInfo.InvariantCulture);
    }

    internal IReadOnlyList<LExample> LEngineExampleRead(long ownerId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            return owner switch
            {
                LOwner.LOwnerMeaning or LOwner.LOwnerCollocation =>
                    [.. LEngineSentenceRead(ownerId, owner)
                        .Select(sentence => sentence.LSentenceExample)
                        .OfType<LExample>()],
                _ => throw LEngineOwnerRaise(owner),
            };
        }
    }

    internal IReadOnlyList<LSentence> LEngineSentenceRead(long meaningId)
    {
        lock (_lEngineGate)
        {
            return _lEngineSentences.LSentenceMeaningRead(meaningId);
        }
    }

    internal IReadOnlyList<LSentence> LEngineSentenceRead(long ownerId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            LSentenceVault sentences = _lEngineSentences;
            return owner switch
            {
                LOwner.LOwnerMeaning => sentences.LSentenceMeaningRead(ownerId),
                LOwner.LOwnerCollocation => sentences.LSentenceCollocationRead(ownerId),
                _ => throw LEngineOwnerRaise(owner),
            };
        }
    }

    internal void LEngineExampleUpdate(LExample example)
    {
        lock (_lEngineGate)
        {
            _lEngineExamples.LExampleUpdate(example);
        }
    }

    internal void LEngineExampleUpdate(long exampleId, LStateAnchor reference)
    {
        lock (_lEngineGate)
        {
            _lEngineExamples.LExampleSourceUpdate(exampleId, reference);
        }
    }

    internal void LEngineExampleAttach(long ownerId, long exampleId, int position, LOwner owner)
    {
        lock (_lEngineGate)
        {
            switch (owner)
            {
                case LOwner.LOwnerMeaning:
                    _lEngineSentences.LSentenceMeaningAttach(ownerId, exampleId, position);
                    return;
                case LOwner.LOwnerCollocation:
                    _lEngineSentences.LSentenceCollocationAttach(ownerId, exampleId, position);
                    return;
                default:
                    throw LEngineOwnerRaise(owner);
            }
        }
    }

    internal void LEngineExampleDetach(long ownerId, long exampleId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            switch (owner)
            {
                case LOwner.LOwnerMeaning:
                    _lEngineSentences.LSentenceMeaningDetach(ownerId, exampleId);
                    return;
                case LOwner.LOwnerCollocation:
                    _lEngineSentences.LSentenceCollocationDetach(ownerId, exampleId);
                    return;
                default:
                    throw LEngineOwnerRaise(owner);
            }
        }
    }

    internal void LEngineExampleRemove(long ownerId, long exampleId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(exampleId);

            using LVaultSession session = _lEngineVault.LVaultSessionStart();

            LEngineExampleDetach(ownerId, exampleId, owner);

            LExampleVault examples = _lEngineExamples;
            if (examples.LExampleReferenceRead(exampleId) == 0)
            {
                examples.LExampleDelete(exampleId);
            }

            LEngineUpdatedSet(ownerId, LEngineOwnerCheck(owner));
            session.LVaultSessionCommit();
        }
    }

    internal void LEngineExampleDelete(long id)
    {
        lock (_lEngineGate)
        {
            _lEngineExamples.LExampleDelete(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectExample, id);
    }

    internal void LEngineExampleDelete(long id, bool detach)
    {
        lock (_lEngineGate)
        {
            _lEngineExamples.LExampleDelete(id, detach);
        }

        LEngineBulletinRaise(LSubject.LSubjectExample, id);
    }
}
