using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TInflectionBook
{
    [Theory]
    [InlineData(
        "recibir", "9+12",
        "recibo recibes recibe recibimos recibis reciben",
        "recibo recibes recibe recibimos recibís reciben")]
    [InlineData(
        "recibir", "9+14",
        "recibía recibías recibía recibíamos recibíais recibían",
        "recibía recibías recibía recibíamos recibíais recibían")]
    [InlineData(
        "recibir", "9+13",
        "recibi recibiste recibio recibimos recibisteis recibieron",
        "recibí recibiste recibió recibimos recibisteis recibieron")]
    [InlineData(
        "recibir", "9+15",
        "recibire recibiras recibira recibiremos recibireis recibiran",
        "recibiré recibirás recibirá recibiremos recibiréis recibirán")]
    [InlineData(
        "recibir", "9+16",
        "recibiría recibirías recibiría recibiríamos recibiríais recibirían",
        "recibiría recibirías recibiría recibiríamos recibiríais recibirían")]
    [InlineData(
        "recibir", "11+22",
        "— recibe reciba recibamos recibid reciban",
        "— recibe reciba recibamos recibid reciban")]
    [InlineData(
        "recibir", "11+23",
        "— recibas reciba recibamos recibais reciban",
        "— recibas reciba recibamos recibáis reciban")]
    [InlineData(
        "recibir", "10+12",
        "reciba recibas reciba recibamos recibais reciban",
        "reciba recibas reciba recibamos recibáis reciban")]
    [InlineData(
        "recibir", "10+14+24",
        "recibiera recibieras recibiera recibieramos recibierais recibieran",
        "recibiera recibieras recibiera recibiéramos recibierais recibieran")]
    [InlineData(
        "recibir", "10+14+25",
        "recibiese recibieses recibiese recibiesemos recibieseis recibiesen",
        "recibiese recibieses recibiese recibiésemos recibieseis recibiesen")]
    [InlineData(
        "recibir", "10+15",
        "recibiere recibieres recibiere recibieremos recibiereis recibieren",
        "recibiere recibieres recibiere recibiéremos recibiereis recibieren")]
    [InlineData(
        "hablar", "9+12",
        "hablo hablas habla hablamos hablais hablan",
        "hablo hablas habla hablamos habláis hablan")]
    [InlineData(
        "hablar", "9+14",
        "hablaba hablabas hablaba hablabamos hablabais hablaban",
        "hablaba hablabas hablaba hablábamos hablabais hablaban")]
    [InlineData(
        "hablar", "9+13",
        "hable hablaste hablo hablamos hablasteis hablaron",
        "hablé hablaste habló hablamos hablasteis hablaron")]
    [InlineData(
        "hablar", "9+15",
        "hablare hablaras hablara hablaremos hablareis hablaran",
        "hablaré hablarás hablará hablaremos hablaréis hablarán")]
    [InlineData(
        "hablar", "9+16",
        "hablaría hablarías hablaría hablaríamos hablaríais hablarían",
        "hablaría hablarías hablaría hablaríamos hablaríais hablarían")]
    [InlineData("hablar", "11+22", "— habla hable hablemos hablad hablen", "— habla hable hablemos hablad hablen")]
    [InlineData("hablar", "11+23", "— hables hable hablemos hableis hablen", "— hables hable hablemos habléis hablen")]
    [InlineData(
        "hablar", "10+12",
        "hable hables hable hablemos hableis hablen",
        "hable hables hable hablemos habléis hablen")]
    [InlineData(
        "hablar", "10+14+24",
        "hablara hablaras hablara hablaramos hablarais hablaran",
        "hablara hablaras hablara habláramos hablarais hablaran")]
    [InlineData(
        "hablar", "10+14+25",
        "hablase hablases hablase hablasemos hablaseis hablasen",
        "hablase hablases hablase hablásemos hablaseis hablasen")]
    [InlineData(
        "hablar", "10+15",
        "hablare hablares hablare hablaremos hablareis hablaren",
        "hablare hablares hablare habláremos hablareis hablaren")]
    public void InflectionBookScan_RegularRow_MarksNothing(
        string headword, string values, string predicted, string actual)
    {
        LInflectionBook book = TInflectionBookCreate();
        long[] row = values.Split('+').Select(long.Parse).ToArray();
        long[][] columns = [[17, 20], [18, 20], [19, 20], [17, 21], [18, 21], [19, 21]];
        string[] predictions = predicted.Split(' ');
        string[] forms = actual.Split(' ');

        for (int column = 0; column < columns.Length; column++)
        {
            long[] codes = [.. row, .. columns[column]];
            if (forms[column] == "—")
            {
                Assert.Null(TInflectionBookScan(book, headword, codes, "x"));
                continue;
            }

            Assert.Equal(predictions[column], TInterfaceInflection.TInflectionBookResolve(book, headword, codes));
            Assert.Equal([], TInflectionBookScan(book, headword, codes, forms[column]));
        }
    }

    [Theory]
    [InlineData("vivir", "9+12+17+21", "vivimos", "vivimos", "")]
    [InlineData("vivir", "9+12+18+21", "vivis", "vivís", "")]
    [InlineData("vivir", "9+12+18+20", "vives", "vives", "")]
    [InlineData("vivir", "9+12+19+21", "viven", "viven", "")]
    [InlineData("vivir", "11+22+18+21", "vivid", "vivid", "")]
    [InlineData("vivir", "11+22+18+20", "vive", "vive", "")]
    [InlineData("tener", "9+13+17+20", "teni", "tuve", "1:2")]
    [InlineData("buscar", "9+13+17+20", "busce", "busqué", "")]
    [InlineData("leer", "9+13+19+20", "leio", "leyó", "")]
    [InlineData("comer", "9+12+18+21", "comeis", "coméis", "")]
    [InlineData("comer", "11+22+18+21", "comed", "comed", "")]
    [InlineData("pagar", "9+13+17+20", "page", "pagué", "")]
    [InlineData("cruzar", "9+13+17+20", "cruze", "crucé", "")]
    [InlineData("averiguar", "9+13+17+20", "averigue", "averigüé", "")]
    [InlineData("coger", "9+12+17+20", "cogo", "cojo", "")]
    [InlineData("construir", "9+12+17+20", "construo", "construyo", "7:1")]
    [InlineData("seguir", "9+12+17+20", "seguo", "sigo", "1:1")]
    public void InflectionBookScan_HandCheck_MarksStatedLetters(
        string headword, string cell, string predicted, string actual, string marks)
    {
        LInflectionBook book = TInflectionBookCreate();
        long[] codes = cell.Split('+').Select(long.Parse).ToArray();

        Assert.Equal(predicted, TInterfaceInflection.TInflectionBookResolve(book, headword, codes));
        IReadOnlyList<LInflectionMark>? scan = TInflectionBookScan(book, headword, codes, actual);
        Assert.NotNull(scan);
        Assert.Equal(marks, TInterfaceInflection.TInflectionMarkFormat(scan));
    }

    [Fact]
    public void InflectionBookScan_NoKind_ReturnsNull()
    {
        LInflectionBook book = TInflectionBookCreate();

        Assert.Null(TInflectionBookScan(book, "casa", [9, 12, 17, 20], "casa"));
        Assert.Null(TInterfaceInflection.TInflectionBookResolve(book, "casa", [9, 12, 17, 20]));
    }

    [Fact]
    public void InflectionBookScan_NoEnding_ReturnsNull()
    {
        LInflectionBook book = TInflectionBookCreate();

        Assert.Null(TInflectionBookScan(book, "hablar", [11, 22, 17, 20], "hablo"));
        Assert.Null(TInflectionBookScan(book, "hablar", [9, 12], "habl"));
        Assert.NotNull(TInflectionBookScan(book, "hablar", [20, 22, 18, 11], "habla"));
    }

    [Fact]
    public void InflectionBookResolve_KindLimitedRule_SkipsOtherKinds()
    {
        LInflectionBook book = TInflectionBookCreate();

        Assert.Equal("comeis", TInterfaceInflection.TInflectionBookResolve(book, "comer", [9, 12, 18, 21]));
        Assert.Equal("comed", TInterfaceInflection.TInflectionBookResolve(book, "comer", [11, 22, 18, 21]));
        Assert.Equal("vivis", TInterfaceInflection.TInflectionBookResolve(book, "vivir", [9, 12, 18, 21]));
        Assert.Equal("vivid", TInterfaceInflection.TInflectionBookResolve(book, "vivir", [11, 22, 18, 21]));
    }

    [Fact]
    public void InflectionBookDivide_RootMarker_AnswersTextAndRoot()
    {
        LInflectionBook book = TInflectionBookCreate();

        Assert.Equal(
            "hablo", TInterfaceInflection.TInflectionBookDivide(book, "hablar", [9, 12, 17, 20], out int? hablo));
        Assert.Equal(4, hablo);
        Assert.Equal(
            "tenes", TInterfaceInflection.TInflectionBookDivide(book, "tener", [9, 12, 18, 20], out int? tenes));
        Assert.Equal(3, tenes);
        Assert.Equal(
            "teni", TInterfaceInflection.TInflectionBookDivide(book, "tener", [9, 13, 17, 20], out int? teni));
        Assert.Equal(3, teni);
    }

    [Fact]
    public void InflectionBookDivide_NoRootMarker_AnswersNullRoot()
    {
        LInflectionBook book = TInterfaceInflection.TInflectionBookCreate(
            [new("a", "^(?<root>.+)(?<vowel>a)r$")],
            [TInterfaceInflection.TInflectionStemCreate(
                [9, 12],
                new Dictionary<string, string> { ["a"] = "${root}a+" },
                [TInterfaceInflection.TInflectionEndingCreate([17, 20], "s")])],
            [new("[·+]", "", null)],
            [],
            null,
            "test");

        string? divided = TInterfaceInflection.TInflectionBookDivide(book, "hablar", [9, 12, 17, 20], out int? root);

        Assert.Equal("hablas", divided);
        Assert.Null(root);
    }

    internal static IReadOnlyList<LInflectionMark>? TInflectionBookScan(
        LInflectionBook book, string headword, IReadOnlyList<long> codes, string actual) =>
        TInterfaceInflection.TInflectionBookResolve(book, headword, codes) is string predicted
            ? TInterfaceInflection.TInflectionDifferenceScan(book.LInflectionBookFolds, predicted, actual)
            : null;

    internal static LInflectionBook TInflectionBookCreate()
    {
        long[][] columns = [[17, 20], [18, 20], [19, 20], [17, 21], [18, 21], [19, 21]];
        (long[], string, string, string?[])[] rows =
        [
            ([9, 12], "${root}·a+", "${root}·e+", ["O", "s", "∅", "mos", "'is", "n"]),
            ([9, 14], "${root}·aba+", "${root}·ía+", ["(e)", "s", "∅", "mos", "'is", "n"]),
            ([9, 13], "${root}·a+", "${root}·i+", ["(i)", "ste", "_o", "mos", "steis", "(e)ron"]),
            ([9, 15], "${root}·${vowel}re+", "${root}·${vowel}re+", ["(e)", "_as", "_a", "mos", "'is", "_an"]),
            ([9, 16], "${root}·${vowel}ría+", "${root}·${vowel}ría+", ["(e)", "_as", "_a", "mos", "'is", "_an"]),
            ([11, 22], "${root}·e+", "${root}·a+", [null, "S*", "∅", "mos", "S*d", "n"]),
            ([11, 23], "${root}·e+", "${root}·a+", [null, "s", "∅", "mos", "'is", "n"]),
            ([10, 12], "${root}·e+", "${root}·a+", ["(e)", "s", "∅", "mos", "'is", "n"]),
            ([10, 14, 24], "${root}·ara+", "${root}·iera+", ["(e)", "s", "∅", "mos", "'is", "n"]),
            ([10, 14, 25], "${root}·ase+", "${root}·iese+", ["(e)", "s", "∅", "mos", "'is", "n"]),
            ([10, 15], "${root}·are+", "${root}·iere+", ["(e)", "s", "∅", "mos", "'is", "n"]),
        ];
        List<LInflectionStem> stems = [];
        foreach ((long[] values, string a, string other, string?[] endings) in rows)
        {
            List<LInflectionEnding> zipped = [];
            for (int column = 0; column < columns.Length; column++)
            {
                if (endings[column] is string ending)
                {
                    zipped.Add(TInterfaceInflection.TInflectionEndingCreate(columns[column], ending));
                }
            }

            stems.Add(TInterfaceInflection.TInflectionStemCreate(
                values, new Dictionary<string, string> { ["a"] = a, ["기타"] = other }, zipped));
        }

        List<LInflectionRule> rules =
        [
            new("·e\\+S\\*", "·a+", null),
            new("·a\\+S\\*", "·e+", null),
            new("·e\\+(?=mos$|'is$|d$)", "·i+", "ir"),
            new("[aeiou]\\+O", "+o", null),
            new("i\\+\\(i\\)", "i+", null),
            new("a\\+\\(i\\)", "+e", null),
            new("([aeouáéóú])\\+\\(e\\)", "$1+", null),
            new("\\(e\\)", "e", null),
            new("[aeouáéóú]\\+_", "+", null),
            new("_", "", null),
            new("'", "", null),
            new("∅", "", null),
            new("i([·+]+)i", "i$1", null),
            new("[·+]", "", null),
        ];
        List<LInflectionRule> folds =
        [
            new("[áà]", "a", null),
            new("[éè]", "e", null),
            new("[íì]", "i", null),
            new("[óò]", "o", null),
            new("[úùü]", "u", null),
            new("qu(?=[ei])", "c", null),
            new("z", "c", null),
            new("gu(?=[ei])", "g", null),
            new("j", "g", null),
            new("(?<=[aeiou])y(?=[aeiou])", "i", null),
        ];
        List<LInflectionKind> kinds =
        [
            new("a", "^(?<root>.+)(?<vowel>a)r$"),
            new("기타", "^(?<root>.+)(?<vowel>[ei])r$"),
            new("ir", "ir$"),
        ];

        return TInterfaceInflection.TInflectionBookCreate(kinds, stems, rules, folds, null, "test");
    }
}
