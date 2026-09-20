using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LSituation LEngineSituationCreate(LSituation situation)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(situation);

            using LVaultSession session = _lEngineVault.LVaultSessionStart();
            LSituationVault situations = _lEngineSituations;
            LSituation stored = situations.LSituationCreate(situation);
            LEngineMediaSync(stored.LSituationId, situation);
            stored = situations.LSituationRead(stored.LSituationId) ?? stored;
            session.LVaultSessionCommit();
            return stored;
        }
    }

    internal IReadOnlyList<LSituation> LEngineSituationRead()
    {
        lock (_lEngineGate)
        {
            return _lEngineSituations.LSituationRead();
        }
    }

    public IReadOnlyList<LCatalogSituation> LEngineSituationFind(string query, LCatalogOrder order)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(query);
            query = query.Trim();

            LSituationVault situations = _lEngineSituations;
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

    public IReadOnlyList<LCatalogSituation> LEngineSituationFind(LVista vista, string unknown = "", string untitled = "")
    {
        ArgumentNullException.ThrowIfNull(vista);
        IReadOnlyList<LCatalogSituation> found = LEngineSituationFind(vista.LVistaQuery, vista.LVistaOrder);
        List<LCatalogSituation> rows = new(found.Count);
        string[] names = LEngineTwinRead(found, row => LEngineNameRead(row.LCatalogSituationStored.LSituationTitle, unknown, untitled), row => row.LCatalogSituationStored.LSituationId);
        for (int index = 0; index < found.Count; index++)
        {
            LCatalogSituation row = found[index];
            rows.Add(row with
            {
                LCatalogSituationName = names[index],
                LCatalogSituationChosen = row.LCatalogSituationStored.LSituationId == vista.LVistaChosen,
            });
        }

        return rows;
    }

    internal LSituation? LEngineSituationRead(long id)
    {
        lock (_lEngineGate)
        {
            return _lEngineSituations.LSituationRead(id);
        }
    }

    internal IReadOnlyList<LSituation> LEngineSituationRead(long ownerId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            LSituationVault situations = _lEngineSituations;
            return LEngineOwnerCheck(owner)
                ? situations.LSituationCollocationRead(ownerId)
                : situations.LSituationMeaningRead(ownerId);
        }
    }

    internal void LEngineSituationUpdate(LSituation situation)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(situation);

            using LVaultSession session = _lEngineVault.LVaultSessionStart();
            _lEngineSituations.LSituationUpdate(situation);
            LEngineMediaSync(situation.LSituationId, situation);
            session.LVaultSessionCommit();
        }
    }

    private void LEngineMediaSync(long situationId, LSituation situation)
    {
        Dictionary<long, long> identity = [];

        LImageVault images = _lEngineImages;
        LEngineFieldSync(
            LEngineImageRead(situation.LSituationImage),
            images.LImageSituationRead(situationId),
            row => row.LImageId,
            written => LEngineImageResolve(images, written, identity),
            rowId => images.LImageSituationDetach(situationId, rowId),
            (rowId, position) => images.LImageSituationAttach(situationId, rowId, position));

        LVideoVault videos = _lEngineVideos;
        LEngineFieldSync(
            LEngineVideoRead(situation.LSituationVideo),
            videos.LVideoSituationRead(situationId),
            row => row.LVideoId,
            written => LEngineVideoResolve(videos, written, identity),
            rowId => videos.LVideoSituationDetach(situationId, rowId),
            (rowId, position) => videos.LVideoSituationAttach(situationId, rowId, position));
    }

    internal void LEngineSituationAttach(long ownerId, long situationId, int position, LOwner owner)
    {
        lock (_lEngineGate)
        {
            LSituationVault situations = _lEngineSituations;
            if (LEngineOwnerCheck(owner))
            {
                situations.LSituationCollocationAttach(ownerId, situationId, position);
                return;
            }

            situations.LSituationMeaningAttach(ownerId, situationId, position);
        }
    }

    internal void LEngineSituationDetach(long ownerId, long situationId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            LSituationVault situations = _lEngineSituations;
            if (LEngineOwnerCheck(owner))
            {
                situations.LSituationCollocationDetach(ownerId, situationId);
                return;
            }

            situations.LSituationMeaningDetach(ownerId, situationId);
        }
    }

    internal void LEngineSituationRemove(long ownerId, long situationId, LOwner owner)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(situationId);

            using LVaultSession session = _lEngineVault.LVaultSessionStart();

            LEngineSituationDetach(ownerId, situationId, owner);

            LSituationVault situations = _lEngineSituations;
            if (situations.LSituationReferenceRead(situationId) == 0)
            {
                situations.LSituationDelete(situationId);
            }

            LEngineUpdatedSet(ownerId, LEngineOwnerCheck(owner));
            session.LVaultSessionCommit();
        }
    }

    internal void LEngineSituationDelete(long id)
    {
        lock (_lEngineGate)
        {
            _lEngineSituations.LSituationDelete(id);
        }

        LEngineBulletinRaise(LSubject.LSubjectSituation, id);
    }

    internal void LEngineSituationDelete(long id, bool detach)
    {
        lock (_lEngineGate)
        {
            _lEngineSituations.LSituationDelete(id, detach);
        }

        LEngineBulletinRaise(LSubject.LSubjectSituation, id);
    }
}
