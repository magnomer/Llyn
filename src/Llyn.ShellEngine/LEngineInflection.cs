using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal IReadOnlyList<LInflection> LEngineInflectionRead(long entryId)
    {
        lock (_lEngineGate)
        {
            return new LInflectionArchive(_lEngineDatabase).LInflectionRead(entryId);
        }
    }

    internal void LEngineInflectionSet(long entryId, IReadOnlyList<LInflection> inflections)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(inflections);
            LEngineInflectionValidate(inflections);
            new LInflectionArchive(_lEngineDatabase).LInflectionSet(entryId, inflections);
            LEngineParadigmUpdate(entryId);
            LEngineInflectionReset(entryId);
            LEngineUpdatedSet(entryId);
        }
    }

    internal void LEngineInflectionAppend(long entryId, IReadOnlyList<LInflection> inflections)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(inflections);
            LEngineInflectionValidate(inflections);
            new LInflectionArchive(_lEngineDatabase).LInflectionAppend(entryId, inflections);
            LEngineParadigmUpdate(entryId);
        }
    }

    private void LEngineInflectionValidate(IReadOnlyList<LInflection> inflections)
    {
        LSpeechArchive speeches = new(_lEngineDatabase);
        LMorphologyArchive morphologies = new(_lEngineDatabase);
        foreach (LInflection inflection in inflections)
        {
            if (inflection.LInflectionSpeechId is long speechId
                && speeches.LSpeechValueRead(speechId) is null)
            {
                throw new LRefusal(LRefusal.LRefusalLink);
            }

            foreach (long morphologyId in inflection.LInflectionMorphology)
            {
                if (morphologies.LMorphologyRead(morphologyId) is null)
                {
                    throw new LRefusal(LRefusal.LRefusalLink);
                }
            }
        }
    }

    internal void LEngineInflectionMove(long entryId, int position, int target)
    {
        lock (_lEngineGate)
        {
            new LInflectionArchive(_lEngineDatabase).LInflectionMove(entryId, position, target);
            LEngineUpdatedSet(entryId);
        }
    }

    internal void LEngineInflectionDelete(long entryId, int position)
    {
        lock (_lEngineGate)
        {
            new LInflectionArchive(_lEngineDatabase).LInflectionDelete(entryId, position);
            LEngineInflectionReset(entryId);
            LEngineUpdatedSet(entryId);
        }
    }
}
