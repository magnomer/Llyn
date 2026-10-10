# TXieshengFold.cs
Hash: `d6c47197ce6d5ae6`

## `public sealed class TXieshengFold`

Covers the xiesheng gate that folds one series member, on a real workspace with fake pages.
It reuses the series pack and fetch helpers of `TXiesheng`.

## `public async Task XieshengFoldToggle_ShownMember_StoresItsOwnStateAndRedrawsThePage()`

Toggling the shown member opens it on the page and tells the driver to redraw.
The entry page's fold of the same entry stays closed.

## `public async Task XieshengChanged_EntryFoldNotice_StaysSilentWhileSeriesFoldRedraws()`

An entry page fold raises the plain Fold notice, which the series page does not hear.
A series member toggle raises its own notice, which redraws the page.

## `public async Task XieshengFoldToggle_NoSeriesShown_AnswersFalseAndStoresNothing()`

With no series page shown the gate answers false and writes no mark.

## `public async Task XieshengFoldToggle_FailingPort_ShowsTheNoticeAndAnswersFalse()`

A refusing port shows `Reflex.SpreadFailed` once, answers false and writes no mark.

## `public void StemMemberRead_UnfoldedLanguageRows_FoldableWhileABareMemberIsNot()`

A member whose rows are all in unfolded languages still folds on the series page.
A bare member has no reflex row and so no toggle.
Its member carries one unfolded guise per reflex row, as the engine hands them.
