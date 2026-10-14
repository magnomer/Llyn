# Syntax statistics - 0.20.14510

- Generated: 2026-10-14 10:48:36 +09:00
- Source roots: src, tests
- Root-level files: none
- Project segments: 1
- Excluded directories: .git, .vs, artifacts, bin, node_modules, obj, packages, publish
- Excluded suffixes: .g.cs, .g.i.cs, .AssemblyInfo.cs, .GlobalUsings.g.cs, .Designer.cs
- Roslyn: 4.14.0-3.25262.10
- Language version: Preview
- Scanned: 2,016 files, 15 projects, 215,373 lines

## Summary

| Measure | Value |
|---|---:|
| Files | 2,016 |
| Lines | 215,373 |
| Nodes | 1,145,880 |
| Syntax kinds used | 197 |
| Features used | 101 of 138 |
| Minimum C# version | 14 |
| Parse errors | 0 |

## Families

| Family | Features | Used | Count | Per 1k lines |
|---|---:|---:|---:|---:|
| Types | 15 | 12 | 4,204 | 19.52 |
| Namespaces | 6 | 3 | 2,018 | 9.37 |
| Members | 29 | 21 | 9,973 | 46.31 |
| Patterns | 16 | 16 | 6,595 | 30.62 |
| Expressions | 36 | 29 | 27,914 | 129.61 |
| Statements | 20 | 14 | 13,326 | 61.87 |
| Generics | 6 | 2 | 76 | 0.35 |
| Async | 2 | 2 | 653 | 3.03 |
| Nullability | 3 | 2 | 4,229 | 19.64 |
| Unsafe | 2 | 0 | 0 | 0.00 |
| Directives | 3 | 0 | 0 | 0.00 |
| Total | 138 | 101 | 68,988 | 320.32 |

## Projects

| Project | Files | Lines | Nodes | Features used | Newest C# |
|---|---:|---:|---:|---:|---:|
| Llyn.Tests.Engine | 225 | 37,433 | 261,331 | 61 | 12 |
| Llyn.UIDeportment | 312 | 34,215 | 158,181 | 88 | 14 |
| Llyn.Tests.Conduct | 156 | 28,315 | 205,500 | 61 | 12 |
| Llyn.Infrastructure | 185 | 24,621 | 113,370 | 70 | 12 |
| Llyn.Application | 130 | 19,623 | 91,657 | 73 | 12 |
| Llyn.Conduct | 268 | 17,714 | 70,020 | 68 | 12 |
| Llyn.Tests.Convention | 115 | 17,615 | 85,419 | 79 | 12 |
| Llyn.Core | 358 | 12,409 | 51,200 | 60 | 12 |
| Llyn.ShellEngine | 98 | 10,071 | 40,552 | 58 | 12 |
| Llyn.Tests.Interface | 93 | 9,201 | 46,616 | 72 | 12 |
| Llyn.Tests.Windows | 25 | 2,759 | 17,108 | 47 | 12 |
| Llyn.Core.Windows | 7 | 798 | 3,140 | 37 | 10 |
| Llyn.UIVeneer | 38 | 416 | 824 | 4 | 10 |
| Llyn.UIDeportment.Capsule | 5 | 96 | 427 | 15 | 10 |
| Llyn.Host | 1 | 87 | 535 | 4 | 9 |
| Total | 2,016 | 215,373 | 1,145,880 | 101 | 14 |

## Versions

| C# version | Features | Used | Count |
|---|---:|---:|---:|
| 1 | 51 | 35 | 17,873 |
| 2 | 10 | 8 | 2,236 |
| 3 | 7 | 5 | 6,881 |
| 4 | 3 | 2 | 821 |
| 5 | 3 | 3 | 1,516 |
| 6 | 6 | 5 | 4,998 |
| 7 | 11 | 10 | 6,594 |
| 7.1 | 1 | 1 | 8 |
| 7.2 | 1 | 0 | 0 |
| 7.3 | 1 | 0 | 0 |
| 8 | 17 | 13 | 11,219 |
| 9 | 10 | 9 | 6,667 |
| 10 | 3 | 2 | 2,018 |
| 11 | 6 | 3 | 650 |
| 12 | 4 | 4 | 7,503 |
| 13 | 1 | 0 | 0 |
| 14 | 3 | 1 | 4 |

## Features

| Feature | Family | C# | Count | Per 1k lines | Files | First |
|---|---|---|---:|---:|---:|---|
| Collection expression | Expressions | 12 | 7,282 | 33.81 | 914 | src/Llyn.Application/Card/LCardClerk.cs:60 |
| using declaration | Statements | 8 | 5,614 | 26.07 | 425 | src/Llyn.Application/Card/LTranslationClerk.cs:160 |
| Lambda | Expressions | 3 | 4,769 | 22.14 | 751 | src/Llyn.Application/Card/LCardClerk.cs:95 |
| if | Statements | 1 | 4,242 | 19.70 | 710 | src/Llyn.Application/Card/LCardClerk.cs:73 |
| Nullable reference type T? | Nullability | 8 | 3,565 | 16.55 | 978 | src/Llyn.Application/Card/LCardClerkField.cs:91 |
| Expression-bodied member | Members | 6 | 2,693 | 12.50 | 507 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Target-typed new | Expressions | 9 | 2,333 | 10.83 | 566 | src/Llyn.Application/Card/LMentionClerk.cs:153 |
| Readonly field | Members | 1 | 2,029 | 9.42 | 618 | src/Llyn.Application/Card/LCardClerk.cs:10 |
| File-scoped namespace | Namespaces | 10 | 2,015 | 9.36 | 2,015 | src/Llyn.Application/Card/LCardClerk.cs:6 |
| Declaration pattern | Patterns | 7 | 2,006 | 9.31 | 403 | src/Llyn.Application/Card/LMeaningClerk.cs:105 |
| Tuple literal or tuple type | Expressions | 7 | 1,705 | 7.92 | 282 | src/Llyn.Application/Card/LMeaningClerk.cs:26 |
| Sealed class | Types | 1 | 1,601 | 7.43 | 1,486 | src/Llyn.Application/Card/LCardClerk.cs:8 |
| Conditional ?: | Expressions | 1 | 1,487 | 6.90 | 552 | src/Llyn.Application/Card/LCardClerk.cs:170 |
| foreach | Statements | 1 | 1,484 | 6.89 | 482 | src/Llyn.Application/Card/LCardClerk.cs:62 |
| Class | Types | 1 | 1,434 | 6.66 | 1,416 | src/Llyn.Application/Card/LCardClerk.cs:8 |
| Static lambda | Expressions | 9 | 1,337 | 6.21 | 322 | src/Llyn.Application/Card/LCardClerk.cs:95 |
| Full property | Members | 1 | 1,193 | 5.54 | 356 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Constant pattern | Patterns | 7 | 1,140 | 5.29 | 206 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Null-conditional ?. ?[ | Expressions | 6 | 1,124 | 5.22 | 398 | src/Llyn.Application/Card/LMentionClerk.cs:47 |
| Null-coalescing ?? | Expressions | 2 | 1,019 | 4.73 | 406 | src/Llyn.Application/Card/LCardClerkField.cs:74 |
| not pattern | Patterns | 9 | 940 | 4.36 | 388 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| Auto property | Members | 3 | 919 | 4.27 | 269 | src/Llyn.Application/Citation/LCitationClerk.cs:41 |
| Null-forgiving ! | Expressions | 8 | 880 | 4.09 | 322 | src/Llyn.Application/Draft/Clerk/Card/LDraftClerkCard.cs:124 |
| await | Expressions | 5 | 863 | 4.01 | 173 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:139 |
| is null | Patterns | 7 | 849 | 3.94 | 369 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| with expression | Expressions | 9 | 819 | 3.80 | 265 | src/Llyn.Application/Card/LCardClerk.cs:115 |
| Interpolated string | Expressions | 6 | 703 | 3.26 | 167 | src/Llyn.Application/Localization/LLocalizationReader.cs:55 |
| Extension method | Members | 3 | 681 | 3.16 | 43 | tests/Llyn.Tests.Interface/Atelier/TInterfaceArea.cs:7 |
| Nullable value type T? | Nullability | 2 | 664 | 3.08 | 288 | src/Llyn.Application/Card/LMeaningClerk.cs:104 |
| Raw string | Expressions | 11 | 631 | 2.93 | 144 | src/Llyn.Infrastructure/Database/Base/LDatabase.cs:51 |
| Async method | Async | 5 | 598 | 2.78 | 173 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:91 |
| Constructor | Members | 1 | 586 | 2.72 | 582 | src/Llyn.Application/Card/LCardClerk.cs:21 |
| Const field | Members | 1 | 575 | 2.67 | 249 | src/Llyn.Application/Card/LMentionClerk.cs:11 |
| Record class | Types | 9 | 556 | 2.58 | 458 | src/Llyn.Application/Draft/Row/LDraftClerkAncestor.cs:263 |
| Cast expression | Expressions | 1 | 511 | 2.37 | 169 | src/Llyn.Application/Markup/LMarkupClerkLink.cs:228 |
| Property pattern | Patterns | 8 | 510 | 2.37 | 133 | src/Llyn.Application/Draft/Clerk/LDraftClerk.cs:83 |
| Object initializer | Expressions | 3 | 495 | 2.30 | 207 | src/Llyn.Application/Portrait/LPortraitClerkLabel.cs:25 |
| Optional parameter | Members | 4 | 483 | 2.24 | 161 | src/Llyn.Application/Draft/Clerk/LDraftClerkPanel.cs:170 |
| nameof | Expressions | 6 | 406 | 1.89 | 142 | src/Llyn.Application/Citation/LAuthorClerk.cs:361 |
| catch | Statements | 1 | 382 | 1.77 | 165 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:142 |
| Deconstruction | Expressions | 7 | 362 | 1.68 | 154 | src/Llyn.Application/Card/LMeaningClerk.cs:46 |
| lock | Statements | 1 | 356 | 1.65 | 59 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:98 |
| and / or pattern | Patterns | 9 | 351 | 1.63 | 105 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| try | Statements | 1 | 348 | 1.62 | 182 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:137 |
| Static class | Types | 2 | 346 | 1.61 | 346 | src/Llyn.Application/Card/LCardClerkField.cs:7 |
| Named argument | Members | 4 | 338 | 1.57 | 101 | src/Llyn.Application/Card/LTranslationClerk.cs:164 |
| for | Statements | 1 | 287 | 1.33 | 180 | src/Llyn.Application/Card/LCardClerk.cs:280 |
| Init accessor | Members | 9 | 229 | 1.06 | 80 | src/Llyn.Conduct/Lexicon/CExample.cs:20 |
| throw expression | Expressions | 7 | 217 | 1.01 | 105 | src/Llyn.Application/Card/LCardClerkField.cs:75 |
| Spread element | Expressions | 12 | 214 | 0.99 | 119 | src/Llyn.Application/Card/LCardClerk.cs:95 |
| Discard pattern | Patterns | 8 | 206 | 0.96 | 118 | src/Llyn.Application/Citation/LAuthorClerk.cs:243 |
| Switch expression | Patterns | 8 | 203 | 0.94 | 119 | src/Llyn.Application/Citation/LAuthorClerk.cs:239 |
| while | Statements | 1 | 203 | 0.94 | 122 | src/Llyn.Application/Card/LMeaningClerk.cs:44 |
| typeof | Expressions | 1 | 195 | 0.91 | 43 | src/Llyn.Infrastructure/Configuration/LLocalizationLoader.cs:20 |
| using statement | Statements | 1 | 173 | 0.80 | 74 | src/Llyn.Application/Citation/LAuthorCitation.cs:51 |
| Field-like event | Members | 1 | 163 | 0.76 | 97 | src/Llyn.Conduct/Atelier/CWorkspace.cs:36 |
| Discard _ | Expressions | 7 | 152 | 0.71 | 58 | src/Llyn.Application/Draft/Clerk/Card/LDraftClerkCard.cs:127 |
| Case guard when | Patterns | 7 | 116 | 0.54 | 25 | src/Llyn.Application/Citation/LAuthorClerk.cs:241 |
| Interface | Types | 1 | 104 | 0.48 | 104 | src/Llyn.Application/Pronunciation/Relay/LListener.cs:5 |
| Index from end ^ | Expressions | 8 | 95 | 0.44 | 50 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:230 |
| Verbatim string | Expressions | 1 | 93 | 0.43 | 20 | src/Llyn.Application/Localization/LLocalizationReader.cs:13 |
| Relational pattern | Patterns | 9 | 91 | 0.42 | 31 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Range .. | Expressions | 8 | 89 | 0.41 | 47 | src/Llyn.Application/Card/LMentionClerk.cs:159 |
| as cast | Expressions | 1 | 85 | 0.39 | 43 | src/Llyn.Infrastructure/Database/Base/Workspace/LWorkspaceArchive.cs:132 |
| Partial type | Types | 2 | 75 | 0.35 | 75 | src/Llyn.UIVeneer/Display/View/PDisplay.xaml.cs:5 |
| Exception filter | Statements | 6 | 72 | 0.33 | 46 | src/Llyn.Application/Outpost/LCourierClerk.cs:196 |
| Switch statement | Patterns | 1 | 60 | 0.28 | 37 | src/Llyn.Application/Portrait/LPortraitClerkPress.cs:59 |
| is type test | Patterns | 1 | 59 | 0.27 | 36 | src/Llyn.Application/Outpost/LCourierClerk.cs:288 |
| Generic method | Generics | 2 | 58 | 0.27 | 30 | src/Llyn.Application/Card/LCardClerkField.cs:183 |
| Async lambda | Async | 5 | 55 | 0.26 | 10 | src/Llyn.Application/Outpost/LCourierClerk.cs:251 |
| yield return / break | Statements | 2 | 52 | 0.24 | 21 | src/Llyn.Application/Card/LCardClerkField.cs:35 |
| Local const | Statements | 1 | 51 | 0.24 | 27 | src/Llyn.Application/Markup/LMarkupClerkIntake.cs:271 |
| finally | Statements | 1 | 48 | 0.22 | 34 | src/Llyn.Application/Outpost/LCourierClerk.cs:76 |
| Enum | Types | 1 | 41 | 0.19 | 41 | src/Llyn.Conduct/Catalog/Vista/CCatalogOrder.cs:3 |
| Nested type | Types | 1 | 32 | 0.15 | 19 | src/Llyn.Application/Draft/Row/LDraftClerkAncestor.cs:263 |
| var pattern | Patterns | 7 | 30 | 0.14 | 12 | src/Llyn.Conduct/Card/CFolio.cs:219 |
| ref or out parameter | Members | 1 | 28 | 0.13 | 23 | src/Llyn.Application/Draft/Clerk/Card/LDraftClerkCard.cs:162 |
| Null-coalescing assignment ??= | Expressions | 8 | 21 | 0.10 | 19 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:160 |
| Constraint clause | Generics | 2 | 18 | 0.08 | 10 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:171 |
| Collection initializer | Expressions | 3 | 17 | 0.08 | 15 | src/Llyn.Application/Outpost/LCourierClerk.cs:177 |
| Local function | Members | 7 | 17 | 0.08 | 11 | src/Llyn.Conduct/Desk/CErrand.cs:56 |
| await using | Statements | 8 | 14 | 0.07 | 7 | src/Llyn.Conduct/Portrait/CPortrait.cs:144 |
| List pattern | Patterns | 11 | 14 | 0.07 | 12 | src/Llyn.ShellEngine/Engine/Facade/LDraftFacade.cs:70 |
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
| Null check | is null / is not null | 849 | == null / != null | 0 | 100.00 % |
| Object creation | Target-typed new() | 2,333 | new T() | 4,166 | 35.90 % |
| Collection creation | Collection expression | 7,282 | Collection initializer or array creation | 70 | 99.05 % |
| Member body | Expression body | 2,693 | Block body | 10,627 | 20.22 % |
| Local type | var | 44 | Explicit type | 22,431 | 0.20 % |
| Using | Declaration | 5,614 | Statement | 173 | 97.01 % |
| Namespace | File-scoped | 2,015 | Block | 0 | 100.00 % |
| Text building | Interpolated string | 703 | string.Format or + with a string literal | 571 | 55.18 % |
| Branch | Switch expression | 203 | Switch statement | 60 | 77.19 % |
| Lambda body | Expression | 4,190 | Block | 579 | 87.86 % |
| Type test | is pattern with designation | 1,159 | as | 85 | 93.17 % |
| Constructor | Primary | 6 | Explicit | 582 | 1.02 % |

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
| IdentifierName | 331,262 | 2,016 |
| Argument | 119,585 | 1,532 |
| SimpleMemberAccessExpression | 109,314 | 1,517 |
| ArgumentList | 74,006 | 1,570 |
| InvocationExpression | 67,665 | 1,530 |
| ExpressionStatement | 32,367 | 1,413 |
| PredefinedType | 32,309 | 1,873 |
| StringLiteralExpression | 26,871 | 1,133 |
| Parameter | 23,523 | 1,816 |
| VariableDeclaration | 23,147 | 1,334 |
| VariableDeclarator | 23,147 | 1,334 |
| EqualsValueClause | 22,048 | 1,391 |
| LocalDeclarationStatement | 19,543 | 1,158 |
| Block | 18,840 | 1,495 |
| ParameterList | 14,211 | 1,936 |
| NumericLiteralExpression | 12,017 | 1,120 |
| MethodDeclaration | 11,992 | 1,558 |
| GenericName | 11,103 | 1,400 |
| TypeArgumentList | 11,103 | 1,400 |
| QualifiedName | 9,624 | 2,016 |
| ExpressionElement | 8,103 | 504 |
| SimpleAssignmentExpression | 7,826 | 1,048 |
| CollectionExpression | 7,282 | 914 |
| UsingDirective | 6,524 | 1,730 |
| ReturnStatement | 6,148 | 1,114 |
| NullLiteralExpression | 4,373 | 931 |
| IfStatement | 4,242 | 710 |
| NullableType | 4,229 | 1,050 |
| ObjectCreationExpression | 4,166 | 915 |
| SimpleLambdaExpression | 3,732 | 675 |
| Attribute | 3,713 | 437 |
| AttributeList | 3,713 | 437 |
| SingleVariableDesignation | 3,448 | 592 |
| BracketedArgumentList | 3,321 | 573 |
| FieldDeclaration | 2,990 | 779 |
| IsPatternExpression | 2,862 | 608 |
| ElementAccessExpression | 2,711 | 523 |
| ArrowExpressionClause | 2,695 | 508 |
| ImplicitObjectCreationExpression | 2,333 | 566 |
| PropertyDeclaration | 2,118 | 521 |
| FalseLiteralExpression | 2,109 | 572 |
| CompilationUnit | 2,016 | 2,016 |
| FileScopedNamespaceDeclaration | 2,015 | 2,015 |
| DeclarationPattern | 2,006 | 403 |
| ConstantPattern | 1,989 | 468 |
| TrueLiteralExpression | 1,824 | 530 |
| TupleExpression | 1,694 | 283 |
| EqualsExpression | 1,679 | 602 |
| AddExpression | 1,640 | 372 |
| InterpolatedStringText | 1,584 | 167 |
| ConditionalExpression | 1,487 | 552 |
| LogicalAndExpression | 1,445 | 413 |
| ClassDeclaration | 1,434 | 1,416 |
| Interpolation | 1,398 | 166 |
| ForEachStatement | 1,397 | 464 |
| LogicalNotExpression | 1,252 | 426 |
| AttributeArgument | 1,215 | 61 |
| AddAssignmentExpression | 1,202 | 280 |
| DeclarationExpression | 1,135 | 291 |
| ConditionalAccessExpression | 1,124 | 398 |
| MemberBindingExpression | 1,124 | 398 |
| SwitchExpressionArm | 1,057 | 119 |
| ParenthesizedLambdaExpression | 1,037 | 295 |
| AccessorList | 1,022 | 294 |
| GetAccessorDeclaration | 1,020 | 293 |
| CoalesceExpression | 1,019 | 406 |
| LogicalOrExpression | 989 | 315 |
| NotPattern | 940 | 388 |
| TupleElement | 888 | 205 |
| SuppressNullableWarningExpression | 880 | 322 |
| AwaitExpression | 863 | 173 |
| WithExpression | 819 | 265 |
| WithInitializerExpression | 819 | 265 |
| GreaterThanExpression | 763 | 323 |
| NameColon | 745 | 215 |
| InterpolatedStringExpression | 703 | 167 |
| ArrayRankSpecifier | 669 | 269 |
| ArrayType | 665 | 269 |
| OmittedArraySizeExpression | 647 | 269 |
| ImplicitElementAccess | 610 | 117 |
| ConstructorDeclaration | 588 | 584 |
| RecordDeclaration | 556 | 458 |
| PostIncrementExpression | 552 | 254 |
| LessThanExpression | 539 | 241 |
| CharacterLiteralExpression | 524 | 126 |
| RecursivePattern | 519 | 140 |
| CastExpression | 511 | 169 |
| PropertyPatternClause | 510 | 133 |
| AttributeArgumentList | 498 | 61 |
| ObjectInitializerExpression | 495 | 207 |
| NotEqualsExpression | 469 | 199 |
| Subpattern | 460 | 123 |
| ParenthesizedExpression | 412 | 202 |
| BaseList | 389 | 288 |
| CatchClause | 382 | 165 |
| CatchDeclaration | 379 | 163 |
| TupleType | 374 | 205 |
| LockStatement | 356 | 59 |
| TryStatement | 348 | 182 |
| ContinueStatement | 334 | 162 |
| SwitchSection | 334 | 37 |
| BreakStatement | 321 | 68 |
| OrPattern | 310 | 99 |
| SimpleBaseType | 290 | 269 |
| ForStatement | 287 | 180 |
| EnumMemberDeclaration | 247 | 41 |
| UnaryMinusExpression | 235 | 92 |
| InitAccessorDeclaration | 229 | 80 |
| SubtractExpression | 224 | 103 |
| ThrowExpression | 217 | 105 |
| SpreadElement | 214 | 119 |
| DiscardPattern | 206 | 118 |
| ThrowStatement | 205 | 106 |
| SwitchExpression | 203 | 119 |
| WhileStatement | 203 | 122 |
| TypeOfExpression | 195 | 43 |
| CaseSwitchLabel | 183 | 24 |
| SubtractAssignmentExpression | 179 | 70 |
| UsingStatement | 173 | 74 |
| EventFieldDeclaration | 163 | 97 |
| ElseClause | 158 | 121 |
| SetAccessorDeclaration | 154 | 75 |
| GreaterThanOrEqualExpression | 126 | 78 |
| LessThanOrEqualExpression | 125 | 91 |
| ThisExpression | 125 | 83 |
| WhenClause | 116 | 25 |
| CasePatternSwitchLabel | 114 | 23 |
| PrimaryConstructorBaseType | 110 | 19 |
| MultiplyExpression | 105 | 45 |
| InterfaceDeclaration | 104 | 104 |
| IndexExpression | 95 | 50 |
| RelationalPattern | 91 | 31 |
| RangeExpression | 89 | 47 |
| ForEachVariableStatement | 87 | 64 |
| AsExpression | 85 | 43 |
| ArrayInitializerExpression | 73 | 39 |
| CatchFilterClause | 72 | 46 |
| TypeParameter | 67 | 34 |
| TypeParameterList | 62 | 34 |
| SwitchStatement | 60 | 37 |
| IsExpression | 59 | 36 |
| DivideExpression | 55 | 28 |
| ImplicitArrayCreationExpression | 51 | 36 |
| BitwiseOrExpression | 49 | 25 |
| FinallyClause | 48 | 34 |
| YieldReturnStatement | 45 | 21 |
| DefaultSwitchLabel | 43 | 25 |
| ParenthesizedPattern | 42 | 20 |
| AndPattern | 41 | 18 |
| EnumDeclaration | 41 | 41 |
| ExpressionColon | 37 | 17 |
| OrAssignmentExpression | 35 | 20 |
| ArrayCreationExpression | 34 | 23 |
| LeftShiftExpression | 32 | 2 |
| VarPattern | 30 | 12 |
| CoalesceAssignmentExpression | 21 | 19 |
| AttributeTargetSpecifier | 19 | 8 |
| TypeParameterConstraintClause | 18 | 10 |
| ClassConstraint | 17 | 9 |
| CollectionInitializerExpression | 17 | 15 |
| LocalFunctionStatement | 17 | 11 |
| PostDecrementExpression | 15 | 11 |
| ListPattern | 14 | 12 |
| BaseExpression | 13 | 5 |
| ConversionOperatorDeclaration | 13 | 2 |
| TypePattern | 11 | 4 |
| PositionalPatternClause | 9 | 9 |
| DefaultLiteralExpression | 8 | 6 |
| NameEquals | 8 | 5 |
| InterpolationFormatClause | 7 | 1 |
| YieldBreakStatement | 7 | 6 |
| BaseConstructorInitializer | 6 | 6 |
| ExplicitInterfaceSpecifier | 6 | 5 |
| GlobalStatement | 6 | 1 |
| PreIncrementExpression | 6 | 4 |
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
| Fact | 3,114 | 422 |
| InlineData | 470 | 49 |
| Theory | 99 | 51 |
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
| Collection expression | 3,081 | 168 | 1,868 | 302 | 362 | 197 | 513 | 297 | 77 | 247 | 170 |  |  |  |  |
| using declaration | 2,246 | 5 | 2,648 | 599 | 12 |  | 7 |  |  | 22 | 66 | 9 |  |  |  |
| Lambda | 918 | 226 | 1,492 | 41 | 404 | 386 | 740 | 186 | 73 | 202 | 85 | 7 |  |  | 9 |
| if | 54 | 1,308 | 5 | 715 | 733 | 403 | 471 | 271 | 223 | 39 | 6 | 11 |  | 3 |  |
| Nullable reference type T? | 182 | 490 | 375 | 370 | 377 | 349 | 300 | 356 | 263 | 445 | 44 | 9 |  | 5 |  |
| Expression-bodied member | 50 | 692 | 37 | 16 | 117 | 201 | 27 | 164 | 128 | 1,223 | 37 | 1 |  |  |  |
| Target-typed new | 192 | 737 | 301 | 147 | 177 | 48 | 189 | 65 | 90 | 278 | 93 | 11 |  | 1 | 4 |
| Readonly field | 40 | 616 | 4 | 123 | 418 | 350 | 215 | 31 | 180 | 37 | 5 | 9 |  | 1 |  |
| File-scoped namespace | 225 | 312 | 156 | 185 | 130 | 268 | 115 | 358 | 98 | 93 | 25 | 7 | 38 | 5 |  |
| Declaration pattern | 7 | 998 | 4 | 62 | 212 | 233 | 369 | 29 | 77 | 10 | 3 | 2 |  |  |  |
| Tuple literal or tuple type | 450 | 47 | 295 | 378 | 106 | 73 | 164 | 58 | 76 | 30 | 28 |  |  |  |  |
| Sealed class | 231 | 270 | 156 | 77 | 198 | 247 | 43 | 238 | 70 | 37 | 22 | 7 |  | 5 |  |
| Conditional ?: | 15 | 224 | 18 | 331 | 290 | 174 | 156 | 165 | 78 | 31 | 3 | 2 |  |  |  |
| foreach | 55 | 201 | 17 | 299 | 333 | 30 | 305 | 165 | 31 | 44 | 2 | 2 |  |  |  |
| Class | 234 | 296 | 155 | 183 | 108 | 92 | 108 | 26 | 60 | 101 | 25 | 7 | 38 | 1 |  |
| Static lambda | 113 | 96 | 693 | 11 | 141 | 81 | 15 | 98 | 15 | 65 | 8 |  |  |  | 1 |
| Full property | 7 | 647 | 5 | 6 | 118 | 187 | 2 | 145 | 45 | 30 |  | 1 |  |  |  |
| Constant pattern | 6 | 153 | 8 | 140 | 54 | 175 | 358 | 162 | 49 | 19 |  | 16 |  |  |  |
| Null-conditional ?. ?[ | 131 | 192 | 83 | 63 | 57 | 363 | 91 | 43 | 85 | 9 | 7 |  |  |  |  |
| Null-coalescing ?? | 21 | 81 | 12 | 128 | 215 | 73 | 82 | 273 | 53 | 64 | 11 | 5 |  | 1 |  |
| not pattern | 5 | 341 | 1 | 64 | 80 | 156 | 185 | 37 | 57 | 11 | 1 | 2 |  |  |  |
| Auto property | 5 | 385 | 3 | 8 | 7 | 143 | 23 | 246 | 59 | 40 |  |  |  |  |  |
| Null-forgiving ! | 239 | 153 | 279 | 60 | 16 | 9 | 28 | 5 | 4 | 68 | 19 |  |  |  |  |
| await | 263 | 62 | 322 | 62 | 67 | 22 | 1 |  | 24 | 14 | 1 | 25 |  |  |  |
| is null | 9 | 140 | 5 | 104 | 185 | 95 | 174 | 53 | 63 | 18 | 2 | 1 |  |  |  |
| with expression | 298 | 8 | 41 | 46 | 279 | 25 | 10 | 29 | 48 | 35 |  |  |  |  |  |
| Interpolated string | 57 | 19 | 7 | 162 | 58 |  | 384 | 7 |  | 3 | 6 |  |  |  |  |
| Extension method |  |  |  |  |  |  |  |  |  | 676 | 5 |  |  |  |  |
| Nullable value type T? | 35 | 72 | 30 | 43 | 51 | 143 | 11 | 52 | 126 | 80 | 19 |  |  | 2 |  |
| Raw string | 154 | 2 | 7 | 220 |  |  | 235 |  |  | 8 | 5 |  |  |  |  |
| Async method | 201 | 61 | 201 | 32 | 38 | 20 | 1 |  | 20 | 10 | 1 | 13 |  |  |  |
| Constructor | 2 | 231 | 1 | 69 | 84 | 80 | 7 | 3 | 59 | 8 |  | 4 | 38 |  |  |
| Const field | 79 | 75 | 14 | 180 | 13 | 3 | 112 | 73 | 4 | 12 | 6 | 3 |  | 1 |  |
| Record class |  | 18 | 2 | 2 | 114 | 163 | 6 | 235 | 10 | 2 |  |  |  | 4 |  |
| Cast expression | 12 | 128 | 83 | 92 | 4 | 8 | 50 | 4 |  | 47 | 77 | 6 |  |  |  |
| Property pattern | 2 | 172 |  | 10 | 18 | 17 | 274 | 5 | 6 | 4 | 2 |  |  |  |  |
| Object initializer | 63 | 131 | 104 | 29 | 3 |  | 59 | 3 | 1 | 35 | 64 | 2 |  | 1 |  |
| Optional parameter | 14 | 8 | 6 | 6 | 12 | 11 | 1 | 287 | 12 | 122 |  |  |  | 4 |  |
| nameof | 1 | 198 | 3 | 10 | 129 | 30 | 17 | 6 | 5 | 2 | 1 | 4 |  |  |  |
| catch | 1 | 21 |  | 133 | 33 | 131 | 7 | 5 | 26 | 1 | 8 | 13 |  | 3 |  |
| Deconstruction | 76 | 36 | 121 | 31 | 30 | 23 | 30 | 9 | 5 |  | 1 |  |  |  |  |
| lock |  | 14 |  | 2 | 83 | 2 | 5 |  | 245 | 5 |  |  |  |  |  |
| and / or pattern | 3 | 47 | 3 | 68 | 13 | 2 | 161 | 43 | 2 | 8 |  | 1 |  |  |  |
| try | 8 | 21 |  | 82 | 31 | 133 | 9 | 5 | 28 | 3 | 12 | 15 |  | 1 |  |
| Static class | 3 | 43 | 1 | 108 | 23 | 8 | 71 | 23 |  | 63 | 3 |  |  |  |  |
| Named argument | 150 |  | 39 | 16 | 27 |  | 16 | 23 | 3 | 64 |  |  |  |  |  |
| for | 18 | 44 | 6 | 68 | 61 | 9 | 29 | 24 | 7 | 19 | 2 |  |  |  |  |
| Init accessor |  | 2 |  |  |  | 1 | 3 | 223 |  |  |  |  |  |  |  |
| throw expression | 5 | 7 | 30 | 8 | 85 | 29 | 1 | 6 | 11 | 26 | 1 | 8 |  |  |  |
| Spread element | 38 | 11 | 10 | 6 | 46 | 10 | 29 | 39 | 7 | 16 | 2 |  |  |  |  |
| Discard pattern |  | 13 | 1 | 17 | 26 | 33 | 67 | 33 | 10 | 1 |  | 5 |  |  |  |
| Switch expression |  | 11 | 1 | 17 | 26 | 34 | 65 | 33 | 10 | 1 |  | 5 |  |  |  |
| while | 19 | 9 | 5 | 100 | 16 | 3 | 25 | 20 | 1 | 5 |  |  |  |  |  |
| typeof |  | 141 | 5 | 4 |  |  | 3 |  |  | 4 | 38 |  |  |  |  |
| using statement | 36 | 1 | 2 | 121 | 7 | 2 |  |  |  | 1 | 1 | 2 |  |  |  |
| Field-like event |  | 65 |  |  |  | 96 |  |  | 2 |  |  |  |  |  |  |
| Discard _ | 33 | 27 | 49 | 4 | 7 | 3 | 10 | 10 | 6 | 1 | 2 |  |  |  |  |
| Case guard when |  | 6 |  | 1 | 1 | 1 | 100 | 5 |  | 2 |  |  |  |  |  |
| Interface |  | 2 |  |  | 1 | 1 |  | 72 | 28 |  |  |  |  |  |  |
| Index from end ^ | 17 | 5 | 49 | 1 | 4 | 2 | 9 | 8 |  |  |  |  |  |  |  |
| Verbatim string | 9 |  |  |  | 3 |  | 79 |  |  | 2 |  |  |  |  |  |
| Relational pattern |  | 7 |  | 28 | 8 | 2 | 5 | 40 | 1 |  |  |  |  |  |  |
| Range .. | 9 |  | 12 | 15 | 2 |  | 29 | 21 |  | 1 |  |  |  |  |  |
| as cast |  | 56 |  | 3 |  |  | 9 |  |  | 16 | 1 |  |  |  |  |
| Partial type |  |  | 4 |  |  |  | 7 |  |  | 26 |  |  | 38 |  |  |
| Exception filter |  | 3 |  | 33 | 16 |  | 2 |  | 18 |  |  |  |  |  |  |
| Switch statement |  | 8 |  | 8 | 1 | 2 | 20 | 18 | 1 | 2 |  |  |  |  |  |
| is type test | 1 | 21 |  | 2 | 3 |  | 32 |  |  |  |  |  |  |  |  |
| Generic method |  | 25 | 1 |  | 14 | 4 | 1 | 7 | 1 | 5 |  |  |  |  |  |
| Async lambda |  |  | 53 |  | 1 |  |  |  |  | 1 |  |  |  |  |  |
| yield return / break |  | 9 |  | 7 | 9 |  | 25 | 2 |  |  |  |  |  |  |  |
| Local const | 37 |  | 1 | 4 | 1 | 1 | 6 |  | 1 |  |  |  |  |  |  |
| finally | 7 | 3 |  | 2 | 12 | 5 | 2 |  | 5 | 2 | 4 | 6 |  |  |  |
| Enum |  | 2 |  |  |  | 15 |  | 24 |  |  |  |  |  |  |  |
| Nested type | 9 | 7 | 1 |  | 2 | 3 |  |  |  | 10 |  |  |  |  |  |
| var pattern |  |  |  |  |  | 1 | 28 |  |  |  | 1 |  |  |  |  |
| ref or out parameter | 2 | 1 | 1 | 1 | 7 |  | 3 | 5 | 3 | 5 |  |  |  |  |  |
| Null-coalescing assignment ??= | 1 | 10 |  | 1 | 4 | 1 | 3 |  |  |  |  | 1 |  |  |  |
| Constraint clause |  | 12 | 1 |  | 1 |  |  |  |  | 4 |  |  |  |  |  |
| Collection initializer |  | 2 | 3 | 5 | 2 | 1 |  |  |  | 2 | 2 |  |  |  |  |
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
| Object creation | 52.60 % | 48.04 % | 38.05 % | 24.83 % | 30.78 % | 10.00 % | 42.00 % | 27.43 % | 29.32 % | 32.18 % | 38.59 % | 30.56 % | - | 14.29 % | 20.00 % |
| Collection creation | 99.48 % | 97.11 % | 99.20 % | 98.05 % | 99.18 % | 99.49 % | 99.03 % | 99.00 % | 100.00 % | 99.60 % | 92.39 % | 0.00 % | - | - | - |
| Member body | 2.65 % | 25.45 % | 2.53 % | 1.52 % | 10.80 % | 16.26 % | 3.80 % | 28.92 % | 15.50 % | 78.95 % | 23.72 % | 2.63 % | 0.00 % | 0.00 % | - |
| Local type | 0.03 % | 4.04 % | 0.00 % | 0.00 % | 0.00 % | 0.23 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | - | 0.00 % | 0.00 % |
| Using | 98.42 % | 83.33 % | 99.92 % | 83.19 % | 63.16 % | 0.00 % | 100.00 % | - | - | 95.65 % | 98.51 % | 81.82 % | - | - | - |
| Namespace | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - |
| Text building | 29.08 % | 47.50 % | 13.21 % | 46.42 % | 76.32 % | 0.00 % | 81.70 % | 24.14 % | - | 25.00 % | 85.71 % | 0.00 % | - | 0.00 % | - |
| Branch | - | 57.89 % | 100.00 % | 68.00 % | 96.30 % | 94.44 % | 76.47 % | 64.71 % | 90.91 % | 33.33 % | - | 100.00 % | - | - | - |
| Lambda body | 96.73 % | 87.17 % | 75.67 % | 90.24 % | 93.07 % | 96.63 % | 98.92 % | 98.39 % | 91.78 % | 82.67 % | 32.94 % | 85.71 % | - | - | 77.78 % |
| Type test | 100.00 % | 92.25 % | 100.00 % | 93.62 % | 100.00 % | 100.00 % | 94.58 % | 100.00 % | 100.00 % | 44.83 % | 75.00 % | - | - | - | - |
| Constructor | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 1.25 % | 36.36 % | 0.00 % | 0.00 % | 14.29 % | - | 0.00 % | 0.00 % | - | - |
