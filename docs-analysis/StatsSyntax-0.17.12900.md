# Syntax statistics - 0.17.12900

- Generated: 2026-10-04 06:52:21 +09:00
- Source roots: src, tests
- Root-level files: none
- Project segments: 1
- Excluded directories: .git, .vs, artifacts, bin, node_modules, obj, packages, publish
- Excluded suffixes: .g.cs, .g.i.cs, .AssemblyInfo.cs, .GlobalUsings.g.cs, .Designer.cs
- Roslyn: 4.14.0-3.25262.10
- Language version: Preview
- Scanned: 1,627 files, 15 projects, 174,438 lines

## Summary

| Measure | Value |
|---|---:|
| Files | 1,627 |
| Lines | 174,438 |
| Nodes | 909,441 |
| Syntax kinds used | 188 |
| Features used | 98 of 138 |
| Minimum C# version | 12 |
| Parse errors | 0 |

## Families

| Family | Features | Used | Count | Per 1k lines |
|---|---:|---:|---:|---:|
| Types | 15 | 12 | 3,478 | 19.94 |
| Namespaces | 6 | 2 | 1,627 | 9.33 |
| Members | 29 | 20 | 8,892 | 50.98 |
| Patterns | 16 | 16 | 5,384 | 30.86 |
| Expressions | 36 | 29 | 20,559 | 117.86 |
| Statements | 20 | 13 | 11,064 | 63.43 |
| Generics | 6 | 2 | 72 | 0.41 |
| Async | 2 | 2 | 365 | 2.09 |
| Nullability | 3 | 2 | 3,378 | 19.37 |
| Unsafe | 2 | 0 | 0 | 0.00 |
| Directives | 3 | 0 | 0 | 0.00 |
| Total | 138 | 98 | 54,819 | 314.26 |

## Projects

| Project | Files | Lines | Nodes | Features used | Newest C# |
|---|---:|---:|---:|---:|---:|
| Llyn.UIDeportment | 281 | 32,351 | 150,362 | 85 | 12 |
| Llyn.Tests.Engine | 188 | 28,988 | 201,583 | 57 | 12 |
| Llyn.Tests.Conduct | 116 | 20,737 | 150,431 | 48 | 12 |
| Llyn.Infrastructure | 142 | 19,068 | 83,988 | 68 | 12 |
| Llyn.Application | 106 | 16,576 | 75,637 | 66 | 12 |
| Llyn.Tests.Convention | 104 | 15,555 | 76,540 | 79 | 12 |
| Llyn.Conduct | 214 | 13,838 | 53,519 | 57 | 12 |
| Llyn.Core | 291 | 10,123 | 40,224 | 58 | 12 |
| Llyn.ShellEngine | 50 | 8,306 | 34,144 | 58 | 12 |
| Llyn.Tests.Interface | 84 | 7,194 | 35,061 | 63 | 12 |
| Llyn.Tests.Windows | 12 | 875 | 5,250 | 34 | 12 |
| Llyn.Core.Windows | 3 | 341 | 1,251 | 26 | 10 |
| Llyn.UIVeneer | 31 | 339 | 672 | 4 | 10 |
| Llyn.UIDeportment.Capsule | 4 | 88 | 380 | 15 | 10 |
| Llyn.Host | 1 | 59 | 399 | 3 | 9 |
| Total | 1,627 | 174,438 | 909,441 | 98 | 12 |

## Versions

| C# version | Features | Used | Count |
|---|---:|---:|---:|
| 1 | 51 | 35 | 14,942 |
| 2 | 10 | 8 | 1,862 |
| 3 | 7 | 5 | 4,916 |
| 4 | 3 | 2 | 625 |
| 5 | 3 | 3 | 836 |
| 6 | 6 | 5 | 4,878 |
| 7 | 11 | 10 | 5,089 |
| 7.1 | 1 | 1 | 6 |
| 7.2 | 1 | 0 | 0 |
| 7.3 | 1 | 0 | 0 |
| 8 | 17 | 12 | 8,991 |
| 9 | 10 | 9 | 4,799 |
| 10 | 3 | 2 | 1,628 |
| 11 | 6 | 3 | 555 |
| 12 | 4 | 3 | 5,692 |
| 13 | 1 | 0 | 0 |
| 14 | 3 | 0 | 0 |

## Features

| Feature | Family | C# | Count | Per 1k lines | Files | First |
|---|---|---|---:|---:|---:|---|
| Collection expression | Expressions | 12 | 5,536 | 31.74 | 729 | src/Llyn.Application/Card/LCardClerk.cs:57 |
| using declaration | Statements | 8 | 4,562 | 26.15 | 356 | src/Llyn.Application/Card/LTranslationClerk.cs:159 |
| if | Statements | 1 | 3,642 | 20.88 | 575 | src/Llyn.Application/Card/LCardClerk.cs:70 |
| Lambda | Expressions | 3 | 3,268 | 18.73 | 572 | src/Llyn.Application/Card/LCardClerk.cs:174 |
| Nullable reference type T? | Nullability | 8 | 2,892 | 16.58 | 806 | src/Llyn.Application/Card/LCardClerkField.cs:91 |
| Expression-bodied member | Members | 6 | 2,720 | 15.59 | 420 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Target-typed new | Expressions | 9 | 1,729 | 9.91 | 443 | src/Llyn.Application/Card/LMentionClerk.cs:155 |
| Declaration pattern | Patterns | 7 | 1,718 | 9.85 | 320 | src/Llyn.Application/Card/LMeaningClerk.cs:93 |
| File-scoped namespace | Namespaces | 10 | 1,626 | 9.32 | 1,626 | src/Llyn.Application/Card/LCardClerk.cs:5 |
| Readonly field | Members | 1 | 1,469 | 8.42 | 473 | src/Llyn.Application/Card/LCardClerk.cs:9 |
| Full property | Members | 1 | 1,412 | 8.09 | 305 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Sealed class | Types | 1 | 1,303 | 7.47 | 1,204 | src/Llyn.Application/Card/LCardClerk.cs:7 |
| foreach | Statements | 1 | 1,293 | 7.41 | 408 | src/Llyn.Application/Card/LCardClerk.cs:59 |
| Class | Types | 1 | 1,166 | 6.68 | 1,150 | src/Llyn.Application/Card/LCardClerk.cs:7 |
| Conditional ?: | Expressions | 1 | 1,147 | 6.58 | 448 | src/Llyn.Application/Card/LCardClerk.cs:173 |
| Null-conditional ?. ?[ | Expressions | 6 | 1,098 | 6.29 | 329 | src/Llyn.Application/Card/LMentionClerk.cs:47 |
| Tuple literal or tuple type | Expressions | 7 | 1,093 | 6.27 | 216 | src/Llyn.Application/Card/LMeaningClerk.cs:26 |
| Constant pattern | Patterns | 7 | 871 | 4.99 | 166 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Null-coalescing ?? | Expressions | 2 | 845 | 4.84 | 350 | src/Llyn.Application/Card/LCardClerkField.cs:74 |
| not pattern | Patterns | 9 | 779 | 4.47 | 315 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| Auto property | Members | 3 | 737 | 4.22 | 227 | src/Llyn.Application/Citation/LCitationClerk.cs:40 |
| is null | Patterns | 7 | 681 | 3.90 | 299 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| Static lambda | Expressions | 9 | 676 | 3.88 | 213 | src/Llyn.Application/Card/LCardEquality.cs:113 |
| Null-forgiving ! | Expressions | 8 | 650 | 3.73 | 249 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:98 |
| Interpolated string | Expressions | 6 | 639 | 3.66 | 147 | src/Llyn.Application/Localization/LLocalizationReader.cs:55 |
| with expression | Expressions | 9 | 573 | 3.28 | 199 | src/Llyn.Application/Card/LCardClerk.cs:109 |
| Extension method | Members | 3 | 565 | 3.24 | 41 | tests/Llyn.Tests.Interface/TInterfaceArea.cs:7 |
| Raw string | Expressions | 11 | 536 | 3.07 | 126 | src/Llyn.Infrastructure/Database/Base/LDatabase.cs:51 |
| Const field | Members | 1 | 491 | 2.81 | 208 | src/Llyn.Application/Card/LMentionClerk.cs:11 |
| Nullable value type T? | Nullability | 2 | 486 | 2.79 | 206 | src/Llyn.Application/Card/LMeaningClerk.cs:92 |
| await | Expressions | 5 | 471 | 2.70 | 124 | src/Llyn.Application/Language/LLanguageClerk.cs:140 |
| Record class | Types | 9 | 467 | 2.68 | 380 | src/Llyn.Application/Ensign/LEnsignRow.cs:3 |
| Constructor | Members | 1 | 434 | 2.49 | 428 | src/Llyn.Application/Card/LCardClerk.cs:19 |
| Property pattern | Patterns | 8 | 415 | 2.38 | 102 | src/Llyn.Application/Draft/Clerk/LDraftClerk.cs:179 |
| Optional parameter | Members | 4 | 399 | 2.29 | 132 | src/Llyn.Application/Draft/Clerk/LDraftClerkPanel.cs:128 |
| nameof | Expressions | 6 | 383 | 2.20 | 121 | src/Llyn.Application/Citation/LAuthorClerk.cs:361 |
| Async method | Async | 5 | 363 | 2.08 | 125 | src/Llyn.Application/Language/LLanguageClerk.cs:128 |
| Cast expression | Expressions | 1 | 356 | 2.04 | 127 | src/Llyn.Application/Markup/LMarkupClerkLink.cs:234 |
| Object initializer | Expressions | 3 | 335 | 1.92 | 149 | src/Llyn.Core.Windows/LUsherShell.cs:37 |
| lock | Statements | 1 | 323 | 1.85 | 49 | src/Llyn.Application/Ensign/LEnsign.cs:30 |
| Deconstruction | Expressions | 7 | 302 | 1.73 | 123 | src/Llyn.Application/Card/LMeaningClerk.cs:46 |
| catch | Statements | 1 | 293 | 1.68 | 116 | src/Llyn.Application/Localization/LLocalization.cs:64 |
| Static class | Types | 2 | 288 | 1.65 | 288 | src/Llyn.Application/Card/LCardClerkField.cs:7 |
| and / or pattern | Patterns | 9 | 280 | 1.61 | 87 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| try | Statements | 1 | 253 | 1.45 | 122 | src/Llyn.Application/Localization/LLocalization.cs:60 |
| for | Statements | 1 | 230 | 1.32 | 142 | src/Llyn.Application/Card/LCardClerk.cs:247 |
| Named argument | Members | 4 | 226 | 1.30 | 74 | src/Llyn.Application/Card/LTranslationClerk.cs:163 |
| Init accessor | Members | 9 | 215 | 1.23 | 72 | src/Llyn.Core/Catalog/Kind/LCatalogAuthor.cs:13 |
| typeof | Expressions | 1 | 175 | 1.00 | 39 | src/Llyn.Infrastructure/Configuration/LLocalizationLoader.cs:20 |
| while | Statements | 1 | 170 | 0.97 | 102 | src/Llyn.Application/Card/LMeaningClerk.cs:44 |
| using statement | Statements | 1 | 157 | 0.90 | 65 | src/Llyn.Application/Citation/LAuthorCitation.cs:48 |
| Spread element | Expressions | 12 | 152 | 0.87 | 75 | src/Llyn.Application/Card/LCardEquality.cs:53 |
| Discard pattern | Patterns | 8 | 149 | 0.85 | 89 | src/Llyn.Application/Citation/LAuthorClerk.cs:243 |
| throw expression | Expressions | 7 | 145 | 0.83 | 78 | src/Llyn.Application/Card/LCardClerkField.cs:75 |
| Switch expression | Patterns | 8 | 143 | 0.82 | 88 | src/Llyn.Application/Citation/LAuthorClerk.cs:239 |
| Field-like event | Members | 1 | 142 | 0.81 | 75 | src/Llyn.Conduct/Atelier/CNavigation.cs:46 |
| Discard _ | Expressions | 7 | 130 | 0.75 | 45 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:61 |
| Partial type | Types | 2 | 121 | 0.69 | 121 | src/Llyn.Infrastructure/Database/Context/LSituationArchive.cs:8 |
| Case guard when | Patterns | 7 | 106 | 0.61 | 21 | src/Llyn.Application/Citation/LAuthorClerk.cs:241 |
| Verbatim string | Expressions | 1 | 87 | 0.50 | 18 | src/Llyn.Application/Localization/LLocalizationReader.cs:13 |
| Interface | Types | 1 | 72 | 0.41 | 72 | src/Llyn.Application/Pronunciation/Relay/LListener.cs:5 |
| Range .. | Expressions | 8 | 72 | 0.41 | 36 | src/Llyn.Application/Card/LMentionClerk.cs:161 |
| Relational pattern | Patterns | 9 | 69 | 0.40 | 26 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| as cast | Expressions | 1 | 64 | 0.37 | 42 | src/Llyn.Infrastructure/Database/Entry/LCollocationArchive.cs:163 |
| Index from end ^ | Expressions | 8 | 60 | 0.34 | 37 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:230 |
| Switch statement | Patterns | 1 | 58 | 0.33 | 34 | src/Llyn.Conduct/Configuration/CLedger.cs:175 |
| Generic method | Generics | 2 | 54 | 0.31 | 25 | src/Llyn.Application/Card/LCardClerkField.cs:183 |
| is type test | Patterns | 1 | 52 | 0.30 | 31 | src/Llyn.UIDeportment/Editor/Context/PContextSelector.cs:14 |
| yield return / break | Statements | 2 | 48 | 0.28 | 18 | src/Llyn.Application/Card/LCardClerkField.cs:35 |
| Exception filter | Statements | 6 | 38 | 0.22 | 28 | src/Llyn.Application/Pronunciation/Clerk/LReflexClerkEpithet.cs:75 |
| Local const | Statements | 1 | 32 | 0.18 | 18 | src/Llyn.Conduct/Lexicon/CStateWording.cs:10 |
| var pattern | Patterns | 7 | 29 | 0.17 | 12 | src/Llyn.Conduct/Card/CFolio.cs:141 |
| Enum | Types | 1 | 27 | 0.15 | 27 | src/Llyn.Conduct/Catalog/CCatalogOrder.cs:3 |
| Nested type | Types | 1 | 24 | 0.14 | 14 | src/Llyn.UIDeportment/Kit/Bind/QLook.cs:19 |
| finally | Statements | 1 | 23 | 0.13 | 19 | src/Llyn.Application/Pronunciation/Clerk/LFanqieClerk.cs:344 |
| ref or out parameter | Members | 1 | 23 | 0.13 | 19 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:102 |
| Default interface method body | Members | 8 | 19 | 0.11 | 6 | src/Llyn.ShellEngine/Port/LDraftPort.cs:58 |
| Constraint clause | Generics | 2 | 18 | 0.10 | 8 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:171 |
| Null-coalescing assignment ??= | Expressions | 8 | 18 | 0.10 | 16 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:160 |
| List pattern | Patterns | 11 | 14 | 0.08 | 12 | src/Llyn.ShellEngine/Engine/LDraftFacade.cs:65 |
| Local function | Members | 7 | 14 | 0.08 | 8 | src/Llyn.Conduct/Desk/CErrand.cs:53 |
| Conversion operator | Members | 1 | 13 | 0.07 | 2 | src/Llyn.Core/Lexicon/State/LStateValue.cs:49 |
| Collection initializer | Expressions | 3 | 11 | 0.06 | 9 | src/Llyn.Infrastructure/Database/Source/LAuthorArchive.cs:117 |
| Type pattern | Patterns | 9 | 11 | 0.06 | 4 | src/Llyn.UIDeportment/Sound/Accent/QAccentControl.cs:62 |
| Positional pattern | Patterns | 8 | 9 | 0.05 | 9 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:33 |
| default literal | Expressions | 7.1 | 6 | 0.03 | 4 | src/Llyn.Infrastructure/Pronunciation/Loader/LSourceLoader.cs:73 |
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
| Null check | is null / is not null | 681 | == null / != null | 0 | 100.00 % |
| Object creation | Target-typed new() | 1,729 | new T() | 3,194 | 35.12 % |
| Collection creation | Collection expression | 5,536 | Collection initializer or array creation | 40 | 99.28 % |
| Member body | Expression body | 2,720 | Block body | 9,010 | 23.19 % |
| Local type | var | 42 | Explicit type | 17,377 | 0.24 % |
| Using | Declaration | 4,562 | Statement | 157 | 96.67 % |
| Namespace | File-scoped | 1,626 | Block | 0 | 100.00 % |
| Text building | Interpolated string | 639 | string.Format or + with a string literal | 313 | 67.12 % |
| Branch | Switch expression | 143 | Switch statement | 58 | 71.14 % |
| Lambda body | Expression | 2,967 | Block | 301 | 90.79 % |
| Type test | is pattern with designation | 962 | as | 64 | 93.76 % |
| Constructor | Primary | 4 | Explicit | 428 | 0.93 % |

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
| IdentifierName | 263,791 | 1,627 |
| Argument | 93,808 | 1,238 |
| SimpleMemberAccessExpression | 86,175 | 1,233 |
| ArgumentList | 58,850 | 1,275 |
| InvocationExpression | 53,999 | 1,244 |
| PredefinedType | 26,649 | 1,532 |
| ExpressionStatement | 26,330 | 1,130 |
| StringLiteralExpression | 20,397 | 902 |
| Parameter | 18,972 | 1,468 |
| VariableDeclaration | 17,849 | 1,061 |
| VariableDeclarator | 17,849 | 1,061 |
| EqualsValueClause | 17,186 | 1,125 |
| Block | 15,787 | 1,200 |
| LocalDeclarationStatement | 14,959 | 929 |
| ParameterList | 11,889 | 1,565 |
| MethodDeclaration | 10,214 | 1,237 |
| NumericLiteralExpression | 9,285 | 925 |
| GenericName | 8,853 | 1,085 |
| TypeArgumentList | 8,853 | 1,085 |
| QualifiedName | 7,603 | 1,627 |
| SimpleAssignmentExpression | 6,261 | 826 |
| ExpressionElement | 5,562 | 393 |
| CollectionExpression | 5,536 | 729 |
| ReturnStatement | 5,218 | 893 |
| UsingDirective | 5,048 | 1,389 |
| IfStatement | 3,642 | 575 |
| NullLiteralExpression | 3,542 | 742 |
| NullableType | 3,378 | 844 |
| ObjectCreationExpression | 3,194 | 701 |
| SingleVariableDesignation | 2,858 | 477 |
| Attribute | 2,835 | 348 |
| AttributeList | 2,835 | 348 |
| ArrowExpressionClause | 2,720 | 420 |
| SimpleLambdaExpression | 2,528 | 510 |
| BracketedArgumentList | 2,489 | 454 |
| FieldDeclaration | 2,365 | 620 |
| IsPatternExpression | 2,356 | 486 |
| PropertyDeclaration | 2,153 | 427 |
| ElementAccessExpression | 2,042 | 410 |
| ImplicitObjectCreationExpression | 1,729 | 443 |
| DeclarationPattern | 1,718 | 320 |
| FalseLiteralExpression | 1,650 | 480 |
| CompilationUnit | 1,627 | 1,627 |
| FileScopedNamespaceDeclaration | 1,626 | 1,626 |
| ConstantPattern | 1,552 | 375 |
| EqualsExpression | 1,343 | 489 |
| TrueLiteralExpression | 1,338 | 425 |
| InterpolatedStringText | 1,298 | 147 |
| ForEachStatement | 1,224 | 398 |
| ClassDeclaration | 1,166 | 1,150 |
| LogicalAndExpression | 1,160 | 324 |
| ConditionalExpression | 1,147 | 448 |
| Interpolation | 1,126 | 146 |
| TupleExpression | 1,110 | 230 |
| ConditionalAccessExpression | 1,098 | 329 |
| MemberBindingExpression | 1,098 | 329 |
| AddExpression | 1,089 | 289 |
| AddAssignmentExpression | 1,062 | 226 |
| LogicalNotExpression | 1,040 | 343 |
| DeclarationExpression | 885 | 230 |
| AccessorList | 860 | 249 |
| GetAccessorDeclaration | 856 | 248 |
| CoalesceExpression | 845 | 350 |
| LogicalOrExpression | 823 | 258 |
| SwitchExpressionArm | 816 | 88 |
| AttributeArgument | 786 | 47 |
| NotPattern | 779 | 315 |
| ParenthesizedLambdaExpression | 740 | 239 |
| TupleElement | 664 | 160 |
| SuppressNullableWarningExpression | 650 | 249 |
| InterpolatedStringExpression | 639 | 147 |
| WithExpression | 573 | 199 |
| WithInitializerExpression | 573 | 199 |
| NameColon | 564 | 167 |
| GreaterThanExpression | 555 | 259 |
| ArrayRankSpecifier | 499 | 197 |
| ArrayType | 498 | 197 |
| OmittedArraySizeExpression | 485 | 197 |
| AwaitExpression | 471 | 124 |
| RecordDeclaration | 467 | 380 |
| PostIncrementExpression | 463 | 211 |
| ImplicitElementAccess | 447 | 81 |
| CharacterLiteralExpression | 438 | 99 |
| ConstructorDeclaration | 436 | 430 |
| RecursivePattern | 424 | 108 |
| LessThanExpression | 417 | 191 |
| PropertyPatternClause | 415 | 102 |
| Subpattern | 384 | 100 |
| NotEqualsExpression | 381 | 160 |
| CastExpression | 356 | 127 |
| AttributeArgumentList | 338 | 47 |
| ObjectInitializerExpression | 335 | 149 |
| BaseList | 332 | 239 |
| LockStatement | 323 | 49 |
| SwitchSection | 315 | 34 |
| ParenthesizedExpression | 299 | 159 |
| BreakStatement | 297 | 58 |
| CatchClause | 293 | 116 |
| CatchDeclaration | 289 | 115 |
| TupleType | 286 | 160 |
| ContinueStatement | 281 | 137 |
| TryStatement | 253 | 122 |
| OrPattern | 247 | 83 |
| SimpleBaseType | 234 | 220 |
| ForStatement | 230 | 142 |
| InitAccessorDeclaration | 215 | 72 |
| EnumMemberDeclaration | 197 | 27 |
| TypeOfExpression | 175 | 39 |
| CaseSwitchLabel | 172 | 23 |
| WhileStatement | 170 | 102 |
| SubtractExpression | 163 | 79 |
| ThrowStatement | 157 | 79 |
| UsingStatement | 157 | 65 |
| SetAccessorDeclaration | 152 | 67 |
| SpreadElement | 152 | 75 |
| SubtractAssignmentExpression | 151 | 62 |
| ThisExpression | 150 | 81 |
| DiscardPattern | 149 | 89 |
| ThrowExpression | 145 | 78 |
| SwitchExpression | 143 | 88 |
| EventFieldDeclaration | 142 | 75 |
| UnaryMinusExpression | 142 | 61 |
| ElseClause | 134 | 108 |
| CasePatternSwitchLabel | 108 | 19 |
| LessThanOrEqualExpression | 107 | 76 |
| WhenClause | 106 | 21 |
| PrimaryConstructorBaseType | 102 | 19 |
| GreaterThanOrEqualExpression | 82 | 57 |
| InterfaceDeclaration | 72 | 72 |
| RangeExpression | 72 | 36 |
| MultiplyExpression | 71 | 29 |
| ForEachVariableStatement | 69 | 48 |
| RelationalPattern | 69 | 26 |
| AsExpression | 64 | 42 |
| IndexExpression | 60 | 37 |
| TypeParameter | 59 | 27 |
| SwitchStatement | 58 | 34 |
| TypeParameterList | 56 | 27 |
| IsExpression | 52 | 31 |
| ArrayInitializerExpression | 48 | 23 |
| BitwiseOrExpression | 44 | 23 |
| YieldReturnStatement | 42 | 18 |
| DefaultSwitchLabel | 41 | 22 |
| CatchFilterClause | 38 | 28 |
| OrAssignmentExpression | 35 | 19 |
| ParenthesizedPattern | 34 | 15 |
| AndPattern | 33 | 13 |
| LeftShiftExpression | 32 | 2 |
| ExpressionColon | 30 | 13 |
| DivideExpression | 29 | 17 |
| VarPattern | 29 | 12 |
| EnumDeclaration | 27 | 27 |
| ImplicitArrayCreationExpression | 27 | 21 |
| FinallyClause | 23 | 19 |
| ArrayCreationExpression | 21 | 15 |
| ClassConstraint | 18 | 8 |
| CoalesceAssignmentExpression | 18 | 16 |
| TypeParameterConstraintClause | 18 | 8 |
| AttributeTargetSpecifier | 15 | 7 |
| ListPattern | 14 | 12 |
| LocalFunctionStatement | 14 | 8 |
| BaseExpression | 13 | 5 |
| ConversionOperatorDeclaration | 13 | 2 |
| CollectionInitializerExpression | 11 | 9 |
| TypePattern | 11 | 4 |
| PositionalPatternClause | 9 | 9 |
| PostDecrementExpression | 8 | 7 |
| InterpolationFormatClause | 7 | 1 |
| BaseConstructorInitializer | 6 | 6 |
| DefaultLiteralExpression | 6 | 4 |
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
| Fact | 2,432 | 336 |
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
| Collection expression | 144 | 2,280 | 1,335 | 245 | 299 | 485 | 140 | 266 | 92 | 199 | 51 |  |  |  |  |
| using declaration | 5 | 1,743 | 2,212 | 547 | 12 | 7 |  |  |  | 18 | 18 |  |  |  |  |
| if | 1,191 | 45 | 2 | 525 | 648 | 431 | 343 | 218 | 200 | 24 | 3 | 9 |  | 3 |  |
| Lambda | 181 | 641 | 883 | 21 | 290 | 633 | 303 | 94 | 67 | 118 | 25 | 4 |  |  | 8 |
| Nullable reference type T? | 456 | 127 | 242 | 270 | 285 | 264 | 300 | 316 | 244 | 354 | 24 | 6 |  | 4 |  |
| Expression-bodied member | 938 | 33 | 20 | 6 | 109 | 26 | 243 | 152 | 199 | 969 | 25 |  |  |  |  |
| Target-typed new | 716 | 134 | 83 | 118 | 137 | 172 | 33 | 46 | 76 | 189 | 19 | 1 |  | 1 | 4 |
| Declaration pattern | 898 | 5 | 1 | 23 | 190 | 326 | 171 | 21 | 73 | 7 | 3 |  |  |  |  |
| File-scoped namespace | 281 | 188 | 116 | 142 | 106 | 104 | 214 | 291 | 50 | 84 | 12 | 3 | 31 | 4 |  |
| Readonly field | 432 | 31 | 1 | 100 | 347 | 204 | 199 | 28 | 88 | 35 |  | 3 |  | 1 |  |
| Full property | 873 | 7 |  | 6 | 109 | 2 | 228 | 140 | 36 | 11 |  |  |  |  |  |
| Sealed class | 233 | 193 | 116 | 69 | 169 | 40 | 198 | 194 | 44 | 30 | 10 | 3 |  | 4 |  |
| foreach | 183 | 49 | 14 | 224 | 292 | 286 | 24 | 147 | 33 | 40 | 1 |  |  |  |  |
| Class | 268 | 196 | 116 | 140 | 85 | 97 | 61 | 22 | 43 | 91 | 12 | 3 | 31 | 1 |  |
| Conditional ?: | 217 | 10 | 11 | 207 | 224 | 134 | 136 | 125 | 56 | 24 | 1 | 2 |  |  |  |
| Null-conditional ?. ?[ | 171 | 126 | 68 | 49 | 44 | 82 | 433 | 36 | 76 | 7 | 5 | 1 |  |  |  |
| Tuple literal or tuple type | 38 | 383 | 196 | 79 | 67 | 138 | 49 | 36 | 76 | 21 | 10 |  |  |  |  |
| Constant pattern | 131 | 3 | 8 | 98 | 35 | 296 | 111 | 120 | 47 | 17 |  | 5 |  |  |  |
| Null-coalescing ?? | 80 | 18 | 11 | 91 | 167 | 67 | 81 | 246 | 40 | 38 | 5 |  |  | 1 |  |
| not pattern | 307 | 4 |  | 43 | 54 | 164 | 119 | 32 | 49 | 6 | 1 |  |  |  |  |
| Auto property | 343 | 5 |  | 8 | 5 | 23 | 73 | 225 | 42 | 13 |  |  |  |  |  |
| is null | 119 | 7 | 2 | 81 | 143 | 159 | 67 | 38 | 52 | 12 | 1 |  |  |  |  |
| Static lambda | 78 | 26 | 310 | 2 | 97 | 15 | 59 | 37 | 13 | 38 | 1 |  |  |  |  |
| Null-forgiving ! | 155 | 197 | 167 | 52 | 9 | 23 | 5 |  | 3 | 38 | 1 |  |  |  |  |
| Interpolated string | 19 | 48 | 6 | 158 | 51 | 349 |  | 6 |  | 2 |  |  |  |  |  |
| with expression | 7 | 172 | 18 | 42 | 240 | 9 | 12 | 17 | 32 | 24 |  |  |  |  |  |
| Extension method |  |  |  |  |  |  |  |  |  | 558 | 7 |  |  |  |  |
| Raw string | 2 | 106 | 7 | 210 |  | 203 |  |  |  | 8 |  |  |  |  |  |
| Const field | 75 | 57 | 11 | 153 | 7 | 102 | 3 | 64 | 4 | 10 |  | 4 |  | 1 |  |
| Nullable value type T? | 53 | 20 | 19 | 39 | 32 | 10 | 118 | 29 | 99 | 58 | 7 |  |  | 2 |  |
| await | 55 | 207 | 106 | 32 | 28 | 1 | 10 |  | 17 | 5 |  | 10 |  |  |  |
| Record class | 16 |  |  | 2 | 103 | 6 | 143 | 191 | 1 | 2 |  |  |  | 3 |  |
| Constructor | 164 | 2 |  | 60 | 66 | 7 | 53 | 3 | 39 | 7 |  | 2 | 31 |  |  |
| Property pattern | 142 | 2 |  | 6 | 13 | 235 | 8 | 3 | 4 | 1 | 1 |  |  |  |  |
| Optional parameter | 8 | 11 | 3 | 6 | 9 | 1 | 4 | 245 | 16 | 93 |  |  |  | 3 |  |
| nameof | 223 |  |  | 5 | 116 | 16 | 16 | 1 | 4 | 1 | 1 |  |  |  |  |
| Async method | 54 | 160 | 76 | 19 | 20 | 1 | 10 |  | 13 | 5 |  | 5 |  |  |  |
| Cast expression | 146 | 2 | 52 | 71 | 2 | 41 | 2 | 4 |  | 22 | 13 | 1 |  |  |  |
| Object initializer | 135 | 35 | 62 | 16 |  | 47 |  | 3 |  | 28 | 7 | 1 |  | 1 |  |
| lock | 13 |  |  | 2 | 73 | 5 |  |  | 225 | 5 |  |  |  |  |  |
| Deconstruction | 27 | 65 | 107 | 21 | 23 | 30 | 19 | 4 | 6 |  |  |  |  |  |  |
| catch | 21 |  |  | 110 | 28 | 8 | 85 | 1 | 27 | 1 | 5 | 4 |  | 3 |  |
| Static class | 43 | 3 |  | 73 | 18 | 63 | 6 | 19 |  | 61 | 2 |  |  |  |  |
| and / or pattern | 37 | 1 | 3 | 51 | 9 | 131 | 1 | 39 | 2 | 6 |  |  |  |  |  |
| try | 18 | 5 |  | 66 | 28 | 10 | 86 | 1 | 28 | 1 | 5 | 4 |  | 1 |  |
| for | 44 | 11 | 6 | 45 | 57 | 23 | 4 | 12 | 10 | 16 | 2 |  |  |  |  |
| Named argument |  | 87 | 18 | 16 | 23 | 16 |  | 18 | 2 | 46 |  |  |  |  |  |
| Init accessor | 3 |  |  |  |  | 3 |  | 209 |  |  |  |  |  |  |  |
| typeof | 161 |  | 1 | 3 |  | 3 |  |  |  | 1 | 6 |  |  |  |  |
| while | 10 | 13 | 4 | 91 | 9 | 19 | 2 | 16 | 1 | 5 |  |  |  |  |  |
| using statement |  | 35 | 1 | 113 | 7 |  |  |  |  |  | 1 |  |  |  |  |
| Spread element | 7 | 11 | 6 |  | 23 | 23 | 6 | 42 | 21 | 13 |  |  |  |  |  |
| Discard pattern | 14 |  | 1 | 9 | 13 | 60 | 14 | 24 | 11 | 1 |  | 2 |  |  |  |
| throw expression | 4 | 2 | 19 | 4 | 68 | 1 | 14 | 1 | 8 | 23 | 1 |  |  |  |  |
| Switch expression | 12 |  | 1 | 9 | 13 | 57 | 14 | 23 | 11 | 1 |  | 2 |  |  |  |
| Field-like event | 51 |  |  |  |  |  | 90 |  | 1 |  |  |  |  |  |  |
| Discard _ | 20 | 22 | 55 | 1 | 8 | 7 | 2 | 9 | 5 | 1 |  |  |  |  |  |
| Partial type | 48 |  |  | 5 |  | 7 |  |  | 4 | 26 |  |  | 31 |  |  |
| Case guard when | 6 |  |  |  | 1 | 94 |  | 3 |  | 2 |  |  |  |  |  |
| Verbatim string |  | 4 |  |  | 3 | 78 |  |  |  | 2 |  |  |  |  |  |
| Interface | 2 |  |  |  | 1 |  | 1 | 62 | 6 |  |  |  |  |  |  |
| Range .. |  | 2 | 16 | 7 | 2 | 27 |  | 18 |  |  |  |  |  |  |  |
| Relational pattern | 1 |  |  | 15 | 10 | 2 |  | 39 | 2 |  |  |  |  |  |  |
| as cast | 54 |  |  | 2 |  | 8 |  |  |  |  |  |  |  |  |  |
| Index from end ^ | 2 | 7 | 31 | 1 | 4 | 11 |  | 4 |  |  |  |  |  |  |  |
| Switch statement | 7 |  |  | 8 |  | 19 | 2 | 19 | 1 | 2 |  |  |  |  |  |
| Generic method | 26 |  |  |  | 12 | 1 | 5 | 4 | 2 | 4 |  |  |  |  |  |
| is type test | 21 | 1 |  |  |  | 30 |  |  |  |  |  |  |  |  |  |
| yield return / break | 9 |  |  | 7 | 7 | 23 |  | 2 |  |  |  |  |  |  |  |
| Exception filter | 3 |  |  | 24 | 5 | 3 |  |  | 3 |  |  |  |  |  |  |
| Local const |  | 22 | 1 | 4 |  | 4 | 1 |  |  |  |  |  |  |  |  |
| var pattern |  |  |  |  |  | 27 | 1 |  | 1 |  |  |  |  |  |  |
| Enum | 2 |  |  |  |  |  | 9 | 16 |  |  |  |  |  |  |  |
| Nested type | 7 | 8 |  |  |  |  |  |  |  | 9 |  |  |  |  |  |
| finally |  | 5 |  | 2 | 8 | 2 | 1 |  | 3 |  |  | 2 |  |  |  |
| ref or out parameter | 2 | 2 | 1 | 1 | 5 | 2 |  | 4 | 2 | 4 |  |  |  |  |  |
| Default interface method body |  |  |  |  |  |  |  |  | 19 |  |  |  |  |  |  |
| Constraint clause | 14 |  |  |  | 1 |  |  |  |  | 3 |  |  |  |  |  |
| Null-coalescing assignment ??= | 10 |  |  | 1 | 3 | 3 |  |  |  |  |  | 1 |  |  |  |
| List pattern | 4 | 1 |  |  |  | 8 |  |  | 1 |  |  |  |  |  |  |
| Local function | 4 | 1 |  |  |  | 3 | 5 |  |  |  |  | 1 |  |  |  |
| Conversion operator | 12 |  |  |  |  |  |  | 1 |  |  |  |  |  |  |  |
| Collection initializer | 3 |  | 2 | 3 |  |  |  |  |  | 1 | 2 |  |  |  |  |
| Type pattern | 1 |  |  |  |  | 10 |  |  |  |  |  |  |  |  |  |
| Positional pattern | 2 |  |  | 1 | 1 | 1 | 4 |  |  |  |  |  |  |  |  |
| default literal |  |  |  | 1 |  | 5 |  |  |  |  |  |  |  |  |  |
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
| Object creation | 50.96 % | 54.25 % | 20.29 % | 25.76 % | 28.48 % | 42.47 % | 9.62 % | 25.56 % | 29.92 % | 29.12 % | 35.19 % | 7.69 % | - | 14.29 % | 22.22 % |
| Collection creation | 94.74 % | 99.61 % | 99.11 % | 99.19 % | 100.00 % | 98.98 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 92.73 % | - | - | - | - |
| Member body | 32.45 % | 2.21 % | 1.80 % | 0.71 % | 11.57 % | 4.09 % | 20.80 % | 30.83 % | 24.78 % | 78.59 % | 38.46 % | 0.00 % | 0.00 % | 0.00 % | - |
| Local type | 4.64 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.35 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | - | 0.00 % | 0.00 % |
| Using | 100.00 % | 98.03 % | 99.95 % | 82.88 % | 63.16 % | 100.00 % | - | - | - | 100.00 % | 94.74 % | - | - | - | - |
| Namespace | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - |
| Text building | 54.29 % | 35.29 % | 16.22 % | 77.45 % | 83.61 % | 81.73 % | 0.00 % | 26.09 % | - | 22.22 % | - | 0.00 % | - | 0.00 % | - |
| Branch | 63.16 % | - | 100.00 % | 52.94 % | 100.00 % | 75.00 % | 87.50 % | 54.76 % | 91.67 % | 33.33 % | - | 100.00 % | - | - | - |
| Lambda body | 86.74 % | 96.88 % | 81.20 % | 85.71 % | 91.03 % | 98.89 % | 98.68 % | 95.74 % | 94.03 % | 78.81 % | 44.00 % | 50.00 % | - | - | 75.00 % |
| Type test | 91.65 % | 100.00 % | 100.00 % | 89.47 % | 100.00 % | 94.41 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - | - | - | - |
| Constructor | 0.00 % | 0.00 % | - | 0.00 % | 0.00 % | 36.36 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | - | 0.00 % | 0.00 % | - | - |
