# LShengfuSource.cs

## `public interface LShengfuSource`

The fetch port the engine reads a character's phonetic series through.
It says nothing about HTTP, and `LShengfuSourceHttp` in Infrastructure is its adapter.

## `Task<(LShengfu? LShengfuFound, bool LShengfuReached)> LShengfuSourceFind(`

The series of the character under that rule, or null when the source named none.
The second value tells whether the source answered at all, so a failed fetch is retried later.
