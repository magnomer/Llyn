# TInterfaceTenureObserver.cs

## `TTenureObserverAttach(this LTenure tenure, LSubject subject, Action<LBulletin> observer)`

Forwards a subject-filtered observer subscription to the tenure API.

## `TTenureDraftAttach(this LTenure tenure, LSubject subject, Action<LBulletin> observer)`

Forwards a draft-identity-filtered observer subscription to the tenure API.

## `TTenureEntryAttach(this LTenure tenure, LSubject subject, Action<LBulletin> observer)`

Forwards a stored-Entry-identity observer subscription to the tenure API.

## `TTenurePrepare(this LTenure tenure, Action prepare)`

Exposes the tenure's prepare scope to tests and returns its resulting draft.

## `TEngineBulletinRaise(this LEngine engine, LSubject subject, long id)`

Raises a bulletin through the engine so observer tests exercise normal event delivery.
