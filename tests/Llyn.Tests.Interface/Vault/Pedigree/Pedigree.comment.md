# Pedigree

This folder holds the in-memory Ancestor store a fake rig needs.

## `TVaultFakeAncestor`

An in-memory Ancestor store that keeps one map per entry, as a fake rig needs.
A save mints a new id for each box and rewrites each child id to match, as an archive would.
The five built-in relations are seeded and cannot be deleted.
A create returns the item already held under the trimmed name, so a name never repeats.
A listed relation or tongue counts the boxes that use it across every map.
A delete of a custom item clears it from every box and keeps the boxes.
It then reports whether a row went.

## Stamp

Commit: `aae5349dbb94`
- `TVaultFakeAncestor.cs` `c045bd98f192efe5`
