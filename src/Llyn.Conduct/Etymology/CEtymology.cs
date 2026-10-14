using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CEtymology
{
    private readonly CDesk _cEtymologyDesk;

    private readonly LAncestorPort _cEtymologyAncestorPort;

    private readonly LSettingsPort _cEtymologySettings;

    private readonly CEnvoy _cEtymologyEnvoy;

    private readonly CLedgerNoticed _cEtymologyNoticed;

    private LTenure? _cEtymologyShelfTenure;

    private long _cEtymologyShelfAncestor;

    internal CEtymology(
        CDesk desk, LAncestorPort ancestors, LSettingsPort settings, CEnvoy envoy, CLedgerNoticed noticed)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(ancestors);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(noticed);

        _cEtymologyDesk = desk;
        _cEtymologyAncestorPort = ancestors;
        _cEtymologySettings = settings;
        _cEtymologyEnvoy = envoy;
        _cEtymologyNoticed = noticed;
    }

    public event Action? CEtymologyChanged;

    public CEtymologyState CEtymologyRead()
    {
        return _cEtymologyDesk.CDeskDraft.CDeskDraftTenure is LTenure held
            ? new CEtymologyState(LEtymologyAncestorRead(held), LEtymologyRelationRead(), LEtymologyLanguageRead())
            : new CEtymologyState([], [], []);
    }

    private IReadOnlyList<CAncestor> LEtymologyAncestorRead(LTenure held)
    {
        return _cEtymologyNoticed.LLedgerRepaintRead<IReadOnlyList<CAncestor>>(
            _cEtymologyEnvoy,
            _cEtymologySettings,
            () => CAncestor.CAncestorRead(
                new LQuillAncestor(held, _cEtymologyAncestorPort).LQuillLineageRead(),
                ReferenceEquals(held, _cEtymologyShelfTenure) ? _cEtymologyShelfAncestor : null),
            [],
            "Input.AncestryFailed");
    }

    private IReadOnlyList<CAncestorChoice> LEtymologyRelationRead()
    {
        return _cEtymologyNoticed.LLedgerRepaintRead<IReadOnlyList<CAncestorChoice>>(
            _cEtymologyEnvoy,
            _cEtymologySettings,
            () =>
            [
                .. _cEtymologyAncestorPort.LEngineRelationRead().Select(CAncestorChoice.CAncestorChoiceRead),
                new CAncestorChoice(null, "Relation.Add", string.Empty, false, true),
            ],
            [],
            "Input.AncestryFailed");
    }

    private IReadOnlyList<CAncestorChoice> LEtymologyLanguageRead()
    {
        return _cEtymologyNoticed.LLedgerRepaintRead<IReadOnlyList<CAncestorChoice>>(
            _cEtymologyEnvoy,
            _cEtymologySettings,
            () =>
            [
                .. _cEtymologyAncestorPort.LEngineLanguageRead().Select(static language => new CAncestorChoice(
                    language.LAncestorLanguageTongue,
                    string.Empty,
                    language.LAncestorLanguageName,
                    language.LAncestorLanguageTongue is not null,
                    false)),
                new CAncestorChoice(null, "Tongue.Add", string.Empty, false, true),
            ],
            [],
            "Input.AncestryFailed");
    }

    public CProspect CEtymologyTextSet(long ancestor, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        CEtymologyQuill?.LQuillTextSet(ancestor, text);
        return CMention.LMentionProspectRead(_cEtymologyDesk, text, _cEtymologyEnvoy, _cEtymologySettings);
    }

    public void CEtymologyTargetSet(long ancestor, long entry)
    {
        if (CEtymologyQuill is LQuillAncestor quill && quill.LQuillTargetSet(ancestor, entry))
        {
            _cEtymologyShelfTenure = CEtymologyTenure;
            _cEtymologyShelfAncestor = ancestor;
        }

        CEtymologyChanged?.Invoke();
    }

    public void CEtymologyLanguageSet(long ancestor, CAncestorChoice choice)
    {
        ArgumentNullException.ThrowIfNull(choice);

        if (CEtymologyQuill is LQuillAncestor quill)
        {
            if (!choice.CAncestorChoiceAddition)
            {
                quill.LQuillLanguageSet(ancestor, choice.CAncestorChoiceName);
            }
            else if (_cEtymologyEnvoy.CEnvoyCoinageRead("Coinage.Tongue") is string name
                && LEtymologyTongueCreate(name) is LAncestorLanguage created)
            {
                quill.LQuillLanguageSet(ancestor, created.LAncestorLanguageName);
            }
        }

        CEtymologyChanged?.Invoke();
    }

    public void CEtymologyGlossSet(long ancestor, string gloss)
    {
        ArgumentNullException.ThrowIfNull(gloss);

        CEtymologyQuill?.LQuillGlossSet(ancestor, gloss);
    }

    public void CEtymologyRelationSet(long ancestor, CAncestorChoice choice)
    {
        ArgumentNullException.ThrowIfNull(choice);

        if (CEtymologyQuill is LQuillAncestor quill)
        {
            if (!choice.CAncestorChoiceAddition)
            {
                quill.LQuillRelationSet(ancestor, choice.CAncestorChoiceId ?? 0);
            }
            else if (_cEtymologyEnvoy.CEnvoyCoinageRead("Coinage.Relation") is string name
                && LEtymologyRelationCreate(name) is LAncestorRelation created)
            {
                quill.LQuillRelationSet(ancestor, created.LAncestorRelationId);
            }
        }

        CEtymologyChanged?.Invoke();
    }

    public void CEtymologyRelationDelete(CAncestorChoice choice)
    {
        ArgumentNullException.ThrowIfNull(choice);

        LEtymologyRelationDelete(choice);
        CEtymologyChanged?.Invoke();
    }

    public void CEtymologyLanguageDelete(CAncestorChoice choice)
    {
        ArgumentNullException.ThrowIfNull(choice);

        LEtymologyTongueDelete(choice);
        CEtymologyChanged?.Invoke();
    }

    private void LEtymologyRelationDelete(CAncestorChoice choice)
    {
        if (!choice.CAncestorChoiceCustom || choice.CAncestorChoiceId is not long relation)
        {
            return;
        }

        try
        {
            if (_cEtymologyAncestorPort.LEngineRelationRead()
                    .FirstOrDefault(stored => stored.LAncestorRelationId == relation) is not { } held
                || held.LAncestorRelationBuiltin
                || !LEtymologyConfirm("Relation", held.LAncestorRelationUsage)
                || _cEtymologyAncestorPort.LEngineRelationDelete(relation) is null)
            {
                return;
            }
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cEtymologyEnvoy, _cEtymologySettings, "Relation.DeleteFailed", exception);
            return;
        }

        LEtymologyRelationClear(relation);
    }

    private void LEtymologyTongueDelete(CAncestorChoice choice)
    {
        if (!choice.CAncestorChoiceCustom || choice.CAncestorChoiceId is not long tongue)
        {
            return;
        }

        try
        {
            if (_cEtymologyAncestorPort.LEngineLanguageRead()
                    .FirstOrDefault(stored => stored.LAncestorLanguageTongue == tongue) is not { } held
                || !LEtymologyConfirm("Tongue", held.LAncestorLanguageUsage)
                || _cEtymologyAncestorPort.LEngineTongueDelete(tongue) is null)
            {
                return;
            }
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cEtymologyEnvoy, _cEtymologySettings, "Tongue.DeleteFailed", exception);
            return;
        }

        LEtymologyTongueClear(tongue);
    }

    public void CEtymologyInsert(long? ancestor)
    {
        CEtymologyQuill?.LQuillAncestorInsert(ancestor ?? 0);
        CEtymologyChanged?.Invoke();
    }

    public void CEtymologyAdd(long ancestor)
    {
        CEtymologyQuill?.LQuillAncestorAdd(ancestor);
        CEtymologyChanged?.Invoke();
    }

    public void CEtymologyRemove(long ancestor)
    {
        CEtymologyQuill?.LQuillAncestorRemove(ancestor);
        CEtymologyChanged?.Invoke();
    }

    public void CEtymologyRestore(long ancestor)
    {
        if (CEtymologyQuill is LQuillAncestor quill)
        {
            quill.LQuillAncestorRestore(ancestor);
            if (ReferenceEquals(CEtymologyTenure, _cEtymologyShelfTenure) && ancestor == _cEtymologyShelfAncestor)
            {
                _cEtymologyShelfTenure = null;
            }
        }

        CEtymologyChanged?.Invoke();
    }

    private LAncestorRelation? LEtymologyRelationCreate(string name)
    {
        try
        {
            return _cEtymologyAncestorPort.LEngineRelationCreate(name);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cEtymologyEnvoy, _cEtymologySettings, "Relation.CreateFailed", exception);
            return null;
        }
    }

    private LAncestorLanguage? LEtymologyTongueCreate(string name)
    {
        try
        {
            return _cEtymologyAncestorPort.LEngineTongueCreate(name);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cEtymologyEnvoy, _cEtymologySettings, "Tongue.CreateFailed", exception);
            return null;
        }
    }

    private bool LEtymologyConfirm(string scope, int usage)
    {
        return usage > 0
            ? _cEtymologyEnvoy.CEnvoyConfirm(scope + ".DetachConfirm", scope + ".DetachCount", usage)
            : _cEtymologyEnvoy.CEnvoyConfirm(scope + ".DeleteConfirm");
    }

    private void LEtymologyRelationClear(long relation)
    {
        if (CEtymologyTenure is not LTenure held)
        {
            return;
        }

        LQuillAncestor quill = new(held, _cEtymologyAncestorPort);
        IReadOnlyList<LAncestorDraft> boxes = held.LTenureRead()?.LDraftContent.LEntryDraftAncestors ?? [];
        foreach (LAncestorDraft box in boxes)
        {
            if (box.LAncestorDraftRelation == relation)
            {
                quill.LQuillRelationSet(box.LAncestorDraftId, 0);
            }
        }
    }

    private void LEtymologyTongueClear(long tongue)
    {
        if (CEtymologyTenure is not LTenure held)
        {
            return;
        }

        LQuillAncestor quill = new(held, _cEtymologyAncestorPort);
        IReadOnlyList<LAncestorDraft> boxes = held.LTenureRead()?.LDraftContent.LEntryDraftAncestors ?? [];
        foreach (LAncestorDraft box in boxes)
        {
            if (box.LAncestorDraftTongue == tongue)
            {
                quill.LQuillLanguageSet(box.LAncestorDraftId, string.Empty);
            }
        }
    }

    private LTenure? CEtymologyTenure =>
        _cEtymologyDesk.CDeskDraft.CDeskDraftFilling ? null : _cEtymologyDesk.CDeskDraft.CDeskDraftTenure;

    private LQuillAncestor? CEtymologyQuill =>
        CEtymologyTenure is LTenure held ? new LQuillAncestor(held, _cEtymologyAncestorPort) : null;
}
