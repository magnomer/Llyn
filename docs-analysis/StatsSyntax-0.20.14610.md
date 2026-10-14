# Syntax statistics - 0.20.14610

- Generated: 2026-10-15 01:22:39 +09:00
- Source roots: src, tests
- Root-level files: none
- Project segments: 1
- Excluded directories: .git, .vs, artifacts, bin, node_modules, obj, packages, publish
- Excluded suffixes: .g.cs, .g.i.cs, .AssemblyInfo.cs, .GlobalUsings.g.cs, .Designer.cs
- Roslyn: 4.14.0-3.25262.10
- Language version: Preview
- Scanned: 2,086 files, 15 projects, 221,454 lines

## Summary

| Measure | Value |
|---|---:|
| Files | 2,086 |
| Lines | 221,454 |
| Nodes | 1,179,361 |
| Syntax kinds used | 197 |
| Features used | 101 of 138 |
| Minimum C# version | 14 |
| Parse errors | 0 |

## Families

| Family | Features | Used | Count | Per 1k lines |
|---|---:|---:|---:|---:|
| Types | 15 | 12 | 4,332 | 19.56 |
| Namespaces | 6 | 3 | 2,088 | 9.43 |
| Members | 29 | 21 | 10,281 | 46.42 |
| Patterns | 16 | 16 | 6,756 | 30.51 |
| Expressions | 36 | 29 | 28,837 | 130.22 |
| Statements | 20 | 14 | 13,664 | 61.70 |
| Generics | 6 | 2 | 76 | 0.34 |
| Async | 2 | 2 | 672 | 3.03 |
| Nullability | 3 | 2 | 4,331 | 19.56 |
| Unsafe | 2 | 0 | 0 | 0.00 |
| Directives | 3 | 0 | 0 | 0.00 |
| Total | 138 | 101 | 71,037 | 320.78 |

## Projects

| Project | Files | Lines | Nodes | Features used | Newest C# |
|---|---:|---:|---:|---:|---:|
| Llyn.Tests.Engine | 240 | 39,195 | 274,368 | 61 | 12 |
| Llyn.UIDeportment | 312 | 34,231 | 158,299 | 88 | 14 |
| Llyn.Tests.Conduct | 156 | 28,315 | 205,484 | 61 | 12 |
| Llyn.Infrastructure | 193 | 26,029 | 120,035 | 70 | 12 |
| Llyn.Application | 136 | 20,766 | 98,047 | 73 | 12 |
| Llyn.Conduct | 268 | 17,711 | 69,968 | 68 | 12 |
| Llyn.Tests.Convention | 115 | 17,623 | 85,435 | 79 | 12 |
| Llyn.Core | 388 | 13,045 | 53,385 | 60 | 12 |
| Llyn.ShellEngine | 101 | 10,475 | 42,139 | 58 | 12 |
| Llyn.Tests.Interface | 100 | 9,866 | 49,971 | 72 | 12 |
| Llyn.Tests.Windows | 26 | 2,801 | 17,304 | 47 | 12 |
| Llyn.Core.Windows | 7 | 798 | 3,140 | 37 | 10 |
| Llyn.UIVeneer | 38 | 416 | 824 | 4 | 10 |
| Llyn.UIDeportment.Capsule | 5 | 96 | 427 | 15 | 10 |
| Llyn.Host | 1 | 87 | 535 | 4 | 9 |
| Total | 2,086 | 221,454 | 1,179,361 | 101 | 14 |

## Versions

| C# version | Features | Used | Count |
|---|---:|---:|---:|
| 1 | 51 | 35 | 18,352 |
| 2 | 10 | 8 | 2,324 |
| 3 | 7 | 5 | 7,082 |
| 4 | 3 | 2 | 854 |
| 5 | 3 | 3 | 1,554 |
| 6 | 6 | 5 | 5,139 |
| 7 | 11 | 10 | 6,781 |
| 7.1 | 1 | 1 | 8 |
| 7.2 | 1 | 0 | 0 |
| 7.3 | 1 | 0 | 0 |
| 8 | 17 | 13 | 11,459 |
| 9 | 10 | 9 | 6,899 |
| 10 | 3 | 2 | 2,088 |
| 11 | 6 | 3 | 669 |
| 12 | 4 | 4 | 7,824 |
| 13 | 1 | 0 | 0 |
| 14 | 3 | 1 | 4 |

## Features

| Feature | Family | C# | Count | Per 1k lines | Files | First |
|---|---|---|---:|---:|---:|---|
| Collection expression | Expressions | 12 | 7,591 | 34.28 | 946 | src/Llyn.Application/Card/LCardClerk.cs:60 |
| using declaration | Statements | 8 | 5,751 | 25.97 | 444 | src/Llyn.Application/Card/LTranslationClerk.cs:160 |
| Lambda | Expressions | 3 | 4,906 | 22.15 | 772 | src/Llyn.Application/Card/LCardClerk.cs:95 |
| if | Statements | 1 | 4,327 | 19.54 | 728 | src/Llyn.Application/Card/LCardClerk.cs:73 |
| Nullable reference type T? | Nullability | 8 | 3,621 | 16.35 | 1,001 | src/Llyn.Application/Card/LCardClerkField.cs:91 |
| Expression-bodied member | Members | 6 | 2,810 | 12.69 | 524 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Target-typed new | Expressions | 9 | 2,400 | 10.84 | 583 | src/Llyn.Application/Card/LMentionClerk.cs:153 |
| File-scoped namespace | Namespaces | 10 | 2,085 | 9.42 | 2,085 | src/Llyn.Application/Card/LCardClerk.cs:6 |
| Readonly field | Members | 1 | 2,081 | 9.40 | 635 | src/Llyn.Application/Card/LCardClerk.cs:10 |
| Declaration pattern | Patterns | 7 | 2,020 | 9.12 | 408 | src/Llyn.Application/Card/LMeaningClerk.cs:105 |
| Tuple literal or tuple type | Expressions | 7 | 1,755 | 7.92 | 300 | src/Llyn.Application/Card/LMeaningClerk.cs:26 |
| Sealed class | Types | 1 | 1,652 | 7.46 | 1,537 | src/Llyn.Application/Card/LCardClerk.cs:8 |
| Conditional ?: | Expressions | 1 | 1,530 | 6.91 | 566 | src/Llyn.Application/Card/LCardClerk.cs:170 |
| foreach | Statements | 1 | 1,522 | 6.87 | 497 | src/Llyn.Application/Card/LCardClerk.cs:62 |
| Class | Types | 1 | 1,476 | 6.67 | 1,458 | src/Llyn.Application/Card/LCardClerk.cs:8 |
| Static lambda | Expressions | 9 | 1,409 | 6.36 | 338 | src/Llyn.Application/Card/LCardClerk.cs:95 |
| Constant pattern | Patterns | 7 | 1,221 | 5.51 | 216 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Full property | Members | 1 | 1,196 | 5.40 | 358 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Null-conditional ?. ?[ | Expressions | 6 | 1,126 | 5.08 | 400 | src/Llyn.Application/Card/LMentionClerk.cs:47 |
| Null-coalescing ?? | Expressions | 2 | 1,054 | 4.76 | 415 | src/Llyn.Application/Card/LCardClerkField.cs:74 |
| not pattern | Patterns | 9 | 953 | 4.30 | 393 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| Auto property | Members | 3 | 927 | 4.19 | 271 | src/Llyn.Application/Citation/LCitationClerk.cs:41 |
| Null-forgiving ! | Expressions | 8 | 903 | 4.08 | 332 | src/Llyn.Application/Draft/Clerk/Card/LDraftClerkCard.cs:124 |
| await | Expressions | 5 | 882 | 3.98 | 177 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:139 |
| with expression | Expressions | 9 | 879 | 3.97 | 284 | src/Llyn.Application/Card/LCardClerk.cs:115 |
| is null | Patterns | 7 | 875 | 3.95 | 380 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| Extension method | Members | 3 | 730 | 3.30 | 44 | tests/Llyn.Tests.Interface/Atelier/TInterfaceArea.cs:7 |
| Interpolated string | Expressions | 6 | 720 | 3.25 | 179 | src/Llyn.Application/Localization/LLocalizationReader.cs:55 |
| Nullable value type T? | Nullability | 2 | 710 | 3.21 | 304 | src/Llyn.Application/Card/LMeaningClerk.cs:104 |
| Raw string | Expressions | 11 | 650 | 2.94 | 149 | src/Llyn.Infrastructure/Database/Base/LDatabase.cs:51 |
| Async method | Async | 5 | 617 | 2.79 | 177 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:91 |
| Const field | Members | 1 | 606 | 2.74 | 256 | src/Llyn.Application/Card/LMentionClerk.cs:11 |
| Constructor | Members | 1 | 600 | 2.71 | 596 | src/Llyn.Application/Card/LCardClerk.cs:21 |
| Record class | Types | 9 | 571 | 2.58 | 473 | src/Llyn.Application/Draft/Row/LDraftClerkAncestor.cs:263 |
| Cast expression | Expressions | 1 | 540 | 2.44 | 174 | src/Llyn.Application/Markup/LMarkupClerkLink.cs:228 |
| Property pattern | Patterns | 8 | 511 | 2.31 | 135 | src/Llyn.Application/Draft/Clerk/LDraftClerk.cs:83 |
| Object initializer | Expressions | 3 | 501 | 2.26 | 211 | src/Llyn.Application/Portrait/LPortraitClerkLabel.cs:25 |
| Optional parameter | Members | 4 | 492 | 2.22 | 163 | src/Llyn.Application/Draft/Clerk/LDraftClerkPanel.cs:170 |
| nameof | Expressions | 6 | 409 | 1.85 | 143 | src/Llyn.Application/Citation/LAuthorClerk.cs:361 |
| catch | Statements | 1 | 389 | 1.76 | 167 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:142 |
| lock | Statements | 1 | 383 | 1.73 | 61 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:98 |
| Deconstruction | Expressions | 7 | 366 | 1.65 | 158 | src/Llyn.Application/Card/LMeaningClerk.cs:46 |
| Named argument | Members | 4 | 362 | 1.63 | 104 | src/Llyn.Application/Card/LTranslationClerk.cs:164 |
| and / or pattern | Patterns | 9 | 357 | 1.61 | 108 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| try | Statements | 1 | 355 | 1.60 | 184 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:137 |
| Static class | Types | 2 | 352 | 1.59 | 352 | src/Llyn.Application/Card/LCardClerkField.cs:7 |
| for | Statements | 1 | 293 | 1.32 | 185 | src/Llyn.Application/Card/LCardClerk.cs:280 |
| Init accessor | Members | 9 | 229 | 1.03 | 80 | src/Llyn.Conduct/Lexicon/CExample.cs:20 |
| throw expression | Expressions | 7 | 229 | 1.03 | 110 | src/Llyn.Application/Card/LCardClerkField.cs:75 |
| Spread element | Expressions | 12 | 226 | 1.02 | 126 | src/Llyn.Application/Card/LCardClerk.cs:95 |
| Discard pattern | Patterns | 8 | 216 | 0.98 | 127 | src/Llyn.Application/Citation/LAuthorClerk.cs:243 |
| while | Statements | 1 | 215 | 0.97 | 130 | src/Llyn.Application/Card/LMeaningClerk.cs:44 |
| Switch expression | Patterns | 8 | 213 | 0.96 | 128 | src/Llyn.Application/Citation/LAuthorClerk.cs:239 |
| typeof | Expressions | 1 | 195 | 0.88 | 43 | src/Llyn.Infrastructure/Configuration/LLocalizationLoader.cs:20 |
| using statement | Statements | 1 | 188 | 0.85 | 79 | src/Llyn.Application/Citation/LAuthorCitation.cs:51 |
| Field-like event | Members | 1 | 163 | 0.74 | 97 | src/Llyn.Conduct/Atelier/CWorkspace.cs:36 |
| Discard _ | Expressions | 7 | 152 | 0.69 | 58 | src/Llyn.Application/Draft/Clerk/Card/LDraftClerkCard.cs:127 |
| Case guard when | Patterns | 7 | 116 | 0.52 | 25 | src/Llyn.Application/Citation/LAuthorClerk.cs:241 |
| Interface | Types | 1 | 111 | 0.50 | 111 | src/Llyn.Application/Pronunciation/Relay/LListener.cs:5 |
| Index from end ^ | Expressions | 8 | 95 | 0.43 | 50 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:230 |
| Verbatim string | Expressions | 1 | 93 | 0.42 | 20 | src/Llyn.Application/Localization/LLocalizationReader.cs:13 |
| Range .. | Expressions | 8 | 92 | 0.42 | 49 | src/Llyn.Application/Card/LMentionClerk.cs:159 |
| Relational pattern | Patterns | 9 | 90 | 0.41 | 31 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| as cast | Expressions | 1 | 85 | 0.38 | 43 | src/Llyn.Infrastructure/Database/Base/Workspace/LWorkspaceArchive.cs:132 |
| Partial type | Types | 2 | 76 | 0.34 | 76 | src/Llyn.UIVeneer/Display/View/PDisplay.xaml.cs:5 |
| Exception filter | Statements | 6 | 74 | 0.33 | 47 | src/Llyn.Application/Outpost/LCourierClerk.cs:196 |
| is type test | Patterns | 1 | 60 | 0.27 | 37 | src/Llyn.Application/Outpost/LCourierClerk.cs:288 |
| Switch statement | Patterns | 1 | 60 | 0.27 | 37 | src/Llyn.Application/Portrait/LPortraitClerkPress.cs:59 |
| Generic method | Generics | 2 | 58 | 0.26 | 30 | src/Llyn.Application/Card/LCardClerkField.cs:183 |
| Async lambda | Async | 5 | 55 | 0.25 | 10 | src/Llyn.Application/Outpost/LCourierClerk.cs:251 |
| yield return / break | Statements | 2 | 52 | 0.23 | 21 | src/Llyn.Application/Card/LCardClerkField.cs:35 |
| Local const | Statements | 1 | 51 | 0.23 | 27 | src/Llyn.Application/Markup/LMarkupClerkIntake.cs:271 |
| finally | Statements | 1 | 50 | 0.23 | 35 | src/Llyn.Application/Outpost/LCourierClerk.cs:76 |
| Enum | Types | 1 | 47 | 0.21 | 47 | src/Llyn.Conduct/Catalog/Vista/CCatalogOrder.cs:3 |
| Nested type | Types | 1 | 32 | 0.14 | 19 | src/Llyn.Application/Draft/Row/LDraftClerkAncestor.cs:263 |
| var pattern | Patterns | 7 | 30 | 0.14 | 12 | src/Llyn.Conduct/Card/CFolio.cs:219 |
| ref or out parameter | Members | 1 | 29 | 0.13 | 24 | src/Llyn.Application/Draft/Clerk/Card/LDraftClerkCard.cs:162 |
| Null-coalescing assignment ??= | Expressions | 8 | 21 | 0.09 | 19 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:160 |
| Collection initializer | Expressions | 3 | 18 | 0.08 | 16 | src/Llyn.Application/Outpost/LCourierClerk.cs:177 |
| Constraint clause | Generics | 2 | 18 | 0.08 | 10 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:171 |
| Local function | Members | 7 | 17 | 0.08 | 11 | src/Llyn.Conduct/Desk/CErrand.cs:56 |
| await using | Statements | 8 | 14 | 0.06 | 7 | src/Llyn.Conduct/Portrait/CPortrait.cs:144 |
| List pattern | Patterns | 11 | 14 | 0.06 | 12 | src/Llyn.ShellEngine/Engine/Facade/LDraftFacade.cs:70 |
| Conversion operator | Members | 1 | 13 | 0.06 | 2 | src/Llyn.Core/Lexicon/State/LStateValue.cs:49 |
| Default interface method body | Members | 8 | 11 | 0.05 | 6 | src/Llyn.ShellEngine/Port/Entry/LEntryPort.cs:21 |
| Type pattern | Patterns | 9 | 11 | 0.05 | 4 | src/Llyn.UIDeportment/Sound/Accent/QAccentControl.cs:62 |
| Positional pattern | Patterns | 8 | 9 | 0.04 | 9 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:37 |
| default literal | Expressions | 7.1 | 8 | 0.04 | 6 | src/Llyn.Infrastructure/Pronunciation/Loader/LSourceLoader.cs:73 |
| Primary constructor on class or struct | Types | 12 | 6 | 0.03 | 6 | src/Llyn.Conduct/Dialog/Galley/CSGalleyDelay.cs:7 |
| Required member | Members | 11 | 5 | 0.02 | 3 | src/Llyn.UIDeportment/Panel/Taxonomy/QMembershipItem.cs:27 |
| field keyword in accessor | Members | 14 | 4 | 0.02 | 1 | src/Llyn.UIDeportment/Panel/Dialog/Galley/QSGalleyItem.cs:31 |
| Generic type | Types | 2 | 4 | 0.02 | 4 | src/Llyn.Conduct/Configuration/CEnsignSheet.cs:5 |
| Record struct | Types | 10 | 3 | 0.01 | 3 | src/Llyn.Core/Lexicon/Inflection/LInflectionLetter.cs:3 |
| Abstract class | Types | 1 | 2 | 0.01 | 2 | src/Llyn.Application/Request/Entry/LRequest.cs:3 |
| checked / unchecked expression | Expressions | 1 | 2 | 0.01 | 2 | src/Llyn.Core.Windows/LPressBrowser.cs:14 |
| Static constructor | Members | 1 | 2 | 0.01 | 2 | src/Llyn.UIDeportment/Kit/Bind/Look/QLookItem.cs:23 |
| Static local function | Members | 8 | 2 | 0.01 | 1 | src/Llyn.UIDeportment/Kit/Asset/QIcon.cs:144 |
| Using alias | Namespaces | 1 | 2 | 0.01 | 2 | src/Llyn.Host/LHost.cs:11 |
| Alias of any type | Namespaces | 12 | 1 | 0.00 | 1 | tests/Llyn.Tests.Interface/Entry/TInterfaceInflection.cs:2 |
| Event accessors | Members | 1 | 1 | 0.00 | 1 | src/Llyn.UIDeportment/Mention/Text/PMention.cs:46 |
| Indexer | Members | 1 | 1 | 0.00 | 1 | src/Llyn.UIDeportment/Kit/Asset/QLocalizationCatalog.cs:19 |

## Styles

| Style | A | A count | B | B count | A share |
|---|---|---:|---|---:|---:|
| Null check | is null / is not null | 875 | == null / != null | 0 | 100.00 % |
| Object creation | Target-typed new() | 2,400 | new T() | 4,276 | 35.95 % |
| Collection creation | Collection expression | 7,591 | Collection initializer or array creation | 73 | 99.05 % |
| Member body | Expression body | 2,810 | Block body | 10,924 | 20.46 % |
| Local type | var | 46 | Explicit type | 23,134 | 0.20 % |
| Using | Declaration | 5,751 | Statement | 188 | 96.83 % |
| Namespace | File-scoped | 2,085 | Block | 0 | 100.00 % |
| Text building | Interpolated string | 720 | string.Format or + with a string literal | 589 | 55.00 % |
| Branch | Switch expression | 213 | Switch statement | 60 | 78.02 % |
| Lambda body | Expression | 4,325 | Block | 581 | 88.16 % |
| Type test | is pattern with designation | 1,169 | as | 85 | 93.22 % |
| Constructor | Primary | 6 | Explicit | 596 | 1.00 % |

## Features unused (37)

| Feature | Family | C# |
|---|---|---|
| Struct | Types | 1 |
| Delegate | Types | 1 |
| File-local type | Types | 11 |
| Block namespace | Namespaces | 1 |
| Global using | Namespaces | 10 |
| Using static | Namespaces | 6 |
| Readonly struct member | Members | 8 |
| Operator overload | Members | 1 |
| Extension block | Members | 14 |
| Static abstract interface member | Members | 11 |
| Finalizer | Members | 1 |
| Params collection, not array | Members | 13 |
| in parameter | Members | 7.2 |
| ref return or ref local | Members | 7 |
| Anonymous method | Expressions | 2 |
| Null-conditional assignment | Expressions | 14 |
| UTF-8 literal u8 | Expressions | 11 |
| sizeof | Expressions | 1 |
| stackalloc | Expressions | 1 |
| Anonymous type | Expressions | 3 |
| LINQ query expression | Expressions | 3 |
| do | Statements | 1 |
| await foreach | Statements | 8 |
| goto | Statements | 1 |
| fixed | Statements | 1 |
| unsafe block | Statements | 1 |
| checked / unchecked block | Statements | 1 |
| notnull constraint | Generics | 8 |
| unmanaged constraint | Generics | 7.3 |
| Variance in / out | Generics | 4 |
| default(T) expression | Generics | 2 |
| #nullable directive | Nullability | 8 |
| Pointer type | Unsafe | 1 |
| Function pointer | Unsafe | 9 |
| #if | Directives | 1 |
| #region | Directives | 1 |
| #pragma | Directives | 1 |

## Syntax kinds

| Kind | Count | Files |
|---|---:|---:|
| IdentifierName | 340,865 | 2,086 |
| Argument | 122,893 | 1,576 |
| SimpleMemberAccessExpression | 112,574 | 1,561 |
| ArgumentList | 76,205 | 1,614 |
| InvocationExpression | 69,690 | 1,572 |
| PredefinedType | 33,410 | 1,935 |
| ExpressionStatement | 33,125 | 1,454 |
| StringLiteralExpression | 27,493 | 1,160 |
| Parameter | 24,397 | 1,872 |
| VariableDeclaration | 23,887 | 1,373 |
| VariableDeclarator | 23,887 | 1,373 |
| EqualsValueClause | 22,770 | 1,433 |
| LocalDeclarationStatement | 20,173 | 1,193 |
| Block | 19,343 | 1,536 |
| ParameterList | 14,702 | 2,000 |
| MethodDeclaration | 12,446 | 1,609 |
| NumericLiteralExpression | 12,417 | 1,157 |
| GenericName | 11,427 | 1,446 |
| TypeArgumentList | 11,427 | 1,446 |
| QualifiedName | 9,893 | 2,086 |
| ExpressionElement | 8,318 | 523 |
| SimpleAssignmentExpression | 8,050 | 1,080 |
| CollectionExpression | 7,591 | 946 |
| UsingDirective | 6,703 | 1,785 |
| ReturnStatement | 6,278 | 1,140 |
| NullLiteralExpression | 4,557 | 951 |
| NullableType | 4,331 | 1,079 |
| IfStatement | 4,327 | 728 |
| ObjectCreationExpression | 4,276 | 938 |
| SimpleLambdaExpression | 3,861 | 695 |
| Attribute | 3,823 | 453 |
| AttributeList | 3,823 | 453 |
| SingleVariableDesignation | 3,480 | 601 |
| BracketedArgumentList | 3,402 | 592 |
| FieldDeclaration | 3,079 | 801 |
| IsPatternExpression | 2,907 | 621 |
| ArrowExpressionClause | 2,812 | 525 |
| ElementAccessExpression | 2,783 | 542 |
| ImplicitObjectCreationExpression | 2,400 | 583 |
| FalseLiteralExpression | 2,144 | 582 |
| PropertyDeclaration | 2,129 | 525 |
| ConstantPattern | 2,096 | 482 |
| CompilationUnit | 2,086 | 2,086 |
| FileScopedNamespaceDeclaration | 2,085 | 2,085 |
| DeclarationPattern | 2,020 | 408 |
| TrueLiteralExpression | 1,842 | 540 |
| EqualsExpression | 1,747 | 623 |
| TupleExpression | 1,729 | 295 |
| AddExpression | 1,712 | 383 |
| InterpolatedStringText | 1,626 | 179 |
| ConditionalExpression | 1,530 | 566 |
| LogicalAndExpression | 1,480 | 428 |
| ClassDeclaration | 1,476 | 1,458 |
| ForEachStatement | 1,433 | 479 |
| Interpolation | 1,426 | 178 |
| LogicalNotExpression | 1,272 | 431 |
| AttributeArgument | 1,252 | 63 |
| AddAssignmentExpression | 1,202 | 280 |
| DeclarationExpression | 1,153 | 298 |
| SwitchExpressionArm | 1,136 | 128 |
| ConditionalAccessExpression | 1,126 | 400 |
| MemberBindingExpression | 1,126 | 400 |
| CoalesceExpression | 1,054 | 415 |
| ParenthesizedLambdaExpression | 1,045 | 301 |
| AccessorList | 1,030 | 296 |
| GetAccessorDeclaration | 1,028 | 295 |
| LogicalOrExpression | 998 | 319 |
| NotPattern | 953 | 393 |
| TupleElement | 936 | 219 |
| SuppressNullableWarningExpression | 903 | 332 |
| AwaitExpression | 882 | 177 |
| WithExpression | 879 | 284 |
| WithInitializerExpression | 879 | 284 |
| GreaterThanExpression | 779 | 330 |
| NameColon | 770 | 220 |
| InterpolatedStringExpression | 720 | 179 |
| ArrayRankSpecifier | 676 | 276 |
| ArrayType | 672 | 276 |
| OmittedArraySizeExpression | 654 | 276 |
| ImplicitElementAccess | 619 | 121 |
| ConstructorDeclaration | 602 | 598 |
| RecordDeclaration | 571 | 473 |
| PostIncrementExpression | 564 | 262 |
| LessThanExpression | 558 | 249 |
| CharacterLiteralExpression | 552 | 131 |
| CastExpression | 540 | 174 |
| RecursivePattern | 520 | 142 |
| AttributeArgumentList | 513 | 63 |
| PropertyPatternClause | 511 | 135 |
| ObjectInitializerExpression | 501 | 211 |
| NotEqualsExpression | 488 | 210 |
| Subpattern | 461 | 125 |
| ParenthesizedExpression | 419 | 208 |
| BaseList | 402 | 301 |
| TupleType | 394 | 219 |
| CatchClause | 389 | 167 |
| CatchDeclaration | 386 | 165 |
| LockStatement | 383 | 61 |
| TryStatement | 355 | 184 |
| ContinueStatement | 341 | 167 |
| SwitchSection | 334 | 37 |
| BreakStatement | 322 | 69 |
| OrPattern | 314 | 102 |
| SimpleBaseType | 303 | 282 |
| ForStatement | 293 | 185 |
| EnumMemberDeclaration | 289 | 47 |
| UnaryMinusExpression | 237 | 94 |
| SubtractExpression | 233 | 106 |
| InitAccessorDeclaration | 229 | 80 |
| ThrowExpression | 229 | 110 |
| SpreadElement | 226 | 126 |
| ThrowStatement | 226 | 121 |
| DiscardPattern | 216 | 127 |
| WhileStatement | 215 | 130 |
| SwitchExpression | 213 | 128 |
| TypeOfExpression | 195 | 43 |
| UsingStatement | 188 | 79 |
| CaseSwitchLabel | 183 | 24 |
| SubtractAssignmentExpression | 179 | 70 |
| ElseClause | 170 | 126 |
| EventFieldDeclaration | 163 | 97 |
| SetAccessorDeclaration | 155 | 76 |
| GreaterThanOrEqualExpression | 132 | 82 |
| LessThanOrEqualExpression | 125 | 91 |
| ThisExpression | 125 | 83 |
| WhenClause | 116 | 25 |
| CasePatternSwitchLabel | 114 | 23 |
| InterfaceDeclaration | 111 | 111 |
| PrimaryConstructorBaseType | 110 | 19 |
| MultiplyExpression | 105 | 45 |
| IndexExpression | 95 | 50 |
| RangeExpression | 92 | 49 |
| RelationalPattern | 90 | 31 |
| ForEachVariableStatement | 89 | 66 |
| AsExpression | 85 | 43 |
| ArrayInitializerExpression | 75 | 41 |
| CatchFilterClause | 74 | 47 |
| TypeParameter | 67 | 34 |
| TypeParameterList | 62 | 34 |
| IsExpression | 60 | 37 |
| SwitchStatement | 60 | 37 |
| DivideExpression | 58 | 29 |
| ImplicitArrayCreationExpression | 53 | 38 |
| FinallyClause | 50 | 35 |
| BitwiseOrExpression | 49 | 25 |
| EnumDeclaration | 47 | 47 |
| YieldReturnStatement | 45 | 21 |
| AndPattern | 43 | 19 |
| DefaultSwitchLabel | 43 | 25 |
| ParenthesizedPattern | 43 | 21 |
| ExpressionColon | 37 | 17 |
| OrAssignmentExpression | 35 | 20 |
| ArrayCreationExpression | 34 | 23 |
| LeftShiftExpression | 32 | 2 |
| VarPattern | 30 | 12 |
| CoalesceAssignmentExpression | 21 | 19 |
| AttributeTargetSpecifier | 19 | 8 |
| CollectionInitializerExpression | 18 | 16 |
| TypeParameterConstraintClause | 18 | 10 |
| ClassConstraint | 17 | 9 |
| LocalFunctionStatement | 17 | 11 |
| PostDecrementExpression | 16 | 12 |
| ListPattern | 14 | 12 |
| BaseExpression | 13 | 5 |
| ConversionOperatorDeclaration | 13 | 2 |
| TypePattern | 11 | 4 |
| PreIncrementExpression | 10 | 8 |
| PositionalPatternClause | 9 | 9 |
| DefaultLiteralExpression | 8 | 6 |
| NameEquals | 8 | 5 |
| InterpolationFormatClause | 7 | 1 |
| YieldBreakStatement | 7 | 6 |
| BaseConstructorInitializer | 6 | 6 |
| ExplicitInterfaceSpecifier | 6 | 5 |
| GlobalStatement | 6 | 1 |
| FieldExpression | 4 | 1 |
| AndAssignmentExpression | 3 | 3 |
| BitwiseAndExpression | 3 | 3 |
| InterpolationAlignmentClause | 3 | 1 |
| RecordStructDeclaration | 3 | 3 |
| SlicePattern | 3 | 3 |
| ComplexElementInitializerExpression | 2 | 1 |
| ModuloExpression | 2 | 1 |
| RightShiftExpression | 2 | 1 |
| UncheckedExpression | 2 | 2 |
| AddAccessorDeclaration | 1 | 1 |
| BitwiseNotExpression | 1 | 1 |
| BracketedParameterList | 1 | 1 |
| DiscardDesignation | 1 | 1 |
| EventDeclaration | 1 | 1 |
| ExclusiveOrAssignmentExpression | 1 | 1 |
| ExclusiveOrExpression | 1 | 1 |
| IndexerDeclaration | 1 | 1 |
| OmittedTypeArgument | 1 | 1 |
| RemoveAccessorDeclaration | 1 | 1 |
| ThisConstructorInitializer | 1 | 1 |
| TypeConstraint | 1 | 1 |

## Attributes

| Attribute | Count | Files |
|---|---:|---:|
| Fact | 3,205 | 438 |
| InlineData | 485 | 51 |
| Theory | 103 | 53 |
| JsonPropertyName | 14 | 4 |
| DllImport | 4 | 1 |
| MemberData | 3 | 2 |
| CollectionBehavior | 2 | 2 |
| MarshalAs | 2 | 1 |
| Flags | 1 | 1 |
| NotNullIfNotNull | 1 | 1 |
| NotNullWhen | 1 | 1 |
| Out | 1 | 1 |
| SupportedOSPlatform | 1 | 1 |

## Parse errors (0)

None.

## Feature counts per project

| Feature | Llyn.Tests.Engine | Llyn.UIDeportment | Llyn.Tests.Conduct | Llyn.Infrastructure | Llyn.Application | Llyn.Conduct | Llyn.Tests.Convention | Llyn.Core | Llyn.ShellEngine | Llyn.Tests.Interface | Llyn.Tests.Windows | Llyn.Core.Windows | Llyn.UIVeneer | Llyn.UIDeportment.Capsule | Llyn.Host |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| Collection expression | 3,309 | 168 | 1,868 | 324 | 400 | 197 | 513 | 303 | 77 | 261 | 171 |  |  |  |  |
| using declaration | 2,325 | 5 | 2,648 | 652 | 17 |  | 7 |  |  | 22 | 66 | 9 |  |  |  |
| Lambda | 956 | 226 | 1,492 | 44 | 466 | 386 | 740 | 186 | 75 | 234 | 85 | 7 |  |  | 9 |
| if | 54 | 1,308 | 5 | 738 | 765 | 403 | 471 | 291 | 224 | 48 | 6 | 11 |  | 3 |  |
| Nullable reference type T? | 184 | 490 | 375 | 394 | 385 | 349 | 300 | 366 | 263 | 457 | 44 | 9 |  | 5 |  |
| Expression-bodied member | 54 | 692 | 37 | 22 | 118 | 201 | 27 | 168 | 153 | 1,300 | 37 | 1 |  |  |  |
| Target-typed new | 203 | 737 | 301 | 155 | 192 | 48 | 189 | 65 | 95 | 303 | 96 | 11 |  | 1 | 4 |
| File-scoped namespace | 240 | 312 | 156 | 193 | 136 | 268 | 115 | 388 | 101 | 100 | 26 | 7 | 38 | 5 |  |
| Readonly field | 40 | 618 | 4 | 128 | 450 | 350 | 215 | 31 | 183 | 46 | 6 | 9 |  | 1 |  |
| Declaration pattern | 7 | 998 | 4 | 64 | 224 | 233 | 369 | 29 | 77 | 10 | 3 | 2 |  |  |  |
| Tuple literal or tuple type | 465 | 47 | 295 | 390 | 112 | 73 | 164 | 60 | 83 | 38 | 28 |  |  |  |  |
| Sealed class | 246 | 270 | 156 | 83 | 204 | 247 | 43 | 253 | 72 | 43 | 23 | 7 |  | 5 |  |
| Conditional ?: | 15 | 225 | 18 | 354 | 306 | 172 | 156 | 167 | 78 | 34 | 3 | 2 |  |  |  |
| foreach | 56 | 201 | 17 | 309 | 353 | 30 | 305 | 168 | 31 | 48 | 2 | 2 |  |  |  |
| Class | 249 | 296 | 155 | 191 | 114 | 92 | 108 | 29 | 62 | 108 | 26 | 7 | 38 | 1 |  |
| Static lambda | 137 | 96 | 693 | 11 | 172 | 81 | 15 | 98 | 15 | 82 | 8 |  |  |  | 1 |
| Constant pattern | 6 | 153 | 8 | 144 | 100 | 175 | 358 | 192 | 49 | 20 |  | 16 |  |  |  |
| Full property | 7 | 647 | 5 | 6 | 118 | 187 | 2 | 147 | 46 | 30 |  | 1 |  |  |  |
| Null-conditional ?. ?[ | 131 | 192 | 83 | 63 | 59 | 363 | 91 | 43 | 85 | 9 | 7 |  |  |  |  |
| Null-coalescing ?? | 22 | 81 | 12 | 144 | 232 | 73 | 82 | 273 | 53 | 65 | 11 | 5 |  | 1 |  |
| not pattern | 5 | 341 | 1 | 64 | 93 | 156 | 185 | 37 | 57 | 11 | 1 | 2 |  |  |  |
| Auto property | 5 | 385 | 3 | 8 | 7 | 143 | 23 | 246 | 60 | 47 |  |  |  |  |  |
| Null-forgiving ! | 255 | 153 | 279 | 67 | 16 | 9 | 28 | 5 | 4 | 68 | 19 |  |  |  |  |
| await | 279 | 62 | 322 | 63 | 69 | 22 | 1 |  | 24 | 14 | 1 | 25 |  |  |  |
| with expression | 319 | 8 | 41 | 54 | 297 | 25 | 10 | 29 | 49 | 47 |  |  |  |  |  |
| is null | 9 | 140 | 5 | 112 | 193 | 95 | 174 | 62 | 63 | 19 | 2 | 1 |  |  |  |
| Extension method |  |  |  |  |  |  |  |  |  | 725 | 5 |  |  |  |  |
| Interpolated string | 58 | 19 | 7 | 171 | 60 |  | 384 | 7 |  | 8 | 6 |  |  |  |  |
| Nullable value type T? | 35 | 72 | 30 | 47 | 57 | 143 | 11 | 64 | 138 | 92 | 19 |  |  | 2 |  |
| Raw string | 154 | 2 | 7 | 239 |  |  | 235 |  |  | 8 | 5 |  |  |  |  |
| Async method | 217 | 61 | 201 | 33 | 40 | 20 | 1 |  | 20 | 10 | 1 | 13 |  |  |  |
| Const field | 83 | 75 | 14 | 195 | 17 | 3 | 112 | 81 | 4 | 12 | 6 | 3 |  | 1 |  |
| Constructor | 2 | 231 | 1 | 74 | 89 | 80 | 7 | 3 | 61 | 10 |  | 4 | 38 |  |  |
| Record class |  | 18 | 2 | 2 | 114 | 163 | 6 | 250 | 10 | 2 |  |  |  | 4 |  |
| Cast expression | 14 | 128 | 83 | 119 | 4 | 8 | 50 | 4 |  | 47 | 77 | 6 |  |  |  |
| Property pattern | 2 | 172 |  | 10 | 19 | 16 | 274 | 5 | 6 | 5 | 2 |  |  |  |  |
| Object initializer | 65 | 131 | 104 | 32 | 4 |  | 59 | 3 | 1 | 35 | 64 | 2 |  | 1 |  |
| Optional parameter | 15 | 8 | 6 | 6 | 12 | 11 | 1 | 295 | 12 | 122 |  |  |  | 4 |  |
| nameof | 1 | 198 | 3 | 13 | 129 | 30 | 17 | 6 | 5 | 2 | 1 | 4 |  |  |  |
| catch | 1 | 21 |  | 138 | 35 | 131 | 7 | 5 | 26 | 1 | 8 | 13 |  | 3 |  |
| lock |  | 14 |  | 2 | 85 | 2 | 5 |  | 270 | 5 |  |  |  |  |  |
| Deconstruction | 76 | 36 | 121 | 35 | 30 | 23 | 30 | 9 | 5 |  | 1 |  |  |  |  |
| Named argument | 170 |  | 39 | 16 | 31 |  | 16 | 23 | 3 | 64 |  |  |  |  |  |
| and / or pattern | 3 | 47 | 3 | 68 | 19 | 2 | 161 | 43 | 2 | 8 |  | 1 |  |  |  |
| try | 8 | 21 |  | 85 | 35 | 133 | 9 | 5 | 28 | 3 | 12 | 15 |  | 1 |  |
| Static class | 3 | 43 | 1 | 110 | 23 | 8 | 71 | 26 |  | 64 | 3 |  |  |  |  |
| for | 18 | 44 | 6 | 70 | 62 | 9 | 29 | 25 | 7 | 21 | 2 |  |  |  |  |
| Init accessor |  | 2 |  |  |  | 1 | 3 | 223 |  |  |  |  |  |  |  |
| throw expression | 5 | 7 | 30 | 9 | 96 | 29 | 1 | 6 | 11 | 26 | 1 | 8 |  |  |  |
| Spread element | 39 | 11 | 10 | 7 | 56 | 10 | 29 | 39 | 7 | 16 | 2 |  |  |  |  |
| Discard pattern |  | 13 | 1 | 19 | 31 | 33 | 67 | 36 | 10 | 1 |  | 5 |  |  |  |
| while | 19 | 9 | 5 | 109 | 18 | 3 | 25 | 21 | 1 | 5 |  |  |  |  |  |
| Switch expression |  | 11 | 1 | 19 | 31 | 34 | 65 | 36 | 10 | 1 |  | 5 |  |  |  |
| typeof |  | 141 | 5 | 4 |  |  | 3 |  |  | 4 | 38 |  |  |  |  |
| using statement | 37 | 1 | 2 | 135 | 7 | 2 |  |  |  | 1 | 1 | 2 |  |  |  |
| Field-like event |  | 65 |  |  |  | 96 |  |  | 2 |  |  |  |  |  |  |
| Discard _ | 33 | 27 | 49 | 4 | 7 | 3 | 10 | 10 | 6 | 1 | 2 |  |  |  |  |
| Case guard when |  | 6 |  | 1 | 1 | 1 | 100 | 5 |  | 2 |  |  |  |  |  |
| Interface |  | 2 |  |  | 1 | 1 |  | 78 | 29 |  |  |  |  |  |  |
| Index from end ^ | 17 | 5 | 49 | 1 | 4 | 2 | 9 | 8 |  |  |  |  |  |  |  |
| Verbatim string | 9 |  |  |  | 3 |  | 79 |  |  | 2 |  |  |  |  |  |
| Range .. | 11 |  | 12 | 16 | 2 |  | 29 | 21 |  | 1 |  |  |  |  |  |
| Relational pattern |  | 7 |  | 28 | 8 | 1 | 5 | 40 | 1 |  |  |  |  |  |  |
| as cast |  | 56 |  | 3 |  |  | 9 |  |  | 16 | 1 |  |  |  |  |
| Partial type |  |  | 4 |  |  |  | 7 |  |  | 27 |  |  | 38 |  |  |
| Exception filter |  | 3 |  | 33 | 18 |  | 2 |  | 18 |  |  |  |  |  |  |
| is type test | 1 | 22 |  | 2 | 3 |  | 32 |  |  |  |  |  |  |  |  |
| Switch statement |  | 8 |  | 8 | 1 | 2 | 20 | 18 | 1 | 2 |  |  |  |  |  |
| Generic method |  | 25 | 1 |  | 14 | 4 | 1 | 7 | 1 | 5 |  |  |  |  |  |
| Async lambda |  |  | 53 |  | 1 |  |  |  |  | 1 |  |  |  |  |  |
| yield return / break |  | 9 |  | 7 | 9 |  | 25 | 2 |  |  |  |  |  |  |  |
| Local const | 37 |  | 1 | 4 | 1 | 1 | 6 |  | 1 |  |  |  |  |  |  |
| finally | 7 | 3 |  | 2 | 14 | 5 | 2 |  | 5 | 2 | 4 | 6 |  |  |  |
| Enum |  | 2 |  |  |  | 15 |  | 30 |  |  |  |  |  |  |  |
| Nested type | 9 | 7 | 1 |  | 2 | 3 |  |  |  | 10 |  |  |  |  |  |
| var pattern |  |  |  |  |  | 1 | 28 |  |  |  | 1 |  |  |  |  |
| ref or out parameter | 2 | 1 | 1 | 2 | 7 |  | 3 | 5 | 3 | 5 |  |  |  |  |  |
| Null-coalescing assignment ??= | 1 | 10 |  | 1 | 4 | 1 | 3 |  |  |  |  | 1 |  |  |  |
| Collection initializer |  | 2 | 3 | 5 | 3 | 1 |  |  |  | 2 | 2 |  |  |  |  |
| Constraint clause |  | 12 | 1 |  | 1 |  |  |  |  | 4 |  |  |  |  |  |
| Local function | 1 | 4 |  |  |  | 5 | 4 |  |  | 1 | 1 | 1 |  |  |  |
| await using | 12 |  |  |  |  | 1 |  |  |  |  |  | 1 |  |  |  |
| List pattern | 1 | 4 |  |  |  |  | 8 |  | 1 |  |  |  |  |  |  |
| Conversion operator |  | 12 |  |  |  |  |  | 1 |  |  |  |  |  |  |  |
| Default interface method body |  |  |  |  |  |  |  |  | 11 |  |  |  |  |  |  |
| Type pattern |  | 1 |  |  |  |  | 10 |  |  |  |  |  |  |  |  |
| Positional pattern |  | 2 |  | 1 | 1 | 4 | 1 |  |  |  |  |  |  |  |  |
| default literal |  | 1 | 1 | 1 |  |  | 5 |  |  |  |  |  |  |  |  |
| Primary constructor on class or struct |  |  |  |  |  | 1 | 4 |  |  | 1 |  |  |  |  |  |
| Required member |  | 2 |  |  |  |  | 3 |  |  |  |  |  |  |  |  |
| field keyword in accessor |  | 4 |  |  |  |  |  |  |  |  |  |  |  |  |  |
| Generic type |  | 2 |  |  |  | 2 |  |  |  |  |  |  |  |  |  |
| Record struct |  | 1 |  |  |  |  | 1 | 1 |  |  |  |  |  |  |  |
| Abstract class |  | 1 |  |  | 1 |  |  |  |  |  |  |  |  |  |  |
| checked / unchecked expression |  | 1 |  |  |  |  |  |  |  |  |  | 1 |  |  |  |
| Static constructor |  | 1 |  |  |  |  |  |  |  | 1 |  |  |  |  |  |
| Static local function |  | 2 |  |  |  |  |  |  |  |  |  |  |  |  |  |
| Using alias |  |  |  |  |  |  |  |  |  | 1 |  |  |  |  | 1 |
| Alias of any type |  |  |  |  |  |  |  |  |  | 1 |  |  |  |  |  |
| Event accessors |  | 1 |  |  |  |  |  |  |  |  |  |  |  |  |  |
| Indexer |  | 1 |  |  |  |  |  |  |  |  |  |  |  |  |  |

## Style A share per project

| Style | Llyn.Tests.Engine | Llyn.UIDeportment | Llyn.Tests.Conduct | Llyn.Infrastructure | Llyn.Application | Llyn.Conduct | Llyn.Tests.Convention | Llyn.Core | Llyn.ShellEngine | Llyn.Tests.Interface | Llyn.Tests.Windows | Llyn.Core.Windows | Llyn.UIVeneer | Llyn.UIDeportment.Capsule | Llyn.Host |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| Null check | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - | - | - |
| Object creation | 52.86 % | 47.98 % | 38.05 % | 24.37 % | 30.92 % | 10.00 % | 42.00 % | 25.90 % | 30.35 % | 33.41 % | 39.34 % | 30.56 % | - | 14.29 % | 20.00 % |
| Collection creation | 99.49 % | 97.11 % | 99.20 % | 98.18 % | 98.77 % | 99.49 % | 99.03 % | 99.02 % | 100.00 % | 99.62 % | 92.43 % | 0.00 % | - | - | - |
| Member body | 2.72 % | 25.44 % | 2.53 % | 1.97 % | 10.32 % | 16.26 % | 3.80 % | 28.97 % | 17.39 % | 78.13 % | 23.27 % | 2.63 % | 0.00 % | 0.00 % | - |
| Local type | 0.03 % | 4.04 % | 0.00 % | 0.00 % | 0.11 % | 0.24 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | - | 0.00 % | 0.00 % |
| Using | 98.43 % | 83.33 % | 99.92 % | 82.85 % | 70.83 % | 0.00 % | 100.00 % | - | - | 95.65 % | 98.51 % | 81.82 % | - | - | - |
| Namespace | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - |
| Text building | 27.75 % | 47.50 % | 14.00 % | 47.50 % | 69.77 % | 0.00 % | 81.70 % | 24.14 % | - | 44.44 % | 85.71 % | 0.00 % | - | 0.00 % | - |
| Branch | - | 57.89 % | 100.00 % | 70.37 % | 96.88 % | 94.44 % | 76.47 % | 66.67 % | 90.91 % | 33.33 % | - | 100.00 % | - | - | - |
| Lambda body | 96.86 % | 87.17 % | 75.67 % | 90.91 % | 93.78 % | 96.63 % | 98.92 % | 98.39 % | 92.00 % | 84.62 % | 32.94 % | 85.71 % | - | - | 77.78 % |
| Type test | 100.00 % | 92.25 % | 100.00 % | 93.88 % | 100.00 % | 100.00 % | 94.58 % | 100.00 % | 100.00 % | 44.83 % | 75.00 % | - | - | - | - |
| Constructor | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 1.25 % | 36.36 % | 0.00 % | 0.00 % | 11.11 % | - | 0.00 % | 0.00 % | - | - |
