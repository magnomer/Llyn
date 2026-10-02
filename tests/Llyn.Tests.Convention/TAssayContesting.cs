using Xunit;

namespace Convention.Tests;

public sealed class TAssayContesting
{
    private const string TAssayMarkupPath = "src/Llyn.UIVeneer/obj/Assay/PAssayView.g.cs";

    private const string TAssayMarkupDriver = """
        using Llyn.Conduct.Assay;

        namespace Llyn.UIDeportment.Assay;

        public sealed class QAssay
        {
            private int _count;

            public void QAssayRun(CAssay gate, PAssayView view)
            {
                view.PAssayViewCount = gate.CAssayRead();
                _count = gate.CAssayRead();
                _count = view.PAssayViewCount;
            }
        }

        public partial class PAssayView : System.Windows.Controls.UserControl
        {
            public int PAssayViewCount { get; set; }
        }
        """;

    private const string TAssayMarkupText = """
        namespace Llyn.UIDeportment.Assay
        {
            public partial class PAssayView :
                System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector
            {
                private bool _contentLoaded;

                public void InitializeComponent()
                {
                    if (_contentLoaded)
                    {
                        return;
                    }

                    _contentLoaded = true;
                    System.Uri resourceLocater =
                        new System.Uri("/Llyn.UIVeneer;component/assay/passayview.xaml", System.UriKind.Relative);
                    System.Windows.Application.LoadComponent(this, resourceLocater);
                }

                void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
                {
                    this._contentLoaded = true;
                }
            }
        }
        """;

    [Fact]
    public void AuditTruth_OnePlainWriter_ReportsContesting()
    {
        TAssayTruth.TAssayTruthCheck(TAssayTruth.TAssayDriverFormat("""
                private int _count;
    """, """
                    _count = gate.CAssayRead();
                    _count = 3;
    """), "Contesting", new TViolation(
            TAssayTruth.TAssayDriverPath, 13, "QAssay._count", "Contesting",
            "written by the engine at line 12 and by the shell here"));
    }

    [Fact]
    public void AuditTruth_TwoPlainWriters_ListsSecondWriter()
    {
        TAssayTruth.TAssayTruthCheck(TAssayTruth.TAssayDriverFormat("""
                private int _count;
    """, """
                    _count = gate.CAssayRead();
                    _count = 3;
                    _count = 4;
    """), "Contesting", new TViolation(
            TAssayTruth.TAssayDriverPath, 13, "QAssay._count", "Contesting",
            "written by the engine at line 12 and by the shell here, also at QAssay.cs:14"));
    }

    [Fact]
    public void AuditTruth_PlainRecord_ReportsContesting()
    {
        TAssayTruth.TAssayTruthCheck(TAssayTruth.TAssayDriverFormat("""
        private int _count;
        private readonly CAssay _gate = new();

        private sealed record QAssayRow(int QAssayRowCount);

        private void QAssayRowTake()
        {
            Array.ForEach(new[] { new QAssayRow(3) }, QAssayRowShow);
        }

        private void QAssayRowShow(QAssayRow row)
        {
            _count = row.QAssayRowCount;
        }
    """, """
            _count = gate.CAssayRead();
    """), "Contesting", new TViolation(
            TAssayTruth.TAssayDriverPath, 20, "QAssay._count", "Contesting",
            "written by the engine at line 25 and by the shell here"));
    }

    [Fact]
    public void AuditTruth_EngineRecord_AllowsEngineCopy()
    {
        TAssayTruth.TAssayTruthCheck(TAssayTruth.TAssayDriverFormat("""
        private int _count;
        private readonly CAssay _gate = new();

        private sealed record QAssayRow(int QAssayRowCount);

        private void QAssayRowTake()
        {
            Array.ForEach(new[] { new QAssayRow(_gate.CAssayRead()) }, QAssayRowShow);
        }

        private void QAssayRowShow(QAssayRow row)
        {
            _count = row.QAssayRowCount;
        }
    """, """
            _count = gate.CAssayRead();
    """), "Contesting", null);
    }

    [Fact]
    public void AuditTruth_PlainProperty_ReportsContesting()
    {
        TAssayTruth.TAssayTruthCheck(TAssayTruth.TAssayDriverFormat("""
        private int _count;
        private readonly CAssay _gate = new();

        private sealed class QAssayBox
        {
            public int QAssayBoxCount { get; set; }
        }

        private void QAssayBoxTake(QAssayBox box)
        {
            box.QAssayBoxCount = 3;
            _count = box.QAssayBoxCount;
        }
    """, """
            _count = gate.CAssayRead();
    """), "Contesting", new TViolation(
            TAssayTruth.TAssayDriverPath, 19, "QAssay._count", "Contesting",
            "written by the engine at line 24 and by the shell here"));
    }

    [Fact]
    public void AuditTruth_EngineProperty_AllowsEngineCopy()
    {
        TAssayTruth.TAssayTruthCheck(TAssayTruth.TAssayDriverFormat("""
        private int _count;
        private readonly CAssay _gate = new();

        private sealed class QAssayBox
        {
            public int QAssayBoxCount { get; set; }
        }

        private void QAssayBoxTake(QAssayBox box)
        {
            box.QAssayBoxCount = _gate.CAssayRead();
            _count = box.QAssayBoxCount;
        }
    """, """
            _count = gate.CAssayRead();
    """), "Contesting", null);
    }

    [Fact]
    public void AuditTruth_PlainHeld_ReportsContesting()
    {
        TAssayTruth.TAssayTruthCheck(TAssayTruth.TAssayDriverFormat("""
        private int _count;
    """, """
            int held = items.Length;
            _count = gate.CAssayRead();
            _count = held;
    """), "Contesting", new TViolation(
            TAssayTruth.TAssayDriverPath, 14, "QAssay._count", "Contesting",
            "written by the engine at line 13 and by the shell here"));
    }

    [Fact]
    public void AuditTruth_EngineHeld_AllowsEngineCopy()
    {
        TAssayTruth.TAssayTruthCheck(TAssayTruth.TAssayDriverFormat("""
        private int _count;
    """, """
            int held = gate.CAssayRead();
            _count = gate.CAssayRead();
            _count = held;
    """), "Contesting", null);
    }

    [Fact]
    public void AuditTruth_PlainSetter_ReportsContesting()
    {
        TAssayTruth.TAssayTruthCheck(TAssayTruth.TAssayDriverFormat("""
        private int _count;

        private int QAssayCount
        {
            get => _count;
            set => _count = value;
        }
    """, """
            _count = gate.CAssayRead();
            QAssayCount = items.Length;
    """), "Contesting", new TViolation(
            TAssayTruth.TAssayDriverPath, 13, "QAssay._count", "Contesting",
            "written by the engine at line 18 and by the shell here"));
    }

    [Fact]
    public void AuditTruth_EngineSetter_AllowsEngineCopy()
    {
        TAssayTruth.TAssayTruthCheck(TAssayTruth.TAssayDriverFormat("""
        private int _count;

        private int QAssayCount
        {
            get => _count;
            set => _count = value;
        }
    """, """
            _count = gate.CAssayRead();
            QAssayCount = gate.CAssayRead();
    """), "Contesting", null);
    }

    [Fact]
    public void AuditTruth_ContentLookup_ReportsContesting()
    {
        TAssayTruth.TAssayTruthCheck(TAssayTruth.TAssayDriverFormat("""
        private int _count;

        private static int QAssayLookupRead(int key) => key;
    """, """
            _count = QAssayLookupRead(gate.CAssayRead());
            _count = QAssayLookupRead(items.Length);
    """), "Contesting", new TViolation(
            TAssayTruth.TAssayDriverPath, 15, "QAssay._count", "Contesting",
            "written by the engine at line 14 and by the shell here"));
    }

    [Fact]
    public void AuditTruth_KeyLookup_AllowsEngineCopy()
    {
        TAssayTruth.TAssayTruthCheck(TAssayTruth.TAssayDriverFormat("""
        private int _count;

        private static int QAssayLookupRead(int key) => key;
    """, """
            _count = QAssayLookupRead(gate.CAssayRead());
            _count = QAssayLookupRead(3);
    """), "Contesting", null);
    }

    [Fact]
    public void AuditTruth_PlainConstructor_ReportsContesting()
    {
        TAssayTruth.TAssayTruthCheck(TAssayTruth.TAssayDriverFormat("""
        private int _count;
        private readonly CAssay _gate = new();

        private sealed class QAssayCell
        {
            public QAssayCell(int count)
            {
                QAssayCellCount = count;
            }

            public int QAssayCellCount { get; }
        }

        private void QAssayCellTake()
        {
            Array.ForEach(new[] { new QAssayCell(3) }, QAssayCellShow);
        }

        private void QAssayCellShow(QAssayCell cell)
        {
            _count = cell.QAssayCellCount;
        }
    """, """
            _count = gate.CAssayRead();
    """), "Contesting", new TViolation(
            TAssayTruth.TAssayDriverPath, 28, "QAssay._count", "Contesting",
            "written by the engine at line 33 and by the shell here"));
    }

    [Fact]
    public void AuditTruth_EngineConstructor_AllowsEngineCopy()
    {
        TAssayTruth.TAssayTruthCheck(TAssayTruth.TAssayDriverFormat("""
        private int _count;
        private readonly CAssay _gate = new();

        private sealed class QAssayCell
        {
            public QAssayCell(int count)
            {
                QAssayCellCount = count;
            }

            public int QAssayCellCount { get; }
        }

        private void QAssayCellTake()
        {
            Array.ForEach(new[] { new QAssayCell(_gate.CAssayRead()) }, QAssayCellShow);
        }

        private void QAssayCellShow(QAssayCell cell)
        {
            _count = cell.QAssayCellCount;
        }
    """, """
            _count = gate.CAssayRead();
    """), "Contesting", null);
    }

    [Fact]
    public void AuditTruth_PlainHelper_ReportsContesting()
    {
        TAssayTruth.TAssayTruthCheck(TAssayTruth.TAssayDriverFormat("""
        private int _count;

        private static void QAssayValueRefine(ref int field, int value)
        {
            field = value;
        }
    """, """
            _count = gate.CAssayRead();
            QAssayValueRefine(ref _count, items.Length);
    """), "Contesting", new TViolation(
            TAssayTruth.TAssayDriverPath, 18, "QAssay._count", "Contesting",
            "written by the engine at line 17 and by the shell here"));
    }

    [Fact]
    public void AuditTruth_EngineHelper_AllowsEngineCopy()
    {
        TAssayTruth.TAssayTruthCheck(TAssayTruth.TAssayDriverFormat("""
        private int _count;

        private static void QAssayValueRefine(ref int field, int value)
        {
            field = value;
        }
    """, """
            _count = gate.CAssayRead();
            QAssayValueRefine(ref _count, gate.CAssayRead());
    """), "Contesting", null);
    }

    [Fact]
    public void AuditTruth_CopyOnly_ReportsContesting()
    {
        TAssayTruth.TAssayTruthCheck(TAssayTruth.TAssayDriverFormat("""
        private int _count;
        private int _mirror;

        private void QAssayMirrorSync(QAssay fresh)
        {
            _mirror = fresh._mirror;
        }
    """, """
            _count = gate.CAssayRead();
            _count = _mirror;
    """), "Contesting", new TViolation(
            TAssayTruth.TAssayDriverPath, 19, "QAssay._count", "Contesting",
            "written by the engine at line 18 and by the shell here"));
    }

    [Fact]
    public void AuditTruth_CopyBeside_AllowsEngineCopy()
    {
        TAssayTruth.TAssayTruthCheck(TAssayTruth.TAssayDriverFormat("""
        private int _count;
        private int _mirror;

        private void QAssayMirrorSync(QAssay fresh)
        {
            _mirror = fresh._mirror;
        }
    """, """
            _mirror = gate.CAssayRead();
            _count = gate.CAssayRead();
            _count = _mirror;
    """), "Contesting", null);
    }

    [Fact]
    public void AuditTruth_MarkupProperty_ReportsContesting()
    {
        Assert.Equal(
            [
                new TViolation(
                    TAssayTruth.TAssayDriverPath, 13, "QAssay._count", "Contesting",
                    "written by the engine at line 12 and by the shell here")
            ],
            TAssayMarkupRun(true));
    }

    [Fact]
    public void AuditTruth_CodeProperty_AllowsEngineCopy()
    {
        Assert.Empty(TAssayMarkupRun(false));
    }

    private static List<TViolation> TAssayMarkupRun(bool markup)
    {
        Dictionary<string, string> sources = new(StringComparer.Ordinal)
        {
            [TAssayTruth.TAssayDriverPath] = TAssayMarkupDriver,
            [TAssayTruth.TAssayGatePath] = TAssayTruth.TAssayGateText,
        };
        if (markup)
        {
            sources[TAssayMarkupPath] = TAssayMarkupText;
        }

        string driverPath = Path.GetFullPath(Path.Combine(TAuditBinder.TAuditRoot, TAssayTruth.TAssayDriverPath));
        return TAuditBinder.TAuditAssayRun(sources, () => TAuditTruthWalker.TAuditRun([driverPath]))
            .Where(hit => hit.TViolationKind == "Contesting")
            .Select(hit => hit with { TViolationPath = TAuditBinder.TAuditRelativeRead(hit.TViolationPath) })
            .ToList();
    }
}
