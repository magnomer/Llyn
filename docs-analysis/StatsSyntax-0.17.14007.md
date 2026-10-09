# Syntax statistics - 0.17.14007

- Generated: 2026-10-09 11:37:17 +09:00
- Source roots: src, tests
- Root-level files: none
- Project segments: 1
- Excluded directories: .git, .vs, artifacts, bin, node_modules, obj, packages, publish
- Excluded suffixes: .g.cs, .g.i.cs, .AssemblyInfo.cs, .GlobalUsings.g.cs, .Designer.cs
- Roslyn: 4.14.0-3.25262.10
- Language version: Preview
- Scanned: 1,839 files, 15 projects, 191,689 lines

## Summary

| Measure | Value |
|---|---:|
| Files | 1,839 |
| Lines | 191,689 |
| Nodes | 1,009,875 |
| Syntax kinds used | 191 |
| Features used | 98 of 138 |
| Minimum C# version | 12 |
| Parse errors | 0 |

## Families

| Family | Features | Used | Count | Per 1k lines |
|---|---:|---:|---:|---:|
| Types | 15 | 12 | 3,828 | 19.97 |
| Namespaces | 6 | 2 | 1,839 | 9.59 |
| Members | 29 | 20 | 9,178 | 47.88 |
| Patterns | 16 | 16 | 5,948 | 31.03 |
| Expressions | 36 | 29 | 23,837 | 124.35 |
| Statements | 20 | 13 | 12,051 | 62.87 |
| Generics | 6 | 2 | 72 | 0.38 |
| Async | 2 | 2 | 442 | 2.31 |
| Nullability | 3 | 2 | 3,748 | 19.55 |
| Unsafe | 2 | 0 | 0 | 0.00 |
| Directives | 3 | 0 | 0 | 0.00 |
| Total | 138 | 98 | 60,943 | 317.93 |

## Projects

| Project | Files | Lines | Nodes | Features used | Newest C# |
|---|---:|---:|---:|---:|---:|
| Llyn.Tests.Engine | 200 | 32,028 | 225,167 | 59 | 12 |
| Llyn.UIDeportment | 292 | 31,781 | 145,803 | 85 | 12 |
| Llyn.Tests.Conduct | 139 | 24,479 | 178,199 | 61 | 12 |
| Llyn.Infrastructure | 168 | 22,108 | 100,200 | 70 | 12 |
| Llyn.Application | 121 | 18,082 | 83,223 | 70 | 12 |
| Llyn.Tests.Convention | 115 | 17,401 | 84,730 | 79 | 12 |
| Llyn.Conduct | 241 | 15,402 | 60,256 | 59 | 12 |
| Llyn.Core | 322 | 10,714 | 43,147 | 58 | 12 |
| Llyn.ShellEngine | 90 | 9,276 | 37,158 | 57 | 12 |
| Llyn.Tests.Interface | 88 | 8,306 | 41,936 | 65 | 12 |
| Llyn.Tests.Windows | 19 | 1,171 | 6,970 | 40 | 12 |
| Llyn.Core.Windows | 4 | 385 | 1,427 | 26 | 10 |
| Llyn.UIVeneer | 35 | 383 | 760 | 4 | 10 |
| Llyn.UIDeportment.Capsule | 4 | 88 | 380 | 15 | 10 |
| Llyn.Host | 1 | 85 | 519 | 4 | 9 |
| Total | 1,839 | 191,689 | 1,009,875 | 98 | 12 |

## Versions

| C# version | Features | Used | Count |
|---|---:|---:|---:|
| 1 | 51 | 35 | 16,294 |
| 2 | 10 | 8 | 1,920 |
| 3 | 7 | 5 | 5,980 |
| 4 | 3 | 2 | 711 |
| 5 | 3 | 3 | 1,037 |
| 6 | 6 | 5 | 4,633 |
| 7 | 11 | 10 | 5,729 |
| 7.1 | 1 | 1 | 7 |
| 7.2 | 1 | 0 | 0 |
| 7.3 | 1 | 0 | 0 |
| 8 | 17 | 12 | 10,031 |
| 9 | 10 | 9 | 5,715 |
| 10 | 3 | 2 | 1,840 |
| 11 | 6 | 3 | 603 |
| 12 | 4 | 3 | 6,443 |
| 13 | 1 | 0 | 0 |
| 14 | 3 | 0 | 0 |

## Features

| Feature | Family | C# | Count | Per 1k lines | Files | First |
|---|---|---|---:|---:|---:|---|
| Collection expression | Expressions | 12 | 6,275 | 32.74 | 824 | src/Llyn.Application/Card/LCardClerk.cs:57 |
| using declaration | Statements | 8 | 5,044 | 26.31 | 387 | src/Llyn.Application/Card/LTranslationClerk.cs:160 |
| Lambda | Expressions | 3 | 4,124 | 21.51 | 661 | src/Llyn.Application/Card/LCardClerk.cs:174 |
| if | Statements | 1 | 3,836 | 20.01 | 644 | src/Llyn.Application/Card/LCardClerk.cs:70 |
| Nullable reference type T? | Nullability | 8 | 3,233 | 16.87 | 891 | src/Llyn.Application/Card/LCardClerkField.cs:91 |
| Expression-bodied member | Members | 6 | 2,467 | 12.87 | 472 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Target-typed new | Expressions | 9 | 2,025 | 10.56 | 511 | src/Llyn.Application/Card/LMentionClerk.cs:167 |
| Readonly field | Members | 1 | 1,918 | 10.01 | 580 | src/Llyn.Application/Card/LCardClerk.cs:9 |
| File-scoped namespace | Namespaces | 10 | 1,838 | 9.59 | 1,838 | src/Llyn.Application/Card/LCardClerk.cs:5 |
| Declaration pattern | Patterns | 7 | 1,818 | 9.48 | 360 | src/Llyn.Application/Card/LMeaningClerk.cs:93 |
| Sealed class | Types | 1 | 1,450 | 7.56 | 1,349 | src/Llyn.Application/Card/LCardClerk.cs:7 |
| foreach | Statements | 1 | 1,402 | 7.31 | 445 | src/Llyn.Application/Card/LCardClerk.cs:59 |
| Tuple literal or tuple type | Expressions | 7 | 1,346 | 7.02 | 256 | src/Llyn.Application/Card/LMeaningClerk.cs:26 |
| Class | Types | 1 | 1,317 | 6.87 | 1,300 | src/Llyn.Application/Card/LCardClerk.cs:7 |
| Conditional ?: | Expressions | 1 | 1,266 | 6.60 | 489 | src/Llyn.Application/Card/LCardClerk.cs:173 |
| Full property | Members | 1 | 1,105 | 5.76 | 340 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Static lambda | Expressions | 9 | 1,050 | 5.48 | 265 | src/Llyn.Application/Card/LCardEquality.cs:113 |
| Null-conditional ?. ?[ | Expressions | 6 | 1,042 | 5.44 | 364 | src/Llyn.Application/Card/LMentionClerk.cs:47 |
| Constant pattern | Patterns | 7 | 1,022 | 5.33 | 184 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Null-coalescing ?? | Expressions | 2 | 883 | 4.61 | 363 | src/Llyn.Application/Card/LCardClerkField.cs:74 |
| not pattern | Patterns | 9 | 841 | 4.39 | 349 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| Auto property | Members | 3 | 827 | 4.31 | 242 | src/Llyn.Application/Citation/LCitationClerk.cs:41 |
| is null | Patterns | 7 | 742 | 3.87 | 330 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| Null-forgiving ! | Expressions | 8 | 739 | 3.86 | 290 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:145 |
| Interpolated string | Expressions | 6 | 675 | 3.52 | 158 | src/Llyn.Application/Localization/LLocalizationReader.cs:55 |
| with expression | Expressions | 9 | 661 | 3.45 | 226 | src/Llyn.Application/Card/LCardClerk.cs:109 |
| await | Expressions | 5 | 595 | 3.10 | 145 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:139 |
| Extension method | Members | 3 | 595 | 3.10 | 41 | tests/Llyn.Tests.Interface/TInterfaceArea.cs:7 |
| Raw string | Expressions | 11 | 584 | 3.05 | 133 | src/Llyn.Infrastructure/Database/Base/LDatabase.cs:51 |
| Const field | Members | 1 | 554 | 2.89 | 232 | src/Llyn.Application/Card/LMentionClerk.cs:11 |
| Constructor | Members | 1 | 548 | 2.86 | 543 | src/Llyn.Application/Card/LCardClerk.cs:19 |
| Nullable value type T? | Nullability | 2 | 515 | 2.69 | 234 | src/Llyn.Application/Card/LMeaningClerk.cs:92 |
| Record class | Types | 9 | 498 | 2.60 | 410 | src/Llyn.Application/Ensign/LEnsignRow.cs:3 |
| Property pattern | Patterns | 8 | 456 | 2.38 | 112 | src/Llyn.Application/Draft/Clerk/LDraftClerk.cs:82 |
| Async method | Async | 5 | 435 | 2.27 | 145 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:91 |
| Optional parameter | Members | 4 | 425 | 2.22 | 141 | src/Llyn.Application/Draft/Clerk/LDraftClerkPanel.cs:170 |
| Cast expression | Expressions | 1 | 418 | 2.18 | 151 | src/Llyn.Application/Markup/LMarkupClerkLink.cs:234 |
| Object initializer | Expressions | 3 | 418 | 2.18 | 187 | src/Llyn.Application/Portrait/LPortraitClerkLabel.cs:25 |
| nameof | Expressions | 6 | 374 | 1.95 | 132 | src/Llyn.Application/Citation/LAuthorClerk.cs:361 |
| catch | Statements | 1 | 343 | 1.79 | 152 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:142 |
| lock | Statements | 1 | 335 | 1.75 | 57 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:98 |
| and / or pattern | Patterns | 9 | 332 | 1.73 | 100 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Static class | Types | 2 | 325 | 1.70 | 325 | src/Llyn.Application/Card/LCardClerkField.cs:7 |
| Deconstruction | Expressions | 7 | 322 | 1.68 | 136 | src/Llyn.Application/Card/LMeaningClerk.cs:46 |
| try | Statements | 1 | 299 | 1.56 | 162 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:137 |
| Named argument | Members | 4 | 286 | 1.49 | 89 | src/Llyn.Application/Card/LTranslationClerk.cs:164 |
| for | Statements | 1 | 248 | 1.29 | 155 | src/Llyn.Application/Card/LCardClerk.cs:247 |
| Init accessor | Members | 9 | 217 | 1.13 | 73 | src/Llyn.Conduct/Lexicon/CExample.cs:20 |
| while | Statements | 1 | 188 | 0.98 | 112 | src/Llyn.Application/Card/LMeaningClerk.cs:44 |
| typeof | Expressions | 1 | 186 | 0.97 | 42 | src/Llyn.Infrastructure/Configuration/LLocalizationLoader.cs:20 |
| Discard pattern | Patterns | 8 | 184 | 0.96 | 111 | src/Llyn.Application/Citation/LAuthorClerk.cs:243 |
| Switch expression | Patterns | 8 | 179 | 0.93 | 111 | src/Llyn.Application/Citation/LAuthorClerk.cs:239 |
| throw expression | Expressions | 7 | 175 | 0.91 | 93 | src/Llyn.Application/Card/LCardClerkField.cs:75 |
| Spread element | Expressions | 12 | 164 | 0.86 | 89 | src/Llyn.Application/Card/LCardEquality.cs:53 |
| using statement | Statements | 1 | 161 | 0.84 | 68 | src/Llyn.Application/Citation/LAuthorCitation.cs:51 |
| Field-like event | Members | 1 | 158 | 0.82 | 94 | src/Llyn.Conduct/Atelier/CNavigation.cs:46 |
| Discard _ | Expressions | 7 | 143 | 0.75 | 51 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:108 |
| Case guard when | Patterns | 7 | 115 | 0.60 | 24 | src/Llyn.Application/Citation/LAuthorClerk.cs:241 |
| Interface | Types | 1 | 98 | 0.51 | 98 | src/Llyn.Application/Pronunciation/Relay/LListener.cs:5 |
| Verbatim string | Expressions | 1 | 87 | 0.45 | 19 | src/Llyn.Application/Localization/LLocalizationReader.cs:13 |
| as cast | Expressions | 1 | 84 | 0.44 | 44 | src/Llyn.Infrastructure/Database/Base/LWorkspaceArchive.cs:132 |
| Range .. | Expressions | 8 | 84 | 0.44 | 41 | src/Llyn.Application/Card/LMentionClerk.cs:173 |
| Relational pattern | Patterns | 9 | 80 | 0.42 | 28 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Exception filter | Statements | 6 | 75 | 0.39 | 49 | src/Llyn.Application/Outpost/LCourierClerk.cs:204 |
| Index from end ^ | Expressions | 8 | 74 | 0.39 | 43 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:230 |
| Partial type | Types | 2 | 72 | 0.38 | 72 | src/Llyn.UIVeneer/Display/View/PDisplay.xaml.cs:5 |
| is type test | Patterns | 1 | 59 | 0.31 | 36 | src/Llyn.Application/Outpost/LCourierClerk.cs:296 |
| Switch statement | Patterns | 1 | 57 | 0.30 | 34 | src/Llyn.Conduct/Configuration/CLedger.cs:240 |
| Generic method | Generics | 2 | 54 | 0.28 | 29 | src/Llyn.Application/Card/LCardClerkField.cs:183 |
| yield return / break | Statements | 2 | 50 | 0.26 | 20 | src/Llyn.Application/Card/LCardClerkField.cs:35 |
| Local const | Statements | 1 | 37 | 0.19 | 22 | src/Llyn.Conduct/Lexicon/CStateWording.cs:10 |
| finally | Statements | 1 | 33 | 0.17 | 25 | src/Llyn.Application/Outpost/LCourierClerk.cs:80 |
| Enum | Types | 1 | 32 | 0.17 | 32 | src/Llyn.Conduct/Catalog/CCatalogOrder.cs:3 |
| var pattern | Patterns | 7 | 29 | 0.15 | 11 | src/Llyn.Conduct/Card/CFolio.cs:193 |
| Nested type | Types | 1 | 25 | 0.13 | 15 | src/Llyn.UIDeportment/Kit/Bind/QLook.cs:19 |
| ref or out parameter | Members | 1 | 25 | 0.13 | 19 | src/Llyn.Application/Draft/Clerk/LDraftClerkCard.cs:149 |
| Constraint clause | Generics | 2 | 18 | 0.09 | 10 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:171 |
| Null-coalescing assignment ??= | Expressions | 8 | 18 | 0.09 | 16 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:160 |
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
| Event accessors | Members | 1 | 4 | 0.02 | 3 | src/Llyn.ShellEngine/Port/LSettingsOutlet.cs:72 |
| Primary constructor on class or struct | Types | 12 | 4 | 0.02 | 4 | tests/Llyn.Tests.Convention/TAuditFakeMember.cs:3 |
| Generic type | Types | 2 | 3 | 0.02 | 3 | src/Llyn.Conduct/Configuration/CEnsignSheet.cs:5 |
| Abstract class | Types | 1 | 2 | 0.01 | 2 | src/Llyn.Application/Request/Entry/LRequest.cs:3 |
| checked / unchecked expression | Expressions | 1 | 2 | 0.01 | 2 | src/Llyn.Core.Windows/LPressBrowser.cs:14 |
| Record struct | Types | 10 | 2 | 0.01 | 2 | src/Llyn.UIDeportment/Mention/Menu/PSwathSeam.cs:6 |
| Static constructor | Members | 1 | 2 | 0.01 | 2 | src/Llyn.UIDeportment/Kit/Bind/QLookItem.cs:23 |
| Static local function | Members | 8 | 2 | 0.01 | 1 | src/Llyn.UIDeportment/Kit/Asset/QIcon.cs:144 |
| Indexer | Members | 1 | 1 | 0.01 | 1 | src/Llyn.UIDeportment/Kit/Asset/QLocalizationCatalog.cs:19 |
| Using alias | Namespaces | 1 | 1 | 0.01 | 1 | src/Llyn.Host/LHost.cs:11 |

## Styles

| Style | A | A count | B | B count | A share |
|---|---|---:|---|---:|---:|
| Null check | is null / is not null | 742 | == null / != null | 0 | 100.00 % |
| Object creation | Target-typed new() | 2,025 | new T() | 3,697 | 35.39 % |
| Collection creation | Collection expression | 6,275 | Collection initializer or array creation | 51 | 99.19 % |
| Member body | Expression body | 2,467 | Block body | 9,548 | 20.53 % |
| Local type | var | 42 | Explicit type | 19,504 | 0.21 % |
| Using | Declaration | 5,044 | Statement | 161 | 96.91 % |
| Namespace | File-scoped | 1,838 | Block | 0 | 100.00 % |
| Text building | Interpolated string | 675 | string.Format or + with a string literal | 425 | 61.36 % |
| Branch | Switch expression | 179 | Switch statement | 57 | 75.85 % |
| Lambda body | Expression | 3,698 | Block | 426 | 89.67 % |
| Type test | is pattern with designation | 1,031 | as | 84 | 92.47 % |
| Constructor | Primary | 4 | Explicit | 543 | 0.73 % |

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
| IdentifierName | 292,710 | 1,839 |
| Argument | 104,998 | 1,395 |
| SimpleMemberAccessExpression | 96,315 | 1,378 |
| ArgumentList | 65,070 | 1,430 |
| InvocationExpression | 59,480 | 1,387 |
| PredefinedType | 28,762 | 1,712 |
| ExpressionStatement | 28,523 | 1,274 |
| StringLiteralExpression | 23,301 | 1,023 |
| Parameter | 20,994 | 1,660 |
| VariableDeclaration | 20,282 | 1,211 |
| VariableDeclarator | 20,282 | 1,211 |
| EqualsValueClause | 19,215 | 1,258 |
| Block | 16,918 | 1,347 |
| LocalDeclarationStatement | 16,893 | 1,038 |
| ParameterList | 12,794 | 1,768 |
| MethodDeclaration | 10,759 | 1,414 |
| NumericLiteralExpression | 10,271 | 1,004 |
| GenericName | 9,879 | 1,259 |
| TypeArgumentList | 9,879 | 1,259 |
| QualifiedName | 8,612 | 1,839 |
| SimpleAssignmentExpression | 6,942 | 956 |
| ExpressionElement | 6,816 | 440 |
| CollectionExpression | 6,275 | 824 |
| UsingDirective | 5,819 | 1,577 |
| ReturnStatement | 5,547 | 990 |
| NullLiteralExpression | 3,864 | 829 |
| IfStatement | 3,836 | 644 |
| NullableType | 3,748 | 937 |
| ObjectCreationExpression | 3,697 | 824 |
| Attribute | 3,178 | 388 |
| AttributeList | 3,178 | 388 |
| SimpleLambdaExpression | 3,172 | 585 |
| SingleVariableDesignation | 3,060 | 534 |
| BracketedArgumentList | 2,827 | 507 |
| FieldDeclaration | 2,827 | 730 |
| IsPatternExpression | 2,540 | 537 |
| ArrowExpressionClause | 2,469 | 473 |
| ElementAccessExpression | 2,231 | 454 |
| ImplicitObjectCreationExpression | 2,025 | 511 |
| PropertyDeclaration | 1,937 | 483 |
| FalseLiteralExpression | 1,870 | 522 |
| CompilationUnit | 1,839 | 1,839 |
| FileScopedNamespaceDeclaration | 1,838 | 1,838 |
| DeclarationPattern | 1,818 | 360 |
| ConstantPattern | 1,764 | 416 |
| TrueLiteralExpression | 1,536 | 467 |
| InterpolatedStringText | 1,483 | 158 |
| EqualsExpression | 1,458 | 536 |
| TupleExpression | 1,346 | 257 |
| AddExpression | 1,329 | 324 |
| ForEachStatement | 1,322 | 431 |
| ClassDeclaration | 1,317 | 1,300 |
| Interpolation | 1,310 | 157 |
| LogicalAndExpression | 1,284 | 362 |
| ConditionalExpression | 1,266 | 489 |
| LogicalNotExpression | 1,116 | 387 |
| AddAssignmentExpression | 1,100 | 264 |
| ConditionalAccessExpression | 1,042 | 364 |
| MemberBindingExpression | 1,042 | 364 |
| DeclarationExpression | 964 | 258 |
| SwitchExpressionArm | 953 | 111 |
| ParenthesizedLambdaExpression | 952 | 276 |
| AccessorList | 933 | 266 |
| AttributeArgument | 928 | 53 |
| GetAccessorDeclaration | 928 | 264 |
| LogicalOrExpression | 886 | 289 |
| CoalesceExpression | 883 | 363 |
| NotPattern | 841 | 349 |
| TupleElement | 761 | 184 |
| SuppressNullableWarningExpression | 739 | 290 |
| InterpolatedStringExpression | 675 | 158 |
| WithExpression | 661 | 226 |
| WithInitializerExpression | 661 | 226 |
| NameColon | 649 | 189 |
| GreaterThanExpression | 632 | 280 |
| ArrayRankSpecifier | 608 | 243 |
| ArrayType | 607 | 243 |
| ImplicitElementAccess | 596 | 115 |
| AwaitExpression | 595 | 145 |
| OmittedArraySizeExpression | 591 | 243 |
| ConstructorDeclaration | 550 | 545 |
| PostIncrementExpression | 499 | 228 |
| RecordDeclaration | 498 | 410 |
| CharacterLiteralExpression | 497 | 113 |
| RecursivePattern | 465 | 119 |
| PropertyPatternClause | 456 | 112 |
| LessThanExpression | 449 | 203 |
| CastExpression | 418 | 151 |
| ObjectInitializerExpression | 418 | 187 |
| Subpattern | 412 | 109 |
| NotEqualsExpression | 408 | 175 |
| AttributeArgumentList | 396 | 53 |
| BaseList | 361 | 267 |
| ParenthesizedExpression | 344 | 175 |
| CatchClause | 343 | 152 |
| CatchDeclaration | 340 | 150 |
| LockStatement | 335 | 57 |
| TupleType | 323 | 184 |
| SwitchSection | 308 | 34 |
| ContinueStatement | 299 | 146 |
| TryStatement | 299 | 162 |
| BreakStatement | 294 | 59 |
| OrPattern | 290 | 93 |
| SimpleBaseType | 269 | 248 |
| ForStatement | 248 | 155 |
| EnumMemberDeclaration | 218 | 32 |
| InitAccessorDeclaration | 217 | 73 |
| ThrowStatement | 197 | 100 |
| WhileStatement | 188 | 112 |
| SubtractExpression | 186 | 85 |
| TypeOfExpression | 186 | 42 |
| DiscardPattern | 184 | 111 |
| SwitchExpression | 179 | 111 |
| ThrowExpression | 175 | 93 |
| CaseSwitchLabel | 167 | 22 |
| SpreadElement | 164 | 89 |
| UsingStatement | 161 | 68 |
| UnaryMinusExpression | 159 | 71 |
| EventFieldDeclaration | 158 | 94 |
| SubtractAssignmentExpression | 156 | 68 |
| ElseClause | 141 | 112 |
| SetAccessorDeclaration | 131 | 67 |
| ThisExpression | 116 | 78 |
| WhenClause | 115 | 24 |
| LessThanOrEqualExpression | 111 | 80 |
| CasePatternSwitchLabel | 106 | 20 |
| PrimaryConstructorBaseType | 103 | 19 |
| InterfaceDeclaration | 98 | 98 |
| GreaterThanOrEqualExpression | 96 | 65 |
| AsExpression | 84 | 44 |
| RangeExpression | 84 | 41 |
| MultiplyExpression | 81 | 32 |
| ForEachVariableStatement | 80 | 57 |
| RelationalPattern | 80 | 28 |
| CatchFilterClause | 75 | 49 |
| IndexExpression | 74 | 43 |
| TypeParameter | 60 | 32 |
| IsExpression | 59 | 36 |
| SwitchStatement | 57 | 34 |
| TypeParameterList | 57 | 32 |
| ArrayInitializerExpression | 54 | 28 |
| BitwiseOrExpression | 47 | 24 |
| YieldReturnStatement | 43 | 20 |
| AndPattern | 42 | 18 |
| DefaultSwitchLabel | 41 | 22 |
| ParenthesizedPattern | 41 | 19 |
| OrAssignmentExpression | 37 | 21 |
| ExpressionColon | 33 | 14 |
| FinallyClause | 33 | 25 |
| ImplicitArrayCreationExpression | 33 | 26 |
| EnumDeclaration | 32 | 32 |
| LeftShiftExpression | 32 | 2 |
| DivideExpression | 31 | 19 |
| VarPattern | 29 | 11 |
| ArrayCreationExpression | 24 | 17 |
| CoalesceAssignmentExpression | 18 | 16 |
| TypeParameterConstraintClause | 18 | 10 |
| ClassConstraint | 17 | 9 |
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
| Fact | 2,705 | 374 |
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

| Feature | Llyn.Tests.Engine | Llyn.UIDeportment | Llyn.Tests.Conduct | Llyn.Infrastructure | Llyn.Application | Llyn.Tests.Convention | Llyn.Conduct | Llyn.Core | Llyn.ShellEngine | Llyn.Tests.Interface | Llyn.Tests.Windows | Llyn.Core.Windows | Llyn.UIVeneer | Llyn.UIDeportment.Capsule | Llyn.Host |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| Collection expression | 2,602 | 147 | 1,609 | 275 | 328 | 523 | 159 | 265 | 78 | 226 | 63 |  |  |  |  |
| using declaration | 1,954 | 5 | 2,449 | 578 | 12 | 7 |  |  |  | 18 | 21 |  |  |  |  |
| Lambda | 791 | 184 | 1,259 | 28 | 341 | 730 | 336 | 162 | 69 | 180 | 31 | 4 |  |  | 9 |
| if | 49 | 1,183 | 5 | 636 | 683 | 464 | 341 | 222 | 206 | 31 | 3 | 10 |  | 3 |  |
| Nullable reference type T? | 148 | 457 | 356 | 319 | 333 | 297 | 312 | 321 | 245 | 405 | 29 | 7 |  | 4 |  |
| Expression-bodied member | 34 | 642 | 35 | 15 | 110 | 26 | 193 | 154 | 124 | 1,107 | 27 |  |  |  |  |
| Target-typed new | 144 | 718 | 222 | 141 | 150 | 189 | 37 | 50 | 85 | 247 | 36 | 1 |  | 1 | 4 |
| Readonly field | 37 | 595 | 4 | 119 | 393 | 215 | 315 | 28 | 172 | 35 |  | 4 |  | 1 |  |
| File-scoped namespace | 200 | 292 | 139 | 168 | 121 | 115 | 241 | 322 | 90 | 88 | 19 | 4 | 35 | 4 |  |
| Declaration pattern | 5 | 898 | 4 | 40 | 193 | 368 | 203 | 22 | 72 | 10 | 3 |  |  |  |  |
| Sealed class | 205 | 249 | 139 | 75 | 182 | 43 | 222 | 215 | 63 | 33 | 16 | 4 |  | 4 |  |
| foreach | 52 | 187 | 18 | 275 | 317 | 306 | 25 | 149 | 32 | 40 | 1 |  |  |  |  |
| Tuple literal or tuple type | 411 | 39 | 265 | 170 | 84 | 168 | 55 | 44 | 73 | 27 | 10 |  |  |  |  |
| Class | 208 | 277 | 138 | 166 | 100 | 108 | 84 | 24 | 57 | 96 | 19 | 4 | 35 | 1 |  |
| Conditional ?: | 13 | 203 | 16 | 269 | 243 | 156 | 143 | 129 | 62 | 28 | 2 | 2 |  |  |  |
| Full property | 7 | 587 | 5 | 6 | 110 | 2 | 179 | 142 | 39 | 28 |  |  |  |  |  |
| Static lambda | 78 | 77 | 527 | 4 | 114 | 15 | 63 | 87 | 17 | 64 | 3 |  |  |  | 1 |
| Null-conditional ?. ?[ | 129 | 180 | 83 | 58 | 47 | 91 | 323 | 37 | 79 | 8 | 6 | 1 |  |  |  |
| Constant pattern | 3 | 133 | 8 | 142 | 54 | 353 | 126 | 138 | 43 | 17 |  | 5 |  |  |  |
| Null-coalescing ?? | 18 | 72 | 12 | 102 | 189 | 80 | 50 | 254 | 41 | 58 | 6 |  |  | 1 |  |
| not pattern | 4 | 309 | 1 | 52 | 70 | 185 | 134 | 30 | 49 | 6 | 1 |  |  |  |  |
| Auto property | 5 | 352 | 3 | 8 | 7 | 23 | 127 | 229 | 51 | 22 |  |  |  |  |  |
| is null | 8 | 121 | 5 | 91 | 152 | 175 | 83 | 37 | 55 | 13 | 2 |  |  |  |  |
| Null-forgiving ! | 222 | 146 | 201 | 56 | 15 | 25 | 9 |  | 3 | 61 | 1 |  |  |  |  |
| Interpolated string | 51 | 19 | 7 | 158 | 51 | 380 |  | 6 |  | 3 |  |  |  |  |  |
| with expression | 215 | 7 | 35 | 42 | 248 | 10 | 22 | 17 | 36 | 29 |  |  |  |  |  |
| await | 228 | 57 | 143 | 62 | 57 | 1 | 13 |  | 17 | 6 | 1 | 10 |  |  |  |
| Extension method |  |  |  |  |  |  |  |  |  | 589 | 6 |  |  |  |  |
| Raw string | 121 | 2 | 7 | 213 |  | 233 |  |  |  | 8 |  |  |  |  |  |
| Const field | 69 | 72 | 14 | 179 | 13 | 113 | 3 | 70 | 4 | 12 |  | 4 |  | 1 |  |
| Constructor | 2 | 215 | 1 | 67 | 78 | 7 | 74 | 3 | 56 | 8 |  | 2 | 35 |  |  |
| Nullable value type T? | 20 | 55 | 21 | 42 | 34 | 11 | 115 | 30 | 111 | 67 | 7 |  |  | 2 |  |
| Record class |  | 16 | 2 | 2 | 104 | 6 | 145 | 212 | 6 | 2 |  |  |  | 3 |  |
| Property pattern | 2 | 143 |  | 8 | 15 | 270 | 9 | 3 | 4 | 1 | 1 |  |  |  |  |
| Async method | 180 | 56 | 98 | 32 | 30 | 1 | 13 |  | 13 | 6 | 1 | 5 |  |  |  |
| Optional parameter | 12 | 8 | 6 | 6 | 11 | 1 | 8 | 256 | 12 | 102 |  |  |  | 3 |  |
| Cast expression | 7 | 137 | 78 | 79 | 4 | 50 | 3 | 4 |  | 40 | 15 | 1 |  |  |  |
| Object initializer | 55 | 124 | 95 | 29 | 1 | 60 |  | 3 |  | 33 | 16 | 1 |  | 1 |  |
| nameof | 1 | 200 | 3 | 8 | 117 | 17 | 18 | 3 | 4 | 2 | 1 |  |  |  |  |
| catch | 1 | 21 |  | 126 | 33 | 8 | 108 | 1 | 27 | 1 | 8 | 6 |  | 3 |  |
| lock |  | 13 |  | 2 | 77 | 5 | 2 |  | 231 | 5 |  |  |  |  |  |
| and / or pattern | 1 | 37 | 3 | 69 | 15 | 159 | 2 | 39 | 1 | 6 |  |  |  |  |  |
| Static class | 3 | 43 | 1 | 93 | 21 | 71 | 7 | 21 |  | 62 | 3 |  |  |  |  |
| Deconstruction | 66 | 28 | 113 | 26 | 27 | 33 | 19 | 5 | 5 |  |  |  |  |  |  |
| try | 7 | 19 |  | 78 | 30 | 10 | 110 | 1 | 28 | 1 | 9 | 5 |  | 1 |  |
| Named argument | 135 |  | 21 | 16 | 23 | 16 |  | 18 | 3 | 54 |  |  |  |  |  |
| for | 11 | 41 | 6 | 60 | 58 | 27 | 6 | 14 | 7 | 16 | 2 |  |  |  |  |
| Init accessor |  | 2 |  |  |  | 3 | 1 | 211 |  |  |  |  |  |  |  |
| while | 16 | 9 | 5 | 97 | 11 | 25 | 2 | 17 | 1 | 5 |  |  |  |  |  |
| typeof |  | 161 | 5 | 3 |  | 3 |  |  |  | 4 | 10 |  |  |  |  |
| Discard pattern |  | 14 | 1 | 15 | 27 | 67 | 20 | 28 | 9 | 1 |  | 2 |  |  |  |
| Switch expression |  | 12 | 1 | 15 | 27 | 65 | 19 | 28 | 9 | 1 |  | 2 |  |  |  |
| throw expression | 4 | 4 | 33 | 6 | 73 | 1 | 16 | 3 | 8 | 26 | 1 |  |  |  |  |
| Spread element | 19 | 9 | 9 | 2 | 35 | 25 | 6 | 36 | 9 | 13 | 1 |  |  |  |  |
| using statement | 36 |  | 2 | 115 | 7 |  |  |  |  |  | 1 |  |  |  |  |
| Field-like event |  | 61 |  |  |  |  | 93 |  | 4 |  |  |  |  |  |  |
| Discard _ | 22 | 25 | 59 | 1 | 8 | 9 | 2 | 9 | 6 | 1 | 1 |  |  |  |  |
| Case guard when |  | 6 |  | 1 | 3 | 100 |  | 3 |  | 2 |  |  |  |  |  |
| Interface |  | 2 |  |  | 1 |  | 1 | 67 | 27 |  |  |  |  |  |  |
| Verbatim string | 5 |  |  |  | 3 | 77 |  |  |  | 2 |  |  |  |  |  |
| as cast |  | 57 |  | 3 |  | 9 |  |  |  | 15 |  |  |  |  |  |
| Range .. | 3 |  | 16 | 15 | 2 | 30 |  | 18 |  |  |  |  |  |  |  |
| Relational pattern |  | 1 |  | 25 | 10 | 3 |  | 40 | 1 |  |  |  |  |  |  |
| Exception filter |  | 3 |  | 34 | 16 | 3 |  |  | 19 |  |  |  |  |  |  |
| Index from end ^ | 13 | 2 | 38 | 2 | 4 | 11 |  | 4 |  |  |  |  |  |  |  |
| Partial type |  |  | 4 |  |  | 7 |  |  |  | 26 |  |  | 35 |  |  |
| is type test | 1 | 21 |  | 2 | 3 | 32 |  |  |  |  |  |  |  |  |  |
| Switch statement |  | 5 |  | 8 |  | 20 | 2 | 19 | 1 | 2 |  |  |  |  |  |
| Generic method |  | 22 | 1 |  | 13 | 1 | 4 | 7 | 1 | 5 |  |  |  |  |  |
| yield return / break |  | 9 |  | 7 | 7 | 25 |  | 2 |  |  |  |  |  |  |  |
| Local const | 25 |  | 1 | 4 |  | 6 | 1 |  |  |  |  |  |  |  |  |
| finally | 6 | 1 |  | 2 | 11 | 2 | 4 |  | 4 |  | 1 | 2 |  |  |  |
| Enum |  | 2 |  |  |  |  | 11 | 19 |  |  |  |  |  |  |  |
| var pattern |  |  |  |  |  | 28 | 1 |  |  |  |  |  |  |  |  |
| Nested type | 8 | 6 | 1 |  |  |  |  |  |  | 10 |  |  |  |  |  |
| ref or out parameter | 2 | 1 | 1 | 1 | 6 | 3 |  | 4 | 3 | 4 |  |  |  |  |  |
| Constraint clause |  | 12 | 1 |  | 1 |  |  |  |  | 4 |  |  |  |  |  |
| Null-coalescing assignment ??= |  | 10 |  | 1 | 3 | 3 |  |  |  |  |  | 1 |  |  |  |
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
| Abstract class |  | 1 |  |  | 1 |  |  |  |  |  |  |  |  |  |  |
| checked / unchecked expression |  | 1 |  |  |  |  |  |  |  |  |  | 1 |  |  |  |
| Record struct |  | 1 |  |  |  | 1 |  |  |  |  |  |  |  |  |  |
| Static constructor |  | 1 |  |  |  |  |  |  |  | 1 |  |  |  |  |  |
| Static local function |  | 2 |  |  |  |  |  |  |  |  |  |  |  |  |  |
| Indexer |  | 1 |  |  |  |  |  |  |  |  |  |  |  |  |  |
| Using alias |  |  |  |  |  |  |  |  |  |  |  |  |  |  | 1 |

## Style A share per project

| Style | Llyn.Tests.Engine | Llyn.UIDeportment | Llyn.Tests.Conduct | Llyn.Infrastructure | Llyn.Application | Llyn.Tests.Convention | Llyn.Conduct | Llyn.Core | Llyn.ShellEngine | Llyn.Tests.Interface | Llyn.Tests.Windows | Llyn.Core.Windows | Llyn.UIVeneer | Llyn.UIDeportment.Capsule | Llyn.Host |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| Null check | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - | - | - | - |
| Object creation | 50.88 % | 48.51 % | 35.58 % | 25.27 % | 28.74 % | 42.00 % | 9.00 % | 26.74 % | 30.25 % | 31.03 % | 39.56 % | 7.69 % | - | 14.29 % | 21.05 % |
| Collection creation | 99.58 % | 94.84 % | 99.20 % | 97.86 % | 99.39 % | 99.05 % | 100.00 % | 100.00 % | 100.00 % | 99.56 % | 92.65 % | - | - | - | - |
| Member body | 2.08 % | 25.40 % | 2.76 % | 1.56 % | 10.93 % | 3.69 % | 17.47 % | 30.56 % | 16.21 % | 79.07 % | 36.49 % | 0.00 % | 0.00 % | 0.00 % | - |
| Local type | 0.00 % | 4.50 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.32 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | - | 0.00 % | 0.00 % |
| Using | 98.19 % | 100.00 % | 99.92 % | 83.41 % | 63.16 % | 100.00 % | - | - | - | 100.00 % | 95.45 % | - | - | - | - |
| Namespace | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - |
| Text building | 33.12 % | 52.78 % | 15.91 % | 58.09 % | 76.12 % | 82.07 % | 0.00 % | 26.09 % | - | 27.27 % | - | 0.00 % | - | 0.00 % | - |
| Branch | - | 70.59 % | 100.00 % | 65.22 % | 100.00 % | 76.47 % | 90.48 % | 59.57 % | 90.00 % | 33.33 % | - | 100.00 % | - | - | - |
| Lambda body | 96.59 % | 84.24 % | 79.35 % | 85.71 % | 92.08 % | 99.04 % | 97.32 % | 98.15 % | 91.30 % | 82.78 % | 38.71 % | 50.00 % | - | - | 77.78 % |
| Type test | 100.00 % | 91.27 % | 100.00 % | 90.63 % | 100.00 % | 94.51 % | 100.00 % | 100.00 % | 100.00 % | 40.00 % | 100.00 % | - | - | - | - |
| Constructor | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 36.36 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | - | 0.00 % | 0.00 % | - | - |
