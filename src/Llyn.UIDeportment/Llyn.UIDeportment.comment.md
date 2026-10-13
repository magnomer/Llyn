# Llyn.UIDeportment

Deportment is the code behind the veneer's pages.
It drives windows, controls and events, and holds no markup.
It sits directly above Conduct and compiles against Conduct and its own capsule alone.

## `TargetFramework`

Deportment drives the veneer's controls, so it builds for Windows.
It hosts the WebView2 player of `QScreenBrowser`, so it takes the Windows version WebView2 needs.

## `DisableTransitiveProjectReferences`

A name from ShellEngine, Application or Core fails the build, not only the border audit.

## `InternalsVisibleTo`

Only the Windows behaviour tests can load Deportment, so only they reach its internals.

## `ProjectReference`

Conduct is the only ring Deportment compiles against.
Deportment reaches the engine only through Conduct's atelier and gates.
`Llyn.Core.Windows` is not referenced, since playback runs through WPF and WebView2 controls held here.
The capsule project keeps window geometry and panel widths between runs.
Only Deportment names the capsule, so no layer below the cut sees GUI-only state.

## `PackageReference`

`QScreenBrowser` builds its WebView2 player in code, so that package stays in Deportment.
The veneer holds only the empty slot the player is put into.
SharpVectors renders flag and interface SVGs into WPF drawings, which WPF's decoders cannot read.
The icons are embedded in the veneer, so `QIcon` reads them by pack URI.

## Role rule

Every file in Deportment holds one role.
A file or type that would pass its limit is split by role.
Each role moves to its own sealed class in its own file, with a fresh one-word base registered in `ListObject.md`.
Code is never fitted to a file.
No lines, lambdas or ternaries are merged to save lines.
No member is placed where there is room rather than with its role.
A moved member keeps its name under its new owner's prefix and base.

## Area pattern

Every panel area moves to the veneer by the same steps, first proven on taxonomy and tenor.

- The veneer page gains an `x:Class`, and the view becomes an internal sealed partial `Q{X}` shaped like `QEstablishment`.
  It takes the page as its surface and pulls each part through `QContract.QContractFind`.
  The parent builds it from the page pulled by contract ID.
- A map the view used becomes an internal static on its Conduct owner, as in `CAtlas`.
  A map shared by every panel sits on `CPanel`.
- A choice the view made over controller answers becomes a Conduct verdict, such as `CPanelPressAllowed`.
- A controller that names WPF types splits in two.
  Its WPF half joins the driver, and its engine half stays a sealed controller.
  A driver never holds a vista, since only handles escape the Truth audit.
- Sealed-controller tests stay in `Llyn.Tests.Windows` while their controllers sit in a Windows project.
  Shape tests go to `Llyn.Tests.Conduct`.
- A file is renamed only once its Truth and Strict rows reach zero.
  The ratchet reads a renamed row as new.
  A big class is cleaned in place first and renamed last.
- A page nested in another page gets its own driver, which the parent driver builds.
  `QGuild` builds `QVita` and `QAutograph` this way, as `QDuplex` builds each `QWing`.
- An item model takes the chosen mark as a constructor parameter, so the mark has one kind of writer.
- A template dictionary whose class only forwarded clicks dissolves into the page's merged dictionaries.
  The driver subscribes its own handlers on each realized part, as `QCorpus` does for the transcript.
- A control is fed the C record a gate returns, with no Deportment copy of it.
- A rule a controller would compute over engine rows sinks below Deportment.
  `LReferenceClerk.LReferenceCitationFind` and `LEngineGlossRead` are examples.

## Gate holders

A panel's edit requests are built in ShellEngine, over the tenure the desk holds.
Each gate holder is a small `LQuill*` class over one tenure, and `LEasel` builds images and videos.
`CDeskDraft` exposes the holders as properties.
Each is null while no tenure is held, and most are null while the desk fills its controls.
A member takes ids and .NET values, and names no engine type in its parameters.
It reads the draft from its tenure without a flush, and builds the request inside.
It defers when the request it replaced was deferred, and sends otherwise.
Which member runs for a field is the caller's decision.
A driver hands a member its input and never builds a request.
A driver over a veneer page answers the undo and redo keys through `QChronicle.QChronicleIntroduce`.
A row's buttons each subscribe to their own gate, as `QAuthor` does for the four credit handles.
No handler reads an action from a part's name.

## Stamp

Commit: `c2f335c331fe`
- `Llyn.UIDeportment.csproj` `8a587084008458d3`
