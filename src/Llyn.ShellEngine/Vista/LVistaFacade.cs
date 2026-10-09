using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LVistaFacade : LVistaPort
{
    private readonly LEngineHearth _lVistaFacadeHearth;
    private readonly LAuthorFacade _lVistaFacadeAuthor;
    private readonly LCatalogFacade _lVistaFacadeCatalog;
    private readonly LEntryFacade _lVistaFacadeEntry;
    private readonly LExampleFacade _lVistaFacadeExample;
    private readonly LReferenceFacade _lVistaFacadeReference;
    private readonly LSettingsFacade _lVistaFacadeSettings;
    private readonly LSituationFacade _lVistaFacadeSituation;
    private readonly LWorkspaceFacade _lVistaFacadeWorkspace;
    private readonly LVistaRowFacade _lVistaFacadeRow;
    private readonly object _lVistaFacadeGate;
    private long _lVistaFacadeCount;

    private readonly Dictionary<string, LVista> _lVistaFacadeTabs = [];

    internal LVistaFacade(
        LEngineHearth hearth,
        LAuthorFacade author,
        LCatalogFacade catalog,
        LEntryFacade entry,
        LExampleFacade example,
        LReferenceFacade reference,
        LSettingsFacade settings,
        LSituationFacade situation,
        LWorkspaceFacade workspace,
        LVistaRowFacade row)
    {
        ArgumentNullException.ThrowIfNull(hearth);
        ArgumentNullException.ThrowIfNull(author);
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(example);
        ArgumentNullException.ThrowIfNull(reference);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(situation);
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(row);
        _lVistaFacadeHearth = hearth;
        _lVistaFacadeAuthor = author;
        _lVistaFacadeCatalog = catalog;
        _lVistaFacadeEntry = entry;
        _lVistaFacadeExample = example;
        _lVistaFacadeReference = reference;
        _lVistaFacadeSettings = settings;
        _lVistaFacadeSituation = situation;
        _lVistaFacadeWorkspace = workspace;
        _lVistaFacadeRow = row;
        _lVistaFacadeGate = _lVistaFacadeHearth.LEngineGate;
    }

    public LVista LEngineVistaStart(
        string tab, LSubject? subject, LCatalogOrder order, LCatalogFilter filter, bool editing, bool blank = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tab);
        ArgumentNullException.ThrowIfNull(filter);

        long id = Interlocked.Increment(ref _lVistaFacadeCount);
        LVista vista = new(_lVistaFacadeHearth, id, tab, subject, order, filter, blank, editing);
        lock (_lVistaFacadeGate)
        {
            if (_lVistaFacadeTabs.TryGetValue(tab, out LVista? former))
            {
                _lVistaFacadeHearth.LEngineObserverDetach(former.LVistaBulletinHandle);
            }

            _lVistaFacadeTabs[tab] = vista;
        }

        _lVistaFacadeHearth.LEngineObserverAttach(vista.LVistaBulletinHandle);
        return vista;
    }

    public LVista? LEngineVistaRead(long id)
    {
        lock (_lVistaFacadeGate)
        {
            foreach (LVista vista in _lVistaFacadeTabs.Values)
            {
                if (vista.LVistaId == id)
                {
                    return vista;
                }
            }

            return null;
        }
    }

    public IReadOnlyList<LVistaRow> LEngineEntryFind(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        if (vista.LVistaBlank && string.IsNullOrWhiteSpace(vista.LVistaQuery))
        {
            return [];
        }

        lock (_lVistaFacadeGate)
        {
            IReadOnlyList<LEntry> entries = _lVistaFacadeEntry.LEngineEntryFind(
                vista.LVistaQuery, vista.LVistaOrder, vista.LVistaFilter);
            return _lVistaFacadeRow.LEngineVistaBuild(entries, vista.LVistaChosen);
        }
    }

    public IReadOnlyList<LVistaRow> LEngineEntryFind(LVista? parent, LVista? child)
    {
        if (parent is null || child is null)
        {
            return [];
        }

        lock (_lVistaFacadeGate)
        {
            IReadOnlyList<LEntry> entries = parent.LVistaSubject is LSubject subject
                ? _lVistaFacadeEntry.LEngineEntryFind(subject, parent.LVistaChosen ?? 0)
                : [];
            entries = LEntryQueryClerk.LEntryMatch(
                entries, parent.LVistaFilter, child.LVistaQuery, child.LVistaOrder);
            return _lVistaFacadeRow.LEngineVistaBuild(entries, child.LVistaChosen);
        }
    }

    public IReadOnlyList<string> LEngineNameResolve(IReadOnlyList<string> labels)
    {
        return LEntryClerkTwin.LTwinNameResolve(labels);
    }

    public LDraft? LEngineVistaLoad(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        LDraft? loaded = null;
        lock (_lVistaFacadeGate)
        {
            if (vista.LVistaChosen is long id && id > 0)
            {
                LDraft draft = new(0, vista.LVistaTab, id, LClaimClerk.LDraftBlank,
                    _lVistaFacadeSettings.LEngineStampRead());
                loaded = vista.LVistaSubject switch
                {
                    LSubject.LSubjectEntry =>
                        _lVistaFacadeEntry.LEngineEntryLoad(id) is LEntryDraft entry
                        ? draft with { LDraftContent = entry } : null,
                    LSubject.LSubjectExample =>
                        _lVistaFacadeExample.LEngineExampleRead(id) is LExample example
                        ? draft with { LDraftExample = example } : null,
                    LSubject.LSubjectSituation =>
                        _lVistaFacadeSituation.LEngineSituationRead(id) is LSituation situation
                        ? draft with { LDraftSituation = situation } : null,
                    LSubject.LSubjectReference =>
                        _lVistaFacadeReference.LEngineReferenceRead(id) is LReference reference
                        ? draft with
                        {
                            LDraftReference = reference,
                            LDraftAuthor =
                                _lVistaFacadeAuthor.LEngineAuthorRead(id, LOwner.LOwnerReference),
                        }
                        : null,
                    LSubject.LSubjectAuthor =>
                        _lVistaFacadeAuthor.LEngineAuthorRead(id) is LAuthor author
                        ? draft with { LDraftAuthorHeld = author } : null,
                    LSubject.LSubjectTag => _lVistaFacadeCatalog.LEngineTagRead()
                        .FirstOrDefault(row => row.LTagId == id) is not null
                        ? draft : null,
                    LSubject.LSubjectRegister => _lVistaFacadeCatalog.LEngineRegisterFind(
                        string.Empty, LCatalogOrder.LCatalogOrderName)
                        .FirstOrDefault(row => row.LCatalogRegisterStored.LRegisterId == id) is not null
                        ? draft : null,
                    _ => null,
                };
            }
        }

        if (loaded is null && vista.LVistaStored is not null)
        {
            vista.LVistaSelect(null);
        }

        return loaded;
    }

    public LDraft? LEngineVistaLoad(LVista vista, long? id)
    {
        ArgumentNullException.ThrowIfNull(vista);

        long? prior = vista.LVistaChosen;
        vista.LVistaSelect(id);
        try
        {
            return LEngineVistaLoad(vista);
        }
        catch (Exception)
        {
            vista.LVistaSelect(prior);
            throw;
        }
    }

    public string LEngineFileRead(LVista? vista)
    {
        string headword = vista is null
            ? string.Empty
            : LEngineVistaLoad(vista)?.LDraftContent.LEntryDraftHeadword ?? string.Empty;
        string trimmed = headword.Trim();
        return trimmed.Length == 0 || vista is null
            ? "entry"
            : _lVistaFacadeSettings.LEngineTrailNormalize(trimmed);
    }

    public void LEngineSideSave(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        if (vista.LVistaLeft)
        {
            _lVistaFacadeWorkspace.LEngineLeftSave(vista.LVistaChosen);
            return;
        }

        _lVistaFacadeWorkspace.LEngineRightSave(vista.LVistaChosen);
    }

    public int LEngineUsageRead(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        if (vista.LVistaStored is not long id)
        {
            return 0;
        }

        return vista.LVistaSubject switch
        {
            LSubject.LSubjectExample =>
                _lVistaFacadeEntry.LEngineUsageRead(LOwner.LOwnerExample).GetValueOrDefault(id),
            LSubject.LSubjectSituation =>
                _lVistaFacadeEntry.LEngineUsageRead(LOwner.LOwnerSituation).GetValueOrDefault(id),
            LSubject.LSubjectReference =>
                _lVistaFacadeEntry.LEngineUsageRead(LOwner.LOwnerReference).GetValueOrDefault(id),
            LSubject.LSubjectAuthor =>
                _lVistaFacadeAuthor.LEngineAuthorFind(id)?.LCatalogAuthorWork ?? 0,
            _ => 0,
        };
    }

    public string LEngineTallyRead(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        LOwner owner = vista.LVistaSubject switch
        {
            LSubject.LSubjectExample => LOwner.LOwnerExample,
            LSubject.LSubjectSituation => LOwner.LOwnerSituation,
            LSubject.LSubjectReference => LOwner.LOwnerReference,
            _ => throw new InvalidOperationException("The vista lists nothing a tally counts."),
        };
        return _lVistaFacadeEntry.LEngineTallyRead(vista.LVistaStored, owner);
    }

    public LRevision? LEngineVistaDelete(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        if (vista.LVistaChosen is not long id || id <= 0)
        {
            return null;
        }

        LRevision? revision;
        switch (vista.LVistaSubject)
        {
            case LSubject.LSubjectEntry:
                revision = _lVistaFacadeEntry.LEngineEntryDelete(id);
                break;
            case LSubject.LSubjectExample:
                _lVistaFacadeExample.LEngineExampleDelete(id, true);
                revision = null;
                break;
            case LSubject.LSubjectSituation:
                _lVistaFacadeSituation.LEngineSituationDelete(id, true);
                revision = null;
                break;
            case LSubject.LSubjectReference:
                _lVistaFacadeReference.LEngineReferenceDelete(id, true);
                revision = null;
                break;
            case LSubject.LSubjectAuthor:
                _lVistaFacadeAuthor.LEngineAuthorDelete(id, true);
                revision = null;
                break;
            default:
                return null;
        }

        if (vista.LVistaChosen == id)
        {
            vista.LVistaSelect(null);
        }

        return revision;
    }
}
