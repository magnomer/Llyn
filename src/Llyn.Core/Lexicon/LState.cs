namespace Llyn.Core;

/// <summary>
/// The three-state distinction a stored value can carry: whether it was never given, was
/// deliberately marked as not known, or holds a real value. Source metadata (job10) and any other
/// field that must tell "no value yet" apart from "known to be absent" use this.
/// </summary>
public enum LState
{
    /// <summary>No value has been supplied.</summary>
    LStateUnspecified,

    /// <summary>A value was deliberately marked as not known.</summary>
    LStateUnknown,

    /// <summary>A real value is present.</summary>
    LStateSpecified,
}
