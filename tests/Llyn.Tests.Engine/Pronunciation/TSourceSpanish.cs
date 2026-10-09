using System.Globalization;
using System.Net;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TSourceSpanish
{
    private const string TSourceSpanishBody = """
        <table class="roa-inflection-table">
        <tr><th class="roa-imperative-left-rail"><span title="imperativo afirmativo">Affirmative</span></th>
        <td rowspan="2"></td>
        <td><span lang="gl"><a href="./recibe">recibe</a></span></td>
        <td><span lang="gl"><a href="./reciba">reciba</a></span></td>
        <td><span lang="gl"><a href="./recibamos">recibamos</a></span></td>
        <td><span lang="gl"><a href="./recibide">recibide</a></span></td>
        <td><span lang="gl"><a href="./reciban">reciban</a></span></td></tr>
        <tr><th class="roa-imperative-left-rail"><span title="imperativo negativo">Negative</span></th>
        <td><span class="Latn" lang="gl"><a href="./non">non</a> <a href="./recibas">recibas</a></span></td>
        <td><span class="Latn" lang="gl"><a href="./non">non</a> <a href="./reciba">reciba</a></span></td>
        <td><span class="Latn" lang="gl"><a href="./non">non</a> <a href="./recibamos">recibamos</a></span></td>
        <td><span class="Latn" lang="gl"><a href="./non">non</a> <a href="./recibades">recibades</a></span></td>
        <td><span class="Latn" lang="gl"><a href="./non">non</a> <a href="./reciban">reciban</a></span></td></tr>
        </table>
        <table class="roa-inflection-table">
        <tr><th class="roa-finite-header"><span title="presente de indicativo">present</span></th>
        <td><span class="Latn lang-es"><a href="./recibo">recibo</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibes">recibes</a></span><sup><sup>tú</sup></sup><br/><span
            class="Latn lang-es"><a href="./recibís">recibís</a></span><sup><sup>vos</sup></sup></td>
        <td><span class="Latn lang-es"><a href="./recibe">recibe</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibimos">recibimos</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibís">recibís</a></span></td>
        <td><span class="Latn lang-es"><a href="./reciben">reciben</a></span></td></tr>
        <tr><th class="roa-finite-header"><span title="pretérito imperfecto (copréterito)">imperfect</span></th>
        <td><span class="Latn lang-es"><a href="./recibía">recibía</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibías">recibías</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibía">recibía</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibíamos">recibíamos</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibíais">recibíais</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibían">recibían</a></span></td></tr>
        <tr><th
            class="roa-finite-header">
            <span title="pretérito perfecto simple (pretérito indefinido)">preterite</span></th>
        <td><span class="Latn lang-es"><a href="./recibí">recibí</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibiste">recibiste</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibió">recibió</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibimos">recibimos</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibisteis">recibisteis</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibieron">recibieron</a></span></td></tr>
        <tr><th class="roa-finite-header"><span title="futuro simple (futuro imperfecto)">future</span></th>
        <td><span class="Latn lang-es"><a href="./recibiré">recibiré</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibirás">recibirás</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibirá">recibirá</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibiremos">recibiremos</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibiréis">recibiréis</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibirán">recibirán</a></span></td></tr>
        <tr><th
            class="roa-finite-header">
            <span title="condicional simple (pospretérito de modo indicativo)">conditional</span></th>
        <td><span class="Latn lang-es"><a href="./recibiría">recibiría</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibirías">recibirías</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibiría">recibiría</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibiríamos">recibiríamos</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibiríais">recibiríais</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibirían">recibirían</a></span></td></tr>
        <tr><th class="roa-finite-header"><span title="presente de subjuntivo">present</span></th>
        <td><span class="Latn lang-es"><a href="./reciba">reciba</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibas">recibas</a></span><sup><sup>tú</sup></sup><br/><span
            class="Latn lang-es"><a href="./recibás">recibás</a></span><sup><sup>vos</sup></sup></td>
        <td><span class="Latn lang-es"><a href="./reciba">reciba</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibamos">recibamos</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibáis">recibáis</a></span></td>
        <td><span class="Latn lang-es"><a href="./reciban">reciban</a></span></td></tr>
        <tr><th
            class="roa-finite-header"><span title="pretérito imperfecto de subjuntivo">imperfect</span><br/>(ra)</th>
        <td><span class="Latn lang-es"><a href="./recibiera">recibiera</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibieras">recibieras</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibiera">recibiera</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibiéramos">recibiéramos</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibierais">recibierais</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibieran">recibieran</a></span></td></tr>
        <tr><th
            class="roa-finite-header"><span title="pretérito imperfecto de subjuntivo">imperfect</span><br/>(se)</th>
        <td><span class="Latn lang-es"><a href="./recibiese">recibiese</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibieses">recibieses</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibiese">recibiese</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibiésemos">recibiésemos</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibieseis">recibieseis</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibiesen">recibiesen</a></span></td></tr>
        <tr><th
            class="roa-finite-header"><span title="futuro simple de subjuntivo (futuro de subjuntivo)">future</span><sup
            class="roa-red-superscript">1</sup></th>
        <td><span class="Latn lang-es"><a href="./recibiere">recibiere</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibieres">recibieres</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibiere">recibiere</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibiéremos">recibiéremos</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibiereis">recibiereis</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibieren">recibieren</a></span></td></tr>
        <tr><th class="roa-finite-header"><span title="imperativo afirmativo">affirmative</span></th>
        <td></td>
        <td><span class="Latn lang-es"><a href="./recibe">recibe</a></span><sup><sup>tú</sup></sup><br/><span
            class="Latn lang-es"><a href="./recibí">recibí</a></span><sup><sup>vos</sup></sup></td>
        <td><span class="Latn lang-es"><a href="./reciba">reciba</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibamos">recibamos</a></span></td>
        <td><span class="Latn lang-es"><a href="./recibid">recibid</a></span></td>
        <td><span class="Latn lang-es"><a href="./reciban">reciban</a></span></td></tr>
        <tr><th class="roa-finite-header"><span title="imperativo negativo">negative</span></th>
        <td></td>
        <td><span class="Latn" lang="es"><a href="./no">no</a> <a href="./recibas">recibas</a></span></td>
        <td><span class="Latn" lang="es"><a href="./no">no</a> <a href="./reciba">reciba</a></span></td>
        <td><span class="Latn" lang="es"><a href="./no">no</a> <a href="./recibamos">recibamos</a></span></td>
        <td><span class="Latn" lang="es"><a href="./no">no</a> <a href="./recibáis">recibáis</a></span></td>
        <td><span class="Latn" lang="es"><a href="./no">no</a> <a href="./reciban">reciban</a></span></td></tr>
        </table>
        """;

    private static readonly string[] TSourceSpanishForms =
    [
        "recibo", "recibes", "recibe", "recibimos", "recibís", "reciben",
        "recibía", "recibías", "recibía", "recibíamos", "recibíais", "recibían",
        "recibí", "recibiste", "recibió", "recibimos", "recibisteis", "recibieron",
        "recibiré", "recibirás", "recibirá", "recibiremos", "recibiréis", "recibirán",
        "recibiría", "recibirías", "recibiría", "recibiríamos", "recibiríais", "recibirían",
        "recibe", "reciba", "recibamos", "recibid", "reciban",
        "recibas", "reciba", "recibamos", "recibáis", "reciban",
        "reciba", "recibas", "reciba", "recibamos", "recibáis", "reciban",
        "recibiera", "recibieras", "recibiera", "recibiéramos", "recibierais", "recibieran",
        "recibiese", "recibieses", "recibiese", "recibiésemos", "recibieseis", "recibiesen",
        "recibiere", "recibieres", "recibiere", "recibiéremos", "recibiereis", "recibieren",
    ];

    [Fact]
    public async Task SourceFind_SpanishTable_ReturnsSixtyFourCellsKeyedByCode()
    {
        LParadigm verb = Assert.Single(
            TInterface.TSpeechPackLoad("Spanish").LSpeechPackParadigms, row => row.LParadigmSpeechCode == 6);
        IEnumerable<string> keys = verb.LParadigmCells.Select(
            cell => string.Join("+", cell.Order().Select(code => code.ToString(CultureInfo.InvariantCulture))));

        IReadOnlyList<LReading> readings = await TSourceSpanishRead();

        Assert.Equal(64, readings.Count);
        Assert.Equal(keys, readings.Select(reading => reading.LReadingVariety));
        Assert.Equal(TSourceSpanishForms, readings.Select(reading => reading.LReadingPhonetic));
    }

    [Fact]
    public async Task SourceFind_VosSecond_KeepsTheTuForm()
    {
        IReadOnlyList<LReading> readings = await TSourceSpanishRead();

        Assert.Equal(
            "recibes", Assert.Single(readings, reading => reading.LReadingVariety == "9+12+18+20").LReadingPhonetic);
        Assert.Equal(
            "recibas", Assert.Single(readings, reading => reading.LReadingVariety == "10+12+18+20").LReadingPhonetic);
        Assert.Equal(
            "recibe", Assert.Single(readings, reading => reading.LReadingVariety == "11+18+20+22").LReadingPhonetic);
        Assert.DoesNotContain(readings, reading => reading.LReadingPhonetic == "recibás");
    }

    [Fact]
    public async Task SourceFind_NegativeImperative_SkipsNo()
    {
        IReadOnlyList<LReading> readings = await TSourceSpanishRead();

        IEnumerable<LReading> negative = readings.Where(reading => reading.LReadingVariety.Split('+').Contains("23"));
        Assert.Equal(
            ["recibas", "reciba", "recibamos", "recibáis", "reciban"],
            negative.Select(reading => reading.LReadingPhonetic));
    }

    [Fact]
    public async Task SourceFind_DecoyTableFirst_ReadsSpanishOnly()
    {
        IReadOnlyList<LReading> readings = await TSourceSpanishRead();

        Assert.Equal(
            "recibid", Assert.Single(readings, reading => reading.LReadingVariety == "11+18+21+22").LReadingPhonetic);
        Assert.Equal(
            "recibáis", Assert.Single(readings, reading => reading.LReadingVariety == "11+18+21+23").LReadingPhonetic);
        Assert.DoesNotContain(readings, reading => reading.LReadingPhonetic is "recibide" or "recibades" or "non");
    }

    private static async Task<IReadOnlyList<LReading>> TSourceSpanishRead()
    {
        LSourceSpec spec = Assert.Single(TInterface.TLanguageLoad("Spanish").LLanguageMorphologies ?? []);
        LSource source = TInterfaceSource.TSourceGenericCreate(
            spec, TPronunciationHelper.TSourceClientCreate(TSourceSpanishBody, HttpStatusCode.OK));

        LAnswer answer = await source.TSourceFind("recibir", CancellationToken.None);

        return answer.LAnswerReadings;
    }
}
