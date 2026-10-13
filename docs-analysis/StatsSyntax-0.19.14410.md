# Syntax statistics - 0.19.14410

- Generated: 2026-10-13 21:03:20 +09:00
- Source roots: src, tests
- Root-level files: none
- Project segments: 1
- Excluded directories: .git, .vs, artifacts, bin, node_modules, obj, packages, publish
- Excluded suffixes: .g.cs, .g.i.cs, .AssemblyInfo.cs, .GlobalUsings.g.cs, .Designer.cs
- Roslyn: 4.14.0-3.25262.10
- Language version: Preview
- Scanned: 2,000 files, 15 projects, 212,939 lines

## Summary

| Measure | Value |
|---|---:|
| Files | 2,000 |
| Lines | 212,939 |
| Nodes | 1,130,358 |
| Syntax kinds used | 197 |
| Features used | 101 of 138 |
| Minimum C# version | 14 |
| Parse errors | 0 |

## Families

| Family | Features | Used | Count | Per 1k lines |
|---|---:|---:|---:|---:|
| Types | 15 | 12 | 4,156 | 19.52 |
| Namespaces | 6 | 3 | 2,002 | 9.40 |
| Members | 29 | 21 | 9,893 | 46.46 |
| Patterns | 16 | 16 | 6,467 | 30.37 |
| Expressions | 36 | 29 | 27,462 | 128.97 |
| Statements | 20 | 14 | 13,205 | 62.01 |
| Generics | 6 | 2 | 74 | 0.35 |
| Async | 2 | 2 | 653 | 3.07 |
| Nullability | 3 | 2 | 4,183 | 19.64 |
| Unsafe | 2 | 0 | 0 | 0.00 |
| Directives | 3 | 0 | 0 | 0.00 |
| Total | 138 | 101 | 68,095 | 319.79 |

## Projects

| Project | Files | Lines | Nodes | Features used | Newest C# |
|---|---:|---:|---:|---:|---:|
| Llyn.Tests.Engine | 222 | 36,904 | 256,893 | 61 | 12 |
| Llyn.UIDeportment | 313 | 34,079 | 157,458 | 88 | 14 |
| Llyn.Tests.Conduct | 154 | 27,951 | 202,520 | 61 | 12 |
| Llyn.Infrastructure | 184 | 24,390 | 112,092 | 70 | 12 |
| Llyn.Application | 127 | 19,210 | 89,177 | 72 | 12 |
| Llyn.Tests.Convention | 115 | 17,607 | 85,403 | 79 | 12 |
| Llyn.Conduct | 266 | 17,377 | 68,712 | 68 | 12 |
| Llyn.Core | 357 | 12,394 | 51,102 | 60 | 12 |
| Llyn.ShellEngine | 96 | 9,958 | 40,116 | 58 | 12 |
| Llyn.Tests.Interface | 92 | 9,034 | 45,650 | 72 | 12 |
| Llyn.Tests.Windows | 24 | 2,650 | 16,335 | 47 | 12 |
| Llyn.Core.Windows | 7 | 798 | 3,140 | 37 | 10 |
| Llyn.UIVeneer | 37 | 405 | 802 | 4 | 10 |
| Llyn.UIDeportment.Capsule | 5 | 96 | 427 | 15 | 10 |
| Llyn.Host | 1 | 86 | 531 | 4 | 9 |
| Total | 2,000 | 212,939 | 1,130,358 | 101 | 14 |

## Versions

| C# version | Features | Used | Count |
|---|---:|---:|---:|
| 1 | 51 | 35 | 17,709 |
| 2 | 10 | 8 | 2,155 |
| 3 | 7 | 5 | 6,754 |
| 4 | 3 | 2 | 792 |
| 5 | 3 | 3 | 1,516 |
| 6 | 6 | 5 | 4,961 |
| 7 | 11 | 10 | 6,441 |
| 7.1 | 1 | 1 | 8 |
| 7.2 | 1 | 0 | 0 |
| 7.3 | 1 | 0 | 0 |
| 8 | 17 | 13 | 11,162 |
| 9 | 10 | 9 | 6,524 |
| 10 | 3 | 2 | 2,002 |
| 11 | 6 | 3 | 651 |
| 12 | 4 | 4 | 7,416 |
| 13 | 1 | 0 | 0 |
| 14 | 3 | 1 | 4 |

## Features

| Feature | Family | C# | Count | Per 1k lines | Files | First |
|---|---|---|---:|---:|---:|---|
| Collection expression | Expressions | 12 | 7,205 | 33.84 | 903 | src/Llyn.Application/Card/LCardClerk.cs:60 |
| using declaration | Statements | 8 | 5,574 | 26.18 | 422 | src/Llyn.Application/Card/LTranslationClerk.cs:160 |
| Lambda | Expressions | 3 | 4,655 | 21.86 | 732 | src/Llyn.Application/Card/LCardClerk.cs:95 |
| if | Statements | 1 | 4,187 | 19.66 | 703 | src/Llyn.Application/Card/LCardClerk.cs:73 |
| Nullable reference type T? | Nullability | 8 | 3,569 | 16.76 | 978 | src/Llyn.Application/Card/LCardClerkField.cs:91 |
| Expression-bodied member | Members | 6 | 2,670 | 12.54 | 503 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Target-typed new | Expressions | 9 | 2,306 | 10.83 | 553 | src/Llyn.Application/Card/LMentionClerk.cs:167 |
| Readonly field | Members | 1 | 2,021 | 9.49 | 615 | src/Llyn.Application/Card/LCardClerk.cs:10 |
| File-scoped namespace | Namespaces | 10 | 1,999 | 9.39 | 1,999 | src/Llyn.Application/Card/LCardClerk.cs:6 |
| Declaration pattern | Patterns | 7 | 1,949 | 9.15 | 396 | src/Llyn.Application/Card/LMeaningClerk.cs:105 |
| Tuple literal or tuple type | Expressions | 7 | 1,664 | 7.81 | 278 | src/Llyn.Application/Card/LMeaningClerk.cs:26 |
| Sealed class | Types | 1 | 1,581 | 7.42 | 1,473 | src/Llyn.Application/Card/LCardClerk.cs:8 |
| foreach | Statements | 1 | 1,485 | 6.97 | 478 | src/Llyn.Application/Card/LCardClerk.cs:62 |
| Conditional ?: | Expressions | 1 | 1,437 | 6.75 | 537 | src/Llyn.Application/Card/LCardClerk.cs:170 |
| Class | Types | 1 | 1,420 | 6.67 | 1,403 | src/Llyn.Application/Card/LCardClerk.cs:8 |
| Static lambda | Expressions | 9 | 1,295 | 6.08 | 308 | src/Llyn.Application/Card/LCardClerk.cs:95 |
| Full property | Members | 1 | 1,196 | 5.62 | 356 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Constant pattern | Patterns | 7 | 1,126 | 5.29 | 202 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Null-conditional ?. ?[ | Expressions | 6 | 1,109 | 5.21 | 397 | src/Llyn.Application/Card/LMentionClerk.cs:47 |
| Null-coalescing ?? | Expressions | 2 | 992 | 4.66 | 396 | src/Llyn.Application/Card/LCardClerkField.cs:74 |
| not pattern | Patterns | 9 | 925 | 4.34 | 380 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| Auto property | Members | 3 | 916 | 4.30 | 267 | src/Llyn.Application/Citation/LCitationClerk.cs:41 |
| Null-forgiving ! | Expressions | 8 | 885 | 4.16 | 320 | src/Llyn.Application/Draft/Clerk/Card/LDraftClerkCard.cs:124 |
| await | Expressions | 5 | 863 | 4.05 | 173 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:139 |
| is null | Patterns | 7 | 828 | 3.89 | 359 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| with expression | Expressions | 9 | 776 | 3.64 | 257 | src/Llyn.Application/Card/LCardClerk.cs:115 |
| Interpolated string | Expressions | 6 | 700 | 3.29 | 165 | src/Llyn.Application/Localization/LLocalizationReader.cs:55 |
| Extension method | Members | 3 | 672 | 3.16 | 42 | tests/Llyn.Tests.Interface/Atelier/TInterfaceArea.cs:7 |
| Raw string | Expressions | 11 | 632 | 2.97 | 143 | src/Llyn.Infrastructure/Database/Base/LDatabase.cs:51 |
| Nullable value type T? | Nullability | 2 | 614 | 2.88 | 265 | src/Llyn.Application/Card/LMeaningClerk.cs:104 |
| Async method | Async | 5 | 598 | 2.81 | 173 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:91 |
| Constructor | Members | 1 | 581 | 2.73 | 576 | src/Llyn.Application/Card/LCardClerk.cs:21 |
| Const field | Members | 1 | 573 | 2.69 | 247 | src/Llyn.Application/Card/LMentionClerk.cs:11 |
| Record class | Types | 9 | 548 | 2.57 | 454 | src/Llyn.Application/Ensign/LEnsignRow.cs:3 |
| Cast expression | Expressions | 1 | 510 | 2.40 | 170 | src/Llyn.Application/Markup/LMarkupClerkLink.cs:285 |
| Property pattern | Patterns | 8 | 496 | 2.33 | 130 | src/Llyn.Application/Draft/Clerk/LDraftClerk.cs:82 |
| Object initializer | Expressions | 3 | 493 | 2.32 | 206 | src/Llyn.Application/Portrait/LPortraitClerkLabel.cs:25 |
| Optional parameter | Members | 4 | 464 | 2.18 | 155 | src/Llyn.Application/Draft/Clerk/LDraftClerkPanel.cs:170 |
| nameof | Expressions | 6 | 409 | 1.92 | 142 | src/Llyn.Application/Citation/LAuthorClerk.cs:361 |
| catch | Statements | 1 | 379 | 1.78 | 165 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:142 |
| Deconstruction | Expressions | 7 | 356 | 1.67 | 150 | src/Llyn.Application/Card/LMeaningClerk.cs:46 |
| lock | Statements | 1 | 351 | 1.65 | 58 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:98 |
| and / or pattern | Patterns | 9 | 347 | 1.63 | 104 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Static class | Types | 2 | 345 | 1.62 | 345 | src/Llyn.Application/Card/LCardClerkField.cs:7 |
| try | Statements | 1 | 345 | 1.62 | 182 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:137 |
| Named argument | Members | 4 | 328 | 1.54 | 99 | src/Llyn.Application/Card/LTranslationClerk.cs:164 |
| for | Statements | 1 | 279 | 1.31 | 175 | src/Llyn.Application/Card/LCardClerk.cs:280 |
| Init accessor | Members | 9 | 225 | 1.06 | 78 | src/Llyn.Conduct/Lexicon/CExample.cs:20 |
| typeof | Expressions | 1 | 213 | 1.00 | 46 | src/Llyn.Infrastructure/Configuration/LLocalizationLoader.cs:20 |
| throw expression | Expressions | 7 | 212 | 1.00 | 103 | src/Llyn.Application/Card/LCardClerkField.cs:75 |
| Discard pattern | Patterns | 8 | 205 | 0.96 | 117 | src/Llyn.Application/Citation/LAuthorClerk.cs:243 |
| Spread element | Expressions | 12 | 204 | 0.96 | 112 | src/Llyn.Application/Card/LCardClerk.cs:95 |
| Switch expression | Patterns | 8 | 202 | 0.95 | 118 | src/Llyn.Application/Citation/LAuthorClerk.cs:239 |
| while | Statements | 1 | 194 | 0.91 | 116 | src/Llyn.Application/Card/LMeaningClerk.cs:44 |
| using statement | Statements | 1 | 173 | 0.81 | 74 | src/Llyn.Application/Citation/LAuthorCitation.cs:51 |
| Field-like event | Members | 1 | 162 | 0.76 | 96 | src/Llyn.Conduct/Atelier/CWorkspace.cs:36 |
| Discard _ | Expressions | 7 | 144 | 0.68 | 56 | src/Llyn.Application/Draft/Clerk/Card/LDraftClerkCard.cs:127 |
| Case guard when | Patterns | 7 | 115 | 0.54 | 24 | src/Llyn.Application/Citation/LAuthorClerk.cs:241 |
| Interface | Types | 1 | 103 | 0.48 | 103 | src/Llyn.Application/Pronunciation/Relay/LListener.cs:5 |
| Verbatim string | Expressions | 1 | 93 | 0.44 | 20 | src/Llyn.Application/Localization/LLocalizationReader.cs:13 |
| Relational pattern | Patterns | 9 | 91 | 0.43 | 31 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Range .. | Expressions | 8 | 89 | 0.42 | 47 | src/Llyn.Application/Card/LMentionClerk.cs:173 |
| as cast | Expressions | 1 | 86 | 0.40 | 45 | src/Llyn.Infrastructure/Database/Base/Workspace/LWorkspaceArchive.cs:132 |
| Index from end ^ | Expressions | 8 | 85 | 0.40 | 47 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:230 |
| Partial type | Types | 2 | 74 | 0.35 | 74 | src/Llyn.UIVeneer/Display/View/PDisplay.xaml.cs:5 |
| Exception filter | Statements | 6 | 73 | 0.34 | 47 | src/Llyn.Application/Outpost/LCourierClerk.cs:196 |
| Switch statement | Patterns | 1 | 60 | 0.28 | 37 | src/Llyn.Application/Portrait/LPortraitClerkPress.cs:59 |
| is type test | Patterns | 1 | 59 | 0.28 | 36 | src/Llyn.Application/Outpost/LCourierClerk.cs:288 |
| Generic method | Generics | 2 | 56 | 0.26 | 30 | src/Llyn.Application/Card/LCardClerkField.cs:183 |
| Async lambda | Async | 5 | 55 | 0.26 | 10 | src/Llyn.Application/Outpost/LCourierClerk.cs:251 |
| yield return / break | Statements | 2 | 52 | 0.24 | 21 | src/Llyn.Application/Card/LCardClerkField.cs:35 |
| Local const | Statements | 1 | 51 | 0.24 | 27 | src/Llyn.Application/Markup/LMarkupClerkIntake.cs:271 |
| finally | Statements | 1 | 48 | 0.23 | 34 | src/Llyn.Application/Outpost/LCourierClerk.cs:76 |
| Enum | Types | 1 | 41 | 0.19 | 41 | src/Llyn.Conduct/Catalog/Vista/CCatalogOrder.cs:3 |
| var pattern | Patterns | 7 | 30 | 0.14 | 12 | src/Llyn.Conduct/Card/CFolio.cs:220 |
| Nested type | Types | 1 | 29 | 0.14 | 16 | src/Llyn.Conduct/Dialog/Galley/CSLens.cs:110 |
| ref or out parameter | Members | 1 | 29 | 0.14 | 23 | src/Llyn.Application/Draft/Clerk/Card/LDraftClerkCard.cs:162 |
| Null-coalescing assignment ??= | Expressions | 8 | 21 | 0.10 | 19 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:160 |
| Collection initializer | Expressions | 3 | 18 | 0.08 | 15 | src/Llyn.Application/Outpost/LCourierClerk.cs:177 |
| Constraint clause | Generics | 2 | 18 | 0.08 | 10 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:171 |
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
| Null check | is null / is not null | 828 | == null / != null | 0 | 100.00 % |
| Object creation | Target-typed new() | 2,306 | new T() | 4,126 | 35.85 % |
| Collection creation | Collection expression | 7,205 | Collection initializer or array creation | 72 | 99.01 % |
| Member body | Expression body | 2,670 | Block body | 10,526 | 20.23 % |
| Local type | var | 44 | Explicit type | 22,107 | 0.20 % |
| Using | Declaration | 5,574 | Statement | 173 | 96.99 % |
| Namespace | File-scoped | 1,999 | Block | 0 | 100.00 % |
| Text building | Interpolated string | 700 | string.Format or + with a string literal | 560 | 55.56 % |
| Branch | Switch expression | 202 | Switch statement | 60 | 77.10 % |
| Lambda body | Expression | 4,076 | Block | 579 | 87.56 % |
| Type test | is pattern with designation | 1,121 | as | 86 | 92.87 % |
| Constructor | Primary | 6 | Explicit | 576 | 1.03 % |

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
| IdentifierName | 327,003 | 2,000 |
| Argument | 117,892 | 1,520 |
| SimpleMemberAccessExpression | 107,889 | 1,500 |
| ArgumentList | 73,006 | 1,557 |
| InvocationExpression | 66,738 | 1,516 |
| ExpressionStatement | 32,041 | 1,397 |
| PredefinedType | 31,835 | 1,857 |
| StringLiteralExpression | 26,485 | 1,119 |
| Parameter | 23,194 | 1,801 |
| VariableDeclaration | 22,825 | 1,320 |
| VariableDeclarator | 22,825 | 1,320 |
| EqualsValueClause | 21,728 | 1,374 |
| LocalDeclarationStatement | 19,241 | 1,143 |
| Block | 18,658 | 1,480 |
| ParameterList | 14,052 | 1,919 |
| MethodDeclaration | 11,856 | 1,542 |
| NumericLiteralExpression | 11,799 | 1,102 |
| GenericName | 10,895 | 1,385 |
| TypeArgumentList | 10,895 | 1,385 |
| QualifiedName | 9,511 | 2,000 |
| ExpressionElement | 7,975 | 488 |
| SimpleAssignmentExpression | 7,718 | 1,036 |
| CollectionExpression | 7,205 | 903 |
| UsingDirective | 6,439 | 1,716 |
| ReturnStatement | 6,098 | 1,100 |
| NullLiteralExpression | 4,240 | 911 |
| IfStatement | 4,187 | 703 |
| NullableType | 4,183 | 1,033 |
| ObjectCreationExpression | 4,126 | 907 |
| Attribute | 3,677 | 431 |
| AttributeList | 3,677 | 431 |
| SimpleLambdaExpression | 3,628 | 656 |
| SingleVariableDesignation | 3,373 | 585 |
| BracketedArgumentList | 3,211 | 557 |
| FieldDeclaration | 2,979 | 775 |
| IsPatternExpression | 2,786 | 599 |
| ArrowExpressionClause | 2,672 | 504 |
| ElementAccessExpression | 2,601 | 507 |
| ImplicitObjectCreationExpression | 2,306 | 553 |
| PropertyDeclaration | 2,118 | 518 |
| FalseLiteralExpression | 2,098 | 565 |
| CompilationUnit | 2,000 | 2,000 |
| FileScopedNamespaceDeclaration | 1,999 | 1,999 |
| ConstantPattern | 1,954 | 457 |
| DeclarationPattern | 1,949 | 396 |
| TrueLiteralExpression | 1,812 | 525 |
| TupleExpression | 1,655 | 282 |
| EqualsExpression | 1,620 | 588 |
| AddExpression | 1,589 | 365 |
| InterpolatedStringText | 1,567 | 165 |
| ConditionalExpression | 1,437 | 537 |
| LogicalAndExpression | 1,431 | 405 |
| ClassDeclaration | 1,420 | 1,403 |
| ForEachStatement | 1,398 | 460 |
| Interpolation | 1,378 | 164 |
| AttributeArgument | 1,242 | 64 |
| LogicalNotExpression | 1,230 | 420 |
| AddAssignmentExpression | 1,192 | 279 |
| DeclarationExpression | 1,120 | 284 |
| ConditionalAccessExpression | 1,109 | 397 |
| MemberBindingExpression | 1,109 | 397 |
| SwitchExpressionArm | 1,047 | 118 |
| ParenthesizedLambdaExpression | 1,027 | 292 |
| AccessorList | 1,025 | 293 |
| GetAccessorDeclaration | 1,023 | 292 |
| CoalesceExpression | 992 | 396 |
| LogicalOrExpression | 990 | 315 |
| NotPattern | 925 | 380 |
| SuppressNullableWarningExpression | 885 | 320 |
| TupleElement | 870 | 201 |
| AwaitExpression | 863 | 173 |
| WithExpression | 776 | 257 |
| WithInitializerExpression | 776 | 257 |
| GreaterThanExpression | 745 | 317 |
| NameColon | 721 | 211 |
| InterpolatedStringExpression | 700 | 165 |
| ArrayRankSpecifier | 666 | 266 |
| ArrayType | 662 | 266 |
| OmittedArraySizeExpression | 644 | 266 |
| ImplicitElementAccess | 610 | 117 |
| ConstructorDeclaration | 583 | 578 |
| RecordDeclaration | 548 | 454 |
| PostIncrementExpression | 547 | 251 |
| LessThanExpression | 535 | 239 |
| CharacterLiteralExpression | 526 | 126 |
| CastExpression | 510 | 170 |
| AttributeArgumentList | 507 | 64 |
| RecursivePattern | 505 | 137 |
| PropertyPatternClause | 496 | 130 |
| ObjectInitializerExpression | 493 | 206 |
| NotEqualsExpression | 462 | 196 |
| Subpattern | 445 | 121 |
| ParenthesizedExpression | 403 | 198 |
| BaseList | 384 | 287 |
| CatchClause | 379 | 165 |
| CatchDeclaration | 376 | 163 |
| TupleType | 366 | 201 |
| LockStatement | 351 | 58 |
| TryStatement | 345 | 182 |
| ContinueStatement | 335 | 160 |
| SwitchSection | 330 | 37 |
| BreakStatement | 319 | 68 |
| OrPattern | 306 | 98 |
| SimpleBaseType | 289 | 268 |
| ForStatement | 279 | 175 |
| EnumMemberDeclaration | 247 | 41 |
| InitAccessorDeclaration | 225 | 78 |
| SubtractExpression | 219 | 100 |
| TypeOfExpression | 213 | 46 |
| ThrowExpression | 212 | 103 |
| DiscardPattern | 205 | 117 |
| ThrowStatement | 205 | 106 |
| SpreadElement | 204 | 112 |
| SwitchExpression | 202 | 118 |
| UnaryMinusExpression | 195 | 83 |
| WhileStatement | 194 | 116 |
| CaseSwitchLabel | 180 | 24 |
| UsingStatement | 173 | 74 |
| SubtractAssignmentExpression | 171 | 70 |
| EventFieldDeclaration | 162 | 96 |
| ElseClause | 157 | 121 |
| SetAccessorDeclaration | 155 | 76 |
| LessThanOrEqualExpression | 129 | 91 |
| ThisExpression | 124 | 83 |
| GreaterThanOrEqualExpression | 123 | 77 |
| WhenClause | 115 | 24 |
| CasePatternSwitchLabel | 112 | 22 |
| PrimaryConstructorBaseType | 106 | 19 |
| MultiplyExpression | 105 | 45 |
| InterfaceDeclaration | 103 | 103 |
| RelationalPattern | 91 | 31 |
| RangeExpression | 89 | 47 |
| ForEachVariableStatement | 87 | 64 |
| AsExpression | 86 | 45 |
| IndexExpression | 85 | 47 |
| ArrayInitializerExpression | 73 | 39 |
| CatchFilterClause | 73 | 47 |
| TypeParameter | 63 | 34 |
| SwitchStatement | 60 | 37 |
| TypeParameterList | 60 | 34 |
| IsExpression | 59 | 36 |
| DivideExpression | 55 | 28 |
| ImplicitArrayCreationExpression | 51 | 36 |
| BitwiseOrExpression | 49 | 25 |
| FinallyClause | 48 | 34 |
| YieldReturnStatement | 45 | 21 |
| DefaultSwitchLabel | 44 | 25 |
| ParenthesizedPattern | 42 | 20 |
| AndPattern | 41 | 18 |
| EnumDeclaration | 41 | 41 |
| ExpressionColon | 36 | 16 |
| OrAssignmentExpression | 35 | 20 |
| ArrayCreationExpression | 34 | 23 |
| LeftShiftExpression | 32 | 2 |
| VarPattern | 30 | 12 |
| CoalesceAssignmentExpression | 21 | 19 |
| AttributeTargetSpecifier | 19 | 8 |
| CollectionInitializerExpression | 18 | 15 |
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
| AndAssignmentExpression | 3 | 3 |
| BitwiseAndExpression | 3 | 3 |
| InterpolationAlignmentClause | 3 | 1 |
| PreIncrementExpression | 3 | 3 |
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
| Fact | 3,066 | 415 |
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
| Collection expression | 3,061 | 162 | 1,852 | 300 | 359 | 513 | 182 | 304 | 80 | 245 | 147 |  |  |  |  |
| using declaration | 2,228 | 5 | 2,627 | 598 | 12 | 7 |  |  |  | 22 | 66 | 9 |  |  |  |
| Lambda | 893 | 214 | 1,471 | 35 | 376 | 740 | 381 | 188 | 71 | 187 | 83 | 7 |  |  | 9 |
| if | 54 | 1,275 | 5 | 714 | 728 | 471 | 389 | 271 | 222 | 38 | 6 | 11 |  | 3 |  |
| Nullable reference type T? | 178 | 488 | 376 | 365 | 376 | 300 | 346 | 369 | 264 | 451 | 42 | 9 |  | 5 |  |
| Expression-bodied member | 42 | 703 | 37 | 16 | 113 | 27 | 199 | 170 | 128 | 1,198 | 36 | 1 |  |  |  |
| Target-typed new | 189 | 753 | 297 | 144 | 165 | 189 | 45 | 62 | 87 | 269 | 90 | 11 |  | 1 | 4 |
| Readonly field | 40 | 629 | 4 | 123 | 406 | 215 | 345 | 31 | 177 | 36 | 5 | 9 |  | 1 |  |
| File-scoped namespace | 222 | 313 | 154 | 184 | 127 | 115 | 266 | 357 | 96 | 92 | 24 | 7 | 37 | 5 |  |
| Declaration pattern | 7 | 964 | 4 | 59 | 201 | 369 | 225 | 29 | 77 | 9 | 3 | 2 |  |  |  |
| Tuple literal or tuple type | 438 | 47 | 295 | 361 | 93 | 164 | 70 | 58 | 76 | 30 | 32 |  |  |  |  |
| Sealed class | 227 | 270 | 154 | 77 | 189 | 43 | 245 | 237 | 69 | 37 | 21 | 7 |  | 5 |  |
| foreach | 56 | 202 | 17 | 300 | 332 | 305 | 28 | 167 | 32 | 42 | 2 | 2 |  |  |  |
| Conditional ?: | 15 | 217 | 18 | 315 | 269 | 156 | 172 | 163 | 77 | 30 | 3 | 2 |  |  |  |
| Class | 230 | 297 | 153 | 182 | 105 | 108 | 91 | 26 | 59 | 100 | 24 | 7 | 37 | 1 |  |
| Static lambda | 112 | 94 | 675 | 8 | 125 | 15 | 80 | 100 | 15 | 64 | 6 |  |  |  | 1 |
| Full property | 7 | 652 | 5 | 6 | 114 | 2 | 185 | 151 | 44 | 29 |  | 1 |  |  |  |
| Constant pattern | 6 | 151 | 8 | 140 | 53 | 358 | 175 | 151 | 49 | 19 |  | 16 |  |  |  |
| Null-conditional ?. ?[ | 131 | 191 | 84 | 65 | 53 | 91 | 351 | 46 | 81 | 9 | 7 |  |  |  |  |
| Null-coalescing ?? | 19 | 80 | 12 | 123 | 206 | 82 | 69 | 270 | 51 | 63 | 11 | 5 |  | 1 |  |
| not pattern | 4 | 341 | 1 | 63 | 76 | 185 | 149 | 38 | 56 | 9 | 1 | 2 |  |  |  |
| Auto property | 5 | 389 | 3 | 8 | 7 | 23 | 141 | 242 | 58 | 40 |  |  |  |  |  |
| Null-forgiving ! | 241 | 156 | 279 | 62 | 16 | 28 | 9 | 3 | 4 | 68 | 19 |  |  |  |  |
| await | 263 | 62 | 322 | 62 | 67 | 1 | 22 |  | 24 | 14 | 1 | 25 |  |  |  |
| is null | 8 | 137 | 5 | 100 | 177 | 174 | 92 | 53 | 62 | 17 | 2 | 1 |  |  |  |
| with expression | 278 | 8 | 41 | 45 | 269 | 10 | 25 | 21 | 48 | 31 |  |  |  |  |  |
| Interpolated string | 56 | 19 | 7 | 162 | 56 | 384 |  | 7 |  | 3 | 6 |  |  |  |  |
| Extension method |  |  |  |  |  |  |  |  |  | 665 | 7 |  |  |  |  |
| Raw string | 155 | 2 | 7 | 221 |  | 235 |  |  |  | 8 | 4 |  |  |  |  |
| Nullable value type T? | 25 | 74 | 28 | 42 | 38 | 11 | 140 | 39 | 122 | 73 | 20 |  |  | 2 |  |
| Async method | 201 | 61 | 201 | 32 | 38 | 1 | 20 |  | 20 | 10 | 1 | 13 |  |  |  |
| Constructor | 2 | 232 | 1 | 69 | 81 | 7 | 79 | 3 | 58 | 8 |  | 4 | 37 |  |  |
| Const field | 79 | 75 | 14 | 180 | 12 | 112 | 3 | 73 | 4 | 12 | 5 | 3 |  | 1 |  |
| Record class |  | 18 | 2 | 2 | 108 | 6 | 162 | 234 | 10 | 2 |  |  |  | 4 |  |
| Cast expression | 9 | 138 | 83 | 89 | 4 | 50 | 8 | 4 |  | 47 | 72 | 6 |  |  |  |
| Property pattern | 2 | 161 |  | 10 | 17 | 274 | 15 | 5 | 6 | 4 | 2 |  |  |  |  |
| Object initializer | 63 | 129 | 104 | 29 | 3 | 59 |  | 3 | 1 | 35 | 64 | 2 |  | 1 |  |
| Optional parameter | 14 | 8 | 6 | 6 | 12 | 1 | 10 | 273 | 12 | 118 |  |  |  | 4 |  |
| nameof | 1 | 205 | 3 | 10 | 125 | 17 | 30 | 6 | 5 | 2 | 1 | 4 |  |  |  |
| catch | 1 | 21 |  | 133 | 33 | 7 | 127 | 5 | 27 | 1 | 8 | 13 |  | 3 |  |
| Deconstruction | 67 | 36 | 127 | 31 | 28 | 30 | 22 | 9 | 5 |  | 1 |  |  |  |  |
| lock |  | 14 |  | 2 | 83 | 5 | 2 |  | 240 | 5 |  |  |  |  |  |
| and / or pattern | 3 | 47 | 3 | 68 | 13 | 161 | 2 | 39 | 2 | 8 |  | 1 |  |  |  |
| Static class | 3 | 44 | 1 | 107 | 23 | 71 | 8 | 23 |  | 62 | 3 |  |  |  |  |
| try | 8 | 21 |  | 82 | 31 | 9 | 129 | 5 | 29 | 3 | 12 | 15 |  | 1 |  |
| Named argument | 149 |  | 39 | 16 | 23 | 16 |  | 23 | 3 | 59 |  |  |  |  |  |
| for | 16 | 42 | 6 | 68 | 59 | 29 | 7 | 24 | 7 | 19 | 2 |  |  |  |  |
| Init accessor |  | 2 |  |  |  | 3 | 1 | 219 |  |  |  |  |  |  |  |
| typeof |  | 159 | 5 | 4 |  | 3 |  |  |  | 4 | 38 |  |  |  |  |
| throw expression | 4 | 7 | 30 | 8 | 81 | 1 | 29 | 6 | 11 | 26 | 1 | 8 |  |  |  |
| Discard pattern |  | 13 | 1 | 17 | 26 | 67 | 33 | 32 | 10 | 1 |  | 5 |  |  |  |
| Spread element | 34 | 9 | 9 | 6 | 45 | 29 | 7 | 39 | 8 | 16 | 2 |  |  |  |  |
| Switch expression |  | 11 | 1 | 17 | 26 | 65 | 34 | 32 | 10 | 1 |  | 5 |  |  |  |
| while | 18 | 9 | 5 | 98 | 11 | 25 | 2 | 20 | 1 | 5 |  |  |  |  |  |
| using statement | 36 | 1 | 2 | 121 | 7 |  | 2 |  |  | 1 | 1 | 2 |  |  |  |
| Field-like event |  | 65 |  |  |  |  | 95 |  | 2 |  |  |  |  |  |  |
| Discard _ | 24 | 27 | 50 | 4 | 7 | 10 | 3 | 10 | 6 | 1 | 2 |  |  |  |  |
| Case guard when |  | 6 |  | 1 | 1 | 100 | 1 | 4 |  | 2 |  |  |  |  |  |
| Interface |  | 2 |  |  | 1 |  | 1 | 72 | 27 |  |  |  |  |  |  |
| Verbatim string | 9 |  |  |  | 3 | 79 |  |  |  | 2 |  |  |  |  |  |
| Relational pattern |  | 7 |  | 28 | 8 | 5 | 2 | 40 | 1 |  |  |  |  |  |  |
| Range .. | 9 |  | 12 | 15 | 2 | 29 |  | 21 |  | 1 |  |  |  |  |  |
| as cast |  | 58 |  | 3 |  | 9 |  |  |  | 15 | 1 |  |  |  |  |
| Index from end ^ | 16 | 3 | 42 | 1 | 4 | 9 | 2 | 8 |  |  |  |  |  |  |  |
| Partial type |  |  | 4 |  |  | 7 |  |  |  | 26 |  |  | 37 |  |  |
| Exception filter |  | 3 |  | 33 | 16 | 2 |  |  | 19 |  |  |  |  |  |  |
| Switch statement |  | 7 |  | 8 | 1 | 20 | 2 | 19 | 1 | 2 |  |  |  |  |  |
| is type test | 1 | 21 |  | 2 | 3 | 32 |  |  |  |  |  |  |  |  |  |
| Generic method |  | 23 | 1 |  | 14 | 1 | 4 | 7 | 1 | 5 |  |  |  |  |  |
| Async lambda |  |  | 53 |  | 1 |  |  |  |  | 1 |  |  |  |  |  |
| yield return / break |  | 9 |  | 7 | 9 | 25 |  | 2 |  |  |  |  |  |  |  |
| Local const | 37 |  | 1 | 4 | 1 | 6 | 1 |  | 1 |  |  |  |  |  |  |
| finally | 7 | 3 |  | 2 | 12 | 2 | 5 |  | 5 | 2 | 4 | 6 |  |  |  |
| Enum |  | 2 |  |  |  |  | 15 | 24 |  |  |  |  |  |  |  |
| var pattern |  |  |  |  |  | 28 | 1 |  |  |  | 1 |  |  |  |  |
| Nested type | 8 | 7 | 1 |  |  |  | 3 |  |  | 10 |  |  |  |  |  |
| ref or out parameter | 3 | 1 | 1 | 1 | 7 | 3 |  | 5 | 3 | 5 |  |  |  |  |  |
| Null-coalescing assignment ??= | 1 | 10 |  | 1 | 4 | 3 | 1 |  |  |  |  | 1 |  |  |  |
| Collection initializer |  | 3 | 3 | 5 | 2 |  | 1 |  |  | 2 | 2 |  |  |  |  |
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
| Record struct |  | 1 |  |  |  | 1 |  | 1 |  |  |  |  |  |  |  |
| Abstract class |  | 1 |  |  | 1 |  |  |  |  |  |  |  |  |  |  |
| checked / unchecked expression |  | 1 |  |  |  |  |  |  |  |  |  | 1 |  |  |  |
| Static constructor |  | 1 |  |  |  |  |  |  |  | 1 |  |  |  |  |  |
| Static local function |  | 2 |  |  |  |  |  |  |  |  |  |  |  |  |  |
| Using alias |  |  |  |  |  |  |  |  |  | 1 |  |  |  |  | 1 |
| Alias of any type |  |  |  |  |  |  |  |  |  | 1 |  |  |  |  |  |
| Event accessors |  | 1 |  |  |  |  |  |  |  |  |  |  |  |  |  |
| Indexer |  | 1 |  |  |  |  |  |  |  |  |  |  |  |  |  |

## Style A share per project

| Style | Llyn.Tests.Engine | Llyn.UIDeportment | Llyn.Tests.Conduct | Llyn.Infrastructure | Llyn.Application | Llyn.Tests.Convention | Llyn.Conduct | Llyn.Core | Llyn.ShellEngine | Llyn.Tests.Interface | Llyn.Tests.Windows | Llyn.Core.Windows | Llyn.UIVeneer | Llyn.UIDeportment.Capsule | Llyn.Host |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| Null check | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - | - | - |
| Object creation | 53.09 % | 48.33 % | 38.08 % | 24.41 % | 29.73 % | 42.00 % | 9.53 % | 26.16 % | 29.10 % | 32.14 % | 38.30 % | 30.56 % | - | 14.29 % | 20.00 % |
| Collection creation | 99.48 % | 95.29 % | 99.20 % | 98.04 % | 99.45 % | 99.03 % | 99.45 % | 99.02 % | 100.00 % | 99.59 % | 91.30 % | 0.00 % | - | - | - |
| Member body | 2.27 % | 25.83 % | 2.57 % | 1.54 % | 10.58 % | 3.80 % | 16.32 % | 29.36 % | 15.69 % | 78.66 % | 23.84 % | 2.63 % | 0.00 % | 0.00 % | - |
| Local type | 0.03 % | 4.04 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.24 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | - | 0.00 % | 0.00 % |
| Using | 98.41 % | 83.33 % | 99.92 % | 83.17 % | 63.16 % | 100.00 % | 0.00 % | - | - | 95.65 % | 98.51 % | 81.82 % | - | - | - |
| Namespace | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - |
| Text building | 28.57 % | 47.50 % | 13.46 % | 47.23 % | 76.71 % | 81.70 % | 0.00 % | 24.14 % | - | 25.00 % | 85.71 % | 0.00 % | - | 0.00 % | - |
| Branch | - | 61.11 % | 100.00 % | 68.00 % | 96.30 % | 76.47 % | 94.44 % | 62.75 % | 90.91 % | 33.33 % | - | 100.00 % | - | - | - |
| Lambda body | 96.64 % | 86.45 % | 75.32 % | 88.57 % | 92.55 % | 98.92 % | 96.59 % | 98.40 % | 91.55 % | 81.28 % | 31.33 % | 85.71 % | - | - | 77.78 % |
| Type test | 100.00 % | 91.75 % | 100.00 % | 93.33 % | 100.00 % | 94.58 % | 100.00 % | 100.00 % | 100.00 % | 44.44 % | 75.00 % | - | - | - | - |
| Constructor | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 36.36 % | 1.27 % | 0.00 % | 0.00 % | 14.29 % | - | 0.00 % | 0.00 % | - | - |
