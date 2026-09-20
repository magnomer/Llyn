using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LEntry LEngineEntryCreate(LEntry entry, IReadOnlyList<LForm> forms, IReadOnlyList<LSpeech> speeches)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkCreate(entry, forms, speeches);
        }
    }

    public LEntry? LEngineEntryRead(long id)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkRead(id);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(string query)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkFind(query);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(string query, LCatalogOrder order)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkFind(query, order);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(string query, LCatalogOrder order, LCatalogFilter filter)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkFind(query, order, filter);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LTag tag)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkFind(tag);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LTag tag, LCatalogFilter filter)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkFind(tag, filter);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LTag tag, string query, LCatalogFilter filter)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkFind(tag, query, filter);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LRegister register)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkFind(register);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LRegister register, LCatalogFilter filter)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkFind(register, filter);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LRegister register, string query, LCatalogFilter filter)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkFind(register, query, filter);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LSituation situation)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkFind(situation);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LSituation situation, string query, LCatalogFilter filter)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkFind(situation, query, filter);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LExample example)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkFind(example);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LExample example, string query, LCatalogFilter filter)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkFind(example, query, filter);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LReference reference)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkFind(reference);
        }
    }

    public IReadOnlyList<LEntry> LEngineEntryFind(LReference reference, string query, LCatalogFilter filter)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LEntryClerkFind(reference, query, filter);
        }
    }

    public LEntryDraft? LEngineEntryLoad(long id)
    {
        lock (_lEngineGate)
        {
            LEntryDraft? draft = _lEngineEntryClerk.LEntryClerkLoad(id);
            if (draft is null)
            {
                return null;
            }

            List<LPronunciationDraft> resolved = new(draft.LEntryDraftPronunciations.Count);
            foreach (LPronunciationDraft spoken in draft.LEntryDraftPronunciations)
            {
                resolved.Add(spoken.LPronunciationDraftAudio.Length == 0
                    ? spoken
                    : spoken with
                    {
                        LPronunciationDraftAudio = LEngineRecordingResolve(spoken.LPronunciationDraftAudio),
                    });
            }

            return draft with { LEntryDraftPronunciations = resolved };
        }
    }

    internal LRevision LEngineEntryDelete(long id)
    {
        LRevision recorded;
        lock (_lEngineGate)
        {
            recorded = _lEngineEntryClerk.LEntryClerkDelete(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectEntry, id);
        return recorded;
    }

    internal LTombstone? LEngineTombstoneRead(long entryId)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LTombstoneRead(entryId);
        }
    }

    internal LRevision? LEngineRevisionRead()
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LRevisionRead();
        }
    }

    internal IReadOnlyList<LRevisionChange> LEngineChangeRead(long revisionId)
    {
        lock (_lEngineGate)
        {
            return _lEngineEntryClerk.LRevisionChangeRead(revisionId);
        }
    }
}
