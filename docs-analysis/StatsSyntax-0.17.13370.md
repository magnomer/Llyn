# Syntax statistics - 0.17.13370

- Generated: 2026-10-05 11:24:05 +09:00
- Source roots: src, tests
- Root-level files: none
- Project segments: 1
- Excluded directories: .git, .vs, artifacts, bin, node_modules, obj, packages, publish
- Excluded suffixes: .g.cs, .g.i.cs, .AssemblyInfo.cs, .GlobalUsings.g.cs, .Designer.cs
- Roslyn: 4.14.0-3.25262.10
- Language version: Preview
- Scanned: 1,658 files, 15 projects, 181,621 lines

## Summary

| Measure | Value |
|---|---:|
| Files | 1,658 |
| Lines | 181,621 |
| Nodes | 951,535 |
| Syntax kinds used | 190 |
| Features used | 98 of 138 |
| Minimum C# version | 12 |
| Parse errors | 0 |

## Families

| Family | Features | Used | Count | Per 1k lines |
|---|---:|---:|---:|---:|
| Types | 15 | 12 | 3,553 | 19.56 |
| Namespaces | 6 | 2 | 1,658 | 9.13 |
| Members | 29 | 20 | 9,085 | 50.02 |
| Patterns | 16 | 16 | 5,682 | 31.28 |
| Expressions | 36 | 29 | 22,260 | 122.56 |
| Statements | 20 | 13 | 11,480 | 63.21 |
| Generics | 6 | 2 | 75 | 0.41 |
| Async | 2 | 2 | 398 | 2.19 |
| Nullability | 3 | 2 | 3,632 | 20.00 |
| Unsafe | 2 | 0 | 0 | 0.00 |
| Directives | 3 | 0 | 0 | 0.00 |
| Total | 138 | 98 | 57,823 | 318.37 |

## Projects

| Project | Files | Lines | Nodes | Features used | Newest C# |
|---|---:|---:|---:|---:|---:|
| Llyn.UIDeportment | 281 | 32,475 | 150,917 | 86 | 12 |
| Llyn.Tests.Engine | 189 | 29,589 | 206,009 | 59 | 12 |
| Llyn.Tests.Conduct | 123 | 23,746 | 171,335 | 60 | 12 |
| Llyn.Infrastructure | 142 | 19,106 | 84,148 | 69 | 12 |
| Llyn.Tests.Convention | 112 | 17,315 | 84,616 | 79 | 12 |
| Llyn.Application | 106 | 16,739 | 76,576 | 66 | 12 |
| Llyn.Conduct | 223 | 14,812 | 57,583 | 58 | 12 |
| Llyn.Core | 291 | 10,059 | 40,031 | 58 | 12 |
| Llyn.ShellEngine | 50 | 8,348 | 34,062 | 58 | 12 |
| Llyn.Tests.Interface | 85 | 7,532 | 37,101 | 64 | 12 |
| Llyn.Tests.Windows | 17 | 1,073 | 6,455 | 38 | 12 |
| Llyn.Core.Windows | 3 | 341 | 1,251 | 26 | 10 |
| Llyn.UIVeneer | 31 | 339 | 672 | 4 | 10 |
| Llyn.UIDeportment.Capsule | 4 | 88 | 380 | 15 | 10 |
| Llyn.Host | 1 | 59 | 399 | 3 | 9 |
| Total | 1,658 | 181,621 | 951,535 | 98 | 12 |

## Versions

| C# version | Features | Used | Count |
|---|---:|---:|---:|
| 1 | 51 | 35 | 15,358 |
| 2 | 10 | 8 | 1,932 |
| 3 | 7 | 5 | 5,513 |
| 4 | 3 | 2 | 630 |
| 5 | 3 | 3 | 909 |
| 6 | 6 | 5 | 5,033 |
| 7 | 11 | 10 | 5,380 |
| 7.1 | 1 | 1 | 7 |
| 7.2 | 1 | 0 | 0 |
| 7.3 | 1 | 0 | 0 |
| 8 | 17 | 12 | 9,574 |
| 9 | 10 | 9 | 5,248 |
| 10 | 3 | 2 | 1,659 |
| 11 | 6 | 3 | 586 |
| 12 | 4 | 3 | 5,994 |
| 13 | 1 | 0 | 0 |
| 14 | 3 | 0 | 0 |

## Features

| Feature | Family | C# | Count | Per 1k lines | Files | First |
|---|---|---|---:|---:|---:|---|
| Collection expression | Expressions | 12 | 5,837 | 32.14 | 756 | src/Llyn.Application/Card/LCardClerk.cs:57 |
| using declaration | Statements | 8 | 4,789 | 26.37 | 363 | src/Llyn.Application/Card/LTranslationClerk.cs:159 |
| Lambda | Expressions | 3 | 3,775 | 20.79 | 595 | src/Llyn.Application/Card/LCardClerk.cs:174 |
| if | Statements | 1 | 3,713 | 20.44 | 591 | src/Llyn.Application/Card/LCardClerk.cs:70 |
| Nullable reference type T? | Nullability | 8 | 3,114 | 17.15 | 828 | src/Llyn.Application/Card/LCardClerkField.cs:91 |
| Expression-bodied member | Members | 6 | 2,780 | 15.31 | 430 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Target-typed new | Expressions | 9 | 1,878 | 10.34 | 465 | src/Llyn.Application/Card/LMentionClerk.cs:167 |
| Declaration pattern | Patterns | 7 | 1,774 | 9.77 | 330 | src/Llyn.Application/Card/LMeaningClerk.cs:93 |
| File-scoped namespace | Namespaces | 10 | 1,657 | 9.12 | 1,657 | src/Llyn.Application/Card/LCardClerk.cs:5 |
| Readonly field | Members | 1 | 1,523 | 8.39 | 487 | src/Llyn.Application/Card/LCardClerk.cs:9 |
| Full property | Members | 1 | 1,420 | 7.82 | 313 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Sealed class | Types | 1 | 1,328 | 7.31 | 1,226 | src/Llyn.Application/Card/LCardClerk.cs:7 |
| foreach | Statements | 1 | 1,319 | 7.26 | 412 | src/Llyn.Application/Card/LCardClerk.cs:59 |
| Class | Types | 1 | 1,197 | 6.59 | 1,179 | src/Llyn.Application/Card/LCardClerk.cs:7 |
| Tuple literal or tuple type | Expressions | 7 | 1,182 | 6.51 | 231 | src/Llyn.Application/Card/LMeaningClerk.cs:26 |
| Conditional ?: | Expressions | 1 | 1,181 | 6.50 | 459 | src/Llyn.Application/Card/LCardClerk.cs:173 |
| Null-conditional ?. ?[ | Expressions | 6 | 1,130 | 6.22 | 339 | src/Llyn.Application/Card/LMentionClerk.cs:47 |
| Constant pattern | Patterns | 7 | 949 | 5.23 | 169 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Static lambda | Expressions | 9 | 888 | 4.89 | 223 | src/Llyn.Application/Card/LCardEquality.cs:113 |
| Null-coalescing ?? | Expressions | 2 | 867 | 4.77 | 354 | src/Llyn.Application/Card/LCardClerkField.cs:74 |
| not pattern | Patterns | 9 | 814 | 4.48 | 322 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| Auto property | Members | 3 | 754 | 4.15 | 233 | src/Llyn.Application/Citation/LCitationClerk.cs:40 |
| Null-forgiving ! | Expressions | 8 | 715 | 3.94 | 261 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:97 |
| is null | Patterns | 7 | 709 | 3.90 | 307 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| Interpolated string | Expressions | 6 | 673 | 3.71 | 155 | src/Llyn.Application/Localization/LLocalizationReader.cs:55 |
| with expression | Expressions | 9 | 592 | 3.26 | 208 | src/Llyn.Application/Card/LCardClerk.cs:109 |
| Extension method | Members | 3 | 580 | 3.19 | 41 | tests/Llyn.Tests.Interface/TInterfaceArea.cs:7 |
| Raw string | Expressions | 11 | 567 | 3.12 | 129 | src/Llyn.Infrastructure/Database/Base/LDatabase.cs:51 |
| Nullable value type T? | Nullability | 2 | 518 | 2.85 | 226 | src/Llyn.Application/Card/LMeaningClerk.cs:92 |
| await | Expressions | 5 | 511 | 2.81 | 130 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:139 |
| Const field | Members | 1 | 505 | 2.78 | 215 | src/Llyn.Application/Card/LMentionClerk.cs:11 |
| Record class | Types | 9 | 469 | 2.58 | 382 | src/Llyn.Application/Ensign/LEnsignRow.cs:3 |
| Property pattern | Patterns | 8 | 452 | 2.49 | 104 | src/Llyn.Application/Draft/Clerk/LDraftClerk.cs:179 |
| Constructor | Members | 1 | 441 | 2.43 | 435 | src/Llyn.Application/Card/LCardClerk.cs:19 |
| Cast expression | Expressions | 1 | 405 | 2.23 | 140 | src/Llyn.Application/Markup/LMarkupClerkLink.cs:234 |
| Optional parameter | Members | 4 | 401 | 2.21 | 132 | src/Llyn.Application/Draft/Clerk/LDraftClerkPanel.cs:128 |
| Async method | Async | 5 | 392 | 2.16 | 130 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:91 |
| Object initializer | Expressions | 3 | 392 | 2.16 | 170 | src/Llyn.Core.Windows/LUsherShell.cs:37 |
| nameof | Expressions | 6 | 389 | 2.14 | 124 | src/Llyn.Application/Citation/LAuthorClerk.cs:361 |
| lock | Statements | 1 | 330 | 1.82 | 51 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:98 |
| catch | Statements | 1 | 315 | 1.73 | 131 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:142 |
| and / or pattern | Patterns | 9 | 312 | 1.72 | 90 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Deconstruction | Expressions | 7 | 311 | 1.71 | 126 | src/Llyn.Application/Card/LMeaningClerk.cs:46 |
| Static class | Types | 2 | 295 | 1.62 | 295 | src/Llyn.Application/Card/LCardClerkField.cs:7 |
| try | Statements | 1 | 271 | 1.49 | 140 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:137 |
| for | Statements | 1 | 235 | 1.29 | 145 | src/Llyn.Application/Card/LCardClerk.cs:247 |
| Named argument | Members | 4 | 229 | 1.26 | 76 | src/Llyn.Application/Card/LTranslationClerk.cs:163 |
| Init accessor | Members | 9 | 215 | 1.18 | 72 | src/Llyn.Core/Catalog/Kind/LCatalogAuthor.cs:13 |
| typeof | Expressions | 1 | 181 | 1.00 | 41 | src/Llyn.Infrastructure/Configuration/LLocalizationLoader.cs:20 |
| while | Statements | 1 | 177 | 0.97 | 104 | src/Llyn.Application/Card/LMeaningClerk.cs:44 |
| throw expression | Expressions | 7 | 162 | 0.89 | 82 | src/Llyn.Application/Card/LCardClerkField.cs:75 |
| Discard pattern | Patterns | 8 | 160 | 0.88 | 91 | src/Llyn.Application/Citation/LAuthorClerk.cs:243 |
| using statement | Statements | 1 | 158 | 0.87 | 66 | src/Llyn.Application/Citation/LAuthorCitation.cs:51 |
| Switch expression | Patterns | 8 | 154 | 0.85 | 90 | src/Llyn.Application/Citation/LAuthorClerk.cs:239 |
| Spread element | Expressions | 12 | 153 | 0.84 | 78 | src/Llyn.Application/Card/LCardEquality.cs:53 |
| Field-like event | Members | 1 | 148 | 0.81 | 82 | src/Llyn.Conduct/Atelier/CNavigation.cs:46 |
| Discard _ | Expressions | 7 | 135 | 0.74 | 46 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:60 |
| Partial type | Types | 2 | 125 | 0.69 | 125 | src/Llyn.Infrastructure/Database/Context/LSituationArchive.cs:8 |
| Case guard when | Patterns | 7 | 112 | 0.62 | 22 | src/Llyn.Application/Citation/LAuthorClerk.cs:241 |
| Verbatim string | Expressions | 1 | 87 | 0.48 | 18 | src/Llyn.Application/Localization/LLocalizationReader.cs:13 |
| Range .. | Expressions | 8 | 75 | 0.41 | 36 | src/Llyn.Application/Card/LMentionClerk.cs:173 |
| Interface | Types | 1 | 72 | 0.40 | 72 | src/Llyn.Application/Pronunciation/Relay/LListener.cs:5 |
| Relational pattern | Patterns | 9 | 69 | 0.38 | 26 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| as cast | Expressions | 1 | 68 | 0.37 | 44 | src/Llyn.Infrastructure/Database/Entry/LCollocationArchive.cs:163 |
| Index from end ^ | Expressions | 8 | 67 | 0.37 | 38 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:230 |
| Exception filter | Statements | 6 | 61 | 0.34 | 39 | src/Llyn.Application/Pronunciation/Clerk/LFanqieClerk.cs:342 |
| Switch statement | Patterns | 1 | 59 | 0.32 | 35 | src/Llyn.Conduct/Configuration/CLedger.cs:236 |
| Generic method | Generics | 2 | 55 | 0.30 | 28 | src/Llyn.Application/Card/LCardClerkField.cs:183 |
| is type test | Patterns | 1 | 55 | 0.30 | 33 | src/Llyn.Infrastructure/Pronunciation/Source/LRecordingArchive.cs:152 |
| yield return / break | Statements | 2 | 50 | 0.28 | 19 | src/Llyn.Application/Card/LCardClerkField.cs:35 |
| Local const | Statements | 1 | 35 | 0.19 | 20 | src/Llyn.Conduct/Lexicon/CStateWording.cs:10 |
| Enum | Types | 1 | 29 | 0.16 | 29 | src/Llyn.Conduct/Catalog/CCatalogOrder.cs:3 |
| var pattern | Patterns | 7 | 29 | 0.16 | 11 | src/Llyn.Conduct/Card/CFolio.cs:193 |
| Nested type | Types | 1 | 28 | 0.15 | 17 | src/Llyn.UIDeportment/Kit/Bind/QLook.cs:19 |
| finally | Statements | 1 | 27 | 0.15 | 23 | src/Llyn.Application/Pronunciation/Clerk/LFanqieClerk.cs:351 |
| ref or out parameter | Members | 1 | 26 | 0.14 | 20 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:101 |
| Constraint clause | Generics | 2 | 20 | 0.11 | 10 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:171 |
| Default interface method body | Members | 8 | 19 | 0.10 | 6 | src/Llyn.ShellEngine/Port/LDraftPort.cs:58 |
| Null-coalescing assignment ??= | Expressions | 8 | 18 | 0.10 | 16 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:160 |
| Local function | Members | 7 | 17 | 0.09 | 11 | src/Llyn.Conduct/Desk/CErrand.cs:54 |
| List pattern | Patterns | 11 | 14 | 0.08 | 12 | src/Llyn.ShellEngine/Engine/LDraftFacade.cs:66 |
| Conversion operator | Members | 1 | 13 | 0.07 | 2 | src/Llyn.Core/Lexicon/State/LStateValue.cs:49 |
| Collection initializer | Expressions | 3 | 12 | 0.07 | 10 | src/Llyn.Infrastructure/Database/Source/LAuthorArchive.cs:117 |
| Type pattern | Patterns | 9 | 11 | 0.06 | 4 | src/Llyn.UIDeportment/Sound/Accent/QAccentControl.cs:62 |
| Positional pattern | Patterns | 8 | 9 | 0.05 | 9 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:37 |
| default literal | Expressions | 7.1 | 7 | 0.04 | 5 | src/Llyn.Infrastructure/Pronunciation/Loader/LSourceLoader.cs:73 |
| Async lambda | Async | 5 | 6 | 0.03 | 3 | src/Llyn.Core.Windows/LPressBrowser.cs:31 |
| Required member | Members | 11 | 5 | 0.03 | 3 | src/Llyn.UIDeportment/Panel/Taxonomy/QMembershipItem.cs:27 |
| Event accessors | Members | 1 | 4 | 0.02 | 3 | src/Llyn.ShellEngine/Port/LSettingsOutlet.cs:71 |
| Primary constructor on class or struct | Types | 12 | 4 | 0.02 | 4 | tests/Llyn.Tests.Convention/TAuditFakeMember.cs:3 |
| Abstract class | Types | 1 | 2 | 0.01 | 2 | src/Llyn.Application/Request/Entry/LRequest.cs:3 |
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
| Null check | is null / is not null | 709 | == null / != null | 0 | 100.00 % |
| Object creation | Target-typed new() | 1,878 | new T() | 3,365 | 35.82 % |
| Collection creation | Collection expression | 5,837 | Collection initializer or array creation | 42 | 99.29 % |
| Member body | Expression body | 2,780 | Block body | 9,278 | 23.06 % |
| Local type | var | 42 | Explicit type | 18,311 | 0.23 % |
| Using | Declaration | 4,789 | Statement | 158 | 96.81 % |
| Namespace | File-scoped | 1,657 | Block | 0 | 100.00 % |
| Text building | Interpolated string | 673 | string.Format or + with a string literal | 320 | 67.77 % |
| Branch | Switch expression | 154 | Switch statement | 59 | 72.30 % |
| Lambda body | Expression | 3,363 | Block | 412 | 89.09 % |
| Type test | is pattern with designation | 991 | as | 68 | 93.58 % |
| Constructor | Primary | 4 | Explicit | 435 | 0.91 % |

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
| IdentifierName | 274,887 | 1,658 |
| Argument | 98,578 | 1,267 |
| SimpleMemberAccessExpression | 89,787 | 1,259 |
| ArgumentList | 61,735 | 1,300 |
| InvocationExpression | 56,599 | 1,268 |
| PredefinedType | 27,588 | 1,561 |
| ExpressionStatement | 27,362 | 1,157 |
| StringLiteralExpression | 21,577 | 933 |
| Parameter | 19,733 | 1,492 |
| VariableDeclaration | 18,818 | 1,092 |
| VariableDeclarator | 18,818 | 1,092 |
| EqualsValueClause | 18,093 | 1,153 |
| Block | 16,331 | 1,223 |
| LocalDeclarationStatement | 15,833 | 952 |
| ParameterList | 12,385 | 1,590 |
| MethodDeclaration | 10,527 | 1,260 |
| NumericLiteralExpression | 9,752 | 947 |
| GenericName | 9,409 | 1,118 |
| TypeArgumentList | 9,409 | 1,118 |
| QualifiedName | 7,769 | 1,658 |
| SimpleAssignmentExpression | 6,541 | 858 |
| ExpressionElement | 6,012 | 411 |
| CollectionExpression | 5,837 | 756 |
| ReturnStatement | 5,380 | 913 |
| UsingDirective | 5,188 | 1,413 |
| IfStatement | 3,713 | 591 |
| NullLiteralExpression | 3,688 | 756 |
| NullableType | 3,632 | 868 |
| ObjectCreationExpression | 3,365 | 727 |
| Attribute | 3,050 | 361 |
| AttributeList | 3,050 | 361 |
| SingleVariableDesignation | 2,971 | 490 |
| SimpleLambdaExpression | 2,864 | 530 |
| ArrowExpressionClause | 2,782 | 431 |
| BracketedArgumentList | 2,692 | 473 |
| FieldDeclaration | 2,449 | 638 |
| IsPatternExpression | 2,448 | 501 |
| PropertyDeclaration | 2,178 | 438 |
| ElementAccessExpression | 2,141 | 422 |
| ImplicitObjectCreationExpression | 1,878 | 465 |
| FalseLiteralExpression | 1,797 | 503 |
| DeclarationPattern | 1,774 | 330 |
| CompilationUnit | 1,658 | 1,658 |
| ConstantPattern | 1,658 | 388 |
| FileScopedNamespaceDeclaration | 1,657 | 1,657 |
| TrueLiteralExpression | 1,463 | 440 |
| EqualsExpression | 1,386 | 502 |
| InterpolatedStringText | 1,362 | 155 |
| ForEachStatement | 1,248 | 402 |
| LogicalAndExpression | 1,215 | 333 |
| ClassDeclaration | 1,197 | 1,179 |
| TupleExpression | 1,192 | 238 |
| Interpolation | 1,190 | 154 |
| ConditionalExpression | 1,181 | 459 |
| ConditionalAccessExpression | 1,130 | 339 |
| MemberBindingExpression | 1,130 | 339 |
| AddExpression | 1,119 | 294 |
| LogicalNotExpression | 1,089 | 355 |
| AddAssignmentExpression | 1,087 | 233 |
| AttributeArgument | 927 | 51 |
| DeclarationExpression | 919 | 236 |
| ParenthesizedLambdaExpression | 911 | 259 |
| AccessorList | 880 | 257 |
| GetAccessorDeclaration | 875 | 255 |
| SwitchExpressionArm | 874 | 90 |
| CoalesceExpression | 867 | 354 |
| LogicalOrExpression | 856 | 265 |
| NotPattern | 814 | 322 |
| SuppressNullableWarningExpression | 715 | 261 |
| TupleElement | 711 | 168 |
| InterpolatedStringExpression | 673 | 155 |
| WithExpression | 592 | 208 |
| WithInitializerExpression | 592 | 208 |
| NameColon | 589 | 170 |
| ArrayRankSpecifier | 568 | 216 |
| ArrayType | 567 | 216 |
| GreaterThanExpression | 564 | 261 |
| OmittedArraySizeExpression | 553 | 216 |
| ImplicitElementAccess | 551 | 101 |
| AwaitExpression | 511 | 130 |
| PostIncrementExpression | 478 | 214 |
| RecordDeclaration | 469 | 382 |
| RecursivePattern | 461 | 111 |
| PropertyPatternClause | 452 | 104 |
| CharacterLiteralExpression | 447 | 100 |
| ConstructorDeclaration | 443 | 437 |
| LessThanExpression | 429 | 195 |
| Subpattern | 409 | 103 |
| CastExpression | 405 | 140 |
| AttributeArgumentList | 395 | 51 |
| ObjectInitializerExpression | 392 | 170 |
| NotEqualsExpression | 387 | 161 |
| BaseList | 334 | 241 |
| LockStatement | 330 | 51 |
| SwitchSection | 318 | 35 |
| ParenthesizedExpression | 317 | 165 |
| CatchClause | 315 | 131 |
| CatchDeclaration | 312 | 129 |
| TupleType | 302 | 168 |
| BreakStatement | 300 | 59 |
| ContinueStatement | 292 | 139 |
| OrPattern | 280 | 87 |
| TryStatement | 271 | 140 |
| SimpleBaseType | 236 | 222 |
| ForStatement | 235 | 145 |
| InitAccessorDeclaration | 215 | 72 |
| EnumMemberDeclaration | 205 | 29 |
| TypeOfExpression | 181 | 41 |
| WhileStatement | 177 | 104 |
| CaseSwitchLabel | 172 | 23 |
| ThrowStatement | 168 | 88 |
| SubtractExpression | 163 | 78 |
| ThrowExpression | 162 | 82 |
| DiscardPattern | 160 | 91 |
| UsingStatement | 158 | 66 |
| SetAccessorDeclaration | 157 | 70 |
| SubtractAssignmentExpression | 157 | 66 |
| UnaryMinusExpression | 156 | 68 |
| SwitchExpression | 154 | 90 |
| SpreadElement | 153 | 78 |
| ThisExpression | 150 | 81 |
| EventFieldDeclaration | 148 | 82 |
| ElseClause | 137 | 110 |
| WhenClause | 112 | 22 |
| CasePatternSwitchLabel | 111 | 20 |
| LessThanOrEqualExpression | 108 | 77 |
| PrimaryConstructorBaseType | 102 | 19 |
| GreaterThanOrEqualExpression | 84 | 57 |
| RangeExpression | 75 | 36 |
| InterfaceDeclaration | 72 | 72 |
| ForEachVariableStatement | 71 | 48 |
| MultiplyExpression | 71 | 29 |
| RelationalPattern | 69 | 26 |
| AsExpression | 68 | 44 |
| IndexExpression | 67 | 38 |
| CatchFilterClause | 61 | 39 |
| TypeParameter | 60 | 30 |
| SwitchStatement | 59 | 35 |
| TypeParameterList | 57 | 30 |
| IsExpression | 55 | 33 |
| ArrayInitializerExpression | 49 | 24 |
| BitwiseOrExpression | 47 | 24 |
| YieldReturnStatement | 43 | 19 |
| DefaultSwitchLabel | 41 | 22 |
| OrAssignmentExpression | 37 | 21 |
| ParenthesizedPattern | 37 | 16 |
| ExpressionColon | 33 | 14 |
| AndPattern | 32 | 12 |
| LeftShiftExpression | 32 | 2 |
| DivideExpression | 29 | 17 |
| EnumDeclaration | 29 | 29 |
| VarPattern | 29 | 11 |
| ImplicitArrayCreationExpression | 28 | 22 |
| FinallyClause | 27 | 23 |
| ArrayCreationExpression | 22 | 16 |
| TypeParameterConstraintClause | 20 | 10 |
| ClassConstraint | 19 | 9 |
| CoalesceAssignmentExpression | 18 | 16 |
| LocalFunctionStatement | 17 | 11 |
| AttributeTargetSpecifier | 15 | 7 |
| ListPattern | 14 | 12 |
| BaseExpression | 13 | 5 |
| ConversionOperatorDeclaration | 13 | 2 |
| CollectionInitializerExpression | 12 | 10 |
| TypePattern | 11 | 4 |
| PositionalPatternClause | 9 | 9 |
| DefaultLiteralExpression | 7 | 5 |
| InterpolationFormatClause | 7 | 1 |
| YieldBreakStatement | 7 | 6 |
| BaseConstructorInitializer | 6 | 6 |
| GlobalStatement | 6 | 1 |
| NameEquals | 6 | 4 |
| PostDecrementExpression | 6 | 5 |
| AddAccessorDeclaration | 4 | 3 |
| EventDeclaration | 4 | 3 |
| ExplicitInterfaceSpecifier | 4 | 3 |
| RemoveAccessorDeclaration | 4 | 3 |
| InterpolationAlignmentClause | 3 | 1 |
| PreIncrementExpression | 3 | 3 |
| SlicePattern | 3 | 3 |
| ModuloExpression | 2 | 1 |
| RecordStructDeclaration | 2 | 2 |
| ThisConstructorInitializer | 2 | 2 |
| UncheckedExpression | 2 | 2 |
| AndAssignmentExpression | 1 | 1 |
| BitwiseAndExpression | 1 | 1 |
| BracketedParameterList | 1 | 1 |
| IndexerDeclaration | 1 | 1 |
| OmittedTypeArgument | 1 | 1 |
| TypeConstraint | 1 | 1 |

## Attributes

| Attribute | Count | Files |
|---|---:|---:|
| Fact | 2,578 | 348 |
| InlineData | 374 | 41 |
| Theory | 76 | 43 |
| JsonPropertyName | 11 | 3 |
| MemberData | 3 | 2 |
| CollectionBehavior | 2 | 2 |
| DllImport | 2 | 1 |
| Flags | 1 | 1 |
| MarshalAs | 1 | 1 |
| NotNullIfNotNull | 1 | 1 |
| NotNullWhen | 1 | 1 |

## Parse errors (0)

None.

## Feature counts per project

| Feature | Llyn.UIDeportment | Llyn.Tests.Engine | Llyn.Tests.Conduct | Llyn.Infrastructure | Llyn.Tests.Convention | Llyn.Application | Llyn.Conduct | Llyn.Core | Llyn.ShellEngine | Llyn.Tests.Interface | Llyn.Tests.Windows | Llyn.Core.Windows | Llyn.UIVeneer | Llyn.UIDeportment.Capsule | Llyn.Host |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| Collection expression | 148 | 2,289 | 1,557 | 245 | 523 | 303 | 157 | 266 | 82 | 206 | 61 |  |  |  |  |
| using declaration | 5 | 1,798 | 2,381 | 547 | 7 | 12 |  |  |  | 18 | 21 |  |  |  |  |
| Lambda | 179 | 673 | 1,229 | 21 | 730 | 296 | 317 | 92 | 58 | 137 | 31 | 4 |  |  | 8 |
| if | 1,186 | 49 | 6 | 525 | 464 | 655 | 365 | 216 | 206 | 26 | 3 | 9 |  | 3 |  |
| Nullable reference type T? | 456 | 146 | 353 | 270 | 297 | 302 | 309 | 312 | 253 | 379 | 27 | 6 |  | 4 |  |
| Expression-bodied member | 938 | 33 | 35 | 6 | 26 | 109 | 247 | 152 | 203 | 1,003 | 28 |  |  |  |  |
| Target-typed new | 717 | 140 | 185 | 118 | 189 | 137 | 33 | 46 | 74 | 202 | 31 | 1 |  | 1 | 4 |
| Declaration pattern | 896 | 5 | 2 | 23 | 368 | 191 | 187 | 21 | 70 | 8 | 3 |  |  |  |  |
| File-scoped namespace | 281 | 189 | 123 | 142 | 112 | 106 | 223 | 291 | 50 | 85 | 17 | 3 | 31 | 4 |  |
| Readonly field | 433 | 33 | 4 | 100 | 215 | 355 | 228 | 28 | 88 | 35 |  | 3 |  | 1 |  |
| Full property | 873 | 7 | 4 | 6 | 2 | 109 | 232 | 140 | 36 | 11 |  |  |  |  |  |
| Sealed class | 233 | 194 | 126 | 69 | 43 | 169 | 205 | 194 | 44 | 30 | 14 | 3 |  | 4 |  |
| foreach | 184 | 49 | 18 | 224 | 306 | 292 | 25 | 145 | 35 | 40 | 1 |  |  |  |  |
| Class | 268 | 197 | 124 | 140 | 105 | 85 | 68 | 22 | 43 | 93 | 17 | 3 | 31 | 1 |  |
| Tuple literal or tuple type | 38 | 381 | 252 | 79 | 168 | 67 | 52 | 36 | 76 | 23 | 10 |  |  |  |  |
| Conditional ?: | 214 | 11 | 12 | 208 | 156 | 224 | 142 | 125 | 58 | 27 | 2 | 2 |  |  |  |
| Null-conditional ?. ?[ | 174 | 129 | 72 | 49 | 91 | 47 | 442 | 35 | 76 | 8 | 6 | 1 |  |  |  |
| Constant pattern | 138 | 3 | 8 | 103 | 353 | 38 | 121 | 120 | 43 | 17 |  | 5 |  |  |  |
| Static lambda | 80 | 26 | 511 | 2 | 15 | 97 | 60 | 37 | 13 | 44 | 3 |  |  |  |  |
| Null-coalescing ?? | 77 | 18 | 12 | 91 | 80 | 170 | 91 | 246 | 38 | 38 | 5 |  |  | 1 |  |
| not pattern | 308 | 4 | 1 | 43 | 185 | 57 | 129 | 31 | 49 | 6 | 1 |  |  |  |  |
| Auto property | 344 | 5 | 3 | 8 | 23 | 7 | 81 | 225 | 42 | 16 |  |  |  |  |  |
| Null-forgiving ! | 163 | 205 | 206 | 52 | 25 | 9 | 5 |  | 3 | 46 | 1 |  |  |  |  |
| is null | 122 | 7 | 3 | 81 | 175 | 147 | 71 | 37 | 51 | 13 | 2 |  |  |  |  |
| Interpolated string | 19 | 49 | 7 | 158 | 380 | 51 |  | 6 |  | 3 |  |  |  |  |  |
| with expression | 7 | 176 | 28 | 42 | 10 | 240 | 13 | 17 | 34 | 25 |  |  |  |  |  |
| Extension method |  |  |  |  |  |  |  |  |  | 572 | 8 |  |  |  |  |
| Raw string | 2 | 107 | 7 | 210 | 233 |  |  |  |  | 8 |  |  |  |  |  |
| Nullable value type T? | 56 | 20 | 21 | 39 | 11 | 33 | 135 | 29 | 101 | 64 | 7 |  |  | 2 |  |
| await | 55 | 223 | 127 | 32 | 1 | 31 | 11 |  | 15 | 5 | 1 | 10 |  |  |  |
| Const field | 74 | 58 | 12 | 155 | 113 | 7 | 3 | 64 | 4 | 10 |  | 4 |  | 1 |  |
| Record class | 16 |  | 2 | 2 | 6 | 103 | 143 | 191 | 1 | 2 |  |  |  | 3 |  |
| Property pattern | 143 | 2 |  | 6 | 270 | 14 | 8 | 3 | 4 | 1 | 1 |  |  |  |  |
| Constructor | 164 | 2 | 1 | 60 | 7 | 66 | 59 | 3 | 39 | 7 |  | 2 | 31 |  |  |
| Cast expression | 142 | 6 | 83 | 71 | 50 | 2 | 3 | 4 |  | 29 | 14 | 1 |  |  |  |
| Optional parameter | 8 | 11 | 3 | 6 | 1 | 9 | 4 | 247 | 16 | 93 |  |  |  | 3 |  |
| Async method | 54 | 175 | 87 | 19 | 1 | 23 | 11 |  | 11 | 5 | 1 | 5 |  |  |  |
| Object initializer | 135 | 41 | 94 | 17 | 60 |  |  | 3 |  | 32 | 8 | 1 |  | 1 |  |
| nameof | 223 | 1 | 3 | 5 | 17 | 116 | 16 | 1 | 4 | 2 | 1 |  |  |  |  |
| lock | 13 |  |  | 2 | 5 | 75 | 2 |  | 228 | 5 |  |  |  |  |  |
| catch | 21 | 1 |  | 112 | 8 | 25 | 106 | 1 | 26 | 1 | 7 | 4 |  | 3 |  |
| and / or pattern | 37 | 1 | 3 | 55 | 159 | 9 | 2 | 39 | 1 | 6 |  |  |  |  |  |
| Deconstruction | 27 | 65 | 113 | 21 | 33 | 23 | 19 | 4 | 6 |  |  |  |  |  |  |
| Static class | 43 | 3 |  | 73 | 68 | 18 | 6 | 19 |  | 62 | 3 |  |  |  |  |
| try | 19 | 7 |  | 68 | 10 | 19 | 107 | 1 | 27 | 1 | 7 | 4 |  | 1 |  |
| for | 44 | 11 | 6 | 45 | 27 | 57 | 6 | 12 | 9 | 16 | 2 |  |  |  |  |
| Named argument |  | 87 | 21 | 16 | 16 | 23 |  | 18 | 2 | 46 |  |  |  |  |  |
| Init accessor | 3 |  |  |  | 3 |  |  | 209 |  |  |  |  |  |  |  |
| typeof | 161 |  | 4 | 3 | 3 |  |  |  |  | 4 | 6 |  |  |  |  |
| while | 9 | 14 | 5 | 91 | 25 | 9 | 2 | 16 | 1 | 5 |  |  |  |  |  |
| throw expression | 4 | 4 | 33 | 4 | 1 | 68 | 14 | 1 | 8 | 24 | 1 |  |  |  |  |
| Discard pattern | 15 |  | 1 | 9 | 67 | 14 | 18 | 24 | 9 | 1 |  | 2 |  |  |  |
| using statement |  | 35 | 2 | 113 |  | 7 |  |  |  |  | 1 |  |  |  |  |
| Switch expression | 13 |  | 1 | 9 | 65 | 14 | 17 | 23 | 9 | 1 |  | 2 |  |  |  |
| Spread element | 9 | 11 | 8 |  | 25 | 26 | 6 | 42 | 12 | 13 | 1 |  |  |  |  |
| Field-like event | 53 |  |  |  |  |  | 91 |  | 4 |  |  |  |  |  |  |
| Discard _ | 20 | 22 | 58 | 1 | 9 | 8 | 2 | 9 | 5 | 1 |  |  |  |  |  |
| Partial type | 48 |  | 4 | 5 | 7 |  |  |  | 4 | 26 |  |  | 31 |  |  |
| Case guard when | 6 |  |  |  | 100 | 1 |  | 3 |  | 2 |  |  |  |  |  |
| Verbatim string |  | 4 |  |  | 78 | 3 |  |  |  | 2 |  |  |  |  |  |
| Range .. |  | 2 | 16 | 7 | 30 | 2 |  | 18 |  |  |  |  |  |  |  |
| Interface | 2 |  |  |  |  | 1 | 1 | 62 | 6 |  |  |  |  |  |  |
| Relational pattern | 1 |  |  | 15 | 3 | 10 |  | 39 | 1 |  |  |  |  |  |  |
| as cast | 57 |  |  | 2 | 9 |  |  |  |  |  |  |  |  |  |  |
| Index from end ^ | 2 | 7 | 38 | 1 | 11 | 4 |  | 4 |  |  |  |  |  |  |  |
| Exception filter | 3 |  |  | 26 | 3 | 11 |  |  | 18 |  |  |  |  |  |  |
| Switch statement | 7 |  |  | 8 | 20 |  | 2 | 19 | 1 | 2 |  |  |  |  |  |
| Generic method | 25 |  | 1 |  | 1 | 13 | 4 | 4 | 2 | 5 |  |  |  |  |  |
| is type test | 21 | 1 |  | 1 | 32 |  |  |  |  |  |  |  |  |  |  |
| yield return / break | 9 |  |  | 7 | 25 | 7 |  | 2 |  |  |  |  |  |  |  |
| Local const |  | 23 | 1 | 4 | 6 |  | 1 |  |  |  |  |  |  |  |  |
| Enum | 2 |  |  |  |  |  | 11 | 16 |  |  |  |  |  |  |  |
| var pattern |  |  |  |  | 28 |  | 1 |  |  |  |  |  |  |  |  |
| Nested type | 7 | 8 | 3 |  |  |  |  |  |  | 10 |  |  |  |  |  |
| finally | 1 | 6 |  | 2 | 2 | 8 | 2 |  | 4 |  |  | 2 |  |  |  |
| ref or out parameter | 2 | 2 | 1 | 1 | 3 | 6 |  | 4 | 3 | 4 |  |  |  |  |  |
| Constraint clause | 14 |  | 1 |  |  | 1 |  |  |  | 4 |  |  |  |  |  |
| Default interface method body |  |  |  |  |  |  |  |  | 19 |  |  |  |  |  |  |
| Null-coalescing assignment ??= | 10 |  |  | 1 | 3 | 3 |  |  |  |  |  | 1 |  |  |  |
| Local function | 4 | 1 |  |  | 4 |  | 5 |  |  | 1 | 1 | 1 |  |  |  |
| List pattern | 4 | 1 |  |  | 8 |  |  |  | 1 |  |  |  |  |  |  |
| Conversion operator | 12 |  |  |  |  |  |  | 1 |  |  |  |  |  |  |  |
| Collection initializer | 3 |  | 3 | 3 |  |  |  |  |  | 1 | 2 |  |  |  |  |
| Type pattern | 1 |  |  |  | 10 |  |  |  |  |  |  |  |  |  |  |
| Positional pattern | 2 |  |  | 1 | 1 | 1 | 4 |  |  |  |  |  |  |  |  |
| default literal |  |  | 1 | 1 | 5 |  |  |  |  |  |  |  |  |  |  |
| Async lambda |  |  | 4 |  |  |  |  |  |  |  |  | 2 |  |  |  |
| Required member | 2 |  |  |  | 3 |  |  |  |  |  |  |  |  |  |  |
| Event accessors | 3 |  |  |  |  |  |  |  | 1 |  |  |  |  |  |  |
| Primary constructor on class or struct |  |  |  |  | 4 |  |  |  |  |  |  |  |  |  |  |
| Abstract class | 1 |  |  |  |  | 1 |  |  |  |  |  |  |  |  |  |
| checked / unchecked expression | 1 |  |  |  |  |  |  |  |  |  |  | 1 |  |  |  |
| Generic type | 1 |  |  |  |  |  | 1 |  |  |  |  |  |  |  |  |
| Record struct | 1 |  |  |  | 1 |  |  |  |  |  |  |  |  |  |  |
| Static constructor | 1 |  |  |  |  |  |  |  |  | 1 |  |  |  |  |  |
| Static local function | 2 |  |  |  |  |  |  |  |  |  |  |  |  |  |  |
| Indexer | 1 |  |  |  |  |  |  |  |  |  |  |  |  |  |  |
| Using alias |  |  |  |  |  |  |  |  |  |  |  |  |  |  | 1 |

## Style A share per project

| Style | Llyn.UIDeportment | Llyn.Tests.Engine | Llyn.Tests.Conduct | Llyn.Infrastructure | Llyn.Tests.Convention | Llyn.Application | Llyn.Conduct | Llyn.Core | Llyn.ShellEngine | Llyn.Tests.Interface | Llyn.Tests.Windows | Llyn.Core.Windows | Llyn.UIVeneer | Llyn.UIDeportment.Capsule | Llyn.Host |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| Null check | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - | - | - | - |
| Object creation | 51.00 % | 52.43 % | 32.46 % | 25.65 % | 42.00 % | 28.60 % | 9.40 % | 25.56 % | 29.48 % | 28.13 % | 42.47 % | 7.69 % | - | 14.29 % | 22.22 % |
| Collection creation | 94.87 % | 99.61 % | 99.11 % | 99.19 % | 99.05 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 93.85 % | - | - | - | - |
| Member body | 32.41 % | 2.17 % | 2.82 % | 0.71 % | 3.69 % | 11.40 % | 20.55 % | 31.34 % | 25.25 % | 78.42 % | 38.36 % | 0.00 % | 0.00 % | 0.00 % | - |
| Local type | 4.58 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.32 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | - | 0.00 % | 0.00 % |
| Using | 100.00 % | 98.09 % | 99.92 % | 82.88 % | 100.00 % | 63.16 % | - | - | - | 100.00 % | 95.45 % | - | - | - | - |
| Namespace | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - |
| Text building | 55.88 % | 35.77 % | 17.95 % | 77.45 % | 82.07 % | 83.61 % | 0.00 % | 26.09 % | - | 27.27 % | - | 0.00 % | - | 0.00 % | - |
| Branch | 65.00 % | - | 100.00 % | 52.94 % | 76.47 % | 100.00 % | 89.47 % | 54.76 % | 90.00 % | 33.33 % | - | 100.00 % | - | - | - |
| Lambda body | 83.80 % | 95.99 % | 79.25 % | 85.71 % | 99.04 % | 91.22 % | 98.11 % | 95.65 % | 89.66 % | 79.56 % | 45.16 % | 50.00 % | - | - | 75.00 % |
| Type test | 91.22 % | 100.00 % | 100.00 % | 89.47 % | 94.51 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - | - | - | - |
| Constructor | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 36.36 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | - | 0.00 % | 0.00 % | - | - |
