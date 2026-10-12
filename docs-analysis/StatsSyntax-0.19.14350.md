# Syntax statistics - 0.19.14350

- Generated: 2026-10-13 01:57:52 +09:00
- Source roots: src, tests
- Root-level files: none
- Project segments: 1
- Excluded directories: .git, .vs, artifacts, bin, node_modules, obj, packages, publish
- Excluded suffixes: .g.cs, .g.i.cs, .AssemblyInfo.cs, .GlobalUsings.g.cs, .Designer.cs
- Roslyn: 4.14.0-3.25262.10
- Language version: Preview
- Scanned: 1,998 files, 15 projects, 212,082 lines

## Summary

| Measure | Value |
|---|---:|
| Files | 1,998 |
| Lines | 212,082 |
| Nodes | 1,124,925 |
| Syntax kinds used | 197 |
| Features used | 101 of 138 |
| Minimum C# version | 14 |
| Parse errors | 0 |

## Families

| Family | Features | Used | Count | Per 1k lines |
|---|---:|---:|---:|---:|
| Types | 15 | 12 | 4,152 | 19.58 |
| Namespaces | 6 | 3 | 2,000 | 9.43 |
| Members | 29 | 21 | 9,884 | 46.60 |
| Patterns | 16 | 16 | 6,450 | 30.41 |
| Expressions | 36 | 29 | 27,270 | 128.58 |
| Statements | 20 | 14 | 13,161 | 62.06 |
| Generics | 6 | 2 | 74 | 0.35 |
| Async | 2 | 2 | 653 | 3.08 |
| Nullability | 3 | 2 | 4,145 | 19.54 |
| Unsafe | 2 | 0 | 0 | 0.00 |
| Directives | 3 | 0 | 0 | 0.00 |
| Total | 138 | 101 | 67,789 | 319.64 |

## Projects

| Project | Files | Lines | Nodes | Features used | Newest C# |
|---|---:|---:|---:|---:|---:|
| Llyn.Tests.Engine | 222 | 36,904 | 256,893 | 61 | 12 |
| Llyn.UIDeportment | 313 | 34,132 | 157,345 | 88 | 14 |
| Llyn.Tests.Conduct | 153 | 27,578 | 199,188 | 61 | 12 |
| Llyn.Infrastructure | 184 | 24,390 | 112,092 | 70 | 12 |
| Llyn.Application | 127 | 19,210 | 89,177 | 72 | 12 |
| Llyn.Tests.Convention | 116 | 17,540 | 84,868 | 79 | 12 |
| Llyn.Conduct | 264 | 17,065 | 67,230 | 68 | 12 |
| Llyn.Core | 357 | 12,394 | 51,102 | 60 | 12 |
| Llyn.ShellEngine | 96 | 9,958 | 40,116 | 58 | 12 |
| Llyn.Tests.Interface | 92 | 9,034 | 45,650 | 72 | 12 |
| Llyn.Tests.Windows | 24 | 2,492 | 16,364 | 43 | 12 |
| Llyn.Core.Windows | 7 | 798 | 3,140 | 37 | 10 |
| Llyn.UIVeneer | 37 | 405 | 802 | 4 | 10 |
| Llyn.UIDeportment.Capsule | 5 | 96 | 427 | 15 | 10 |
| Llyn.Host | 1 | 86 | 531 | 4 | 9 |
| Total | 1,998 | 212,082 | 1,124,925 | 101 | 14 |

## Versions

| C# version | Features | Used | Count |
|---|---:|---:|---:|
| 1 | 51 | 35 | 17,669 |
| 2 | 10 | 8 | 2,119 |
| 3 | 7 | 5 | 6,698 |
| 4 | 3 | 2 | 792 |
| 5 | 3 | 3 | 1,516 |
| 6 | 6 | 5 | 4,947 |
| 7 | 11 | 10 | 6,410 |
| 7.1 | 1 | 1 | 8 |
| 7.2 | 1 | 0 | 0 |
| 7.3 | 1 | 0 | 0 |
| 8 | 17 | 13 | 11,108 |
| 9 | 10 | 9 | 6,451 |
| 10 | 3 | 2 | 2,000 |
| 11 | 6 | 3 | 647 |
| 12 | 4 | 4 | 7,420 |
| 13 | 1 | 0 | 0 |
| 14 | 3 | 1 | 4 |

## Features

| Feature | Family | C# | Count | Per 1k lines | Files | First |
|---|---|---|---:|---:|---:|---|
| Collection expression | Expressions | 12 | 7,215 | 34.02 | 901 | src/Llyn.Application/Card/LCardClerk.cs:60 |
| using declaration | Statements | 8 | 5,540 | 26.12 | 420 | src/Llyn.Application/Card/LTranslationClerk.cs:160 |
| Lambda | Expressions | 3 | 4,605 | 21.71 | 730 | src/Llyn.Application/Card/LCardClerk.cs:95 |
| if | Statements | 1 | 4,176 | 19.69 | 701 | src/Llyn.Application/Card/LCardClerk.cs:73 |
| Nullable reference type T? | Nullability | 8 | 3,560 | 16.79 | 978 | src/Llyn.Application/Card/LCardClerkField.cs:91 |
| Expression-bodied member | Members | 6 | 2,678 | 12.63 | 503 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Target-typed new | Expressions | 9 | 2,261 | 10.66 | 550 | src/Llyn.Application/Card/LMentionClerk.cs:167 |
| Readonly field | Members | 1 | 2,014 | 9.50 | 612 | src/Llyn.Application/Card/LCardClerk.cs:10 |
| File-scoped namespace | Namespaces | 10 | 1,997 | 9.42 | 1,997 | src/Llyn.Application/Card/LCardClerk.cs:6 |
| Declaration pattern | Patterns | 7 | 1,946 | 9.18 | 392 | src/Llyn.Application/Card/LMeaningClerk.cs:105 |
| Tuple literal or tuple type | Expressions | 7 | 1,654 | 7.80 | 278 | src/Llyn.Application/Card/LMeaningClerk.cs:26 |
| Sealed class | Types | 1 | 1,578 | 7.44 | 1,470 | src/Llyn.Application/Card/LCardClerk.cs:8 |
| foreach | Statements | 1 | 1,482 | 6.99 | 479 | src/Llyn.Application/Card/LCardClerk.cs:62 |
| Conditional ?: | Expressions | 1 | 1,433 | 6.76 | 534 | src/Llyn.Application/Card/LCardClerk.cs:170 |
| Class | Types | 1 | 1,418 | 6.69 | 1,401 | src/Llyn.Application/Card/LCardClerk.cs:8 |
| Static lambda | Expressions | 9 | 1,275 | 6.01 | 306 | src/Llyn.Application/Card/LCardClerk.cs:95 |
| Full property | Members | 1 | 1,198 | 5.65 | 356 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Constant pattern | Patterns | 7 | 1,120 | 5.28 | 202 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Null-conditional ?. ?[ | Expressions | 6 | 1,097 | 5.17 | 393 | src/Llyn.Application/Card/LMentionClerk.cs:47 |
| Null-coalescing ?? | Expressions | 2 | 984 | 4.64 | 395 | src/Llyn.Application/Card/LCardClerkField.cs:74 |
| not pattern | Patterns | 9 | 923 | 4.35 | 381 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| Auto property | Members | 3 | 910 | 4.29 | 267 | src/Llyn.Application/Citation/LCitationClerk.cs:41 |
| Null-forgiving ! | Expressions | 8 | 875 | 4.13 | 317 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:124 |
| await | Expressions | 5 | 863 | 4.07 | 173 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:139 |
| is null | Patterns | 7 | 831 | 3.92 | 360 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| with expression | Expressions | 9 | 775 | 3.65 | 257 | src/Llyn.Application/Card/LCardClerk.cs:115 |
| Interpolated string | Expressions | 6 | 690 | 3.25 | 165 | src/Llyn.Application/Localization/LLocalizationReader.cs:55 |
| Extension method | Members | 3 | 672 | 3.17 | 42 | tests/Llyn.Tests.Interface/TInterfaceArea.cs:7 |
| Raw string | Expressions | 11 | 628 | 2.96 | 142 | src/Llyn.Infrastructure/Database/Base/LDatabase.cs:51 |
| Async method | Async | 5 | 598 | 2.82 | 173 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:91 |
| Nullable value type T? | Nullability | 2 | 585 | 2.76 | 255 | src/Llyn.Application/Card/LMeaningClerk.cs:104 |
| Constructor | Members | 1 | 580 | 2.73 | 575 | src/Llyn.Application/Card/LCardClerk.cs:21 |
| Const field | Members | 1 | 569 | 2.68 | 247 | src/Llyn.Application/Card/LMentionClerk.cs:11 |
| Record class | Types | 9 | 548 | 2.58 | 454 | src/Llyn.Application/Ensign/LEnsignRow.cs:3 |
| Cast expression | Expressions | 1 | 499 | 2.35 | 168 | src/Llyn.Application/Markup/LMarkupClerkLink.cs:285 |
| Property pattern | Patterns | 8 | 492 | 2.32 | 129 | src/Llyn.Application/Draft/Clerk/LDraftClerk.cs:82 |
| Object initializer | Expressions | 3 | 491 | 2.32 | 206 | src/Llyn.Application/Portrait/LPortraitClerkLabel.cs:25 |
| Optional parameter | Members | 4 | 464 | 2.19 | 155 | src/Llyn.Application/Draft/Clerk/LDraftClerkPanel.cs:170 |
| nameof | Expressions | 6 | 408 | 1.92 | 142 | src/Llyn.Application/Citation/LAuthorClerk.cs:361 |
| catch | Statements | 1 | 381 | 1.80 | 167 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:142 |
| lock | Statements | 1 | 351 | 1.66 | 58 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:98 |
| Static class | Types | 2 | 346 | 1.63 | 346 | src/Llyn.Application/Card/LCardClerkField.cs:7 |
| try | Statements | 1 | 346 | 1.63 | 183 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:137 |
| and / or pattern | Patterns | 9 | 344 | 1.62 | 104 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Deconstruction | Expressions | 7 | 336 | 1.58 | 145 | src/Llyn.Application/Card/LMeaningClerk.cs:46 |
| Named argument | Members | 4 | 328 | 1.55 | 99 | src/Llyn.Application/Card/LTranslationClerk.cs:164 |
| for | Statements | 1 | 280 | 1.32 | 174 | src/Llyn.Application/Card/LCardClerk.cs:280 |
| Init accessor | Members | 9 | 225 | 1.06 | 78 | src/Llyn.Conduct/Lexicon/CExample.cs:20 |
| typeof | Expressions | 1 | 217 | 1.02 | 46 | src/Llyn.Infrastructure/Configuration/LLocalizationLoader.cs:20 |
| throw expression | Expressions | 7 | 209 | 0.99 | 103 | src/Llyn.Application/Card/LCardClerkField.cs:75 |
| Discard pattern | Patterns | 8 | 205 | 0.97 | 117 | src/Llyn.Application/Citation/LAuthorClerk.cs:243 |
| Switch expression | Patterns | 8 | 202 | 0.95 | 118 | src/Llyn.Application/Citation/LAuthorClerk.cs:239 |
| Spread element | Expressions | 12 | 198 | 0.93 | 109 | src/Llyn.Application/Card/LCardClerk.cs:95 |
| while | Statements | 1 | 194 | 0.91 | 116 | src/Llyn.Application/Card/LMeaningClerk.cs:44 |
| using statement | Statements | 1 | 173 | 0.82 | 74 | src/Llyn.Application/Citation/LAuthorCitation.cs:51 |
| Field-like event | Members | 1 | 159 | 0.75 | 95 | src/Llyn.Conduct/Atelier/CNavigation.cs:46 |
| Discard _ | Expressions | 7 | 152 | 0.72 | 59 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:127 |
| Case guard when | Patterns | 7 | 115 | 0.54 | 24 | src/Llyn.Application/Citation/LAuthorClerk.cs:241 |
| Interface | Types | 1 | 103 | 0.49 | 103 | src/Llyn.Application/Pronunciation/Relay/LListener.cs:5 |
| Verbatim string | Expressions | 1 | 91 | 0.43 | 19 | src/Llyn.Application/Localization/LLocalizationReader.cs:13 |
| Range .. | Expressions | 8 | 90 | 0.42 | 47 | src/Llyn.Application/Card/LMentionClerk.cs:173 |
| Relational pattern | Patterns | 9 | 89 | 0.42 | 30 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Index from end ^ | Expressions | 8 | 87 | 0.41 | 49 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:230 |
| as cast | Expressions | 1 | 86 | 0.41 | 45 | src/Llyn.Infrastructure/Database/Base/LWorkspaceArchive.cs:132 |
| Exception filter | Statements | 6 | 74 | 0.35 | 48 | src/Llyn.Application/Outpost/LCourierClerk.cs:196 |
| Partial type | Types | 2 | 74 | 0.35 | 74 | src/Llyn.UIVeneer/Display/View/PDisplay.xaml.cs:5 |
| Switch statement | Patterns | 1 | 60 | 0.28 | 37 | src/Llyn.Application/Portrait/LPortraitClerkPress.cs:59 |
| is type test | Patterns | 1 | 59 | 0.28 | 36 | src/Llyn.Application/Outpost/LCourierClerk.cs:288 |
| Generic method | Generics | 2 | 56 | 0.26 | 30 | src/Llyn.Application/Card/LCardClerkField.cs:183 |
| Async lambda | Async | 5 | 55 | 0.26 | 10 | src/Llyn.Application/Outpost/LCourierClerk.cs:251 |
| yield return / break | Statements | 2 | 52 | 0.25 | 21 | src/Llyn.Application/Card/LCardClerkField.cs:35 |
| Local const | Statements | 1 | 51 | 0.24 | 27 | src/Llyn.Application/Markup/LMarkupClerkIntake.cs:271 |
| finally | Statements | 1 | 47 | 0.22 | 33 | src/Llyn.Application/Outpost/LCourierClerk.cs:76 |
| Enum | Types | 1 | 41 | 0.19 | 41 | src/Llyn.Conduct/Catalog/CCatalogOrder.cs:3 |
| var pattern | Patterns | 7 | 30 | 0.14 | 12 | src/Llyn.Conduct/Card/CFolio.cs:213 |
| Nested type | Types | 1 | 29 | 0.14 | 16 | src/Llyn.Conduct/Dialog/CSLens.cs:110 |
| ref or out parameter | Members | 1 | 29 | 0.14 | 23 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:162 |
| Null-coalescing assignment ??= | Expressions | 8 | 21 | 0.10 | 19 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:160 |
| Collection initializer | Expressions | 3 | 20 | 0.09 | 16 | src/Llyn.Application/Outpost/LCourierClerk.cs:177 |
| Constraint clause | Generics | 2 | 18 | 0.08 | 10 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:171 |
| Local function | Members | 7 | 17 | 0.08 | 11 | src/Llyn.Conduct/Desk/CErrand.cs:55 |
| await using | Statements | 8 | 14 | 0.07 | 7 | src/Llyn.Conduct/Portrait/CPortrait.cs:144 |
| List pattern | Patterns | 11 | 14 | 0.07 | 12 | src/Llyn.ShellEngine/Engine/LDraftFacade.cs:70 |
| Conversion operator | Members | 1 | 13 | 0.06 | 2 | src/Llyn.Core/Lexicon/State/LStateValue.cs:49 |
| Default interface method body | Members | 8 | 11 | 0.05 | 6 | src/Llyn.ShellEngine/Port/LDraftPort.cs:58 |
| Type pattern | Patterns | 9 | 11 | 0.05 | 4 | src/Llyn.UIDeportment/Sound/Accent/QAccentControl.cs:62 |
| Positional pattern | Patterns | 8 | 9 | 0.04 | 9 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:37 |
| default literal | Expressions | 7.1 | 8 | 0.04 | 6 | src/Llyn.Infrastructure/Pronunciation/Loader/LSourceLoader.cs:73 |
| Primary constructor on class or struct | Types | 12 | 6 | 0.03 | 6 | src/Llyn.Conduct/Dialog/CSGalleyDelay.cs:7 |
| Required member | Members | 11 | 5 | 0.02 | 3 | src/Llyn.UIDeportment/Panel/Taxonomy/QMembershipItem.cs:27 |
| field keyword in accessor | Members | 14 | 4 | 0.02 | 1 | src/Llyn.UIDeportment/Panel/Dialog/QSGalleyItem.cs:31 |
| Generic type | Types | 2 | 4 | 0.02 | 4 | src/Llyn.Conduct/Configuration/CEnsignSheet.cs:5 |
| Event accessors | Members | 1 | 3 | 0.01 | 2 | src/Llyn.UIDeportment/Display/View/PGrasp.cs:81 |
| Record struct | Types | 10 | 3 | 0.01 | 3 | src/Llyn.Core/Lexicon/Inflection/LInflectionLetter.cs:3 |
| Abstract class | Types | 1 | 2 | 0.01 | 2 | src/Llyn.Application/Request/Entry/LRequest.cs:3 |
| checked / unchecked expression | Expressions | 1 | 2 | 0.01 | 2 | src/Llyn.Core.Windows/LPressBrowser.cs:14 |
| Static constructor | Members | 1 | 2 | 0.01 | 2 | src/Llyn.UIDeportment/Kit/Bind/QLookItem.cs:23 |
| Static local function | Members | 8 | 2 | 0.01 | 1 | src/Llyn.UIDeportment/Kit/Asset/QIcon.cs:144 |
| Using alias | Namespaces | 1 | 2 | 0.01 | 2 | src/Llyn.Host/LHost.cs:11 |
| Alias of any type | Namespaces | 12 | 1 | 0.00 | 1 | tests/Llyn.Tests.Interface/TInterfaceInflection.cs:2 |
| Indexer | Members | 1 | 1 | 0.00 | 1 | src/Llyn.UIDeportment/Kit/Asset/QLocalizationCatalog.cs:19 |

## Styles

| Style | A | A count | B | B count | A share |
|---|---|---:|---|---:|---:|
| Null check | is null / is not null | 831 | == null / != null | 0 | 100.00 % |
| Object creation | Target-typed new() | 2,261 | new T() | 4,125 | 35.41 % |
| Collection creation | Collection expression | 7,215 | Collection initializer or array creation | 73 | 99.00 % |
| Member body | Expression body | 2,678 | Block body | 10,478 | 20.36 % |
| Local type | var | 44 | Explicit type | 21,892 | 0.20 % |
| Using | Declaration | 5,540 | Statement | 173 | 96.97 % |
| Namespace | File-scoped | 1,997 | Block | 0 | 100.00 % |
| Text building | Interpolated string | 690 | string.Format or + with a string literal | 555 | 55.42 % |
| Branch | Switch expression | 202 | Switch statement | 60 | 77.10 % |
| Lambda body | Expression | 4,027 | Block | 578 | 87.45 % |
| Type test | is pattern with designation | 1,116 | as | 86 | 92.85 % |
| Constructor | Primary | 6 | Explicit | 575 | 1.03 % |

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
| IdentifierName | 325,414 | 1,998 |
| Argument | 117,383 | 1,518 |
| SimpleMemberAccessExpression | 107,412 | 1,498 |
| ArgumentList | 72,627 | 1,555 |
| InvocationExpression | 66,408 | 1,514 |
| ExpressionStatement | 31,878 | 1,395 |
| PredefinedType | 31,650 | 1,854 |
| StringLiteralExpression | 26,384 | 1,119 |
| Parameter | 23,091 | 1,799 |
| VariableDeclaration | 22,643 | 1,318 |
| VariableDeclarator | 22,643 | 1,318 |
| EqualsValueClause | 21,555 | 1,369 |
| LocalDeclarationStatement | 19,079 | 1,138 |
| Block | 18,598 | 1,478 |
| ParameterList | 13,997 | 1,917 |
| NumericLiteralExpression | 11,832 | 1,097 |
| MethodDeclaration | 11,811 | 1,540 |
| GenericName | 10,873 | 1,381 |
| TypeArgumentList | 10,873 | 1,381 |
| QualifiedName | 9,477 | 1,998 |
| ExpressionElement | 7,953 | 487 |
| SimpleAssignmentExpression | 7,661 | 1,033 |
| CollectionExpression | 7,215 | 901 |
| UsingDirective | 6,417 | 1,714 |
| ReturnStatement | 6,078 | 1,098 |
| NullLiteralExpression | 4,232 | 912 |
| IfStatement | 4,176 | 701 |
| NullableType | 4,145 | 1,029 |
| ObjectCreationExpression | 4,125 | 905 |
| Attribute | 3,660 | 430 |
| AttributeList | 3,660 | 430 |
| SimpleLambdaExpression | 3,587 | 654 |
| SingleVariableDesignation | 3,310 | 580 |
| BracketedArgumentList | 3,197 | 555 |
| FieldDeclaration | 2,961 | 771 |
| IsPatternExpression | 2,781 | 595 |
| ArrowExpressionClause | 2,680 | 504 |
| ElementAccessExpression | 2,587 | 505 |
| ImplicitObjectCreationExpression | 2,261 | 550 |
| FalseLiteralExpression | 2,118 | 570 |
| PropertyDeclaration | 2,114 | 518 |
| CompilationUnit | 1,998 | 1,998 |
| FileScopedNamespaceDeclaration | 1,997 | 1,997 |
| ConstantPattern | 1,951 | 456 |
| DeclarationPattern | 1,946 | 392 |
| TrueLiteralExpression | 1,815 | 526 |
| TupleExpression | 1,623 | 277 |
| EqualsExpression | 1,612 | 582 |
| AddExpression | 1,583 | 362 |
| InterpolatedStringText | 1,539 | 165 |
| ConditionalExpression | 1,433 | 534 |
| LogicalAndExpression | 1,423 | 403 |
| ClassDeclaration | 1,418 | 1,401 |
| ForEachStatement | 1,394 | 461 |
| Interpolation | 1,360 | 164 |
| AttributeArgument | 1,242 | 64 |
| LogicalNotExpression | 1,216 | 419 |
| AddAssignmentExpression | 1,189 | 277 |
| ConditionalAccessExpression | 1,097 | 393 |
| MemberBindingExpression | 1,097 | 393 |
| DeclarationExpression | 1,070 | 278 |
| SwitchExpressionArm | 1,047 | 118 |
| AccessorList | 1,022 | 293 |
| GetAccessorDeclaration | 1,018 | 292 |
| ParenthesizedLambdaExpression | 1,018 | 292 |
| LogicalOrExpression | 985 | 315 |
| CoalesceExpression | 984 | 395 |
| NotPattern | 923 | 381 |
| TupleElement | 877 | 202 |
| SuppressNullableWarningExpression | 875 | 317 |
| AwaitExpression | 863 | 173 |
| WithExpression | 775 | 257 |
| WithInitializerExpression | 775 | 257 |
| GreaterThanExpression | 738 | 316 |
| NameColon | 719 | 210 |
| InterpolatedStringExpression | 690 | 165 |
| ArrayRankSpecifier | 665 | 265 |
| ArrayType | 661 | 265 |
| OmittedArraySizeExpression | 643 | 265 |
| ImplicitElementAccess | 610 | 117 |
| ConstructorDeclaration | 582 | 577 |
| PostIncrementExpression | 549 | 251 |
| RecordDeclaration | 548 | 454 |
| LessThanExpression | 532 | 237 |
| CharacterLiteralExpression | 523 | 126 |
| AttributeArgumentList | 507 | 64 |
| RecursivePattern | 501 | 136 |
| CastExpression | 499 | 168 |
| PropertyPatternClause | 492 | 129 |
| ObjectInitializerExpression | 491 | 206 |
| NotEqualsExpression | 463 | 196 |
| Subpattern | 441 | 120 |
| ParenthesizedExpression | 407 | 197 |
| BaseList | 384 | 287 |
| CatchClause | 381 | 167 |
| CatchDeclaration | 378 | 165 |
| TupleType | 368 | 202 |
| LockStatement | 351 | 58 |
| TryStatement | 346 | 183 |
| ContinueStatement | 335 | 161 |
| SwitchSection | 330 | 37 |
| BreakStatement | 314 | 63 |
| OrPattern | 304 | 98 |
| SimpleBaseType | 289 | 268 |
| ForStatement | 280 | 174 |
| EnumMemberDeclaration | 247 | 41 |
| InitAccessorDeclaration | 225 | 78 |
| SubtractExpression | 220 | 100 |
| TypeOfExpression | 217 | 46 |
| ThrowExpression | 209 | 103 |
| DiscardPattern | 205 | 117 |
| ThrowStatement | 205 | 106 |
| SwitchExpression | 202 | 118 |
| SpreadElement | 198 | 109 |
| WhileStatement | 194 | 116 |
| CaseSwitchLabel | 180 | 24 |
| UnaryMinusExpression | 177 | 78 |
| UsingStatement | 173 | 74 |
| SubtractAssignmentExpression | 171 | 70 |
| EventFieldDeclaration | 159 | 95 |
| ElseClause | 157 | 121 |
| SetAccessorDeclaration | 155 | 76 |
| LessThanOrEqualExpression | 127 | 91 |
| ThisExpression | 124 | 83 |
| GreaterThanOrEqualExpression | 123 | 77 |
| WhenClause | 115 | 24 |
| CasePatternSwitchLabel | 112 | 22 |
| PrimaryConstructorBaseType | 106 | 19 |
| MultiplyExpression | 105 | 45 |
| InterfaceDeclaration | 103 | 103 |
| RangeExpression | 90 | 47 |
| RelationalPattern | 89 | 30 |
| ForEachVariableStatement | 88 | 65 |
| IndexExpression | 87 | 49 |
| AsExpression | 86 | 45 |
| CatchFilterClause | 74 | 48 |
| ArrayInitializerExpression | 72 | 38 |
| TypeParameter | 63 | 34 |
| SwitchStatement | 60 | 37 |
| TypeParameterList | 60 | 34 |
| IsExpression | 59 | 36 |
| DivideExpression | 55 | 28 |
| ImplicitArrayCreationExpression | 50 | 35 |
| BitwiseOrExpression | 48 | 25 |
| FinallyClause | 47 | 33 |
| YieldReturnStatement | 45 | 21 |
| DefaultSwitchLabel | 44 | 25 |
| ParenthesizedPattern | 42 | 20 |
| EnumDeclaration | 41 | 41 |
| AndPattern | 40 | 17 |
| OrAssignmentExpression | 35 | 20 |
| ArrayCreationExpression | 34 | 23 |
| ExpressionColon | 34 | 15 |
| LeftShiftExpression | 32 | 2 |
| VarPattern | 30 | 12 |
| CoalesceAssignmentExpression | 21 | 19 |
| CollectionInitializerExpression | 20 | 16 |
| AttributeTargetSpecifier | 19 | 8 |
| TypeParameterConstraintClause | 18 | 10 |
| ClassConstraint | 17 | 9 |
| LocalFunctionStatement | 17 | 11 |
| ListPattern | 14 | 12 |
| BaseExpression | 13 | 5 |
| ConversionOperatorDeclaration | 13 | 2 |
| PostDecrementExpression | 11 | 9 |
| TypePattern | 11 | 4 |
| PositionalPatternClause | 9 | 9 |
| DefaultLiteralExpression | 8 | 6 |
| NameEquals | 8 | 5 |
| InterpolationFormatClause | 7 | 1 |
| YieldBreakStatement | 7 | 6 |
| BaseConstructorInitializer | 6 | 6 |
| ExplicitInterfaceSpecifier | 6 | 5 |
| GlobalStatement | 6 | 1 |
| FieldExpression | 4 | 1 |
| AddAccessorDeclaration | 3 | 2 |
| AndAssignmentExpression | 3 | 3 |
| BitwiseAndExpression | 3 | 3 |
| EventDeclaration | 3 | 2 |
| InterpolationAlignmentClause | 3 | 1 |
| PreIncrementExpression | 3 | 3 |
| RecordStructDeclaration | 3 | 3 |
| RemoveAccessorDeclaration | 3 | 2 |
| SlicePattern | 3 | 3 |
| ComplexElementInitializerExpression | 2 | 1 |
| ModuloExpression | 2 | 1 |
| RightShiftExpression | 2 | 1 |
| UncheckedExpression | 2 | 2 |
| BitwiseNotExpression | 1 | 1 |
| BracketedParameterList | 1 | 1 |
| DiscardDesignation | 1 | 1 |
| ExclusiveOrAssignmentExpression | 1 | 1 |
| ExclusiveOrExpression | 1 | 1 |
| IndexerDeclaration | 1 | 1 |
| OmittedTypeArgument | 1 | 1 |
| ThisConstructorInitializer | 1 | 1 |
| TypeConstraint | 1 | 1 |

## Attributes

| Attribute | Count | Files |
|---|---:|---:|
| Fact | 3,049 | 414 |
| InlineData | 479 | 52 |
| Theory | 102 | 54 |
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

| Feature | Llyn.Tests.Engine | Llyn.UIDeportment | Llyn.Tests.Conduct | Llyn.Infrastructure | Llyn.Application | Llyn.Tests.Convention | Llyn.Conduct | Llyn.Core | Llyn.ShellEngine | Llyn.Tests.Interface | Llyn.Tests.Windows | Llyn.Core.Windows | Llyn.UIVeneer | Llyn.UIDeportment.Capsule | Llyn.Host |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| Collection expression | 3,061 | 163 | 1,838 | 300 | 359 | 523 | 177 | 304 | 80 | 245 | 165 |  |  |  |  |
| using declaration | 2,228 | 5 | 2,594 | 598 | 12 | 7 |  |  |  | 22 | 65 | 9 |  |  |  |
| Lambda | 893 | 207 | 1,458 | 35 | 376 | 729 | 366 | 188 | 71 | 187 | 79 | 7 |  |  | 9 |
| if | 54 | 1,283 | 5 | 714 | 728 | 464 | 379 | 271 | 222 | 38 | 4 | 11 |  | 3 |  |
| Nullable reference type T? | 178 | 483 | 376 | 365 | 376 | 297 | 344 | 369 | 264 | 451 | 43 | 9 |  | 5 |  |
| Expression-bodied member | 42 | 708 | 37 | 16 | 113 | 27 | 199 | 170 | 128 | 1,198 | 39 | 1 |  |  |  |
| Target-typed new | 189 | 749 | 261 | 144 | 165 | 190 | 45 | 62 | 87 | 269 | 84 | 11 |  | 1 | 4 |
| Readonly field | 40 | 631 | 4 | 123 | 406 | 216 | 337 | 31 | 177 | 36 | 3 | 9 |  | 1 |  |
| File-scoped namespace | 222 | 313 | 153 | 184 | 127 | 116 | 264 | 357 | 96 | 92 | 24 | 7 | 37 | 5 |  |
| Declaration pattern | 7 | 966 | 4 | 59 | 201 | 368 | 221 | 29 | 77 | 9 | 3 | 2 |  |  |  |
| Tuple literal or tuple type | 438 | 47 | 288 | 361 | 93 | 168 | 65 | 58 | 76 | 30 | 30 |  |  |  |  |
| Sealed class | 227 | 270 | 153 | 77 | 189 | 43 | 243 | 237 | 69 | 37 | 21 | 7 |  | 5 |  |
| foreach | 56 | 199 | 17 | 300 | 332 | 306 | 27 | 167 | 32 | 42 | 2 | 2 |  |  |  |
| Conditional ?: | 15 | 219 | 18 | 315 | 269 | 156 | 166 | 163 | 77 | 30 | 3 | 2 |  |  |  |
| Class | 230 | 297 | 152 | 182 | 105 | 109 | 89 | 26 | 59 | 100 | 24 | 7 | 37 | 1 |  |
| Static lambda | 112 | 94 | 663 | 8 | 125 | 15 | 72 | 100 | 15 | 64 | 6 |  |  |  | 1 |
| Full property | 7 | 654 | 5 | 6 | 114 | 2 | 185 | 151 | 44 | 29 |  | 1 |  |  |  |
| Constant pattern | 6 | 149 | 8 | 140 | 53 | 354 | 175 | 151 | 49 | 19 |  | 16 |  |  |  |
| Null-conditional ?. ?[ | 131 | 187 | 84 | 65 | 53 | 91 | 342 | 46 | 81 | 9 | 8 |  |  |  |  |
| Null-coalescing ?? | 19 | 76 | 12 | 123 | 206 | 80 | 68 | 270 | 51 | 63 | 10 | 5 |  | 1 |  |
| not pattern | 4 | 340 | 1 | 63 | 76 | 186 | 147 | 38 | 56 | 9 | 1 | 2 |  |  |  |
| Auto property | 5 | 389 | 3 | 8 | 7 | 23 | 135 | 242 | 58 | 40 |  |  |  |  |  |
| Null-forgiving ! | 241 | 150 | 278 | 62 | 16 | 25 | 9 | 3 | 4 | 68 | 19 |  |  |  |  |
| await | 263 | 62 | 322 | 62 | 67 | 1 | 22 |  | 24 | 14 | 1 | 25 |  |  |  |
| is null | 8 | 139 | 5 | 100 | 177 | 175 | 92 | 53 | 62 | 17 | 2 | 1 |  |  |  |
| with expression | 278 | 8 | 40 | 45 | 269 | 10 | 25 | 21 | 48 | 31 |  |  |  |  |  |
| Interpolated string | 56 | 19 | 7 | 162 | 56 | 380 |  | 7 |  | 3 |  |  |  |  |  |
| Extension method |  |  |  |  |  |  |  |  |  | 665 | 7 |  |  |  |  |
| Raw string | 155 | 2 | 7 | 221 |  | 235 |  |  |  | 8 |  |  |  |  |  |
| Async method | 201 | 61 | 201 | 32 | 38 | 1 | 20 |  | 20 | 10 | 1 | 13 |  |  |  |
| Nullable value type T? | 25 | 56 | 20 | 42 | 38 | 11 | 134 | 39 | 122 | 73 | 23 |  |  | 2 |  |
| Constructor | 2 | 233 | 1 | 69 | 81 | 7 | 77 | 3 | 58 | 8 |  | 4 | 37 |  |  |
| Const field | 79 | 75 | 14 | 180 | 12 | 113 | 3 | 73 | 4 | 12 |  | 3 |  | 1 |  |
| Record class |  | 18 | 2 | 2 | 108 | 6 | 162 | 234 | 10 | 2 |  |  |  | 4 |  |
| Cast expression | 9 | 139 | 79 | 89 | 4 | 50 | 8 | 4 |  | 47 | 64 | 6 |  |  |  |
| Property pattern | 2 | 160 |  | 10 | 17 | 271 | 15 | 5 | 6 | 4 | 2 |  |  |  |  |
| Object initializer | 63 | 129 | 104 | 29 | 3 | 59 |  | 3 | 1 | 35 | 62 | 2 |  | 1 |  |
| Optional parameter | 14 | 8 | 6 | 6 | 12 | 1 | 10 | 273 | 12 | 118 |  |  |  | 4 |  |
| nameof | 1 | 204 | 3 | 10 | 125 | 17 | 30 | 6 | 5 | 2 | 1 | 4 |  |  |  |
| catch | 1 | 21 |  | 133 | 33 | 8 | 127 | 5 | 27 | 1 | 9 | 13 |  | 3 |  |
| lock |  | 14 |  | 2 | 83 | 5 | 2 |  | 240 | 5 |  |  |  |  |  |
| Static class | 3 | 44 | 1 | 107 | 23 | 72 | 8 | 23 |  | 62 | 3 |  |  |  |  |
| try | 8 | 21 |  | 82 | 31 | 10 | 129 | 5 | 29 | 3 | 12 | 15 |  | 1 |  |
| and / or pattern | 3 | 46 | 3 | 68 | 13 | 159 | 2 | 39 | 2 | 8 |  | 1 |  |  |  |
| Deconstruction | 67 | 32 | 113 | 31 | 28 | 32 | 19 | 9 | 5 |  |  |  |  |  |  |
| Named argument | 149 |  | 39 | 16 | 23 | 16 |  | 23 | 3 | 59 |  |  |  |  |  |
| for | 16 | 45 | 6 | 68 | 59 | 27 | 7 | 24 | 7 | 19 | 2 |  |  |  |  |
| Init accessor |  | 2 |  |  |  | 3 | 1 | 219 |  |  |  |  |  |  |  |
| typeof |  | 163 | 5 | 4 |  | 3 |  |  |  | 4 | 38 |  |  |  |  |
| throw expression | 4 | 4 | 30 | 8 | 81 | 1 | 29 | 6 | 11 | 26 | 1 | 8 |  |  |  |
| Discard pattern |  | 13 | 1 | 17 | 26 | 67 | 33 | 32 | 10 | 1 |  | 5 |  |  |  |
| Switch expression |  | 11 | 1 | 17 | 26 | 65 | 34 | 32 | 10 | 1 |  | 5 |  |  |  |
| Spread element | 34 | 9 | 9 | 6 | 45 | 25 | 6 | 39 | 8 | 16 | 1 |  |  |  |  |
| while | 18 | 9 | 5 | 98 | 11 | 25 | 2 | 20 | 1 | 5 |  |  |  |  |  |
| using statement | 36 | 1 | 2 | 121 | 7 |  | 2 |  |  | 1 | 1 | 2 |  |  |  |
| Field-like event |  | 62 |  |  |  |  | 95 |  | 2 |  |  |  |  |  |  |
| Discard _ | 24 | 27 | 59 | 4 | 7 | 9 | 3 | 10 | 6 | 1 | 2 |  |  |  |  |
| Case guard when |  | 6 |  | 1 | 1 | 100 | 1 | 4 |  | 2 |  |  |  |  |  |
| Interface |  | 2 |  |  | 1 |  | 1 | 72 | 27 |  |  |  |  |  |  |
| Verbatim string | 9 |  |  |  | 3 | 77 |  |  |  | 2 |  |  |  |  |  |
| Range .. | 9 |  | 12 | 15 | 2 | 30 |  | 21 |  | 1 |  |  |  |  |  |
| Relational pattern |  | 7 |  | 28 | 8 | 3 | 2 | 40 | 1 |  |  |  |  |  |  |
| Index from end ^ | 16 | 3 | 42 | 1 | 4 | 11 | 2 | 8 |  |  |  |  |  |  |  |
| as cast |  | 58 |  | 3 |  | 9 |  |  |  | 15 | 1 |  |  |  |  |
| Exception filter |  | 3 |  | 33 | 16 | 3 |  |  | 19 |  |  |  |  |  |  |
| Partial type |  |  | 4 |  |  | 7 |  |  |  | 26 |  |  | 37 |  |  |
| Switch statement |  | 7 |  | 8 | 1 | 20 | 2 | 19 | 1 | 2 |  |  |  |  |  |
| is type test | 1 | 21 |  | 2 | 3 | 32 |  |  |  |  |  |  |  |  |  |
| Generic method |  | 23 | 1 |  | 14 | 1 | 4 | 7 | 1 | 5 |  |  |  |  |  |
| Async lambda |  |  | 53 |  | 1 |  |  |  |  | 1 |  |  |  |  |  |
| yield return / break |  | 9 |  | 7 | 9 | 25 |  | 2 |  |  |  |  |  |  |  |
| Local const | 37 |  | 1 | 4 | 1 | 6 | 1 |  | 1 |  |  |  |  |  |  |
| finally | 7 | 3 |  | 2 | 12 | 2 | 5 |  | 5 | 2 | 3 | 6 |  |  |  |
| Enum |  | 2 |  |  |  |  | 15 | 24 |  |  |  |  |  |  |  |
| var pattern |  |  |  |  |  | 28 | 1 |  |  |  | 1 |  |  |  |  |
| Nested type | 8 | 7 | 1 |  |  |  | 3 |  |  | 10 |  |  |  |  |  |
| ref or out parameter | 3 | 1 | 1 | 1 | 7 | 3 |  | 5 | 3 | 5 |  |  |  |  |  |
| Null-coalescing assignment ??= | 1 | 10 |  | 1 | 4 | 3 | 1 |  |  |  |  | 1 |  |  |  |
| Collection initializer |  | 3 | 3 | 5 | 2 |  | 1 |  |  | 2 | 4 |  |  |  |  |
| Constraint clause |  | 12 | 1 |  | 1 |  |  |  |  | 4 |  |  |  |  |  |
| Local function | 1 | 4 |  |  |  | 4 | 5 |  |  | 1 | 1 | 1 |  |  |  |
| await using | 12 |  |  |  |  |  | 1 |  |  |  |  | 1 |  |  |  |
| List pattern | 1 | 4 |  |  |  | 8 |  |  | 1 |  |  |  |  |  |  |
| Conversion operator |  | 12 |  |  |  |  |  | 1 |  |  |  |  |  |  |  |
| Default interface method body |  |  |  |  |  |  |  |  | 11 |  |  |  |  |  |  |
| Type pattern |  | 1 |  |  |  | 10 |  |  |  |  |  |  |  |  |  |
| Positional pattern |  | 2 |  | 1 | 1 | 1 | 4 |  |  |  |  |  |  |  |  |
| default literal |  | 1 | 1 | 1 |  | 5 |  |  |  |  |  |  |  |  |  |
| Primary constructor on class or struct |  |  |  |  |  | 4 | 1 |  |  | 1 |  |  |  |  |  |
| Required member |  | 2 |  |  |  | 3 |  |  |  |  |  |  |  |  |  |
| field keyword in accessor |  | 4 |  |  |  |  |  |  |  |  |  |  |  |  |  |
| Generic type |  | 2 |  |  |  |  | 2 |  |  |  |  |  |  |  |  |
| Event accessors |  | 3 |  |  |  |  |  |  |  |  |  |  |  |  |  |
| Record struct |  | 1 |  |  |  | 1 |  | 1 |  |  |  |  |  |  |  |
| Abstract class |  | 1 |  |  | 1 |  |  |  |  |  |  |  |  |  |  |
| checked / unchecked expression |  | 1 |  |  |  |  |  |  |  |  |  | 1 |  |  |  |
| Static constructor |  | 1 |  |  |  |  |  |  |  | 1 |  |  |  |  |  |
| Static local function |  | 2 |  |  |  |  |  |  |  |  |  |  |  |  |  |
| Using alias |  |  |  |  |  |  |  |  |  | 1 |  |  |  |  | 1 |
| Alias of any type |  |  |  |  |  |  |  |  |  | 1 |  |  |  |  |  |
| Indexer |  | 1 |  |  |  |  |  |  |  |  |  |  |  |  |  |

## Style A share per project

| Style | Llyn.Tests.Engine | Llyn.UIDeportment | Llyn.Tests.Conduct | Llyn.Infrastructure | Llyn.Application | Llyn.Tests.Convention | Llyn.Conduct | Llyn.Core | Llyn.ShellEngine | Llyn.Tests.Interface | Llyn.Tests.Windows | Llyn.Core.Windows | Llyn.UIVeneer | Llyn.UIDeportment.Capsule | Llyn.Host |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| Null check | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - | - | - |
| Object creation | 53.09 % | 48.29 % | 35.08 % | 24.41 % | 29.73 % | 42.13 % | 9.66 % | 26.16 % | 29.10 % | 32.14 % | 35.44 % | 30.56 % | - | 14.29 % | 20.00 % |
| Collection creation | 99.48 % | 95.32 % | 99.19 % | 98.04 % | 99.45 % | 99.05 % | 99.44 % | 99.02 % | 100.00 % | 99.59 % | 91.67 % | 0.00 % | - | - | - |
| Member body | 2.27 % | 25.95 % | 2.60 % | 1.54 % | 10.58 % | 3.82 % | 16.69 % | 29.36 % | 15.69 % | 78.66 % | 25.66 % | 2.63 % | 0.00 % | 0.00 % | - |
| Local type | 0.03 % | 4.09 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.26 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | - | 0.00 % | 0.00 % |
| Using | 98.41 % | 83.33 % | 99.92 % | 83.17 % | 63.16 % | 100.00 % | 0.00 % | - | - | 95.65 % | 98.48 % | 81.82 % | - | - | - |
| Namespace | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - |
| Text building | 28.57 % | 47.50 % | 13.73 % | 47.23 % | 76.71 % | 82.07 % | 0.00 % | 24.14 % | - | 25.00 % | - | 0.00 % | - | 0.00 % | - |
| Branch | - | 61.11 % | 100.00 % | 68.00 % | 96.30 % | 76.47 % | 94.44 % | 62.75 % | 90.91 % | 33.33 % | - | 100.00 % | - | - | - |
| Lambda body | 96.64 % | 85.99 % | 75.10 % | 88.57 % | 92.55 % | 99.04 % | 96.45 % | 98.40 % | 91.55 % | 81.28 % | 27.85 % | 85.71 % | - | - | 77.78 % |
| Type test | 100.00 % | 91.74 % | 100.00 % | 93.33 % | 100.00 % | 94.51 % | 100.00 % | 100.00 % | 100.00 % | 44.44 % | 75.00 % | - | - | - | - |
| Constructor | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 36.36 % | 1.30 % | 0.00 % | 0.00 % | 14.29 % | - | 0.00 % | 0.00 % | - | - |
