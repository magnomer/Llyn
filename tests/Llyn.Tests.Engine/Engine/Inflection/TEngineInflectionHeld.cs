using System.Net;
using System.Net.Http;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineInflectionHeld
{
    private const string TInflectionHeldPage = """
        <table class="roa-inflection-table">
        <tbody><tr>
        <th colspan="3" class="roa-nonfinite-header"><span title="infinitivo">infinitive</span></th>
        <td colspan="5"><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a
            rel="mw:WikiLink" href="./vivir#Spanish" class="mw-selflink-fragment">vivir</a></span></td></tr>
        <tr>
        <th colspan="3" class="roa-nonfinite-header"><span title="gerundio">gerund</span></th>
        <td colspan="5"><span class="Latn form-of lang-es gerund-vivir-form-of origin-vivir" lang="es"><a
            rel="mw:WikiLink" href="./viviendo#Spanish" title="viviendo">viviendo</a></span></td></tr>
        <tr>
        <th rowspan="3" colspan="2" class="roa-nonfinite-header">
            <span title="participio (pasado)">past participle</span></th>
        <td colspan="2" class="roa-nonfinite-header"></td>
        <th colspan="2" class="roa-nonfinite-header"><span title="masculino">masculine</span></th>
        <th colspan="2" class="roa-nonfinite-header"><span title="femenino">feminine</span></th></tr>
        <tr>
        <th colspan="2" class="roa-nonfinite-header">singular</th>
        <td colspan="2"><span class="Latn form-of lang-es pp_ms-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivido#Spanish" title="vivido">vivido</a></span></td>
        <td colspan="2"><span class="Latn form-of lang-es pp_fs-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivida#Spanish" title="vivida">vivida</a></span></td></tr>
        <tr>
        <th colspan="2" class="roa-nonfinite-header">plural</th>
        <td colspan="2"><span class="Latn form-of lang-es pp_mp-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vividos#Spanish" title="vividos">vividos</a></span></td>
        <td colspan="2"><span class="Latn form-of lang-es pp_fp-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vividas#Spanish" title="vividas">vividas</a></span></td></tr>
        <tr>
        <th colspan="2" rowspan="2" class="roa-person-number-header"></th>
        <th colspan="3" class="roa-person-number-header">singular</th>
        <th colspan="3" class="roa-person-number-header">plural</th></tr>
        <tr>
        <th class="roa-person-number-header">1st person</th>
        <th class="roa-person-number-header">2nd person</th>
        <th class="roa-person-number-header">3rd person</th>
        <th class="roa-person-number-header">1st person</th>
        <th class="roa-person-number-header">2nd person</th>
        <th class="roa-person-number-header">3rd person</th></tr>
        <tr>
        <th rowspan="6" class="roa-indicative-left-rail"><span title="indicativo">indicative</span></th>
        <th class="roa-native-person-number-header"></th>
        <th class="roa-native-person-number-header">yo</th>
        <th class="roa-native-person-number-header">tú<br/>vos</th>
        <th class="roa-native-person-number-header">él/ella/ello<br/>usted</th>
        <th class="roa-native-person-number-header">nosotros<br/>nosotras</th>
        <th class="roa-native-person-number-header">vosotros<br/>vosotras</th>
        <th class="roa-native-person-number-header">ellos/ellas<br/>ustedes</th></tr>
        <tr>
        <th class="roa-finite-header"><span title="presente de indicativo">present</span></th>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivo#Spanish" title="vivo">vivo</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vives#Spanish" title="vives">vives</a></span><sup><sup>tú</sup></sup><br/>
            <span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivís#Spanish" title="vivís">vivís</a></span><sup><sup>vos</sup></sup></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vive#Spanish" title="vive">vive</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivimos#Spanish" title="vivimos">vivimos</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivís#Spanish" title="vivís">vivís</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./viven#Spanish" title="viven">viven</a></span></td></tr>
        <tr>
        <th class="roa-finite-header"><span title="pretérito imperfecto (copréterito)">imperfect</span></th>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivía#Spanish" title="vivía">vivía</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivías#Spanish" title="vivías">vivías</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivía#Spanish" title="vivía">vivía</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivíamos#Spanish" title="vivíamos">vivíamos</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivíais#Spanish" title="vivíais">vivíais</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivían#Spanish" title="vivían">vivían</a></span></td></tr>
        <tr>
        <th class="roa-finite-header"><span title="pretérito perfecto simple (pretérito indefinido)">preterite</span>
            </th>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./viví#Spanish" title="viví">viví</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./viviste#Spanish" title="viviste">viviste</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivió#Spanish" title="vivió">vivió</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivimos#Spanish" title="vivimos">vivimos</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivisteis#Spanish" title="vivisteis">vivisteis</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivieron#Spanish" title="vivieron">vivieron</a></span></td></tr>
        <tr>
        <th class="roa-finite-header"><span title="futuro simple (futuro imperfecto)">future</span></th>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./viviré#Spanish" title="viviré">viviré</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivirás#Spanish" title="vivirás">vivirás</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivirá#Spanish" title="vivirá">vivirá</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./viviremos#Spanish" title="viviremos">viviremos</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./viviréis#Spanish" title="viviréis">viviréis</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivirán#Spanish" title="vivirán">vivirán</a></span></td></tr>
        <tr>
        <th class="roa-finite-header">
            <span title="condicional simple (pospretérito de modo indicativo)">conditional</span></th>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./viviría#Spanish" title="viviría">viviría</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivirías#Spanish" title="vivirías">vivirías</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./viviría#Spanish" title="viviría">viviría</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./viviríamos#Spanish" title="viviríamos">viviríamos</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./viviríais#Spanish" title="viviríais">viviríais</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivirían#Spanish" title="vivirían">vivirían</a></span></td></tr>
        <tr>
        <th class="roa-person-number-header" style="height:.75em" colspan="8"></th></tr>
        <tr>
        <th rowspan="5" class="roa-subjunctive-left-rail"><span title="subjuntivo">subjunctive</span></th>
        <th class="roa-native-person-number-header"></th>
        <th class="roa-native-person-number-header">yo</th>
        <th class="roa-native-person-number-header">tú<br/>vos</th>
        <th class="roa-native-person-number-header">él/ella/ello<br/>usted</th>
        <th class="roa-native-person-number-header">nosotros<br/>nosotras</th>
        <th class="roa-native-person-number-header">vosotros<br/>vosotras</th>
        <th class="roa-native-person-number-header">ellos/ellas<br/>ustedes</th></tr>
        <tr>
        <th class="roa-finite-header"><span title="presente de subjuntivo">present</span></th>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./viva#Spanish" title="viva">viva</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivas#Spanish" title="vivas">vivas</a></span><sup><sup>tú</sup></sup><br/>
            <span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivás#Spanish" title="vivás">vivás</a></span><sup><sup>vos<sup class="roa-red-superscript">2</sup>
            </sup></sup></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./viva#Spanish" title="viva">viva</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivamos#Spanish" title="vivamos">vivamos</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./viváis#Spanish" title="viváis">viváis</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivan#Spanish" title="vivan">vivan</a></span></td></tr>
        <tr>
        <th class="roa-finite-header"><span title="pretérito imperfecto de subjuntivo">imperfect</span><br/>(ra)</th>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./viviera#Spanish" title="viviera">viviera</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivieras#Spanish" title="vivieras">vivieras</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./viviera#Spanish" title="viviera">viviera</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./viviéramos#Spanish" title="viviéramos">viviéramos</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivierais#Spanish" title="vivierais">vivierais</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivieran#Spanish" title="vivieran">vivieran</a></span></td></tr>
        <tr>
        <th class="roa-finite-header"><span title="pretérito imperfecto de subjuntivo">imperfect</span><br/>(se)</th>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./viviese#Spanish" title="viviese">viviese</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivieses#Spanish" title="vivieses">vivieses</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./viviese#Spanish" title="viviese">viviese</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./viviésemos#Spanish" title="viviésemos">viviésemos</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivieseis#Spanish" title="vivieseis">vivieseis</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./viviesen#Spanish" title="viviesen">viviesen</a></span></td></tr>
        <tr>
        <th class="roa-finite-header"><span title="futuro simple de subjuntivo (futuro de subjuntivo)">future</span>
            <sup class="roa-red-superscript">1</sup></th>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./viviere#Spanish" title="viviere">viviere</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivieres#Spanish" title="vivieres">vivieres</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./viviere#Spanish" title="viviere">viviere</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./viviéremos#Spanish" title="viviéremos">viviéremos</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./viviereis#Spanish" title="viviereis">viviereis</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivieren#Spanish" title="vivieren">vivieren</a></span></td></tr>
        <tr>
        <th class="roa-person-number-header" style="height:.75em" colspan="8"></th></tr>
        <tr>
        <th rowspan="6" class="roa-imperative-left-rail"><span title="imperativo">imperative</span></th>
        <th class="roa-native-person-number-header"></th>
        <th class="roa-native-person-number-header">—</th>
        <th class="roa-native-person-number-header">tú<br/>vos</th>
        <th class="roa-native-person-number-header">usted</th>
        <th class="roa-native-person-number-header">nosotros<br/>nosotras</th>
        <th class="roa-native-person-number-header">vosotros<br/>vosotras</th>
        <th class="roa-native-person-number-header">ustedes</th></tr>
        <tr>
        <th class="roa-finite-header"><span title="imperativo afirmativo">affirmative</span></th>
        <td></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vive#Spanish" title="vive">vive</a></span><sup><sup>tú</sup></sup><br/>
            <span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./viví#Spanish" title="viví">viví</a></span><sup><sup>vos</sup></sup></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./viva#Spanish" title="viva">viva</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivamos#Spanish" title="vivamos">vivamos</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivid#Spanish" title="vivid">vivid</a></span></td>
        <td><span class="Latn form-of lang-es verb-form-vivir-form-of origin-vivir" lang="es"><a rel="mw:WikiLink"
            href="./vivan#Spanish" title="vivan">vivan</a></span></td></tr>
        <tr>
        <th class="roa-finite-header"><span title="imperativo negativo">negative</span></th>
        <td></td>
        <td><span class="Latn" lang="es"><a rel="mw:WikiLink" href="./no#Spanish" title="no">no</a> <a
            rel="mw:WikiLink" href="./vivas#Spanish" title="vivas">vivas</a></span></td>
        <td><span class="Latn" lang="es"><a rel="mw:WikiLink" href="./no#Spanish" title="no">no</a> <a
            rel="mw:WikiLink" href="./viva#Spanish" title="viva">viva</a></span></td>
        <td><span class="Latn" lang="es"><a rel="mw:WikiLink" href="./no#Spanish" title="no">no</a> <a
            rel="mw:WikiLink" href="./vivamos#Spanish" title="vivamos">vivamos</a></span></td>
        <td><span class="Latn" lang="es"><a rel="mw:WikiLink" href="./no#Spanish" title="no">no</a> <a
            rel="mw:WikiLink" href="./viváis#Spanish" title="viváis">viváis</a></span></td>
        <td><span class="Latn" lang="es"><a rel="mw:WikiLink" href="./no#Spanish" title="no">no</a> <a
            rel="mw:WikiLink" href="./vivan#Spanish" title="vivan">vivan</a></span></td></tr>
        </tbody></table>
        """;

    private static readonly TimeSpan TInflectionHeldPatience = TimeSpan.FromSeconds(10);

    [Fact]
    public async Task InflectionStart_SavedEntryHeldUnchanged_FillsEveryCell()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        TaskCompletionSource gate = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TSourceHandler handler = new(TInflectionHeldPage, HttpStatusCode.OK, gate.Task);
        using LEngine engine = workspace.TWorkspaceEngineStart(new HttpClient(handler, false));
        engine.TEngineFrequencySave(false);
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "vivir",
            "Spanish",
            string.Empty,
            string.Empty,
            [TInterface.TCardDraftCreate(string.Empty, string.Empty, "to live", [], [], [], [], [], 1)],
            [],
            string.Empty,
            null,
            ["Verb"]));
        LDraft held = engine.TEngineDraftStart("library", entry.LEntryId);

        gate.SetResult();
        await TInflectionHeldSettle(engine, handler, entry.LEntryId);
        engine.TEngineSoundStart(entry.LEntryId);
        await TInflectionHeldSettle(engine, handler, entry.LEntryId);

        LParadigmView view = Assert.IsType<LParadigmView>(engine.TEngineInflectionRead(entry.LEntryId, false, true));
        IReadOnlyList<LParadigmForm> forms =
            [.. view.LParadigmViewExpanded.LParadigmTableLines.SelectMany(line => line.LParadigmLineForms)];
        Assert.All(forms, form => Assert.Null(form.LParadigmFormTip));
        Assert.Equal(64, forms.Count(form => form.LParadigmFormText.Length > 0));
        Assert.Equal(64, engine.TEntryInflectionRead(entry.LEntryId).Count);
        Assert.Equal(64, engine.TEngineDraftRead(held.LDraftId)?.LDraftContent.LEntryDraftInflections.Count);
    }

    private static async Task TInflectionHeldSettle(LEngine engine, TSourceHandler handler, long entryId)
    {
        DateTime deadline = DateTime.UtcNow + TInflectionHeldPatience;
        while (handler.TSourceHandlerCount == 0 || engine.TEngineInflectionCheck(entryId))
        {
            Assert.True(DateTime.UtcNow < deadline, "Waited for the fetch to settle.");
            await Task.Delay(20);
        }
    }
}
