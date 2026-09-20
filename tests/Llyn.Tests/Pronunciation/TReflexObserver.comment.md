# TReflexObserver.cs

## `internal sealed class TReflexObserver`

An observer that completes once on the bulletin a reflex test waits for.
Its handler is attached to the engine as a method group, since an observer is a delegate.
Made without a draft it waits for the reflex bulletin of any entry.
Made with a draft it waits for the draft bulletin carrying that id instead.
