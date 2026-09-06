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

    public IReadOnlyList<LTag> LEngineTagRead(string ownerId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            LTagArchive tags = new(_lEngineDatabase);
            return LEngineOwnerCheck(owner)
                ? tags.LTagCollocationRead(ownerId)
                : tags.LTagMeaningRead(ownerId);
        }
    }

    public void LEngineTagSave(string ownerId, IReadOnlyList<LTag> written, LOwner owner)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(written);

            LTagArchive tags = new(_lEngineDatabase);
            if (LEngineOwnerCheck(owner))
            {
                tags.LTagCollocationSave(ownerId, written);
                return;
            }

            tags.LTagMeaningSave(ownerId, written);
        }
    }

    public void LEngineTagChange(string text, string renamed)
    {
        lock (_lEngineGate)
        {
            new LTagArchive(_lEngineDatabase).LTagChange(text, renamed);
        }

        LEngineBulletinRaise(LSubject.LSubjectTag, string.Empty);
    }

    public void LEngineTagDelete(string text)
    {
        lock (_lEngineGate)
        {
            new LTagArchive(_lEngineDatabase).LTagDelete(text);
        }

        LEngineBulletinRaise(LSubject.LSubjectTag, string.Empty);
    }
}
