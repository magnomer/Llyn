# Llyn.UIDeportment.csproj

## `<TargetFramework>net10.0-windows10.0.17763.0</TargetFramework>`

Deportment drives the veneer's controls, so it builds for Windows.
It hosts the WebView2 player of `PScreen`, so it takes the Windows version WebView2 needs.

## `<UseWPF>true</UseWPF>`

Deportment names windows, controls and event arguments directly.
It holds no markup, since every page and dictionary lives in the veneer.

## `<InternalsVisibleTo Include="Llyn.Windows" />`

Only the Windows behaviour tests can load Deportment, so only they reach its internals.

## `<InternalsVisibleTo Include="Llyn" />`

The host, built as `Llyn`, builds the window deportment over the ports it makes.
The constructor naming those ports stays internal, so no driver can build one.

## `<ProjectReference Include="..\Llyn.Conduct\Llyn.Conduct.csproj" />`

Conduct is the only reference, and ShellEngine, Application and Core arrive through it.
The engine handles a panel keeps, `LVista`, `LTenure` and `LForay`, are its whole reason to exist.
A deportment holds the engine through the `L*Port` slices it calls, never through `LEngine` itself.
The ring guard lists the ports and handles it may name, and every other engine type is out of reach.
`Llyn.Core.Windows` is not referenced, since playback runs through WPF and WebView2 controls held here.

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
- A new medium-free class in Deportment takes `Q` until job44 sinks it, as `QQuill` does.

## Area pattern

Every panel area moves to the veneer by the same steps, first proven on taxonomy and tenor.

- The veneer page `P{X}.xaml` gains an `x:Class` and a shell copied from `PEstablishment.xaml.cs`.
- The parent swaps `<local:P{X}` for `<veneer:P{X}` and keeps the `x:Name`.
- The view `P{X} : UserControl` becomes `internal sealed partial class Q{X}`, shaped like `QEstablishment`.
- It takes the page as its surface and pulls each part through `QContract.QContractFind`.
- The pack URI load and the name scope copy go.
- The parent builds it as the window builds `QEstablishment`, from the page pulled by contract ID.
- Part files and `P…Item` models are renamed with the class.
- The controller is sealed by the pattern of job05.
  Its maps are internal statics on the controller, as in `LCard`.
  A map shared by every panel sits on `LPanel`.
- A choice the view made over controller answers becomes a controller verdict, such as `LTaxonomyCoinageCheck`.
- A controller that names WPF types splits in two, as `LLibrary` and `LWing` did.
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
- A control fed a Conduct list takes Deportment rows instead, as the excerpt hands `PMention` its `PMentionMark` rows.
- A rule a controller would compute over engine rows sinks below Deportment.
  `CCitationRow.CCitationRowFind` splits the citation rows in Conduct, and `LEngineGlossRead` picks the gloss language in the engine.

## Gate holders

A panel's edit requests become gates, held by the desk the draft sits on.

- `QQuill` gates text rows and `QEasel` images and videos.
  `QQuill` also takes the Author name the guild types, through `QQuillAuthorSet`.
  It also gates the corpus Example's text, language, citation, Gloss rows and Mentions.
- A gate that changes one of several fields takes a Conduct field enum and branches with a `switch`.
  `QChord` joins them for sound rows in job23.
- `LDesk` builds each over itself and exposes it, as `LDeskQuill` and `LDeskEasel`.
- They take `Q` while they live here, and become `C` in job44, once they sink into Conduct.
- A gate takes ids, .NET values and Conduct shapes, and names no engine type in its public members.
- It reads the draft from its desk without a flush, and builds the request and each written state inside.
- It defers when the request it replaced was deferred, and sends otherwise.
- A driver hands a gate its input and never builds a request.
- A driver over a veneer page answers the undo and redo keys through `QChronicle.QChronicleAttach`.
- A row's buttons each subscribe to their own gate, as `QImprint` does for the four credit handles.
  No handler reads an action from a part's name.
