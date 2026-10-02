# TInterfaceTenureObserver.cs
Hash: `03c60c2e9c3720c1`

## `internal static void TTenureObserverAttach(this LTenure tenure, LSubject subject, Action<LBulletin> observer) =>`

Forwards a subject-filtered observer subscription to the tenure API.

## `internal static void TTenureDraftAttach(this LTenure tenure, LSubject subject, Action<LBulletin> observer) =>`

Forwards a draft-identity-filtered observer subscription to the tenure API.

## `internal static void TTenureEntryAttach(this LTenure tenure, LSubject subject, Action<LBulletin> observer) =>`

Forwards a stored-Entry-identity observer subscription to the tenure API.

## `internal static LDraft? TTenurePrepare(this LTenure tenure, Action prepare) =>`

Exposes the tenure's prepare scope to tests and returns its resulting draft.

## `internal static void TEngineBulletinRaise(this LEngine engine, LSubject subject, long id) =>`

Raises a bulletin through the engine so observer tests exercise normal event delivery.
