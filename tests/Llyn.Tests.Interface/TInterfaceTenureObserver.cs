using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static void TTenureObserverAttach(this LTenure tenure, LSubject subject, Action<LBulletin> observer) =>
        tenure.LTenureObserverAttach(subject, observer);

    internal static void TTenureDraftAttach(this LTenure tenure, LSubject subject, Action<LBulletin> observer) =>
        tenure.LTenureDraftAttach(subject, observer);

    internal static void TTenureEntryAttach(this LTenure tenure, LSubject subject, Action<LBulletin> observer) =>
        tenure.LTenureEntryAttach(subject, observer);

    internal static LDraft? TTenurePrepare(this LTenure tenure, Action prepare) => tenure.LTenurePrepare(prepare);

    internal static void TEngineBulletinRaise(this LEngine engine, LSubject subject, long id) =>
        engine.LEngineBulletinRaise(subject, id);
}
