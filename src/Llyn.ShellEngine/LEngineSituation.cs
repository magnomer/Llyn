using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LSituation LEngineSituationCreate(LSituation situation)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(situation);

            using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();
            LSituationArchive situations = new(_lEngineDatabase);
            LSituation stored = situations.LSituationCreate(situation);
            LEngineMediaSync(stored.LSituationId, situation);
            stored = situations.LSituationRead(stored.LSituationId) ?? stored;
            session.LDatabaseSessionCommit();
            return stored;
        }
    }

    public IReadOnlyList<LSituation> LEngineSituationRead()
    {
        lock (_lEngineGate)
        {
            return new LSituationArchive(_lEngineDatabase).LSituationRead();
        }
    }

    public IReadOnlyList<LCatalogSituation> LEngineSituationFind(string query, LCatalogOrder order)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(query);
            query = query.Trim();

            LSituationArchive situations = new(_lEngineDatabase);
            IReadOnlyList<LSituation> read = situations.LSituationRead();
            IReadOnlyDictionary<long, int> usage = situations.LSituationReferenceRead();

            List<LCatalogSituation> rows = [];
            foreach (LSituation situation in read)
            {
                usage.TryGetValue(situation.LSituationId, out int counted);

                LCatalogSituation row = LCatalogSituation.LCatalogSituationCreate(situation, counted);
                if (row.LCatalogSituationMatch(query))
                {
                    rows.Add(row);
                }
            }

            return LCatalogSituation.LCatalogSituationSort(rows, order);
        }
    }

    private static LSituation? LEngineSituationResolve(LSituationArchive situations, LStateValue title)
    {
        string written = LCatalog.LCatalogTextNormalize(title.LStateValueShow());
        if (written.Length == 0)
        {
            return null;
        }

        LSituation? found = null;
        foreach (LSituation situation in situations.LSituationRead())
        {
            if (!string.Equals(
                    LCatalog.LCatalogTextNormalize(situation.LSituationTitle.LStateValueShow()),
                    written,
                    StringComparison.Ordinal))
            {
                continue;
            }

            if (found is not null)
            {
                return null;
            }

            found = situation;
        }

        return found;
    }

    public LSituation? LEngineSituationRead(long id)
    {
        lock (_lEngineGate)
        {
            return new LSituationArchive(_lEngineDatabase).LSituationRead(id);
        }
    }

    public IReadOnlyList<LSituation> LEngineSituationRead(long ownerId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            LSituationArchive situations = new(_lEngineDatabase);
            return LEngineOwnerCheck(owner)
                ? situations.LSituationCollocationRead(ownerId)
                : situations.LSituationMeaningRead(ownerId);
        }
    }

    public void LEngineSituationUpdate(LSituation situation)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(situation);

            using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();
            new LSituationArchive(_lEngineDatabase).LSituationUpdate(situation);
            LEngineMediaSync(situation.LSituationId, situation);
            session.LDatabaseSessionCommit();
        }
    }

    private void LEngineMediaSync(long situationId, LSituation situation)
    {
        Dictionary<long, long> identity = [];

        LImageArchive images = new(_lEngineDatabase);
        LEngineFieldSync(
            LEngineImageRead(situation.LSituationImage),
            images.LImageSituationRead(situationId),
            row => row.LImageId,
            written => LEngineImageResolve(images, written, identity),
            rowId => images.LImageSituationDetach(situationId, rowId),
            (rowId, position) => images.LImageSituationAttach(situationId, rowId, position));

        LVideoArchive videos = new(_lEngineDatabase);
        LEngineFieldSync(
            LEngineVideoRead(situation.LSituationVideo),
            videos.LVideoSituationRead(situationId),
            row => row.LVideoId,
            written => LEngineVideoResolve(videos, written, identity),
            rowId => videos.LVideoSituationDetach(situationId, rowId),
            (rowId, position) => videos.LVideoSituationAttach(situationId, rowId, position));
    }

    public void LEngineSituationAttach(long ownerId, long situationId, int position, LOwner owner)
    {
        lock (_lEngineGate)
        {
            LSituationArchive situations = new(_lEngineDatabase);
            if (LEngineOwnerCheck(owner))
            {
                situations.LSituationCollocationAttach(ownerId, situationId, position);
                return;
            }

            situations.LSituationMeaningAttach(ownerId, situationId, position);
        }
    }

    public void LEngineSituationDetach(long ownerId, long situationId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            LSituationArchive situations = new(_lEngineDatabase);
            if (LEngineOwnerCheck(owner))
            {
                situations.LSituationCollocationDetach(ownerId, situationId);
                return;
            }

            situations.LSituationMeaningDetach(ownerId, situationId);
        }
    }

    public void LEngineSituationRemove(long ownerId, long situationId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(situationId);

            using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();

            LEngineSituationDetach(ownerId, situationId, owner);

            LSituationArchive situations = new(_lEngineDatabase);
            if (situations.LSituationReferenceRead(situationId) == 0)
            {
                situations.LSituationDelete(situationId);
            }

            LEngineUpdatedSet(ownerId, LEngineOwnerCheck(owner));
            session.LDatabaseSessionCommit();
        }
    }

    public void LEngineSituationDelete(long id)
    {
        lock (_lEngineGate)
        {
            new LSituationArchive(_lEngineDatabase).LSituationDelete(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectSituation, id);
    }

    public void LEngineSituationDelete(long id, bool detach)
    {
        lock (_lEngineGate)
        {
            new LSituationArchive(_lEngineDatabase).LSituationDelete(id, detach);
        }

        LEngineBulletinRaise(LSubject.LSubjectSituation, id);
    }
}
