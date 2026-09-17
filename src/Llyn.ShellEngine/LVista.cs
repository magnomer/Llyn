using System;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LVista
{
    private readonly LEngine _lEngine;

    internal LVista(LEngine engine, long id, string tab, LCatalogOrder order, LCatalogFilter filter, bool blank)
    {
        _lEngine = engine;
        LVistaId = id;
        LVistaTab = tab;
        LVistaOrder = order;
        LVistaFilter = filter;
        LVistaBlank = blank;
    }

    public long LVistaId { get; }

    public string LVistaTab { get; }

    public bool LVistaBlank { get; }

    public LCatalogOrder LVistaOrder { get; private set; }

    public LCatalogFilter LVistaFilter { get; private set; }

    public string LVistaQuery { get; private set; } = string.Empty;

    public long? LVistaChosen { get; private set; }

    public void LVistaOrderSet(LCatalogOrder order)
    {
        if (order == LVistaOrder)
        {
            return;
        }

        LVistaOrder = order;
        _lEngine.LEngineLayoutSave([new LLayout(LVistaTab, LLayoutOrder: order)]);
        _lEngine.LEngineBulletinRaise(LSubject.LSubjectVista, LVistaId);
    }

    public void LVistaFilterSet(LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        if (filter == LVistaFilter)
        {
            return;
        }

        LVistaFilter = filter;
        _lEngine.LEngineLayoutSave([new LLayout(LVistaTab, LLayoutFilter: filter)]);
        _lEngine.LEngineBulletinRaise(LSubject.LSubjectVista, LVistaId);
    }

    public void LVistaQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (string.Equals(query, LVistaQuery, StringComparison.Ordinal))
        {
            return;
        }

        LVistaQuery = query;
        _lEngine.LEngineBulletinRaise(LSubject.LSubjectVista, LVistaId);
    }

    public void LVistaSelect(long? id)
    {
        if (id == LVistaChosen)
        {
            return;
        }

        LVistaChosen = id;
    }
}
