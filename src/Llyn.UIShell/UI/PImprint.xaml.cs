using System.Windows.Controls;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PImprint : UserControl
{
    private PWindow _pImprintHost = null!;

    private LEngine _lEngine = null!;

    private PReference _pImprintOwner = null!;

    public PImprint()
    {
        InitializeComponent();
    }

    internal void PImprintAttach(PWindow host, LEngine engine, PReference owner)
    {
        _pImprintHost = host;
        _lEngine = engine;
        _pImprintOwner = owner;

        PAuthorCredit.ItemsSource = _pAuthorCredit;
        PAuthorList.ItemsSource = _pAuthorCatalog;
    }

    internal void PImprintDraftOpen(long? reference)
    {
        PImprintDraftShow(PImprintDraftStart(reference));
    }

    internal void PImprintClear()
    {
        PImprintApply(null);
    }

    internal void PImprintClose()
    {
        PAuthorMenu.IsOpen = false;
    }
}
