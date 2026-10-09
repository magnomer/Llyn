using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LCatalogFacade : LTagPort, LRegisterPort, LFavoritePort
{
    private readonly LEngineHearth _lCatalogFacadeHearth;
    private readonly LVistaRowFacade _lCatalogFacadeRow;
    private readonly object _lCatalogFacadeGate;
    private LEngineStaff LCatalogFacadeStaff => _lCatalogFacadeHearth.LEngineStaffHeld;

    internal LCatalogFacade(LEngineHearth hearth, LVistaRowFacade row)
    {
        ArgumentNullException.ThrowIfNull(hearth);
        ArgumentNullException.ThrowIfNull(row);
        _lCatalogFacadeHearth = hearth;
        _lCatalogFacadeRow = row;
        _lCatalogFacadeGate = _lCatalogFacadeHearth.LEngineGate;
    }

    internal IReadOnlyList<LTag> LEngineTagRead()
    {
        lock (_lCatalogFacadeGate)
        {
            return LCatalogFacadeStaff.LEngineStaffCatalog.LCatalogStaffTag.LTagClerkRead();
        }
    }

    public IReadOnlyList<LTag> LEngineTagFind(string query, LCatalogOrder order)
    {
        lock (_lCatalogFacadeGate)
        {
            return LCatalogFacadeStaff.LEngineStaffCatalog.LCatalogStaffTag.LTagClerkFind(query, order);
        }
    }

    public LTagOffer LEngineTagFind(LTenure held, long card, string text)
    {
        ArgumentNullException.ThrowIfNull(held);

        LDraft? draft = held.LTenureRead();
        lock (_lCatalogFacadeGate)
        {
            return LCatalogFacadeStaff.LEngineStaffCatalog.LCatalogStaffTag.LTagClerkFind(text, draft, card);
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

    public LTag LEngineTagCreate(string text)
    {
        LTag created;
        lock (_lCatalogFacadeGate)
        {
            created = LCatalogFacadeStaff.LEngineStaffCatalog.LCatalogStaffTag.LTagClerkCreate(text);
        }

        _lCatalogFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectTag, created.LTagId);
        return created;
    }

    public LRegisterOffer LEngineRegisterFind(LTenure held, long card, string text)
    {
        ArgumentNullException.ThrowIfNull(held);

        LDraft? draft = held.LTenureRead();
        string language = held.LTenureLanguageRead();
        lock (_lCatalogFacadeGate)
        {
            return LCatalogFacadeStaff.LEngineStaffCatalog.LCatalogStaffRegister
                .LRegisterClerkFind(text, language, draft, card);
        }
    }

    public IReadOnlyList<LCatalogRegister> LEngineRegisterFind(string query, LCatalogOrder order)
    {
        lock (_lCatalogFacadeGate)
        {
            return LCatalogFacadeStaff.LEngineStaffCatalog.LCatalogStaffRegister.LRegisterClerkFind(query, order);
        }
    }

    public IReadOnlyList<LCatalogRegister> LEngineRegisterFind(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);
        List<LCatalogRegister> rows = [];
        bool kept = false;
        foreach (LCatalogRegister row in LEngineRegisterFind(vista.LVistaQuery, vista.LVistaOrder))
        {
            bool chosen = vista.LVistaMatch(row.LCatalogRegisterStored.LRegisterId);
            kept |= chosen;
            rows.Add(row with { LCatalogRegisterChosen = chosen });
        }

        if (!kept)
        {
            vista.LVistaSelect(null);
        }

        return rows;
    }

    public LRegister LEngineRegisterCreate(string name)
    {
        LRegister created;
        lock (_lCatalogFacadeGate)
        {
            created = LCatalogFacadeStaff.LEngineStaffCatalog.LCatalogStaffRegister.LRegisterClerkCreate(name);
        }

        _lCatalogFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectRegister, created.LRegisterId);
        return created;
    }

    public IReadOnlyList<LCatalogFavorite> LEngineFavoriteFind(string query, LCatalogOrder order, LCatalogFilter filter)
    {
        lock (_lCatalogFacadeGate)
        {
            return LCatalogFacadeStaff.LEngineStaffCatalog.LCatalogStaffFavorite
                .LFavoriteClerkFind(query, order, filter);
        }
    }

    public IReadOnlyList<LVistaRow> LEngineFavoriteFind(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        lock (_lCatalogFacadeGate)
        {
            IReadOnlyList<LCatalogFavorite> favorites = LEngineFavoriteFind(
                vista.LVistaQuery, vista.LVistaOrder, vista.LVistaFilter);
            List<LEntry> entries = new(favorites.Count);
            foreach (LCatalogFavorite favorite in favorites)
            {
                entries.Add(favorite.LCatalogFavoriteEntry);
            }

            return _lCatalogFacadeRow.LEngineVistaBuild(entries, vista.LVistaChosen);
        }
    }

    public bool LEngineFavoriteCheck(long entryId)
    {
        lock (_lCatalogFacadeGate)
        {
            return LCatalogFacadeStaff.LEngineStaffCatalog.LCatalogStaffFavorite.LFavoriteClerkCheck(entryId);
        }
    }

    public void LEngineFavoriteSave(long entryId)
    {
        lock (_lCatalogFacadeGate)
        {
            LCatalogFacadeStaff.LEngineStaffCatalog.LCatalogStaffFavorite.LFavoriteClerkSave(entryId);
        }

        _lCatalogFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectFavorite, entryId);
    }

    public void LEngineFavoriteDelete(long entryId)
    {
        lock (_lCatalogFacadeGate)
        {
            LCatalogFacadeStaff.LEngineStaffCatalog.LCatalogStaffFavorite.LFavoriteClerkDelete(entryId);
        }

        _lCatalogFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectFavorite, entryId);
    }
}
