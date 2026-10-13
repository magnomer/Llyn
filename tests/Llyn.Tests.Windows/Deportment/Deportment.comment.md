# Deportment

Windows tests for the Deportment drivers and painters.
Each case that builds a WPF object runs on its own STA thread, since a WPF control demands one.
Cases that touch only plain notifying objects need no thread.

## `TBootstrapContext`

The bootstrap installs a dispatcher context on the startup thread.
An await on a task still running must resume on that thread once the context is installed.
Without it the continuation lands on the thread pool, which is how the first launch on an empty workspace crashed.
A dispatcher frame is pumped until the task ends, standing in for the run that follows the window build.

## `TCapsulePosture`

The window's GUI-only state is covered over Deportment's Capsule under a real workspace folder.
A reset drops every width and keeps each tab.
Unlinking raises the linked notice once, because only a change raises it.
A posture moved to another root reads and writes that root, and the first root keeps its own state.
A window stored at once reaches the file, and a minimized window stores nothing.

## `TDrawer`

The citation drawer is driven over a mirror of the product XAML.
Each mirror is copied verbatim from its source.
- `TDrawerSurface` copies `src/Llyn.UIVeneer/Theme/Base/PThemePopup.xaml:9-20`.
- `TDrawerPopup` copies `src/Llyn.UIVeneer/Editor/Core/PEditorPopup.xaml:9-54`.
- `TDrawerRow` copies `src/Llyn.UIVeneer/Panel/Corpus/PCorpusTranscript.xaml:112-126`.
- `TDrawerField` copies `src/Llyn.UIVeneer/Panel/Corpus/PCorpus.xaml:260-278`.

Long XAML lines are wrapped one attribute per line, which leaves the parsed tree the same.
The mirror departs from its source in two places.
- A 138-pixel minimum stands in for the field style.
  It is the field width the supervisor observed.
- The test host pins the drawer's `StaysOpen` to true.
  The test run has no focus owner, so an unpinned drawer closes on any lost mouse capture.
  Another process taking capture or activation then closed the drawer mid-test.
  The width check then found no presentation source, and the full run failed at random.
  The width under test does not depend on this setting.

## `TEtymologyFace`

The etymology field paints only the verdicts its owning driver hands it.
The editor's fixed verdicts show the row of links and keep the read narrative hidden over text.
The lectern's verdicts show the read narrative with words and hide a row with no link.
So a linked read side shows its row.

## `TIndexItem`

The rows of the Library and Duplex entry lists are built by the deportment.
A built row carries the chosen mark the engine row holds.
Two rows match when only the mark differs, and refuse when the language differs.
A sync raises the change only when the mark moved.

## `TInflectionSwitch`

The custom analysis switch on the Inflection settings page runs over a real ledger and a real workspace's settings.
The opened ledger lists the Inflection page right after Listing, with its localized title.
Its summary reads on, since custom analysis is on by default.
The refine paints the toggle from the ledger's read, so it shows the stored setting.
Each click hands the raw toggle to the gate, and no failure is shown.

## `TLocalizationChoice`

The settings language choice is painted from the ledger over a real workspace's settings.
A repeated code paint with a language other than the stored one moves the box selection.
No save runs and the stored language stays, so the code paint is never heard as the user's pick.
A heard paint would save the painted language and change the settings on every repaint.

## `TLookPart`

A driver finds a named part inside an item's template.
A chip list picks its templates through a selector, so the presenter's own template stays empty.
The part must still be found in the template the selector chose.
Without that, every edited chip showed an empty frame with no text and no eraser.

## `TVolumeCatalog`

The window's one volume owner paints a shared volume level on every tray.
The level is a plain value, so the tests need no WPF thread.
A set to the level already held raises no notice.
This silence keeps the trays and the display from notifying each other in a loop.

## Stamp

Commit: `c2f335c331fe`
- `TBootstrapContext.cs` `160359e5c04253c5`
- `TCapsulePosture.cs` `56b6ed906b2a3514`
- `TEtymologyFace.cs` `c9beed6df8dc3c9f`
- `TIndexItem.cs` `95431f069623230a`
- `TInflectionSwitch.cs` `2bc53daaa347571a`
- `TLocalizationChoice.cs` `3d142b2f15fb96b3`
- `TLookPart.cs` `acb778ddff8b2e79`
- `TVolumeCatalog.cs` `7d8119e688ca8282`
