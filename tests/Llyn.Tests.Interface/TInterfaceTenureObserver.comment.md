# TInterfaceTenureObserver.cs
Hash: `67d3fb0c717acc3b`

## `internal static void TTenureObserverAttach(this LTenure tenure, LSubject subject, Action<LBulletin> observer)`

Forwards a subject-filtered observer subscription to the tenure API.

## `internal static void TTenureDraftAttach(this LTenure tenure, LSubject subject, Action<LBulletin> observer)`

Forwards a draft-identity-filtered observer subscription to the tenure API.

## `internal static void TTenureEntryAttach(this LTenure tenure, LSubject subject, Action<LBulletin> observer)`

Forwards a stored-Entry-identity observer subscription to the tenure API.

## `internal static LDraft? TTenurePrepare(this LTenure tenure, Action prepare)`

Exposes the tenure's prepare scope to tests and returns its resulting draft.

## `internal static long? TTenureStoredRead(this LTenure tenure)`

Exposes the tenure's stored-id read to tests.

## `internal static void TEngineBulletinRaise(this LEngine engine, LSubject subject, long id)`

Raises a bulletin through the engine so observer tests exercise normal event delivery.
