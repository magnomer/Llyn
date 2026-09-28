# LQuotation.cs

## `public sealed class LQuotation`

The deportment of the corpus panel's quotation list: the panel state over the quotation vista and its rows.
The rows are the entries quoting the chosen Example, so it keeps a handle on the example vista too.
The quotation side never deletes, so its panel has no delete scope.
It prints and exports the quotation vista's chosen entry.
It is sealed, so its rows, labels, tickets and formats cross as Conduct shapes.
Its constructor and vista restore take engine types, so they stay internal.

## `public string LQuotationEmptyRead(string? dredge)`

The wording key of the empty quotation list, chosen by whether the dredge field holds any text.
An empty field means nothing quotes the Example, and typed text means nothing matched.
