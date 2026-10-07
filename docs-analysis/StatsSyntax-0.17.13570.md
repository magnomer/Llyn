# Syntax statistics - 0.17.13570

- Generated: 2026-10-07 11:50:01 +09:00
- Source roots: src, tests
- Root-level files: none
- Project segments: 1
- Excluded directories: .git, .vs, artifacts, bin, node_modules, obj, packages, publish
- Excluded suffixes: .g.cs, .g.i.cs, .AssemblyInfo.cs, .GlobalUsings.g.cs, .Designer.cs
- Roslyn: 4.14.0-3.25262.10
- Language version: Preview
- Scanned: 1,706 files, 15 projects, 187,401 lines

## Summary

| Measure | Value |
|---|---:|
| Files | 1,706 |
| Lines | 187,401 |
| Nodes | 985,636 |
| Syntax kinds used | 191 |
| Features used | 98 of 138 |
| Minimum C# version | 12 |
| Parse errors | 0 |

## Families

| Family | Features | Used | Count | Per 1k lines |
|---|---:|---:|---:|---:|
| Types | 15 | 12 | 3,643 | 19.44 |
| Namespaces | 6 | 2 | 1,706 | 9.10 |
| Members | 29 | 20 | 9,237 | 49.29 |
| Patterns | 16 | 16 | 5,841 | 31.17 |
| Expressions | 36 | 29 | 23,255 | 124.09 |
| Statements | 20 | 13 | 11,919 | 63.60 |
| Generics | 6 | 2 | 75 | 0.40 |
| Async | 2 | 2 | 437 | 2.33 |
| Nullability | 3 | 2 | 3,724 | 19.87 |
| Unsafe | 2 | 0 | 0 | 0.00 |
| Directives | 3 | 0 | 0 | 0.00 |
| Total | 138 | 98 | 59,837 | 319.30 |

## Projects

| Project | Files | Lines | Nodes | Features used | Newest C# |
|---|---:|---:|---:|---:|---:|
| Llyn.UIDeportment | 283 | 32,511 | 151,105 | 86 | 12 |
| Llyn.Tests.Engine | 192 | 30,592 | 214,037 | 59 | 12 |
| Llyn.Tests.Conduct | 124 | 24,096 | 174,004 | 60 | 12 |
| Llyn.Infrastructure | 156 | 21,717 | 98,599 | 71 | 12 |
| Llyn.Application | 109 | 17,562 | 81,202 | 69 | 12 |
| Llyn.Tests.Convention | 112 | 17,326 | 84,638 | 79 | 12 |
| Llyn.Conduct | 226 | 15,024 | 58,330 | 58 | 12 |
| Llyn.Core | 307 | 10,286 | 40,786 | 58 | 12 |
| Llyn.ShellEngine | 52 | 8,517 | 34,905 | 58 | 12 |
| Llyn.Tests.Interface | 87 | 7,770 | 38,439 | 64 | 12 |
| Llyn.Tests.Windows | 18 | 1,128 | 6,701 | 38 | 12 |
| Llyn.Core.Windows | 4 | 385 | 1,427 | 26 | 10 |
| Llyn.UIVeneer | 31 | 339 | 672 | 4 | 10 |
| Llyn.UIDeportment.Capsule | 4 | 88 | 380 | 15 | 10 |
| Llyn.Host | 1 | 60 | 411 | 3 | 9 |
| Total | 1,706 | 187,401 | 985,636 | 98 | 12 |

## Versions

| C# version | Features | Used | Count |
|---|---:|---:|---:|
| 1 | 51 | 35 | 15,904 |
| 2 | 10 | 8 | 1,972 |
| 3 | 7 | 5 | 5,686 |
| 4 | 3 | 2 | 652 |
| 5 | 3 | 3 | 1,027 |
| 6 | 6 | 5 | 5,099 |
| 7 | 11 | 10 | 5,630 |
| 7.1 | 1 | 1 | 7 |
| 7.2 | 1 | 0 | 0 |
| 7.3 | 1 | 0 | 0 |
| 8 | 17 | 12 | 9,820 |
| 9 | 10 | 9 | 5,481 |
| 10 | 3 | 2 | 1,707 |
| 11 | 6 | 3 | 594 |
| 12 | 4 | 3 | 6,258 |
| 13 | 1 | 0 | 0 |
| 14 | 3 | 0 | 0 |

## Features

| Feature | Family | C# | Count | Per 1k lines | Files | First |
|---|---|---|---:|---:|---:|---|
| Collection expression | Expressions | 12 | 6,085 | 32.47 | 775 | src/Llyn.Application/Card/LCardClerk.cs:57 |
| using declaration | Statements | 8 | 4,913 | 26.22 | 369 | src/Llyn.Application/Card/LTranslationClerk.cs:159 |
| Lambda | Expressions | 3 | 3,909 | 20.86 | 613 | src/Llyn.Application/Card/LCardClerk.cs:174 |
| if | Statements | 1 | 3,854 | 20.57 | 613 | src/Llyn.Application/Card/LCardClerk.cs:70 |
| Nullable reference type T? | Nullability | 8 | 3,202 | 17.09 | 850 | src/Llyn.Application/Card/LCardClerkField.cs:91 |
| Expression-bodied member | Members | 6 | 2,817 | 15.03 | 434 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Target-typed new | Expressions | 9 | 1,937 | 10.34 | 473 | src/Llyn.Application/Card/LMentionClerk.cs:167 |
| Declaration pattern | Patterns | 7 | 1,794 | 9.57 | 338 | src/Llyn.Application/Card/LMeaningClerk.cs:93 |
| File-scoped namespace | Namespaces | 10 | 1,705 | 9.10 | 1,705 | src/Llyn.Application/Card/LCardClerk.cs:5 |
| Readonly field | Members | 1 | 1,548 | 8.26 | 500 | src/Llyn.Application/Card/LCardClerk.cs:9 |
| Full property | Members | 1 | 1,423 | 7.59 | 316 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| foreach | Statements | 1 | 1,390 | 7.42 | 429 | src/Llyn.Application/Card/LCardClerk.cs:59 |
| Sealed class | Types | 1 | 1,356 | 7.24 | 1,254 | src/Llyn.Application/Card/LCardClerk.cs:7 |
| Tuple literal or tuple type | Expressions | 7 | 1,320 | 7.04 | 244 | src/Llyn.Application/Card/LMeaningClerk.cs:26 |
| Conditional ?: | Expressions | 1 | 1,264 | 6.74 | 477 | src/Llyn.Application/Card/LCardClerk.cs:173 |
| Class | Types | 1 | 1,228 | 6.55 | 1,210 | src/Llyn.Application/Card/LCardClerk.cs:7 |
| Null-conditional ?. ?[ | Expressions | 6 | 1,142 | 6.09 | 345 | src/Llyn.Application/Card/LMentionClerk.cs:47 |
| Constant pattern | Patterns | 7 | 1,003 | 5.35 | 178 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Static lambda | Expressions | 9 | 970 | 5.18 | 235 | src/Llyn.Application/Card/LCardEquality.cs:113 |
| Null-coalescing ?? | Expressions | 2 | 889 | 4.74 | 364 | src/Llyn.Application/Card/LCardClerkField.cs:74 |
| not pattern | Patterns | 9 | 833 | 4.45 | 329 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| Auto property | Members | 3 | 761 | 4.06 | 234 | src/Llyn.Application/Citation/LCitationClerk.cs:40 |
| is null | Patterns | 7 | 724 | 3.86 | 313 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| Null-forgiving ! | Expressions | 8 | 721 | 3.85 | 264 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:97 |
| Interpolated string | Expressions | 6 | 677 | 3.61 | 157 | src/Llyn.Application/Localization/LLocalizationReader.cs:55 |
| with expression | Expressions | 9 | 626 | 3.34 | 214 | src/Llyn.Application/Card/LCardClerk.cs:109 |
| await | Expressions | 5 | 590 | 3.15 | 139 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:139 |
| Extension method | Members | 3 | 582 | 3.11 | 41 | tests/Llyn.Tests.Interface/TInterfaceArea.cs:7 |
| Raw string | Expressions | 11 | 575 | 3.07 | 132 | src/Llyn.Infrastructure/Database/Base/LDatabase.cs:51 |
| Const field | Members | 1 | 552 | 2.95 | 230 | src/Llyn.Application/Card/LMentionClerk.cs:11 |
| Nullable value type T? | Nullability | 2 | 522 | 2.79 | 229 | src/Llyn.Application/Card/LMeaningClerk.cs:92 |
| Record class | Types | 9 | 480 | 2.56 | 393 | src/Llyn.Application/Ensign/LEnsignRow.cs:3 |
| Property pattern | Patterns | 8 | 454 | 2.42 | 107 | src/Llyn.Application/Draft/Clerk/LDraftClerk.cs:179 |
| Constructor | Members | 1 | 449 | 2.40 | 443 | src/Llyn.Application/Card/LCardClerk.cs:19 |
| Async method | Async | 5 | 430 | 2.29 | 139 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:91 |
| Object initializer | Expressions | 3 | 418 | 2.23 | 176 | src/Llyn.Core.Windows/LUsherShell.cs:37 |
| Optional parameter | Members | 4 | 413 | 2.20 | 135 | src/Llyn.Application/Draft/Clerk/LDraftClerkPanel.cs:128 |
| Cast expression | Expressions | 1 | 407 | 2.17 | 146 | src/Llyn.Application/Markup/LMarkupClerkLink.cs:234 |
| nameof | Expressions | 6 | 388 | 2.07 | 128 | src/Llyn.Application/Citation/LAuthorClerk.cs:361 |
| catch | Statements | 1 | 344 | 1.84 | 141 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:142 |
| lock | Statements | 1 | 334 | 1.78 | 53 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:98 |
| and / or pattern | Patterns | 9 | 330 | 1.76 | 97 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Deconstruction | Expressions | 7 | 321 | 1.71 | 133 | src/Llyn.Application/Card/LMeaningClerk.cs:46 |
| Static class | Types | 2 | 309 | 1.65 | 309 | src/Llyn.Application/Card/LCardClerkField.cs:7 |
| try | Statements | 1 | 299 | 1.60 | 150 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:137 |
| for | Statements | 1 | 249 | 1.33 | 152 | src/Llyn.Application/Card/LCardClerk.cs:247 |
| Named argument | Members | 4 | 239 | 1.28 | 81 | src/Llyn.Application/Card/LTranslationClerk.cs:163 |
| Init accessor | Members | 9 | 215 | 1.15 | 72 | src/Llyn.Core/Catalog/Kind/LCatalogAuthor.cs:13 |
| typeof | Expressions | 1 | 185 | 0.99 | 42 | src/Llyn.Infrastructure/Configuration/LLocalizationLoader.cs:20 |
| while | Statements | 1 | 185 | 0.99 | 110 | src/Llyn.Application/Card/LMeaningClerk.cs:44 |
| throw expression | Expressions | 7 | 173 | 0.92 | 88 | src/Llyn.Application/Card/LCardClerkField.cs:75 |
| Spread element | Expressions | 12 | 169 | 0.90 | 87 | src/Llyn.Application/Card/LCardEquality.cs:53 |
| Discard pattern | Patterns | 8 | 168 | 0.90 | 98 | src/Llyn.Application/Citation/LAuthorClerk.cs:243 |
| Switch expression | Patterns | 8 | 162 | 0.86 | 97 | src/Llyn.Application/Citation/LAuthorClerk.cs:239 |
| using statement | Statements | 1 | 159 | 0.85 | 66 | src/Llyn.Application/Citation/LAuthorCitation.cs:51 |
| Field-like event | Members | 1 | 149 | 0.80 | 83 | src/Llyn.Conduct/Atelier/CNavigation.cs:46 |
| Discard _ | Expressions | 7 | 136 | 0.73 | 47 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:60 |
| Partial type | Types | 2 | 125 | 0.67 | 125 | src/Llyn.Infrastructure/Database/Context/LSituationArchive.cs:8 |
| Case guard when | Patterns | 7 | 113 | 0.60 | 23 | src/Llyn.Application/Citation/LAuthorClerk.cs:241 |
| Verbatim string | Expressions | 1 | 88 | 0.47 | 19 | src/Llyn.Application/Localization/LLocalizationReader.cs:13 |
| Range .. | Expressions | 8 | 84 | 0.45 | 40 | src/Llyn.Application/Card/LMentionClerk.cs:173 |
| Relational pattern | Patterns | 9 | 79 | 0.42 | 28 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Interface | Types | 1 | 76 | 0.41 | 76 | src/Llyn.Application/Pronunciation/Relay/LListener.cs:5 |
| Exception filter | Statements | 6 | 75 | 0.40 | 45 | src/Llyn.Application/Outpost/LCourierClerk.cs:107 |
| as cast | Expressions | 1 | 70 | 0.37 | 46 | src/Llyn.Infrastructure/Database/Base/LWorkspaceArchive.cs:132 |
| Index from end ^ | Expressions | 8 | 68 | 0.36 | 39 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:230 |
| is type test | Patterns | 1 | 59 | 0.31 | 36 | src/Llyn.Application/Outpost/LCourierClerk.cs:363 |
| Switch statement | Patterns | 1 | 59 | 0.31 | 35 | src/Llyn.Conduct/Configuration/CLedger.cs:240 |
| Generic method | Generics | 2 | 55 | 0.29 | 28 | src/Llyn.Application/Card/LCardClerkField.cs:183 |
| yield return / break | Statements | 2 | 50 | 0.27 | 19 | src/Llyn.Application/Card/LCardClerkField.cs:35 |
| Local const | Statements | 1 | 35 | 0.19 | 20 | src/Llyn.Conduct/Lexicon/CStateWording.cs:10 |
| finally | Statements | 1 | 32 | 0.17 | 25 | src/Llyn.Application/Outpost/LCourierClerk.cs:83 |
| Enum | Types | 1 | 31 | 0.17 | 31 | src/Llyn.Conduct/Catalog/CCatalogOrder.cs:3 |
| var pattern | Patterns | 7 | 29 | 0.15 | 11 | src/Llyn.Conduct/Card/CFolio.cs:193 |
| Nested type | Types | 1 | 28 | 0.15 | 17 | src/Llyn.UIDeportment/Kit/Bind/QLook.cs:19 |
| ref or out parameter | Members | 1 | 26 | 0.14 | 20 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:101 |
| Constraint clause | Generics | 2 | 20 | 0.11 | 10 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:171 |
| Default interface method body | Members | 8 | 19 | 0.10 | 6 | src/Llyn.ShellEngine/Port/LDraftPort.cs:58 |
| Null-coalescing assignment ??= | Expressions | 8 | 18 | 0.10 | 16 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:160 |
| Local function | Members | 7 | 17 | 0.09 | 11 | src/Llyn.Conduct/Desk/CErrand.cs:54 |
| Collection initializer | Expressions | 3 | 16 | 0.09 | 12 | src/Llyn.Application/Outpost/LCourierClerk.cs:253 |
| List pattern | Patterns | 11 | 14 | 0.07 | 12 | src/Llyn.ShellEngine/Engine/LDraftFacade.cs:66 |
| Conversion operator | Members | 1 | 13 | 0.07 | 2 | src/Llyn.Core/Lexicon/State/LStateValue.cs:49 |
| Type pattern | Patterns | 9 | 11 | 0.06 | 4 | src/Llyn.UIDeportment/Sound/Accent/QAccentControl.cs:62 |
| Positional pattern | Patterns | 8 | 9 | 0.05 | 9 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:37 |
| Async lambda | Async | 5 | 7 | 0.04 | 4 | src/Llyn.Application/Outpost/LCourierClerk.cs:327 |
| default literal | Expressions | 7.1 | 7 | 0.04 | 5 | src/Llyn.Infrastructure/Pronunciation/Loader/LSourceLoader.cs:73 |
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
| Null check | is null / is not null | 724 | == null / != null | 0 | 100.00 % |
| Object creation | Target-typed new() | 1,937 | new T() | 3,492 | 35.68 % |
| Collection creation | Collection expression | 6,085 | Collection initializer or array creation | 50 | 99.19 % |
| Member body | Expression body | 2,817 | Block body | 9,484 | 22.90 % |
| Local type | var | 42 | Explicit type | 19,079 | 0.22 % |
| Using | Declaration | 4,913 | Statement | 159 | 96.87 % |
| Namespace | File-scoped | 1,705 | Block | 0 | 100.00 % |
| Text building | Interpolated string | 677 | string.Format or + with a string literal | 418 | 61.83 % |
| Branch | Switch expression | 162 | Switch statement | 59 | 73.30 % |
| Lambda body | Expression | 3,491 | Block | 418 | 89.31 % |
| Type test | is pattern with designation | 1,006 | as | 70 | 93.49 % |
| Constructor | Primary | 4 | Explicit | 443 | 0.89 % |

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
| IdentifierName | 284,267 | 1,706 |
| Argument | 102,469 | 1,298 |
| SimpleMemberAccessExpression | 92,833 | 1,290 |
| ArgumentList | 63,872 | 1,331 |
| InvocationExpression | 58,566 | 1,300 |
| PredefinedType | 28,910 | 1,606 |
| ExpressionStatement | 28,031 | 1,188 |
| StringLiteralExpression | 22,660 | 961 |
| Parameter | 20,517 | 1,537 |
| VariableDeclaration | 19,562 | 1,123 |
| VariableDeclarator | 19,562 | 1,123 |
| EqualsValueClause | 18,823 | 1,184 |
| Block | 16,846 | 1,254 |
| LocalDeclarationStatement | 16,485 | 980 |
| ParameterList | 12,683 | 1,636 |
| MethodDeclaration | 10,790 | 1,295 |
| NumericLiteralExpression | 10,102 | 972 |
| GenericName | 9,758 | 1,154 |
| TypeArgumentList | 9,758 | 1,154 |
| QualifiedName | 8,005 | 1,706 |
| SimpleAssignmentExpression | 6,730 | 880 |
| ExpressionElement | 6,331 | 422 |
| CollectionExpression | 6,085 | 775 |
| ReturnStatement | 5,534 | 941 |
| UsingDirective | 5,362 | 1,454 |
| IfStatement | 3,854 | 613 |
| NullLiteralExpression | 3,759 | 771 |
| NullableType | 3,724 | 890 |
| ObjectCreationExpression | 3,492 | 749 |
| Attribute | 3,101 | 367 |
| AttributeList | 3,101 | 367 |
| SingleVariableDesignation | 3,028 | 505 |
| SimpleLambdaExpression | 2,982 | 546 |
| ArrowExpressionClause | 2,819 | 435 |
| BracketedArgumentList | 2,786 | 490 |
| FieldDeclaration | 2,525 | 659 |
| IsPatternExpression | 2,497 | 512 |
| ElementAccessExpression | 2,201 | 437 |
| PropertyDeclaration | 2,188 | 442 |
| ImplicitObjectCreationExpression | 1,937 | 473 |
| FalseLiteralExpression | 1,893 | 514 |
| DeclarationPattern | 1,794 | 338 |
| ConstantPattern | 1,727 | 400 |
| CompilationUnit | 1,706 | 1,706 |
| FileScopedNamespaceDeclaration | 1,705 | 1,705 |
| TrueLiteralExpression | 1,516 | 450 |
| InterpolatedStringText | 1,482 | 157 |
| EqualsExpression | 1,453 | 521 |
| TupleExpression | 1,325 | 251 |
| AddExpression | 1,322 | 313 |
| ForEachStatement | 1,311 | 418 |
| Interpolation | 1,309 | 156 |
| ConditionalExpression | 1,264 | 477 |
| LogicalAndExpression | 1,264 | 346 |
| ClassDeclaration | 1,228 | 1,210 |
| ConditionalAccessExpression | 1,142 | 345 |
| MemberBindingExpression | 1,142 | 345 |
| LogicalNotExpression | 1,124 | 369 |
| AddAssignmentExpression | 1,097 | 237 |
| DeclarationExpression | 957 | 248 |
| AttributeArgument | 928 | 52 |
| ParenthesizedLambdaExpression | 927 | 264 |
| SwitchExpressionArm | 914 | 97 |
| CoalesceExpression | 889 | 364 |
| LogicalOrExpression | 887 | 279 |
| AccessorList | 882 | 258 |
| GetAccessorDeclaration | 877 | 256 |
| NotPattern | 833 | 329 |
| TupleElement | 744 | 175 |
| SuppressNullableWarningExpression | 721 | 264 |
| InterpolatedStringExpression | 677 | 157 |
| GreaterThanExpression | 632 | 276 |
| WithExpression | 626 | 214 |
| WithInitializerExpression | 626 | 214 |
| NameColon | 601 | 178 |
| ArrayRankSpecifier | 597 | 231 |
| ArrayType | 596 | 231 |
| AwaitExpression | 590 | 139 |
| ImplicitElementAccess | 585 | 106 |
| OmittedArraySizeExpression | 580 | 231 |
| CharacterLiteralExpression | 497 | 112 |
| PostIncrementExpression | 496 | 221 |
| RecordDeclaration | 480 | 393 |
| RecursivePattern | 463 | 114 |
| LessThanExpression | 461 | 206 |
| PropertyPatternClause | 454 | 107 |
| ConstructorDeclaration | 451 | 445 |
| ObjectInitializerExpression | 418 | 176 |
| Subpattern | 411 | 106 |
| CastExpression | 407 | 146 |
| NotEqualsExpression | 400 | 167 |
| AttributeArgumentList | 396 | 52 |
| CatchClause | 344 | 141 |
| ParenthesizedExpression | 344 | 172 |
| CatchDeclaration | 341 | 139 |
| BaseList | 340 | 247 |
| LockStatement | 334 | 53 |
| SwitchSection | 318 | 35 |
| TupleType | 317 | 175 |
| BreakStatement | 304 | 60 |
| ContinueStatement | 301 | 146 |
| TryStatement | 299 | 150 |
| OrPattern | 288 | 91 |
| ForStatement | 249 | 152 |
| SimpleBaseType | 242 | 228 |
| InitAccessorDeclaration | 215 | 72 |
| EnumMemberDeclaration | 213 | 31 |
| ThrowStatement | 197 | 96 |
| TypeOfExpression | 185 | 42 |
| WhileStatement | 185 | 110 |
| SubtractExpression | 183 | 84 |
| ThrowExpression | 173 | 88 |
| CaseSwitchLabel | 172 | 23 |
| SpreadElement | 169 | 87 |
| DiscardPattern | 168 | 98 |
| SwitchExpression | 162 | 97 |
| UsingStatement | 159 | 66 |
| SubtractAssignmentExpression | 157 | 66 |
| UnaryMinusExpression | 157 | 69 |
| SetAccessorDeclaration | 152 | 70 |
| ThisExpression | 152 | 81 |
| EventFieldDeclaration | 149 | 83 |
| ElseClause | 140 | 113 |
| WhenClause | 113 | 23 |
| CasePatternSwitchLabel | 111 | 20 |
| LessThanOrEqualExpression | 111 | 79 |
| PrimaryConstructorBaseType | 102 | 19 |
| GreaterThanOrEqualExpression | 97 | 63 |
| RangeExpression | 84 | 40 |
| MultiplyExpression | 81 | 32 |
| ForEachVariableStatement | 79 | 54 |
| RelationalPattern | 79 | 28 |
| InterfaceDeclaration | 76 | 76 |
| CatchFilterClause | 75 | 45 |
| AsExpression | 70 | 46 |
| IndexExpression | 68 | 39 |
| TypeParameter | 60 | 30 |
| IsExpression | 59 | 36 |
| SwitchStatement | 59 | 35 |
| TypeParameterList | 57 | 30 |
| ArrayInitializerExpression | 53 | 28 |
| BitwiseOrExpression | 47 | 24 |
| YieldReturnStatement | 43 | 19 |
| AndPattern | 42 | 17 |
| DefaultSwitchLabel | 41 | 22 |
| ParenthesizedPattern | 41 | 18 |
| OrAssignmentExpression | 37 | 21 |
| ExpressionColon | 33 | 14 |
| FinallyClause | 32 | 25 |
| ImplicitArrayCreationExpression | 32 | 26 |
| LeftShiftExpression | 32 | 2 |
| DivideExpression | 31 | 18 |
| EnumDeclaration | 31 | 31 |
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
| GlobalStatement | 6 | 1 |
| NameEquals | 6 | 4 |
| AddAccessorDeclaration | 4 | 3 |
| EventDeclaration | 4 | 3 |
| ExplicitInterfaceSpecifier | 4 | 3 |
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
| Fact | 2,628 | 353 |
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

| Feature | Llyn.UIDeportment | Llyn.Tests.Engine | Llyn.Tests.Conduct | Llyn.Infrastructure | Llyn.Application | Llyn.Tests.Convention | Llyn.Conduct | Llyn.Core | Llyn.ShellEngine | Llyn.Tests.Interface | Llyn.Tests.Windows | Llyn.Core.Windows | Llyn.UIVeneer | Llyn.UIDeportment.Capsule | Llyn.Host |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| Collection expression | 148 | 2,449 | 1,589 | 272 | 323 | 523 | 157 | 269 | 83 | 211 | 61 |  |  |  |  |
| using declaration | 5 | 1,872 | 2,409 | 569 | 12 | 7 |  |  |  | 18 | 21 |  |  |  |  |
| Lambda | 179 | 723 | 1,252 | 28 | 332 | 730 | 320 | 93 | 64 | 144 | 32 | 4 |  |  | 8 |
| if | 1,186 | 49 | 6 | 628 | 682 | 464 | 369 | 216 | 209 | 29 | 3 | 10 |  | 3 |  |
| Nullable reference type T? | 456 | 147 | 355 | 319 | 316 | 297 | 310 | 317 | 255 | 390 | 29 | 7 |  | 4 |  |
| Expression-bodied member | 933 | 34 | 35 | 6 | 109 | 26 | 247 | 153 | 208 | 1,038 | 28 |  |  |  |  |
| Target-typed new | 717 | 142 | 198 | 128 | 149 | 189 | 35 | 44 | 75 | 220 | 34 | 1 |  | 1 | 4 |
| Declaration pattern | 898 | 5 | 4 | 36 | 193 | 368 | 187 | 21 | 71 | 8 | 3 |  |  |  |  |
| File-scoped namespace | 283 | 192 | 124 | 156 | 109 | 112 | 226 | 307 | 52 | 87 | 18 | 4 | 31 | 4 |  |
| Readonly field | 430 | 37 | 4 | 105 | 370 | 215 | 229 | 28 | 90 | 35 |  | 4 |  | 1 |  |
| Full property | 873 | 7 | 4 | 6 | 109 | 2 | 232 | 141 | 38 | 11 |  |  |  |  |  |
| foreach | 184 | 49 | 18 | 272 | 315 | 306 | 25 | 145 | 35 | 40 | 1 |  |  |  |  |
| Sealed class | 234 | 197 | 127 | 72 | 171 | 43 | 207 | 204 | 46 | 32 | 15 | 4 |  | 4 |  |
| Tuple literal or tuple type | 38 | 391 | 266 | 170 | 81 | 168 | 53 | 42 | 76 | 25 | 10 |  |  |  |  |
| Conditional ?: | 217 | 12 | 16 | 268 | 236 | 156 | 142 | 125 | 60 | 28 | 2 | 2 |  |  |  |
| Class | 270 | 200 | 125 | 154 | 88 | 105 | 70 | 22 | 45 | 95 | 18 | 4 | 31 | 1 |  |
| Null-conditional ?. ?[ | 175 | 129 | 72 | 58 | 47 | 91 | 443 | 35 | 76 | 9 | 6 | 1 |  |  |  |
| Constant pattern | 133 | 3 | 8 | 140 | 51 | 353 | 126 | 124 | 43 | 17 |  | 5 |  |  |  |
| Static lambda | 78 | 70 | 521 | 4 | 114 | 15 | 62 | 37 | 15 | 51 | 3 |  |  |  |  |
| Null-coalescing ?? | 79 | 18 | 12 | 101 | 175 | 80 | 91 | 246 | 38 | 43 | 5 |  |  | 1 |  |
| not pattern | 307 | 4 | 1 | 50 | 69 | 185 | 129 | 31 | 50 | 6 | 1 |  |  |  |  |
| Auto property | 344 | 5 | 3 | 8 | 7 | 23 | 81 | 225 | 44 | 21 |  |  |  |  |  |
| is null | 122 | 7 | 5 | 91 | 149 | 175 | 71 | 37 | 52 | 13 | 2 |  |  |  |  |
| Null-forgiving ! | 165 | 205 | 206 | 56 | 9 | 25 | 5 |  | 3 | 46 | 1 |  |  |  |  |
| Interpolated string | 19 | 49 | 7 | 162 | 51 | 380 |  | 6 |  | 3 |  |  |  |  |  |
| with expression | 7 | 203 | 31 | 42 | 240 | 10 | 15 | 17 | 36 | 25 |  |  |  |  |  |
| await | 57 | 226 | 142 | 62 | 56 | 1 | 13 |  | 17 | 5 | 1 | 10 |  |  |  |
| Extension method |  |  |  |  |  |  |  |  |  | 574 | 8 |  |  |  |  |
| Raw string | 2 | 114 | 7 | 211 |  | 233 |  |  |  | 8 |  |  |  |  |  |
| Const field | 74 | 66 | 14 | 177 | 14 | 113 | 3 | 70 | 4 | 12 |  | 4 |  | 1 |  |
| Nullable value type T? | 56 | 20 | 21 | 40 | 33 | 11 | 135 | 30 | 101 | 66 | 7 |  |  | 2 |  |
| Record class | 16 |  | 2 | 2 | 103 | 6 | 144 | 201 | 1 | 2 |  |  |  | 3 |  |
| Property pattern | 142 | 2 |  | 8 | 15 | 270 | 8 | 3 | 4 | 1 | 1 |  |  |  |  |
| Constructor | 165 | 2 | 1 | 62 | 68 | 7 | 60 | 3 | 41 | 7 |  | 2 | 31 |  |  |
| Async method | 56 | 178 | 97 | 32 | 29 | 1 | 13 |  | 13 | 5 | 1 | 5 |  |  |  |
| Object initializer | 135 | 50 | 94 | 29 |  | 60 |  | 3 |  | 32 | 13 | 1 |  | 1 |  |
| Optional parameter | 8 | 11 | 6 | 6 | 11 | 1 | 4 | 249 | 16 | 98 |  |  |  | 3 |  |
| Cast expression | 137 | 6 | 83 | 75 | 4 | 50 | 3 | 4 |  | 29 | 15 | 1 |  |  |  |
| nameof | 218 | 1 | 3 | 8 | 116 | 17 | 17 | 1 | 4 | 2 | 1 |  |  |  |  |
| catch | 21 | 1 |  | 126 | 33 | 8 | 109 | 1 | 27 | 1 | 8 | 6 |  | 3 |  |
| lock | 13 |  |  | 2 | 77 | 5 | 2 |  | 230 | 5 |  |  |  |  |  |
| and / or pattern | 37 | 1 | 3 | 68 | 14 | 159 | 2 | 39 | 1 | 6 |  |  |  |  |  |
| Deconstruction | 27 | 65 | 113 | 26 | 27 | 33 | 19 | 4 | 7 |  |  |  |  |  |  |
| Static class | 44 | 3 |  | 84 | 19 | 68 | 7 | 19 |  | 62 | 3 |  |  |  |  |
| try | 19 | 7 |  | 78 | 30 | 10 | 111 | 1 | 28 | 1 | 8 | 5 |  | 1 |  |
| for | 44 | 11 | 6 | 58 | 58 | 27 | 6 | 12 | 9 | 16 | 2 |  |  |  |  |
| Named argument |  | 92 | 25 | 16 | 23 | 16 |  | 18 | 3 | 46 |  |  |  |  |  |
| Init accessor | 3 |  |  |  |  | 3 |  | 209 |  |  |  |  |  |  |  |
| typeof | 161 |  | 4 | 3 |  | 3 |  |  |  | 4 | 10 |  |  |  |  |
| while | 9 | 16 | 5 | 95 | 11 | 25 | 2 | 16 | 1 | 5 |  |  |  |  |  |
| throw expression | 4 | 4 | 34 | 6 | 73 | 1 | 15 | 1 | 8 | 26 | 1 |  |  |  |  |
| Spread element | 9 | 16 | 8 | 2 | 33 | 25 | 6 | 43 | 13 | 13 | 1 |  |  |  |  |
| Discard pattern | 14 |  | 1 | 15 | 15 | 67 | 19 | 25 | 9 | 1 |  | 2 |  |  |  |
| Switch expression | 12 |  | 1 | 15 | 15 | 65 | 18 | 24 | 9 | 1 |  | 2 |  |  |  |
| using statement |  | 35 | 2 | 114 | 7 |  |  |  |  |  | 1 |  |  |  |  |
| Field-like event | 53 |  |  |  |  |  | 92 |  | 4 |  |  |  |  |  |  |
| Discard _ | 20 | 22 | 58 | 1 | 8 | 9 | 2 | 9 | 6 | 1 |  |  |  |  |  |
| Partial type | 48 |  | 4 | 5 |  | 7 |  |  | 4 | 26 |  |  | 31 |  |  |
| Case guard when | 6 |  |  | 1 | 1 | 100 |  | 3 |  | 2 |  |  |  |  |  |
| Verbatim string |  | 5 |  |  | 3 | 78 |  |  |  | 2 |  |  |  |  |  |
| Range .. |  | 3 | 16 | 15 | 2 | 30 |  | 18 |  |  |  |  |  |  |  |
| Relational pattern | 1 |  |  | 25 | 10 | 3 |  | 39 | 1 |  |  |  |  |  |  |
| Interface | 2 |  |  |  | 1 |  | 1 | 66 | 6 |  |  |  |  |  |  |
| Exception filter | 3 |  |  | 34 | 16 | 3 |  |  | 19 |  |  |  |  |  |  |
| as cast | 58 |  |  | 3 |  | 9 |  |  |  |  |  |  |  |  |  |
| Index from end ^ | 2 | 7 | 38 | 2 | 4 | 11 |  | 4 |  |  |  |  |  |  |  |
| is type test | 21 | 1 |  | 2 | 3 | 32 |  |  |  |  |  |  |  |  |  |
| Switch statement | 7 |  |  | 8 |  | 20 | 2 | 19 | 1 | 2 |  |  |  |  |  |
| Generic method | 25 |  | 1 |  | 13 | 1 | 4 | 4 | 2 | 5 |  |  |  |  |  |
| yield return / break | 9 |  |  | 7 | 7 | 25 |  | 2 |  |  |  |  |  |  |  |
| Local const |  | 23 | 1 | 4 |  | 6 | 1 |  |  |  |  |  |  |  |  |
| finally | 1 | 6 |  | 2 | 11 | 2 | 4 |  | 4 |  |  | 2 |  |  |  |
| Enum | 2 |  |  |  |  |  | 11 | 18 |  |  |  |  |  |  |  |
| var pattern |  |  |  |  |  | 28 | 1 |  |  |  |  |  |  |  |  |
| Nested type | 7 | 8 | 3 |  |  |  |  |  |  | 10 |  |  |  |  |  |
| ref or out parameter | 2 | 2 | 1 | 1 | 6 | 3 |  | 4 | 3 | 4 |  |  |  |  |  |
| Constraint clause | 14 |  | 1 |  | 1 |  |  |  |  | 4 |  |  |  |  |  |
| Default interface method body |  |  |  |  |  |  |  |  | 19 |  |  |  |  |  |  |
| Null-coalescing assignment ??= | 10 |  |  | 1 | 3 | 3 |  |  |  |  |  | 1 |  |  |  |
| Local function | 4 | 1 |  |  |  | 4 | 5 |  |  | 1 | 1 | 1 |  |  |  |
| Collection initializer | 3 |  | 3 | 5 | 2 |  |  |  |  | 1 | 2 |  |  |  |  |
| List pattern | 4 | 1 |  |  |  | 8 |  |  | 1 |  |  |  |  |  |  |
| Conversion operator | 12 |  |  |  |  |  |  | 1 |  |  |  |  |  |  |  |
| Type pattern | 1 |  |  |  |  | 10 |  |  |  |  |  |  |  |  |  |
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
| Object creation | 51.00 % | 51.26 % | 33.85 % | 24.06 % | 28.82 % | 42.00 % | 9.89 % | 24.72 % | 29.53 % | 29.18 % | 40.96 % | 7.69 % | - | 14.29 % | 21.05 % |
| Collection creation | 94.87 % | 99.59 % | 99.13 % | 97.84 % | 99.38 % | 99.05 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 92.42 % | - | - | - | - |
| Member body | 32.24 % | 2.18 % | 2.78 % | 0.64 % | 11.19 % | 3.69 % | 20.35 % | 31.42 % | 25.46 % | 78.64 % | 37.84 % | 0.00 % | 0.00 % | 0.00 % | - |
| Local type | 4.58 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.31 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | - | 0.00 % | 0.00 % |
| Using | 100.00 % | 98.16 % | 99.92 % | 83.31 % | 63.16 % | 100.00 % | - | - | - | 100.00 % | 95.45 % | - | - | - | - |
| Namespace | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - |
| Text building | 55.88 % | 32.24 % | 15.91 % | 58.70 % | 76.12 % | 82.07 % | 0.00 % | 26.09 % | - | 27.27 % | - | 0.00 % | - | 0.00 % | - |
| Branch | 63.16 % | - | 100.00 % | 65.22 % | 100.00 % | 76.47 % | 90.00 % | 55.81 % | 90.00 % | 33.33 % | - | 100.00 % | - | - | - |
| Lambda body | 83.80 % | 96.27 % | 79.31 % | 85.71 % | 91.87 % | 99.04 % | 98.44 % | 95.70 % | 90.63 % | 80.56 % | 43.75 % | 50.00 % | - | - | 75.00 % |
| Type test | 91.09 % | 100.00 % | 100.00 % | 90.00 % | 100.00 % | 94.51 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - | - | - | - |
| Constructor | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 36.36 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | - | 0.00 % | 0.00 % | - | - |
