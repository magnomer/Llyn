# Syntax statistics - 0.18.14100

- Generated: 2026-10-09 23:56:29 +09:00
- Source roots: src, tests
- Root-level files: none
- Project segments: 1
- Excluded directories: .git, .vs, artifacts, bin, node_modules, obj, packages, publish
- Excluded suffixes: .g.cs, .g.i.cs, .AssemblyInfo.cs, .GlobalUsings.g.cs, .Designer.cs
- Roslyn: 4.14.0-3.25262.10
- Language version: Preview
- Scanned: 1,880 files, 15 projects, 195,860 lines

## Summary

| Measure | Value |
|---|---:|
| Files | 1,880 |
| Lines | 195,860 |
| Nodes | 1,034,063 |
| Syntax kinds used | 191 |
| Features used | 98 of 138 |
| Minimum C# version | 12 |
| Parse errors | 0 |

## Families

| Family | Features | Used | Count | Per 1k lines |
|---|---:|---:|---:|---:|
| Types | 15 | 12 | 3,909 | 19.96 |
| Namespaces | 6 | 2 | 1,880 | 9.60 |
| Members | 29 | 20 | 9,302 | 47.49 |
| Patterns | 16 | 16 | 6,031 | 30.79 |
| Expressions | 36 | 29 | 24,549 | 125.34 |
| Statements | 20 | 13 | 12,262 | 62.61 |
| Generics | 6 | 2 | 72 | 0.37 |
| Async | 2 | 2 | 453 | 2.31 |
| Nullability | 3 | 2 | 3,863 | 19.72 |
| Unsafe | 2 | 0 | 0 | 0.00 |
| Directives | 3 | 0 | 0 | 0.00 |
| Total | 138 | 98 | 62,321 | 318.19 |

## Projects

| Project | Files | Lines | Nodes | Features used | Newest C# |
|---|---:|---:|---:|---:|---:|
| Llyn.Tests.Engine | 210 | 33,894 | 236,573 | 59 | 12 |
| Llyn.UIDeportment | 299 | 32,222 | 148,059 | 86 | 12 |
| Llyn.Tests.Conduct | 140 | 24,640 | 179,445 | 61 | 12 |
| Llyn.Infrastructure | 169 | 22,477 | 102,271 | 70 | 12 |
| Llyn.Application | 121 | 18,205 | 84,131 | 70 | 12 |
| Llyn.Tests.Convention | 115 | 17,411 | 84,750 | 79 | 12 |
| Llyn.Conduct | 246 | 15,549 | 60,893 | 59 | 12 |
| Llyn.Core | 337 | 11,338 | 46,107 | 59 | 12 |
| Llyn.ShellEngine | 90 | 9,311 | 37,302 | 57 | 12 |
| Llyn.Tests.Interface | 88 | 8,417 | 42,480 | 65 | 12 |
| Llyn.Tests.Windows | 21 | 1,455 | 8,966 | 41 | 12 |
| Llyn.Core.Windows | 4 | 385 | 1,427 | 26 | 10 |
| Llyn.UIVeneer | 35 | 383 | 760 | 4 | 10 |
| Llyn.UIDeportment.Capsule | 4 | 88 | 380 | 15 | 10 |
| Llyn.Host | 1 | 85 | 519 | 4 | 9 |
| Total | 1,880 | 195,860 | 1,034,063 | 98 | 12 |

## Versions

| C# version | Features | Used | Count |
|---|---:|---:|---:|
| 1 | 51 | 35 | 16,590 |
| 2 | 10 | 8 | 1,955 |
| 3 | 7 | 5 | 6,134 |
| 4 | 3 | 2 | 726 |
| 5 | 3 | 3 | 1,061 |
| 6 | 6 | 5 | 4,690 |
| 7 | 11 | 10 | 5,800 |
| 7.1 | 1 | 1 | 7 |
| 7.2 | 1 | 0 | 0 |
| 7.3 | 1 | 0 | 0 |
| 8 | 17 | 12 | 10,246 |
| 9 | 10 | 9 | 5,887 |
| 10 | 3 | 2 | 1,882 |
| 11 | 6 | 3 | 620 |
| 12 | 4 | 3 | 6,723 |
| 13 | 1 | 0 | 0 |
| 14 | 3 | 0 | 0 |

## Features

| Feature | Family | C# | Count | Per 1k lines | Files | First |
|---|---|---|---:|---:|---:|---|
| Collection expression | Expressions | 12 | 6,541 | 33.40 | 847 | src/Llyn.Application/Card/LCardClerk.cs:57 |
| using declaration | Statements | 8 | 5,119 | 26.14 | 396 | src/Llyn.Application/Card/LTranslationClerk.cs:160 |
| Lambda | Expressions | 3 | 4,237 | 21.63 | 682 | src/Llyn.Application/Card/LCardClerk.cs:174 |
| if | Statements | 1 | 3,910 | 19.96 | 651 | src/Llyn.Application/Card/LCardClerk.cs:70 |
| Nullable reference type T? | Nullability | 8 | 3,339 | 17.05 | 910 | src/Llyn.Application/Card/LCardClerkField.cs:91 |
| Expression-bodied member | Members | 6 | 2,511 | 12.82 | 484 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Target-typed new | Expressions | 9 | 2,121 | 10.83 | 525 | src/Llyn.Application/Card/LMentionClerk.cs:167 |
| Readonly field | Members | 1 | 1,933 | 9.87 | 587 | src/Llyn.Application/Card/LCardClerk.cs:9 |
| File-scoped namespace | Namespaces | 10 | 1,879 | 9.59 | 1,879 | src/Llyn.Application/Card/LCardClerk.cs:5 |
| Declaration pattern | Patterns | 7 | 1,830 | 9.34 | 363 | src/Llyn.Application/Card/LMeaningClerk.cs:93 |
| Sealed class | Types | 1 | 1,488 | 7.60 | 1,387 | src/Llyn.Application/Card/LCardClerk.cs:7 |
| foreach | Statements | 1 | 1,430 | 7.30 | 456 | src/Llyn.Application/Card/LCardClerk.cs:59 |
| Tuple literal or tuple type | Expressions | 7 | 1,368 | 6.98 | 259 | src/Llyn.Application/Card/LMeaningClerk.cs:26 |
| Class | Types | 1 | 1,339 | 6.84 | 1,322 | src/Llyn.Application/Card/LCardClerk.cs:7 |
| Conditional ?: | Expressions | 1 | 1,299 | 6.63 | 501 | src/Llyn.Application/Card/LCardClerk.cs:173 |
| Full property | Members | 1 | 1,112 | 5.68 | 343 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Static lambda | Expressions | 9 | 1,065 | 5.44 | 275 | src/Llyn.Application/Card/LCardEquality.cs:113 |
| Null-conditional ?. ?[ | Expressions | 6 | 1,052 | 5.37 | 371 | src/Llyn.Application/Card/LMentionClerk.cs:47 |
| Constant pattern | Patterns | 7 | 1,025 | 5.23 | 185 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Null-coalescing ?? | Expressions | 2 | 907 | 4.63 | 371 | src/Llyn.Application/Card/LCardClerkField.cs:74 |
| not pattern | Patterns | 9 | 859 | 4.39 | 354 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| Auto property | Members | 3 | 841 | 4.29 | 248 | src/Llyn.Application/Citation/LCitationClerk.cs:41 |
| is null | Patterns | 7 | 770 | 3.93 | 337 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| Null-forgiving ! | Expressions | 8 | 756 | 3.86 | 297 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:145 |
| Interpolated string | Expressions | 6 | 677 | 3.46 | 160 | src/Llyn.Application/Localization/LLocalizationReader.cs:55 |
| with expression | Expressions | 9 | 673 | 3.44 | 232 | src/Llyn.Application/Card/LCardClerk.cs:109 |
| await | Expressions | 5 | 608 | 3.10 | 148 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:139 |
| Extension method | Members | 3 | 605 | 3.09 | 41 | tests/Llyn.Tests.Interface/TInterfaceArea.cs:7 |
| Raw string | Expressions | 11 | 601 | 3.07 | 138 | src/Llyn.Infrastructure/Database/Base/LDatabase.cs:51 |
| Const field | Members | 1 | 563 | 2.87 | 237 | src/Llyn.Application/Card/LMentionClerk.cs:11 |
| Constructor | Members | 1 | 555 | 2.83 | 550 | src/Llyn.Application/Card/LCardClerk.cs:19 |
| Nullable value type T? | Nullability | 2 | 524 | 2.68 | 238 | src/Llyn.Application/Card/LMeaningClerk.cs:92 |
| Record class | Types | 9 | 516 | 2.63 | 428 | src/Llyn.Application/Ensign/LEnsignRow.cs:3 |
| Property pattern | Patterns | 8 | 466 | 2.38 | 116 | src/Llyn.Application/Draft/Clerk/LDraftClerk.cs:82 |
| Async method | Async | 5 | 446 | 2.28 | 148 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:91 |
| Cast expression | Expressions | 1 | 439 | 2.24 | 156 | src/Llyn.Application/Markup/LMarkupClerkLink.cs:234 |
| Optional parameter | Members | 4 | 439 | 2.24 | 148 | src/Llyn.Application/Draft/Clerk/LDraftClerkPanel.cs:170 |
| Object initializer | Expressions | 3 | 435 | 2.22 | 192 | src/Llyn.Application/Portrait/LPortraitClerkLabel.cs:25 |
| nameof | Expressions | 6 | 375 | 1.91 | 132 | src/Llyn.Application/Citation/LAuthorClerk.cs:361 |
| catch | Statements | 1 | 350 | 1.79 | 155 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:142 |
| lock | Statements | 1 | 336 | 1.72 | 57 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:98 |
| and / or pattern | Patterns | 9 | 335 | 1.71 | 102 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Static class | Types | 2 | 327 | 1.67 | 327 | src/Llyn.Application/Card/LCardClerkField.cs:7 |
| Deconstruction | Expressions | 7 | 324 | 1.65 | 138 | src/Llyn.Application/Card/LMeaningClerk.cs:46 |
| try | Statements | 1 | 308 | 1.57 | 166 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:137 |
| Named argument | Members | 4 | 287 | 1.47 | 89 | src/Llyn.Application/Card/LTranslationClerk.cs:164 |
| for | Statements | 1 | 258 | 1.32 | 158 | src/Llyn.Application/Card/LCardClerk.cs:247 |
| Init accessor | Members | 9 | 218 | 1.11 | 74 | src/Llyn.Conduct/Lexicon/CExample.cs:20 |
| while | Statements | 1 | 192 | 0.98 | 115 | src/Llyn.Application/Card/LMeaningClerk.cs:44 |
| typeof | Expressions | 1 | 191 | 0.98 | 43 | src/Llyn.Infrastructure/Configuration/LLocalizationLoader.cs:20 |
| Discard pattern | Patterns | 8 | 184 | 0.94 | 111 | src/Llyn.Application/Citation/LAuthorClerk.cs:243 |
| Switch expression | Patterns | 8 | 179 | 0.91 | 111 | src/Llyn.Application/Citation/LAuthorClerk.cs:239 |
| Spread element | Expressions | 12 | 178 | 0.91 | 98 | src/Llyn.Application/Card/LCardEquality.cs:53 |
| throw expression | Expressions | 7 | 176 | 0.90 | 94 | src/Llyn.Application/Card/LCardClerkField.cs:75 |
| using statement | Statements | 1 | 162 | 0.83 | 68 | src/Llyn.Application/Citation/LAuthorCitation.cs:51 |
| Field-like event | Members | 1 | 158 | 0.81 | 94 | src/Llyn.Conduct/Atelier/CNavigation.cs:46 |
| Discard _ | Expressions | 7 | 146 | 0.75 | 53 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:108 |
| Case guard when | Patterns | 7 | 115 | 0.59 | 24 | src/Llyn.Application/Citation/LAuthorClerk.cs:241 |
| Interface | Types | 1 | 98 | 0.50 | 98 | src/Llyn.Application/Pronunciation/Relay/LListener.cs:5 |
| Relational pattern | Patterns | 9 | 89 | 0.45 | 30 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Verbatim string | Expressions | 1 | 87 | 0.44 | 19 | src/Llyn.Application/Localization/LLocalizationReader.cs:13 |
| as cast | Expressions | 1 | 85 | 0.43 | 44 | src/Llyn.Infrastructure/Database/Base/LWorkspaceArchive.cs:132 |
| Range .. | Expressions | 8 | 85 | 0.43 | 42 | src/Llyn.Application/Card/LMentionClerk.cs:173 |
| Index from end ^ | Expressions | 8 | 79 | 0.40 | 45 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:230 |
| Exception filter | Statements | 6 | 75 | 0.38 | 49 | src/Llyn.Application/Outpost/LCourierClerk.cs:204 |
| Partial type | Types | 2 | 72 | 0.37 | 72 | src/Llyn.UIVeneer/Display/View/PDisplay.xaml.cs:5 |
| is type test | Patterns | 1 | 59 | 0.30 | 36 | src/Llyn.Application/Outpost/LCourierClerk.cs:296 |
| Switch statement | Patterns | 1 | 57 | 0.29 | 34 | src/Llyn.Conduct/Configuration/CLedger.cs:262 |
| Generic method | Generics | 2 | 54 | 0.28 | 29 | src/Llyn.Application/Card/LCardClerkField.cs:183 |
| yield return / break | Statements | 2 | 50 | 0.26 | 20 | src/Llyn.Application/Card/LCardClerkField.cs:35 |
| Local const | Statements | 1 | 37 | 0.19 | 22 | src/Llyn.Conduct/Lexicon/CStateWording.cs:10 |
| finally | Statements | 1 | 35 | 0.18 | 26 | src/Llyn.Application/Outpost/LCourierClerk.cs:80 |
| Enum | Types | 1 | 32 | 0.16 | 32 | src/Llyn.Conduct/Catalog/CCatalogOrder.cs:3 |
| var pattern | Patterns | 7 | 29 | 0.15 | 11 | src/Llyn.Conduct/Card/CFolio.cs:193 |
| ref or out parameter | Members | 1 | 27 | 0.14 | 21 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:149 |
| Nested type | Types | 1 | 25 | 0.13 | 15 | src/Llyn.UIDeportment/Kit/Bind/QLook.cs:19 |
| Null-coalescing assignment ??= | Expressions | 8 | 19 | 0.10 | 17 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:160 |
| Constraint clause | Generics | 2 | 18 | 0.09 | 10 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:171 |
| Local function | Members | 7 | 17 | 0.09 | 11 | src/Llyn.Conduct/Desk/CErrand.cs:55 |
| Collection initializer | Expressions | 3 | 16 | 0.08 | 12 | src/Llyn.Application/Outpost/LCourierClerk.cs:185 |
| List pattern | Patterns | 11 | 14 | 0.07 | 12 | src/Llyn.ShellEngine/Engine/LDraftFacade.cs:70 |
| Conversion operator | Members | 1 | 13 | 0.07 | 2 | src/Llyn.Core/Lexicon/State/LStateValue.cs:49 |
| Type pattern | Patterns | 9 | 11 | 0.06 | 4 | src/Llyn.UIDeportment/Sound/Accent/QAccentControl.cs:62 |
| Default interface method body | Members | 8 | 9 | 0.05 | 4 | src/Llyn.ShellEngine/Port/LDraftPort.cs:58 |
| Positional pattern | Patterns | 8 | 9 | 0.05 | 9 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:37 |
| Async lambda | Async | 5 | 7 | 0.04 | 4 | src/Llyn.Application/Outpost/LCourierClerk.cs:259 |
| default literal | Expressions | 7.1 | 7 | 0.04 | 5 | src/Llyn.Infrastructure/Pronunciation/Loader/LSourceLoader.cs:73 |
| Required member | Members | 11 | 5 | 0.03 | 3 | src/Llyn.UIDeportment/Panel/Taxonomy/QMembershipItem.cs:27 |
| Event accessors | Members | 1 | 4 | 0.02 | 3 | src/Llyn.ShellEngine/Port/LSettingsOutlet.cs:77 |
| Primary constructor on class or struct | Types | 12 | 4 | 0.02 | 4 | tests/Llyn.Tests.Convention/TAuditFakeMember.cs:3 |
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
| Object creation | Target-typed new() | 2,121 | new T() | 3,772 | 35.99 % |
| Collection creation | Collection expression | 6,541 | Collection initializer or array creation | 51 | 99.23 % |
| Member body | Expression body | 2,511 | Block body | 9,694 | 20.57 % |
| Local type | var | 42 | Explicit type | 20,009 | 0.21 % |
| Using | Declaration | 5,119 | Statement | 162 | 96.93 % |
| Namespace | File-scoped | 1,879 | Block | 0 | 100.00 % |
| Text building | Interpolated string | 677 | string.Format or + with a string literal | 430 | 61.16 % |
| Branch | Switch expression | 179 | Switch statement | 57 | 75.85 % |
| Lambda body | Expression | 3,801 | Block | 436 | 89.71 % |
| Type test | is pattern with designation | 1,039 | as | 85 | 92.44 % |
| Constructor | Primary | 4 | Explicit | 550 | 0.72 % |

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
| IdentifierName | 299,216 | 1,880 |
| Argument | 107,607 | 1,427 |
| SimpleMemberAccessExpression | 98,551 | 1,410 |
| ArgumentList | 66,558 | 1,462 |
| InvocationExpression | 60,803 | 1,419 |
| PredefinedType | 29,326 | 1,752 |
| ExpressionStatement | 29,133 | 1,305 |
| StringLiteralExpression | 24,221 | 1,044 |
| Parameter | 21,421 | 1,700 |
| VariableDeclaration | 20,750 | 1,237 |
| VariableDeclarator | 20,750 | 1,237 |
| EqualsValueClause | 19,694 | 1,289 |
| LocalDeclarationStatement | 17,324 | 1,061 |
| Block | 17,213 | 1,379 |
| ParameterList | 13,014 | 1,809 |
| MethodDeclaration | 10,938 | 1,446 |
| NumericLiteralExpression | 10,756 | 1,027 |
| GenericName | 10,098 | 1,293 |
| TypeArgumentList | 10,098 | 1,293 |
| QualifiedName | 8,811 | 1,880 |
| ExpressionElement | 7,282 | 454 |
| SimpleAssignmentExpression | 7,049 | 974 |
| CollectionExpression | 6,541 | 847 |
| UsingDirective | 5,955 | 1,616 |
| ReturnStatement | 5,623 | 1,015 |
| NullLiteralExpression | 3,987 | 850 |
| IfStatement | 3,910 | 651 |
| NullableType | 3,863 | 957 |
| ObjectCreationExpression | 3,772 | 844 |
| Attribute | 3,305 | 401 |
| AttributeList | 3,305 | 401 |
| SimpleLambdaExpression | 3,270 | 606 |
| SingleVariableDesignation | 3,114 | 539 |
| BracketedArgumentList | 2,953 | 520 |
| FieldDeclaration | 2,853 | 739 |
| IsPatternExpression | 2,592 | 547 |
| ArrowExpressionClause | 2,513 | 485 |
| ElementAccessExpression | 2,343 | 467 |
| ImplicitObjectCreationExpression | 2,121 | 525 |
| PropertyDeclaration | 1,958 | 490 |
| FalseLiteralExpression | 1,926 | 530 |
| CompilationUnit | 1,880 | 1,880 |
| FileScopedNamespaceDeclaration | 1,879 | 1,879 |
| DeclarationPattern | 1,830 | 363 |
| ConstantPattern | 1,795 | 425 |
| TrueLiteralExpression | 1,558 | 473 |
| EqualsExpression | 1,505 | 551 |
| InterpolatedStringText | 1,486 | 160 |
| AddExpression | 1,374 | 333 |
| TupleExpression | 1,367 | 260 |
| ForEachStatement | 1,348 | 440 |
| ClassDeclaration | 1,339 | 1,322 |
| LogicalAndExpression | 1,327 | 374 |
| Interpolation | 1,313 | 159 |
| ConditionalExpression | 1,299 | 501 |
| AttributeArgument | 1,165 | 57 |
| LogicalNotExpression | 1,149 | 392 |
| AddAssignmentExpression | 1,106 | 267 |
| ConditionalAccessExpression | 1,052 | 371 |
| MemberBindingExpression | 1,052 | 371 |
| DeclarationExpression | 1,001 | 263 |
| ParenthesizedLambdaExpression | 967 | 280 |
| SwitchExpressionArm | 953 | 111 |
| AccessorList | 948 | 272 |
| GetAccessorDeclaration | 943 | 270 |
| LogicalOrExpression | 941 | 294 |
| CoalesceExpression | 907 | 371 |
| NotPattern | 859 | 354 |
| TupleElement | 772 | 187 |
| SuppressNullableWarningExpression | 756 | 297 |
| InterpolatedStringExpression | 677 | 160 |
| WithExpression | 673 | 232 |
| WithInitializerExpression | 673 | 232 |
| NameColon | 657 | 191 |
| GreaterThanExpression | 651 | 288 |
| ArrayRankSpecifier | 637 | 250 |
| ArrayType | 634 | 250 |
| OmittedArraySizeExpression | 622 | 250 |
| ImplicitElementAccess | 610 | 117 |
| AwaitExpression | 608 | 148 |
| ConstructorDeclaration | 557 | 552 |
| PostIncrementExpression | 520 | 233 |
| RecordDeclaration | 516 | 428 |
| CharacterLiteralExpression | 507 | 118 |
| RecursivePattern | 475 | 123 |
| LessThanExpression | 473 | 210 |
| PropertyPatternClause | 466 | 116 |
| AttributeArgumentList | 453 | 57 |
| NotEqualsExpression | 445 | 180 |
| CastExpression | 439 | 156 |
| ObjectInitializerExpression | 435 | 192 |
| Subpattern | 419 | 111 |
| BaseList | 363 | 269 |
| ParenthesizedExpression | 357 | 177 |
| CatchClause | 350 | 155 |
| CatchDeclaration | 347 | 153 |
| LockStatement | 336 | 57 |
| TupleType | 326 | 187 |
| ContinueStatement | 318 | 151 |
| SwitchSection | 309 | 34 |
| TryStatement | 308 | 166 |
| BreakStatement | 297 | 60 |
| OrPattern | 293 | 95 |
| SimpleBaseType | 271 | 250 |
| ForStatement | 258 | 158 |
| EnumMemberDeclaration | 218 | 32 |
| InitAccessorDeclaration | 218 | 74 |
| SubtractExpression | 199 | 90 |
| ThrowStatement | 197 | 100 |
| WhileStatement | 192 | 115 |
| TypeOfExpression | 191 | 43 |
| DiscardPattern | 184 | 111 |
| SwitchExpression | 179 | 111 |
| SpreadElement | 178 | 98 |
| ThrowExpression | 176 | 94 |
| CaseSwitchLabel | 168 | 22 |
| UsingStatement | 162 | 68 |
| UnaryMinusExpression | 159 | 71 |
| EventFieldDeclaration | 158 | 94 |
| SubtractAssignmentExpression | 156 | 68 |
| ElseClause | 147 | 114 |
| SetAccessorDeclaration | 132 | 67 |
| LessThanOrEqualExpression | 118 | 84 |
| ThisExpression | 116 | 78 |
| WhenClause | 115 | 24 |
| CasePatternSwitchLabel | 106 | 20 |
| GreaterThanOrEqualExpression | 103 | 67 |
| PrimaryConstructorBaseType | 103 | 19 |
| InterfaceDeclaration | 98 | 98 |
| RelationalPattern | 89 | 30 |
| AsExpression | 85 | 44 |
| RangeExpression | 85 | 42 |
| ForEachVariableStatement | 82 | 59 |
| MultiplyExpression | 81 | 32 |
| IndexExpression | 79 | 45 |
| CatchFilterClause | 75 | 49 |
| TypeParameter | 60 | 32 |
| IsExpression | 59 | 36 |
| SwitchStatement | 57 | 34 |
| TypeParameterList | 57 | 32 |
| ArrayInitializerExpression | 54 | 28 |
| BitwiseOrExpression | 48 | 25 |
| YieldReturnStatement | 43 | 20 |
| AndPattern | 42 | 18 |
| DefaultSwitchLabel | 41 | 22 |
| ParenthesizedPattern | 41 | 19 |
| OrAssignmentExpression | 36 | 21 |
| FinallyClause | 35 | 26 |
| ExpressionColon | 33 | 14 |
| ImplicitArrayCreationExpression | 33 | 26 |
| EnumDeclaration | 32 | 32 |
| LeftShiftExpression | 32 | 2 |
| DivideExpression | 31 | 19 |
| VarPattern | 29 | 11 |
| ArrayCreationExpression | 26 | 19 |
| CoalesceAssignmentExpression | 19 | 17 |
| TypeParameterConstraintClause | 18 | 10 |
| ClassConstraint | 17 | 9 |
| LocalFunctionStatement | 17 | 11 |
| CollectionInitializerExpression | 16 | 12 |
| AttributeTargetSpecifier | 15 | 7 |
| ListPattern | 14 | 12 |
| BaseExpression | 13 | 5 |
| ConversionOperatorDeclaration | 13 | 2 |
| PostDecrementExpression | 11 | 9 |
| TypePattern | 11 | 4 |
| PositionalPatternClause | 9 | 9 |
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
| RecordStructDeclaration | 3 | 3 |
| SlicePattern | 3 | 3 |
| ComplexElementInitializerExpression | 2 | 1 |
| ModuloExpression | 2 | 1 |
| UncheckedExpression | 2 | 2 |
| BitwiseAndExpression | 1 | 1 |
| BracketedParameterList | 1 | 1 |
| IndexerDeclaration | 1 | 1 |
| OmittedTypeArgument | 1 | 1 |
| ThisConstructorInitializer | 1 | 1 |
| TypeConstraint | 1 | 1 |

## Attributes

| Attribute | Count | Files |
|---|---:|---:|
| Fact | 2,769 | 387 |
| InlineData | 431 | 46 |
| Theory | 82 | 48 |
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
| Collection expression | 2,765 | 148 | 1,634 | 294 | 334 | 523 | 165 | 279 | 78 | 226 | 95 |  |  |  |  |
| using declaration | 2,008 | 5 | 2,460 | 579 | 12 | 7 |  |  |  | 18 | 30 |  |  |  |  |
| Lambda | 844 | 185 | 1,274 | 31 | 347 | 730 | 342 | 174 | 70 | 182 | 45 | 4 |  |  | 9 |
| if | 51 | 1,193 | 5 | 661 | 692 | 464 | 342 | 248 | 207 | 31 | 3 | 10 |  | 3 |  |
| Nullable reference type T? | 165 | 464 | 367 | 339 | 345 | 297 | 320 | 342 | 247 | 412 | 30 | 7 |  | 4 |  |
| Expression-bodied member | 39 | 646 | 37 | 15 | 110 | 26 | 193 | 164 | 126 | 1,126 | 29 |  |  |  |  |
| Target-typed new | 183 | 736 | 229 | 143 | 153 | 189 | 37 | 52 | 85 | 255 | 53 | 1 |  | 1 | 4 |
| Readonly field | 40 | 604 | 4 | 119 | 394 | 215 | 315 | 30 | 172 | 35 |  | 4 |  | 1 |  |
| File-scoped namespace | 210 | 299 | 140 | 169 | 121 | 115 | 246 | 337 | 90 | 88 | 21 | 4 | 35 | 4 |  |
| Declaration pattern | 7 | 899 | 4 | 42 | 195 | 368 | 204 | 26 | 72 | 10 | 3 |  |  |  |  |
| Sealed class | 215 | 256 | 140 | 75 | 182 | 43 | 227 | 228 | 63 | 33 | 18 | 4 |  | 4 |  |
| foreach | 53 | 192 | 18 | 285 | 319 | 306 | 25 | 159 | 32 | 40 | 1 |  |  |  |  |
| Tuple literal or tuple type | 423 | 43 | 266 | 170 | 84 | 168 | 56 | 44 | 73 | 27 | 14 |  |  |  |  |
| Class | 218 | 284 | 139 | 167 | 100 | 108 | 84 | 25 | 57 | 96 | 21 | 4 | 35 | 1 |  |
| Conditional ?: | 14 | 205 | 17 | 278 | 251 | 156 | 147 | 137 | 62 | 28 | 2 | 2 |  |  |  |
| Full property | 7 | 590 | 5 | 6 | 110 | 2 | 179 | 146 | 39 | 28 |  |  |  |  |  |
| Static lambda | 81 | 77 | 528 | 6 | 117 | 15 | 63 | 92 | 17 | 64 | 4 |  |  |  | 1 |
| Null-conditional ?. ?[ | 133 | 180 | 83 | 59 | 49 | 91 | 324 | 39 | 79 | 8 | 6 | 1 |  |  |  |
| Constant pattern | 6 | 133 | 8 | 142 | 54 | 353 | 126 | 138 | 43 | 17 |  | 5 |  |  |  |
| Null-coalescing ?? | 19 | 72 | 12 | 117 | 191 | 80 | 50 | 258 | 41 | 58 | 8 |  |  | 1 |  |
| not pattern | 4 | 317 | 1 | 55 | 73 | 185 | 134 | 34 | 49 | 6 | 1 |  |  |  |  |
| Auto property | 5 | 365 | 3 | 8 | 7 | 23 | 127 | 230 | 51 | 22 |  |  |  |  |  |
| is null | 8 | 127 | 5 | 95 | 164 | 175 | 84 | 42 | 55 | 13 | 2 |  |  |  |  |
| Null-forgiving ! | 229 | 148 | 203 | 62 | 15 | 25 | 9 |  | 3 | 61 | 1 |  |  |  |  |
| Interpolated string | 52 | 19 | 7 | 158 | 51 | 380 |  | 7 |  | 3 |  |  |  |  |  |
| with expression | 220 | 7 | 35 | 43 | 251 | 10 | 23 | 18 | 37 | 29 |  |  |  |  |  |
| await | 241 | 57 | 143 | 62 | 57 | 1 | 13 |  | 17 | 6 | 1 | 10 |  |  |  |
| Extension method |  |  |  |  |  |  |  |  |  | 599 | 6 |  |  |  |  |
| Raw string | 137 | 2 | 7 | 214 |  | 233 |  |  |  | 8 |  |  |  |  |  |
| Const field | 76 | 72 | 14 | 181 | 13 | 113 | 3 | 70 | 4 | 12 |  | 4 |  | 1 |  |
| Constructor | 2 | 222 | 1 | 67 | 78 | 7 | 74 | 3 | 56 | 8 |  | 2 | 35 |  |  |
| Nullable value type T? | 24 | 55 | 21 | 42 | 34 | 11 | 115 | 33 | 111 | 68 | 8 |  |  | 2 |  |
| Record class |  | 16 | 2 | 2 | 104 | 6 | 150 | 225 | 6 | 2 |  |  |  | 3 |  |
| Property pattern | 2 | 149 |  | 10 | 17 | 270 | 9 | 3 | 4 | 1 | 1 |  |  |  |  |
| Async method | 191 | 56 | 98 | 32 | 30 | 1 | 13 |  | 13 | 6 | 1 | 5 |  |  |  |
| Cast expression | 7 | 139 | 81 | 86 | 4 | 50 | 3 | 4 |  | 40 | 24 | 1 |  |  |  |
| Optional parameter | 12 | 8 | 6 | 6 | 11 | 1 | 10 | 265 | 12 | 103 | 2 |  |  | 3 |  |
| Object initializer | 57 | 128 | 97 | 29 | 1 | 60 |  | 3 |  | 34 | 24 | 1 |  | 1 |  |
| nameof | 1 | 201 | 3 | 8 | 117 | 17 | 18 | 3 | 4 | 2 | 1 |  |  |  |  |
| catch | 1 | 21 |  | 128 | 33 | 8 | 109 | 5 | 27 | 1 | 8 | 6 |  | 3 |  |
| lock |  | 13 |  | 2 | 77 | 5 | 2 |  | 232 | 5 |  |  |  |  |  |
| and / or pattern | 3 | 37 | 3 | 70 | 15 | 159 | 2 | 39 | 1 | 6 |  |  |  |  |  |
| Static class | 3 | 43 | 1 | 94 | 21 | 71 | 7 | 22 |  | 62 | 3 |  |  |  |  |
| Deconstruction | 67 | 29 | 113 | 26 | 27 | 33 | 19 | 5 | 5 |  |  |  |  |  |  |
| try | 7 | 19 |  | 80 | 30 | 10 | 111 | 5 | 28 | 1 | 11 | 5 |  | 1 |  |
| Named argument | 136 |  | 21 | 16 | 23 | 16 |  | 18 | 3 | 54 |  |  |  |  |  |
| for | 13 | 46 | 6 | 60 | 58 | 27 | 6 | 17 | 7 | 16 | 2 |  |  |  |  |
| Init accessor |  | 2 |  |  |  | 3 | 1 | 212 |  |  |  |  |  |  |  |
| while | 18 | 9 | 5 | 97 | 11 | 25 | 2 | 19 | 1 | 5 |  |  |  |  |  |
| typeof |  | 163 | 5 | 3 |  | 3 |  |  |  | 4 | 13 |  |  |  |  |
| Discard pattern |  | 14 | 1 | 15 | 27 | 67 | 20 | 28 | 9 | 1 |  | 2 |  |  |  |
| Switch expression |  | 12 | 1 | 15 | 27 | 65 | 19 | 28 | 9 | 1 |  | 2 |  |  |  |
| Spread element | 28 | 9 | 9 | 4 | 37 | 25 | 6 | 37 | 9 | 13 | 1 |  |  |  |  |
| throw expression | 4 | 4 | 34 | 6 | 73 | 1 | 16 | 3 | 8 | 26 | 1 |  |  |  |  |
| using statement | 36 |  | 2 | 116 | 7 |  |  |  |  |  | 1 |  |  |  |  |
| Field-like event |  | 61 |  |  |  |  | 93 |  | 4 |  |  |  |  |  |  |
| Discard _ | 22 | 25 | 59 | 3 | 8 | 9 | 2 | 10 | 6 | 1 | 1 |  |  |  |  |
| Case guard when |  | 6 |  | 1 | 3 | 100 |  | 3 |  | 2 |  |  |  |  |  |
| Interface |  | 2 |  |  | 1 |  | 1 | 67 | 27 |  |  |  |  |  |  |
| Relational pattern |  | 7 |  | 28 | 10 | 3 |  | 40 | 1 |  |  |  |  |  |  |
| Verbatim string | 5 |  |  |  | 3 | 77 |  |  |  | 2 |  |  |  |  |  |
| as cast |  | 58 |  | 3 |  | 9 |  |  |  | 15 |  |  |  |  |  |
| Range .. | 3 | 1 | 16 | 15 | 2 | 30 |  | 18 |  |  |  |  |  |  |  |
| Index from end ^ | 14 | 2 | 38 | 2 | 4 | 11 |  | 8 |  |  |  |  |  |  |  |
| Exception filter |  | 3 |  | 34 | 16 | 3 |  |  | 19 |  |  |  |  |  |  |
| Partial type |  |  | 4 |  |  | 7 |  |  |  | 26 |  |  | 35 |  |  |
| is type test | 1 | 21 |  | 2 | 3 | 32 |  |  |  |  |  |  |  |  |  |
| Switch statement |  | 5 |  | 8 |  | 20 | 2 | 19 | 1 | 2 |  |  |  |  |  |
| Generic method |  | 22 | 1 |  | 13 | 1 | 4 | 7 | 1 | 5 |  |  |  |  |  |
| yield return / break |  | 9 |  | 7 | 7 | 25 |  | 2 |  |  |  |  |  |  |  |
| Local const | 25 |  | 1 | 4 |  | 6 | 1 |  |  |  |  |  |  |  |  |
| finally | 6 | 1 |  | 2 | 11 | 2 | 4 |  | 4 |  | 3 | 2 |  |  |  |
| Enum |  | 2 |  |  |  |  | 11 | 19 |  |  |  |  |  |  |  |
| var pattern |  |  |  |  |  | 28 | 1 |  |  |  |  |  |  |  |  |
| ref or out parameter | 2 | 1 | 1 | 1 | 6 | 3 |  | 5 | 3 | 5 |  |  |  |  |  |
| Nested type | 8 | 6 | 1 |  |  |  |  |  |  | 10 |  |  |  |  |  |
| Null-coalescing assignment ??= |  | 10 |  | 1 | 4 | 3 |  |  |  |  |  | 1 |  |  |  |
| Constraint clause |  | 12 | 1 |  | 1 |  |  |  |  | 4 |  |  |  |  |  |
| Local function | 1 | 4 |  |  |  | 4 | 5 |  |  | 1 | 1 | 1 |  |  |  |
| Collection initializer |  | 3 | 2 | 5 | 2 |  |  |  |  | 2 | 2 |  |  |  |  |
| List pattern | 1 | 4 |  |  |  | 8 |  |  | 1 |  |  |  |  |  |  |
| Conversion operator |  | 12 |  |  |  |  |  | 1 |  |  |  |  |  |  |  |
| Type pattern |  | 1 |  |  |  | 10 |  |  |  |  |  |  |  |  |  |
| Default interface method body |  |  |  |  |  |  |  |  | 9 |  |  |  |  |  |  |
| Positional pattern |  | 2 |  | 1 | 1 | 1 | 4 |  |  |  |  |  |  |  |  |
| Async lambda |  |  | 4 |  | 1 |  |  |  |  |  |  | 2 |  |  |  |
| default literal |  |  | 1 | 1 |  | 5 |  |  |  |  |  |  |  |  |  |
| Required member |  | 2 |  |  |  | 3 |  |  |  |  |  |  |  |  |  |
| Event accessors |  | 3 |  |  |  |  |  |  | 1 |  |  |  |  |  |  |
| Primary constructor on class or struct |  |  |  |  |  | 4 |  |  |  |  |  |  |  |  |  |
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
| Object creation | 55.12 % | 48.74 % | 36.06 % | 25.18 % | 29.09 % | 42.00 % | 8.89 % | 25.74 % | 30.25 % | 31.72 % | 40.77 % | 7.69 % | - | 14.29 % | 21.05 % |
| Collection creation | 99.60 % | 94.87 % | 99.21 % | 98.00 % | 99.40 % | 99.05 % | 100.00 % | 100.00 % | 100.00 % | 99.56 % | 95.00 % | - | - | - | - |
| Member body | 2.28 % | 25.30 % | 2.90 % | 1.54 % | 10.90 % | 3.69 % | 17.34 % | 31.00 % | 16.36 % | 79.07 % | 33.33 % | 0.00 % | 0.00 % | 0.00 % | - |
| Local type | 0.00 % | 4.32 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.31 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | - | 0.00 % | 0.00 % |
| Using | 98.24 % | 100.00 % | 99.92 % | 83.31 % | 63.16 % | 100.00 % | - | - | - | 100.00 % | 96.77 % | - | - | - | - |
| Namespace | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - |
| Text building | 32.50 % | 52.78 % | 15.91 % | 58.09 % | 76.12 % | 82.07 % | 0.00 % | 29.17 % | - | 27.27 % | - | 0.00 % | - | 0.00 % | - |
| Branch | - | 70.59 % | 100.00 % | 65.22 % | 100.00 % | 76.47 % | 90.48 % | 59.57 % | 90.00 % | 33.33 % | - | 100.00 % | - | - | - |
| Lambda body | 96.80 % | 84.32 % | 79.51 % | 87.10 % | 92.22 % | 99.04 % | 97.08 % | 98.28 % | 91.43 % | 82.97 % | 40.00 % | 50.00 % | - | - | 77.78 % |
| Type test | 100.00 % | 91.13 % | 100.00 % | 90.91 % | 100.00 % | 94.51 % | 100.00 % | 100.00 % | 100.00 % | 40.00 % | 100.00 % | - | - | - | - |
| Constructor | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 36.36 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | - | 0.00 % | 0.00 % | - | - |
