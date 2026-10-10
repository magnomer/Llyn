# Syntax statistics - 0.18.14100

- Generated: 2026-10-10 21:42:15 +09:00
- Source roots: src, tests
- Root-level files: none
- Project segments: 1
- Excluded directories: .git, .vs, artifacts, bin, node_modules, obj, packages, publish
- Excluded suffixes: .g.cs, .g.i.cs, .AssemblyInfo.cs, .GlobalUsings.g.cs, .Designer.cs
- Roslyn: 4.14.0-3.25262.10
- Language version: Preview
- Scanned: 1,899 files, 15 projects, 199,697 lines

## Summary

| Measure | Value |
|---|---:|
| Files | 1,899 |
| Lines | 199,697 |
| Nodes | 1,058,718 |
| Syntax kinds used | 191 |
| Features used | 98 of 138 |
| Minimum C# version | 12 |
| Parse errors | 0 |

## Families

| Family | Features | Used | Count | Per 1k lines |
|---|---:|---:|---:|---:|
| Types | 15 | 12 | 3,948 | 19.77 |
| Namespaces | 6 | 2 | 1,899 | 9.51 |
| Members | 29 | 20 | 9,379 | 46.97 |
| Patterns | 16 | 16 | 6,085 | 30.47 |
| Expressions | 36 | 29 | 25,077 | 125.58 |
| Statements | 20 | 13 | 12,565 | 62.92 |
| Generics | 6 | 2 | 72 | 0.36 |
| Async | 2 | 2 | 457 | 2.29 |
| Nullability | 3 | 2 | 3,896 | 19.51 |
| Unsafe | 2 | 0 | 0 | 0.00 |
| Directives | 3 | 0 | 0 | 0.00 |
| Total | 138 | 98 | 63,378 | 317.37 |

## Projects

| Project | Files | Lines | Nodes | Features used | Newest C# |
|---|---:|---:|---:|---:|---:|
| Llyn.Tests.Engine | 213 | 34,758 | 242,916 | 59 | 12 |
| Llyn.UIDeportment | 301 | 32,520 | 149,736 | 86 | 12 |
| Llyn.Tests.Conduct | 143 | 25,392 | 185,821 | 61 | 12 |
| Llyn.Infrastructure | 171 | 22,853 | 103,840 | 70 | 12 |
| Llyn.Application | 122 | 18,333 | 84,660 | 70 | 12 |
| Llyn.Tests.Convention | 115 | 17,418 | 84,778 | 79 | 12 |
| Llyn.Conduct | 247 | 15,859 | 61,928 | 59 | 12 |
| Llyn.Core | 341 | 11,446 | 46,637 | 59 | 12 |
| Llyn.ShellEngine | 90 | 9,397 | 37,649 | 56 | 12 |
| Llyn.Tests.Interface | 88 | 8,465 | 42,728 | 65 | 12 |
| Llyn.Tests.Windows | 24 | 2,315 | 14,939 | 44 | 12 |
| Llyn.Core.Windows | 4 | 385 | 1,427 | 26 | 10 |
| Llyn.UIVeneer | 35 | 383 | 760 | 4 | 10 |
| Llyn.UIDeportment.Capsule | 4 | 88 | 380 | 15 | 10 |
| Llyn.Host | 1 | 85 | 519 | 4 | 9 |
| Total | 1,899 | 199,697 | 1,058,718 | 98 | 12 |

## Versions

| C# version | Features | Used | Count |
|---|---:|---:|---:|
| 1 | 51 | 35 | 16,799 |
| 2 | 10 | 8 | 1,987 |
| 3 | 7 | 5 | 6,253 |
| 4 | 3 | 2 | 731 |
| 5 | 3 | 3 | 1,069 |
| 6 | 6 | 5 | 4,716 |
| 7 | 11 | 10 | 5,858 |
| 7.1 | 1 | 1 | 7 |
| 7.2 | 1 | 0 | 0 |
| 7.3 | 1 | 0 | 0 |
| 8 | 17 | 12 | 10,512 |
| 9 | 10 | 9 | 5,993 |
| 10 | 3 | 2 | 1,901 |
| 11 | 6 | 3 | 629 |
| 12 | 4 | 3 | 6,923 |
| 13 | 1 | 0 | 0 |
| 14 | 3 | 0 | 0 |

## Features

| Feature | Family | C# | Count | Per 1k lines | Files | First |
|---|---|---|---:|---:|---:|---|
| Collection expression | Expressions | 12 | 6,739 | 33.75 | 858 | src/Llyn.Application/Card/LCardClerk.cs:57 |
| using declaration | Statements | 8 | 5,334 | 26.71 | 407 | src/Llyn.Application/Card/LTranslationClerk.cs:160 |
| Lambda | Expressions | 3 | 4,309 | 21.58 | 691 | src/Llyn.Application/Card/LCardClerk.cs:174 |
| if | Statements | 1 | 3,962 | 19.84 | 662 | src/Llyn.Application/Card/LCardClerk.cs:70 |
| Nullable reference type T? | Nullability | 8 | 3,348 | 16.77 | 919 | src/Llyn.Application/Card/LCardClerkField.cs:91 |
| Expression-bodied member | Members | 6 | 2,529 | 12.66 | 487 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Target-typed new | Expressions | 9 | 2,156 | 10.80 | 526 | src/Llyn.Application/Card/LMentionClerk.cs:167 |
| Readonly field | Members | 1 | 1,959 | 9.81 | 591 | src/Llyn.Application/Card/LCardClerk.cs:9 |
| File-scoped namespace | Namespaces | 10 | 1,898 | 9.50 | 1,898 | src/Llyn.Application/Card/LCardClerk.cs:5 |
| Declaration pattern | Patterns | 7 | 1,862 | 9.32 | 369 | src/Llyn.Application/Card/LMeaningClerk.cs:93 |
| Sealed class | Types | 1 | 1,504 | 7.53 | 1,402 | src/Llyn.Application/Card/LCardClerk.cs:7 |
| foreach | Statements | 1 | 1,431 | 7.17 | 457 | src/Llyn.Application/Card/LCardClerk.cs:59 |
| Tuple literal or tuple type | Expressions | 7 | 1,389 | 6.96 | 261 | src/Llyn.Application/Card/LMeaningClerk.cs:26 |
| Class | Types | 1 | 1,353 | 6.78 | 1,336 | src/Llyn.Application/Card/LCardClerk.cs:7 |
| Conditional ?: | Expressions | 1 | 1,317 | 6.59 | 506 | src/Llyn.Application/Card/LCardClerk.cs:173 |
| Full property | Members | 1 | 1,120 | 5.61 | 343 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Static lambda | Expressions | 9 | 1,109 | 5.55 | 282 | src/Llyn.Application/Card/LCardEquality.cs:113 |
| Null-conditional ?. ?[ | Expressions | 6 | 1,053 | 5.27 | 372 | src/Llyn.Application/Card/LMentionClerk.cs:47 |
| Constant pattern | Patterns | 7 | 1,029 | 5.15 | 186 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Null-coalescing ?? | Expressions | 2 | 913 | 4.57 | 375 | src/Llyn.Application/Card/LCardClerkField.cs:74 |
| not pattern | Patterns | 9 | 872 | 4.37 | 358 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| Auto property | Members | 3 | 853 | 4.27 | 251 | src/Llyn.Application/Citation/LCitationClerk.cs:41 |
| Null-forgiving ! | Expressions | 8 | 789 | 3.95 | 302 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:145 |
| is null | Patterns | 7 | 770 | 3.86 | 338 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| Interpolated string | Expressions | 6 | 682 | 3.42 | 162 | src/Llyn.Application/Localization/LLocalizationReader.cs:55 |
| with expression | Expressions | 9 | 681 | 3.41 | 233 | src/Llyn.Application/Card/LCardClerk.cs:109 |
| Extension method | Members | 3 | 615 | 3.08 | 41 | tests/Llyn.Tests.Interface/TInterfaceArea.cs:7 |
| await | Expressions | 5 | 612 | 3.06 | 149 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:139 |
| Raw string | Expressions | 11 | 610 | 3.05 | 140 | src/Llyn.Infrastructure/Database/Base/LDatabase.cs:51 |
| Const field | Members | 1 | 560 | 2.80 | 237 | src/Llyn.Application/Card/LMentionClerk.cs:11 |
| Constructor | Members | 1 | 558 | 2.79 | 553 | src/Llyn.Application/Card/LCardClerk.cs:19 |
| Nullable value type T? | Nullability | 2 | 548 | 2.74 | 241 | src/Llyn.Application/Card/LMeaningClerk.cs:92 |
| Record class | Types | 9 | 520 | 2.60 | 431 | src/Llyn.Application/Ensign/LEnsignRow.cs:3 |
| Property pattern | Patterns | 8 | 469 | 2.35 | 118 | src/Llyn.Application/Draft/Clerk/LDraftClerk.cs:82 |
| Cast expression | Expressions | 1 | 465 | 2.33 | 158 | src/Llyn.Application/Markup/LMarkupClerkLink.cs:234 |
| Object initializer | Expressions | 3 | 457 | 2.29 | 194 | src/Llyn.Application/Portrait/LPortraitClerkLabel.cs:25 |
| Async method | Async | 5 | 450 | 2.25 | 149 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:91 |
| Optional parameter | Members | 4 | 444 | 2.22 | 150 | src/Llyn.Application/Draft/Clerk/LDraftClerkPanel.cs:170 |
| nameof | Expressions | 6 | 377 | 1.89 | 133 | src/Llyn.Application/Citation/LAuthorClerk.cs:361 |
| catch | Statements | 1 | 360 | 1.80 | 160 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:142 |
| lock | Statements | 1 | 344 | 1.72 | 57 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:98 |
| and / or pattern | Patterns | 9 | 335 | 1.68 | 102 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Static class | Types | 2 | 329 | 1.65 | 329 | src/Llyn.Application/Card/LCardClerkField.cs:7 |
| Deconstruction | Expressions | 7 | 324 | 1.62 | 138 | src/Llyn.Application/Card/LMeaningClerk.cs:46 |
| try | Statements | 1 | 318 | 1.59 | 171 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:137 |
| Named argument | Members | 4 | 287 | 1.44 | 90 | src/Llyn.Application/Card/LTranslationClerk.cs:164 |
| for | Statements | 1 | 260 | 1.30 | 159 | src/Llyn.Application/Card/LCardClerk.cs:247 |
| Init accessor | Members | 9 | 221 | 1.11 | 76 | src/Llyn.Conduct/Lexicon/CExample.cs:20 |
| typeof | Expressions | 1 | 204 | 1.02 | 45 | src/Llyn.Infrastructure/Configuration/LLocalizationLoader.cs:20 |
| while | Statements | 1 | 192 | 0.96 | 115 | src/Llyn.Application/Card/LMeaningClerk.cs:44 |
| Discard pattern | Patterns | 8 | 185 | 0.93 | 112 | src/Llyn.Application/Citation/LAuthorClerk.cs:243 |
| Spread element | Expressions | 12 | 180 | 0.90 | 98 | src/Llyn.Application/Card/LCardEquality.cs:53 |
| Switch expression | Patterns | 8 | 180 | 0.90 | 112 | src/Llyn.Application/Citation/LAuthorClerk.cs:239 |
| throw expression | Expressions | 7 | 173 | 0.87 | 94 | src/Llyn.Application/Card/LCardClerkField.cs:75 |
| using statement | Statements | 1 | 167 | 0.84 | 69 | src/Llyn.Application/Citation/LAuthorCitation.cs:51 |
| Field-like event | Members | 1 | 153 | 0.77 | 91 | src/Llyn.Conduct/Atelier/CNavigation.cs:46 |
| Discard _ | Expressions | 7 | 149 | 0.75 | 56 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:108 |
| Case guard when | Patterns | 7 | 115 | 0.58 | 24 | src/Llyn.Application/Citation/LAuthorClerk.cs:241 |
| Interface | Types | 1 | 99 | 0.50 | 99 | src/Llyn.Application/Pronunciation/Relay/LListener.cs:5 |
| Relational pattern | Patterns | 9 | 88 | 0.44 | 29 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Verbatim string | Expressions | 1 | 88 | 0.44 | 19 | src/Llyn.Application/Localization/LLocalizationReader.cs:13 |
| Range .. | Expressions | 8 | 87 | 0.44 | 44 | src/Llyn.Application/Card/LMentionClerk.cs:173 |
| as cast | Expressions | 1 | 86 | 0.43 | 45 | src/Llyn.Infrastructure/Database/Base/LWorkspaceArchive.cs:132 |
| Index from end ^ | Expressions | 8 | 81 | 0.41 | 46 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:230 |
| Exception filter | Statements | 6 | 75 | 0.38 | 49 | src/Llyn.Application/Outpost/LCourierClerk.cs:198 |
| Partial type | Types | 2 | 72 | 0.36 | 72 | src/Llyn.UIVeneer/Display/View/PDisplay.xaml.cs:5 |
| is type test | Patterns | 1 | 59 | 0.30 | 36 | src/Llyn.Application/Outpost/LCourierClerk.cs:290 |
| Switch statement | Patterns | 1 | 57 | 0.29 | 34 | src/Llyn.Conduct/Configuration/CLedger.cs:262 |
| Generic method | Generics | 2 | 54 | 0.27 | 29 | src/Llyn.Application/Card/LCardClerkField.cs:183 |
| yield return / break | Statements | 2 | 50 | 0.25 | 20 | src/Llyn.Application/Card/LCardClerkField.cs:35 |
| Local const | Statements | 1 | 37 | 0.19 | 22 | src/Llyn.Conduct/Lexicon/CStateWording.cs:10 |
| finally | Statements | 1 | 35 | 0.18 | 26 | src/Llyn.Application/Outpost/LCourierClerk.cs:78 |
| Enum | Types | 1 | 33 | 0.17 | 33 | src/Llyn.Conduct/Catalog/CCatalogOrder.cs:3 |
| var pattern | Patterns | 7 | 30 | 0.15 | 12 | src/Llyn.Conduct/Card/CFolio.cs:200 |
| ref or out parameter | Members | 1 | 28 | 0.14 | 22 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:149 |
| Nested type | Types | 1 | 26 | 0.13 | 15 | src/Llyn.UIDeportment/Kit/Bind/QLook.cs:19 |
| Collection initializer | Expressions | 3 | 19 | 0.10 | 15 | src/Llyn.Application/Outpost/LCourierClerk.cs:179 |
| Null-coalescing assignment ??= | Expressions | 8 | 19 | 0.10 | 17 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:160 |
| Constraint clause | Generics | 2 | 18 | 0.09 | 10 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:171 |
| Local function | Members | 7 | 17 | 0.09 | 11 | src/Llyn.Conduct/Desk/CErrand.cs:55 |
| List pattern | Patterns | 11 | 14 | 0.07 | 12 | src/Llyn.ShellEngine/Engine/LDraftFacade.cs:70 |
| Conversion operator | Members | 1 | 13 | 0.07 | 2 | src/Llyn.Core/Lexicon/State/LStateValue.cs:49 |
| Type pattern | Patterns | 9 | 11 | 0.06 | 4 | src/Llyn.UIDeportment/Sound/Accent/QAccentControl.cs:62 |
| Default interface method body | Members | 8 | 9 | 0.05 | 4 | src/Llyn.ShellEngine/Port/LDraftPort.cs:58 |
| Positional pattern | Patterns | 8 | 9 | 0.05 | 9 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:37 |
| Async lambda | Async | 5 | 7 | 0.04 | 4 | src/Llyn.Application/Outpost/LCourierClerk.cs:253 |
| default literal | Expressions | 7.1 | 7 | 0.04 | 5 | src/Llyn.Infrastructure/Pronunciation/Loader/LSourceLoader.cs:73 |
| Required member | Members | 11 | 5 | 0.03 | 3 | src/Llyn.UIDeportment/Panel/Taxonomy/QMembershipItem.cs:27 |
| Primary constructor on class or struct | Types | 12 | 4 | 0.02 | 4 | tests/Llyn.Tests.Convention/TAuditFakeMember.cs:3 |
| Event accessors | Members | 1 | 3 | 0.02 | 2 | src/Llyn.UIDeportment/Display/View/PGrasp.cs:81 |
| Generic type | Types | 2 | 3 | 0.02 | 3 | src/Llyn.Conduct/Configuration/CEnsignSheet.cs:5 |
| Record struct | Types | 10 | 3 | 0.02 | 3 | src/Llyn.Core/Lexicon/Inflection/LInflectionLetter.cs:3 |
| Abstract class | Types | 1 | 2 | 0.01 | 2 | src/Llyn.Application/Request/Entry/LRequest.cs:3 |
| checked / unchecked expression | Expressions | 1 | 2 | 0.01 | 2 | src/Llyn.Core.Windows/LPressBrowser.cs:14 |
| Static constructor | Members | 1 | 2 | 0.01 | 2 | src/Llyn.UIDeportment/Kit/Bind/QLookItem.cs:23 |
| Static local function | Members | 8 | 2 | 0.01 | 1 | src/Llyn.UIDeportment/Kit/Asset/QIcon.cs:144 |
| Indexer | Members | 1 | 1 | 0.01 | 1 | src/Llyn.UIDeportment/Kit/Asset/QLocalizationCatalog.cs:19 |
| Using alias | Namespaces | 1 | 1 | 0.01 | 1 | src/Llyn.Host/LHost.cs:11 |

## Styles

| Style | A | A count | B | B count | A share |
|---|---|---:|---|---:|---:|
| Null check | is null / is not null | 770 | == null / != null | 0 | 100.00 % |
| Object creation | Target-typed new() | 2,156 | new T() | 3,841 | 35.95 % |
| Collection creation | Collection expression | 6,739 | Collection initializer or array creation | 62 | 99.09 % |
| Member body | Expression body | 2,529 | Block body | 9,884 | 20.37 % |
| Local type | var | 42 | Explicit type | 20,660 | 0.20 % |
| Using | Declaration | 5,334 | Statement | 167 | 96.96 % |
| Namespace | File-scoped | 1,898 | Block | 0 | 100.00 % |
| Text building | Interpolated string | 682 | string.Format or + with a string literal | 439 | 60.84 % |
| Branch | Switch expression | 180 | Switch statement | 57 | 75.95 % |
| Lambda body | Expression | 3,843 | Block | 466 | 89.19 % |
| Type test | is pattern with designation | 1,057 | as | 86 | 92.48 % |
| Constructor | Primary | 4 | Explicit | 553 | 0.72 % |

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
| IdentifierName | 306,474 | 1,899 |
| Argument | 110,459 | 1,445 |
| SimpleMemberAccessExpression | 101,098 | 1,428 |
| ArgumentList | 68,356 | 1,480 |
| InvocationExpression | 62,518 | 1,438 |
| ExpressionStatement | 29,955 | 1,323 |
| PredefinedType | 29,952 | 1,769 |
| StringLiteralExpression | 24,736 | 1,060 |
| Parameter | 21,768 | 1,718 |
| VariableDeclaration | 21,393 | 1,248 |
| VariableDeclarator | 21,393 | 1,248 |
| EqualsValueClause | 20,330 | 1,302 |
| LocalDeclarationStatement | 17,944 | 1,072 |
| Block | 17,525 | 1,397 |
| ParameterList | 13,258 | 1,827 |
| MethodDeclaration | 11,151 | 1,465 |
| NumericLiteralExpression | 10,942 | 1,038 |
| GenericName | 10,213 | 1,309 |
| TypeArgumentList | 10,213 | 1,309 |
| QualifiedName | 8,961 | 1,899 |
| ExpressionElement | 7,417 | 462 |
| SimpleAssignmentExpression | 7,167 | 982 |
| CollectionExpression | 6,739 | 858 |
| UsingDirective | 6,052 | 1,635 |
| ReturnStatement | 5,710 | 1,031 |
| NullLiteralExpression | 4,023 | 861 |
| IfStatement | 3,962 | 662 |
| NullableType | 3,896 | 967 |
| ObjectCreationExpression | 3,841 | 854 |
| Attribute | 3,417 | 410 |
| AttributeList | 3,417 | 410 |
| SimpleLambdaExpression | 3,318 | 612 |
| SingleVariableDesignation | 3,173 | 548 |
| BracketedArgumentList | 3,001 | 526 |
| FieldDeclaration | 2,874 | 742 |
| IsPatternExpression | 2,624 | 554 |
| ArrowExpressionClause | 2,531 | 488 |
| ElementAccessExpression | 2,410 | 475 |
| ImplicitObjectCreationExpression | 2,156 | 526 |
| FalseLiteralExpression | 2,019 | 543 |
| PropertyDeclaration | 1,978 | 492 |
| CompilationUnit | 1,899 | 1,899 |
| FileScopedNamespaceDeclaration | 1,898 | 1,898 |
| DeclarationPattern | 1,862 | 369 |
| ConstantPattern | 1,799 | 427 |
| TrueLiteralExpression | 1,709 | 489 |
| EqualsExpression | 1,520 | 554 |
| InterpolatedStringText | 1,515 | 162 |
| AddExpression | 1,394 | 335 |
| TupleExpression | 1,383 | 262 |
| ClassDeclaration | 1,353 | 1,336 |
| ForEachStatement | 1,349 | 441 |
| LogicalAndExpression | 1,338 | 381 |
| Interpolation | 1,337 | 161 |
| ConditionalExpression | 1,317 | 506 |
| AttributeArgument | 1,195 | 59 |
| LogicalNotExpression | 1,161 | 393 |
| AddAssignmentExpression | 1,116 | 268 |
| ConditionalAccessExpression | 1,053 | 372 |
| MemberBindingExpression | 1,053 | 372 |
| DeclarationExpression | 1,024 | 265 |
| ParenthesizedLambdaExpression | 991 | 282 |
| AccessorList | 959 | 274 |
| SwitchExpressionArm | 958 | 112 |
| GetAccessorDeclaration | 955 | 273 |
| LogicalOrExpression | 948 | 298 |
| CoalesceExpression | 913 | 375 |
| NotPattern | 872 | 358 |
| SuppressNullableWarningExpression | 789 | 302 |
| TupleElement | 782 | 190 |
| InterpolatedStringExpression | 682 | 162 |
| WithExpression | 681 | 233 |
| WithInitializerExpression | 681 | 233 |
| GreaterThanExpression | 676 | 292 |
| NameColon | 660 | 194 |
| ArrayRankSpecifier | 637 | 250 |
| ArrayType | 634 | 250 |
| OmittedArraySizeExpression | 621 | 250 |
| AwaitExpression | 612 | 149 |
| ImplicitElementAccess | 591 | 114 |
| ConstructorDeclaration | 560 | 555 |
| PostIncrementExpression | 522 | 232 |
| RecordDeclaration | 520 | 431 |
| CharacterLiteralExpression | 504 | 118 |
| RecursivePattern | 478 | 125 |
| LessThanExpression | 475 | 211 |
| AttributeArgumentList | 471 | 59 |
| PropertyPatternClause | 469 | 118 |
| CastExpression | 465 | 158 |
| ObjectInitializerExpression | 457 | 194 |
| NotEqualsExpression | 446 | 181 |
| Subpattern | 422 | 113 |
| ParenthesizedExpression | 372 | 180 |
| BaseList | 364 | 270 |
| CatchClause | 360 | 160 |
| CatchDeclaration | 357 | 158 |
| LockStatement | 344 | 57 |
| TupleType | 331 | 190 |
| ContinueStatement | 321 | 152 |
| TryStatement | 318 | 171 |
| SwitchSection | 309 | 34 |
| BreakStatement | 297 | 60 |
| OrPattern | 293 | 95 |
| SimpleBaseType | 272 | 251 |
| ForStatement | 260 | 159 |
| EnumMemberDeclaration | 224 | 33 |
| InitAccessorDeclaration | 221 | 76 |
| TypeOfExpression | 204 | 45 |
| SubtractExpression | 200 | 91 |
| ThrowStatement | 198 | 101 |
| WhileStatement | 192 | 115 |
| DiscardPattern | 185 | 112 |
| SpreadElement | 180 | 98 |
| SwitchExpression | 180 | 112 |
| ThrowExpression | 173 | 94 |
| CaseSwitchLabel | 168 | 22 |
| UsingStatement | 167 | 69 |
| UnaryMinusExpression | 163 | 73 |
| SubtractAssignmentExpression | 154 | 66 |
| EventFieldDeclaration | 153 | 91 |
| ElseClause | 151 | 118 |
| SetAccessorDeclaration | 131 | 67 |
| LessThanOrEqualExpression | 118 | 84 |
| ThisExpression | 117 | 78 |
| WhenClause | 115 | 24 |
| CasePatternSwitchLabel | 106 | 20 |
| GreaterThanOrEqualExpression | 104 | 68 |
| PrimaryConstructorBaseType | 103 | 19 |
| InterfaceDeclaration | 99 | 99 |
| RelationalPattern | 88 | 29 |
| RangeExpression | 87 | 44 |
| AsExpression | 86 | 45 |
| ForEachVariableStatement | 82 | 59 |
| IndexExpression | 81 | 46 |
| MultiplyExpression | 81 | 32 |
| CatchFilterClause | 75 | 49 |
| ArrayInitializerExpression | 62 | 31 |
| TypeParameter | 60 | 32 |
| IsExpression | 59 | 36 |
| SwitchStatement | 57 | 34 |
| TypeParameterList | 57 | 32 |
| BitwiseOrExpression | 48 | 25 |
| YieldReturnStatement | 43 | 20 |
| AndPattern | 42 | 18 |
| DefaultSwitchLabel | 41 | 22 |
| ImplicitArrayCreationExpression | 41 | 29 |
| ParenthesizedPattern | 41 | 19 |
| FinallyClause | 35 | 26 |
| OrAssignmentExpression | 35 | 20 |
| EnumDeclaration | 33 | 33 |
| ExpressionColon | 33 | 14 |
| LeftShiftExpression | 32 | 2 |
| DivideExpression | 31 | 19 |
| VarPattern | 30 | 12 |
| ArrayCreationExpression | 27 | 20 |
| CoalesceAssignmentExpression | 19 | 17 |
| CollectionInitializerExpression | 19 | 15 |
| TypeParameterConstraintClause | 18 | 10 |
| ClassConstraint | 17 | 9 |
| LocalFunctionStatement | 17 | 11 |
| AttributeTargetSpecifier | 15 | 7 |
| ListPattern | 14 | 12 |
| BaseExpression | 13 | 5 |
| ConversionOperatorDeclaration | 13 | 2 |
| TypePattern | 11 | 4 |
| PostDecrementExpression | 10 | 8 |
| PositionalPatternClause | 9 | 9 |
| DefaultLiteralExpression | 7 | 5 |
| InterpolationFormatClause | 7 | 1 |
| YieldBreakStatement | 7 | 6 |
| BaseConstructorInitializer | 6 | 6 |
| ExplicitInterfaceSpecifier | 6 | 5 |
| GlobalStatement | 6 | 1 |
| NameEquals | 6 | 4 |
| AddAccessorDeclaration | 3 | 2 |
| AndAssignmentExpression | 3 | 3 |
| EventDeclaration | 3 | 2 |
| InterpolationAlignmentClause | 3 | 1 |
| PreIncrementExpression | 3 | 3 |
| RecordStructDeclaration | 3 | 3 |
| RemoveAccessorDeclaration | 3 | 2 |
| SlicePattern | 3 | 3 |
| BitwiseAndExpression | 2 | 2 |
| ComplexElementInitializerExpression | 2 | 1 |
| ModuloExpression | 2 | 1 |
| UncheckedExpression | 2 | 2 |
| BracketedParameterList | 1 | 1 |
| IndexerDeclaration | 1 | 1 |
| OmittedTypeArgument | 1 | 1 |
| ThisConstructorInitializer | 1 | 1 |
| TypeConstraint | 1 | 1 |

## Attributes

| Attribute | Count | Files |
|---|---:|---:|
| Fact | 2,855 | 396 |
| InlineData | 449 | 48 |
| Theory | 90 | 50 |
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

| Feature | Llyn.Tests.Engine | Llyn.UIDeportment | Llyn.Tests.Conduct | Llyn.Infrastructure | Llyn.Application | Llyn.Tests.Convention | Llyn.Conduct | Llyn.Core | Llyn.ShellEngine | Llyn.Tests.Interface | Llyn.Tests.Windows | Llyn.Core.Windows | Llyn.UIVeneer | Llyn.UIDeportment.Capsule | Llyn.Host |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| Collection expression | 2,861 | 151 | 1,683 | 293 | 337 | 523 | 160 | 283 | 78 | 227 | 143 |  |  |  |  |
| using declaration | 2,079 | 5 | 2,557 | 594 | 12 | 7 |  |  |  | 18 | 62 |  |  |  |  |
| Lambda | 865 | 192 | 1,282 | 30 | 346 | 730 | 345 | 185 | 68 | 179 | 74 | 4 |  |  | 9 |
| if | 54 | 1,216 | 5 | 668 | 698 | 464 | 356 | 246 | 207 | 31 | 4 | 10 |  | 3 |  |
| Nullable reference type T? | 166 | 465 | 353 | 341 | 346 | 297 | 318 | 349 | 244 | 416 | 42 | 7 |  | 4 |  |
| Expression-bodied member | 40 | 652 | 37 | 16 | 110 | 27 | 191 | 168 | 122 | 1,130 | 36 |  |  |  |  |
| Target-typed new | 183 | 745 | 225 | 143 | 153 | 190 | 36 | 51 | 86 | 257 | 81 | 1 |  | 1 | 4 |
| Readonly field | 40 | 609 | 4 | 120 | 399 | 216 | 325 | 30 | 173 | 35 | 3 | 4 |  | 1 |  |
| File-scoped namespace | 213 | 301 | 143 | 171 | 122 | 115 | 247 | 341 | 90 | 88 | 24 | 4 | 35 | 4 |  |
| Declaration pattern | 7 | 915 | 4 | 43 | 197 | 368 | 216 | 26 | 74 | 9 | 3 |  |  |  |  |
| Sealed class | 218 | 258 | 143 | 76 | 183 | 43 | 228 | 230 | 63 | 33 | 21 | 4 |  | 4 |  |
| foreach | 55 | 192 | 18 | 287 | 319 | 306 | 25 | 155 | 32 | 40 | 2 |  |  |  |  |
| Tuple literal or tuple type | 425 | 43 | 277 | 170 | 82 | 168 | 56 | 46 | 73 | 27 | 22 |  |  |  |  |
| Class | 221 | 286 | 142 | 169 | 101 | 108 | 84 | 25 | 57 | 96 | 24 | 4 | 35 | 1 |  |
| Conditional ?: | 14 | 206 | 18 | 291 | 254 | 156 | 145 | 139 | 61 | 28 | 3 | 2 |  |  |  |
| Full property | 7 | 596 | 5 | 6 | 110 | 2 | 177 | 150 | 39 | 28 |  |  |  |  |  |
| Static lambda | 98 | 81 | 541 | 6 | 118 | 15 | 65 | 99 | 16 | 63 | 6 |  |  |  | 1 |
| Null-conditional ?. ?[ | 133 | 178 | 83 | 61 | 49 | 91 | 323 | 41 | 77 | 8 | 8 | 1 |  |  |  |
| Constant pattern | 6 | 133 | 8 | 144 | 54 | 353 | 123 | 143 | 43 | 17 |  | 5 |  |  |  |
| Null-coalescing ?? | 19 | 72 | 12 | 117 | 191 | 80 | 49 | 261 | 41 | 60 | 10 |  |  | 1 |  |
| not pattern | 4 | 318 | 1 | 55 | 73 | 185 | 144 | 34 | 51 | 6 | 1 |  |  |  |  |
| Auto property | 5 | 372 | 3 | 8 | 7 | 23 | 129 | 233 | 51 | 22 |  |  |  |  |  |
| Null-forgiving ! | 229 | 147 | 220 | 62 | 15 | 25 | 9 |  | 3 | 62 | 17 |  |  |  |  |
| is null | 8 | 126 | 5 | 96 | 164 | 175 | 84 | 42 | 55 | 13 | 2 |  |  |  |  |
| Interpolated string | 54 | 19 | 7 | 161 | 51 | 380 |  | 7 |  | 3 |  |  |  |  |  |
| with expression | 231 | 7 | 33 | 43 | 251 | 10 | 22 | 18 | 37 | 29 |  |  |  |  |  |
| Extension method |  |  |  |  |  |  |  |  |  | 608 | 7 |  |  |  |  |
| await | 241 | 57 | 147 | 62 | 57 | 1 | 13 |  | 17 | 6 | 1 | 10 |  |  |  |
| Raw string | 139 | 2 | 7 | 221 |  | 233 |  |  |  | 8 |  |  |  |  |  |
| Const field | 76 | 72 | 14 | 179 | 12 | 113 | 3 | 70 | 4 | 12 |  | 4 |  | 1 |  |
| Constructor | 2 | 223 | 1 | 68 | 79 | 7 | 74 | 3 | 56 | 8 |  | 2 | 35 |  |  |
| Nullable value type T? | 24 | 55 | 20 | 42 | 36 | 11 | 119 | 34 | 113 | 70 | 22 |  |  | 2 |  |
| Record class |  | 17 | 2 | 2 | 104 | 6 | 151 | 227 | 6 | 2 |  |  |  | 3 |  |
| Property pattern | 2 | 152 |  | 10 | 17 | 270 | 9 | 2 | 4 | 1 | 2 |  |  |  |  |
| Cast expression | 7 | 139 | 78 | 86 | 4 | 50 | 3 | 4 |  | 41 | 52 | 1 |  |  |  |
| Object initializer | 57 | 127 | 93 | 29 | 2 | 60 |  | 3 |  | 34 | 50 | 1 |  | 1 |  |
| Async method | 191 | 56 | 102 | 32 | 30 | 1 | 13 |  | 13 | 6 | 1 | 5 |  |  |  |
| Optional parameter | 12 | 8 | 6 | 6 | 11 | 1 | 10 | 265 | 12 | 108 | 2 |  |  | 3 |  |
| nameof | 1 | 202 | 3 | 9 | 117 | 17 | 17 | 4 | 4 | 2 | 1 |  |  |  |  |
| catch | 1 | 21 |  | 128 | 33 | 8 | 118 | 5 | 27 | 1 | 9 | 6 |  | 3 |  |
| lock |  | 13 |  | 2 | 77 | 5 | 2 |  | 240 | 5 |  |  |  |  |  |
| and / or pattern | 3 | 37 | 3 | 70 | 15 | 159 | 2 | 39 | 1 | 6 |  |  |  |  |  |
| Static class | 3 | 44 | 1 | 95 | 21 | 71 | 7 | 22 |  | 62 | 3 |  |  |  |  |
| Deconstruction | 67 | 29 | 113 | 26 | 27 | 33 | 19 | 5 | 5 |  |  |  |  |  |  |
| try | 7 | 19 |  | 80 | 30 | 10 | 120 | 5 | 28 | 1 | 12 | 5 |  | 1 |  |
| Named argument | 138 |  | 19 | 16 | 23 | 16 |  | 18 | 3 | 54 |  |  |  |  |  |
| for | 13 | 46 | 6 | 62 | 58 | 27 | 6 | 17 | 7 | 16 | 2 |  |  |  |  |
| Init accessor |  | 2 |  |  |  | 3 | 1 | 215 |  |  |  |  |  |  |  |
| typeof |  | 163 | 5 | 3 |  | 3 |  |  |  | 4 | 26 |  |  |  |  |
| while | 18 | 9 | 5 | 98 | 11 | 25 | 2 | 18 | 1 | 5 |  |  |  |  |  |
| Discard pattern |  | 14 | 1 | 16 | 27 | 67 | 19 | 29 | 9 | 1 |  | 2 |  |  |  |
| Spread element | 29 | 9 | 9 | 4 | 36 | 25 | 6 | 39 | 9 | 13 | 1 |  |  |  |  |
| Switch expression |  | 12 | 1 | 16 | 27 | 65 | 18 | 29 | 9 | 1 |  | 2 |  |  |  |
| throw expression | 4 | 4 | 30 | 7 | 73 | 1 | 15 | 4 | 8 | 26 | 1 |  |  |  |  |
| using statement | 36 |  | 2 | 121 | 7 |  |  |  |  |  | 1 |  |  |  |  |
| Field-like event |  | 59 |  |  |  |  | 92 |  | 2 |  |  |  |  |  |  |
| Discard _ | 24 | 25 | 59 | 3 | 8 | 9 | 2 | 10 | 6 | 1 | 2 |  |  |  |  |
| Case guard when |  | 6 |  | 1 | 3 | 100 |  | 3 |  | 2 |  |  |  |  |  |
| Interface |  | 2 |  |  | 1 |  | 1 | 68 | 27 |  |  |  |  |  |  |
| Relational pattern |  | 7 |  | 28 | 10 | 3 |  | 39 | 1 |  |  |  |  |  |  |
| Verbatim string | 6 |  |  |  | 3 | 77 |  |  |  | 2 |  |  |  |  |  |
| Range .. | 4 | 1 | 16 | 16 | 2 | 30 |  | 18 |  |  |  |  |  |  |  |
| as cast |  | 58 |  | 3 |  | 9 |  |  |  | 15 | 1 |  |  |  |  |
| Index from end ^ | 14 | 3 | 40 | 1 | 4 | 11 |  | 8 |  |  |  |  |  |  |  |
| Exception filter |  | 3 |  | 34 | 16 | 3 |  |  | 19 |  |  |  |  |  |  |
| Partial type |  |  | 4 |  |  | 7 |  |  |  | 26 |  |  | 35 |  |  |
| is type test | 1 | 21 |  | 2 | 3 | 32 |  |  |  |  |  |  |  |  |  |
| Switch statement |  | 5 |  | 8 |  | 20 | 2 | 19 | 1 | 2 |  |  |  |  |  |
| Generic method |  | 22 | 1 |  | 13 | 1 | 4 | 7 | 1 | 5 |  |  |  |  |  |
| yield return / break |  | 9 |  | 7 | 7 | 25 |  | 2 |  |  |  |  |  |  |  |
| Local const | 25 |  | 1 | 4 |  | 6 | 1 |  |  |  |  |  |  |  |  |
| finally | 6 | 1 |  | 2 | 11 | 2 | 4 |  | 4 |  | 3 | 2 |  |  |  |
| Enum |  | 2 |  |  |  |  | 11 | 20 |  |  |  |  |  |  |  |
| var pattern |  |  |  |  |  | 28 | 1 |  |  |  | 1 |  |  |  |  |
| ref or out parameter | 3 | 1 | 1 | 1 | 6 | 3 |  | 5 | 3 | 5 |  |  |  |  |  |
| Nested type | 8 | 7 | 1 |  |  |  |  |  |  | 10 |  |  |  |  |  |
| Collection initializer |  | 3 | 3 | 5 | 2 |  |  |  |  | 2 | 4 |  |  |  |  |
| Null-coalescing assignment ??= |  | 10 |  | 1 | 4 | 3 |  |  |  |  |  | 1 |  |  |  |
| Constraint clause |  | 12 | 1 |  | 1 |  |  |  |  | 4 |  |  |  |  |  |
| Local function | 1 | 4 |  |  |  | 4 | 5 |  |  | 1 | 1 | 1 |  |  |  |
| List pattern | 1 | 4 |  |  |  | 8 |  |  | 1 |  |  |  |  |  |  |
| Conversion operator |  | 12 |  |  |  |  |  | 1 |  |  |  |  |  |  |  |
| Type pattern |  | 1 |  |  |  | 10 |  |  |  |  |  |  |  |  |  |
| Default interface method body |  |  |  |  |  |  |  |  | 9 |  |  |  |  |  |  |
| Positional pattern |  | 2 |  | 1 | 1 | 1 | 4 |  |  |  |  |  |  |  |  |
| Async lambda |  |  | 4 |  | 1 |  |  |  |  |  |  | 2 |  |  |  |
| default literal |  |  | 1 | 1 |  | 5 |  |  |  |  |  |  |  |  |  |
| Required member |  | 2 |  |  |  | 3 |  |  |  |  |  |  |  |  |  |
| Primary constructor on class or struct |  |  |  |  |  | 4 |  |  |  |  |  |  |  |  |  |
| Event accessors |  | 3 |  |  |  |  |  |  |  |  |  |  |  |  |  |
| Generic type |  | 2 |  |  |  |  | 1 |  |  |  |  |  |  |  |  |
| Record struct |  | 1 |  |  |  | 1 |  | 1 |  |  |  |  |  |  |  |
| Abstract class |  | 1 |  |  | 1 |  |  |  |  |  |  |  |  |  |  |
| checked / unchecked expression |  | 1 |  |  |  |  |  |  |  |  |  | 1 |  |  |  |
| Static constructor |  | 1 |  |  |  |  |  |  |  | 1 |  |  |  |  |  |
| Static local function |  | 2 |  |  |  |  |  |  |  |  |  |  |  |  |  |
| Indexer |  | 1 |  |  |  |  |  |  |  |  |  |  |  |  |  |
| Using alias |  |  |  |  |  |  |  |  |  |  |  |  |  |  | 1 |

## Style A share per project

| Style | Llyn.Tests.Engine | Llyn.UIDeportment | Llyn.Tests.Conduct | Llyn.Infrastructure | Llyn.Application | Llyn.Tests.Convention | Llyn.Conduct | Llyn.Core | Llyn.ShellEngine | Llyn.Tests.Interface | Llyn.Tests.Windows | Llyn.Core.Windows | Llyn.UIVeneer | Llyn.UIDeportment.Capsule | Llyn.Host |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| Null check | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - | - | - | - |
| Object creation | 54.95 % | 48.82 % | 36.00 % | 25.00 % | 28.98 % | 42.13 % | 8.63 % | 24.52 % | 30.50 % | 31.89 % | 38.57 % | 7.69 % | - | 14.29 % | 21.05 % |
| Collection creation | 99.51 % | 94.97 % | 99.18 % | 97.99 % | 99.41 % | 99.05 % | 100.00 % | 100.00 % | 100.00 % | 99.56 % | 92.26 % | - | - | - | - |
| Member body | 2.29 % | 25.34 % | 2.82 % | 1.62 % | 10.75 % | 3.82 % | 17.04 % | 31.17 % | 15.80 % | 78.80 % | 25.17 % | 0.00 % | 0.00 % | 0.00 % | - |
| Local type | 0.00 % | 4.28 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.31 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | - | 0.00 % | 0.00 % |
| Using | 98.30 % | 100.00 % | 99.92 % | 83.08 % | 63.16 % | 100.00 % | - | - | - | 100.00 % | 98.41 % | - | - | - | - |
| Namespace | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - |
| Text building | 32.14 % | 52.78 % | 15.91 % | 57.91 % | 76.12 % | 82.07 % | 0.00 % | 29.17 % | - | 27.27 % | - | 0.00 % | - | 0.00 % | - |
| Branch | - | 70.59 % | 100.00 % | 66.67 % | 100.00 % | 76.47 % | 90.00 % | 60.42 % | 90.00 % | 33.33 % | - | 100.00 % | - | - | - |
| Lambda body | 96.53 % | 84.90 % | 79.56 % | 86.67 % | 92.20 % | 99.04 % | 97.10 % | 98.38 % | 91.18 % | 82.68 % | 28.38 % | 50.00 % | - | - | 77.78 % |
| Type test | 100.00 % | 91.32 % | 100.00 % | 91.18 % | 100.00 % | 94.51 % | 100.00 % | 100.00 % | 100.00 % | 37.50 % | 75.00 % | - | - | - | - |
| Constructor | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 36.36 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | - | 0.00 % | 0.00 % | - | - |
