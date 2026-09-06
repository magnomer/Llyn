using System.Windows.Controls;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PReference : UserControl
{
    private PWindow _pReferenceHost = null!;

    private LEngine _lEngine = null!;

    private PObserver? _pReferenceObserver;

    public PReference()
    {
        InitializeComponent();
    }

    internal void PReferenceAttach(PWindow host, LEngine engine)
    {
        _pReferenceHost = host;
        _lEngine = engine;

        PShelf.ItemsSource = _pShelfList;
        PFootnote.ItemsSource = _pFootnoteList;
        PAuthorCredit.ItemsSource = _pAuthorCredit;
        PAuthorList.ItemsSource = _pAuthorCatalog;

        _pReferenceObserver = new PObserver(this, PReferenceBulletinHandle);
        engine.LEngineObserverAttach(_pReferenceObserver);
    }

    internal void PReferenceReset()
    {
        PReferenceClear();
        PShelfFind(PSurvey.Text ?? string.Empty);
    }

    internal bool PReferenceChangeCheck()
    {
        return PImprintChangeCheck();
    }

    internal void PReferenceClose()
    {
        if (_pReferenceObserver is not null)
        {
            _lEngine.LEngineObserverDetach(_pReferenceObserver);
            _pReferenceObserver = null;
        }

        PAuthorMenu.IsOpen = false;
        PGradeDropdown.IsOpen = false;
    }
}
