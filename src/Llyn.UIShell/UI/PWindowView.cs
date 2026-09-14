using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PWindow
{
    internal void PWindowViewRestore(LWorkspaceState state)
    {
        LSettings settings = _lEngine.LEngineSettingsRead();
        Dictionary<string, LLayout> layout = new(StringComparer.Ordinal);

        foreach (LLayout record in settings.LSettingsLayout ?? [])
        {
            layout[record.LLayoutTab] = record;
        }

        PLibrary.PSieveRestore(PWindowFilterRead(layout, "library"));
        PPhonology.PLensRestore(PWindowFilterRead(layout, "phonology"));
        PFavorite.PStrainerRestore(PWindowFilterRead(layout, "favorite"));
        PTaxonomy.PLatticeRestore(PWindowFilterRead(layout, "taxonomy"));
        PTenor.PGrilleRestore(PWindowFilterRead(layout, "tenor"));
        PRepertoire.PMeshRestore(PWindowFilterRead(layout, "repertoire"));
        PCorpus.PGauzeRestore(PWindowFilterRead(layout, "corpus"));
        PReference.PTrellisRestore(PWindowFilterRead(layout, "reference"));
        PGuild.PLouverRestore(PWindowFilterRead(layout, "guild"));

        PLibrary.POrderRestore(PWindowOrderRead(layout, "library", LCatalogOrder.LCatalogOrderHeadword));
        PPhonology.PSequenceRestore(PWindowOrderRead(layout, "phonology", LCatalogOrder.LCatalogOrderHeadword));
        PFavorite.PSeriesRestore(PWindowOrderRead(layout, "favorite", LCatalogOrder.LCatalogOrderHeadword));
        PTaxonomy.PFunnelRestore(PWindowOrderRead(layout, "taxonomy", LCatalogOrder.LCatalogOrderName));
        PTenor.PDegreeRestore(PWindowOrderRead(layout, "tenor", LCatalogOrder.LCatalogOrderName));
        PRepertoire.PTierRestore(PWindowOrderRead(layout, "repertoire", LCatalogOrder.LCatalogOrderName));
        PReference.PGradeRestore(PWindowOrderRead(layout, "reference", LCatalogOrder.LCatalogOrderName));
        PCorpus.PRankRestore(PWindowOrderRead(layout, "corpus", LCatalogOrder.LCatalogOrderText));
        PGuild.PEchelonRestore(PWindowOrderRead(layout, "guild", LCatalogOrder.LCatalogOrderName));

        PDuplex.PDuplexRestore(state);

        PNavigationRestore(settings);
    }

    private static LCatalogFilter PWindowFilterRead(Dictionary<string, LLayout> layout, string tab)
    {
        return layout.TryGetValue(tab, out LLayout? record)
            ? record.LLayoutFilter ?? LCatalogFilter.LCatalogFilterEmpty
            : LCatalogFilter.LCatalogFilterEmpty;
    }

    private static LCatalogOrder PWindowOrderRead(Dictionary<string, LLayout> layout, string tab, LCatalogOrder fallback)
    {
        return layout.TryGetValue(tab, out LLayout? record)
            ? record.LLayoutOrder ?? fallback
            : fallback;
    }
}
