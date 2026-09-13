using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<LTag> LEngineTagRead()
    {
        lock (_lEngineGate)
        {
            return new LTagArchive(_lEngineDatabase).LTagCatalogRead();
        }
    }

    public IReadOnlyList<LTag> LEngineTagFind(string query, LCatalogOrder order)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(query);
            query = query.Trim();

            List<LTag> found = [];
            foreach (LTag tag in new LTagArchive(_lEngineDatabase).LTagCatalogRead())
            {
                if (LCatalogTag.LCatalogTagMatch(tag, query))
                {
                    found.Add(tag);
                }
            }

            return LCatalogTag.LCatalogTagSort(found, order);
        }
    }

    public IReadOnlyList<LTag> LEngineTagRead(long ownerId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            LTagArchive tags = new(_lEngineDatabase);
            return LEngineOwnerCheck(owner)
                ? tags.LTagCollocationRead(ownerId)
                : tags.LTagMeaningRead(ownerId);
        }
    }

    public void LEngineTagSave(long ownerId, IReadOnlyList<LTag> written, LOwner owner)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(written);

            LTagArchive tags = new(_lEngineDatabase);
            bool collocation = LEngineOwnerCheck(owner);
            if (collocation)
            {
                tags.LTagCollocationSave(ownerId, written);
            }
            else
            {
                tags.LTagMeaningSave(ownerId, written);
            }

            LEngineUpdatedSet(ownerId, collocation);
        }
    }

    public void LEngineTagChange(long tagId, string renamed)
    {
        lock (_lEngineGate)
        {
            new LTagArchive(_lEngineDatabase).LTagChange(tagId, renamed);
        }

        LEngineBulletinRaise(LSubject.LSubjectTag, tagId);
    }

    public void LEngineTagDelete(long tagId)
    {
        lock (_lEngineGate)
        {
            new LTagArchive(_lEngineDatabase).LTagDelete(tagId);
        }

        LEngineBulletinRaise(LSubject.LSubjectTag, tagId);
    }
}
