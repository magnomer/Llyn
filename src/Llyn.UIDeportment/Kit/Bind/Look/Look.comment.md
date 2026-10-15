# Look

This folder maps bare facts to framework property values and applies visual state rows without XAML triggers.
It also owns generated item fills, their subscriptions and identity-based collection updates.

## `QLook`

A row holds only while every cue it names holds, as a multi-trigger did.
A null part means the control itself.
Cue and icon property changes run the state handler immediately.
The inherited reach property attaches empty load and unload handlers to elements entering an attached view.
Those handlers let the registered class handlers receive lifecycle events from otherwise bare elements.
An already loaded element is refined immediately when reach arrives.

The original local value is saved when the first claim takes a target property.
Claims are tracked separately for each owner, because a part can also carry rows of its own.
Releasing one owner reapplies the latest remaining claim.
Releasing the final claim restores and removes the saved original.
A renewed claim is removed and appended, keeping its precedence explicit.

Enable watchers are tracked separately from saved target values.
Being another control's target therefore does not prevent an element from gaining its own enable watcher.
Value-change descriptors are attached only while loaded and removed on unload.
A later load can attach them again.

Press, release and focus refreshes are deferred until input handlers have updated the control's flags.
Styles are gathered base first, so derived rows follow their base rows.
The last active row wins for each part and property.
A type name is used only when no mapped style was found in the chain.
Missing template parts fall back to logical children, then the first templated presenter.
The presenter lookup also asks a template selector, so selected item templates remain reachable.

An empty combo box is judged by its text rather than its item count.
An image without a source and a content control holding an empty string also receive `Empty`.
A text block or a text box without text receives `Empty` as well.
Null content receives `Mute`, keeping it distinct from an empty string.
A missing icon receives `Bare`, independently of content.

`QLookCheckedRefine` reverses a toggle only on a false verdict, restoring a refused click.
A true verdict leaves the current checked state untouched.
`QLookVisibleRead` collapses rather than hides, so the control takes no room.
`QLookEpithetRead` supplies one en space, keeping epithet separation consistent for its callers.
`QLookPromptApply` uses muted placeholder text only while the supplied text is empty.
A supplied ink affects real text, while absent ink clears its local foreground.

## `QLookCue`

Flags let one row require several states without separate trigger objects.
`QLookCueBase` is zero, so base rows require no active flag.
Framework flags and externally supplied cues share the same mask.

## `QLookValue`

Each value kind owns application and release, keeping the state handler free of a switch over value kinds.
Implicit conversions retain the supplied value's kind rather than treating all values as untyped settings.
Resource keys stay dynamic, so later resource changes can reach the target.
Relays bind to the styled owner, not the target part.

Ordinary release restores a saved binding, clears an unset value or sets the saved local value back.
Motion release instead stops the render-transform animation.
An already animated transform is left running, avoiding a restart during unrelated state refreshes.
A new animation clones the current transform before starting, avoiding mutation of the existing transform instance.

## `QLookSheet`

The common setter table combines its rows with input, control, sound and display rows in that order.
Later matching rows win on the same part and property.
Base rows supply template values, icons and animations without waiting for an interaction cue.

The card hinge's base transform turns the icon half a turn.
Checked overrides it with the identity transform and the unfolding tooltip.
The paradigm's checked rows follow hover, keeping the checked pill's background and outline authoritative.
The progress sweep comes from the contracted `Theme.Popup.ProgressBar.Sweep` resource.

## `QLookControl`

Catalog, choice, command, language, paradigm, playback and volume rows stay together by style.
Later checked and disabled rows can override earlier hover settings without changing the state handler.

## `QLookDisplay`

Chip links light their edge on hover and dim while pressed.
Empty frames, citations, usage details, contents numbers and media controls collapse instead of reserving blank space.
Base rows relay control surface values onto their template parts.
Contents rows use the supplied `Chosen` cue rather than deciding the current section themselves.

## `QLookInput`

Choice rows relay dropdown state to the button and popup, keeping those parts tied to their owner.
The popup's minimum width follows the choice box's actual width, so a wider option can widen the popup.
An empty `Theme.Input.Caret` field drops its minimum width and takes `Theme.Input.CaretMargin`.
Focus clears both again, so a field being typed in keeps its style's room.
Bin buttons use warning resources, while disabled rows replace their border and ink.
The search helper hides a bare mark or muted label and selects matching inset resources.

## `QLookSound`

Sound rows relay data context into command parameters and chip text, so those values need no separate item fill.
Pronunciation tools become visible and hittable on hover or focus.
The rebuild icon uses the contracted spin animation while `Pending` holds.
The two diwei choices send boolean parameters directly.
Marked and faded representative rows expose the order, while faded additionally dims the control.
An empty reflex label retains space through `Hidden`, unlike the collapsing edge and anchor rows.

## `QLookItem`

Repeated attachment to a list is ignored, preventing duplicate generator and lifecycle subscriptions.
Localization changes request full scans, because fills may have copied localized text into generated parts.
A recycled container drops its old item's property-change handler before attaching the new one.
Unload removes every rostered container's item subscription.

`QLookItemShow` pairs each draft with the first unclaimed row carrying its id, preserving repeated ids one-to-one.
An id is a number or a string compared ordinally, and both overloads share one body.
An unmatched draft is created, and an update may replace the paired object.
Rows are retained by identity while they match the next requested object.
Other keyed rows are removed, and the remaining requested objects are appended.
This preserves the matching head without inserting or moving controls.
A row whose key is null is skipped during removal, leaving non-engine rows in place.

Scans use container indices rather than item equality, keeping equal items' containers independent.
A full scan refills known containers as well as new ones.
An ordinary scan leaves a known container untouched when its item is unchanged.
Teardown removes subscriptions from containers absent from the live set, then merges the remaining live containers into the roster.

## Inline notes

### QLookSheet.cs: `QLookSheetState`

The order of the spread is load-bearing, since a later row wins on the same part and property.

## Stamp

Commit: `2f9a06b8eb95`
- `QLook.cs` `5175b5af4fad7616`
- `QLookControl.cs` `f8e1fe06320adcd7`
- `QLookCue.cs` `6b9a3fd8e550a325`
- `QLookDisplay.cs` `17249a7d68d91dee`
- `QLookInput.cs` `317591fa67165b39`
- `QLookItem.cs` `7b4cd30d0e67e04f`
- `QLookSheet.cs` `3a3b88410d4a3703`
- `QLookSound.cs` `369c1c12b487a6e6`
- `QLookValue.cs` `e4dda57669786dd7`
