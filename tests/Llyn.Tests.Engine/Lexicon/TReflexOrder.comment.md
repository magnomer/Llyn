# TReflexOrder.cs
Hash: `8c81c82e194f8fb1`

## `public sealed class TReflexOrder`

Covers the order an Entry's reflex rows read back in.
Rows read back in the order the pack declares, never in the order they were stored.
The order rule puts declared languages and kinds first and the rest after by name.
Within one language and kind, the main row leads and the rest follow by text.
The Classical Chinese pack declares its order apart from its fetch rules.
The entry view, the editor draft, Livery, the portrait and the markup export all list one declared order.
The editor draft opened in that order counts as unchanged.

Saving the same rows in another order writes nothing and logs no reflex revision.
The stored positions stay as saved, so the declared order never rewrites them.
Two drafts holding the same rows in another order match, while a changed row does not.
