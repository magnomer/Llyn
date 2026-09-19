using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal IReadOnlyList<LTag> LEngineTagRead()
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

    public IReadOnlyList<LCatalogTag> LEngineTagFind(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);
        List<LCatalogTag> rows = [];
        bool kept = false;
        foreach (LTag tag in LEngineTagFind(vista.LVistaQuery, vista.LVistaOrder))
        {
            bool chosen = vista.LVistaMatch(tag.LTagId);
            kept |= chosen;
            rows.Add(new LCatalogTag(tag, chosen));
        }

        if (!kept)
        {
            vista.LVistaSelect(null);
        }

        return rows;
    }

    internal IReadOnlyList<LTag> LEngineTagRead(long ownerId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            LTagArchive tags = new(_lEngineDatabase);
            return LEngineOwnerCheck(owner)
                ? tags.LTagCollocationRead(ownerId)
                : tags.LTagMeaningRead(ownerId);
        }
    }

    internal void LEngineTagSave(long ownerId, IReadOnlyList<LTag> written, LOwner owner)
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

    public LTag LEngineTagCreate(string text)
    {
        LTag created;
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(text);

            LTagArchive tags = new(_lEngineDatabase);
            long id = tags.LTagResolve(text);
            created = tags.LTagRead(id) ?? throw new LRefusal(LRefusal.LRefusalItem);
        }

        LEngineBulletinRaise(LSubject.LSubjectTag, created.LTagId);
        return created;
    }

    internal void LEngineTagChange(long tagId, string renamed)
    {
        lock (_lEngineGate)
        {
            new LTagArchive(_lEngineDatabase).LTagChange(tagId, renamed);
        }

        LEngineBulletinRaise(LSubject.LSubjectTag, tagId);
    }

    internal void LEngineTagDelete(long tagId)
    {
        lock (_lEngineGate)
        {
            new LTagArchive(_lEngineDatabase).LTagDelete(tagId);
        }

        LEngineBulletinRaise(LSubject.LSubjectTag, tagId);
    }
}
