using System;
using System.Windows.Controls;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PImprint : UserControl, PChronicleHost
{
    private PWindow _pImprintHost = null!;

    private LEngine _lEngine = null!;

    private PReference _pImprintOwner = null!;

    internal Action<bool>? PImprintChangeNotice { get; set; }

    public PImprint()
    {
        InitializeComponent();

        Resources.MergedDictionaries.Add(new PBylineTemplate(this));

        PAuthorCredit.ItemsSource = _pAuthorCredit;
        PBylineList.ItemsSource = _pBylineItem;
        PByline.CustomPopupPlacementCallback = PBylinePlace;
    }

    internal void PImprintAttach(PWindow host, LEngine engine, PReference owner)
    {
        _pImprintHost = host;
        _lEngine = engine;
        _pImprintOwner = owner;
    }

    internal void PImprintDraftOpen(long? reference)
    {
        PImprintDraftShow(PImprintDraftStart(reference));
    }

    internal void PImprintClear()
    {
        PImprintApply(null);
    }

    internal void PImprintAuthorUpdate()
    {
        PAuthorFind();
        PImprintDraftRestore();
    }

    internal void PImprintClose()
    {
        PBylineHide();
        PImprintKindMenu.IsOpen = false;
    }
}
