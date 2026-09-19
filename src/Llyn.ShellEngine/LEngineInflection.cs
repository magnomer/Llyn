using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal IReadOnlyList<LInflection> LEngineInflectionRead(long entryId)
    {
        lock (_lEngineGate)
        {
            return _lEngineInflections.LInflectionRead(entryId);
        }
    }

    internal void LEngineInflectionSet(long entryId, IReadOnlyList<LInflection> inflections)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(inflections);
            LEngineInflectionValidate(inflections);
            _lEngineInflections.LInflectionSet(entryId, inflections);
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
            _lEngineInflections.LInflectionAppend(entryId, inflections);
            LEngineParadigmUpdate(entryId);
        }
    }

    private void LEngineInflectionValidate(IReadOnlyList<LInflection> inflections)
    {
        LSpeechVault speeches = _lEngineSpeeches;
        LMorphologyVault morphologies = _lEngineMorphologies;
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
            _lEngineInflections.LInflectionMove(entryId, position, target);
            LEngineUpdatedSet(entryId);
        }
    }

    internal void LEngineInflectionDelete(long entryId, int position)
    {
        lock (_lEngineGate)
        {
            _lEngineInflections.LInflectionDelete(entryId, position);
            LEngineInflectionReset(entryId);
            LEngineUpdatedSet(entryId);
        }
    }
}
