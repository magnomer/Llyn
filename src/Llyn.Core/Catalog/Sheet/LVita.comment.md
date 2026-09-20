# LVita.cs

## `public sealed record LVita(`

The read sheet of one Author: every text already decided, so the vita page writes and never branches.
The name falls back to the unnamed label when the stored name is blank, and the flag says so.
The work and citation sentences are composed here from the counts the catalog row carries.
The fellows and the citing places travel as rows for the veneer to copy.

## `public static LVita LVitaCreate(`

Composes the sheet from the catalog row, the fellows and the usages of one Author.
A missing row composes the sheet of nobody, so a fresh author draft still shows zero counts.

## `public static string LVitaWorkFormat(int count, Func<string, string> localize)`

The sentence for how many Sources credit an Author, one of three forms by count.
