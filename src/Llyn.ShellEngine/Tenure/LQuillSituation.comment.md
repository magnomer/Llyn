# LQuillSituation.cs
Hash: `dacb4ab21213311a`

## `public sealed class LQuillSituation`

The typed edits of the scenario form on one tenure: its title, kind and description.
It is one of the per-subject quills, beside `LQuillReference` for the source form.
Its members keep the `LQuill` name base the other quills share.

## `private readonly LTenure _lQuillSituationTenure;`

The tenure every request is built for and handed to.

## `public LQuillSituation(LTenure tenure)`

Builds the edits over one tenure, which they never swap.

## `public void LQuillTitleSet(string text)`

Defers the typed title for the situation the draft holds.
Each field has its own request, so typing one field never rewrites another.

## `public void LQuillKindSet(string text)`

Defers the typed kind, as the title above.

## `public void LQuillDescriptionSet(string text)`

Defers the typed description, as the title above.

## `private LSituation? LQuillSituationRead()`

The situation the draft holds, read without a flush, so a pending keystroke keeps its delay.
Its id names the request's target, and its held value keeps an unknown field unknown.
With no situation held, the id stays zero and the clerk refuses the request.

## `private static LStateWritten LQuillWrittenRead(string text, LStateValue? held)`

An empty field whose held value is unknown stays unknown.
Any other text is written as it stands.
