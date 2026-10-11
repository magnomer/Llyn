# Syntax statistics - 0.19.14250

- Generated: 2026-10-12 00:24:00 +09:00
- Source roots: src, tests
- Root-level files: none
- Project segments: 1
- Excluded directories: .git, .vs, artifacts, bin, node_modules, obj, packages, publish
- Excluded suffixes: .g.cs, .g.i.cs, .AssemblyInfo.cs, .GlobalUsings.g.cs, .Designer.cs
- Roslyn: 4.14.0-3.25262.10
- Language version: Preview
- Scanned: 1,987 files, 15 projects, 208,751 lines

## Summary

| Measure | Value |
|---|---:|
| Files | 1,987 |
| Lines | 208,751 |
| Nodes | 1,104,959 |
| Syntax kinds used | 197 |
| Features used | 101 of 138 |
| Minimum C# version | 14 |
| Parse errors | 0 |

## Families

| Family | Features | Used | Count | Per 1k lines |
|---|---:|---:|---:|---:|
| Types | 15 | 12 | 4,126 | 19.77 |
| Namespaces | 6 | 3 | 1,989 | 9.53 |
| Members | 29 | 21 | 9,812 | 47.00 |
| Patterns | 16 | 16 | 6,351 | 30.42 |
| Expressions | 36 | 29 | 26,620 | 127.52 |
| Statements | 20 | 14 | 12,946 | 62.02 |
| Generics | 6 | 2 | 73 | 0.35 |
| Async | 2 | 2 | 653 | 3.13 |
| Nullability | 3 | 2 | 4,119 | 19.73 |
| Unsafe | 2 | 0 | 0 | 0.00 |
| Directives | 3 | 0 | 0 | 0.00 |
| Total | 138 | 101 | 66,689 | 319.47 |

## Projects

| Project | Files | Lines | Nodes | Features used | Newest C# |
|---|---:|---:|---:|---:|---:|
| Llyn.Tests.Engine | 219 | 35,819 | 249,803 | 61 | 12 |
| Llyn.UIDeportment | 310 | 33,801 | 155,712 | 88 | 14 |
| Llyn.Tests.Conduct | 152 | 27,294 | 196,777 | 61 | 12 |
| Llyn.Infrastructure | 183 | 24,120 | 110,737 | 70 | 12 |
| Llyn.Application | 126 | 18,799 | 86,768 | 71 | 12 |
| Llyn.Tests.Convention | 115 | 17,452 | 84,846 | 79 | 12 |
| Llyn.Conduct | 264 | 16,887 | 66,146 | 66 | 12 |
| Llyn.Core | 356 | 11,978 | 49,081 | 60 | 12 |
| Llyn.ShellEngine | 96 | 9,913 | 39,905 | 58 | 12 |
| Llyn.Tests.Interface | 92 | 8,973 | 45,299 | 72 | 12 |
| Llyn.Tests.Windows | 24 | 2,330 | 14,985 | 43 | 12 |
| Llyn.Core.Windows | 7 | 798 | 3,140 | 37 | 10 |
| Llyn.UIVeneer | 37 | 405 | 802 | 4 | 10 |
| Llyn.UIDeportment.Capsule | 5 | 96 | 427 | 15 | 10 |
| Llyn.Host | 1 | 86 | 531 | 4 | 9 |
| Total | 1,987 | 208,751 | 1,104,959 | 101 | 14 |

## Versions

| C# version | Features | Used | Count |
|---|---:|---:|---:|
| 1 | 51 | 35 | 17,461 |
| 2 | 10 | 8 | 2,086 |
| 3 | 7 | 5 | 6,596 |
| 4 | 3 | 2 | 781 |
| 5 | 3 | 3 | 1,516 |
| 6 | 6 | 5 | 4,904 |
| 7 | 11 | 10 | 6,277 |
| 7.1 | 1 | 1 | 8 |
| 7.2 | 1 | 0 | 0 |
| 7.3 | 1 | 0 | 0 |
| 8 | 17 | 13 | 10,967 |
| 9 | 10 | 9 | 6,327 |
| 10 | 3 | 2 | 1,989 |
| 11 | 6 | 3 | 632 |
| 12 | 4 | 4 | 7,141 |
| 13 | 1 | 0 | 0 |
| 14 | 3 | 1 | 4 |

## Features

| Feature | Family | C# | Count | Per 1k lines | Files | First |
|---|---|---|---:|---:|---:|---|
| Collection expression | Expressions | 12 | 6,944 | 33.26 | 889 | src/Llyn.Application/Card/LCardClerk.cs:57 |
| using declaration | Statements | 8 | 5,444 | 26.08 | 416 | src/Llyn.Application/Card/LTranslationClerk.cs:160 |
| Lambda | Expressions | 3 | 4,534 | 21.72 | 720 | src/Llyn.Application/Card/LCardClerk.cs:174 |
| if | Statements | 1 | 4,111 | 19.69 | 695 | src/Llyn.Application/Card/LCardClerk.cs:70 |
| Nullable reference type T? | Nullability | 8 | 3,543 | 16.97 | 974 | src/Llyn.Application/Card/LCardClerkField.cs:91 |
| Expression-bodied member | Members | 6 | 2,659 | 12.74 | 500 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Target-typed new | Expressions | 9 | 2,241 | 10.74 | 546 | src/Llyn.Application/Card/LMentionClerk.cs:167 |
| Readonly field | Members | 1 | 2,005 | 9.60 | 611 | src/Llyn.Application/Card/LCardClerk.cs:9 |
| File-scoped namespace | Namespaces | 10 | 1,986 | 9.51 | 1,986 | src/Llyn.Application/Card/LCardClerk.cs:5 |
| Declaration pattern | Patterns | 7 | 1,901 | 9.11 | 386 | src/Llyn.Application/Card/LMeaningClerk.cs:93 |
| Tuple literal or tuple type | Expressions | 7 | 1,589 | 7.61 | 270 | src/Llyn.Application/Card/LMeaningClerk.cs:26 |
| Sealed class | Types | 1 | 1,568 | 7.51 | 1,462 | src/Llyn.Application/Card/LCardClerk.cs:7 |
| foreach | Statements | 1 | 1,456 | 6.97 | 474 | src/Llyn.Application/Card/LCardClerk.cs:59 |
| Conditional ?: | Expressions | 1 | 1,411 | 6.76 | 528 | src/Llyn.Application/Card/LCardClerk.cs:173 |
| Class | Types | 1 | 1,408 | 6.74 | 1,391 | src/Llyn.Application/Card/LCardClerk.cs:7 |
| Static lambda | Expressions | 9 | 1,240 | 5.94 | 298 | src/Llyn.Application/Card/LCardEquality.cs:113 |
| Full property | Members | 1 | 1,191 | 5.71 | 355 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Constant pattern | Patterns | 7 | 1,110 | 5.32 | 199 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Null-conditional ?. ?[ | Expressions | 6 | 1,082 | 5.18 | 389 | src/Llyn.Application/Card/LMentionClerk.cs:47 |
| Null-coalescing ?? | Expressions | 2 | 966 | 4.63 | 392 | src/Llyn.Application/Card/LCardClerkField.cs:74 |
| not pattern | Patterns | 9 | 904 | 4.33 | 375 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| Auto property | Members | 3 | 895 | 4.29 | 265 | src/Llyn.Application/Citation/LCitationClerk.cs:41 |
| await | Expressions | 5 | 863 | 4.13 | 173 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:139 |
| Null-forgiving ! | Expressions | 8 | 863 | 4.13 | 314 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:145 |
| is null | Patterns | 7 | 824 | 3.95 | 358 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| with expression | Expressions | 9 | 734 | 3.52 | 250 | src/Llyn.Application/Card/LCardClerk.cs:109 |
| Interpolated string | Expressions | 6 | 683 | 3.27 | 163 | src/Llyn.Application/Localization/LLocalizationReader.cs:55 |
| Extension method | Members | 3 | 668 | 3.20 | 41 | tests/Llyn.Tests.Interface/TInterfaceArea.cs:7 |
| Raw string | Expressions | 11 | 613 | 2.94 | 140 | src/Llyn.Infrastructure/Database/Base/LDatabase.cs:51 |
| Async method | Async | 5 | 598 | 2.86 | 173 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:91 |
| Constructor | Members | 1 | 578 | 2.77 | 573 | src/Llyn.Application/Card/LCardClerk.cs:19 |
| Nullable value type T? | Nullability | 2 | 576 | 2.76 | 253 | src/Llyn.Application/Card/LMeaningClerk.cs:92 |
| Const field | Members | 1 | 568 | 2.72 | 246 | src/Llyn.Application/Card/LMentionClerk.cs:11 |
| Record class | Types | 9 | 545 | 2.61 | 453 | src/Llyn.Application/Ensign/LEnsignRow.cs:3 |
| Cast expression | Expressions | 1 | 484 | 2.32 | 167 | src/Llyn.Application/Markup/LMarkupClerkLink.cs:234 |
| Property pattern | Patterns | 8 | 483 | 2.31 | 127 | src/Llyn.Application/Draft/Clerk/LDraftClerk.cs:82 |
| Object initializer | Expressions | 3 | 479 | 2.29 | 205 | src/Llyn.Application/Portrait/LPortraitClerkLabel.cs:25 |
| Optional parameter | Members | 4 | 460 | 2.20 | 154 | src/Llyn.Application/Draft/Clerk/LDraftClerkPanel.cs:170 |
| nameof | Expressions | 6 | 406 | 1.94 | 142 | src/Llyn.Application/Citation/LAuthorClerk.cs:361 |
| catch | Statements | 1 | 380 | 1.82 | 166 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:142 |
| lock | Statements | 1 | 351 | 1.68 | 58 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:98 |
| try | Statements | 1 | 344 | 1.65 | 182 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:137 |
| Static class | Types | 2 | 343 | 1.64 | 343 | src/Llyn.Application/Card/LCardClerkField.cs:7 |
| and / or pattern | Patterns | 9 | 342 | 1.64 | 104 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Deconstruction | Expressions | 7 | 334 | 1.60 | 144 | src/Llyn.Application/Card/LMeaningClerk.cs:46 |
| Named argument | Members | 4 | 321 | 1.54 | 99 | src/Llyn.Application/Card/LTranslationClerk.cs:164 |
| for | Statements | 1 | 272 | 1.30 | 169 | src/Llyn.Application/Card/LCardClerk.cs:247 |
| Init accessor | Members | 9 | 223 | 1.07 | 78 | src/Llyn.Conduct/Lexicon/CExample.cs:20 |
| throw expression | Expressions | 7 | 206 | 0.99 | 102 | src/Llyn.Application/Card/LCardClerkField.cs:75 |
| typeof | Expressions | 1 | 205 | 0.98 | 46 | src/Llyn.Infrastructure/Configuration/LLocalizationLoader.cs:20 |
| Discard pattern | Patterns | 8 | 204 | 0.98 | 116 | src/Llyn.Application/Citation/LAuthorClerk.cs:243 |
| Switch expression | Patterns | 8 | 200 | 0.96 | 116 | src/Llyn.Application/Citation/LAuthorClerk.cs:239 |
| while | Statements | 1 | 192 | 0.92 | 115 | src/Llyn.Application/Card/LMeaningClerk.cs:44 |
| Spread element | Expressions | 12 | 190 | 0.91 | 106 | src/Llyn.Application/Card/LCardEquality.cs:53 |
| using statement | Statements | 1 | 173 | 0.83 | 74 | src/Llyn.Application/Citation/LAuthorCitation.cs:51 |
| Field-like event | Members | 1 | 158 | 0.76 | 95 | src/Llyn.Conduct/Atelier/CNavigation.cs:46 |
| Discard _ | Expressions | 7 | 153 | 0.73 | 59 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:108 |
| Case guard when | Patterns | 7 | 113 | 0.54 | 23 | src/Llyn.Application/Citation/LAuthorClerk.cs:241 |
| Interface | Types | 1 | 103 | 0.49 | 103 | src/Llyn.Application/Pronunciation/Relay/LListener.cs:5 |
| Verbatim string | Expressions | 1 | 90 | 0.43 | 19 | src/Llyn.Application/Localization/LLocalizationReader.cs:13 |
| Range .. | Expressions | 8 | 89 | 0.43 | 47 | src/Llyn.Application/Card/LMentionClerk.cs:173 |
| Relational pattern | Patterns | 9 | 87 | 0.42 | 29 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| as cast | Expressions | 1 | 86 | 0.41 | 45 | src/Llyn.Infrastructure/Database/Base/LWorkspaceArchive.cs:132 |
| Index from end ^ | Expressions | 8 | 84 | 0.40 | 47 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:230 |
| Exception filter | Statements | 6 | 74 | 0.35 | 48 | src/Llyn.Application/Outpost/LCourierClerk.cs:196 |
| Partial type | Types | 2 | 74 | 0.35 | 74 | src/Llyn.UIVeneer/Display/View/PDisplay.xaml.cs:5 |
| Switch statement | Patterns | 1 | 60 | 0.29 | 37 | src/Llyn.Application/Portrait/LPortraitClerkPress.cs:59 |
| is type test | Patterns | 1 | 59 | 0.28 | 36 | src/Llyn.Application/Outpost/LCourierClerk.cs:288 |
| Async lambda | Async | 5 | 55 | 0.26 | 10 | src/Llyn.Application/Outpost/LCourierClerk.cs:251 |
| Generic method | Generics | 2 | 55 | 0.26 | 30 | src/Llyn.Application/Card/LCardClerkField.cs:183 |
| yield return / break | Statements | 2 | 50 | 0.24 | 20 | src/Llyn.Application/Card/LCardClerkField.cs:35 |
| finally | Statements | 1 | 46 | 0.22 | 33 | src/Llyn.Application/Outpost/LCourierClerk.cs:76 |
| Enum | Types | 1 | 41 | 0.20 | 41 | src/Llyn.Conduct/Catalog/CCatalogOrder.cs:3 |
| Local const | Statements | 1 | 39 | 0.19 | 24 | src/Llyn.Conduct/Lexicon/CStateWording.cs:10 |
| var pattern | Patterns | 7 | 30 | 0.14 | 12 | src/Llyn.Conduct/Card/CFolio.cs:200 |
| Nested type | Types | 1 | 29 | 0.14 | 16 | src/Llyn.Conduct/Dialog/CSLens.cs:110 |
| ref or out parameter | Members | 1 | 28 | 0.13 | 22 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:149 |
| Null-coalescing assignment ??= | Expressions | 8 | 21 | 0.10 | 19 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:160 |
| Collection initializer | Expressions | 3 | 20 | 0.10 | 16 | src/Llyn.Application/Outpost/LCourierClerk.cs:177 |
| Constraint clause | Generics | 2 | 18 | 0.09 | 10 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:171 |
| Local function | Members | 7 | 17 | 0.08 | 11 | src/Llyn.Conduct/Desk/CErrand.cs:55 |
| await using | Statements | 8 | 14 | 0.07 | 7 | src/Llyn.Conduct/Portrait/CPortrait.cs:142 |
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
| Null check | is null / is not null | 824 | == null / != null | 0 | 100.00 % |
| Object creation | Target-typed new() | 2,241 | new T() | 4,074 | 35.49 % |
| Collection creation | Collection expression | 6,944 | Collection initializer or array creation | 69 | 99.02 % |
| Member body | Expression body | 2,659 | Block body | 10,346 | 20.45 % |
| Local type | var | 44 | Explicit type | 21,403 | 0.21 % |
| Using | Declaration | 5,444 | Statement | 173 | 96.92 % |
| Namespace | File-scoped | 1,986 | Block | 0 | 100.00 % |
| Text building | Interpolated string | 683 | string.Format or + with a string literal | 530 | 56.31 % |
| Branch | Switch expression | 200 | Switch statement | 60 | 76.92 % |
| Lambda body | Expression | 3,962 | Block | 572 | 87.38 % |
| Type test | is pattern with designation | 1,091 | as | 86 | 92.69 % |
| Constructor | Primary | 6 | Explicit | 573 | 1.04 % |

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
| IdentifierName | 319,630 | 1,987 |
| Argument | 115,011 | 1,507 |
| SimpleMemberAccessExpression | 105,366 | 1,489 |
| ArgumentList | 71,365 | 1,544 |
| InvocationExpression | 65,217 | 1,503 |
| ExpressionStatement | 31,448 | 1,383 |
| PredefinedType | 31,055 | 1,843 |
| StringLiteralExpression | 26,052 | 1,107 |
| Parameter | 22,748 | 1,791 |
| VariableDeclaration | 22,168 | 1,306 |
| VariableDeclarator | 22,168 | 1,306 |
| EqualsValueClause | 21,082 | 1,358 |
| LocalDeclarationStatement | 18,624 | 1,128 |
| Block | 18,354 | 1,466 |
| ParameterList | 13,836 | 1,907 |
| MethodDeclaration | 11,664 | 1,529 |
| NumericLiteralExpression | 11,560 | 1,084 |
| GenericName | 10,635 | 1,369 |
| TypeArgumentList | 10,635 | 1,369 |
| QualifiedName | 9,395 | 1,987 |
| ExpressionElement | 7,851 | 480 |
| SimpleAssignmentExpression | 7,529 | 1,022 |
| CollectionExpression | 6,944 | 889 |
| UsingDirective | 6,353 | 1,704 |
| ReturnStatement | 6,013 | 1,087 |
| NullLiteralExpression | 4,208 | 907 |
| NullableType | 4,119 | 1,024 |
| IfStatement | 4,111 | 695 |
| ObjectCreationExpression | 4,074 | 901 |
| Attribute | 3,601 | 426 |
| AttributeList | 3,601 | 426 |
| SimpleLambdaExpression | 3,525 | 642 |
| SingleVariableDesignation | 3,250 | 573 |
| BracketedArgumentList | 3,093 | 550 |
| FieldDeclaration | 2,950 | 768 |
| IsPatternExpression | 2,729 | 588 |
| ArrowExpressionClause | 2,661 | 501 |
| ElementAccessExpression | 2,484 | 499 |
| ImplicitObjectCreationExpression | 2,241 | 546 |
| PropertyDeclaration | 2,092 | 515 |
| FalseLiteralExpression | 2,091 | 563 |
| CompilationUnit | 1,987 | 1,987 |
| FileScopedNamespaceDeclaration | 1,986 | 1,986 |
| ConstantPattern | 1,934 | 451 |
| DeclarationPattern | 1,901 | 386 |
| TrueLiteralExpression | 1,785 | 518 |
| EqualsExpression | 1,577 | 574 |
| TupleExpression | 1,573 | 271 |
| AddExpression | 1,554 | 358 |
| InterpolatedStringText | 1,519 | 163 |
| ConditionalExpression | 1,411 | 528 |
| ClassDeclaration | 1,408 | 1,391 |
| LogicalAndExpression | 1,399 | 398 |
| ForEachStatement | 1,369 | 456 |
| Interpolation | 1,340 | 162 |
| AttributeArgument | 1,238 | 63 |
| LogicalNotExpression | 1,195 | 409 |
| AddAssignmentExpression | 1,183 | 275 |
| ConditionalAccessExpression | 1,082 | 389 |
| MemberBindingExpression | 1,082 | 389 |
| DeclarationExpression | 1,057 | 275 |
| SwitchExpressionArm | 1,038 | 116 |
| ParenthesizedLambdaExpression | 1,009 | 288 |
| AccessorList | 1,007 | 291 |
| GetAccessorDeclaration | 1,003 | 290 |
| LogicalOrExpression | 969 | 310 |
| CoalesceExpression | 966 | 392 |
| NotPattern | 904 | 375 |
| AwaitExpression | 863 | 173 |
| SuppressNullableWarningExpression | 863 | 314 |
| TupleElement | 823 | 197 |
| WithExpression | 734 | 250 |
| WithInitializerExpression | 734 | 250 |
| GreaterThanExpression | 721 | 306 |
| NameColon | 702 | 208 |
| InterpolatedStringExpression | 683 | 163 |
| ArrayRankSpecifier | 666 | 262 |
| ArrayType | 663 | 262 |
| OmittedArraySizeExpression | 644 | 262 |
| ImplicitElementAccess | 609 | 116 |
| ConstructorDeclaration | 580 | 575 |
| RecordDeclaration | 545 | 453 |
| PostIncrementExpression | 540 | 246 |
| LessThanExpression | 517 | 230 |
| CharacterLiteralExpression | 515 | 125 |
| AttributeArgumentList | 505 | 63 |
| RecursivePattern | 492 | 134 |
| CastExpression | 484 | 167 |
| PropertyPatternClause | 483 | 127 |
| ObjectInitializerExpression | 479 | 205 |
| NotEqualsExpression | 459 | 192 |
| Subpattern | 431 | 118 |
| ParenthesizedExpression | 396 | 194 |
| BaseList | 382 | 287 |
| CatchClause | 380 | 166 |
| CatchDeclaration | 377 | 164 |
| LockStatement | 351 | 58 |
| TupleType | 351 | 197 |
| TryStatement | 344 | 182 |
| ContinueStatement | 327 | 157 |
| SwitchSection | 323 | 37 |
| BreakStatement | 307 | 63 |
| OrPattern | 302 | 98 |
| SimpleBaseType | 289 | 268 |
| ForStatement | 272 | 169 |
| EnumMemberDeclaration | 245 | 41 |
| InitAccessorDeclaration | 223 | 78 |
| SubtractExpression | 219 | 99 |
| ThrowExpression | 206 | 102 |
| TypeOfExpression | 205 | 46 |
| DiscardPattern | 204 | 116 |
| ThrowStatement | 204 | 105 |
| SwitchExpression | 200 | 116 |
| WhileStatement | 192 | 115 |
| SpreadElement | 190 | 106 |
| UnaryMinusExpression | 176 | 78 |
| CaseSwitchLabel | 174 | 24 |
| UsingStatement | 173 | 74 |
| SubtractAssignmentExpression | 167 | 69 |
| EventFieldDeclaration | 158 | 95 |
| ElseClause | 156 | 121 |
| SetAccessorDeclaration | 155 | 76 |
| LessThanOrEqualExpression | 125 | 89 |
| ThisExpression | 124 | 83 |
| GreaterThanOrEqualExpression | 120 | 75 |
| WhenClause | 113 | 23 |
| CasePatternSwitchLabel | 111 | 22 |
| MultiplyExpression | 104 | 44 |
| PrimaryConstructorBaseType | 104 | 19 |
| InterfaceDeclaration | 103 | 103 |
| RangeExpression | 89 | 47 |
| ForEachVariableStatement | 87 | 64 |
| RelationalPattern | 87 | 29 |
| AsExpression | 86 | 45 |
| IndexExpression | 84 | 47 |
| CatchFilterClause | 74 | 48 |
| ArrayInitializerExpression | 68 | 37 |
| TypeParameter | 62 | 34 |
| SwitchStatement | 60 | 37 |
| IsExpression | 59 | 36 |
| TypeParameterList | 59 | 34 |
| DivideExpression | 55 | 28 |
| BitwiseOrExpression | 48 | 25 |
| FinallyClause | 46 | 33 |
| ImplicitArrayCreationExpression | 46 | 34 |
| DefaultSwitchLabel | 44 | 25 |
| YieldReturnStatement | 43 | 20 |
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
| TypePattern | 11 | 4 |
| PostDecrementExpression | 10 | 8 |
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
| Fact | 2,993 | 410 |
| InlineData | 477 | 51 |
| Theory | 101 | 53 |
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
| Collection expression | 2,938 | 159 | 1,767 | 300 | 340 | 523 | 164 | 289 | 79 | 242 | 143 |  |  |  |  |
| using declaration | 2,162 | 5 | 2,568 | 597 | 12 | 7 |  |  |  | 22 | 62 | 9 |  |  |  |
| Lambda | 884 | 201 | 1,442 | 34 | 357 | 730 | 356 | 182 | 71 | 187 | 74 | 7 |  |  | 9 |
| if | 54 | 1,256 | 5 | 711 | 710 | 464 | 376 | 258 | 221 | 38 | 4 | 11 |  | 3 |  |
| Nullable reference type T? | 178 | 476 | 376 | 365 | 375 | 297 | 342 | 364 | 264 | 450 | 42 | 9 |  | 5 |  |
| Expression-bodied member | 41 | 704 | 37 | 16 | 111 | 27 | 198 | 170 | 127 | 1,191 | 36 | 1 |  |  |  |
| Target-typed new | 188 | 748 | 257 | 143 | 157 | 190 | 46 | 59 | 87 | 269 | 81 | 11 |  | 1 | 4 |
| Readonly field | 40 | 630 | 4 | 122 | 403 | 216 | 333 | 31 | 177 | 36 | 3 | 9 |  | 1 |  |
| File-scoped namespace | 219 | 310 | 152 | 183 | 126 | 115 | 264 | 356 | 96 | 92 | 24 | 7 | 37 | 5 |  |
| Declaration pattern | 7 | 935 | 4 | 56 | 197 | 368 | 218 | 26 | 76 | 9 | 3 | 2 |  |  |  |
| Tuple literal or tuple type | 430 | 47 | 280 | 346 | 82 | 168 | 60 | 51 | 73 | 30 | 22 |  |  |  |  |
| Sealed class | 224 | 267 | 152 | 77 | 187 | 43 | 243 | 236 | 69 | 37 | 21 | 7 |  | 5 |  |
| foreach | 55 | 198 | 17 | 297 | 320 | 306 | 26 | 160 | 32 | 41 | 2 | 2 |  |  |  |
| Conditional ?: | 15 | 218 | 18 | 312 | 264 | 156 | 158 | 158 | 77 | 30 | 3 | 2 |  |  |  |
| Class | 227 | 294 | 151 | 181 | 104 | 108 | 89 | 26 | 59 | 100 | 24 | 7 | 37 | 1 |  |
| Static lambda | 113 | 88 | 649 | 8 | 119 | 15 | 65 | 97 | 15 | 64 | 6 |  |  |  | 1 |
| Full property | 7 | 650 | 5 | 6 | 112 | 2 | 184 | 151 | 44 | 29 |  | 1 |  |  |  |
| Constant pattern | 6 | 148 | 8 | 138 | 53 | 353 | 172 | 150 | 49 | 17 |  | 16 |  |  |  |
| Null-conditional ?. ?[ | 131 | 185 | 84 | 65 | 51 | 91 | 332 | 45 | 81 | 9 | 8 |  |  |  |  |
| Null-coalescing ?? | 19 | 76 | 12 | 123 | 199 | 80 | 61 | 267 | 50 | 63 | 10 | 5 |  | 1 |  |
| not pattern | 4 | 329 | 1 | 61 | 76 | 185 | 146 | 35 | 55 | 9 | 1 | 2 |  |  |  |
| Auto property | 5 | 377 | 3 | 8 | 7 | 23 | 134 | 240 | 58 | 40 |  |  |  |  |  |
| await | 263 | 62 | 322 | 62 | 67 | 1 | 22 |  | 24 | 14 | 1 | 25 |  |  |  |
| Null-forgiving ! | 237 | 149 | 274 | 62 | 15 | 25 | 9 | 3 | 4 | 68 | 17 |  |  |  |  |
| is null | 8 | 139 | 5 | 100 | 176 | 175 | 90 | 49 | 62 | 17 | 2 | 1 |  |  |  |
| with expression | 255 | 8 | 33 | 44 | 261 | 10 | 25 | 19 | 48 | 31 |  |  |  |  |  |
| Interpolated string | 54 | 19 | 7 | 162 | 51 | 380 |  | 7 |  | 3 |  |  |  |  |  |
| Extension method |  |  |  |  |  |  |  |  |  | 661 | 7 |  |  |  |  |
| Raw string | 142 | 2 | 7 | 221 |  | 233 |  |  |  | 8 |  |  |  |  |  |
| Async method | 201 | 61 | 201 | 32 | 38 | 1 | 20 |  | 20 | 10 | 1 | 13 |  |  |  |
| Constructor | 2 | 231 | 1 | 69 | 81 | 7 | 77 | 3 | 58 | 8 |  | 4 | 37 |  |  |
| Nullable value type T? | 24 | 56 | 20 | 42 | 38 | 11 | 132 | 35 | 122 | 72 | 22 |  |  | 2 |  |
| Const field | 79 | 74 | 14 | 180 | 12 | 113 | 3 | 73 | 4 | 12 |  | 3 |  | 1 |  |
| Record class |  | 18 | 2 | 2 | 106 | 6 | 162 | 233 | 10 | 2 |  |  |  | 4 |  |
| Cast expression | 9 | 139 | 77 | 89 | 4 | 50 | 7 | 4 |  | 47 | 52 | 6 |  |  |  |
| Property pattern | 2 | 154 |  | 10 | 17 | 270 | 13 | 5 | 6 | 4 | 2 |  |  |  |  |
| Object initializer | 63 | 129 | 103 | 29 | 3 | 60 |  | 3 | 1 | 35 | 50 | 2 |  | 1 |  |
| Optional parameter | 14 | 8 | 6 | 6 | 12 | 1 | 9 | 270 | 12 | 118 |  |  |  | 4 |  |
| nameof | 1 | 204 | 3 | 10 | 123 | 17 | 30 | 6 | 5 | 2 | 1 | 4 |  |  |  |
| catch | 1 | 21 |  | 133 | 33 | 8 | 126 | 5 | 27 | 1 | 9 | 13 |  | 3 |  |
| lock |  | 14 |  | 2 | 83 | 5 | 2 |  | 240 | 5 |  |  |  |  |  |
| try | 7 | 21 |  | 82 | 31 | 10 | 128 | 5 | 29 | 3 | 12 | 15 |  | 1 |  |
| Static class | 3 | 44 | 1 | 106 | 22 | 71 | 8 | 23 |  | 62 | 3 |  |  |  |  |
| and / or pattern | 3 | 46 | 3 | 68 | 13 | 159 | 2 | 39 | 2 | 6 |  | 1 |  |  |  |
| Deconstruction | 67 | 32 | 113 | 31 | 27 | 33 | 19 | 7 | 5 |  |  |  |  |  |  |
| Named argument | 149 |  | 39 | 16 | 23 | 16 |  | 18 | 3 | 57 |  |  |  |  |  |
| for | 13 | 45 | 6 | 68 | 58 | 27 | 7 | 21 | 7 | 18 | 2 |  |  |  |  |
| Init accessor |  | 2 |  |  |  | 3 | 1 | 217 |  |  |  |  |  |  |  |
| throw expression | 4 | 4 | 30 | 8 | 78 | 1 | 29 | 6 | 11 | 26 | 1 | 8 |  |  |  |
| typeof |  | 163 | 5 | 4 |  | 3 |  |  |  | 4 | 26 |  |  |  |  |
| Discard pattern |  | 13 | 1 | 16 | 26 | 67 | 33 | 32 | 10 | 1 |  | 5 |  |  |  |
| Switch expression |  | 11 | 1 | 16 | 26 | 65 | 33 | 32 | 10 | 1 |  | 5 |  |  |  |
| while | 18 | 9 | 5 | 98 | 11 | 25 | 2 | 18 | 1 | 5 |  |  |  |  |  |
| Spread element | 32 | 9 | 9 | 6 | 39 | 25 | 6 | 39 | 8 | 16 | 1 |  |  |  |  |
| using statement | 36 | 1 | 2 | 121 | 7 |  | 2 |  |  | 1 | 1 | 2 |  |  |  |
| Field-like event |  | 61 |  |  |  |  | 95 |  | 2 |  |  |  |  |  |  |
| Discard _ | 24 | 27 | 59 | 4 | 8 | 9 | 3 | 10 | 6 | 1 | 2 |  |  |  |  |
| Case guard when |  | 6 |  | 1 | 1 | 100 |  | 3 |  | 2 |  |  |  |  |  |
| Interface |  | 2 |  |  | 1 |  | 1 | 72 | 27 |  |  |  |  |  |  |
| Verbatim string | 8 |  |  |  | 3 | 77 |  |  |  | 2 |  |  |  |  |  |
| Range .. | 8 |  | 12 | 15 | 2 | 30 |  | 21 |  | 1 |  |  |  |  |  |
| Relational pattern |  | 7 |  | 28 | 8 | 3 |  | 40 | 1 |  |  |  |  |  |  |
| as cast |  | 58 |  | 3 |  | 9 |  |  |  | 15 | 1 |  |  |  |  |
| Index from end ^ | 14 | 3 | 41 | 1 | 4 | 11 | 2 | 8 |  |  |  |  |  |  |  |
| Exception filter |  | 3 |  | 33 | 16 | 3 |  |  | 19 |  |  |  |  |  |  |
| Partial type |  |  | 4 |  |  | 7 |  |  |  | 26 |  |  | 37 |  |  |
| Switch statement |  | 7 |  | 8 | 1 | 20 | 2 | 19 | 1 | 2 |  |  |  |  |  |
| is type test | 1 | 21 |  | 2 | 3 | 32 |  |  |  |  |  |  |  |  |  |
| Async lambda |  |  | 53 |  | 1 |  |  |  |  | 1 |  |  |  |  |  |
| Generic method |  | 23 | 1 |  | 13 | 1 | 4 | 7 | 1 | 5 |  |  |  |  |  |
| yield return / break |  | 9 |  | 7 | 7 | 25 |  | 2 |  |  |  |  |  |  |  |
| finally | 6 | 3 |  | 2 | 12 | 2 | 5 |  | 5 | 2 | 3 | 6 |  |  |  |
| Enum |  | 2 |  |  |  |  | 15 | 24 |  |  |  |  |  |  |  |
| Local const | 26 |  | 1 | 4 |  | 6 | 1 |  | 1 |  |  |  |  |  |  |
| var pattern |  |  |  |  |  | 28 | 1 |  |  |  | 1 |  |  |  |  |
| Nested type | 8 | 7 | 1 |  |  |  | 3 |  |  | 10 |  |  |  |  |  |
| ref or out parameter | 3 | 1 | 1 | 1 | 6 | 3 |  | 5 | 3 | 5 |  |  |  |  |  |
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
| Object creation | 53.26 % | 48.41 % | 34.82 % | 24.32 % | 29.02 % | 42.13 % | 9.98 % | 25.88 % | 29.29 % | 32.29 % | 37.33 % | 30.56 % | - | 14.29 % | 20.00 % |
| Collection creation | 99.49 % | 95.21 % | 99.16 % | 98.04 % | 99.42 % | 99.05 % | 99.39 % | 98.97 % | 100.00 % | 99.59 % | 92.26 % | 0.00 % | - | - | - |
| Member body | 2.27 % | 26.00 % | 2.62 % | 1.55 % | 10.55 % | 3.82 % | 16.67 % | 30.41 % | 15.66 % | 78.61 % | 25.17 % | 2.63 % | 0.00 % | 0.00 % | - |
| Local type | 0.03 % | 4.10 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.28 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | - | 0.00 % | 0.00 % |
| Using | 98.36 % | 83.33 % | 99.92 % | 83.15 % | 63.16 % | 100.00 % | 0.00 % | - | - | 95.65 % | 98.41 % | 81.82 % | - | - | - |
| Namespace | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - |
| Text building | 28.42 % | 47.50 % | 15.56 % | 47.93 % | 76.12 % | 82.07 % | 0.00 % | 26.92 % | - | 25.00 % | - | 0.00 % | - | 0.00 % | - |
| Branch | - | 61.11 % | 100.00 % | 66.67 % | 96.30 % | 76.47 % | 94.29 % | 62.75 % | 90.91 % | 33.33 % | - | 100.00 % | - | - | - |
| Lambda body | 96.61 % | 85.57 % | 74.83 % | 88.24 % | 92.16 % | 99.04 % | 96.91 % | 98.35 % | 91.55 % | 81.28 % | 28.38 % | 85.71 % | - | - | 77.78 % |
| Type test | 100.00 % | 91.52 % | 100.00 % | 93.18 % | 100.00 % | 94.51 % | 100.00 % | 100.00 % | 100.00 % | 44.44 % | 75.00 % | - | - | - | - |
| Constructor | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 36.36 % | 1.30 % | 0.00 % | 0.00 % | 14.29 % | - | 0.00 % | 0.00 % | - | - |
