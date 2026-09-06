using System.Windows;
using System.Windows.Controls;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PReference : UserControl
{
    private PWindow _pReferenceHost = null!;

    private LEngine _lEngine = null!;

    public PReference()
    {
        InitializeComponent();
    }

    internal void PReferenceAttach(PWindow host, LEngine engine)
    {
        _pReferenceHost = host;
        _lEngine = engine;
    }

    internal void PReferenceReset()
    {
        PReferenceClear();
        PShelfFind(PSurvey.Text ?? string.Empty);
    }

    internal bool PReferenceChangeCheck()
    {
        return PImprint.Visibility == Visibility.Visible && PImprintChangeCheck();
    }

    internal void PReferenceClose()
    {
        PAuthorMenu.IsOpen = false;
        PGradeDropdown.IsOpen = false;
    }
}
