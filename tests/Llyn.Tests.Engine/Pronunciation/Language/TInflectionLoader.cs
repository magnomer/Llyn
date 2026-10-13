using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TInflectionLoader
{
    private const string TInflectionBookJson = """
        {
          "kinds": [ { "name": "a", "match": "^(?<root>.+)ar$" } ],
          "columns": [[1], [2]],
          "stems": [ { "values": [9], "templates": { "a": "${root}" }, "endings": ["o", null] } ],
          "rules": [ ["o$", "o"] ],
          "folds": [ ["á", "a"] ]
        }
        """;

    [Fact]
    public void InflectionPackRead_FileNamed_LoadsBook()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(
            """{ "inflection": "inflection.json" }""");
        pack.TLanguageFixtureSave("inflection.json", TInflectionBookJson);

        LInflectionBook? book = TInterface.TLanguageLoad(pack.TLanguageFixtureName).LLanguageInflection;
        pack.TLanguageFixtureSave("inflection.json", TInflectionBookJson.Replace("\"o\", null", "\"os\", null"));
        LInflectionBook? edited = TInterface.TLanguageLoad(pack.TLanguageFixtureName).LLanguageInflection;

        Assert.NotNull(book);
        Assert.Single(book.LInflectionBookKinds);
        LInflectionStem stem = Assert.Single(book.LInflectionBookStems);
        Assert.Equal([9L], stem.LInflectionStemValues);
        Assert.Equal([1L], Assert.Single(stem.LInflectionStemEndings).LInflectionEndingValues);
        Assert.Equal("hablo", TInterfaceInflection.TInflectionBookResolve(book, "hablar", [1, 9]));
        Assert.Matches("^[0-9a-f]{64}$", book.LInflectionBookStamp);
        Assert.NotNull(edited);
        Assert.NotEqual(book.LInflectionBookStamp, edited.LInflectionBookStamp);
        Assert.Null(book.LInflectionBookLayout);
    }

    [Fact]
    public void InflectionPackRead_Inline_LoadsBook()
    {
        LInflectionBook? book = TLanguageFixture.TLanguageFixtureLoad(
            "{ \"inflection\": " + TInflectionBookJson + " }").LLanguageInflection;

        Assert.NotNull(book);
        Assert.Equal("hablo", TInterfaceInflection.TInflectionBookResolve(book, "hablar", [9, 1]));
        Assert.Matches("^[0-9a-f]{64}$", book.LInflectionBookStamp);
    }

    [Fact]
    public void InflectionPackRead_MissingFile_ReturnsNull()
    {
        Assert.Null(
            TLanguageFixture.TLanguageFixtureLoad("""{ "inflection": "inflection.json" }""").LLanguageInflection);
        Assert.Null(TLanguageFixture.TLanguageFixtureLoad("{}").LLanguageInflection);
    }

    [Fact]
    public void InflectionPackRead_BadRegex_SkipsRule()
    {
        string json = TInflectionBookJson
            .Replace("""[ ["o$", "o"] ]""", """[ ["(", "x"], ["o$", "o", "a"] ]""")
            .Replace("""[ ["á", "a"] ]""", """[ ["[", "a"], ["á", "a"] ]""");

        LInflectionBook? book =
            TLanguageFixture.TLanguageFixtureLoad("{ \"inflection\": " + json + " }").LLanguageInflection;

        Assert.NotNull(book);
        LInflectionRule rule = Assert.Single(book.LInflectionBookRules);
        Assert.Equal("a", rule.LInflectionRuleKind);
        Assert.Single(book.LInflectionBookFolds);
    }

    [Fact]
    public void InflectionPackRead_SpanishPack_LoadsLayoutForVerbs()
    {
        LInflectionBook? book = TInterface.TLanguageLoad("Spanish").LLanguageInflection;

        Assert.NotNull(book);
        LInflectionLayout? layout = book.LInflectionBookLayout;
        Assert.NotNull(layout);
        Assert.Equal(6, layout.LInflectionLayoutPart);
        Assert.Equal(
            3,
            layout.LInflectionLayoutCollapsed.LInflectionSheetLines.Count(
                line => line.LInflectionLineGroup.Length > 0));
        Assert.Empty(layout.LInflectionLayoutCollapsed.LInflectionSheetHeaders);
        Assert.Equal(11, layout.LInflectionLayoutExpanded.LInflectionSheetLines.Count);
        Assert.Equal(6, layout.LInflectionLayoutExpanded.LInflectionSheetHeaders.Count);
        Assert.All(
            layout.LInflectionLayoutExpanded.LInflectionSheetLines,
            line => Assert.Equal(6, line.LInflectionLineCells.Count));
        Assert.Equal("Inflection.FirstSingular", layout.LInflectionLayoutExpanded.LInflectionSheetHeaders[0]);
        LInflectionLine imperfect = layout.LInflectionLayoutCollapsed.LInflectionSheetLines
            .Single(
                line => line.LInflectionLineLabel == "Inflection.Imperfect" && line.LInflectionLineCells.Count == 2);
        Assert.Equal([10L, 14, 17, 20, 24], imperfect.LInflectionLineCells[0]);
        Assert.Equal([10L, 14, 17, 20, 25], imperfect.LInflectionLineCells[1]);
    }

    [Fact]
    public void InflectionPackRead_SpanishPack_LoadsDefaultAndCustomLayouts()
    {
        LInflectionLayout layout = TInterface.TLanguageLoad("Spanish").LLanguageInflection!.LInflectionBookLayout!;
        LInflectionLayout? custom = layout.LInflectionLayoutCustom;

        Assert.Equal(
            ["Inflection.Indicative", "Inflection.Subjunctive", "Inflection.Imperative"],
            TInflectionGroupRead(layout.LInflectionLayoutCollapsed));
        Assert.Equal(
            ["Inflection.Indicative", "Inflection.Subjunctive", "Inflection.Imperative"],
            TInflectionGroupRead(layout.LInflectionLayoutExpanded));
        Assert.Equal(
            [11L, 18, 20, 22], layout.LInflectionLayoutCollapsed.LInflectionSheetLines[^1].LInflectionLineCells[0]);
        Assert.Equal(9, layout.LInflectionLayoutCollapsed.LInflectionSheetLines.Count);
        Assert.NotNull(custom);
        Assert.Equal(6, custom.LInflectionLayoutPart);
        Assert.Null(custom.LInflectionLayoutCustom);
        Assert.Equal(
            [
                "Inflection.FutureConditional", "Inflection.Imperative", "Inflection.Indicative",
                "Inflection.Subjunctive",
            ],
            TInflectionGroupRead(custom.LInflectionLayoutCollapsed));
        Assert.Equal(
            [
                "Inflection.Future", "Inflection.Conditional", "Inflection.Imperative", "Inflection.Indicative",
                "Inflection.Subjunctive",
            ],
            TInflectionGroupRead(custom.LInflectionLayoutExpanded));
        Assert.Equal(11, custom.LInflectionLayoutExpanded.LInflectionSheetLines.Count);
        Assert.Equal(6, custom.LInflectionLayoutExpanded.LInflectionSheetHeaders.Count);
    }

    [Fact]
    public void InflectionPackRead_LayoutWithoutCustom_UsesDefaultSheets()
    {
        string json = TInflectionBookJson.Replace(
            "\"folds\": [ [\"á\", \"a\"] ]",
            """
            "folds": [ ["á", "a"] ],
              "layout": {
                "part": 6,
                "collapsed": {
                  "groups": [ { "label": "A", "lines": [ { "label": "", "values": [9], "cells": [[1]] } ] } ]
                },
                "expanded": { "columns": [[1], [2]], "groups": [ { "label": "B", "lines": [ { "values": [9] } ] } ] }
              }
            """);

        LInflectionLayout? layout =
            TLanguageFixture.TLanguageFixtureLoad("{ \"inflection\": " + json + " }")
                .LLanguageInflection?.LInflectionBookLayout;

        Assert.NotNull(layout);
        LInflectionLayout? custom = layout.LInflectionLayoutCustom;
        Assert.NotNull(custom);
        Assert.Equal(6, custom.LInflectionLayoutPart);
        Assert.Same(layout.LInflectionLayoutCollapsed, custom.LInflectionLayoutCollapsed);
        Assert.Same(layout.LInflectionLayoutExpanded, custom.LInflectionLayoutExpanded);
        Assert.Equal(["A"], TInflectionGroupRead(custom.LInflectionLayoutCollapsed));
        Assert.Equal(["B"], TInflectionGroupRead(custom.LInflectionLayoutExpanded));
    }

    private static string[] TInflectionGroupRead(LInflectionSheet sheet) =>
        [.. sheet.LInflectionSheetLines
            .Select(line => line.LInflectionLineGroup)
            .Where(group => group.Length > 0)];

    [Theory]
    [InlineData("recibir", "9+12+18+21", "recibis", "recibís", "")]
    [InlineData("hablar", "9+13+17+20", "hable", "hablé", "")]
    [InlineData("hablar", "10+14+24+17+21", "hablaramos", "habláramos", "")]
    [InlineData("vivir", "11+22+18+21", "vivid", "vivid", "")]
    [InlineData("tener", "9+13+17+20", "teni", "tuve", "1:2")]
    [InlineData("buscar", "9+13+17+20", "busce", "busqué", "")]
    [InlineData("leer", "9+13+19+20", "leio", "leyó", "")]
    [InlineData("comer", "9+12+18+21", "comeis", "coméis", "")]
    [InlineData("construir", "9+12+17+20", "construo", "construyo", "7:1")]
    [InlineData("seguir", "9+12+17+20", "seguo", "sigo", "1:1")]
    public void InflectionPackRead_SpanishPack_PredictsHandChecks(
        string headword, string cell, string predicted, string actual, string marks)
    {
        LInflectionBook book = TInterface.TLanguageLoad("Spanish").LLanguageInflection!;
        long[] codes = [.. cell.Split('+').Select(long.Parse)];

        Assert.Equal(predicted, TInterfaceInflection.TInflectionBookResolve(book, headword, codes));
        IReadOnlyList<LInflectionMark>? scan = TInflectionBook.TInflectionBookScan(book, headword, codes, actual);
        Assert.NotNull(scan);
        Assert.Equal(marks, TInterfaceInflection.TInflectionMarkFormat(scan));
        Assert.Null(TInflectionBook.TInflectionBookScan(book, headword, [11, 22, 17, 20], actual));
    }
}
