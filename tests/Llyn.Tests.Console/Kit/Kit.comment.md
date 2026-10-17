# Kit

Tests of the console framework pieces that know no program, only the console.

## `TCanvasWidth`

Holds one sample per width class, as the plan's width tests list them.
`水` takes 2, `a` 1, a lone combining mark 0 and an emoji 2.
`ǐ` takes 1 under the narrow setting and 2 under the wide one.
A mixed text sums its clusters, so a combining mark joins its base and U+FE0F asks for 2.

## Stamp

Commit: `52c21d68b4c4`
- `TCanvasWidth.cs` `aade731ef24729a9`
