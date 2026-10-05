# Syntax statistics - 0.17.13435

- Generated: 2026-10-05 19:26:44 +09:00
- Source roots: src, tests
- Root-level files: none
- Project segments: 1
- Excluded directories: .git, .vs, artifacts, bin, node_modules, obj, packages, publish
- Excluded suffixes: .g.cs, .g.i.cs, .AssemblyInfo.cs, .GlobalUsings.g.cs, .Designer.cs
- Roslyn: 4.14.0-3.25262.10
- Language version: Preview
- Scanned: 1,679 files, 15 projects, 183,475 lines

## Summary

| Measure | Value |
|---|---:|
| Files | 1,679 |
| Lines | 183,475 |
| Nodes | 960,022 |
| Syntax kinds used | 191 |
| Features used | 98 of 138 |
| Minimum C# version | 12 |
| Parse errors | 0 |

## Families

| Family | Features | Used | Count | Per 1k lines |
|---|---:|---:|---:|---:|
| Types | 15 | 12 | 3,590 | 19.57 |
| Namespaces | 6 | 2 | 1,679 | 9.15 |
| Members | 29 | 20 | 9,156 | 49.90 |
| Patterns | 16 | 16 | 5,755 | 31.37 |
| Expressions | 36 | 29 | 22,487 | 122.56 |
| Statements | 20 | 13 | 11,652 | 63.51 |
| Generics | 6 | 2 | 75 | 0.41 |
| Async | 2 | 2 | 421 | 2.29 |
| Nullability | 3 | 2 | 3,667 | 19.99 |
| Unsafe | 2 | 0 | 0 | 0.00 |
| Directives | 3 | 0 | 0 | 0.00 |
| Total | 138 | 98 | 58,482 | 318.75 |

## Projects

| Project | Files | Lines | Nodes | Features used | Newest C# |
|---|---:|---:|---:|---:|---:|
| Llyn.UIDeportment | 282 | 32,574 | 151,319 | 86 | 12 |
| Llyn.Tests.Engine | 189 | 29,589 | 206,009 | 59 | 12 |
| Llyn.Tests.Conduct | 123 | 23,746 | 171,335 | 60 | 12 |
| Llyn.Infrastructure | 145 | 20,028 | 88,730 | 71 | 12 |
| Llyn.Tests.Convention | 112 | 17,326 | 84,638 | 79 | 12 |
| Llyn.Application | 107 | 17,099 | 78,327 | 68 | 12 |
| Llyn.Conduct | 225 | 14,992 | 58,238 | 58 | 12 |
| Llyn.Core | 302 | 10,181 | 40,406 | 58 | 12 |
| Llyn.ShellEngine | 51 | 8,422 | 34,397 | 58 | 12 |
| Llyn.Tests.Interface | 86 | 7,573 | 37,278 | 64 | 12 |
| Llyn.Tests.Windows | 17 | 1,073 | 6,455 | 38 | 12 |
| Llyn.Core.Windows | 4 | 385 | 1,427 | 26 | 10 |
| Llyn.UIVeneer | 31 | 339 | 672 | 4 | 10 |
| Llyn.UIDeportment.Capsule | 4 | 88 | 380 | 15 | 10 |
| Llyn.Host | 1 | 60 | 411 | 3 | 9 |
| Total | 1,679 | 183,475 | 960,022 | 98 | 12 |

## Versions

| C# version | Features | Used | Count |
|---|---:|---:|---:|
| 1 | 51 | 35 | 15,606 |
| 2 | 10 | 8 | 1,942 |
| 3 | 7 | 5 | 5,534 |
| 4 | 3 | 2 | 634 |
| 5 | 3 | 3 | 981 |
| 6 | 6 | 5 | 5,059 |
| 7 | 11 | 10 | 5,504 |
| 7.1 | 1 | 1 | 7 |
| 7.2 | 1 | 0 | 0 |
| 7.3 | 1 | 0 | 0 |
| 8 | 17 | 12 | 9,638 |
| 9 | 10 | 9 | 5,305 |
| 10 | 3 | 2 | 1,680 |
| 11 | 6 | 3 | 586 |
| 12 | 4 | 3 | 6,006 |
| 13 | 1 | 0 | 0 |
| 14 | 3 | 0 | 0 |

## Features

| Feature | Family | C# | Count | Per 1k lines | Files | First |
|---|---|---|---:|---:|---:|---|
| Collection expression | Expressions | 12 | 5,847 | 31.87 | 760 | src/Llyn.Application/Card/LCardClerk.cs:57 |
| using declaration | Statements | 8 | 4,810 | 26.22 | 365 | src/Llyn.Application/Card/LTranslationClerk.cs:159 |
| Lambda | Expressions | 3 | 3,780 | 20.60 | 599 | src/Llyn.Application/Card/LCardClerk.cs:174 |
| if | Statements | 1 | 3,775 | 20.58 | 601 | src/Llyn.Application/Card/LCardClerk.cs:70 |
| Nullable reference type T? | Nullability | 8 | 3,147 | 17.15 | 838 | src/Llyn.Application/Card/LCardClerkField.cs:91 |
| Expression-bodied member | Members | 6 | 2,789 | 15.20 | 432 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Target-typed new | Expressions | 9 | 1,896 | 10.33 | 469 | src/Llyn.Application/Card/LMentionClerk.cs:167 |
| Declaration pattern | Patterns | 7 | 1,783 | 9.72 | 332 | src/Llyn.Application/Card/LMeaningClerk.cs:93 |
| File-scoped namespace | Namespaces | 10 | 1,678 | 9.15 | 1,678 | src/Llyn.Application/Card/LCardClerk.cs:5 |
| Readonly field | Members | 1 | 1,545 | 8.42 | 496 | src/Llyn.Application/Card/LCardClerk.cs:9 |
| Full property | Members | 1 | 1,425 | 7.77 | 315 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Sealed class | Types | 1 | 1,344 | 7.33 | 1,242 | src/Llyn.Application/Card/LCardClerk.cs:7 |
| foreach | Statements | 1 | 1,338 | 7.29 | 417 | src/Llyn.Application/Card/LCardClerk.cs:59 |
| Tuple literal or tuple type | Expressions | 7 | 1,262 | 6.88 | 232 | src/Llyn.Application/Card/LMeaningClerk.cs:26 |
| Class | Types | 1 | 1,206 | 6.57 | 1,188 | src/Llyn.Application/Card/LCardClerk.cs:7 |
| Conditional ?: | Expressions | 1 | 1,195 | 6.51 | 463 | src/Llyn.Application/Card/LCardClerk.cs:173 |
| Null-conditional ?. ?[ | Expressions | 6 | 1,133 | 6.18 | 341 | src/Llyn.Application/Card/LMentionClerk.cs:47 |
| Constant pattern | Patterns | 7 | 965 | 5.26 | 173 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Static lambda | Expressions | 9 | 888 | 4.84 | 223 | src/Llyn.Application/Card/LCardEquality.cs:113 |
| Null-coalescing ?? | Expressions | 2 | 875 | 4.77 | 357 | src/Llyn.Application/Card/LCardClerkField.cs:74 |
| not pattern | Patterns | 9 | 826 | 4.50 | 325 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| Auto property | Members | 3 | 756 | 4.12 | 233 | src/Llyn.Application/Citation/LCitationClerk.cs:40 |
| Null-forgiving ! | Expressions | 8 | 720 | 3.92 | 263 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:97 |
| is null | Patterns | 7 | 719 | 3.92 | 309 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| Interpolated string | Expressions | 6 | 676 | 3.68 | 156 | src/Llyn.Application/Localization/LLocalizationReader.cs:55 |
| with expression | Expressions | 9 | 594 | 3.24 | 209 | src/Llyn.Application/Card/LCardClerk.cs:109 |
| Extension method | Members | 3 | 580 | 3.16 | 41 | tests/Llyn.Tests.Interface/TInterfaceArea.cs:7 |
| Raw string | Expressions | 11 | 567 | 3.09 | 129 | src/Llyn.Infrastructure/Database/Base/LDatabase.cs:51 |
| await | Expressions | 5 | 560 | 3.05 | 135 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:139 |
| Const field | Members | 1 | 527 | 2.87 | 221 | src/Llyn.Application/Card/LMentionClerk.cs:11 |
| Nullable value type T? | Nullability | 2 | 520 | 2.83 | 228 | src/Llyn.Application/Card/LMeaningClerk.cs:92 |
| Record class | Types | 9 | 476 | 2.59 | 389 | src/Llyn.Application/Ensign/LEnsignRow.cs:3 |
| Property pattern | Patterns | 8 | 453 | 2.47 | 105 | src/Llyn.Application/Draft/Clerk/LDraftClerk.cs:179 |
| Constructor | Members | 1 | 447 | 2.44 | 441 | src/Llyn.Application/Card/LCardClerk.cs:19 |
| Async method | Async | 5 | 415 | 2.26 | 135 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:91 |
| Cast expression | Expressions | 1 | 406 | 2.21 | 141 | src/Llyn.Application/Markup/LMarkupClerkLink.cs:234 |
| Optional parameter | Members | 4 | 405 | 2.21 | 134 | src/Llyn.Application/Draft/Clerk/LDraftClerkPanel.cs:128 |
| Object initializer | Expressions | 3 | 404 | 2.20 | 172 | src/Llyn.Core.Windows/LUsherShell.cs:37 |
| nameof | Expressions | 6 | 391 | 2.13 | 126 | src/Llyn.Application/Citation/LAuthorClerk.cs:361 |
| catch | Statements | 1 | 338 | 1.84 | 136 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:142 |
| lock | Statements | 1 | 333 | 1.81 | 52 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:98 |
| and / or pattern | Patterns | 9 | 324 | 1.77 | 95 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Deconstruction | Expressions | 7 | 313 | 1.71 | 127 | src/Llyn.Application/Card/LMeaningClerk.cs:46 |
| Static class | Types | 2 | 295 | 1.61 | 295 | src/Llyn.Application/Card/LCardClerkField.cs:7 |
| try | Statements | 1 | 293 | 1.60 | 145 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:137 |
| for | Statements | 1 | 239 | 1.30 | 148 | src/Llyn.Application/Card/LCardClerk.cs:247 |
| Named argument | Members | 4 | 229 | 1.25 | 76 | src/Llyn.Application/Card/LTranslationClerk.cs:163 |
| Init accessor | Members | 9 | 215 | 1.17 | 72 | src/Llyn.Core/Catalog/Kind/LCatalogAuthor.cs:13 |
| typeof | Expressions | 1 | 181 | 0.99 | 41 | src/Llyn.Infrastructure/Configuration/LLocalizationLoader.cs:20 |
| while | Statements | 1 | 180 | 0.98 | 107 | src/Llyn.Application/Card/LMeaningClerk.cs:44 |
| throw expression | Expressions | 7 | 168 | 0.92 | 84 | src/Llyn.Application/Card/LCardClerkField.cs:75 |
| Discard pattern | Patterns | 8 | 161 | 0.88 | 92 | src/Llyn.Application/Citation/LAuthorClerk.cs:243 |
| using statement | Statements | 1 | 159 | 0.87 | 66 | src/Llyn.Application/Citation/LAuthorCitation.cs:51 |
| Spread element | Expressions | 12 | 155 | 0.84 | 80 | src/Llyn.Application/Card/LCardEquality.cs:53 |
| Switch expression | Patterns | 8 | 155 | 0.84 | 91 | src/Llyn.Application/Citation/LAuthorClerk.cs:239 |
| Field-like event | Members | 1 | 149 | 0.81 | 83 | src/Llyn.Conduct/Atelier/CNavigation.cs:46 |
| Discard _ | Expressions | 7 | 135 | 0.74 | 46 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:60 |
| Partial type | Types | 2 | 125 | 0.68 | 125 | src/Llyn.Infrastructure/Database/Context/LSituationArchive.cs:8 |
| Case guard when | Patterns | 7 | 113 | 0.62 | 23 | src/Llyn.Application/Citation/LAuthorClerk.cs:241 |
| Verbatim string | Expressions | 1 | 87 | 0.47 | 18 | src/Llyn.Application/Localization/LLocalizationReader.cs:13 |
| Range .. | Expressions | 8 | 77 | 0.42 | 37 | src/Llyn.Application/Card/LMentionClerk.cs:173 |
| Interface | Types | 1 | 76 | 0.41 | 76 | src/Llyn.Application/Pronunciation/Relay/LListener.cs:5 |
| Relational pattern | Patterns | 9 | 75 | 0.41 | 27 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Exception filter | Statements | 6 | 70 | 0.38 | 41 | src/Llyn.Application/Outpost/LCourierClerk.cs:97 |
| as cast | Expressions | 1 | 69 | 0.38 | 45 | src/Llyn.Infrastructure/Database/Base/LWorkspaceArchive.cs:132 |
| Index from end ^ | Expressions | 8 | 67 | 0.37 | 38 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:230 |
| Switch statement | Patterns | 1 | 60 | 0.33 | 36 | src/Llyn.Conduct/Configuration/CLedger.cs:270 |
| is type test | Patterns | 1 | 58 | 0.32 | 35 | src/Llyn.Application/Outpost/LCourierClerk.cs:223 |
| Generic method | Generics | 2 | 55 | 0.30 | 28 | src/Llyn.Application/Card/LCardClerkField.cs:183 |
| yield return / break | Statements | 2 | 50 | 0.27 | 19 | src/Llyn.Application/Card/LCardClerkField.cs:35 |
| Local const | Statements | 1 | 35 | 0.19 | 20 | src/Llyn.Conduct/Lexicon/CStateWording.cs:10 |
| finally | Statements | 1 | 32 | 0.17 | 25 | src/Llyn.Application/Outpost/LCourierClerk.cs:73 |
| Enum | Types | 1 | 30 | 0.16 | 30 | src/Llyn.Conduct/Catalog/CCatalogOrder.cs:3 |
| var pattern | Patterns | 7 | 29 | 0.16 | 11 | src/Llyn.Conduct/Card/CFolio.cs:193 |
| Nested type | Types | 1 | 28 | 0.15 | 17 | src/Llyn.UIDeportment/Kit/Bind/QLook.cs:19 |
| ref or out parameter | Members | 1 | 26 | 0.14 | 20 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:101 |
| Constraint clause | Generics | 2 | 20 | 0.11 | 10 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:171 |
| Default interface method body | Members | 8 | 19 | 0.10 | 6 | src/Llyn.ShellEngine/Port/LDraftPort.cs:58 |
| Null-coalescing assignment ??= | Expressions | 8 | 18 | 0.10 | 16 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:160 |
| Local function | Members | 7 | 17 | 0.09 | 11 | src/Llyn.Conduct/Desk/CErrand.cs:54 |
| Collection initializer | Expressions | 3 | 14 | 0.08 | 12 | src/Llyn.Application/Outpost/LCourierClerk.cs:173 |
| List pattern | Patterns | 11 | 14 | 0.08 | 12 | src/Llyn.ShellEngine/Engine/LDraftFacade.cs:66 |
| Conversion operator | Members | 1 | 13 | 0.07 | 2 | src/Llyn.Core/Lexicon/State/LStateValue.cs:49 |
| Type pattern | Patterns | 9 | 11 | 0.06 | 4 | src/Llyn.UIDeportment/Sound/Accent/QAccentControl.cs:62 |
| Positional pattern | Patterns | 8 | 9 | 0.05 | 9 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:37 |
| default literal | Expressions | 7.1 | 7 | 0.04 | 5 | src/Llyn.Infrastructure/Pronunciation/Loader/LSourceLoader.cs:73 |
| Async lambda | Async | 5 | 6 | 0.03 | 3 | src/Llyn.Core.Windows/LPressBrowser.cs:31 |
| Required member | Members | 11 | 5 | 0.03 | 3 | src/Llyn.UIDeportment/Panel/Taxonomy/QMembershipItem.cs:27 |
| Event accessors | Members | 1 | 4 | 0.02 | 3 | src/Llyn.ShellEngine/Port/LSettingsOutlet.cs:74 |
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
| Null check | is null / is not null | 719 | == null / != null | 0 | 100.00 % |
| Object creation | Target-typed new() | 1,896 | new T() | 3,443 | 35.51 % |
| Collection creation | Collection expression | 5,847 | Collection initializer or array creation | 44 | 99.25 % |
| Member body | Expression body | 2,789 | Block body | 9,352 | 22.97 % |
| Local type | var | 42 | Explicit type | 18,478 | 0.23 % |
| Using | Declaration | 4,810 | Statement | 159 | 96.80 % |
| Namespace | File-scoped | 1,678 | Block | 0 | 100.00 % |
| Text building | Interpolated string | 676 | string.Format or + with a string literal | 363 | 65.06 % |
| Branch | Switch expression | 155 | Switch statement | 60 | 72.09 % |
| Lambda body | Expression | 3,368 | Block | 412 | 89.10 % |
| Type test | is pattern with designation | 998 | as | 69 | 93.53 % |
| Constructor | Primary | 4 | Explicit | 441 | 0.90 % |

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
| IdentifierName | 277,146 | 1,679 |
| Argument | 99,542 | 1,276 |
| SimpleMemberAccessExpression | 90,292 | 1,268 |
| ArgumentList | 62,152 | 1,309 |
| InvocationExpression | 56,928 | 1,278 |
| PredefinedType | 27,956 | 1,581 |
| ExpressionStatement | 27,496 | 1,166 |
| StringLiteralExpression | 21,810 | 941 |
| Parameter | 19,974 | 1,512 |
| VariableDeclaration | 19,004 | 1,101 |
| VariableDeclarator | 19,004 | 1,101 |
| EqualsValueClause | 18,255 | 1,162 |
| Block | 16,549 | 1,232 |
| LocalDeclarationStatement | 15,964 | 960 |
| ParameterList | 12,491 | 1,610 |
| MethodDeclaration | 10,620 | 1,273 |
| NumericLiteralExpression | 9,818 | 954 |
| GenericName | 9,479 | 1,130 |
| TypeArgumentList | 9,479 | 1,130 |
| QualifiedName | 7,872 | 1,679 |
| SimpleAssignmentExpression | 6,628 | 866 |
| ExpressionElement | 6,107 | 413 |
| CollectionExpression | 5,847 | 760 |
| ReturnStatement | 5,458 | 922 |
| UsingDirective | 5,253 | 1,428 |
| IfStatement | 3,775 | 601 |
| NullLiteralExpression | 3,726 | 761 |
| NullableType | 3,667 | 878 |
| ObjectCreationExpression | 3,443 | 734 |
| Attribute | 3,051 | 362 |
| AttributeList | 3,051 | 362 |
| SingleVariableDesignation | 2,994 | 496 |
| SimpleLambdaExpression | 2,869 | 534 |
| ArrowExpressionClause | 2,791 | 433 |
| BracketedArgumentList | 2,723 | 478 |
| FieldDeclaration | 2,498 | 648 |
| IsPatternExpression | 2,475 | 505 |
| PropertyDeclaration | 2,185 | 440 |
| ElementAccessExpression | 2,152 | 427 |
| ImplicitObjectCreationExpression | 1,896 | 469 |
| FalseLiteralExpression | 1,866 | 508 |
| DeclarationPattern | 1,783 | 332 |
| ConstantPattern | 1,684 | 392 |
| CompilationUnit | 1,679 | 1,679 |
| FileScopedNamespaceDeclaration | 1,678 | 1,678 |
| TrueLiteralExpression | 1,494 | 446 |
| EqualsExpression | 1,399 | 507 |
| InterpolatedStringText | 1,369 | 156 |
| TupleExpression | 1,272 | 239 |
| ForEachStatement | 1,265 | 407 |
| LogicalAndExpression | 1,234 | 336 |
| ClassDeclaration | 1,206 | 1,188 |
| Interpolation | 1,197 | 155 |
| ConditionalExpression | 1,195 | 463 |
| AddExpression | 1,187 | 300 |
| ConditionalAccessExpression | 1,133 | 341 |
| MemberBindingExpression | 1,133 | 341 |
| LogicalNotExpression | 1,106 | 361 |
| AddAssignmentExpression | 1,094 | 235 |
| DeclarationExpression | 933 | 241 |
| AttributeArgument | 928 | 52 |
| ParenthesizedLambdaExpression | 911 | 259 |
| AccessorList | 882 | 257 |
| GetAccessorDeclaration | 877 | 255 |
| SwitchExpressionArm | 877 | 91 |
| CoalesceExpression | 875 | 357 |
| LogicalOrExpression | 869 | 271 |
| NotPattern | 826 | 325 |
| SuppressNullableWarningExpression | 720 | 263 |
| TupleElement | 715 | 169 |
| InterpolatedStringExpression | 676 | 156 |
| WithExpression | 594 | 209 |
| WithInitializerExpression | 594 | 209 |
| NameColon | 590 | 171 |
| ArrayRankSpecifier | 577 | 221 |
| ArrayType | 576 | 221 |
| ImplicitElementAccess | 571 | 103 |
| GreaterThanExpression | 570 | 266 |
| OmittedArraySizeExpression | 562 | 221 |
| AwaitExpression | 560 | 135 |
| PostIncrementExpression | 486 | 217 |
| RecordDeclaration | 476 | 389 |
| CharacterLiteralExpression | 472 | 102 |
| RecursivePattern | 462 | 112 |
| PropertyPatternClause | 453 | 105 |
| ConstructorDeclaration | 449 | 443 |
| LessThanExpression | 435 | 199 |
| Subpattern | 410 | 104 |
| CastExpression | 406 | 141 |
| ObjectInitializerExpression | 404 | 172 |
| NotEqualsExpression | 401 | 166 |
| AttributeArgumentList | 396 | 52 |
| BaseList | 339 | 246 |
| CatchClause | 338 | 136 |
| CatchDeclaration | 335 | 134 |
| LockStatement | 333 | 52 |
| SwitchSection | 321 | 36 |
| ParenthesizedExpression | 320 | 166 |
| TupleType | 304 | 169 |
| BreakStatement | 302 | 60 |
| ContinueStatement | 296 | 142 |
| TryStatement | 293 | 145 |
| OrPattern | 286 | 90 |
| SimpleBaseType | 241 | 227 |
| ForStatement | 239 | 148 |
| InitAccessorDeclaration | 215 | 72 |
| EnumMemberDeclaration | 208 | 30 |
| ThrowStatement | 194 | 93 |
| TypeOfExpression | 181 | 41 |
| WhileStatement | 180 | 107 |
| CaseSwitchLabel | 175 | 24 |
| ThrowExpression | 168 | 84 |
| SubtractExpression | 167 | 79 |
| DiscardPattern | 161 | 92 |
| UsingStatement | 159 | 66 |
| SetAccessorDeclaration | 157 | 70 |
| SubtractAssignmentExpression | 157 | 66 |
| UnaryMinusExpression | 156 | 68 |
| SpreadElement | 155 | 80 |
| SwitchExpression | 155 | 91 |
| ThisExpression | 152 | 81 |
| EventFieldDeclaration | 149 | 83 |
| ElseClause | 139 | 112 |
| WhenClause | 113 | 23 |
| CasePatternSwitchLabel | 111 | 20 |
| LessThanOrEqualExpression | 110 | 78 |
| PrimaryConstructorBaseType | 102 | 19 |
| GreaterThanOrEqualExpression | 88 | 59 |
| RangeExpression | 77 | 37 |
| InterfaceDeclaration | 76 | 76 |
| RelationalPattern | 75 | 27 |
| ForEachVariableStatement | 73 | 49 |
| MultiplyExpression | 73 | 30 |
| CatchFilterClause | 70 | 41 |
| AsExpression | 69 | 45 |
| IndexExpression | 67 | 38 |
| SwitchStatement | 60 | 36 |
| TypeParameter | 60 | 30 |
| IsExpression | 58 | 35 |
| TypeParameterList | 57 | 30 |
| ArrayInitializerExpression | 49 | 24 |
| BitwiseOrExpression | 47 | 24 |
| YieldReturnStatement | 43 | 19 |
| DefaultSwitchLabel | 42 | 23 |
| ParenthesizedPattern | 40 | 18 |
| AndPattern | 38 | 15 |
| OrAssignmentExpression | 37 | 21 |
| ExpressionColon | 33 | 14 |
| FinallyClause | 32 | 25 |
| LeftShiftExpression | 32 | 2 |
| EnumDeclaration | 30 | 30 |
| DivideExpression | 29 | 17 |
| VarPattern | 29 | 11 |
| ImplicitArrayCreationExpression | 28 | 22 |
| ArrayCreationExpression | 22 | 16 |
| TypeParameterConstraintClause | 20 | 10 |
| ClassConstraint | 19 | 9 |
| CoalesceAssignmentExpression | 18 | 16 |
| LocalFunctionStatement | 17 | 11 |
| AttributeTargetSpecifier | 15 | 7 |
| CollectionInitializerExpression | 14 | 12 |
| ListPattern | 14 | 12 |
| BaseExpression | 13 | 5 |
| ConversionOperatorDeclaration | 13 | 2 |
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
| AndAssignmentExpression | 2 | 2 |
| ComplexElementInitializerExpression | 2 | 1 |
| ModuloExpression | 2 | 1 |
| RecordStructDeclaration | 2 | 2 |
| ThisConstructorInitializer | 2 | 2 |
| UncheckedExpression | 2 | 2 |
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
| SupportedOSPlatform | 1 | 1 |

## Parse errors (0)

None.

## Feature counts per project

| Feature | Llyn.UIDeportment | Llyn.Tests.Engine | Llyn.Tests.Conduct | Llyn.Infrastructure | Llyn.Tests.Convention | Llyn.Application | Llyn.Conduct | Llyn.Core | Llyn.ShellEngine | Llyn.Tests.Interface | Llyn.Tests.Windows | Llyn.Core.Windows | Llyn.UIVeneer | Llyn.UIDeportment.Capsule | Llyn.Host |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| Collection expression | 148 | 2,289 | 1,557 | 251 | 523 | 307 | 157 | 266 | 82 | 206 | 61 |  |  |  |  |
| using declaration | 5 | 1,798 | 2,381 | 568 | 7 | 12 |  |  |  | 18 | 21 |  |  |  |  |
| Lambda | 179 | 673 | 1,229 | 22 | 730 | 297 | 317 | 92 | 61 | 137 | 31 | 4 |  |  | 8 |
| if | 1,188 | 49 | 6 | 562 | 464 | 669 | 369 | 216 | 209 | 27 | 3 | 10 |  | 3 |  |
| Nullable reference type T? | 456 | 146 | 353 | 295 | 297 | 304 | 310 | 315 | 253 | 380 | 27 | 7 |  | 4 |  |
| Expression-bodied member | 942 | 33 | 35 | 6 | 26 | 109 | 247 | 152 | 207 | 1,004 | 28 |  |  |  |  |
| Target-typed new | 717 | 140 | 185 | 128 | 189 | 143 | 34 | 46 | 75 | 202 | 31 | 1 |  | 1 | 4 |
| Declaration pattern | 896 | 5 | 2 | 32 | 368 | 191 | 187 | 21 | 70 | 8 | 3 |  |  |  |  |
| File-scoped namespace | 282 | 189 | 123 | 145 | 112 | 107 | 225 | 302 | 51 | 86 | 17 | 4 | 31 | 4 |  |
| Readonly field | 435 | 33 | 4 | 106 | 215 | 366 | 229 | 28 | 89 | 35 |  | 4 |  | 1 |  |
| Full property | 877 | 7 | 4 | 6 | 2 | 109 | 232 | 140 | 37 | 11 |  |  |  |  |  |
| Sealed class | 234 | 194 | 126 | 72 | 43 | 170 | 207 | 200 | 45 | 31 | 14 | 4 |  | 4 |  |
| foreach | 184 | 49 | 18 | 238 | 306 | 297 | 25 | 145 | 35 | 40 | 1 |  |  |  |  |
| Tuple literal or tuple type | 38 | 381 | 252 | 159 | 168 | 67 | 52 | 36 | 76 | 23 | 10 |  |  |  |  |
| Class | 269 | 197 | 124 | 143 | 105 | 86 | 69 | 22 | 44 | 94 | 17 | 4 | 31 | 1 |  |
| Conditional ?: | 214 | 11 | 12 | 219 | 156 | 226 | 142 | 125 | 58 | 28 | 2 | 2 |  |  |  |
| Null-conditional ?. ?[ | 174 | 129 | 72 | 51 | 91 | 47 | 443 | 35 | 76 | 8 | 6 | 1 |  |  |  |
| Constant pattern | 138 | 3 | 8 | 113 | 353 | 44 | 121 | 120 | 43 | 17 |  | 5 |  |  |  |
| Static lambda | 80 | 26 | 511 | 2 | 15 | 97 | 60 | 37 | 13 | 44 | 3 |  |  |  |  |
| Null-coalescing ?? | 77 | 18 | 12 | 94 | 80 | 175 | 91 | 246 | 38 | 38 | 5 |  |  | 1 |  |
| not pattern | 308 | 4 | 1 | 49 | 185 | 63 | 129 | 31 | 49 | 6 | 1 |  |  |  |  |
| Auto property | 344 | 5 | 3 | 8 | 23 | 7 | 82 | 225 | 43 | 16 |  |  |  |  |  |
| Null-forgiving ! | 166 | 205 | 206 | 54 | 25 | 9 | 5 |  | 3 | 46 | 1 |  |  |  |  |
| is null | 122 | 7 | 3 | 91 | 175 | 147 | 71 | 37 | 51 | 13 | 2 |  |  |  |  |
| Interpolated string | 19 | 49 | 7 | 161 | 380 | 51 |  | 6 |  | 3 |  |  |  |  |  |
| with expression | 7 | 176 | 28 | 42 | 10 | 240 | 13 | 17 | 36 | 25 |  |  |  |  |  |
| Extension method |  |  |  |  |  |  |  |  |  | 572 | 8 |  |  |  |  |
| Raw string | 2 | 107 | 7 | 210 | 233 |  |  |  |  | 8 |  |  |  |  |  |
| await | 57 | 223 | 127 | 60 | 1 | 47 | 13 |  | 16 | 5 | 1 | 10 |  |  |  |
| Const field | 74 | 58 | 12 | 166 | 113 | 11 | 3 | 70 | 4 | 11 |  | 4 |  | 1 |  |
| Nullable value type T? | 56 | 20 | 21 | 40 | 11 | 33 | 135 | 30 | 101 | 64 | 7 |  |  | 2 |  |
| Record class | 16 |  | 2 | 2 | 6 | 103 | 144 | 197 | 1 | 2 |  |  |  | 3 |  |
| Property pattern | 143 | 2 |  | 7 | 270 | 14 | 8 | 3 | 4 | 1 | 1 |  |  |  |  |
| Constructor | 165 | 2 | 1 | 62 | 7 | 67 | 60 | 3 | 40 | 7 |  | 2 | 31 |  |  |
| Async method | 56 | 175 | 87 | 32 | 1 | 28 | 13 |  | 12 | 5 | 1 | 5 |  |  |  |
| Cast expression | 142 | 6 | 83 | 72 | 50 | 2 | 3 | 4 |  | 29 | 14 | 1 |  |  |  |
| Optional parameter | 8 | 11 | 3 | 6 | 1 | 11 | 4 | 249 | 16 | 93 |  |  |  | 3 |  |
| Object initializer | 135 | 41 | 94 | 29 | 60 |  |  | 3 |  | 32 | 8 | 1 |  | 1 |  |
| nameof | 223 | 1 | 3 | 7 | 17 | 116 | 16 | 1 | 4 | 2 | 1 |  |  |  |  |
| catch | 21 | 1 |  | 124 | 8 | 30 | 110 | 1 | 26 | 1 | 7 | 6 |  | 3 |  |
| lock | 13 |  |  | 2 | 5 | 78 | 2 |  | 228 | 5 |  |  |  |  |  |
| and / or pattern | 37 | 1 | 3 | 64 | 159 | 12 | 2 | 39 | 1 | 6 |  |  |  |  |  |
| Deconstruction | 27 | 65 | 113 | 23 | 33 | 23 | 19 | 4 | 6 |  |  |  |  |  |  |
| Static class | 43 | 3 |  | 73 | 68 | 18 | 6 | 19 |  | 62 | 3 |  |  |  |  |
| try | 19 | 7 |  | 76 | 10 | 27 | 112 | 1 | 27 | 1 | 7 | 5 |  | 1 |  |
| for | 44 | 11 | 6 | 48 | 27 | 58 | 6 | 12 | 9 | 16 | 2 |  |  |  |  |
| Named argument |  | 87 | 21 | 16 | 16 | 23 |  | 18 | 2 | 46 |  |  |  |  |  |
| Init accessor | 3 |  |  |  | 3 |  |  | 209 |  |  |  |  |  |  |  |
| typeof | 161 |  | 4 | 3 | 3 |  |  |  |  | 4 | 6 |  |  |  |  |
| while | 9 | 14 | 5 | 93 | 25 | 10 | 2 | 16 | 1 | 5 |  |  |  |  |  |
| throw expression | 4 | 4 | 33 | 5 | 1 | 73 | 14 | 1 | 8 | 24 | 1 |  |  |  |  |
| Discard pattern | 15 |  | 1 | 10 | 67 | 14 | 18 | 24 | 9 | 1 |  | 2 |  |  |  |
| using statement |  | 35 | 2 | 114 |  | 7 |  |  |  |  | 1 |  |  |  |  |
| Spread element | 9 | 11 | 8 | 1 | 25 | 27 | 6 | 42 | 12 | 13 | 1 |  |  |  |  |
| Switch expression | 13 |  | 1 | 10 | 65 | 14 | 17 | 23 | 9 | 1 |  | 2 |  |  |  |
| Field-like event | 53 |  |  |  |  |  | 92 |  | 4 |  |  |  |  |  |  |
| Discard _ | 20 | 22 | 58 | 1 | 9 | 8 | 2 | 9 | 5 | 1 |  |  |  |  |  |
| Partial type | 48 |  | 4 | 5 | 7 |  |  |  | 4 | 26 |  |  | 31 |  |  |
| Case guard when | 6 |  |  | 1 | 100 | 1 |  | 3 |  | 2 |  |  |  |  |  |
| Verbatim string |  | 4 |  |  | 78 | 3 |  |  |  | 2 |  |  |  |  |  |
| Range .. |  | 2 | 16 | 9 | 30 | 2 |  | 18 |  |  |  |  |  |  |  |
| Interface | 2 |  |  |  |  | 1 | 1 | 66 | 6 |  |  |  |  |  |  |
| Relational pattern | 1 |  |  | 21 | 3 | 10 |  | 39 | 1 |  |  |  |  |  |  |
| Exception filter | 3 |  |  | 32 | 3 | 14 |  |  | 18 |  |  |  |  |  |  |
| as cast | 57 |  |  | 3 | 9 |  |  |  |  |  |  |  |  |  |  |
| Index from end ^ | 2 | 7 | 38 | 1 | 11 | 4 |  | 4 |  |  |  |  |  |  |  |
| Switch statement | 7 |  |  | 9 | 20 |  | 2 | 19 | 1 | 2 |  |  |  |  |  |
| is type test | 21 | 1 |  | 2 | 32 | 2 |  |  |  |  |  |  |  |  |  |
| Generic method | 25 |  | 1 |  | 1 | 13 | 4 | 4 | 2 | 5 |  |  |  |  |  |
| yield return / break | 9 |  |  | 7 | 25 | 7 |  | 2 |  |  |  |  |  |  |  |
| Local const |  | 23 | 1 | 4 | 6 |  | 1 |  |  |  |  |  |  |  |  |
| finally | 1 | 6 |  | 2 | 2 | 11 | 4 |  | 4 |  |  | 2 |  |  |  |
| Enum | 2 |  |  |  |  |  | 11 | 17 |  |  |  |  |  |  |  |
| var pattern |  |  |  |  | 28 |  | 1 |  |  |  |  |  |  |  |  |
| Nested type | 7 | 8 | 3 |  |  |  |  |  |  | 10 |  |  |  |  |  |
| ref or out parameter | 2 | 2 | 1 | 1 | 3 | 6 |  | 4 | 3 | 4 |  |  |  |  |  |
| Constraint clause | 14 |  | 1 |  |  | 1 |  |  |  | 4 |  |  |  |  |  |
| Default interface method body |  |  |  |  |  |  |  |  | 19 |  |  |  |  |  |  |
| Null-coalescing assignment ??= | 10 |  |  | 1 | 3 | 3 |  |  |  |  |  | 1 |  |  |  |
| Local function | 4 | 1 |  |  | 4 |  | 5 |  |  | 1 | 1 | 1 |  |  |  |
| Collection initializer | 3 |  | 3 | 4 |  | 1 |  |  |  | 1 | 2 |  |  |  |  |
| List pattern | 4 | 1 |  |  | 8 |  |  |  | 1 |  |  |  |  |  |  |
| Conversion operator | 12 |  |  |  |  |  |  | 1 |  |  |  |  |  |  |  |
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
| Object creation | 50.96 % | 52.43 % | 32.46 % | 24.76 % | 42.00 % | 28.49 % | 9.52 % | 25.56 % | 29.53 % | 27.94 % | 42.47 % | 7.69 % | - | 14.29 % | 21.05 % |
| Collection creation | 94.87 % | 99.61 % | 99.11 % | 98.82 % | 99.05 % | 99.68 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 93.85 % | - | - | - | - |
| Member body | 32.40 % | 2.17 % | 2.82 % | 0.68 % | 3.69 % | 11.32 % | 20.38 % | 31.34 % | 25.49 % | 78.25 % | 38.36 % | 0.00 % | 0.00 % | 0.00 % | - |
| Local type | 4.58 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.31 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | - | 0.00 % | 0.00 % |
| Using | 100.00 % | 98.09 % | 99.92 % | 83.28 % | 100.00 % | 63.16 % | - | - | - | 100.00 % | 95.45 % | - | - | - | - |
| Namespace | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - |
| Text building | 55.88 % | 35.77 % | 17.95 % | 65.71 % | 82.07 % | 82.26 % | 0.00 % | 26.09 % | - | 27.27 % | - | 0.00 % | - | 0.00 % | - |
| Branch | 65.00 % | - | 100.00 % | 52.63 % | 76.47 % | 100.00 % | 89.47 % | 54.76 % | 90.00 % | 33.33 % | - | 100.00 % | - | - | - |
| Lambda body | 83.80 % | 95.99 % | 79.25 % | 86.36 % | 99.04 % | 91.25 % | 98.11 % | 95.65 % | 90.16 % | 79.56 % | 45.16 % | 50.00 % | - | - | 75.00 % |
| Type test | 91.22 % | 100.00 % | 100.00 % | 88.89 % | 94.51 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - | - | - | - |
| Constructor | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 36.36 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | - | 0.00 % | 0.00 % | - | - |
