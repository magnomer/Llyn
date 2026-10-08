# Syntax statistics - 0.17.13807

- Generated: 2026-10-08 23:57:24 +09:00
- Source roots: src, tests
- Root-level files: none
- Project segments: 1
- Excluded directories: .git, .vs, artifacts, bin, node_modules, obj, packages, publish
- Excluded suffixes: .g.cs, .g.i.cs, .AssemblyInfo.cs, .GlobalUsings.g.cs, .Designer.cs
- Roslyn: 4.14.0-3.25262.10
- Language version: Preview
- Scanned: 1,786 files, 15 projects, 191,716 lines

## Summary

| Measure | Value |
|---|---:|
| Files | 1,786 |
| Lines | 191,716 |
| Nodes | 1,012,764 |
| Syntax kinds used | 191 |
| Features used | 98 of 138 |
| Minimum C# version | 12 |
| Parse errors | 0 |

## Families

| Family | Features | Used | Count | Per 1k lines |
|---|---:|---:|---:|---:|
| Types | 15 | 12 | 3,771 | 19.67 |
| Namespaces | 6 | 2 | 1,786 | 9.32 |
| Members | 29 | 20 | 9,413 | 49.10 |
| Patterns | 16 | 16 | 5,951 | 31.04 |
| Expressions | 36 | 29 | 23,828 | 124.29 |
| Statements | 20 | 13 | 12,058 | 62.90 |
| Generics | 6 | 2 | 78 | 0.41 |
| Async | 2 | 2 | 441 | 2.30 |
| Nullability | 3 | 2 | 3,742 | 19.52 |
| Unsafe | 2 | 0 | 0 | 0.00 |
| Directives | 3 | 0 | 0 | 0.00 |
| Total | 138 | 98 | 61,068 | 318.53 |

## Projects

| Project | Files | Lines | Nodes | Features used | Newest C# |
|---|---:|---:|---:|---:|---:|
| Llyn.UIDeportment | 284 | 32,716 | 152,449 | 86 | 12 |
| Llyn.Tests.Engine | 196 | 31,998 | 225,064 | 59 | 12 |
| Llyn.Tests.Conduct | 134 | 24,439 | 178,091 | 60 | 12 |
| Llyn.Infrastructure | 157 | 21,963 | 99,672 | 71 | 12 |
| Llyn.Application | 119 | 18,021 | 82,792 | 70 | 12 |
| Llyn.Tests.Convention | 112 | 17,383 | 84,736 | 79 | 12 |
| Llyn.Conduct | 241 | 15,394 | 60,099 | 59 | 12 |
| Llyn.Core | 312 | 10,674 | 43,052 | 58 | 12 |
| Llyn.ShellEngine | 85 | 8,925 | 35,894 | 57 | 12 |
| Llyn.Tests.Interface | 87 | 8,132 | 40,931 | 65 | 12 |
| Llyn.Tests.Windows | 19 | 1,174 | 6,986 | 39 | 12 |
| Llyn.Core.Windows | 4 | 385 | 1,427 | 26 | 10 |
| Llyn.UIVeneer | 31 | 339 | 672 | 4 | 10 |
| Llyn.UIDeportment.Capsule | 4 | 88 | 380 | 15 | 10 |
| Llyn.Host | 1 | 85 | 519 | 4 | 9 |
| Total | 1,786 | 191,716 | 1,012,764 | 98 | 12 |

## Versions

| C# version | Features | Used | Count |
|---|---:|---:|---:|
| 1 | 51 | 35 | 16,226 |
| 2 | 10 | 8 | 1,963 |
| 3 | 7 | 5 | 5,969 |
| 4 | 3 | 2 | 711 |
| 5 | 3 | 3 | 1,035 |
| 6 | 6 | 5 | 4,904 |
| 7 | 11 | 10 | 5,726 |
| 7.1 | 1 | 1 | 7 |
| 7.2 | 1 | 0 | 0 |
| 7.3 | 1 | 0 | 0 |
| 8 | 17 | 12 | 10,043 |
| 9 | 10 | 9 | 5,648 |
| 10 | 3 | 2 | 1,787 |
| 11 | 6 | 3 | 603 |
| 12 | 4 | 3 | 6,446 |
| 13 | 1 | 0 | 0 |
| 14 | 3 | 0 | 0 |

## Features

| Feature | Family | C# | Count | Per 1k lines | Files | First |
|---|---|---|---:|---:|---:|---|
| Collection expression | Expressions | 12 | 6,279 | 32.75 | 803 | src/Llyn.Application/Card/LCardClerk.cs:57 |
| using declaration | Statements | 8 | 5,044 | 26.31 | 379 | src/Llyn.Application/Card/LTranslationClerk.cs:158 |
| Lambda | Expressions | 3 | 4,118 | 21.48 | 647 | src/Llyn.Application/Card/LCardClerk.cs:174 |
| if | Statements | 1 | 3,845 | 20.06 | 641 | src/Llyn.Application/Card/LCardClerk.cs:70 |
| Nullable reference type T? | Nullability | 8 | 3,227 | 16.83 | 884 | src/Llyn.Application/Card/LCardClerkField.cs:91 |
| Expression-bodied member | Members | 6 | 2,731 | 14.25 | 443 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Target-typed new | Expressions | 9 | 1,983 | 10.34 | 490 | src/Llyn.Application/Card/LMentionClerk.cs:167 |
| Declaration pattern | Patterns | 7 | 1,825 | 9.52 | 360 | src/Llyn.Application/Card/LMeaningClerk.cs:93 |
| File-scoped namespace | Namespaces | 10 | 1,785 | 9.31 | 1,785 | src/Llyn.Application/Card/LCardClerk.cs:5 |
| Readonly field | Members | 1 | 1,688 | 8.80 | 536 | src/Llyn.Application/Card/LCardClerk.cs:9 |
| Sealed class | Types | 1 | 1,411 | 7.36 | 1,308 | src/Llyn.Application/Card/LCardClerk.cs:7 |
| foreach | Statements | 1 | 1,400 | 7.30 | 439 | src/Llyn.Application/Card/LCardClerk.cs:59 |
| Full property | Members | 1 | 1,381 | 7.20 | 319 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Tuple literal or tuple type | Expressions | 7 | 1,346 | 7.02 | 255 | src/Llyn.Application/Card/LMeaningClerk.cs:26 |
| Conditional ?: | Expressions | 1 | 1,280 | 6.68 | 487 | src/Llyn.Application/Card/LCardClerk.cs:173 |
| Class | Types | 1 | 1,278 | 6.67 | 1,260 | src/Llyn.Application/Card/LCardClerk.cs:7 |
| Static lambda | Expressions | 9 | 1,049 | 5.47 | 260 | src/Llyn.Application/Card/LCardEquality.cs:113 |
| Null-conditional ?. ?[ | Expressions | 6 | 1,031 | 5.38 | 359 | src/Llyn.Application/Card/LMentionClerk.cs:47 |
| Constant pattern | Patterns | 7 | 1,022 | 5.33 | 179 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Null-coalescing ?? | Expressions | 2 | 892 | 4.65 | 366 | src/Llyn.Application/Card/LCardClerkField.cs:74 |
| not pattern | Patterns | 9 | 840 | 4.38 | 345 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| Auto property | Members | 3 | 816 | 4.26 | 244 | src/Llyn.Application/Citation/LCitationClerk.cs:41 |
| Null-forgiving ! | Expressions | 8 | 758 | 3.95 | 270 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:145 |
| is null | Patterns | 7 | 738 | 3.85 | 326 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| Interpolated string | Expressions | 6 | 675 | 3.52 | 157 | src/Llyn.Application/Localization/LLocalizationReader.cs:55 |
| with expression | Expressions | 9 | 646 | 3.37 | 221 | src/Llyn.Application/Card/LCardClerk.cs:109 |
| await | Expressions | 5 | 594 | 3.10 | 140 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:139 |
| Extension method | Members | 3 | 593 | 3.09 | 41 | tests/Llyn.Tests.Interface/TInterfaceArea.cs:7 |
| Raw string | Expressions | 11 | 584 | 3.05 | 131 | src/Llyn.Infrastructure/Database/Base/LDatabase.cs:51 |
| Const field | Members | 1 | 558 | 2.91 | 233 | src/Llyn.Application/Card/LMentionClerk.cs:11 |
| Nullable value type T? | Nullability | 2 | 515 | 2.69 | 233 | src/Llyn.Application/Card/LMeaningClerk.cs:92 |
| Record class | Types | 9 | 489 | 2.55 | 401 | src/Llyn.Application/Ensign/LEnsignRow.cs:3 |
| Constructor | Members | 1 | 488 | 2.55 | 482 | src/Llyn.Application/Card/LCardClerk.cs:19 |
| Property pattern | Patterns | 8 | 455 | 2.37 | 110 | src/Llyn.Application/Draft/Clerk/LDraftClerk.cs:79 |
| Async method | Async | 5 | 434 | 2.26 | 140 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:91 |
| Object initializer | Expressions | 3 | 426 | 2.22 | 183 | src/Llyn.Application/Portrait/LPortraitClerkLabel.cs:25 |
| Optional parameter | Members | 4 | 425 | 2.22 | 141 | src/Llyn.Application/Draft/Clerk/LDraftClerkPanel.cs:170 |
| Cast expression | Expressions | 1 | 418 | 2.18 | 149 | src/Llyn.Application/Markup/LMarkupClerkLink.cs:234 |
| nameof | Expressions | 6 | 392 | 2.04 | 132 | src/Llyn.Application/Citation/LAuthorClerk.cs:361 |
| catch | Statements | 1 | 343 | 1.79 | 149 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:142 |
| lock | Statements | 1 | 335 | 1.75 | 53 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:98 |
| and / or pattern | Patterns | 9 | 332 | 1.73 | 97 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Deconstruction | Expressions | 7 | 322 | 1.68 | 135 | src/Llyn.Application/Card/LMeaningClerk.cs:46 |
| Static class | Types | 2 | 313 | 1.63 | 313 | src/Llyn.Application/Card/LCardClerkField.cs:7 |
| try | Statements | 1 | 299 | 1.56 | 159 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:137 |
| Named argument | Members | 4 | 286 | 1.49 | 86 | src/Llyn.Application/Card/LTranslationClerk.cs:162 |
| for | Statements | 1 | 248 | 1.29 | 153 | src/Llyn.Application/Card/LCardClerk.cs:247 |
| Init accessor | Members | 9 | 218 | 1.14 | 74 | src/Llyn.Conduct/Lexicon/CExample.cs:20 |
| while | Statements | 1 | 188 | 0.98 | 109 | src/Llyn.Application/Card/LMeaningClerk.cs:44 |
| typeof | Expressions | 1 | 186 | 0.97 | 42 | src/Llyn.Infrastructure/Configuration/LLocalizationLoader.cs:20 |
| Discard pattern | Patterns | 8 | 184 | 0.96 | 110 | src/Llyn.Application/Citation/LAuthorClerk.cs:243 |
| Switch expression | Patterns | 8 | 179 | 0.93 | 110 | src/Llyn.Application/Citation/LAuthorClerk.cs:239 |
| throw expression | Expressions | 7 | 175 | 0.91 | 91 | src/Llyn.Application/Card/LCardClerkField.cs:75 |
| Spread element | Expressions | 12 | 163 | 0.85 | 88 | src/Llyn.Application/Card/LCardEquality.cs:53 |
| using statement | Statements | 1 | 161 | 0.84 | 67 | src/Llyn.Application/Citation/LAuthorCitation.cs:51 |
| Field-like event | Members | 1 | 150 | 0.78 | 91 | src/Llyn.Conduct/Atelier/CNavigation.cs:46 |
| Discard _ | Expressions | 7 | 137 | 0.71 | 48 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:108 |
| Case guard when | Patterns | 7 | 115 | 0.60 | 24 | src/Llyn.Application/Citation/LAuthorClerk.cs:241 |
| Partial type | Types | 2 | 113 | 0.59 | 113 | src/Llyn.Infrastructure/Database/Entry/LEntryArchive.cs:9 |
| Interface | Types | 1 | 97 | 0.51 | 97 | src/Llyn.Application/Pronunciation/Relay/LListener.cs:5 |
| Verbatim string | Expressions | 1 | 88 | 0.46 | 19 | src/Llyn.Application/Localization/LLocalizationReader.cs:13 |
| as cast | Expressions | 1 | 85 | 0.44 | 46 | src/Llyn.Infrastructure/Database/Base/LWorkspaceArchive.cs:132 |
| Range .. | Expressions | 8 | 84 | 0.44 | 40 | src/Llyn.Application/Card/LMentionClerk.cs:173 |
| Relational pattern | Patterns | 9 | 80 | 0.42 | 28 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Exception filter | Statements | 6 | 75 | 0.39 | 46 | src/Llyn.Application/Outpost/LCourierClerk.cs:107 |
| Index from end ^ | Expressions | 8 | 74 | 0.39 | 41 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:230 |
| is type test | Patterns | 1 | 59 | 0.31 | 36 | src/Llyn.Application/Outpost/LCourierClerk.cs:363 |
| Switch statement | Patterns | 1 | 59 | 0.31 | 35 | src/Llyn.Conduct/Configuration/CLedger.cs:240 |
| Generic method | Generics | 2 | 58 | 0.30 | 30 | src/Llyn.Application/Card/LCardClerkField.cs:183 |
| yield return / break | Statements | 2 | 50 | 0.26 | 19 | src/Llyn.Application/Card/LCardClerkField.cs:35 |
| Local const | Statements | 1 | 37 | 0.19 | 21 | src/Llyn.Conduct/Lexicon/CStateWording.cs:10 |
| finally | Statements | 1 | 33 | 0.17 | 25 | src/Llyn.Application/Outpost/LCourierClerk.cs:83 |
| Enum | Types | 1 | 32 | 0.17 | 32 | src/Llyn.Conduct/Catalog/CCatalogOrder.cs:3 |
| var pattern | Patterns | 7 | 29 | 0.15 | 11 | src/Llyn.Conduct/Card/CFolio.cs:193 |
| Nested type | Types | 1 | 28 | 0.15 | 17 | src/Llyn.UIDeportment/Kit/Bind/QLook.cs:19 |
| ref or out parameter | Members | 1 | 26 | 0.14 | 20 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:149 |
| Constraint clause | Generics | 2 | 20 | 0.10 | 10 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:171 |
| Null-coalescing assignment ??= | Expressions | 8 | 18 | 0.09 | 16 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:160 |
| Local function | Members | 7 | 17 | 0.09 | 11 | src/Llyn.Conduct/Desk/CErrand.cs:55 |
| Collection initializer | Expressions | 3 | 16 | 0.08 | 12 | src/Llyn.Application/Outpost/LCourierClerk.cs:253 |
| List pattern | Patterns | 11 | 14 | 0.07 | 12 | src/Llyn.ShellEngine/Engine/LDraftFacade.cs:66 |
| Conversion operator | Members | 1 | 13 | 0.07 | 2 | src/Llyn.Core/Lexicon/State/LStateValue.cs:49 |
| Type pattern | Patterns | 9 | 11 | 0.06 | 4 | src/Llyn.UIDeportment/Sound/Accent/QAccentControl.cs:62 |
| Default interface method body | Members | 8 | 9 | 0.05 | 4 | src/Llyn.ShellEngine/Port/LDraftPort.cs:58 |
| Positional pattern | Patterns | 8 | 9 | 0.05 | 9 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:37 |
| Async lambda | Async | 5 | 7 | 0.04 | 4 | src/Llyn.Application/Outpost/LCourierClerk.cs:327 |
| default literal | Expressions | 7.1 | 7 | 0.04 | 5 | src/Llyn.Infrastructure/Pronunciation/Loader/LSourceLoader.cs:73 |
| Required member | Members | 11 | 5 | 0.03 | 3 | src/Llyn.UIDeportment/Panel/Taxonomy/QMembershipItem.cs:27 |
| Event accessors | Members | 1 | 4 | 0.02 | 3 | src/Llyn.ShellEngine/Port/LSettingsOutlet.cs:72 |
| Primary constructor on class or struct | Types | 12 | 4 | 0.02 | 4 | tests/Llyn.Tests.Convention/TAuditFakeMember.cs:3 |
| Abstract class | Types | 1 | 2 | 0.01 | 2 | src/Llyn.Application/Request/Entry/LRequest.cs:3 |
| checked / unchecked expression | Expressions | 1 | 2 | 0.01 | 2 | src/Llyn.Core.Windows/LPressBrowser.cs:14 |
| Generic type | Types | 2 | 2 | 0.01 | 2 | src/Llyn.Conduct/Configuration/CEnsignSheet.cs:5 |
| Record struct | Types | 10 | 2 | 0.01 | 2 | src/Llyn.UIDeportment/Mention/Menu/PSwath.cs:20 |
| Static constructor | Members | 1 | 2 | 0.01 | 2 | src/Llyn.UIDeportment/Kit/Bind/QLookItem.cs:22 |
| Static local function | Members | 8 | 2 | 0.01 | 1 | src/Llyn.UIDeportment/Kit/Asset/QIcon.cs:144 |
| Indexer | Members | 1 | 1 | 0.01 | 1 | src/Llyn.UIDeportment/Kit/Asset/QLocalizationCatalog.cs:19 |
| Using alias | Namespaces | 1 | 1 | 0.01 | 1 | src/Llyn.Host/LHost.cs:11 |

## Styles

| Style | A | A count | B | B count | A share |
|---|---|---:|---|---:|---:|
| Null check | is null / is not null | 738 | == null / != null | 0 | 100.00 % |
| Object creation | Target-typed new() | 1,983 | new T() | 3,593 | 35.56 % |
| Collection creation | Collection expression | 6,279 | Collection initializer or array creation | 51 | 99.19 % |
| Member body | Expression body | 2,731 | Block body | 9,621 | 22.11 % |
| Local type | var | 42 | Explicit type | 19,456 | 0.22 % |
| Using | Declaration | 5,044 | Statement | 161 | 96.91 % |
| Namespace | File-scoped | 1,785 | Block | 0 | 100.00 % |
| Text building | Interpolated string | 675 | string.Format or + with a string literal | 423 | 61.48 % |
| Branch | Switch expression | 179 | Switch statement | 59 | 75.21 % |
| Lambda body | Expression | 3,692 | Block | 426 | 89.66 % |
| Type test | is pattern with designation | 1,032 | as | 85 | 92.39 % |
| Constructor | Primary | 4 | Explicit | 482 | 0.82 % |

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
| IdentifierName | 293,251 | 1,786 |
| Argument | 105,419 | 1,359 |
| SimpleMemberAccessExpression | 97,107 | 1,351 |
| ArgumentList | 65,301 | 1,391 |
| InvocationExpression | 59,854 | 1,357 |
| PredefinedType | 29,085 | 1,679 |
| ExpressionStatement | 28,582 | 1,244 |
| StringLiteralExpression | 23,563 | 989 |
| Parameter | 21,040 | 1,613 |
| VariableDeclaration | 20,046 | 1,175 |
| VariableDeclarator | 20,046 | 1,175 |
| EqualsValueClause | 19,222 | 1,229 |
| Block | 16,998 | 1,315 |
| LocalDeclarationStatement | 16,847 | 1,019 |
| ParameterList | 12,825 | 1,715 |
| MethodDeclaration | 10,862 | 1,376 |
| NumericLiteralExpression | 10,419 | 993 |
| GenericName | 9,989 | 1,217 |
| TypeArgumentList | 9,989 | 1,217 |
| QualifiedName | 8,339 | 1,786 |
| SimpleAssignmentExpression | 6,958 | 923 |
| ExpressionElement | 6,823 | 433 |
| CollectionExpression | 6,279 | 803 |
| UsingDirective | 5,623 | 1,534 |
| ReturnStatement | 5,581 | 989 |
| NullLiteralExpression | 3,890 | 807 |
| IfStatement | 3,845 | 641 |
| NullableType | 3,742 | 929 |
| ObjectCreationExpression | 3,593 | 797 |
| Attribute | 3,177 | 382 |
| AttributeList | 3,177 | 382 |
| SimpleLambdaExpression | 3,169 | 575 |
| SingleVariableDesignation | 3,066 | 535 |
| BracketedArgumentList | 2,837 | 501 |
| ArrowExpressionClause | 2,733 | 444 |
| FieldDeclaration | 2,645 | 694 |
| IsPatternExpression | 2,537 | 537 |
| ElementAccessExpression | 2,232 | 449 |
| PropertyDeclaration | 2,202 | 462 |
| ImplicitObjectCreationExpression | 1,983 | 490 |
| FalseLiteralExpression | 1,898 | 519 |
| DeclarationPattern | 1,825 | 360 |
| CompilationUnit | 1,786 | 1,786 |
| FileScopedNamespaceDeclaration | 1,785 | 1,785 |
| ConstantPattern | 1,760 | 413 |
| TrueLiteralExpression | 1,528 | 461 |
| InterpolatedStringText | 1,483 | 157 |
| EqualsExpression | 1,466 | 529 |
| TupleExpression | 1,346 | 255 |
| AddExpression | 1,333 | 318 |
| ForEachStatement | 1,320 | 426 |
| Interpolation | 1,310 | 156 |
| ConditionalExpression | 1,280 | 487 |
| LogicalAndExpression | 1,280 | 355 |
| ClassDeclaration | 1,278 | 1,260 |
| LogicalNotExpression | 1,108 | 380 |
| AddAssignmentExpression | 1,106 | 245 |
| ConditionalAccessExpression | 1,031 | 359 |
| MemberBindingExpression | 1,031 | 359 |
| DeclarationExpression | 964 | 254 |
| SwitchExpressionArm | 953 | 110 |
| ParenthesizedLambdaExpression | 949 | 274 |
| AccessorList | 939 | 270 |
| GetAccessorDeclaration | 934 | 268 |
| AttributeArgument | 928 | 53 |
| CoalesceExpression | 892 | 366 |
| LogicalOrExpression | 890 | 288 |
| NotPattern | 840 | 345 |
| TupleElement | 761 | 184 |
| SuppressNullableWarningExpression | 758 | 270 |
| InterpolatedStringExpression | 675 | 157 |
| NameColon | 649 | 185 |
| WithExpression | 646 | 221 |
| WithInitializerExpression | 646 | 221 |
| GreaterThanExpression | 638 | 278 |
| ArrayRankSpecifier | 610 | 239 |
| ArrayType | 609 | 239 |
| ImplicitElementAccess | 605 | 113 |
| AwaitExpression | 594 | 140 |
| OmittedArraySizeExpression | 593 | 239 |
| PostIncrementExpression | 499 | 224 |
| CharacterLiteralExpression | 497 | 112 |
| ConstructorDeclaration | 490 | 484 |
| RecordDeclaration | 489 | 401 |
| RecursivePattern | 464 | 117 |
| LessThanExpression | 461 | 205 |
| PropertyPatternClause | 455 | 110 |
| ObjectInitializerExpression | 426 | 183 |
| CastExpression | 418 | 149 |
| Subpattern | 412 | 108 |
| NotEqualsExpression | 401 | 170 |
| AttributeArgumentList | 396 | 53 |
| BaseList | 354 | 260 |
| ParenthesizedExpression | 348 | 175 |
| CatchClause | 343 | 149 |
| CatchDeclaration | 340 | 147 |
| LockStatement | 335 | 53 |
| TupleType | 323 | 184 |
| SwitchSection | 319 | 35 |
| BreakStatement | 305 | 60 |
| ContinueStatement | 299 | 145 |
| TryStatement | 299 | 159 |
| OrPattern | 290 | 91 |
| SimpleBaseType | 263 | 241 |
| ForStatement | 248 | 153 |
| EnumMemberDeclaration | 218 | 32 |
| InitAccessorDeclaration | 218 | 74 |
| ThrowStatement | 197 | 97 |
| SubtractExpression | 189 | 87 |
| WhileStatement | 188 | 109 |
| TypeOfExpression | 186 | 42 |
| DiscardPattern | 184 | 110 |
| SwitchExpression | 179 | 110 |
| ThrowExpression | 175 | 91 |
| CaseSwitchLabel | 173 | 23 |
| SpreadElement | 163 | 88 |
| UsingStatement | 161 | 67 |
| UnaryMinusExpression | 159 | 71 |
| SubtractAssignmentExpression | 158 | 67 |
| SetAccessorDeclaration | 152 | 70 |
| EventFieldDeclaration | 150 | 91 |
| ThisExpression | 150 | 79 |
| ElseClause | 141 | 113 |
| WhenClause | 115 | 24 |
| CasePatternSwitchLabel | 111 | 20 |
| LessThanOrEqualExpression | 111 | 79 |
| PrimaryConstructorBaseType | 103 | 19 |
| GreaterThanOrEqualExpression | 99 | 65 |
| InterfaceDeclaration | 97 | 97 |
| AsExpression | 85 | 46 |
| RangeExpression | 84 | 40 |
| MultiplyExpression | 81 | 32 |
| ForEachVariableStatement | 80 | 56 |
| RelationalPattern | 80 | 28 |
| CatchFilterClause | 75 | 46 |
| IndexExpression | 74 | 41 |
| TypeParameter | 63 | 32 |
| TypeParameterList | 60 | 32 |
| IsExpression | 59 | 36 |
| SwitchStatement | 59 | 35 |
| ArrayInitializerExpression | 54 | 29 |
| BitwiseOrExpression | 47 | 24 |
| YieldReturnStatement | 43 | 19 |
| AndPattern | 42 | 17 |
| DefaultSwitchLabel | 41 | 22 |
| ParenthesizedPattern | 41 | 18 |
| OrAssignmentExpression | 37 | 21 |
| ExpressionColon | 33 | 14 |
| FinallyClause | 33 | 25 |
| ImplicitArrayCreationExpression | 33 | 27 |
| EnumDeclaration | 32 | 32 |
| LeftShiftExpression | 32 | 2 |
| DivideExpression | 31 | 19 |
| VarPattern | 29 | 11 |
| ArrayCreationExpression | 24 | 17 |
| TypeParameterConstraintClause | 20 | 10 |
| ClassConstraint | 19 | 9 |
| CoalesceAssignmentExpression | 18 | 16 |
| LocalFunctionStatement | 17 | 11 |
| CollectionInitializerExpression | 16 | 12 |
| AttributeTargetSpecifier | 15 | 7 |
| ListPattern | 14 | 12 |
| BaseExpression | 13 | 5 |
| ConversionOperatorDeclaration | 13 | 2 |
| TypePattern | 11 | 4 |
| PositionalPatternClause | 9 | 9 |
| PostDecrementExpression | 9 | 8 |
| DefaultLiteralExpression | 7 | 5 |
| InterpolationFormatClause | 7 | 1 |
| YieldBreakStatement | 7 | 6 |
| BaseConstructorInitializer | 6 | 6 |
| ExplicitInterfaceSpecifier | 6 | 5 |
| GlobalStatement | 6 | 1 |
| NameEquals | 6 | 4 |
| AddAccessorDeclaration | 4 | 3 |
| EventDeclaration | 4 | 3 |
| RemoveAccessorDeclaration | 4 | 3 |
| AndAssignmentExpression | 3 | 3 |
| InterpolationAlignmentClause | 3 | 1 |
| PreIncrementExpression | 3 | 3 |
| SlicePattern | 3 | 3 |
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
| Fact | 2,704 | 368 |
| InlineData | 374 | 42 |
| Theory | 76 | 44 |
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

| Feature | Llyn.UIDeportment | Llyn.Tests.Engine | Llyn.Tests.Conduct | Llyn.Infrastructure | Llyn.Application | Llyn.Tests.Convention | Llyn.Conduct | Llyn.Core | Llyn.ShellEngine | Llyn.Tests.Interface | Llyn.Tests.Windows | Llyn.Core.Windows | Llyn.UIVeneer | Llyn.UIDeportment.Capsule | Llyn.Host |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| Collection expression | 149 | 2,609 | 1,612 | 275 | 328 | 523 | 159 | 265 | 77 | 217 | 65 |  |  |  |  |
| using declaration | 5 | 1,954 | 2,449 | 578 | 12 | 7 |  |  |  | 18 | 21 |  |  |  |  |
| Lambda | 181 | 791 | 1,273 | 28 | 341 | 730 | 333 | 162 | 66 | 167 | 33 | 4 |  |  | 9 |
| if | 1,194 | 49 | 6 | 636 | 683 | 464 | 340 | 222 | 205 | 30 | 3 | 10 |  | 3 |  |
| Nullable reference type T? | 455 | 148 | 361 | 319 | 333 | 297 | 309 | 321 | 245 | 399 | 29 | 7 |  | 4 |  |
| Expression-bodied member | 939 | 34 | 35 | 6 | 110 | 26 | 195 | 154 | 123 | 1,080 | 29 |  |  |  |  |
| Target-typed new | 718 | 144 | 197 | 131 | 150 | 189 | 36 | 50 | 84 | 243 | 35 | 1 |  | 1 | 4 |
| Declaration pattern | 905 | 5 | 4 | 40 | 193 | 368 | 204 | 22 | 71 | 10 | 3 |  |  |  |  |
| File-scoped namespace | 284 | 196 | 134 | 157 | 119 | 112 | 241 | 312 | 85 | 87 | 19 | 4 | 31 | 4 |  |
| Readonly field | 439 | 37 | 4 | 116 | 385 | 215 | 315 | 28 | 109 | 35 |  | 4 |  | 1 |  |
| Sealed class | 235 | 201 | 137 | 73 | 180 | 43 | 222 | 206 | 58 | 32 | 16 | 4 |  | 4 |  |
| foreach | 185 | 52 | 18 | 275 | 317 | 306 | 25 | 149 | 32 | 40 | 1 |  |  |  |  |
| Full property | 880 | 7 | 4 | 6 | 110 | 2 | 181 | 142 | 38 | 11 |  |  |  |  |  |
| Tuple literal or tuple type | 39 | 411 | 267 | 170 | 84 | 168 | 55 | 44 | 73 | 25 | 10 |  |  |  |  |
| Conditional ?: | 217 | 13 | 16 | 269 | 243 | 156 | 143 | 129 | 62 | 28 | 2 | 2 |  |  |  |
| Class | 271 | 204 | 135 | 155 | 98 | 105 | 84 | 24 | 52 | 95 | 19 | 4 | 31 | 1 |  |
| Static lambda | 77 | 78 | 530 | 4 | 114 | 15 | 63 | 87 | 14 | 63 | 3 |  |  |  | 1 |
| Null-conditional ?. ?[ | 175 | 129 | 74 | 58 | 47 | 91 | 326 | 37 | 79 | 8 | 6 | 1 |  |  |  |
| Constant pattern | 133 | 3 | 8 | 142 | 54 | 353 | 126 | 138 | 43 | 17 |  | 5 |  |  |  |
| Null-coalescing ?? | 79 | 18 | 12 | 102 | 189 | 80 | 52 | 254 | 41 | 58 | 6 |  |  | 1 |  |
| not pattern | 310 | 4 | 1 | 52 | 70 | 185 | 132 | 30 | 49 | 6 | 1 |  |  |  |  |
| Auto property | 348 | 5 | 3 | 8 | 7 | 23 | 126 | 229 | 46 | 21 |  |  |  |  |  |
| Null-forgiving ! | 165 | 222 | 207 | 56 | 15 | 25 | 9 |  | 3 | 55 | 1 |  |  |  |  |
| is null | 118 | 8 | 5 | 91 | 152 | 175 | 82 | 37 | 55 | 13 | 2 |  |  |  |  |
| Interpolated string | 19 | 51 | 7 | 158 | 51 | 380 |  | 6 |  | 3 |  |  |  |  |  |
| with expression | 7 | 214 | 31 | 42 | 248 | 10 | 16 | 17 | 36 | 25 |  |  |  |  |  |
| await | 57 | 228 | 144 | 62 | 56 | 1 | 13 |  | 17 | 5 | 1 | 10 |  |  |  |
| Extension method |  |  |  |  |  |  |  |  |  | 585 | 8 |  |  |  |  |
| Raw string | 2 | 121 | 7 | 213 |  | 233 |  |  |  | 8 |  |  |  |  |  |
| Const field | 76 | 69 | 14 | 179 | 13 | 113 | 3 | 70 | 4 | 12 |  | 4 |  | 1 |  |
| Nullable value type T? | 56 | 20 | 21 | 42 | 34 | 11 | 115 | 30 | 111 | 66 | 7 |  |  | 2 |  |
| Record class | 16 |  | 2 | 2 | 104 | 6 | 145 | 203 | 6 | 2 |  |  |  | 3 |  |
| Constructor | 169 | 2 | 1 | 65 | 76 | 7 | 74 | 3 | 51 | 7 |  | 2 | 31 |  |  |
| Property pattern | 143 | 2 |  | 8 | 15 | 270 | 8 | 3 | 4 | 1 | 1 |  |  |  |  |
| Async method | 56 | 180 | 99 | 32 | 29 | 1 | 13 |  | 13 | 5 | 1 | 5 |  |  |  |
| Object initializer | 135 | 55 | 95 | 29 | 1 | 60 |  | 3 |  | 33 | 13 | 1 |  | 1 |  |
| Optional parameter | 8 | 12 | 6 | 6 | 11 | 1 | 8 | 256 | 12 | 102 |  |  |  | 3 |  |
| Cast expression | 137 | 7 | 83 | 79 | 4 | 50 | 3 | 4 |  | 35 | 15 | 1 |  |  |  |
| nameof | 218 | 1 | 3 | 8 | 117 | 17 | 18 | 3 | 4 | 2 | 1 |  |  |  |  |
| catch | 21 | 1 |  | 126 | 33 | 8 | 108 | 1 | 27 | 1 | 8 | 6 |  | 3 |  |
| lock | 13 |  |  | 2 | 77 | 5 | 2 |  | 231 | 5 |  |  |  |  |  |
| and / or pattern | 37 | 1 | 3 | 69 | 15 | 159 | 2 | 39 | 1 | 6 |  |  |  |  |  |
| Deconstruction | 28 | 66 | 113 | 26 | 27 | 33 | 19 | 5 | 5 |  |  |  |  |  |  |
| Static class | 44 | 3 |  | 84 | 21 | 68 | 7 | 21 |  | 62 | 3 |  |  |  |  |
| try | 19 | 7 |  | 78 | 30 | 10 | 110 | 1 | 28 | 1 | 9 | 5 |  | 1 |  |
| Named argument |  | 136 | 25 | 16 | 23 | 16 |  | 18 | 3 | 49 |  |  |  |  |  |
| for | 41 | 11 | 6 | 60 | 58 | 27 | 6 | 14 | 7 | 16 | 2 |  |  |  |  |
| Init accessor | 3 |  |  |  |  | 3 | 1 | 211 |  |  |  |  |  |  |  |
| while | 9 | 16 | 5 | 97 | 11 | 25 | 2 | 17 | 1 | 5 |  |  |  |  |  |
| typeof | 161 |  | 5 | 3 |  | 3 |  |  |  | 4 | 10 |  |  |  |  |
| Discard pattern | 14 |  | 1 | 15 | 27 | 67 | 20 | 28 | 9 | 1 |  | 2 |  |  |  |
| Switch expression | 12 |  | 1 | 15 | 27 | 65 | 19 | 28 | 9 | 1 |  | 2 |  |  |  |
| throw expression | 4 | 4 | 33 | 6 | 73 | 1 | 16 | 3 | 8 | 26 | 1 |  |  |  |  |
| Spread element | 9 | 19 | 8 | 2 | 35 | 25 | 6 | 36 | 9 | 13 | 1 |  |  |  |  |
| using statement |  | 36 | 2 | 115 | 7 |  |  |  |  |  | 1 |  |  |  |  |
| Field-like event | 53 |  |  |  |  |  | 93 |  | 4 |  |  |  |  |  |  |
| Discard _ | 20 | 22 | 59 | 1 | 8 | 9 | 2 | 9 | 6 | 1 |  |  |  |  |  |
| Case guard when | 6 |  |  | 1 | 3 | 100 |  | 3 |  | 2 |  |  |  |  |  |
| Partial type | 43 |  | 4 | 2 |  | 7 |  |  |  | 26 |  |  | 31 |  |  |
| Interface | 2 |  |  |  | 1 |  | 1 | 66 | 27 |  |  |  |  |  |  |
| Verbatim string |  | 5 |  |  | 3 | 78 |  |  |  | 2 |  |  |  |  |  |
| as cast | 58 |  |  | 3 |  | 9 |  |  |  | 15 |  |  |  |  |  |
| Range .. |  | 3 | 16 | 15 | 2 | 30 |  | 18 |  |  |  |  |  |  |  |
| Relational pattern | 1 |  |  | 25 | 10 | 3 |  | 40 | 1 |  |  |  |  |  |  |
| Exception filter | 3 |  |  | 34 | 16 | 3 |  |  | 19 |  |  |  |  |  |  |
| Index from end ^ | 2 | 13 | 38 | 2 | 4 | 11 |  | 4 |  |  |  |  |  |  |  |
| is type test | 21 | 1 |  | 2 | 3 | 32 |  |  |  |  |  |  |  |  |  |
| Switch statement | 7 |  |  | 8 |  | 20 | 2 | 19 | 1 | 2 |  |  |  |  |  |
| Generic method | 25 |  | 1 |  | 13 | 1 | 4 | 7 | 2 | 5 |  |  |  |  |  |
| yield return / break | 9 |  |  | 7 | 7 | 25 |  | 2 |  |  |  |  |  |  |  |
| Local const |  | 25 | 1 | 4 |  | 6 | 1 |  |  |  |  |  |  |  |  |
| finally | 1 | 6 |  | 2 | 11 | 2 | 4 |  | 4 |  | 1 | 2 |  |  |  |
| Enum | 2 |  |  |  |  |  | 11 | 19 |  |  |  |  |  |  |  |
| var pattern |  |  |  |  |  | 28 | 1 |  |  |  |  |  |  |  |  |
| Nested type | 7 | 8 | 3 |  |  |  |  |  |  | 10 |  |  |  |  |  |
| ref or out parameter | 2 | 2 | 1 | 1 | 6 | 3 |  | 4 | 3 | 4 |  |  |  |  |  |
| Constraint clause | 14 |  | 1 |  | 1 |  |  |  |  | 4 |  |  |  |  |  |
| Null-coalescing assignment ??= | 10 |  |  | 1 | 3 | 3 |  |  |  |  |  | 1 |  |  |  |
| Local function | 4 | 1 |  |  |  | 4 | 5 |  |  | 1 | 1 | 1 |  |  |  |
| Collection initializer | 3 |  | 3 | 5 | 2 |  |  |  |  | 1 | 2 |  |  |  |  |
| List pattern | 4 | 1 |  |  |  | 8 |  |  | 1 |  |  |  |  |  |  |
| Conversion operator | 12 |  |  |  |  |  |  | 1 |  |  |  |  |  |  |  |
| Type pattern | 1 |  |  |  |  | 10 |  |  |  |  |  |  |  |  |  |
| Default interface method body |  |  |  |  |  |  |  |  | 9 |  |  |  |  |  |  |
| Positional pattern | 2 |  |  | 1 | 1 | 1 | 4 |  |  |  |  |  |  |  |  |
| Async lambda |  |  | 4 |  | 1 |  |  |  |  |  |  | 2 |  |  |  |
| default literal |  |  | 1 | 1 |  | 5 |  |  |  |  |  |  |  |  |  |
| Required member | 2 |  |  |  |  | 3 |  |  |  |  |  |  |  |  |  |
| Event accessors | 3 |  |  |  |  |  |  |  | 1 |  |  |  |  |  |  |
| Primary constructor on class or struct |  |  |  |  |  | 4 |  |  |  |  |  |  |  |  |  |
| Abstract class | 1 |  |  |  | 1 |  |  |  |  |  |  |  |  |  |  |
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
| Object creation | 50.89 % | 50.88 % | 33.68 % | 23.99 % | 28.85 % | 42.00 % | 8.76 % | 26.74 % | 30.43 % | 31.15 % | 39.77 % | 7.69 % | - | 14.29 % | 21.05 % |
| Collection creation | 94.90 % | 99.58 % | 99.14 % | 97.86 % | 99.39 % | 99.05 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 92.86 % | - | - | - | - |
| Member body | 32.23 % | 2.07 % | 2.75 % | 0.63 % | 10.97 % | 3.69 % | 17.60 % | 30.56 % | 16.16 % | 79.06 % | 38.16 % | 0.00 % | 0.00 % | 0.00 % | - |
| Local type | 4.58 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.32 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | - | 0.00 % | 0.00 % |
| Using | 100.00 % | 98.19 % | 99.92 % | 83.41 % | 63.16 % | 100.00 % | - | - | - | 100.00 % | 95.45 % | - | - | - | - |
| Namespace | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - |
| Text building | 55.88 % | 33.12 % | 15.91 % | 58.09 % | 76.12 % | 82.07 % | 0.00 % | 26.09 % | - | 27.27 % | - | 0.00 % | - | 0.00 % | - |
| Branch | 63.16 % | - | 100.00 % | 65.22 % | 100.00 % | 76.47 % | 90.48 % | 59.57 % | 90.00 % | 33.33 % | - | 100.00 % | - | - | - |
| Lambda body | 82.87 % | 96.59 % | 79.50 % | 85.71 % | 92.08 % | 99.04 % | 97.90 % | 98.15 % | 90.91 % | 82.04 % | 42.42 % | 50.00 % | - | - | 77.78 % |
| Type test | 91.13 % | 100.00 % | 100.00 % | 90.63 % | 100.00 % | 94.51 % | 100.00 % | 100.00 % | 100.00 % | 40.00 % | 100.00 % | - | - | - | - |
| Constructor | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 36.36 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | - | 0.00 % | 0.00 % | - | - |
