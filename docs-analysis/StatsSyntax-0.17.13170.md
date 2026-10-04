# Syntax statistics - 0.17.13170

- Generated: 2026-10-04 19:14:45 +09:00
- Source roots: src, tests
- Root-level files: none
- Project segments: 1
- Excluded directories: .git, .vs, artifacts, bin, node_modules, obj, packages, publish
- Excluded suffixes: .g.cs, .g.i.cs, .AssemblyInfo.cs, .GlobalUsings.g.cs, .Designer.cs
- Roslyn: 4.14.0-3.25262.10
- Language version: Preview
- Scanned: 1,634 files, 15 projects, 176,254 lines

## Summary

| Measure | Value |
|---|---:|
| Files | 1,634 |
| Lines | 176,254 |
| Nodes | 921,367 |
| Syntax kinds used | 188 |
| Features used | 98 of 138 |
| Minimum C# version | 12 |
| Parse errors | 0 |

## Families

| Family | Features | Used | Count | Per 1k lines |
|---|---:|---:|---:|---:|
| Types | 15 | 12 | 3,492 | 19.81 |
| Namespaces | 6 | 2 | 1,634 | 9.27 |
| Members | 29 | 20 | 8,960 | 50.84 |
| Patterns | 16 | 16 | 5,396 | 30.61 |
| Expressions | 36 | 29 | 20,820 | 118.12 |
| Statements | 20 | 13 | 11,256 | 63.86 |
| Generics | 6 | 2 | 72 | 0.41 |
| Async | 2 | 2 | 389 | 2.21 |
| Nullability | 3 | 2 | 3,438 | 19.51 |
| Unsafe | 2 | 0 | 0 | 0.00 |
| Directives | 3 | 0 | 0 | 0.00 |
| Total | 138 | 98 | 55,457 | 314.64 |

## Projects

| Project | Files | Lines | Nodes | Features used | Newest C# |
|---|---:|---:|---:|---:|---:|
| Llyn.UIDeportment | 281 | 32,388 | 150,475 | 85 | 12 |
| Llyn.Tests.Engine | 189 | 29,466 | 205,226 | 59 | 12 |
| Llyn.Tests.Conduct | 118 | 21,481 | 155,719 | 49 | 12 |
| Llyn.Infrastructure | 142 | 19,090 | 84,061 | 69 | 12 |
| Llyn.Application | 106 | 16,758 | 76,654 | 66 | 12 |
| Llyn.Tests.Convention | 104 | 15,577 | 76,695 | 79 | 12 |
| Llyn.Conduct | 217 | 14,095 | 54,676 | 58 | 12 |
| Llyn.Core | 291 | 10,056 | 40,017 | 58 | 12 |
| Llyn.ShellEngine | 50 | 8,270 | 33,853 | 57 | 12 |
| Llyn.Tests.Interface | 84 | 7,318 | 35,803 | 64 | 12 |
| Llyn.Tests.Windows | 13 | 928 | 5,486 | 37 | 12 |
| Llyn.Core.Windows | 3 | 341 | 1,251 | 26 | 10 |
| Llyn.UIVeneer | 31 | 339 | 672 | 4 | 10 |
| Llyn.UIDeportment.Capsule | 4 | 88 | 380 | 15 | 10 |
| Llyn.Host | 1 | 59 | 399 | 3 | 9 |
| Total | 1,634 | 176,254 | 921,367 | 98 | 12 |

## Versions

| C# version | Features | Used | Count |
|---|---:|---:|---:|
| 1 | 51 | 35 | 15,024 |
| 2 | 10 | 8 | 1,874 |
| 3 | 7 | 5 | 5,008 |
| 4 | 3 | 2 | 625 |
| 5 | 3 | 3 | 888 |
| 6 | 6 | 5 | 4,926 |
| 7 | 11 | 10 | 5,105 |
| 7.1 | 1 | 1 | 7 |
| 7.2 | 1 | 0 | 0 |
| 7.3 | 1 | 0 | 0 |
| 8 | 17 | 12 | 9,212 |
| 9 | 10 | 9 | 4,826 |
| 10 | 3 | 2 | 1,635 |
| 11 | 6 | 3 | 556 |
| 12 | 4 | 3 | 5,771 |
| 13 | 1 | 0 | 0 |
| 14 | 3 | 0 | 0 |

## Features

| Feature | Family | C# | Count | Per 1k lines | Files | First |
|---|---|---|---:|---:|---:|---|
| Collection expression | Expressions | 12 | 5,621 | 31.89 | 734 | src/Llyn.Application/Card/LCardClerk.cs:57 |
| using declaration | Statements | 8 | 4,712 | 26.73 | 359 | src/Llyn.Application/Card/LTranslationClerk.cs:159 |
| if | Statements | 1 | 3,656 | 20.74 | 580 | src/Llyn.Application/Card/LCardClerk.cs:70 |
| Lambda | Expressions | 3 | 3,335 | 18.92 | 581 | src/Llyn.Application/Card/LCardClerk.cs:174 |
| Nullable reference type T? | Nullability | 8 | 2,943 | 16.70 | 809 | src/Llyn.Application/Card/LCardClerkField.cs:91 |
| Expression-bodied member | Members | 6 | 2,741 | 15.55 | 423 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Target-typed new | Expressions | 9 | 1,739 | 9.87 | 445 | src/Llyn.Application/Card/LMentionClerk.cs:167 |
| Declaration pattern | Patterns | 7 | 1,719 | 9.75 | 325 | src/Llyn.Application/Card/LMeaningClerk.cs:93 |
| File-scoped namespace | Namespaces | 10 | 1,633 | 9.27 | 1,633 | src/Llyn.Application/Card/LCardClerk.cs:5 |
| Readonly field | Members | 1 | 1,493 | 8.47 | 477 | src/Llyn.Application/Card/LCardClerk.cs:9 |
| Full property | Members | 1 | 1,414 | 8.02 | 306 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Sealed class | Types | 1 | 1,310 | 7.43 | 1,211 | src/Llyn.Application/Card/LCardClerk.cs:7 |
| foreach | Statements | 1 | 1,294 | 7.34 | 407 | src/Llyn.Application/Card/LCardClerk.cs:59 |
| Class | Types | 1 | 1,173 | 6.66 | 1,157 | src/Llyn.Application/Card/LCardClerk.cs:7 |
| Conditional ?: | Expressions | 1 | 1,152 | 6.54 | 451 | src/Llyn.Application/Card/LCardClerk.cs:173 |
| Null-conditional ?. ?[ | Expressions | 6 | 1,100 | 6.24 | 330 | src/Llyn.Application/Card/LMentionClerk.cs:47 |
| Tuple literal or tuple type | Expressions | 7 | 1,095 | 6.21 | 218 | src/Llyn.Application/Card/LMeaningClerk.cs:26 |
| Constant pattern | Patterns | 7 | 873 | 4.95 | 164 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Null-coalescing ?? | Expressions | 2 | 848 | 4.81 | 348 | src/Llyn.Application/Card/LCardClerkField.cs:74 |
| not pattern | Patterns | 9 | 782 | 4.44 | 316 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| Auto property | Members | 3 | 743 | 4.22 | 231 | src/Llyn.Application/Citation/LCitationClerk.cs:40 |
| Static lambda | Expressions | 9 | 687 | 3.90 | 215 | src/Llyn.Application/Card/LCardEquality.cs:113 |
| is null | Patterns | 7 | 686 | 3.89 | 301 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| Null-forgiving ! | Expressions | 8 | 668 | 3.79 | 251 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:98 |
| Interpolated string | Expressions | 6 | 640 | 3.63 | 148 | src/Llyn.Application/Localization/LLocalizationReader.cs:55 |
| Extension method | Members | 3 | 575 | 3.26 | 41 | tests/Llyn.Tests.Interface/TInterfaceArea.cs:7 |
| with expression | Expressions | 9 | 574 | 3.26 | 199 | src/Llyn.Application/Card/LCardClerk.cs:109 |
| Raw string | Expressions | 11 | 537 | 3.05 | 127 | src/Llyn.Infrastructure/Database/Base/LDatabase.cs:51 |
| await | Expressions | 5 | 499 | 2.83 | 128 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:139 |
| Nullable value type T? | Nullability | 2 | 495 | 2.81 | 211 | src/Llyn.Application/Card/LMeaningClerk.cs:92 |
| Const field | Members | 1 | 492 | 2.79 | 209 | src/Llyn.Application/Card/LMentionClerk.cs:11 |
| Record class | Types | 9 | 467 | 2.65 | 380 | src/Llyn.Application/Ensign/LEnsignRow.cs:3 |
| Constructor | Members | 1 | 436 | 2.47 | 430 | src/Llyn.Application/Card/LCardClerk.cs:19 |
| Property pattern | Patterns | 8 | 416 | 2.36 | 103 | src/Llyn.Application/Draft/Clerk/LDraftClerk.cs:179 |
| Optional parameter | Members | 4 | 399 | 2.26 | 132 | src/Llyn.Application/Draft/Clerk/LDraftClerkPanel.cs:128 |
| Async method | Async | 5 | 387 | 2.20 | 129 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:91 |
| nameof | Expressions | 6 | 384 | 2.18 | 121 | src/Llyn.Application/Citation/LAuthorClerk.cs:361 |
| Cast expression | Expressions | 1 | 370 | 2.10 | 129 | src/Llyn.Application/Markup/LMarkupClerkLink.cs:234 |
| Object initializer | Expressions | 3 | 344 | 1.95 | 151 | src/Llyn.Core.Windows/LUsherShell.cs:37 |
| lock | Statements | 1 | 329 | 1.87 | 51 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:98 |
| Deconstruction | Expressions | 7 | 302 | 1.71 | 124 | src/Llyn.Application/Card/LMeaningClerk.cs:46 |
| catch | Statements | 1 | 293 | 1.66 | 120 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:142 |
| Static class | Types | 2 | 288 | 1.63 | 288 | src/Llyn.Application/Card/LCardClerkField.cs:7 |
| and / or pattern | Patterns | 9 | 283 | 1.61 | 87 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| try | Statements | 1 | 248 | 1.41 | 128 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:137 |
| for | Statements | 1 | 229 | 1.30 | 141 | src/Llyn.Application/Card/LCardClerk.cs:247 |
| Named argument | Members | 4 | 226 | 1.28 | 74 | src/Llyn.Application/Card/LTranslationClerk.cs:163 |
| Init accessor | Members | 9 | 215 | 1.22 | 72 | src/Llyn.Core/Catalog/Kind/LCatalogAuthor.cs:13 |
| typeof | Expressions | 1 | 175 | 0.99 | 39 | src/Llyn.Infrastructure/Configuration/LLocalizationLoader.cs:20 |
| while | Statements | 1 | 171 | 0.97 | 103 | src/Llyn.Application/Card/LMeaningClerk.cs:44 |
| using statement | Statements | 1 | 157 | 0.89 | 65 | src/Llyn.Application/Citation/LAuthorCitation.cs:51 |
| throw expression | Expressions | 7 | 150 | 0.85 | 78 | src/Llyn.Application/Card/LCardClerkField.cs:75 |
| Discard pattern | Patterns | 8 | 148 | 0.84 | 87 | src/Llyn.Application/Citation/LAuthorClerk.cs:243 |
| Spread element | Expressions | 12 | 146 | 0.83 | 73 | src/Llyn.Application/Card/LCardEquality.cs:53 |
| Field-like event | Members | 1 | 142 | 0.81 | 77 | src/Llyn.Conduct/Atelier/CNavigation.cs:46 |
| Switch expression | Patterns | 8 | 142 | 0.81 | 86 | src/Llyn.Application/Citation/LAuthorClerk.cs:239 |
| Discard _ | Expressions | 7 | 130 | 0.74 | 45 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:61 |
| Partial type | Types | 2 | 121 | 0.69 | 121 | src/Llyn.Infrastructure/Database/Context/LSituationArchive.cs:8 |
| Case guard when | Patterns | 7 | 106 | 0.60 | 21 | src/Llyn.Application/Citation/LAuthorClerk.cs:241 |
| Verbatim string | Expressions | 1 | 87 | 0.49 | 18 | src/Llyn.Application/Localization/LLocalizationReader.cs:13 |
| Interface | Types | 1 | 72 | 0.41 | 72 | src/Llyn.Application/Pronunciation/Relay/LListener.cs:5 |
| Range .. | Expressions | 8 | 72 | 0.41 | 36 | src/Llyn.Application/Card/LMentionClerk.cs:173 |
| Relational pattern | Patterns | 9 | 68 | 0.39 | 25 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| as cast | Expressions | 1 | 64 | 0.36 | 42 | src/Llyn.Infrastructure/Database/Entry/LCollocationArchive.cs:163 |
| Index from end ^ | Expressions | 8 | 63 | 0.36 | 37 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:230 |
| Exception filter | Statements | 6 | 61 | 0.35 | 39 | src/Llyn.Application/Pronunciation/Clerk/LFanqieClerk.cs:342 |
| Switch statement | Patterns | 1 | 58 | 0.33 | 34 | src/Llyn.Conduct/Configuration/CLedger.cs:181 |
| Generic method | Generics | 2 | 54 | 0.31 | 26 | src/Llyn.Application/Card/LCardClerkField.cs:183 |
| is type test | Patterns | 1 | 53 | 0.30 | 32 | src/Llyn.Infrastructure/Pronunciation/Source/LRecordingArchive.cs:152 |
| yield return / break | Statements | 2 | 48 | 0.27 | 18 | src/Llyn.Application/Card/LCardClerkField.cs:35 |
| Local const | Statements | 1 | 33 | 0.19 | 19 | src/Llyn.Conduct/Lexicon/CStateWording.cs:10 |
| var pattern | Patterns | 7 | 28 | 0.16 | 11 | src/Llyn.Conduct/Card/CFolio.cs:141 |
| Enum | Types | 1 | 27 | 0.15 | 27 | src/Llyn.Conduct/Catalog/CCatalogOrder.cs:3 |
| finally | Statements | 1 | 25 | 0.14 | 21 | src/Llyn.Application/Pronunciation/Clerk/LFanqieClerk.cs:351 |
| Nested type | Types | 1 | 24 | 0.14 | 14 | src/Llyn.UIDeportment/Kit/Bind/QLook.cs:19 |
| ref or out parameter | Members | 1 | 23 | 0.13 | 19 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:102 |
| Default interface method body | Members | 8 | 19 | 0.11 | 6 | src/Llyn.ShellEngine/Port/LDraftPort.cs:58 |
| Constraint clause | Generics | 2 | 18 | 0.10 | 8 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:171 |
| Null-coalescing assignment ??= | Expressions | 8 | 18 | 0.10 | 16 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:160 |
| Local function | Members | 7 | 16 | 0.09 | 10 | src/Llyn.Conduct/Desk/CErrand.cs:54 |
| List pattern | Patterns | 11 | 14 | 0.08 | 12 | src/Llyn.ShellEngine/Engine/LDraftFacade.cs:66 |
| Conversion operator | Members | 1 | 13 | 0.07 | 2 | src/Llyn.Core/Lexicon/State/LStateValue.cs:49 |
| Collection initializer | Expressions | 3 | 11 | 0.06 | 9 | src/Llyn.Infrastructure/Database/Source/LAuthorArchive.cs:117 |
| Type pattern | Patterns | 9 | 11 | 0.06 | 4 | src/Llyn.UIDeportment/Sound/Accent/QAccentControl.cs:62 |
| Positional pattern | Patterns | 8 | 9 | 0.05 | 9 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:37 |
| default literal | Expressions | 7.1 | 7 | 0.04 | 5 | src/Llyn.Infrastructure/Pronunciation/Loader/LSourceLoader.cs:73 |
| Required member | Members | 11 | 5 | 0.03 | 3 | src/Llyn.UIDeportment/Panel/Taxonomy/QMembershipItem.cs:27 |
| Primary constructor on class or struct | Types | 12 | 4 | 0.02 | 4 | tests/Llyn.Tests.Convention/TAuditFakeMember.cs:3 |
| Event accessors | Members | 1 | 3 | 0.02 | 2 | src/Llyn.UIDeportment/Display/View/PGrasp.cs:86 |
| Abstract class | Types | 1 | 2 | 0.01 | 2 | src/Llyn.Application/Request/Entry/LRequest.cs:3 |
| Async lambda | Async | 5 | 2 | 0.01 | 1 | src/Llyn.Core.Windows/LPressBrowser.cs:31 |
| checked / unchecked expression | Expressions | 1 | 2 | 0.01 | 2 | src/Llyn.Core.Windows/LPressBrowser.cs:14 |
| Generic type | Types | 2 | 2 | 0.01 | 2 | src/Llyn.Conduct/Configuration/CEnsignSheet.cs:5 |
| Record struct | Types | 10 | 2 | 0.01 | 2 | src/Llyn.UIDeportment/Mention/Menu/PSwath.cs:17 |
| Static constructor | Members | 1 | 2 | 0.01 | 2 | src/Llyn.UIDeportment/Kit/Bind/QLookItem.cs:22 |
| Static local function | Members | 8 | 2 | 0.01 | 1 | src/Llyn.UIDeportment/Kit/Asset/QIcon.cs:144 |
| Indexer | Members | 1 | 1 | 0.01 | 1 | src/Llyn.UIDeportment/Kit/Asset/QLocalizationCatalog.cs:19 |
| Using alias | Namespaces | 1 | 1 | 0.01 | 1 | src/Llyn.Host/LHost.cs:10 |

## Styles

| Style | A | A count | B | B count | A share |
|---|---|---:|---|---:|---:|
| Null check | is null / is not null | 686 | == null / != null | 0 | 100.00 % |
| Object creation | Target-typed new() | 1,739 | new T() | 3,257 | 34.81 % |
| Collection creation | Collection expression | 5,621 | Collection initializer or array creation | 40 | 99.29 % |
| Member body | Expression body | 2,741 | Block body | 9,096 | 23.16 % |
| Local type | var | 42 | Explicit type | 17,697 | 0.24 % |
| Using | Declaration | 4,712 | Statement | 157 | 96.78 % |
| Namespace | File-scoped | 1,633 | Block | 0 | 100.00 % |
| Text building | Interpolated string | 640 | string.Format or + with a string literal | 314 | 67.09 % |
| Branch | Switch expression | 142 | Switch statement | 58 | 71.00 % |
| Lambda body | Expression | 3,022 | Block | 313 | 90.61 % |
| Type test | is pattern with designation | 962 | as | 64 | 93.76 % |
| Constructor | Primary | 4 | Explicit | 430 | 0.92 % |

## Features unused (40)

| Feature | Family | C# |
|---|---|---|
| Struct | Types | 1 |
| Delegate | Types | 1 |
| File-local type | Types | 11 |
| Block namespace | Namespaces | 1 |
| Global using | Namespaces | 10 |
| Using static | Namespaces | 6 |
| Alias of any type | Namespaces | 12 |
| Readonly struct member | Members | 8 |
| Operator overload | Members | 1 |
| Extension block | Members | 14 |
| field keyword in accessor | Members | 14 |
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
| await using | Statements | 8 |
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
| IdentifierName | 267,267 | 1,634 |
| Argument | 95,183 | 1,244 |
| SimpleMemberAccessExpression | 87,290 | 1,239 |
| ArgumentList | 59,757 | 1,281 |
| InvocationExpression | 54,841 | 1,250 |
| PredefinedType | 26,861 | 1,539 |
| ExpressionStatement | 26,759 | 1,138 |
| StringLiteralExpression | 20,670 | 909 |
| Parameter | 19,137 | 1,475 |
| VariableDeclaration | 18,193 | 1,069 |
| VariableDeclarator | 18,193 | 1,069 |
| EqualsValueClause | 17,500 | 1,131 |
| Block | 15,905 | 1,205 |
| LocalDeclarationStatement | 15,277 | 934 |
| ParameterList | 12,032 | 1,572 |
| MethodDeclaration | 10,317 | 1,242 |
| NumericLiteralExpression | 9,389 | 930 |
| GenericName | 8,969 | 1,095 |
| TypeArgumentList | 8,969 | 1,095 |
| QualifiedName | 7,658 | 1,634 |
| SimpleAssignmentExpression | 6,331 | 833 |
| CollectionExpression | 5,621 | 734 |
| ExpressionElement | 5,598 | 396 |
| ReturnStatement | 5,225 | 898 |
| UsingDirective | 5,098 | 1,395 |
| IfStatement | 3,656 | 580 |
| NullLiteralExpression | 3,578 | 745 |
| NullableType | 3,438 | 847 |
| ObjectCreationExpression | 3,257 | 707 |
| Attribute | 2,897 | 352 |
| AttributeList | 2,897 | 352 |
| SingleVariableDesignation | 2,861 | 483 |
| ArrowExpressionClause | 2,742 | 423 |
| SimpleLambdaExpression | 2,559 | 518 |
| BracketedArgumentList | 2,525 | 454 |
| FieldDeclaration | 2,392 | 624 |
| IsPatternExpression | 2,361 | 493 |
| PropertyDeclaration | 2,161 | 431 |
| ElementAccessExpression | 2,058 | 410 |
| ImplicitObjectCreationExpression | 1,739 | 445 |
| DeclarationPattern | 1,719 | 325 |
| FalseLiteralExpression | 1,681 | 488 |
| CompilationUnit | 1,634 | 1,634 |
| FileScopedNamespaceDeclaration | 1,633 | 1,633 |
| ConstantPattern | 1,559 | 380 |
| TrueLiteralExpression | 1,356 | 430 |
| EqualsExpression | 1,354 | 494 |
| InterpolatedStringText | 1,300 | 148 |
| ForEachStatement | 1,225 | 397 |
| ClassDeclaration | 1,173 | 1,157 |
| LogicalAndExpression | 1,163 | 325 |
| ConditionalExpression | 1,152 | 451 |
| Interpolation | 1,127 | 147 |
| TupleExpression | 1,112 | 232 |
| ConditionalAccessExpression | 1,100 | 330 |
| MemberBindingExpression | 1,100 | 330 |
| AddExpression | 1,093 | 289 |
| AddAssignmentExpression | 1,065 | 226 |
| LogicalNotExpression | 1,052 | 347 |
| DeclarationExpression | 888 | 232 |
| AccessorList | 866 | 253 |
| GetAccessorDeclaration | 862 | 252 |
| CoalesceExpression | 848 | 348 |
| LogicalOrExpression | 823 | 258 |
| SwitchExpressionArm | 813 | 86 |
| AttributeArgument | 786 | 47 |
| NotPattern | 782 | 316 |
| ParenthesizedLambdaExpression | 776 | 245 |
| SuppressNullableWarningExpression | 668 | 251 |
| TupleElement | 664 | 160 |
| InterpolatedStringExpression | 640 | 148 |
| WithExpression | 574 | 199 |
| WithInitializerExpression | 574 | 199 |
| NameColon | 565 | 167 |
| GreaterThanExpression | 561 | 260 |
| ArrayRankSpecifier | 507 | 198 |
| ArrayType | 506 | 198 |
| AwaitExpression | 499 | 128 |
| OmittedArraySizeExpression | 493 | 198 |
| ImplicitElementAccess | 467 | 83 |
| RecordDeclaration | 467 | 380 |
| PostIncrementExpression | 463 | 210 |
| CharacterLiteralExpression | 438 | 99 |
| ConstructorDeclaration | 438 | 432 |
| RecursivePattern | 425 | 109 |
| LessThanExpression | 418 | 191 |
| PropertyPatternClause | 416 | 103 |
| Subpattern | 385 | 101 |
| NotEqualsExpression | 381 | 160 |
| CastExpression | 370 | 129 |
| ObjectInitializerExpression | 344 | 151 |
| AttributeArgumentList | 338 | 47 |
| BaseList | 332 | 239 |
| LockStatement | 329 | 51 |
| SwitchSection | 315 | 34 |
| ParenthesizedExpression | 300 | 160 |
| BreakStatement | 297 | 58 |
| CatchClause | 293 | 120 |
| CatchDeclaration | 291 | 119 |
| TupleType | 286 | 160 |
| ContinueStatement | 281 | 137 |
| OrPattern | 251 | 84 |
| TryStatement | 248 | 128 |
| SimpleBaseType | 234 | 220 |
| ForStatement | 229 | 141 |
| InitAccessorDeclaration | 215 | 72 |
| EnumMemberDeclaration | 197 | 27 |
| TypeOfExpression | 175 | 39 |
| CaseSwitchLabel | 172 | 23 |
| WhileStatement | 171 | 103 |
| SubtractExpression | 163 | 79 |
| ThrowStatement | 161 | 82 |
| UsingStatement | 157 | 65 |
| SetAccessorDeclaration | 154 | 69 |
| SubtractAssignmentExpression | 151 | 62 |
| ThisExpression | 151 | 81 |
| ThrowExpression | 150 | 78 |
| DiscardPattern | 148 | 87 |
| SpreadElement | 146 | 73 |
| EventFieldDeclaration | 142 | 77 |
| SwitchExpression | 142 | 86 |
| UnaryMinusExpression | 142 | 61 |
| ElseClause | 136 | 110 |
| CasePatternSwitchLabel | 108 | 19 |
| LessThanOrEqualExpression | 107 | 76 |
| WhenClause | 106 | 21 |
| PrimaryConstructorBaseType | 102 | 19 |
| GreaterThanOrEqualExpression | 82 | 57 |
| InterfaceDeclaration | 72 | 72 |
| RangeExpression | 72 | 36 |
| MultiplyExpression | 71 | 29 |
| ForEachVariableStatement | 69 | 48 |
| RelationalPattern | 68 | 25 |
| AsExpression | 64 | 42 |
| IndexExpression | 63 | 37 |
| CatchFilterClause | 61 | 39 |
| TypeParameter | 59 | 28 |
| SwitchStatement | 58 | 34 |
| TypeParameterList | 56 | 28 |
| IsExpression | 53 | 32 |
| ArrayInitializerExpression | 48 | 23 |
| BitwiseOrExpression | 44 | 23 |
| YieldReturnStatement | 42 | 18 |
| DefaultSwitchLabel | 41 | 22 |
| OrAssignmentExpression | 37 | 21 |
| ParenthesizedPattern | 34 | 15 |
| AndPattern | 32 | 12 |
| LeftShiftExpression | 32 | 2 |
| ExpressionColon | 30 | 13 |
| DivideExpression | 29 | 17 |
| VarPattern | 28 | 11 |
| EnumDeclaration | 27 | 27 |
| ImplicitArrayCreationExpression | 27 | 21 |
| FinallyClause | 25 | 21 |
| ArrayCreationExpression | 21 | 15 |
| ClassConstraint | 18 | 8 |
| CoalesceAssignmentExpression | 18 | 16 |
| TypeParameterConstraintClause | 18 | 8 |
| LocalFunctionStatement | 16 | 10 |
| AttributeTargetSpecifier | 15 | 7 |
| ListPattern | 14 | 12 |
| BaseExpression | 13 | 5 |
| ConversionOperatorDeclaration | 13 | 2 |
| CollectionInitializerExpression | 11 | 9 |
| TypePattern | 11 | 4 |
| PositionalPatternClause | 9 | 9 |
| PostDecrementExpression | 8 | 7 |
| DefaultLiteralExpression | 7 | 5 |
| InterpolationFormatClause | 7 | 1 |
| BaseConstructorInitializer | 6 | 6 |
| GlobalStatement | 6 | 1 |
| NameEquals | 6 | 4 |
| YieldBreakStatement | 6 | 5 |
| ExplicitInterfaceSpecifier | 4 | 3 |
| AddAccessorDeclaration | 3 | 2 |
| EventDeclaration | 3 | 2 |
| InterpolationAlignmentClause | 3 | 1 |
| PreIncrementExpression | 3 | 3 |
| RemoveAccessorDeclaration | 3 | 2 |
| SlicePattern | 3 | 3 |
| ModuloExpression | 2 | 1 |
| RecordStructDeclaration | 2 | 2 |
| ThisConstructorInitializer | 2 | 2 |
| UncheckedExpression | 2 | 2 |
| AndAssignmentExpression | 1 | 1 |
| BitwiseAndExpression | 1 | 1 |
| BracketedParameterList | 1 | 1 |
| IndexerDeclaration | 1 | 1 |

## Attributes

| Attribute | Count | Files |
|---|---:|---:|
| Fact | 2,494 | 340 |
| InlineData | 320 | 39 |
| Theory | 64 | 39 |
| JsonPropertyName | 11 | 3 |
| CollectionBehavior | 2 | 2 |
| DllImport | 2 | 1 |
| Flags | 1 | 1 |
| MarshalAs | 1 | 1 |
| NotNullIfNotNull | 1 | 1 |
| NotNullWhen | 1 | 1 |

## Parse errors (0)

None.

## Feature counts per project

| Feature | Llyn.UIDeportment | Llyn.Tests.Engine | Llyn.Tests.Conduct | Llyn.Infrastructure | Llyn.Application | Llyn.Tests.Convention | Llyn.Conduct | Llyn.Core | Llyn.ShellEngine | Llyn.Tests.Interface | Llyn.Tests.Windows | Llyn.Core.Windows | Llyn.UIVeneer | Llyn.UIDeportment.Capsule | Llyn.Host |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| Collection expression | 144 | 2,287 | 1,414 | 245 | 303 | 486 | 143 | 266 | 82 | 200 | 51 |  |  |  |  |
| using declaration | 5 | 1,795 | 2,310 | 547 | 12 | 7 |  |  |  | 18 | 18 |  |  |  |  |
| if | 1,192 | 49 | 2 | 525 | 656 | 431 | 345 | 216 | 201 | 24 | 3 | 9 |  | 3 |  |
| Lambda | 182 | 665 | 916 | 21 | 296 | 638 | 303 | 92 | 56 | 127 | 27 | 4 |  |  | 8 |
| Nullable reference type T? | 455 | 131 | 265 | 270 | 301 | 264 | 303 | 312 | 245 | 362 | 25 | 6 |  | 4 |  |
| Expression-bodied member | 938 | 33 | 22 | 6 | 109 | 26 | 245 | 152 | 199 | 986 | 25 |  |  |  |  |
| Target-typed new | 716 | 135 | 85 | 118 | 137 | 172 | 34 | 46 | 74 | 195 | 21 | 1 |  | 1 | 4 |
| Declaration pattern | 899 | 5 | 1 | 23 | 191 | 326 | 171 | 21 | 72 | 7 | 3 |  |  |  |  |
| File-scoped namespace | 281 | 189 | 118 | 142 | 106 | 104 | 217 | 291 | 50 | 84 | 13 | 3 | 31 | 4 |  |
| Readonly field | 432 | 33 | 1 | 100 | 355 | 205 | 212 | 28 | 88 | 35 |  | 3 |  | 1 |  |
| Full property | 873 | 7 |  | 6 | 109 | 2 | 230 | 140 | 36 | 11 |  |  |  |  |  |
| Sealed class | 233 | 194 | 118 | 69 | 169 | 40 | 201 | 194 | 44 | 30 | 11 | 3 |  | 4 |  |
| foreach | 183 | 49 | 15 | 224 | 292 | 286 | 24 | 145 | 35 | 40 | 1 |  |  |  |  |
| Class | 268 | 197 | 118 | 140 | 85 | 97 | 64 | 22 | 43 | 91 | 13 | 3 | 31 | 1 |  |
| Conditional ?: | 217 | 11 | 11 | 208 | 224 | 134 | 136 | 125 | 58 | 25 | 1 | 2 |  |  |  |
| Null-conditional ?. ?[ | 171 | 128 | 68 | 49 | 47 | 82 | 434 | 35 | 73 | 7 | 5 | 1 |  |  |  |
| Tuple literal or tuple type | 38 | 383 | 198 | 79 | 67 | 138 | 49 | 36 | 76 | 21 | 10 |  |  |  |  |
| Constant pattern | 131 | 3 | 8 | 103 | 38 | 296 | 109 | 120 | 43 | 17 |  | 5 |  |  |  |
| Null-coalescing ?? | 80 | 18 | 11 | 91 | 170 | 67 | 83 | 246 | 38 | 38 | 5 |  |  | 1 |  |
| not pattern | 308 | 4 |  | 43 | 57 | 164 | 120 | 31 | 48 | 6 | 1 |  |  |  |  |
| Auto property | 343 | 5 |  | 8 | 7 | 23 | 77 | 225 | 42 | 13 |  |  |  |  |  |
| Static lambda | 78 | 26 | 319 | 2 | 97 | 15 | 59 | 37 | 13 | 40 | 1 |  |  |  |  |
| is null | 119 | 7 | 2 | 81 | 147 | 159 | 69 | 37 | 50 | 13 | 2 |  |  |  |  |
| Null-forgiving ! | 157 | 201 | 176 | 52 | 9 | 23 | 5 |  | 3 | 41 | 1 |  |  |  |  |
| Interpolated string | 19 | 49 | 6 | 158 | 51 | 349 |  | 6 |  | 2 |  |  |  |  |  |
| Extension method |  |  |  |  |  |  |  |  |  | 568 | 7 |  |  |  |  |
| with expression | 7 | 172 | 18 | 42 | 240 | 9 | 12 | 17 | 32 | 25 |  |  |  |  |  |
| Raw string | 2 | 107 | 7 | 210 |  | 203 |  |  |  | 8 |  |  |  |  |  |
| await | 55 | 223 | 116 | 32 | 31 | 1 | 10 |  | 15 | 5 | 1 | 10 |  |  |  |
| Nullable value type T? | 53 | 20 | 19 | 39 | 34 | 10 | 119 | 29 | 101 | 62 | 7 |  |  | 2 |  |
| Const field | 75 | 58 | 11 | 153 | 7 | 102 | 3 | 64 | 4 | 10 |  | 4 |  | 1 |  |
| Record class | 16 |  |  | 2 | 103 | 6 | 143 | 191 | 1 | 2 |  |  |  | 3 |  |
| Constructor | 164 | 2 |  | 60 | 66 | 7 | 55 | 3 | 39 | 7 |  | 2 | 31 |  |  |
| Property pattern | 142 | 2 |  | 6 | 14 | 235 | 8 | 3 | 4 | 1 | 1 |  |  |  |  |
| Optional parameter | 8 | 11 | 3 | 6 | 9 | 1 | 4 | 245 | 16 | 93 |  |  |  | 3 |  |
| Async method | 54 | 175 | 83 | 19 | 23 | 1 | 10 |  | 11 | 5 | 1 | 5 |  |  |  |
| nameof | 223 | 1 |  | 5 | 116 | 16 | 16 | 1 | 4 | 1 | 1 |  |  |  |  |
| Cast expression | 146 | 4 | 61 | 71 | 2 | 41 | 2 | 4 |  | 25 | 13 | 1 |  |  |  |
| Object initializer | 135 | 36 | 69 | 16 |  | 47 |  | 3 |  | 29 | 7 | 1 |  | 1 |  |
| lock | 13 |  |  | 2 | 75 | 5 | 2 |  | 227 | 5 |  |  |  |  |  |
| Deconstruction | 27 | 65 | 107 | 21 | 23 | 30 | 19 | 4 | 6 |  |  |  |  |  |  |
| catch | 21 | 1 |  | 112 | 27 | 8 | 84 | 1 | 25 | 1 | 6 | 4 |  | 3 |  |
| Static class | 43 | 3 |  | 73 | 18 | 63 | 6 | 19 |  | 61 | 2 |  |  |  |  |
| and / or pattern | 37 | 1 | 3 | 55 | 9 | 131 | 1 | 39 | 1 | 6 |  |  |  |  |  |
| try | 18 | 7 |  | 68 | 21 | 10 | 85 | 1 | 26 | 1 | 6 | 4 |  | 1 |  |
| for | 44 | 11 | 6 | 45 | 57 | 23 | 4 | 12 | 9 | 16 | 2 |  |  |  |  |
| Named argument |  | 87 | 18 | 16 | 23 | 16 |  | 18 | 2 | 46 |  |  |  |  |  |
| Init accessor | 3 |  |  |  |  | 3 |  | 209 |  |  |  |  |  |  |  |
| typeof | 161 |  | 1 | 3 |  | 3 |  |  |  | 1 | 6 |  |  |  |  |
| while | 10 | 14 | 4 | 91 | 9 | 19 | 2 | 16 | 1 | 5 |  |  |  |  |  |
| using statement |  | 35 | 1 | 113 | 7 |  |  |  |  |  | 1 |  |  |  |  |
| throw expression | 4 | 3 | 22 | 4 | 68 | 1 | 14 | 1 | 8 | 24 | 1 |  |  |  |  |
| Discard pattern | 14 |  | 1 | 9 | 14 | 60 | 14 | 24 | 9 | 1 |  | 2 |  |  |  |
| Spread element | 7 | 11 | 6 |  | 26 | 23 | 6 | 42 | 12 | 13 |  |  |  |  |  |
| Field-like event | 51 |  |  |  |  |  | 90 |  | 1 |  |  |  |  |  |  |
| Switch expression | 12 |  | 1 | 9 | 14 | 57 | 14 | 23 | 9 | 1 |  | 2 |  |  |  |
| Discard _ | 20 | 22 | 55 | 1 | 8 | 7 | 2 | 9 | 5 | 1 |  |  |  |  |  |
| Partial type | 48 |  |  | 5 |  | 7 |  |  | 4 | 26 |  |  | 31 |  |  |
| Case guard when | 6 |  |  |  | 1 | 94 |  | 3 |  | 2 |  |  |  |  |  |
| Verbatim string |  | 4 |  |  | 3 | 78 |  |  |  | 2 |  |  |  |  |  |
| Interface | 2 |  |  |  | 1 |  | 1 | 62 | 6 |  |  |  |  |  |  |
| Range .. |  | 2 | 16 | 7 | 2 | 27 |  | 18 |  |  |  |  |  |  |  |
| Relational pattern | 1 |  |  | 15 | 10 | 2 |  | 39 | 1 |  |  |  |  |  |  |
| as cast | 54 |  |  | 2 |  | 8 |  |  |  |  |  |  |  |  |  |
| Index from end ^ | 2 | 7 | 34 | 1 | 4 | 11 |  | 4 |  |  |  |  |  |  |  |
| Exception filter | 3 |  |  | 26 | 11 | 3 |  |  | 18 |  |  |  |  |  |  |
| Switch statement | 7 |  |  | 8 |  | 19 | 2 | 19 | 1 | 2 |  |  |  |  |  |
| Generic method | 26 |  |  |  | 13 | 1 | 4 | 4 | 2 | 4 |  |  |  |  |  |
| is type test | 21 | 1 |  | 1 |  | 30 |  |  |  |  |  |  |  |  |  |
| yield return / break | 9 |  |  | 7 | 7 | 23 |  | 2 |  |  |  |  |  |  |  |
| Local const |  | 23 | 1 | 4 |  | 4 | 1 |  |  |  |  |  |  |  |  |
| var pattern |  |  |  |  |  | 27 | 1 |  |  |  |  |  |  |  |  |
| Enum | 2 |  |  |  |  |  | 9 | 16 |  |  |  |  |  |  |  |
| finally |  | 6 |  | 2 | 8 | 2 | 1 |  | 4 |  |  | 2 |  |  |  |
| Nested type | 7 | 8 |  |  |  |  |  |  |  | 9 |  |  |  |  |  |
| ref or out parameter | 2 | 2 | 1 | 1 | 5 | 2 |  | 4 | 2 | 4 |  |  |  |  |  |
| Default interface method body |  |  |  |  |  |  |  |  | 19 |  |  |  |  |  |  |
| Constraint clause | 14 |  |  |  | 1 |  |  |  |  | 3 |  |  |  |  |  |
| Null-coalescing assignment ??= | 10 |  |  | 1 | 3 | 3 |  |  |  |  |  | 1 |  |  |  |
| Local function | 4 | 1 |  |  |  | 3 | 5 |  |  | 1 | 1 | 1 |  |  |  |
| List pattern | 4 | 1 |  |  |  | 8 |  |  | 1 |  |  |  |  |  |  |
| Conversion operator | 12 |  |  |  |  |  |  | 1 |  |  |  |  |  |  |  |
| Collection initializer | 3 |  | 2 | 3 |  |  |  |  |  | 1 | 2 |  |  |  |  |
| Type pattern | 1 |  |  |  |  | 10 |  |  |  |  |  |  |  |  |  |
| Positional pattern | 2 |  |  | 1 | 1 | 1 | 4 |  |  |  |  |  |  |  |  |
| default literal |  |  | 1 | 1 |  | 5 |  |  |  |  |  |  |  |  |  |
| Required member | 2 |  |  |  |  | 3 |  |  |  |  |  |  |  |  |  |
| Primary constructor on class or struct |  |  |  |  |  | 4 |  |  |  |  |  |  |  |  |  |
| Event accessors | 3 |  |  |  |  |  |  |  |  |  |  |  |  |  |  |
| Abstract class | 1 |  |  |  | 1 |  |  |  |  |  |  |  |  |  |  |
| Async lambda |  |  |  |  |  |  |  |  |  |  |  | 2 |  |  |  |
| checked / unchecked expression | 1 |  |  |  |  |  |  |  |  |  |  | 1 |  |  |  |
| Generic type | 1 |  |  |  |  |  | 1 |  |  |  |  |  |  |  |  |
| Record struct | 1 |  |  |  |  | 1 |  |  |  |  |  |  |  |  |  |
| Static constructor | 1 |  |  |  |  |  |  |  |  | 1 |  |  |  |  |  |
| Static local function | 2 |  |  |  |  |  |  |  |  |  |  |  |  |  |  |
| Indexer | 1 |  |  |  |  |  |  |  |  |  |  |  |  |  |  |
| Using alias |  |  |  |  |  |  |  |  |  |  |  |  |  |  | 1 |

## Style A share per project

| Style | Llyn.UIDeportment | Llyn.Tests.Engine | Llyn.Tests.Conduct | Llyn.Infrastructure | Llyn.Application | Llyn.Tests.Convention | Llyn.Conduct | Llyn.Core | Llyn.ShellEngine | Llyn.Tests.Interface | Llyn.Tests.Windows | Llyn.Core.Windows | Llyn.UIVeneer | Llyn.UIDeportment.Capsule | Llyn.Host |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| Null check | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - | - | - | - |
| Object creation | 50.92 % | 52.94 % | 19.41 % | 25.65 % | 28.60 % | 42.47 % | 9.80 % | 25.56 % | 29.48 % | 28.63 % | 37.50 % | 7.69 % | - | 14.29 % | 22.22 % |
| Collection creation | 94.74 % | 99.61 % | 99.16 % | 99.19 % | 100.00 % | 98.98 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 92.73 % | - | - | - | - |
| Member body | 32.41 % | 2.18 % | 1.90 % | 0.71 % | 11.40 % | 4.09 % | 20.82 % | 31.34 % | 24.94 % | 78.63 % | 37.88 % | 0.00 % | 0.00 % | 0.00 % | - |
| Local type | 4.63 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.35 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | - | 0.00 % | 0.00 % |
| Using | 100.00 % | 98.09 % | 99.96 % | 82.88 % | 63.16 % | 100.00 % | - | - | - | 100.00 % | 94.74 % | - | - | - | - |
| Namespace | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - |
| Text building | 54.29 % | 35.77 % | 16.22 % | 77.45 % | 83.61 % | 81.73 % | 0.00 % | 26.09 % | - | 22.22 % | - | 0.00 % | - | 0.00 % | - |
| Branch | 63.16 % | - | 100.00 % | 52.94 % | 100.00 % | 75.00 % | 87.50 % | 54.76 % | 90.00 % | 33.33 % | - | 100.00 % | - | - | - |
| Lambda body | 86.26 % | 96.54 % | 81.33 % | 85.71 % | 91.22 % | 98.90 % | 98.68 % | 95.65 % | 89.29 % | 80.31 % | 44.44 % | 50.00 % | - | - | 75.00 % |
| Type test | 91.65 % | 100.00 % | 100.00 % | 89.47 % | 100.00 % | 94.41 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - | - | - | - |
| Constructor | 0.00 % | 0.00 % | - | 0.00 % | 0.00 % | 36.36 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | - | 0.00 % | 0.00 % | - | - |
