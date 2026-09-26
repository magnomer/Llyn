# TAutographUnion.cs

## `public sealed class TAutographUnion`

Covers the edit area of the authors panel: the union list and the store.
The union list matches the typed name and leaves out the Author being written.
A confirmed union folds the written Author into the chosen one and reads the kept one.
A fresh draft refuses to store under a blank name, then stores once named and stands on the new Author.
