# Syntax statistics - 0.17.13707

- Generated: 2026-10-08 20:07:43 +09:00
- Source roots: src, tests
- Root-level files: none
- Project segments: 1
- Excluded directories: .git, .vs, artifacts, bin, node_modules, obj, packages, publish
- Excluded suffixes: .g.cs, .g.i.cs, .AssemblyInfo.cs, .GlobalUsings.g.cs, .Designer.cs
- Roslyn: 4.14.0-3.25262.10
- Language version: Preview
- Scanned: 1,765 files, 15 projects, 191,747 lines

## Summary

| Measure | Value |
|---|---:|
| Files | 1,765 |
| Lines | 191,747 |
| Nodes | 1,010,078 |
| Syntax kinds used | 191 |
| Features used | 98 of 138 |
| Minimum C# version | 12 |
| Parse errors | 0 |

## Families

| Family | Features | Used | Count | Per 1k lines |
|---|---:|---:|---:|---:|
| Types | 15 | 12 | 3,729 | 19.45 |
| Namespaces | 6 | 2 | 1,765 | 9.20 |
| Members | 29 | 20 | 9,441 | 49.24 |
| Patterns | 16 | 16 | 5,963 | 31.10 |
| Expressions | 36 | 29 | 23,932 | 124.81 |
| Statements | 20 | 13 | 12,100 | 63.10 |
| Generics | 6 | 2 | 78 | 0.41 |
| Async | 2 | 2 | 441 | 2.30 |
| Nullability | 3 | 2 | 3,770 | 19.66 |
| Unsafe | 2 | 0 | 0 | 0.00 |
| Directives | 3 | 0 | 0 | 0.00 |
| Total | 138 | 98 | 61,219 | 319.27 |

## Projects

| Project | Files | Lines | Nodes | Features used | Newest C# |
|---|---:|---:|---:|---:|---:|
| Llyn.UIDeportment | 284 | 32,683 | 151,799 | 86 | 12 |
| Llyn.Tests.Engine | 196 | 31,998 | 225,064 | 59 | 12 |
| Llyn.Tests.Conduct | 125 | 24,333 | 175,835 | 60 | 12 |
| Llyn.Infrastructure | 157 | 21,963 | 99,672 | 71 | 12 |
| Llyn.Application | 119 | 18,021 | 82,792 | 70 | 12 |
| Llyn.Tests.Convention | 112 | 17,357 | 84,684 | 79 | 12 |
| Llyn.Conduct | 229 | 15,600 | 60,496 | 59 | 12 |
| Llyn.Core | 312 | 10,672 | 43,030 | 58 | 12 |
| Llyn.ShellEngine | 85 | 8,925 | 35,894 | 57 | 12 |
| Llyn.Tests.Interface | 87 | 8,124 | 40,830 | 65 | 12 |
| Llyn.Tests.Windows | 19 | 1,174 | 6,984 | 39 | 12 |
| Llyn.Core.Windows | 4 | 385 | 1,427 | 26 | 10 |
| Llyn.UIVeneer | 31 | 339 | 672 | 4 | 10 |
| Llyn.UIDeportment.Capsule | 4 | 88 | 380 | 15 | 10 |
| Llyn.Host | 1 | 85 | 519 | 4 | 9 |
| Total | 1,765 | 191,747 | 1,010,078 | 98 | 12 |

## Versions

| C# version | Features | Used | Count |
|---|---:|---:|---:|
| 1 | 51 | 35 | 16,229 |
| 2 | 10 | 8 | 2,019 |
| 3 | 7 | 5 | 5,929 |
| 4 | 3 | 2 | 707 |
| 5 | 3 | 3 | 1,035 |
| 6 | 6 | 5 | 5,056 |
| 7 | 11 | 10 | 5,737 |
| 7.1 | 1 | 1 | 7 |
| 7.2 | 1 | 0 | 0 |
| 7.3 | 1 | 0 | 0 |
| 8 | 17 | 12 | 10,047 |
| 9 | 10 | 9 | 5,645 |
| 10 | 3 | 2 | 1,766 |
| 11 | 6 | 3 | 603 |
| 12 | 4 | 3 | 6,439 |
| 13 | 1 | 0 | 0 |
| 14 | 3 | 0 | 0 |

## Features

| Feature | Family | C# | Count | Per 1k lines | Files | First |
|---|---|---|---:|---:|---:|---|
| Collection expression | Expressions | 12 | 6,272 | 32.71 | 794 | src/Llyn.Application/Card/LCardClerk.cs:57 |
| using declaration | Statements | 8 | 5,044 | 26.31 | 373 | src/Llyn.Application/Card/LTranslationClerk.cs:158 |
| Lambda | Expressions | 3 | 4,097 | 21.37 | 630 | src/Llyn.Application/Card/LCardClerk.cs:174 |
| if | Statements | 1 | 3,885 | 20.26 | 631 | src/Llyn.Application/Card/LCardClerk.cs:70 |
| Nullable reference type T? | Nullability | 8 | 3,235 | 16.87 | 873 | src/Llyn.Application/Card/LCardClerkField.cs:91 |
| Expression-bodied member | Members | 6 | 2,791 | 14.56 | 435 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Target-typed new | Expressions | 9 | 1,983 | 10.34 | 488 | src/Llyn.Application/Card/LMentionClerk.cs:167 |
| Declaration pattern | Patterns | 7 | 1,836 | 9.58 | 352 | src/Llyn.Application/Card/LMeaningClerk.cs:93 |
| File-scoped namespace | Namespaces | 10 | 1,764 | 9.20 | 1,764 | src/Llyn.Application/Card/LCardClerk.cs:5 |
| Readonly field | Members | 1 | 1,634 | 8.52 | 524 | src/Llyn.Application/Card/LCardClerk.cs:9 |
| Full property | Members | 1 | 1,439 | 7.50 | 317 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| foreach | Statements | 1 | 1,400 | 7.30 | 439 | src/Llyn.Application/Card/LCardClerk.cs:59 |
| Sealed class | Types | 1 | 1,390 | 7.25 | 1,287 | src/Llyn.Application/Card/LCardClerk.cs:7 |
| Tuple literal or tuple type | Expressions | 7 | 1,347 | 7.02 | 255 | src/Llyn.Application/Card/LMeaningClerk.cs:26 |
| Conditional ?: | Expressions | 1 | 1,292 | 6.74 | 486 | src/Llyn.Application/Card/LCardClerk.cs:173 |
| Class | Types | 1 | 1,257 | 6.56 | 1,239 | src/Llyn.Application/Card/LCardClerk.cs:7 |
| Null-conditional ?. ?[ | Expressions | 6 | 1,123 | 5.86 | 353 | src/Llyn.Application/Card/LMentionClerk.cs:47 |
| Static lambda | Expressions | 9 | 1,044 | 5.44 | 255 | src/Llyn.Application/Card/LCardEquality.cs:113 |
| Constant pattern | Patterns | 7 | 1,022 | 5.33 | 177 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Null-coalescing ?? | Expressions | 2 | 928 | 4.84 | 376 | src/Llyn.Application/Card/LCardClerkField.cs:74 |
| not pattern | Patterns | 9 | 842 | 4.39 | 337 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| Auto property | Members | 3 | 796 | 4.15 | 242 | src/Llyn.Application/Citation/LCitationClerk.cs:41 |
| Null-forgiving ! | Expressions | 8 | 754 | 3.93 | 268 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:145 |
| is null | Patterns | 7 | 737 | 3.84 | 322 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| Interpolated string | Expressions | 6 | 675 | 3.52 | 157 | src/Llyn.Application/Localization/LLocalizationReader.cs:55 |
| with expression | Expressions | 9 | 646 | 3.37 | 221 | src/Llyn.Application/Card/LCardClerk.cs:109 |
| await | Expressions | 5 | 594 | 3.10 | 139 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:139 |
| Extension method | Members | 3 | 594 | 3.10 | 41 | tests/Llyn.Tests.Interface/TInterfaceArea.cs:7 |
| Raw string | Expressions | 11 | 584 | 3.05 | 131 | src/Llyn.Infrastructure/Database/Base/LDatabase.cs:51 |
| Const field | Members | 1 | 558 | 2.91 | 233 | src/Llyn.Application/Card/LMentionClerk.cs:11 |
| Nullable value type T? | Nullability | 2 | 535 | 2.79 | 234 | src/Llyn.Application/Card/LMeaningClerk.cs:92 |
| Record class | Types | 9 | 489 | 2.55 | 401 | src/Llyn.Application/Ensign/LEnsignRow.cs:3 |
| Constructor | Members | 1 | 476 | 2.48 | 470 | src/Llyn.Application/Card/LCardClerk.cs:19 |
| Property pattern | Patterns | 8 | 455 | 2.37 | 108 | src/Llyn.Application/Draft/Clerk/LDraftClerk.cs:79 |
| Async method | Async | 5 | 434 | 2.26 | 139 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:91 |
| Object initializer | Expressions | 3 | 426 | 2.22 | 182 | src/Llyn.Application/Portrait/LPortraitClerkLabel.cs:25 |
| Optional parameter | Members | 4 | 421 | 2.20 | 139 | src/Llyn.Application/Draft/Clerk/LDraftClerkPanel.cs:170 |
| Cast expression | Expressions | 1 | 418 | 2.18 | 148 | src/Llyn.Application/Markup/LMarkupClerkLink.cs:234 |
| nameof | Expressions | 6 | 392 | 2.04 | 131 | src/Llyn.Application/Citation/LAuthorClerk.cs:361 |
| catch | Statements | 1 | 344 | 1.79 | 143 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:142 |
| lock | Statements | 1 | 335 | 1.75 | 53 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:98 |
| and / or pattern | Patterns | 9 | 332 | 1.73 | 97 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Deconstruction | Expressions | 7 | 322 | 1.68 | 135 | src/Llyn.Application/Card/LMeaningClerk.cs:46 |
| Static class | Types | 2 | 313 | 1.63 | 313 | src/Llyn.Application/Card/LCardClerkField.cs:7 |
| try | Statements | 1 | 300 | 1.56 | 153 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:137 |
| Named argument | Members | 4 | 286 | 1.49 | 86 | src/Llyn.Application/Card/LTranslationClerk.cs:162 |
| for | Statements | 1 | 248 | 1.29 | 153 | src/Llyn.Application/Card/LCardClerk.cs:247 |
| Init accessor | Members | 9 | 218 | 1.14 | 74 | src/Llyn.Conduct/Lexicon/CExample.cs:20 |
| while | Statements | 1 | 188 | 0.98 | 109 | src/Llyn.Application/Card/LMeaningClerk.cs:44 |
| typeof | Expressions | 1 | 186 | 0.97 | 42 | src/Llyn.Infrastructure/Configuration/LLocalizationLoader.cs:20 |
| Discard pattern | Patterns | 8 | 184 | 0.96 | 109 | src/Llyn.Application/Citation/LAuthorClerk.cs:243 |
| Switch expression | Patterns | 8 | 179 | 0.93 | 109 | src/Llyn.Application/Citation/LAuthorClerk.cs:239 |
| throw expression | Expressions | 7 | 175 | 0.91 | 89 | src/Llyn.Application/Card/LCardClerkField.cs:75 |
| Spread element | Expressions | 12 | 163 | 0.85 | 88 | src/Llyn.Application/Card/LCardEquality.cs:53 |
| using statement | Statements | 1 | 161 | 0.84 | 67 | src/Llyn.Application/Citation/LAuthorCitation.cs:51 |
| Field-like event | Members | 1 | 149 | 0.78 | 83 | src/Llyn.Conduct/Atelier/CNavigation.cs:46 |
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
| Local function | Members | 7 | 17 | 0.09 | 11 | src/Llyn.Conduct/Desk/CErrand.cs:54 |
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
| Null check | is null / is not null | 737 | == null / != null | 0 | 100.00 % |
| Object creation | Target-typed new() | 1,983 | new T() | 3,572 | 35.70 % |
| Collection creation | Collection expression | 6,272 | Collection initializer or array creation | 51 | 99.19 % |
| Member body | Expression body | 2,791 | Block body | 9,682 | 22.38 % |
| Local type | var | 42 | Explicit type | 19,462 | 0.22 % |
| Using | Declaration | 5,044 | Statement | 161 | 96.91 % |
| Namespace | File-scoped | 1,764 | Block | 0 | 100.00 % |
| Text building | Interpolated string | 675 | string.Format or + with a string literal | 418 | 61.76 % |
| Branch | Switch expression | 179 | Switch statement | 59 | 75.21 % |
| Lambda body | Expression | 3,675 | Block | 422 | 89.70 % |
| Type test | is pattern with designation | 1,039 | as | 85 | 92.44 % |
| Constructor | Primary | 4 | Explicit | 470 | 0.84 % |

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
| IdentifierName | 291,572 | 1,765 |
| Argument | 105,379 | 1,332 |
| SimpleMemberAccessExpression | 95,466 | 1,324 |
| ArgumentList | 65,405 | 1,365 |
| InvocationExpression | 59,979 | 1,331 |
| PredefinedType | 29,263 | 1,658 |
| ExpressionStatement | 28,625 | 1,217 |
| StringLiteralExpression | 23,526 | 972 |
| Parameter | 21,021 | 1,595 |
| VariableDeclaration | 20,024 | 1,154 |
| VariableDeclarator | 20,024 | 1,154 |
| EqualsValueClause | 19,223 | 1,212 |
| Block | 17,097 | 1,288 |
| LocalDeclarationStatement | 16,853 | 1,004 |
| ParameterList | 12,875 | 1,694 |
| MethodDeclaration | 10,937 | 1,349 |
| NumericLiteralExpression | 10,427 | 989 |
| GenericName | 9,983 | 1,201 |
| TypeArgumentList | 9,983 | 1,201 |
| QualifiedName | 8,243 | 1,765 |
| SimpleAssignmentExpression | 6,918 | 910 |
| ExpressionElement | 6,795 | 429 |
| CollectionExpression | 6,272 | 794 |
| ReturnStatement | 5,622 | 971 |
| UsingDirective | 5,530 | 1,509 |
| NullLiteralExpression | 3,893 | 795 |
| IfStatement | 3,885 | 631 |
| NullableType | 3,770 | 917 |
| ObjectCreationExpression | 3,572 | 782 |
| Attribute | 3,177 | 373 |
| AttributeList | 3,177 | 373 |
| SimpleLambdaExpression | 3,161 | 562 |
| SingleVariableDesignation | 3,077 | 527 |
| BracketedArgumentList | 2,837 | 499 |
| ArrowExpressionClause | 2,793 | 436 |
| FieldDeclaration | 2,618 | 682 |
| IsPatternExpression | 2,547 | 526 |
| PropertyDeclaration | 2,240 | 451 |
| ElementAccessExpression | 2,232 | 446 |
| ImplicitObjectCreationExpression | 1,983 | 488 |
| FalseLiteralExpression | 1,914 | 521 |
| DeclarationPattern | 1,836 | 352 |
| CompilationUnit | 1,765 | 1,765 |
| FileScopedNamespaceDeclaration | 1,764 | 1,764 |
| ConstantPattern | 1,759 | 408 |
| TrueLiteralExpression | 1,530 | 455 |
| InterpolatedStringText | 1,483 | 157 |
| EqualsExpression | 1,473 | 532 |
| TupleExpression | 1,346 | 254 |
| AddExpression | 1,324 | 315 |
| ForEachStatement | 1,320 | 426 |
| Interpolation | 1,310 | 156 |
| ConditionalExpression | 1,292 | 486 |
| LogicalAndExpression | 1,292 | 352 |
| ClassDeclaration | 1,257 | 1,239 |
| LogicalNotExpression | 1,140 | 375 |
| ConditionalAccessExpression | 1,123 | 353 |
| MemberBindingExpression | 1,123 | 353 |
| AddAssignmentExpression | 1,105 | 238 |
| DeclarationExpression | 964 | 254 |
| SwitchExpressionArm | 953 | 109 |
| ParenthesizedLambdaExpression | 936 | 265 |
| AttributeArgument | 928 | 52 |
| CoalesceExpression | 928 | 376 |
| AccessorList | 919 | 268 |
| GetAccessorDeclaration | 914 | 266 |
| LogicalOrExpression | 898 | 286 |
| NotPattern | 842 | 337 |
| TupleElement | 763 | 185 |
| SuppressNullableWarningExpression | 754 | 268 |
| InterpolatedStringExpression | 675 | 157 |
| NameColon | 649 | 184 |
| WithExpression | 646 | 221 |
| WithInitializerExpression | 646 | 221 |
| GreaterThanExpression | 639 | 279 |
| ArrayRankSpecifier | 610 | 238 |
| ArrayType | 609 | 238 |
| ImplicitElementAccess | 605 | 112 |
| AwaitExpression | 594 | 139 |
| OmittedArraySizeExpression | 593 | 238 |
| PostIncrementExpression | 499 | 222 |
| CharacterLiteralExpression | 497 | 112 |
| RecordDeclaration | 489 | 401 |
| ConstructorDeclaration | 478 | 472 |
| RecursivePattern | 464 | 115 |
| LessThanExpression | 461 | 205 |
| PropertyPatternClause | 455 | 108 |
| ObjectInitializerExpression | 426 | 182 |
| CastExpression | 418 | 148 |
| Subpattern | 412 | 107 |
| NotEqualsExpression | 401 | 170 |
| AttributeArgumentList | 396 | 52 |
| BaseList | 354 | 260 |
| ParenthesizedExpression | 349 | 175 |
| CatchClause | 344 | 143 |
| CatchDeclaration | 341 | 141 |
| LockStatement | 335 | 53 |
| TupleType | 324 | 185 |
| SwitchSection | 319 | 35 |
| BreakStatement | 305 | 60 |
| TryStatement | 300 | 153 |
| ContinueStatement | 299 | 145 |
| OrPattern | 290 | 91 |
| SimpleBaseType | 263 | 241 |
| ForStatement | 248 | 153 |
| EnumMemberDeclaration | 218 | 32 |
| InitAccessorDeclaration | 218 | 74 |
| ThrowStatement | 197 | 97 |
| SubtractExpression | 189 | 87 |
| WhileStatement | 188 | 109 |
| TypeOfExpression | 186 | 42 |
| DiscardPattern | 184 | 109 |
| SwitchExpression | 179 | 109 |
| ThrowExpression | 175 | 89 |
| CaseSwitchLabel | 173 | 23 |
| SpreadElement | 163 | 88 |
| UsingStatement | 161 | 67 |
| UnaryMinusExpression | 159 | 70 |
| SubtractAssignmentExpression | 158 | 67 |
| SetAccessorDeclaration | 152 | 70 |
| ThisExpression | 150 | 79 |
| EventFieldDeclaration | 149 | 83 |
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
| Fact | 2,704 | 359 |
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
| Collection expression | 149 | 2,609 | 1,608 | 275 | 328 | 523 | 158 | 265 | 77 | 215 | 65 |  |  |  |  |
| using declaration | 5 | 1,954 | 2,449 | 578 | 12 | 7 |  |  |  | 18 | 21 |  |  |  |  |
| Lambda | 181 | 791 | 1,269 | 28 | 341 | 730 | 321 | 162 | 66 | 162 | 33 | 4 |  |  | 9 |
| if | 1,194 | 49 | 6 | 636 | 683 | 464 | 380 | 222 | 205 | 30 | 3 | 10 |  | 3 |  |
| Nullable reference type T? | 455 | 148 | 361 | 319 | 333 | 297 | 318 | 320 | 245 | 399 | 29 | 7 |  | 4 |  |
| Expression-bodied member | 939 | 34 | 35 | 6 | 110 | 26 | 255 | 153 | 123 | 1,081 | 29 |  |  |  |  |
| Target-typed new | 718 | 144 | 197 | 131 | 150 | 189 | 36 | 50 | 84 | 243 | 35 | 1 |  | 1 | 4 |
| Declaration pattern | 905 | 5 | 4 | 40 | 193 | 368 | 215 | 22 | 71 | 10 | 3 |  |  |  |  |
| File-scoped namespace | 284 | 196 | 125 | 157 | 119 | 112 | 229 | 312 | 85 | 87 | 19 | 4 | 31 | 4 |  |
| Readonly field | 439 | 37 | 4 | 116 | 385 | 215 | 261 | 28 | 109 | 35 |  | 4 |  | 1 |  |
| Full property | 880 | 7 | 4 | 6 | 110 | 2 | 240 | 141 | 38 | 11 |  |  |  |  |  |
| foreach | 185 | 52 | 18 | 275 | 317 | 306 | 25 | 149 | 32 | 40 | 1 |  |  |  |  |
| Sealed class | 235 | 201 | 128 | 73 | 180 | 43 | 210 | 206 | 58 | 32 | 16 | 4 |  | 4 |  |
| Tuple literal or tuple type | 39 | 411 | 267 | 170 | 84 | 168 | 56 | 44 | 73 | 25 | 10 |  |  |  |  |
| Conditional ?: | 217 | 13 | 16 | 269 | 243 | 156 | 156 | 128 | 62 | 28 | 2 | 2 |  |  |  |
| Class | 271 | 204 | 126 | 155 | 98 | 105 | 72 | 24 | 52 | 95 | 19 | 4 | 31 | 1 |  |
| Null-conditional ?. ?[ | 175 | 129 | 74 | 58 | 47 | 91 | 418 | 37 | 79 | 8 | 6 | 1 |  |  |  |
| Static lambda | 77 | 78 | 526 | 4 | 114 | 15 | 62 | 87 | 14 | 63 | 3 |  |  |  | 1 |
| Constant pattern | 133 | 3 | 8 | 142 | 54 | 353 | 126 | 138 | 43 | 17 |  | 5 |  |  |  |
| Null-coalescing ?? | 79 | 18 | 12 | 102 | 189 | 80 | 88 | 254 | 41 | 58 | 6 |  |  | 1 |  |
| not pattern | 310 | 4 | 1 | 52 | 70 | 185 | 134 | 30 | 49 | 6 | 1 |  |  |  |  |
| Auto property | 348 | 5 | 3 | 8 | 7 | 23 | 106 | 229 | 46 | 21 |  |  |  |  |  |
| Null-forgiving ! | 165 | 222 | 207 | 56 | 15 | 25 | 5 |  | 3 | 55 | 1 |  |  |  |  |
| is null | 118 | 8 | 5 | 91 | 152 | 175 | 81 | 37 | 55 | 13 | 2 |  |  |  |  |
| Interpolated string | 19 | 51 | 7 | 158 | 51 | 380 |  | 6 |  | 3 |  |  |  |  |  |
| with expression | 7 | 214 | 31 | 42 | 248 | 10 | 16 | 17 | 36 | 25 |  |  |  |  |  |
| await | 57 | 228 | 144 | 62 | 56 | 1 | 13 |  | 17 | 5 | 1 | 10 |  |  |  |
| Extension method |  |  |  |  |  |  |  |  |  | 586 | 8 |  |  |  |  |
| Raw string | 2 | 121 | 7 | 213 |  | 233 |  |  |  | 8 |  |  |  |  |  |
| Const field | 76 | 69 | 14 | 179 | 13 | 113 | 3 | 70 | 4 | 12 |  | 4 |  | 1 |  |
| Nullable value type T? | 56 | 20 | 21 | 42 | 34 | 11 | 135 | 30 | 111 | 66 | 7 |  |  | 2 |  |
| Record class | 16 |  | 2 | 2 | 104 | 6 | 145 | 203 | 6 | 2 |  |  |  | 3 |  |
| Constructor | 169 | 2 | 1 | 65 | 76 | 7 | 62 | 3 | 51 | 7 |  | 2 | 31 |  |  |
| Property pattern | 143 | 2 |  | 8 | 15 | 270 | 8 | 3 | 4 | 1 | 1 |  |  |  |  |
| Async method | 56 | 180 | 99 | 32 | 29 | 1 | 13 |  | 13 | 5 | 1 | 5 |  |  |  |
| Object initializer | 135 | 55 | 95 | 29 | 1 | 60 |  | 3 |  | 33 | 13 | 1 |  | 1 |  |
| Optional parameter | 8 | 12 | 6 | 6 | 11 | 1 | 4 | 256 | 12 | 102 |  |  |  | 3 |  |
| Cast expression | 137 | 7 | 83 | 79 | 4 | 50 | 3 | 4 |  | 35 | 15 | 1 |  |  |  |
| nameof | 218 | 1 | 3 | 8 | 117 | 17 | 18 | 3 | 4 | 2 | 1 |  |  |  |  |
| catch | 21 | 1 |  | 126 | 33 | 8 | 109 | 1 | 27 | 1 | 8 | 6 |  | 3 |  |
| lock | 13 |  |  | 2 | 77 | 5 | 2 |  | 231 | 5 |  |  |  |  |  |
| and / or pattern | 37 | 1 | 3 | 69 | 15 | 159 | 2 | 39 | 1 | 6 |  |  |  |  |  |
| Deconstruction | 28 | 66 | 113 | 26 | 27 | 33 | 19 | 5 | 5 |  |  |  |  |  |  |
| Static class | 44 | 3 |  | 84 | 21 | 68 | 7 | 21 |  | 62 | 3 |  |  |  |  |
| try | 19 | 7 |  | 78 | 30 | 10 | 111 | 1 | 28 | 1 | 9 | 5 |  | 1 |  |
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
| Field-like event | 53 |  |  |  |  |  | 92 |  | 4 |  |  |  |  |  |  |
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
| Object creation | 50.89 % | 50.88 % | 33.68 % | 23.99 % | 28.85 % | 42.00 % | 9.23 % | 26.74 % | 30.43 % | 31.15 % | 39.77 % | 7.69 % | - | 14.29 % | 21.05 % |
| Collection creation | 94.90 % | 99.58 % | 99.14 % | 97.86 % | 99.39 % | 99.05 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 92.86 % | - | - | - | - |
| Member body | 32.23 % | 2.07 % | 2.75 % | 0.63 % | 10.97 % | 3.69 % | 20.75 % | 30.42 % | 16.16 % | 79.08 % | 38.16 % | 0.00 % | 0.00 % | 0.00 % | - |
| Local type | 4.58 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.31 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | - | 0.00 % | 0.00 % |
| Using | 100.00 % | 98.19 % | 99.92 % | 83.41 % | 63.16 % | 100.00 % | - | - | - | 100.00 % | 95.45 % | - | - | - | - |
| Namespace | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - |
| Text building | 55.88 % | 33.12 % | 15.91 % | 58.09 % | 76.12 % | 82.07 % | 0.00 % | 26.09 % | - | 27.27 % | - | 0.00 % | - | 0.00 % | - |
| Branch | 63.16 % | - | 100.00 % | 65.22 % | 100.00 % | 76.47 % | 90.48 % | 59.57 % | 90.00 % | 33.33 % | - | 100.00 % | - | - | - |
| Lambda body | 82.87 % | 96.59 % | 79.59 % | 85.71 % | 92.08 % | 99.04 % | 98.44 % | 98.15 % | 90.91 % | 81.48 % | 42.42 % | 50.00 % | - | - | 77.78 % |
| Type test | 91.13 % | 100.00 % | 100.00 % | 90.63 % | 100.00 % | 94.51 % | 100.00 % | 100.00 % | 100.00 % | 40.00 % | 100.00 % | - | - | - | - |
| Constructor | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 36.36 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | - | 0.00 % | 0.00 % | - | - |
