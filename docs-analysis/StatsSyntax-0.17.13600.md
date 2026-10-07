# Syntax statistics - 0.17.13600

- Generated: 2026-10-07 23:46:24 +09:00
- Source roots: src, tests
- Root-level files: none
- Project segments: 1
- Excluded directories: .git, .vs, artifacts, bin, node_modules, obj, packages, publish
- Excluded suffixes: .g.cs, .g.i.cs, .AssemblyInfo.cs, .GlobalUsings.g.cs, .Designer.cs
- Roslyn: 4.14.0-3.25262.10
- Language version: Preview
- Scanned: 1,718 files, 15 projects, 189,919 lines

## Summary

| Measure | Value |
|---|---:|
| Files | 1,718 |
| Lines | 189,919 |
| Nodes | 1,002,322 |
| Syntax kinds used | 191 |
| Features used | 98 of 138 |
| Minimum C# version | 12 |
| Parse errors | 0 |

## Families

| Family | Features | Used | Count | Per 1k lines |
|---|---:|---:|---:|---:|
| Types | 15 | 12 | 3,668 | 19.31 |
| Namespaces | 6 | 2 | 1,718 | 9.05 |
| Members | 29 | 20 | 9,371 | 49.34 |
| Patterns | 16 | 16 | 5,898 | 31.06 |
| Expressions | 36 | 29 | 23,786 | 125.24 |
| Statements | 20 | 13 | 12,053 | 63.46 |
| Generics | 6 | 2 | 78 | 0.41 |
| Async | 2 | 2 | 439 | 2.31 |
| Nullability | 3 | 2 | 3,737 | 19.68 |
| Unsafe | 2 | 0 | 0 | 0.00 |
| Directives | 3 | 0 | 0 | 0.00 |
| Total | 138 | 98 | 60,748 | 319.86 |

## Projects

| Project | Files | Lines | Nodes | Features used | Newest C# |
|---|---:|---:|---:|---:|---:|
| Llyn.UIDeportment | 285 | 32,656 | 151,747 | 86 | 12 |
| Llyn.Tests.Engine | 195 | 31,937 | 224,500 | 59 | 12 |
| Llyn.Tests.Conduct | 125 | 24,175 | 174,502 | 60 | 12 |
| Llyn.Infrastructure | 156 | 21,929 | 99,611 | 71 | 12 |
| Llyn.Application | 110 | 17,695 | 81,909 | 70 | 12 |
| Llyn.Tests.Convention | 112 | 17,327 | 84,640 | 79 | 12 |
| Llyn.Conduct | 226 | 15,063 | 58,506 | 58 | 12 |
| Llyn.Core | 312 | 10,672 | 43,030 | 58 | 12 |
| Llyn.ShellEngine | 52 | 8,545 | 35,029 | 58 | 12 |
| Llyn.Tests.Interface | 87 | 7,920 | 39,257 | 64 | 12 |
| Llyn.Tests.Windows | 18 | 1,128 | 6,701 | 38 | 12 |
| Llyn.Core.Windows | 4 | 385 | 1,427 | 26 | 10 |
| Llyn.UIVeneer | 31 | 339 | 672 | 4 | 10 |
| Llyn.UIDeportment.Capsule | 4 | 88 | 380 | 15 | 10 |
| Llyn.Host | 1 | 60 | 411 | 3 | 9 |
| Total | 1,718 | 189,919 | 1,002,322 | 98 | 12 |

## Versions

| C# version | Features | Used | Count |
|---|---:|---:|---:|
| 1 | 51 | 35 | 16,007 |
| 2 | 10 | 8 | 1,992 |
| 3 | 7 | 5 | 5,858 |
| 4 | 3 | 2 | 711 |
| 5 | 3 | 3 | 1,031 |
| 6 | 6 | 5 | 5,149 |
| 7 | 11 | 10 | 5,703 |
| 7.1 | 1 | 1 | 7 |
| 7.2 | 1 | 0 | 0 |
| 7.3 | 1 | 0 | 0 |
| 8 | 17 | 12 | 9,948 |
| 9 | 10 | 9 | 5,593 |
| 10 | 3 | 2 | 1,719 |
| 11 | 6 | 3 | 604 |
| 12 | 4 | 3 | 6,426 |
| 13 | 1 | 0 | 0 |
| 14 | 3 | 0 | 0 |

## Features

| Feature | Family | C# | Count | Per 1k lines | Files | First |
|---|---|---|---:|---:|---:|---|
| Collection expression | Expressions | 12 | 6,256 | 32.94 | 783 | src/Llyn.Application/Card/LCardClerk.cs:57 |
| using declaration | Statements | 8 | 5,004 | 26.35 | 373 | src/Llyn.Application/Card/LTranslationClerk.cs:159 |
| Lambda | Expressions | 3 | 4,061 | 21.38 | 626 | src/Llyn.Application/Card/LCardClerk.cs:174 |
| if | Statements | 1 | 3,875 | 20.40 | 618 | src/Llyn.Application/Card/LCardClerk.cs:70 |
| Nullable reference type T? | Nullability | 8 | 3,213 | 16.92 | 854 | src/Llyn.Application/Card/LCardClerkField.cs:91 |
| Expression-bodied member | Members | 6 | 2,855 | 15.03 | 436 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Target-typed new | Expressions | 9 | 1,958 | 10.31 | 477 | src/Llyn.Application/Card/LMentionClerk.cs:167 |
| Declaration pattern | Patterns | 7 | 1,805 | 9.50 | 342 | src/Llyn.Application/Card/LMeaningClerk.cs:93 |
| File-scoped namespace | Namespaces | 10 | 1,717 | 9.04 | 1,717 | src/Llyn.Application/Card/LCardClerk.cs:5 |
| Readonly field | Members | 1 | 1,555 | 8.19 | 502 | src/Llyn.Application/Card/LCardClerk.cs:9 |
| Full property | Members | 1 | 1,429 | 7.52 | 317 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| foreach | Statements | 1 | 1,403 | 7.39 | 433 | src/Llyn.Application/Card/LCardClerk.cs:59 |
| Sealed class | Types | 1 | 1,365 | 7.19 | 1,262 | src/Llyn.Application/Card/LCardClerk.cs:7 |
| Tuple literal or tuple type | Expressions | 7 | 1,347 | 7.09 | 250 | src/Llyn.Application/Card/LMeaningClerk.cs:26 |
| Conditional ?: | Expressions | 1 | 1,278 | 6.73 | 481 | src/Llyn.Application/Card/LCardClerk.cs:173 |
| Class | Types | 1 | 1,237 | 6.51 | 1,219 | src/Llyn.Application/Card/LCardClerk.cs:7 |
| Null-conditional ?. ?[ | Expressions | 6 | 1,148 | 6.04 | 347 | src/Llyn.Application/Card/LMentionClerk.cs:47 |
| Static lambda | Expressions | 9 | 1,037 | 5.46 | 253 | src/Llyn.Application/Card/LCardEquality.cs:113 |
| Constant pattern | Patterns | 7 | 1,026 | 5.40 | 181 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Null-coalescing ?? | Expressions | 2 | 901 | 4.74 | 368 | src/Llyn.Application/Card/LCardClerkField.cs:74 |
| not pattern | Patterns | 9 | 835 | 4.40 | 332 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| Auto property | Members | 3 | 767 | 4.04 | 238 | src/Llyn.Application/Citation/LCitationClerk.cs:40 |
| Null-forgiving ! | Expressions | 8 | 732 | 3.85 | 268 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:97 |
| is null | Patterns | 7 | 728 | 3.83 | 315 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| Interpolated string | Expressions | 6 | 679 | 3.58 | 158 | src/Llyn.Application/Localization/LLocalizationReader.cs:55 |
| with expression | Expressions | 9 | 640 | 3.37 | 217 | src/Llyn.Application/Card/LCardClerk.cs:109 |
| await | Expressions | 5 | 592 | 3.12 | 139 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:139 |
| Extension method | Members | 3 | 589 | 3.10 | 41 | tests/Llyn.Tests.Interface/TInterfaceArea.cs:7 |
| Raw string | Expressions | 11 | 585 | 3.08 | 132 | src/Llyn.Infrastructure/Database/Base/LDatabase.cs:51 |
| Const field | Members | 1 | 558 | 2.94 | 233 | src/Llyn.Application/Card/LMentionClerk.cs:11 |
| Nullable value type T? | Nullability | 2 | 524 | 2.76 | 230 | src/Llyn.Application/Card/LMeaningClerk.cs:92 |
| Record class | Types | 9 | 483 | 2.54 | 395 | src/Llyn.Application/Ensign/LEnsignRow.cs:3 |
| Property pattern | Patterns | 8 | 455 | 2.40 | 108 | src/Llyn.Application/Draft/Clerk/LDraftClerk.cs:181 |
| Constructor | Members | 1 | 451 | 2.37 | 445 | src/Llyn.Application/Card/LCardClerk.cs:19 |
| Async method | Async | 5 | 432 | 2.27 | 139 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:91 |
| Object initializer | Expressions | 3 | 425 | 2.24 | 181 | src/Llyn.Application/Portrait/LPortraitClerk.cs:220 |
| Optional parameter | Members | 4 | 425 | 2.24 | 139 | src/Llyn.Application/Draft/Clerk/LDraftClerkPanel.cs:128 |
| Cast expression | Expressions | 1 | 412 | 2.17 | 148 | src/Llyn.Application/Markup/LMarkupClerkLink.cs:234 |
| nameof | Expressions | 6 | 392 | 2.06 | 130 | src/Llyn.Application/Citation/LAuthorClerk.cs:361 |
| catch | Statements | 1 | 344 | 1.81 | 141 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:142 |
| lock | Statements | 1 | 334 | 1.76 | 53 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:98 |
| and / or pattern | Patterns | 9 | 332 | 1.75 | 98 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Deconstruction | Expressions | 7 | 324 | 1.71 | 136 | src/Llyn.Application/Card/LMeaningClerk.cs:46 |
| Static class | Types | 2 | 312 | 1.64 | 312 | src/Llyn.Application/Card/LCardClerkField.cs:7 |
| try | Statements | 1 | 299 | 1.57 | 150 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:137 |
| Named argument | Members | 4 | 286 | 1.51 | 85 | src/Llyn.Application/Card/LTranslationClerk.cs:163 |
| for | Statements | 1 | 251 | 1.32 | 155 | src/Llyn.Application/Card/LCardClerk.cs:247 |
| Init accessor | Members | 9 | 217 | 1.14 | 73 | src/Llyn.Core/Catalog/Kind/LCatalogAuthor.cs:13 |
| while | Statements | 1 | 188 | 0.99 | 111 | src/Llyn.Application/Card/LMeaningClerk.cs:44 |
| typeof | Expressions | 1 | 186 | 0.98 | 42 | src/Llyn.Infrastructure/Configuration/LLocalizationLoader.cs:20 |
| throw expression | Expressions | 7 | 176 | 0.93 | 90 | src/Llyn.Application/Card/LCardClerkField.cs:75 |
| Discard pattern | Patterns | 8 | 173 | 0.91 | 101 | src/Llyn.Application/Citation/LAuthorClerk.cs:243 |
| Switch expression | Patterns | 8 | 168 | 0.88 | 101 | src/Llyn.Application/Citation/LAuthorClerk.cs:239 |
| Spread element | Expressions | 12 | 166 | 0.87 | 90 | src/Llyn.Application/Card/LCardEquality.cs:53 |
| using statement | Statements | 1 | 161 | 0.85 | 67 | src/Llyn.Application/Citation/LAuthorCitation.cs:51 |
| Field-like event | Members | 1 | 149 | 0.78 | 83 | src/Llyn.Conduct/Atelier/CNavigation.cs:46 |
| Discard _ | Expressions | 7 | 136 | 0.72 | 47 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:60 |
| Partial type | Types | 2 | 125 | 0.66 | 125 | src/Llyn.Infrastructure/Database/Context/LSituationArchive.cs:8 |
| Case guard when | Patterns | 7 | 115 | 0.61 | 24 | src/Llyn.Application/Citation/LAuthorClerk.cs:241 |
| Verbatim string | Expressions | 1 | 88 | 0.46 | 19 | src/Llyn.Application/Localization/LLocalizationReader.cs:13 |
| Range .. | Expressions | 8 | 84 | 0.44 | 40 | src/Llyn.Application/Card/LMentionClerk.cs:173 |
| Relational pattern | Patterns | 9 | 80 | 0.42 | 28 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Interface | Types | 1 | 76 | 0.40 | 76 | src/Llyn.Application/Pronunciation/Relay/LListener.cs:5 |
| Exception filter | Statements | 6 | 75 | 0.39 | 45 | src/Llyn.Application/Outpost/LCourierClerk.cs:107 |
| as cast | Expressions | 1 | 70 | 0.37 | 46 | src/Llyn.Infrastructure/Database/Base/LWorkspaceArchive.cs:132 |
| Index from end ^ | Expressions | 8 | 70 | 0.37 | 40 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:230 |
| is type test | Patterns | 1 | 59 | 0.31 | 36 | src/Llyn.Application/Outpost/LCourierClerk.cs:363 |
| Switch statement | Patterns | 1 | 59 | 0.31 | 35 | src/Llyn.Conduct/Configuration/CLedger.cs:240 |
| Generic method | Generics | 2 | 58 | 0.31 | 30 | src/Llyn.Application/Card/LCardClerkField.cs:183 |
| yield return / break | Statements | 2 | 50 | 0.26 | 19 | src/Llyn.Application/Card/LCardClerkField.cs:35 |
| Local const | Statements | 1 | 37 | 0.19 | 21 | src/Llyn.Conduct/Lexicon/CStateWording.cs:10 |
| Enum | Types | 1 | 32 | 0.17 | 32 | src/Llyn.Conduct/Catalog/CCatalogOrder.cs:3 |
| finally | Statements | 1 | 32 | 0.17 | 25 | src/Llyn.Application/Outpost/LCourierClerk.cs:83 |
| var pattern | Patterns | 7 | 29 | 0.15 | 11 | src/Llyn.Conduct/Card/CFolio.cs:193 |
| Nested type | Types | 1 | 28 | 0.15 | 17 | src/Llyn.UIDeportment/Kit/Bind/QLook.cs:19 |
| ref or out parameter | Members | 1 | 26 | 0.14 | 20 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:101 |
| Constraint clause | Generics | 2 | 20 | 0.11 | 10 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:171 |
| Default interface method body | Members | 8 | 20 | 0.11 | 6 | src/Llyn.ShellEngine/Port/LDraftPort.cs:58 |
| Null-coalescing assignment ??= | Expressions | 8 | 18 | 0.09 | 16 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:160 |
| Local function | Members | 7 | 17 | 0.09 | 11 | src/Llyn.Conduct/Desk/CErrand.cs:54 |
| Collection initializer | Expressions | 3 | 16 | 0.08 | 12 | src/Llyn.Application/Outpost/LCourierClerk.cs:253 |
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
| Null check | is null / is not null | 728 | == null / != null | 0 | 100.00 % |
| Object creation | Target-typed new() | 1,958 | new T() | 3,511 | 35.80 % |
| Collection creation | Collection expression | 6,256 | Collection initializer or array creation | 51 | 99.19 % |
| Member body | Expression body | 2,855 | Block body | 9,610 | 22.90 % |
| Local type | var | 42 | Explicit type | 19,384 | 0.22 % |
| Using | Declaration | 5,004 | Statement | 161 | 96.88 % |
| Namespace | File-scoped | 1,717 | Block | 0 | 100.00 % |
| Text building | Interpolated string | 679 | string.Format or + with a string literal | 418 | 61.90 % |
| Branch | Switch expression | 168 | Switch statement | 59 | 74.01 % |
| Lambda body | Expression | 3,640 | Block | 421 | 89.63 % |
| Type test | is pattern with designation | 1,012 | as | 70 | 93.53 % |
| Constructor | Primary | 4 | Explicit | 445 | 0.89 % |

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
| IdentifierName | 288,714 | 1,718 |
| Argument | 104,635 | 1,308 |
| SimpleMemberAccessExpression | 94,507 | 1,302 |
| ArgumentList | 65,007 | 1,341 |
| InvocationExpression | 59,665 | 1,310 |
| PredefinedType | 29,256 | 1,618 |
| ExpressionStatement | 28,340 | 1,197 |
| StringLiteralExpression | 23,472 | 968 |
| Parameter | 20,868 | 1,548 |
| VariableDeclaration | 19,864 | 1,132 |
| VariableDeclarator | 19,864 | 1,132 |
| EqualsValueClause | 19,143 | 1,195 |
| Block | 17,017 | 1,265 |
| LocalDeclarationStatement | 16,767 | 986 |
| ParameterList | 12,849 | 1,647 |
| MethodDeclaration | 10,947 | 1,305 |
| NumericLiteralExpression | 10,407 | 981 |
| GenericName | 9,961 | 1,167 |
| TypeArgumentList | 9,961 | 1,167 |
| QualifiedName | 8,066 | 1,718 |
| SimpleAssignmentExpression | 6,799 | 889 |
| ExpressionElement | 6,750 | 427 |
| CollectionExpression | 6,256 | 783 |
| ReturnStatement | 5,583 | 950 |
| UsingDirective | 5,408 | 1,464 |
| IfStatement | 3,875 | 618 |
| NullLiteralExpression | 3,838 | 782 |
| NullableType | 3,737 | 894 |
| ObjectCreationExpression | 3,511 | 754 |
| Attribute | 3,170 | 371 |
| AttributeList | 3,170 | 371 |
| SimpleLambdaExpression | 3,130 | 560 |
| SingleVariableDesignation | 3,050 | 508 |
| ArrowExpressionClause | 2,857 | 437 |
| BracketedArgumentList | 2,825 | 499 |
| FieldDeclaration | 2,541 | 663 |
| IsPatternExpression | 2,511 | 516 |
| ElementAccessExpression | 2,223 | 445 |
| PropertyDeclaration | 2,200 | 447 |
| ImplicitObjectCreationExpression | 1,958 | 477 |
| FalseLiteralExpression | 1,907 | 517 |
| DeclarationPattern | 1,805 | 342 |
| ConstantPattern | 1,754 | 405 |
| CompilationUnit | 1,718 | 1,718 |
| FileScopedNamespaceDeclaration | 1,717 | 1,717 |
| TrueLiteralExpression | 1,527 | 452 |
| InterpolatedStringText | 1,488 | 158 |
| EqualsExpression | 1,473 | 528 |
| TupleExpression | 1,350 | 255 |
| AddExpression | 1,324 | 314 |
| ForEachStatement | 1,321 | 420 |
| Interpolation | 1,313 | 157 |
| ConditionalExpression | 1,278 | 481 |
| LogicalAndExpression | 1,272 | 348 |
| ClassDeclaration | 1,237 | 1,219 |
| ConditionalAccessExpression | 1,148 | 347 |
| MemberBindingExpression | 1,148 | 347 |
| LogicalNotExpression | 1,127 | 371 |
| AddAssignmentExpression | 1,101 | 238 |
| DeclarationExpression | 968 | 252 |
| SwitchExpressionArm | 942 | 101 |
| ParenthesizedLambdaExpression | 931 | 264 |
| AttributeArgument | 928 | 52 |
| CoalesceExpression | 901 | 368 |
| LogicalOrExpression | 894 | 282 |
| AccessorList | 888 | 262 |
| GetAccessorDeclaration | 883 | 260 |
| NotPattern | 835 | 332 |
| TupleElement | 754 | 180 |
| SuppressNullableWarningExpression | 732 | 268 |
| InterpolatedStringExpression | 679 | 158 |
| NameColon | 649 | 183 |
| WithExpression | 640 | 217 |
| WithInitializerExpression | 640 | 217 |
| GreaterThanExpression | 638 | 278 |
| ArrayRankSpecifier | 604 | 236 |
| ArrayType | 603 | 236 |
| ImplicitElementAccess | 602 | 111 |
| AwaitExpression | 592 | 139 |
| OmittedArraySizeExpression | 587 | 236 |
| PostIncrementExpression | 498 | 224 |
| CharacterLiteralExpression | 497 | 112 |
| RecordDeclaration | 483 | 395 |
| LessThanExpression | 464 | 208 |
| RecursivePattern | 464 | 115 |
| PropertyPatternClause | 455 | 108 |
| ConstructorDeclaration | 453 | 447 |
| ObjectInitializerExpression | 425 | 181 |
| CastExpression | 412 | 148 |
| Subpattern | 412 | 107 |
| NotEqualsExpression | 402 | 170 |
| AttributeArgumentList | 396 | 52 |
| ParenthesizedExpression | 346 | 173 |
| CatchClause | 344 | 141 |
| BaseList | 341 | 247 |
| CatchDeclaration | 341 | 139 |
| LockStatement | 334 | 53 |
| TupleType | 322 | 180 |
| SwitchSection | 319 | 35 |
| BreakStatement | 305 | 60 |
| ContinueStatement | 300 | 146 |
| TryStatement | 299 | 150 |
| OrPattern | 290 | 92 |
| ForStatement | 251 | 155 |
| SimpleBaseType | 242 | 228 |
| EnumMemberDeclaration | 218 | 32 |
| InitAccessorDeclaration | 217 | 73 |
| ThrowStatement | 197 | 96 |
| SubtractExpression | 188 | 87 |
| WhileStatement | 188 | 111 |
| TypeOfExpression | 186 | 42 |
| ThrowExpression | 176 | 90 |
| CaseSwitchLabel | 173 | 23 |
| DiscardPattern | 173 | 101 |
| SwitchExpression | 168 | 101 |
| SpreadElement | 166 | 90 |
| UsingStatement | 161 | 67 |
| UnaryMinusExpression | 159 | 70 |
| SubtractAssignmentExpression | 158 | 67 |
| SetAccessorDeclaration | 152 | 70 |
| ThisExpression | 152 | 81 |
| EventFieldDeclaration | 149 | 83 |
| ElseClause | 141 | 114 |
| WhenClause | 115 | 24 |
| LessThanOrEqualExpression | 112 | 80 |
| CasePatternSwitchLabel | 111 | 20 |
| PrimaryConstructorBaseType | 103 | 19 |
| GreaterThanOrEqualExpression | 98 | 64 |
| RangeExpression | 84 | 40 |
| ForEachVariableStatement | 82 | 57 |
| MultiplyExpression | 81 | 32 |
| RelationalPattern | 80 | 28 |
| InterfaceDeclaration | 76 | 76 |
| CatchFilterClause | 75 | 45 |
| AsExpression | 70 | 46 |
| IndexExpression | 70 | 40 |
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
| ImplicitArrayCreationExpression | 33 | 27 |
| EnumDeclaration | 32 | 32 |
| FinallyClause | 32 | 25 |
| LeftShiftExpression | 32 | 2 |
| DivideExpression | 31 | 18 |
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
| Fact | 2,697 | 357 |
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
| Collection expression | 149 | 2,609 | 1,591 | 275 | 329 | 523 | 158 | 265 | 82 | 214 | 61 |  |  |  |  |
| using declaration | 5 | 1,948 | 2,417 | 576 | 12 | 7 |  |  |  | 18 | 21 |  |  |  |  |
| Lambda | 182 | 791 | 1,259 | 28 | 334 | 730 | 321 | 162 | 63 | 147 | 32 | 4 |  |  | 8 |
| if | 1,194 | 49 | 6 | 636 | 681 | 464 | 369 | 222 | 209 | 29 | 3 | 10 |  | 3 |  |
| Nullable reference type T? | 457 | 148 | 355 | 319 | 319 | 297 | 310 | 320 | 255 | 393 | 29 | 7 |  | 4 |  |
| Expression-bodied member | 938 | 34 | 35 | 6 | 110 | 26 | 247 | 153 | 208 | 1,070 | 28 |  |  |  |  |
| Target-typed new | 717 | 144 | 198 | 129 | 150 | 189 | 35 | 50 | 75 | 231 | 34 | 1 |  | 1 | 4 |
| Declaration pattern | 903 | 5 | 4 | 40 | 194 | 368 | 187 | 22 | 71 | 8 | 3 |  |  |  |  |
| File-scoped namespace | 285 | 195 | 125 | 156 | 110 | 112 | 226 | 312 | 52 | 87 | 18 | 4 | 31 | 4 |  |
| Readonly field | 433 | 37 | 4 | 105 | 374 | 215 | 229 | 28 | 90 | 35 |  | 4 |  | 1 |  |
| Full property | 878 | 7 | 4 | 6 | 110 | 2 | 232 | 141 | 38 | 11 |  |  |  |  |  |
| foreach | 185 | 52 | 18 | 275 | 317 | 306 | 25 | 149 | 35 | 40 | 1 |  |  |  |  |
| Sealed class | 236 | 200 | 128 | 72 | 172 | 43 | 207 | 206 | 46 | 32 | 15 | 4 |  | 4 |  |
| Tuple literal or tuple type | 39 | 411 | 267 | 170 | 81 | 168 | 56 | 44 | 76 | 25 | 10 |  |  |  |  |
| Conditional ?: | 217 | 13 | 16 | 270 | 243 | 156 | 142 | 128 | 61 | 28 | 2 | 2 |  |  |  |
| Class | 272 | 203 | 126 | 154 | 89 | 105 | 70 | 24 | 45 | 95 | 18 | 4 | 31 | 1 |  |
| Null-conditional ?. ?[ | 175 | 129 | 72 | 58 | 47 | 91 | 444 | 37 | 79 | 9 | 6 | 1 |  |  |  |
| Static lambda | 78 | 78 | 528 | 4 | 114 | 15 | 62 | 87 | 14 | 54 | 3 |  |  |  |  |
| Constant pattern | 133 | 3 | 8 | 142 | 54 | 353 | 130 | 138 | 43 | 17 |  | 5 |  |  |  |
| Null-coalescing ?? | 79 | 18 | 12 | 102 | 175 | 80 | 91 | 254 | 41 | 43 | 5 |  |  | 1 |  |
| not pattern | 308 | 4 | 1 | 52 | 69 | 185 | 129 | 30 | 50 | 6 | 1 |  |  |  |  |
| Auto property | 346 | 5 | 3 | 8 | 7 | 23 | 81 | 229 | 44 | 21 |  |  |  |  |  |
| Null-forgiving ! | 168 | 212 | 207 | 56 | 9 | 25 | 5 |  | 3 | 46 | 1 |  |  |  |  |
| is null | 122 | 8 | 5 | 91 | 152 | 175 | 71 | 37 | 52 | 13 | 2 |  |  |  |  |
| Interpolated string | 19 | 51 | 7 | 162 | 51 | 380 |  | 6 |  | 3 |  |  |  |  |  |
| with expression | 7 | 214 | 31 | 42 | 243 | 10 | 15 | 17 | 36 | 25 |  |  |  |  |  |
| await | 57 | 228 | 142 | 62 | 56 | 1 | 13 |  | 17 | 5 | 1 | 10 |  |  |  |
| Extension method |  |  |  |  |  |  |  |  |  | 581 | 8 |  |  |  |  |
| Raw string | 2 | 121 | 7 | 214 |  | 233 |  |  |  | 8 |  |  |  |  |  |
| Const field | 75 | 69 | 14 | 179 | 14 | 113 | 3 | 70 | 4 | 12 |  | 4 |  | 1 |  |
| Nullable value type T? | 56 | 20 | 21 | 42 | 33 | 11 | 135 | 30 | 101 | 66 | 7 |  |  | 2 |  |
| Record class | 16 |  | 2 | 2 | 104 | 6 | 144 | 203 | 1 | 2 |  |  |  | 3 |  |
| Property pattern | 143 | 2 |  | 8 | 15 | 270 | 8 | 3 | 4 | 1 | 1 |  |  |  |  |
| Constructor | 167 | 2 | 1 | 62 | 68 | 7 | 60 | 3 | 41 | 7 |  | 2 | 31 |  |  |
| Async method | 56 | 180 | 97 | 32 | 29 | 1 | 13 |  | 13 | 5 | 1 | 5 |  |  |  |
| Object initializer | 135 | 55 | 94 | 29 | 1 | 60 |  | 3 |  | 33 | 13 | 1 |  | 1 |  |
| Optional parameter | 8 | 12 | 6 | 6 | 11 | 1 | 4 | 256 | 16 | 102 |  |  |  | 3 |  |
| Cast expression | 137 | 7 | 83 | 79 | 4 | 50 | 3 | 4 |  | 29 | 15 | 1 |  |  |  |
| nameof | 218 | 1 | 3 | 8 | 117 | 17 | 18 | 3 | 4 | 2 | 1 |  |  |  |  |
| catch | 21 | 1 |  | 126 | 33 | 8 | 109 | 1 | 27 | 1 | 8 | 6 |  | 3 |  |
| lock | 13 |  |  | 2 | 77 | 5 | 2 |  | 230 | 5 |  |  |  |  |  |
| and / or pattern | 37 | 1 | 3 | 69 | 15 | 159 | 2 | 39 | 1 | 6 |  |  |  |  |  |
| Deconstruction | 28 | 66 | 113 | 26 | 27 | 33 | 19 | 5 | 7 |  |  |  |  |  |  |
| Static class | 44 | 3 |  | 84 | 20 | 68 | 7 | 21 |  | 62 | 3 |  |  |  |  |
| try | 19 | 7 |  | 78 | 30 | 10 | 111 | 1 | 28 | 1 | 8 | 5 |  | 1 |  |
| Named argument |  | 136 | 25 | 16 | 23 | 16 |  | 18 | 3 | 49 |  |  |  |  |  |
| for | 44 | 11 | 6 | 60 | 56 | 27 | 6 | 14 | 9 | 16 | 2 |  |  |  |  |
| Init accessor | 3 |  |  |  |  | 3 |  | 211 |  |  |  |  |  |  |  |
| while | 9 | 16 | 5 | 97 | 11 | 25 | 2 | 17 | 1 | 5 |  |  |  |  |  |
| typeof | 161 |  | 5 | 3 |  | 3 |  |  |  | 4 | 10 |  |  |  |  |
| throw expression | 4 | 4 | 34 | 6 | 73 | 1 | 16 | 3 | 8 | 26 | 1 |  |  |  |  |
| Discard pattern | 14 |  | 1 | 15 | 16 | 67 | 20 | 28 | 9 | 1 |  | 2 |  |  |  |
| Switch expression | 12 |  | 1 | 15 | 16 | 65 | 19 | 28 | 9 | 1 |  | 2 |  |  |  |
| Spread element | 9 | 19 | 8 | 2 | 35 | 25 | 6 | 36 | 12 | 13 | 1 |  |  |  |  |
| using statement |  | 36 | 2 | 115 | 7 |  |  |  |  |  | 1 |  |  |  |  |
| Field-like event | 53 |  |  |  |  |  | 92 |  | 4 |  |  |  |  |  |  |
| Discard _ | 20 | 22 | 58 | 1 | 8 | 9 | 2 | 9 | 6 | 1 |  |  |  |  |  |
| Partial type | 48 |  | 4 | 5 |  | 7 |  |  | 4 | 26 |  |  | 31 |  |  |
| Case guard when | 6 |  |  | 1 | 3 | 100 |  | 3 |  | 2 |  |  |  |  |  |
| Verbatim string |  | 5 |  |  | 3 | 78 |  |  |  | 2 |  |  |  |  |  |
| Range .. |  | 3 | 16 | 15 | 2 | 30 |  | 18 |  |  |  |  |  |  |  |
| Relational pattern | 1 |  |  | 25 | 10 | 3 |  | 40 | 1 |  |  |  |  |  |  |
| Interface | 2 |  |  |  | 1 |  | 1 | 66 | 6 |  |  |  |  |  |  |
| Exception filter | 3 |  |  | 34 | 16 | 3 |  |  | 19 |  |  |  |  |  |  |
| as cast | 58 |  |  | 3 |  | 9 |  |  |  |  |  |  |  |  |  |
| Index from end ^ | 2 | 9 | 38 | 2 | 4 | 11 |  | 4 |  |  |  |  |  |  |  |
| is type test | 21 | 1 |  | 2 | 3 | 32 |  |  |  |  |  |  |  |  |  |
| Switch statement | 7 |  |  | 8 |  | 20 | 2 | 19 | 1 | 2 |  |  |  |  |  |
| Generic method | 25 |  | 1 |  | 13 | 1 | 4 | 7 | 2 | 5 |  |  |  |  |  |
| yield return / break | 9 |  |  | 7 | 7 | 25 |  | 2 |  |  |  |  |  |  |  |
| Local const |  | 25 | 1 | 4 |  | 6 | 1 |  |  |  |  |  |  |  |  |
| Enum | 2 |  |  |  |  |  | 11 | 19 |  |  |  |  |  |  |  |
| finally | 1 | 6 |  | 2 | 11 | 2 | 4 |  | 4 |  |  | 2 |  |  |  |
| var pattern |  |  |  |  |  | 28 | 1 |  |  |  |  |  |  |  |  |
| Nested type | 7 | 8 | 3 |  |  |  |  |  |  | 10 |  |  |  |  |  |
| ref or out parameter | 2 | 2 | 1 | 1 | 6 | 3 |  | 4 | 3 | 4 |  |  |  |  |  |
| Constraint clause | 14 |  | 1 |  | 1 |  |  |  |  | 4 |  |  |  |  |  |
| Default interface method body |  |  |  |  |  |  |  |  | 20 |  |  |  |  |  |  |
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
| Object creation | 50.92 % | 50.88 % | 33.85 % | 24.11 % | 28.90 % | 42.00 % | 9.86 % | 26.74 % | 29.41 % | 30.00 % | 40.96 % | 7.69 % | - | 14.29 % | 21.05 % |
| Collection creation | 94.90 % | 99.58 % | 99.13 % | 97.86 % | 99.40 % | 99.05 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 92.42 % | - | - | - | - |
| Member body | 32.28 % | 2.08 % | 2.76 % | 0.63 % | 11.21 % | 3.69 % | 20.30 % | 30.42 % | 25.30 % | 79.08 % | 37.84 % | 0.00 % | 0.00 % | 0.00 % | - |
| Local type | 4.56 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.31 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | - | 0.00 % | 0.00 % |
| Using | 100.00 % | 98.19 % | 99.92 % | 83.36 % | 63.16 % | 100.00 % | - | - | - | 100.00 % | 95.45 % | - | - | - | - |
| Namespace | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - |
| Text building | 55.88 % | 33.12 % | 15.91 % | 58.70 % | 76.12 % | 82.07 % | 0.00 % | 26.09 % | - | 27.27 % | - | 0.00 % | - | 0.00 % | - |
| Branch | 63.16 % | - | 100.00 % | 65.22 % | 100.00 % | 76.47 % | 90.48 % | 59.57 % | 90.00 % | 33.33 % | - | 100.00 % | - | - | - |
| Lambda body | 82.97 % | 96.59 % | 79.43 % | 85.71 % | 91.92 % | 99.04 % | 98.44 % | 98.15 % | 90.48 % | 79.59 % | 43.75 % | 50.00 % | - | - | 75.00 % |
| Type test | 91.13 % | 100.00 % | 100.00 % | 90.63 % | 100.00 % | 94.51 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - | - | - | - |
| Constructor | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 36.36 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | - | 0.00 % | 0.00 % | - | - |
