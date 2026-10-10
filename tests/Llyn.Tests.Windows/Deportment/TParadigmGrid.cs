using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using Llyn.Conduct;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TParadigmGrid
{
    [Fact]
    public void ParadigmView_Set_ShowsCollapsedHidesExpanded()
    {
        List<Visibility> shown = [];

        TWindow.TWindowRun(() =>
        {
            QParadigm box = new() { QParadigmSheet = TParadigmSheetCreate([]) };
            UIElementCollection parts = TParadigmPartsRead(box);
            shown.Add(box.Visibility);
            shown.Add(parts[1].Visibility);
            shown.Add(parts[2].Visibility);
            shown.Add(parts[3].Visibility);
        });

        Assert.Equal([Visibility.Visible, Visibility.Visible, Visibility.Visible, Visibility.Collapsed], shown);
    }

    [Fact]
    public void ParadigmSwitch_FullChecked_ShowsExpanded()
    {
        List<Visibility> shown = [];

        TWindow.TWindowRun(() =>
        {
            QParadigm box = new() { QParadigmSheet = TParadigmSheetCreate([]) };
            UIElementCollection parts = TParadigmPartsRead(box);
            RadioButton full = (RadioButton)((Panel)((Border)parts[1]).Child).Children[1];
            full.IsChecked = true;
            shown.Add(parts[2].Visibility);
            shown.Add(parts[3].Visibility);
        });

        Assert.Equal([Visibility.Collapsed, Visibility.Visible], shown);
    }

    [Fact]
    public void ParadigmForm_Marked_PaintsMarkedRun()
    {
        List<string> texts = [];
        bool styled = false;

        TWindow.TWindowRun(() =>
        {
            System.Windows.Application application =
                System.Windows.Application.Current ?? new System.Windows.Application();
            Style marked = new(typeof(Run));
            marked.Setters.Add(new Setter(TextElement.ForegroundProperty, Brushes.Red));
            application.Resources["Theme.Paradigm.Marked"] = marked;
            try
            {
                QParadigm box = new() { QParadigmSheet = TParadigmSheetCreate([new CParadigmMark(1, 2)]) };
                Grid collapsed = (Grid)TParadigmPartsRead(box)[2];
                TextBlock form = collapsed.Children.OfType<TextBlock>().Single(block => block.Inlines.Count > 0);
                List<Run> runs = form.Inlines.OfType<Run>().ToList();
                texts.AddRange(runs.Select(run => run.Text));
                styled = runs.Count == 3 && ReferenceEquals(runs[1].Style, marked) && runs[0].Style != marked;
            }
            finally
            {
                application.Resources.Remove("Theme.Paradigm.Marked");
            }
        });

        Assert.Equal(["t", "uv", "e"], texts);
        Assert.True(styled);
    }

    [Fact]
    public void ParadigmForm_Split_PaintsRootCutAndEnding()
    {
        List<string> texts = [];
        bool styled = false;

        TWindow.TWindowRun(() =>
        {
            System.Windows.Application application =
                System.Windows.Application.Current ?? new System.Windows.Application();
            Style marked = new(typeof(Run));
            marked.Setters.Add(new Setter(TextElement.ForegroundProperty, Brushes.Red));
            Style cut = new(typeof(Run));
            cut.Setters.Add(new Setter(TextElement.ForegroundProperty, Brushes.Gray));
            application.Resources["Theme.Paradigm.Marked"] = marked;
            application.Resources["Theme.Paradigm.Cut"] = cut;
            try
            {
                QParadigm box = new() { QParadigmSheet = TParadigmSheetCreate([new CParadigmMark(0, 4)], "tengo", 4) };
                Grid collapsed = (Grid)TParadigmPartsRead(box)[2];
                TextBlock form = collapsed.Children.OfType<TextBlock>().Single(block => block.Inlines.Count > 0);
                List<Run> runs = form.Inlines.OfType<Run>().ToList();
                texts.AddRange(runs.Select(run => run.Text));
                styled = runs.Count == 3
                    && ReferenceEquals(runs[0].Style, marked)
                    && ReferenceEquals(runs[1].Style, cut)
                    && runs[2].Style != marked;
            }
            finally
            {
                application.Resources.Remove("Theme.Paradigm.Marked");
                application.Resources.Remove("Theme.Paradigm.Cut");
            }
        });

        Assert.Equal(["teng", "-", "o"], texts);
        Assert.True(styled);
    }

    [Fact]
    public void ParadigmTable_TwoGroups_ClosesGroupAndRulesInside()
    {
        List<(int, int)> rules = [];

        TWindow.TWindowRun(() =>
        {
            CParadigmTable table = new(
                [],
                [
                    new CParadigmLine(
                        "Inflection.Indicative", "Inflection.Present", [new CParadigmForm("tengo", [], null)]),
                    new CParadigmLine("", "Inflection.Preterite", [new CParadigmForm("tuve", [], null)]),
                    new CParadigmLine(
                        "Inflection.Subjunctive", "Inflection.Present", [new CParadigmForm("tenga", [], null)]),
                ]);
            QParadigm box = new()
            {
                QParadigmSheet = QParadigmSheet.QParadigmSheetCreate(new CParadigmView(table, table)),
            };
            Grid collapsed = (Grid)TParadigmPartsRead(box)[2];
            rules.AddRange(collapsed.Children.OfType<Rectangle>()
                .Select(rule => (Grid.GetRow(rule), Grid.GetColumn(rule))));
        });

        Assert.Equal([(0, 1), (1, 0)], rules);
    }

    [Fact]
    public void ParadigmView_NullAndNoItems_Collapses()
    {
        Visibility shown = Visibility.Visible;

        TWindow.TWindowRun(() =>
        {
            QParadigm box = new() { QParadigmSheet = TParadigmSheetCreate([]) };
            box.QParadigmSheet = null;
            shown = box.Visibility;
        });

        Assert.Equal(Visibility.Collapsed, shown);
    }

    private static QParadigmSheet? TParadigmSheetCreate(
        IReadOnlyList<CParadigmMark> marks, string text = "tuve", int split = 0)
    {
        CParadigmTable collapsed = new(
            [],
            [new CParadigmLine(
                "Inflection.Indicative", "Inflection.Preterite", [new CParadigmForm(text, marks, null, split)])]);
        CParadigmTable expanded = new(
            ["Inflection.FirstSingular", "Inflection.SecondSingular"],
            [new CParadigmLine(
                "Inflection.Indicative",
                "Inflection.Preterite",
                [new CParadigmForm(text, marks, null, split), new CParadigmForm("…", [], "Paradigm.Pending")])]);
        return QParadigmSheet.QParadigmSheetCreate(new CParadigmView(collapsed, expanded));
    }

    private static UIElementCollection TParadigmPartsRead(QParadigm box) =>
        ((Panel)((Border)box.Child).Child).Children;
}
