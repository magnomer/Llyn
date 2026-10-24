# Envoy

The user-question port.
It answers Conduct's questions as console dialogs over the window.
It stands apart from `Chamber`, which holds the window and its pieces.

## `BEnvoy`

The window's answer to Conduct's `CEnvoy`, which asks through sublevel boxes drawn over the window.
It answers confirms, failures, the leave question, a coinage name, the import dialog and the omission list.
The file, workspace, folder, media and markup reads go to the path prompt through `BEnvoyPathfinder`.
Only the press read is still unanswered, and it returns false.
It finds `ASParley` in the blueprint for its boxes.
A box needs the window, which `BEnvoyWindowAttach` gives it.
Without a window every show but the refusal falls back to a held notice, and every question gets no answer.
The envoy holds notice lines under a lock, since a fault may come from another thread.

## `BEnvoyParleyConsult`

Builds one box and asks the window to run it.
The box builds its own key rows from its buttons, so the caller passes no key table.
A question has Yes and No buttons with No as the safe answer, and any other box has OK.
The leave, coinage, import and omission boxes name their own buttons, and the last is the safe one.
It answers null when no window is attached or the window has already exited.

## `BEnvoyFaultShow`

Shows a fault box in the `alert` role with the fault sentence and the reason.
The log path is a path line between them when there is one.
The fault handler of the box's own run is the one passed in.
A box that cannot show holds the same lines as notices.

## `BEnvoyRefusalShow`

Shows the start refusal, the workspace when it is not empty, and the reason as an alert box with OK.
It holds no notice when the box cannot show, since the bootstrap prints the three lines itself.

## `BEnvoyRescueShow`

Shows the rescue sentence, then the backup path and the reason when each is given, in a `dialog` box.
A box that cannot show holds the same lines as notices.

## `BEnvoyThemeShow`

Shows a fixed English theme sentence and the reason in a `dialog` box.
The sentence is English because the catalog holds no key for it.
A box that cannot show holds the head, the sentence and the reason as one notice.

## `CEnvoyConfirm`

Asks in an alert box, or in a dialog box when a tally follows the question.
Only Yes answers true, and Escape answers No.

## `CEnvoyUnionConfirm`

Asks a dialog box with the dropped and the kept name around the arrow.
The arrow comes from the window's glyph set in use, so it follows a glyph choice.
Without a window or an arrow mark it falls back to the `→` character.

## `CEnvoyLeaveConfirm`

Asks with Save, Discard and Cancel, and Cancel is the safe answer.
Save answers true, Discard false and anything else null.

## `CEnvoyCoinageRead`

Asks for a name in a field, and the Create button is enabled only while the wording is valid.
It answers the typed text when Create is pressed, and otherwise null.

## `CEnvoyCustomsRead`

Shows the import dialog with an Import button on F2 and a Cancel button.
The Import button carries F2 and the legend `Legend.Import`, so the box has an F2 row.
That row is dim while Import is disabled.
Import starts enabled only if every row is ready.
It answers true only when Import is pressed.

## `BEnvoyWindowAttach`

Gives the envoy the window it draws its boxes on.
It also hands the path prompt the atelier and the file, folder and drive seams.
It hands over the console Capsule that the wardrobe shares too.
Until it is called the path prompt answers null.

## `BEnvoyNoticeAdd`

Holds one line until the console is restored.

## `BEnvoyNoticePrint`

Prints the held lines on stderr in order, then clears them.

## `CEnvoyFailureShow`

Shows the failure sentence in an alert box with OK.
With a ledger notice the box adds the notice's sentence, and its label and path when both exist.
A box that cannot show holds the same sentences after the `llyn: ` head, as strict mode prints them.

## `CEnvoyOmissionShow`

Shows the omissions in a box with a Close button.
A box that cannot show holds one line per omission, with its line number and text, after the same head.

## `BEnvoyPathfinder`

The path prompt starts in the folder it is given.
Without one it starts in the stored folder, and else in the workspace.
The first button reads Save for a save prompt, Choose for a folder prompt and Open for a file prompt.
Pressing that button closes the box.
With text in the field, the body acts as if Enter were pressed.
With an empty field, a folder prompt chooses the folder shown and a file prompt the lit file.
The box reopens until an answer is chosen.
Cancel and Escape answer null.
A save over an existing file asks `Pathfinder.Replace`, and a refusal reopens the prompt.

## `BEnvoyPathfinderSave`

Stores the folder of each accepted answer.
A chosen folder is stored as it stands, and a file answer stores the folder that holds it.
It rewrites only the folder field, and writes nothing when that folder is already stored.
A folder that cannot be written loses the folder silently, and the answer still returns.

## Stamp

Commit: `78fc0ebfff7c`
- `BEnvoy.cs` `abed4cce9dccea6f`
- `BEnvoyPathfinder.cs` `6aba81b72ded4fa2`
