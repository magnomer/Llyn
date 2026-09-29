# Llyn.UIDeportment.csproj

## `<TargetFramework>net10.0-windows10.0.17763.0</TargetFramework>`

Deportment drives the veneer's controls, so it builds for Windows.
It hosts the WebView2 player of `PScreen`, so it takes the Windows version WebView2 needs.

## `<UseWPF>true</UseWPF>`

Deportment names windows, controls and event arguments directly.
It holds no markup, since every page and dictionary lives in the veneer.

## `<InternalsVisibleTo Include="Llyn.Windows" />`

Only the Windows behaviour tests can load Deportment, so only they reach its internals.

## `<ProjectReference Include="..\Llyn.Conduct\Llyn.Conduct.csproj" />`

Conduct is the layer below, and ShellEngine, Application and Core arrive through it.
Deportment reaches the engine only through Conduct's atelier and its transitional port handles.
The engine handles a panel keeps, `LVista`, `LTenure` and `LForay`, are its whole reason to exist.
The ring guard lists the handles it may name, and every other engine type is out of reach.
`Llyn.Core.Windows` is not referenced, since playback runs through WPF and WebView2 controls held here.

## `<ProjectReference Include="..\Llyn.UIDeportment.Capsule\Llyn.UIDeportment.Capsule.csproj" />`

Deportment's own storage, which keeps the window geometry and panel widths between runs.
Only Deportment names it, so no layer below the cut sees GUI-only state.

## `<PackageReference Include="Microsoft.Web.WebView2" Version="1.0.4191.47" />`

`PScreen` builds its WebView2 player in code, so the package stays in Deportment by the first test of the purge.
The veneer holds only the empty slot the player is put into.

## `<PackageReference Include="SharpVectors.Wpf" Version="1.8.4.2" />`

Renders flag and interface SVGs into WPF drawings.
WPF's bitmap decoders can't read SVG on their own.
The icons it renders are embedded in the veneer, so `QIcon` reads them by pack URI.

## Role rule

Every file in Deportment holds one role.

- A file that would pass the line limit is split by role.
- A type that would pass the `Large` threshold is split the same way.
- Each role moves to its own sealed class in its own file.
- Each new class takes a fresh one-word base registered in `ListObject.md`.
- Code is never fitted to a file.
- No lines, lambdas or ternaries are merged to save lines, and no wrap is tightened.
- No member is placed where there is room rather than with its role.
- A moved member keeps its name under its new owner's prefix and base.

## Area pattern

Every panel area moves to the veneer by the same steps, first proven on taxonomy and tenor.

- The veneer page `P{X}.xaml` gains an `x:Class` and a shell copied from `PEstablishment.xaml.cs`.
- The parent swaps `<local:P{X}` for `<veneer:P{X}` and keeps the `x:Name`.
- The view `P{X} : UserControl` becomes `internal sealed partial class Q{X}`, shaped like `QEstablishment`.
- It takes the page as its surface and pulls each part through `QContract.QContractFind`.
- The pack URI load and the name scope copy go.
- The parent builds it as the window builds `QEstablishment`, from the page pulled by contract ID.
- Part files and `P…Item` models are renamed with the class.
  Its maps are internal statics on its Conduct owner, as in `CAtlas`.
  A map shared by every panel sits on `CPanel` in Conduct.
- A choice the view made over controller answers becomes a Conduct verdict, such as `CTaxonomyCoinageAllowed`.
- A controller that names WPF types splits in two, as `LLibrary` did.
  Its WPF half joins the driver, and its engine half stays a sealed controller.
  A driver never holds a vista, since only handles escape the Truth audit.
- Sealed-controller tests stay in `Llyn.Windows` until job44 and job45 sink their controllers.
  Shape tests go to `Llyn.Internal`.
- A file is renamed only in the job that brings its Truth and Strict rows to zero.
  The ratchet reads a renamed row as new.
- A big class is cleaned in place first and renamed last.
- A page nested in another page gets its own driver, which the parent driver builds.
  `QGuild` builds `QVita` and `QAutograph` this way, as `QDuplex` builds each `QWing`.
- An item model takes the chosen mark as a constructor parameter, so the mark has one kind of writer.
- A template dictionary whose class only forwarded clicks dissolves into the page's merged dictionaries.
  The driver subscribes its own handlers on each realized part, as `QCorpus` does for the transcript.
- A control is fed the C record a gate returns, with no Deportment copy of it.
  The excerpt hands `PMention` its `CMentionMark` rows as Conduct stored them.
- A rule a controller would compute over engine rows sinks below Deportment.
  `LReferenceClerk.LReferenceCitationFind` splits the citation rows in Application, and `LEngineGlossRead` picks the gloss language in the engine.

## Gate holders

A panel's edit requests are built in ShellEngine, over the tenure the desk holds.

- `LQuill` builds text rows and `LEasel` images and videos, one member per field.
  `LQuill` also takes the Author name the guild types, through `LQuillAuthorSet`.
  It also builds the corpus Example's text, language, citation, Gloss rows and Mentions.
- Which member runs for a field is the caller's decision.
- `CDesk` builds both when a tenure starts and exposes them as `CDeskQuill` and `CDeskEasel`.
  Each is null while no tenure is held or while the desk fills its controls.
- A member takes ids and .NET values, and names no engine type in its parameters.
- It reads the draft from its tenure without a flush, and builds the request and each written state inside.
- It defers when the request it replaced was deferred, and sends otherwise.
- A driver hands a member its input and never builds a request.
- A driver over a veneer page answers the undo and redo keys through `QChronicle.QChronicleIntroduce`.
- A row's buttons each subscribe to their own gate, as `QImprint` does for the four credit handles.
  No handler reads an action from a part's name.
