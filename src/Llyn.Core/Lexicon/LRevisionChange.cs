namespace Llyn.Core;

/// <summary>
/// One recorded change inside a <see cref="LRevision"/>. A change has no id of its own: its identity
/// is its revision plus <see cref="LRevisionChangePosition"/>, the order it takes within that
/// revision, so the changes of a revision read back in the sequence they were recorded.
/// <para>
/// <see cref="LRevisionChangeTarget"/> and <see cref="LRevisionChangeType"/> name what was touched —
/// the target row's id and the kind of entity it is — as plain recorded text rather than a foreign
/// key, because a change routinely describes a row that no longer exists. That is the point of the
/// history: a deletion is still readable after the deleted row is gone.
/// </para>
/// </summary>
/// <param name="LRevisionChangePosition">Zero-based order of this change within its revision.</param>
/// <param name="LRevisionChangeTarget">Id of the row the change touched; recorded text, not a reference.</param>
/// <param name="LRevisionChangeType">Entity type of the target, for example <c>entry</c> or <c>sense</c>.</param>
/// <param name="LRevisionChangeKind">What was done to the target, for example <c>create</c> or <c>delete</c>.</param>
/// <param name="LRevisionChangeSummary">Human-readable description of the change; <c>null</c> when none was given.</param>
public sealed record LRevisionChange(
    int LRevisionChangePosition,
    string LRevisionChangeTarget,
    string LRevisionChangeType,
    string LRevisionChangeKind,
    string? LRevisionChangeSummary);
