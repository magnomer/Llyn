using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LCorpus
{
    internal LCorpus(CAtelier atelier, LEditor editor, LLectern lectern, Func<bool> shownSeam, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(lectern);

        LCorpusEditor = editor;
        LCorpusStudio = CCorpus.CCorpusCreate(atelier, editor.LEditorStudio, shownSeam, envoy);
        LCorpusQuotation = new LQuotation(
            atelier.CAtelierEntryPort, atelier.CAtelierPortraitPort, LCorpusStudio.CCorpusQuotation);
        LCorpusStudio.CCorpusQuotation.CPanelCleared += lectern.LLecternClear;
        LCorpusStudio.CCorpusQuotation.CPanelDraftChanged += lectern.LLecternDraftShow;
    }

    private LEditor LCorpusEditor { get; }

    public CCorpus LCorpusStudio { get; }

    public LQuotation LCorpusQuotation { get; }

    public void LCorpusRowsApply(IReadOnlyList<CCatalogExample> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        if (!LCorpusStudio.CCorpusRowShown)
        {
            return;
        }

        foreach (CCatalogExample row in rows)
        {
            if (row.CCatalogExampleChosen)
            {
                return;
            }
        }

        LCorpusStudio.CCorpusExampleClose();
    }

    public void LCorpusDelete()
    {
        if (LCorpusStudio.CCorpusQuotationSide)
        {
            return;
        }

        LCorpusStudio.CCorpusAnthology.CAnthologyPanel.CPanelEntryDelete();
    }

    public Task LCorpusPortraitPrint(CPortraitLabel label, CPortraitLegend legend, CPressTicket ticket)
    {
        if (LCorpusStudio.CCorpusDisplayShown)
        {
            return LCorpusQuotation.LQuotationPortraitPrint(label, ticket);
        }

        if (LCorpusStudio.CCorpusPressAllowed)
        {
            return LCorpusStudio.CCorpusAnthology.CAnthologyPortraitPrint(legend, ticket);
        }

        return Task.CompletedTask;
    }

    public Task LCorpusPortraitExport(string path, CPortraitMedium format, CPortraitLabel label)
    {
        if (!LCorpusStudio.CCorpusPortraitAllowed)
        {
            return Task.CompletedTask;
        }

        return LCorpusQuotation.LQuotationPortraitExport(path, format, label);
    }

    internal void LCorpusVistaRestore(LVista vista, LVista quotation)
    {
        ArgumentNullException.ThrowIfNull(vista);
        ArgumentNullException.ThrowIfNull(quotation);

        LCorpusStudio.CCorpusAnthology.CAnthologyVistaRestore(vista);
        LCorpusQuotation.LQuotationVistaRestore(vista, quotation);
        LCorpusEditor.LEditorStudio.CEditorVistaRestore(quotation);
    }

    internal void LCorpusVistaRestore(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        LCorpusVistaRestore(
            atelier.CAtelierVistaStart(
                "corpus", CSubject.CSubjectExample, CCatalogOrder.CCatalogOrderText),
            atelier.CAtelierVistaStart(
                "quotation", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword));
    }
}
