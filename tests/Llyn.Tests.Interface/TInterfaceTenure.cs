using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LTenure TEngineTenureStart(this LEngine engine, string origin, LSubject subject, long? id) =>
        engine.LEngineTenure.LEngineTenureStart(origin, subject, id);

    internal static LTenure TEngineOccurrenceStart(this LEngine engine, LVista vista, long? situation) =>
        engine.LEngineTenure.LEngineOccurrenceStart(vista, situation);

    internal static LTenure TEngineQuotationStart(this LEngine engine, LVista vista, long? example) =>
        engine.LEngineTenure.LEngineQuotationStart(vista, example);

    internal static LTenure TEngineFootnoteStart(this LEngine engine, LVista vista, long? reference) =>
        engine.LEngineTenure.LEngineFootnoteStart(vista, reference);

    internal static LTenure TEngineMembershipStart(this LEngine engine, LVista vista, long? tag) =>
        engine.LEngineTenure.LEngineMembershipStart(vista, tag);

    internal static LTenure TEngineCohortStart(this LEngine engine, LVista vista, long? register) =>
        engine.LEngineTenure.LEngineCohortStart(vista, register);

    internal static void TEngineDelaySet(this LEngine engine, int delay) =>
        engine.LEngineTenure.LEngineTenureDelay = delay;

    internal static LDraft? TTenureRead(this LTenure tenure) =>
        tenure.LTenureRead();

    internal static LTenureState TTenureStateRead(this LTenure tenure) =>
        tenure.LTenureStateRead();

    internal static void TTenureRequestDefer(this LTenure tenure, LRequest request)
    {
        tenure.LTenureRequestDefer(request);
    }

    internal static void TTenureRequestApply(this LTenure tenure, LRequest request)
    {
        tenure.LTenureRequestApply(request);
    }

    internal static void TTenurePersist(this LTenure tenure)
    {
        tenure.LTenurePersist();
    }

    internal static bool TTenureReadyCheck(this LTenure tenure) =>
        tenure.LTenureReadyCheck();

    internal static bool TTenureChangeCheck(this LTenure tenure) => tenure.LTenureChangeCheck();

    internal static bool TTenureStorableRead(this LTenure tenure) => tenure.LTenureStorable;

    internal static LDraft? TTenureUndo(this LTenure tenure) =>
        tenure.LTenureUndo();

    internal static LDraft? TTenureRedo(this LTenure tenure) =>
        tenure.LTenureRedo();

    internal static void TTenureSweep(this LTenure tenure)
    {
        tenure.LTenureSweep();
    }

    internal static void TTenureCancel(this LTenure tenure)
    {
        tenure.LTenureCancel();
    }

    internal static long? TTenureFinish(this LTenure tenure, bool store) =>
        tenure.LTenureFinish(store, static () => false);

    internal static void TTenureGlossInsert(this LTenure tenure, int position)
    {
        tenure.LTenureGlossInsert(0, 0, position);
    }

    internal static void TTenureHeadwordSet(this LTenure tenure, string text)
    {
        tenure.LTenureHeadwordSet(text);
    }

    internal static void TTenureNoteSet(this LTenure tenure, string text)
    {
        tenure.LTenureNoteSet(text);
    }

    internal static bool TTenureNoteCheck(string text, string note) => LTenure.LTenureNoteCheck(text, note);

    internal static void TTenureLanguageSet(this LTenure tenure, string language)
    {
        tenure.LTenureLanguageSet(language);
    }

    internal static LSpeechOffer TTenureSpeechSet(this LTenure tenure, string typed) => tenure.LTenureSpeechSet(typed);

    internal static LUnit TTenureUnitRead(this LTenure tenure) => tenure.LTenureUnitRead();

    internal static IReadOnlyList<LUnit> TTenureUnitScan(this LTenure tenure) => tenure.LTenureUnitScan();

    internal static void TTenureUnitSet(this LTenure tenure, LUnit unit)
    {
        tenure.LTenureUnitSet(unit);
    }

    internal static void TTenureIpaSet(this LTenure tenure, string text)
    {
        tenure.LTenureIpaSet(text);
    }

    internal static void TTenureRespellingSet(this LTenure tenure, string text)
    {
        tenure.LTenureRespellingSet(text);
    }
}
