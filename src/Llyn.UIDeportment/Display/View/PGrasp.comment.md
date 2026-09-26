# PGrasp.cs

## `public sealed class PGrasp : FrameworkElement`

A row of stars the user clicks to say how well they know an entry, half a star per step.
It draws itself, so there is no child element per star and no template to keep in step.
The step it holds is a count of half stars from zero to the limit, in the store's own unit.
The limit comes from the deportment through `PGraspLimit`, so the row names no Core constant.
The row only maps a click to a step and raises it, and never decides what the store holds.

## `public static readonly DependencyProperty PGraspLimitProperty`

The last step, set once by the host from its deportment before any step is shown.
It measures the row, since the star count is half of it.

## `public static readonly DependencyProperty PGraspStepProperty`

The shown step, never negative and clamped to the limit.
So no surface can set a value the store would refuse.

## `public static readonly DependencyProperty PGraspFillProperty`

The brush a committed star is filled with, read from the theme.

## `public static readonly DependencyProperty PGraspEmptyProperty`

The brush every star's outline is drawn with, read from the theme.

## `public static readonly DependencyProperty PGraspUnratedProperty`

The brush every star's outline is drawn with while no step stands and nothing hovers, read from the theme.

## `public static readonly DependencyProperty PGraspPreviewProperty`

The brush the hover preview is filled with, read from the theme.

## `public static readonly RoutedEvent PGraspChangedEvent`

Raised after a click or key changed the step, carrying nothing but the sender.
The listener reads `PGraspStep` and writes it through the engine.

## `public static readonly RoutedEvent PGraspHoveredEvent`

Raised when the pointer moves onto another half star or leaves the row.
The listener reads `PGraspPointed` and words the step beside the stars.

## `public PGrasp()`

Builds the `LGraspStar` that sizes, draws, hit-tests and steps the stars from this row's own properties, events and images.

## `public int PGraspPointed`

The half step to word beside the stars, read from `LGraspStar.LGraspPointed`.

## `protected override Size MeasureOverride(Size availableSize)`

The row's size comes from `LGraspStar.LGraspSizeResolve`.

## `protected override void OnMouseMove(MouseEventArgs e)`

Hands the pointer position to `LGraspStar.LGraspHoverHandle`.

## `protected override void OnMouseLeave(MouseEventArgs e)`

Hands the leave to `LGraspStar.LGraspLeaveHandle`.

## `protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)`

Hands the press to `LGraspStar.LGraspPressHandle` and marks the click handled.

## `protected override void OnKeyDown(KeyEventArgs e)`

Hands the key to `LGraspStar.LGraspKeyHandle`.

## `private static void PGraspLimitHandle(DependencyObject sender, DependencyPropertyChangedEventArgs e)`

A new limit clamps the step already held.
So a limit set after the step never leaves it out of range.
