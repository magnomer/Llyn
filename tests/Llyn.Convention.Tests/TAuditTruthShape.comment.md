# TAuditTruthShape.cs

## `internal static partial class TAuditTruthWalker`

The shape half of the truth walker: what the shell's own shape may not decide.
A control's state, a clock and a deaf handler are shape, not a user act or an engine fact.

## `private static HashSet<string> TAuditControlNames = [];`

The `x:Name` identifiers the markup declares, read before the walk.

## `private static void TAuditShapeScan(SyntaxNode root, List<TViolation> violations)`

Walks every node of a file for the shapes and reports each once per line and name.
An `if` or ternary whose condition reads a control and whose branch requests lets the control decide.
An event subscribed on a clock whose handler requests lets the clock decide.
A method taking an `LBulletin` it never reads shows from shell state on a signal it did not look at.
An observer built on a lambda that ignores its bulletin is the same silence.

## `private static string? TAuditControlRead(ExpressionSyntax condition)`

The first control a condition reads a member of, or null.

## `private static bool TAuditClockCheck(ExpressionSyntax clock, SyntaxNode root)`

True when the subscribed identifier is declared with one of the clock types.

## `private static bool TAuditLambdaCheck(LambdaExpressionSyntax lambda)`

True for a lambda handed to a `PObserver` wrap whose parameter is discarded or never read.

## `private static bool TAuditDriveCheck(ExpressionSyntax handler)`

True when a timer handler requests inline or names a relay.

## `private static string? TAuditDeafRead(MethodDeclarationSyntax handler)`

The name of a bulletin parameter the body never mentions, or null.
