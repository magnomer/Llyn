# PThemeGrasp.xaml

## `<SolidColorBrush x:Key="Theme.Grasp.Fill" ...>`

The fill of a star the user has earned, in the accent.
The heart beside it fills in its own rose, so the two marks are told apart at a glance.

## `<SolidColorBrush x:Key="Theme.Grasp.Empty" ...>`

The outline every star keeps once a step stands or a hover previews one.

## `<SolidColorBrush x:Key="Theme.Grasp.Unrated" ...>`

The outline every star keeps while the entry stands unrated and nothing hovers.
Muted like the label beside it, so an unjudged row does not call for attention.

## `<SolidColorBrush x:Key="Theme.Grasp.Preview" ...>`

The fill drawn while the pointer hovers, before a click commits: the accent at half strength.
Strong enough to read at a glance, yet plainly not the committed fill beside it.

## `<Style x:Key="Theme.Grasp.Mark" TargetType="local:PGrasp">`

The star row's gap after the heart and its centering, set once for both views.

## `<Style x:Key="Theme.Grasp.Label" TargetType="TextBlock">`

The words beside the stars saying what the shown step means.
Muted, so the stars stay the mark and the words stay the gloss.
