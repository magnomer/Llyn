# CDraft.cs

## `public sealed record CDraft(string CDraftAuthorName);`

The held draft as a driver reads it when the desk announces it.
It carries only what a driver writes into its controls, so the full draft stays between controllers.

**Parameters**

- `CDraftAuthorName`: the name of the author the draft holds, empty for any other draft.
