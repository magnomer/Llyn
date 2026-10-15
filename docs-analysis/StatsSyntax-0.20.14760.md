# Syntax statistics - 0.20.14760

- Generated: 2026-10-15 23:36:52 +09:00
- Source roots: src, tests
- Root-level files: none
- Project segments: 1
- Excluded directories: .git, .vs, artifacts, bin, node_modules, obj, packages, publish
- Excluded suffixes: .g.cs, .g.i.cs, .AssemblyInfo.cs, .GlobalUsings.g.cs, .Designer.cs
- Roslyn: 4.14.0-3.25262.10
- Language version: Preview
- Scanned: 2,156 files, 15 projects, 227,831 lines

## Summary

| Measure | Value |
|---|---:|
| Files | 2,156 |
| Lines | 227,831 |
| Nodes | 1,214,301 |
| Syntax kinds used | 198 |
| Features used | 101 of 138 |
| Minimum C# version | 14 |
| Parse errors | 0 |

## Families

| Family | Features | Used | Count | Per 1k lines |
|---|---:|---:|---:|---:|
| Types | 15 | 12 | 4,471 | 19.62 |
| Namespaces | 6 | 3 | 2,158 | 9.47 |
| Members | 29 | 21 | 10,615 | 46.59 |
| Patterns | 16 | 16 | 7,022 | 30.82 |
| Expressions | 36 | 29 | 29,722 | 130.46 |
| Statements | 20 | 14 | 14,107 | 61.92 |
| Generics | 6 | 2 | 77 | 0.34 |
| Async | 2 | 2 | 682 | 2.99 |
| Nullability | 3 | 2 | 4,424 | 19.42 |
| Unsafe | 2 | 0 | 0 | 0.00 |
| Directives | 3 | 0 | 0 | 0.00 |
| Total | 138 | 101 | 73,278 | 321.63 |

## Projects

| Project | Files | Lines | Nodes | Features used | Newest C# |
|---|---:|---:|---:|---:|---:|
| Llyn.Tests.Engine | 247 | 39,792 | 278,035 | 61 | 12 |
| Llyn.UIDeportment | 331 | 36,192 | 166,933 | 88 | 14 |
| Llyn.Tests.Conduct | 164 | 29,953 | 218,738 | 61 | 12 |
| Llyn.Infrastructure | 193 | 25,954 | 119,672 | 70 | 12 |
| Llyn.Application | 136 | 20,900 | 98,921 | 73 | 12 |
| Llyn.Conduct | 296 | 19,393 | 76,625 | 70 | 12 |
| Llyn.Tests.Convention | 115 | 17,691 | 85,575 | 79 | 12 |
| Llyn.Core | 389 | 13,091 | 53,822 | 60 | 12 |
| Llyn.ShellEngine | 101 | 10,574 | 42,712 | 58 | 12 |
| Llyn.Tests.Interface | 100 | 10,015 | 50,878 | 72 | 12 |
| Llyn.Tests.Windows | 26 | 2,801 | 17,304 | 47 | 12 |
| Llyn.Core.Windows | 7 | 798 | 3,140 | 37 | 10 |
| Llyn.UIVeneer | 45 | 493 | 978 | 4 | 10 |
| Llyn.UIDeportment.Capsule | 5 | 96 | 427 | 15 | 10 |
| Llyn.Host | 1 | 88 | 541 | 4 | 9 |
| Total | 2,156 | 227,831 | 1,214,301 | 101 | 14 |

## Versions

| C# version | Features | Used | Count |
|---|---:|---:|---:|
| 1 | 51 | 35 | 18,897 |
| 2 | 10 | 8 | 2,374 |
| 3 | 7 | 5 | 7,325 |
| 4 | 3 | 2 | 859 |
| 5 | 3 | 3 | 1,577 |
| 6 | 6 | 5 | 5,280 |
| 7 | 11 | 10 | 7,009 |
| 7.1 | 1 | 1 | 8 |
| 7.2 | 1 | 0 | 0 |
| 7.3 | 1 | 0 | 0 |
| 8 | 17 | 13 | 11,895 |
| 9 | 10 | 9 | 7,158 |
| 10 | 3 | 2 | 2,158 |
| 11 | 6 | 3 | 673 |
| 12 | 4 | 4 | 8,061 |
| 13 | 1 | 0 | 0 |
| 14 | 3 | 1 | 4 |

## Features

| Feature | Family | C# | Count | Per 1k lines | Files | First |
|---|---|---|---:|---:|---:|---|
| Collection expression | Expressions | 12 | 7,817 | 34.31 | 965 | src/Llyn.Application/Card/LCardClerk.cs:60 |
| using declaration | Statements | 8 | 6,030 | 26.47 | 456 | src/Llyn.Application/Card/LTranslationClerk.cs:160 |
| Lambda | Expressions | 3 | 5,091 | 22.35 | 794 | src/Llyn.Application/Card/LCardClerk.cs:95 |
| if | Statements | 1 | 4,462 | 19.58 | 751 | src/Llyn.Application/Card/LCardClerk.cs:73 |
| Nullable reference type T? | Nullability | 8 | 3,697 | 16.23 | 1,027 | src/Llyn.Application/Card/LCardClerkField.cs:91 |
| Expression-bodied member | Members | 6 | 2,907 | 12.76 | 542 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Target-typed new | Expressions | 9 | 2,501 | 10.98 | 599 | src/Llyn.Application/Card/LMentionClerk.cs:153 |
| File-scoped namespace | Namespaces | 10 | 2,155 | 9.46 | 2,155 | src/Llyn.Application/Card/LCardClerk.cs:6 |
| Readonly field | Members | 1 | 2,147 | 9.42 | 654 | src/Llyn.Application/Card/LCardClerk.cs:10 |
| Declaration pattern | Patterns | 7 | 2,129 | 9.34 | 423 | src/Llyn.Application/Card/LMeaningClerk.cs:105 |
| Tuple literal or tuple type | Expressions | 7 | 1,786 | 7.84 | 305 | src/Llyn.Application/Card/LMeaningClerk.cs:26 |
| Sealed class | Types | 1 | 1,714 | 7.52 | 1,599 | src/Llyn.Application/Card/LCardClerk.cs:8 |
| Conditional ?: | Expressions | 1 | 1,548 | 6.79 | 574 | src/Llyn.Application/Card/LCardClerk.cs:170 |
| foreach | Statements | 1 | 1,537 | 6.75 | 504 | src/Llyn.Application/Card/LCardClerk.cs:62 |
| Class | Types | 1 | 1,527 | 6.70 | 1,509 | src/Llyn.Application/Card/LCardClerk.cs:8 |
| Static lambda | Expressions | 9 | 1,478 | 6.49 | 352 | src/Llyn.Application/Card/LCardClerk.cs:95 |
| Constant pattern | Patterns | 7 | 1,274 | 5.59 | 219 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Full property | Members | 1 | 1,259 | 5.53 | 375 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Null-conditional ?. ?[ | Expressions | 6 | 1,173 | 5.15 | 418 | src/Llyn.Application/Card/LMentionClerk.cs:47 |
| Null-coalescing ?? | Expressions | 2 | 1,079 | 4.74 | 422 | src/Llyn.Application/Card/LCardClerkField.cs:74 |
| not pattern | Patterns | 9 | 1,004 | 4.41 | 411 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| Auto property | Members | 3 | 955 | 4.19 | 282 | src/Llyn.Application/Citation/LCitationClerk.cs:41 |
| Null-forgiving ! | Expressions | 8 | 948 | 4.16 | 346 | src/Llyn.Application/Draft/Clerk/Card/LDraftClerkCard.cs:124 |
| with expression | Expressions | 9 | 899 | 3.95 | 287 | src/Llyn.Application/Card/LCardClerk.cs:115 |
| await | Expressions | 5 | 895 | 3.93 | 184 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:139 |
| is null | Patterns | 7 | 895 | 3.93 | 393 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| Extension method | Members | 3 | 750 | 3.29 | 44 | tests/Llyn.Tests.Interface/Atelier/TInterfaceArea.cs:7 |
| Nullable value type T? | Nullability | 2 | 727 | 3.19 | 313 | src/Llyn.Application/Card/LMeaningClerk.cs:104 |
| Interpolated string | Expressions | 6 | 721 | 3.16 | 180 | src/Llyn.Application/Localization/LLocalizationReader.cs:55 |
| Raw string | Expressions | 11 | 653 | 2.87 | 152 | src/Llyn.Infrastructure/Database/Base/LDatabase.cs:51 |
| Constructor | Members | 1 | 633 | 2.78 | 629 | src/Llyn.Application/Card/LCardClerk.cs:21 |
| Async method | Async | 5 | 627 | 2.75 | 184 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:91 |
| Const field | Members | 1 | 614 | 2.69 | 260 | src/Llyn.Application/Card/LMentionClerk.cs:11 |
| Cast expression | Expressions | 1 | 589 | 2.59 | 184 | src/Llyn.Application/Markup/LMarkupClerkLink.cs:228 |
| Record class | Types | 9 | 589 | 2.59 | 491 | src/Llyn.Application/Draft/Row/LDraftClerkAncestor.cs:263 |
| Property pattern | Patterns | 8 | 529 | 2.32 | 142 | src/Llyn.Application/Draft/Clerk/LDraftClerk.cs:83 |
| Object initializer | Expressions | 3 | 511 | 2.24 | 213 | src/Llyn.Application/Portrait/LPortraitClerkLabel.cs:25 |
| Optional parameter | Members | 4 | 493 | 2.16 | 164 | src/Llyn.Application/Draft/Clerk/LDraftClerkPanel.cs:170 |
| nameof | Expressions | 6 | 422 | 1.85 | 154 | src/Llyn.Application/Citation/LAuthorClerk.cs:361 |
| catch | Statements | 1 | 397 | 1.74 | 170 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:142 |
| lock | Statements | 1 | 391 | 1.72 | 61 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:98 |
| Deconstruction | Expressions | 7 | 370 | 1.62 | 161 | src/Llyn.Application/Card/LMeaningClerk.cs:46 |
| Named argument | Members | 4 | 366 | 1.61 | 105 | src/Llyn.Application/Card/LTranslationClerk.cs:164 |
| try | Statements | 1 | 366 | 1.61 | 188 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:137 |
| and / or pattern | Patterns | 9 | 357 | 1.57 | 107 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Static class | Types | 2 | 352 | 1.55 | 352 | src/Llyn.Application/Card/LCardClerkField.cs:7 |
| for | Statements | 1 | 294 | 1.29 | 186 | src/Llyn.Application/Card/LCardClerk.cs:280 |
| Spread element | Expressions | 12 | 237 | 1.04 | 132 | src/Llyn.Application/Card/LCardClerk.cs:95 |
| throw expression | Expressions | 7 | 235 | 1.03 | 115 | src/Llyn.Application/Card/LCardClerkField.cs:75 |
| Init accessor | Members | 9 | 229 | 1.01 | 80 | src/Llyn.Conduct/Lexicon/CExample.cs:20 |
| Discard pattern | Patterns | 8 | 223 | 0.98 | 131 | src/Llyn.Application/Citation/LAuthorClerk.cs:243 |
| Switch expression | Patterns | 8 | 220 | 0.97 | 132 | src/Llyn.Application/Citation/LAuthorClerk.cs:239 |
| while | Statements | 1 | 216 | 0.95 | 132 | src/Llyn.Application/Card/LMeaningClerk.cs:44 |
| typeof | Expressions | 1 | 195 | 0.86 | 43 | src/Llyn.Infrastructure/Configuration/LLocalizationLoader.cs:20 |
| using statement | Statements | 1 | 188 | 0.83 | 79 | src/Llyn.Application/Citation/LAuthorCitation.cs:51 |
| Field-like event | Members | 1 | 174 | 0.76 | 107 | src/Llyn.Conduct/Atelier/CWorkspace.cs:40 |
| Discard _ | Expressions | 7 | 157 | 0.69 | 63 | src/Llyn.Application/Draft/Clerk/Card/LDraftClerkCard.cs:127 |
| Case guard when | Patterns | 7 | 116 | 0.51 | 25 | src/Llyn.Application/Citation/LAuthorClerk.cs:241 |
| Interface | Types | 1 | 111 | 0.49 | 111 | src/Llyn.Application/Pronunciation/Relay/LListener.cs:5 |
| Index from end ^ | Expressions | 8 | 97 | 0.43 | 51 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:230 |
| Verbatim string | Expressions | 1 | 93 | 0.41 | 20 | src/Llyn.Application/Localization/LLocalizationReader.cs:13 |
| Range .. | Expressions | 8 | 92 | 0.40 | 49 | src/Llyn.Application/Card/LMentionClerk.cs:159 |
| Relational pattern | Patterns | 9 | 90 | 0.40 | 31 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| as cast | Expressions | 1 | 86 | 0.38 | 43 | src/Llyn.Infrastructure/Database/Base/Workspace/LWorkspaceArchive.cs:132 |
| Partial type | Types | 2 | 83 | 0.36 | 83 | src/Llyn.UIVeneer/Display/View/PDisplay.xaml.cs:5 |
| is type test | Patterns | 1 | 60 | 0.26 | 37 | src/Llyn.Application/Outpost/LCourierClerk.cs:288 |
| Switch statement | Patterns | 1 | 60 | 0.26 | 37 | src/Llyn.Application/Portrait/LPortraitClerkPress.cs:59 |
| Generic method | Generics | 2 | 59 | 0.26 | 31 | src/Llyn.Application/Card/LCardClerkField.cs:183 |
| Exception filter | Statements | 6 | 57 | 0.25 | 39 | src/Llyn.Application/Outpost/LCourierClerk.cs:196 |
| Async lambda | Async | 5 | 55 | 0.24 | 10 | src/Llyn.Application/Outpost/LCourierClerk.cs:251 |
| finally | Statements | 1 | 52 | 0.23 | 37 | src/Llyn.Application/Outpost/LCourierClerk.cs:76 |
| yield return / break | Statements | 2 | 52 | 0.23 | 21 | src/Llyn.Application/Card/LCardClerkField.cs:35 |
| Local const | Statements | 1 | 51 | 0.22 | 27 | src/Llyn.Application/Markup/LMarkupClerkIntake.cs:271 |
| Enum | Types | 1 | 48 | 0.21 | 48 | src/Llyn.Conduct/Catalog/Vista/CCatalogOrder.cs:3 |
| Nested type | Types | 1 | 32 | 0.14 | 19 | src/Llyn.Application/Draft/Row/LDraftClerkAncestor.cs:263 |
| ref or out parameter | Members | 1 | 30 | 0.13 | 23 | src/Llyn.Application/Draft/Clerk/Card/LDraftClerkCard.cs:162 |
| var pattern | Patterns | 7 | 30 | 0.13 | 12 | src/Llyn.Conduct/Card/CFolio.cs:219 |
| Null-coalescing assignment ??= | Expressions | 8 | 21 | 0.09 | 19 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:160 |
| Collection initializer | Expressions | 3 | 18 | 0.08 | 16 | src/Llyn.Application/Outpost/LCourierClerk.cs:177 |
| Constraint clause | Generics | 2 | 18 | 0.08 | 10 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:171 |
| Local function | Members | 7 | 17 | 0.07 | 11 | src/Llyn.Conduct/Desk/CErrand.cs:56 |
| List pattern | Patterns | 11 | 15 | 0.07 | 13 | src/Llyn.Conduct/Panel/Rehearsal/Sheaf/CCue.cs:99 |
| await using | Statements | 8 | 14 | 0.06 | 7 | src/Llyn.Conduct/Portrait/CPortrait.cs:144 |
| Conversion operator | Members | 1 | 13 | 0.06 | 2 | src/Llyn.Core/Lexicon/State/LStateValue.cs:49 |
| Default interface method body | Members | 8 | 13 | 0.06 | 7 | src/Llyn.ShellEngine/Port/Entry/LEntryPort.cs:21 |
| Type pattern | Patterns | 9 | 11 | 0.05 | 4 | src/Llyn.UIDeportment/Sound/Accent/QAccentControl.cs:62 |
| Positional pattern | Patterns | 8 | 9 | 0.04 | 9 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:37 |
| default literal | Expressions | 7.1 | 8 | 0.04 | 6 | src/Llyn.Infrastructure/Pronunciation/Loader/LSourceLoader.cs:73 |
| Primary constructor on class or struct | Types | 12 | 6 | 0.03 | 6 | src/Llyn.Conduct/Dialog/Galley/CSGalleyDelay.cs:7 |
| Required member | Members | 11 | 5 | 0.02 | 3 | src/Llyn.UIDeportment/Panel/Taxonomy/QMembershipItem.cs:27 |
| field keyword in accessor | Members | 14 | 4 | 0.02 | 1 | src/Llyn.UIDeportment/Panel/Dialog/Galley/QSGalleyItem.cs:31 |
| Generic type | Types | 2 | 4 | 0.02 | 4 | src/Llyn.Conduct/Configuration/CEnsignSheet.cs:5 |
| Record struct | Types | 10 | 3 | 0.01 | 3 | src/Llyn.Core/Lexicon/Inflection/LInflectionLetter.cs:3 |
| Abstract class | Types | 1 | 2 | 0.01 | 2 | src/Llyn.Application/Request/Entry/LRequest.cs:3 |
| checked / unchecked expression | Expressions | 1 | 2 | 0.01 | 2 | src/Llyn.Core.Windows/LPressBrowser.cs:14 |
| Static constructor | Members | 1 | 2 | 0.01 | 2 | src/Llyn.UIDeportment/Kit/Bind/Look/QLookItem.cs:23 |
| Static local function | Members | 8 | 2 | 0.01 | 1 | src/Llyn.UIDeportment/Kit/Asset/QIcon.cs:144 |
| Using alias | Namespaces | 1 | 2 | 0.01 | 2 | src/Llyn.Host/LHost.cs:11 |
| Alias of any type | Namespaces | 12 | 1 | 0.00 | 1 | tests/Llyn.Tests.Interface/Entry/TInterfaceInflection.cs:2 |
| Event accessors | Members | 1 | 1 | 0.00 | 1 | src/Llyn.UIDeportment/Mention/Text/QMention.cs:46 |
| Indexer | Members | 1 | 1 | 0.00 | 1 | src/Llyn.UIDeportment/Kit/Asset/QLocalizationCatalog.cs:19 |

## Styles

| Style | A | A count | B | B count | A share |
|---|---|---:|---|---:|---:|
| Null check | is null / is not null | 895 | == null / != null | 0 | 100.00 % |
| Object creation | Target-typed new() | 2,501 | new T() | 4,416 | 36.16 % |
| Collection creation | Collection expression | 7,817 | Collection initializer or array creation | 74 | 99.06 % |
| Member body | Expression body | 2,907 | Block body | 11,301 | 20.46 % |
| Local type | var | 47 | Explicit type | 23,937 | 0.20 % |
| Using | Declaration | 6,030 | Statement | 188 | 96.98 % |
| Namespace | File-scoped | 2,155 | Block | 0 | 100.00 % |
| Text building | Interpolated string | 721 | string.Format or + with a string literal | 601 | 54.54 % |
| Branch | Switch expression | 220 | Switch statement | 60 | 78.57 % |
| Lambda body | Expression | 4,485 | Block | 606 | 88.10 % |
| Type test | is pattern with designation | 1,247 | as | 86 | 93.55 % |
| Constructor | Primary | 6 | Explicit | 629 | 0.94 % |

## Features unused (37)

| Feature | Family | C# |
|---|---|---|
| Struct | Types | 1 |
| Delegate | Types | 1 |
| File-local type | Types | 11 |
| Block namespace | Namespaces | 1 |
| Global using | Namespaces | 10 |
| Using static | Namespaces | 6 |
| Readonly struct member | Members | 8 |
| Operator overload | Members | 1 |
| Extension block | Members | 14 |
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
| IdentifierName | 350,987 | 2,156 |
| Argument | 126,228 | 1,617 |
| SimpleMemberAccessExpression | 115,776 | 1,602 |
| ArgumentList | 78,507 | 1,662 |
| InvocationExpression | 71,757 | 1,620 |
| PredefinedType | 34,496 | 1,996 |
| ExpressionStatement | 34,208 | 1,503 |
| StringLiteralExpression | 28,093 | 1,196 |
| Parameter | 25,152 | 1,927 |
| VariableDeclaration | 24,776 | 1,409 |
| VariableDeclarator | 24,776 | 1,409 |
| EqualsValueClause | 23,579 | 1,467 |
| LocalDeclarationStatement | 20,949 | 1,225 |
| Block | 19,927 | 1,586 |
| ParameterList | 15,182 | 2,067 |
| MethodDeclaration | 12,827 | 1,647 |
| NumericLiteralExpression | 12,630 | 1,180 |
| GenericName | 11,777 | 1,477 |
| TypeArgumentList | 11,777 | 1,477 |
| QualifiedName | 10,175 | 2,156 |
| ExpressionElement | 8,525 | 535 |
| SimpleAssignmentExpression | 8,310 | 1,108 |
| CollectionExpression | 7,817 | 965 |
| UsingDirective | 6,899 | 1,838 |
| ReturnStatement | 6,410 | 1,168 |
| NullLiteralExpression | 4,716 | 977 |
| IfStatement | 4,462 | 751 |
| NullableType | 4,424 | 1,107 |
| ObjectCreationExpression | 4,416 | 966 |
| SimpleLambdaExpression | 3,998 | 717 |
| Attribute | 3,934 | 468 |
| AttributeList | 3,934 | 468 |
| SingleVariableDesignation | 3,617 | 616 |
| BracketedArgumentList | 3,441 | 599 |
| FieldDeclaration | 3,180 | 827 |
| IsPatternExpression | 3,041 | 638 |
| ArrowExpressionClause | 2,909 | 543 |
| ElementAccessExpression | 2,812 | 550 |
| ImplicitObjectCreationExpression | 2,501 | 599 |
| FalseLiteralExpression | 2,241 | 595 |
| PropertyDeclaration | 2,220 | 549 |
| ConstantPattern | 2,169 | 495 |
| CompilationUnit | 2,156 | 2,156 |
| FileScopedNamespaceDeclaration | 2,155 | 2,155 |
| DeclarationPattern | 2,129 | 423 |
| TrueLiteralExpression | 1,889 | 554 |
| EqualsExpression | 1,802 | 643 |
| TupleExpression | 1,742 | 300 |
| AddExpression | 1,737 | 393 |
| InterpolatedStringText | 1,628 | 180 |
| ConditionalExpression | 1,548 | 574 |
| ClassDeclaration | 1,527 | 1,509 |
| LogicalAndExpression | 1,508 | 444 |
| ForEachStatement | 1,446 | 486 |
| Interpolation | 1,427 | 179 |
| LogicalNotExpression | 1,289 | 439 |
| AddAssignmentExpression | 1,284 | 296 |
| AttributeArgument | 1,252 | 63 |
| SwitchExpressionArm | 1,196 | 132 |
| ConditionalAccessExpression | 1,173 | 418 |
| MemberBindingExpression | 1,173 | 418 |
| DeclarationExpression | 1,167 | 303 |
| ParenthesizedLambdaExpression | 1,093 | 314 |
| CoalesceExpression | 1,079 | 422 |
| AccessorList | 1,065 | 309 |
| GetAccessorDeclaration | 1,063 | 308 |
| LogicalOrExpression | 1,013 | 327 |
| NotPattern | 1,004 | 411 |
| TupleElement | 982 | 225 |
| SuppressNullableWarningExpression | 948 | 346 |
| WithExpression | 899 | 287 |
| WithInitializerExpression | 899 | 287 |
| AwaitExpression | 895 | 184 |
| NameColon | 787 | 227 |
| GreaterThanExpression | 786 | 336 |
| InterpolatedStringExpression | 721 | 180 |
| ArrayRankSpecifier | 686 | 280 |
| ArrayType | 682 | 280 |
| OmittedArraySizeExpression | 664 | 280 |
| ConstructorDeclaration | 635 | 631 |
| ImplicitElementAccess | 629 | 121 |
| CastExpression | 589 | 184 |
| RecordDeclaration | 589 | 491 |
| PostIncrementExpression | 572 | 267 |
| LessThanExpression | 566 | 255 |
| CharacterLiteralExpression | 548 | 132 |
| RecursivePattern | 538 | 149 |
| PropertyPatternClause | 529 | 142 |
| AttributeArgumentList | 513 | 63 |
| ObjectInitializerExpression | 511 | 213 |
| NotEqualsExpression | 494 | 214 |
| Subpattern | 474 | 130 |
| ParenthesizedExpression | 432 | 212 |
| TupleType | 416 | 225 |
| BaseList | 414 | 313 |
| CatchClause | 397 | 170 |
| CatchDeclaration | 394 | 168 |
| LockStatement | 391 | 61 |
| TryStatement | 366 | 188 |
| ContinueStatement | 338 | 165 |
| SwitchSection | 334 | 37 |
| BreakStatement | 321 | 68 |
| SimpleBaseType | 315 | 294 |
| OrPattern | 314 | 101 |
| ForStatement | 294 | 186 |
| EnumMemberDeclaration | 292 | 48 |
| SubtractExpression | 243 | 111 |
| UnaryMinusExpression | 243 | 98 |
| SpreadElement | 237 | 132 |
| ThrowExpression | 235 | 115 |
| ThrowStatement | 230 | 124 |
| InitAccessorDeclaration | 229 | 80 |
| DiscardPattern | 223 | 131 |
| SwitchExpression | 220 | 132 |
| WhileStatement | 216 | 132 |
| SubtractAssignmentExpression | 210 | 77 |
| TypeOfExpression | 195 | 43 |
| UsingStatement | 188 | 79 |
| CaseSwitchLabel | 183 | 24 |
| EventFieldDeclaration | 174 | 107 |
| ElseClause | 167 | 128 |
| SetAccessorDeclaration | 165 | 82 |
| GreaterThanOrEqualExpression | 139 | 86 |
| ThisExpression | 132 | 88 |
| LessThanOrEqualExpression | 125 | 91 |
| WhenClause | 116 | 25 |
| CasePatternSwitchLabel | 114 | 23 |
| InterfaceDeclaration | 111 | 111 |
| PrimaryConstructorBaseType | 110 | 19 |
| MultiplyExpression | 105 | 45 |
| IndexExpression | 97 | 51 |
| RangeExpression | 92 | 49 |
| ForEachVariableStatement | 91 | 67 |
| RelationalPattern | 90 | 31 |
| AsExpression | 86 | 43 |
| ArrayInitializerExpression | 76 | 42 |
| TypeParameter | 68 | 35 |
| TypeParameterList | 63 | 35 |
| IsExpression | 60 | 37 |
| SwitchStatement | 60 | 37 |
| DivideExpression | 58 | 29 |
| CatchFilterClause | 57 | 39 |
| ImplicitArrayCreationExpression | 54 | 39 |
| FinallyClause | 52 | 37 |
| BitwiseOrExpression | 49 | 25 |
| EnumDeclaration | 48 | 48 |
| ParenthesizedPattern | 45 | 23 |
| YieldReturnStatement | 45 | 21 |
| AndPattern | 43 | 19 |
| DefaultSwitchLabel | 43 | 25 |
| ExpressionColon | 37 | 17 |
| OrAssignmentExpression | 35 | 20 |
| ArrayCreationExpression | 34 | 23 |
| LeftShiftExpression | 34 | 3 |
| VarPattern | 30 | 12 |
| CoalesceAssignmentExpression | 21 | 19 |
| AttributeTargetSpecifier | 19 | 8 |
| CollectionInitializerExpression | 18 | 16 |
| TypeParameterConstraintClause | 18 | 10 |
| ClassConstraint | 17 | 9 |
| LocalFunctionStatement | 17 | 11 |
| PostDecrementExpression | 16 | 12 |
| ListPattern | 15 | 13 |
| BaseExpression | 13 | 5 |
| ConversionOperatorDeclaration | 13 | 2 |
| PreIncrementExpression | 11 | 9 |
| TypePattern | 11 | 4 |
| PositionalPatternClause | 9 | 9 |
| DefaultLiteralExpression | 8 | 6 |
| NameEquals | 8 | 5 |
| InterpolationFormatClause | 7 | 1 |
| YieldBreakStatement | 7 | 6 |
| BaseConstructorInitializer | 6 | 6 |
| ExplicitInterfaceSpecifier | 6 | 5 |
| GlobalStatement | 6 | 1 |
| BitwiseAndExpression | 4 | 4 |
| FieldExpression | 4 | 1 |
| AndAssignmentExpression | 3 | 3 |
| ExclusiveOrExpression | 3 | 2 |
| InterpolationAlignmentClause | 3 | 1 |
| RecordStructDeclaration | 3 | 3 |
| SlicePattern | 3 | 3 |
| ComplexElementInitializerExpression | 2 | 1 |
| DiscardDesignation | 2 | 2 |
| ModuloExpression | 2 | 1 |
| RightShiftExpression | 2 | 1 |
| UncheckedExpression | 2 | 2 |
| AddAccessorDeclaration | 1 | 1 |
| BitwiseNotExpression | 1 | 1 |
| BracketedParameterList | 1 | 1 |
| EventDeclaration | 1 | 1 |
| ExclusiveOrAssignmentExpression | 1 | 1 |
| IndexerDeclaration | 1 | 1 |
| LeftShiftAssignmentExpression | 1 | 1 |
| OmittedTypeArgument | 1 | 1 |
| RemoveAccessorDeclaration | 1 | 1 |
| ThisConstructorInitializer | 1 | 1 |
| TypeConstraint | 1 | 1 |

## Attributes

| Attribute | Count | Files |
|---|---:|---:|
| Fact | 3,316 | 453 |
| InlineData | 485 | 51 |
| Theory | 103 | 53 |
| JsonPropertyName | 14 | 4 |
| DllImport | 4 | 1 |
| MemberData | 3 | 2 |
| CollectionBehavior | 2 | 2 |
| MarshalAs | 2 | 1 |
| Flags | 1 | 1 |
| NotNullIfNotNull | 1 | 1 |
| NotNullWhen | 1 | 1 |
| Out | 1 | 1 |
| SupportedOSPlatform | 1 | 1 |

## Parse errors (0)

None.

## Feature counts per project

| Feature | Llyn.Tests.Engine | Llyn.UIDeportment | Llyn.Tests.Conduct | Llyn.Infrastructure | Llyn.Application | Llyn.Conduct | Llyn.Tests.Convention | Llyn.Core | Llyn.ShellEngine | Llyn.Tests.Interface | Llyn.Tests.Windows | Llyn.Core.Windows | Llyn.UIVeneer | Llyn.UIDeportment.Capsule | Llyn.Host |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| Collection expression | 3,325 | 184 | 2,036 | 320 | 404 | 230 | 513 | 304 | 69 | 261 | 171 |  |  |  |  |
| using declaration | 2,379 | 5 | 2,874 | 651 | 17 |  | 7 |  |  | 22 | 66 | 9 |  |  |  |
| Lambda | 968 | 234 | 1,541 | 44 | 475 | 469 | 740 | 190 | 80 | 249 | 85 | 7 |  |  | 9 |
| if | 54 | 1,399 | 6 | 728 | 770 | 450 | 471 | 287 | 229 | 48 | 6 | 11 |  | 3 |  |
| Nullable reference type T? | 190 | 510 | 380 | 393 | 392 | 367 | 300 | 365 | 274 | 468 | 44 | 9 |  | 5 |  |
| Expression-bodied member | 54 | 751 | 37 | 22 | 120 | 205 | 27 | 169 | 162 | 1,322 | 37 | 1 |  |  |  |
| Target-typed new | 206 | 756 | 375 | 153 | 193 | 50 | 189 | 65 | 96 | 306 | 96 | 11 |  | 1 | 4 |
| File-scoped namespace | 247 | 331 | 164 | 193 | 136 | 296 | 115 | 389 | 101 | 100 | 26 | 7 | 45 | 5 |  |
| Readonly field | 43 | 640 | 4 | 128 | 453 | 385 | 215 | 31 | 186 | 46 | 6 | 9 |  | 1 |  |
| Declaration pattern | 7 | 1,076 | 4 | 64 | 225 | 258 | 369 | 33 | 77 | 11 | 3 | 2 |  |  |  |
| Tuple literal or tuple type | 469 | 51 | 295 | 388 | 115 | 83 | 164 | 60 | 91 | 42 | 28 |  |  |  |  |
| Sealed class | 253 | 290 | 164 | 83 | 204 | 274 | 43 | 253 | 72 | 43 | 23 | 7 |  | 5 |  |
| Conditional ?: | 16 | 229 | 18 | 354 | 311 | 177 | 156 | 171 | 76 | 35 | 3 | 2 |  |  |  |
| foreach | 56 | 216 | 17 | 309 | 354 | 30 | 305 | 166 | 32 | 48 | 2 | 2 |  |  |  |
| Class | 256 | 315 | 163 | 191 | 114 | 101 | 108 | 30 | 62 | 108 | 26 | 7 | 45 | 1 |  |
| Static lambda | 138 | 101 | 728 | 11 | 176 | 98 | 15 | 98 | 15 | 89 | 8 |  |  |  | 1 |
| Constant pattern | 6 | 153 | 8 | 144 | 101 | 206 | 358 | 213 | 49 | 20 |  | 16 |  |  |  |
| Full property | 7 | 706 | 5 | 6 | 118 | 191 | 2 | 147 | 46 | 30 |  | 1 |  |  |  |
| Null-conditional ?. ?[ | 132 | 205 | 99 | 63 | 60 | 376 | 91 | 43 | 88 | 9 | 7 |  |  |  |  |
| Null-coalescing ?? | 22 | 82 | 24 | 141 | 235 | 81 | 82 | 274 | 55 | 66 | 11 | 5 |  | 1 |  |
| not pattern | 6 | 365 | 1 | 64 | 95 | 177 | 185 | 39 | 58 | 11 | 1 | 2 |  |  |  |
| Auto property | 5 | 407 | 3 | 8 | 7 | 149 | 23 | 246 | 60 | 47 |  |  |  |  |  |
| Null-forgiving ! | 257 | 162 | 300 | 67 | 16 | 10 | 28 | 5 | 4 | 80 | 19 |  |  |  |  |
| with expression | 322 | 8 | 41 | 54 | 298 | 40 | 10 | 30 | 49 | 47 |  |  |  |  |  |
| await | 286 | 63 | 326 | 63 | 69 | 23 | 1 |  | 24 | 14 | 1 | 25 |  |  |  |
| is null | 10 | 148 | 6 | 109 | 196 | 100 | 174 | 65 | 65 | 19 | 2 | 1 |  |  |  |
| Extension method |  |  |  |  |  |  |  |  |  | 745 | 5 |  |  |  |  |
| Nullable value type T? | 35 | 72 | 31 | 47 | 57 | 157 | 11 | 64 | 138 | 94 | 19 |  |  | 2 |  |
| Interpolated string | 58 | 19 | 7 | 171 | 60 | 1 | 384 | 7 |  | 8 | 6 |  |  |  |  |
| Raw string | 157 | 2 | 7 | 239 |  |  | 235 |  |  | 8 | 5 |  |  |  |  |
| Constructor | 2 | 250 | 1 | 74 | 89 | 87 | 7 | 3 | 61 | 10 |  | 4 | 45 |  |  |
| Async method | 222 | 62 | 204 | 33 | 40 | 21 | 1 |  | 20 | 10 | 1 | 13 |  |  |  |
| Const field | 88 | 75 | 15 | 195 | 17 | 5 | 112 | 81 | 4 | 12 | 6 | 3 |  | 1 |  |
| Cast expression | 15 | 132 | 105 | 119 | 4 | 24 | 50 | 4 |  | 53 | 77 | 6 |  |  |  |
| Record class |  | 18 | 2 | 2 | 114 | 181 | 6 | 250 | 10 | 2 |  |  |  | 4 |  |
| Property pattern | 2 | 183 |  | 10 | 18 | 24 | 274 | 5 | 6 | 5 | 2 |  |  |  |  |
| Object initializer | 67 | 136 | 104 | 32 | 4 |  | 59 | 3 | 1 | 38 | 64 | 2 |  | 1 |  |
| Optional parameter | 15 | 8 | 6 | 6 | 12 | 12 | 1 | 295 | 12 | 122 |  |  |  | 4 |  |
| nameof | 1 | 205 | 3 | 13 | 130 | 33 | 17 | 8 | 5 | 2 | 1 | 4 |  |  |  |
| catch | 2 | 21 |  | 139 | 32 | 155 | 7 | 5 | 11 | 1 | 8 | 13 |  | 3 |  |
| lock |  | 14 |  | 2 | 85 | 2 | 5 |  | 278 | 5 |  |  |  |  |  |
| Deconstruction | 77 | 38 | 121 | 35 | 30 | 24 | 30 | 9 | 5 |  | 1 |  |  |  |  |
| Named argument | 174 |  | 39 | 16 | 31 |  | 16 | 23 | 3 | 64 |  |  |  |  |  |
| try | 9 | 22 |  | 86 | 32 | 158 | 9 | 5 | 14 | 3 | 12 | 15 |  | 1 |  |
| and / or pattern | 3 | 47 | 3 | 68 | 18 | 3 | 161 | 43 | 2 | 8 |  | 1 |  |  |  |
| Static class | 3 | 42 | 1 | 110 | 23 | 8 | 71 | 27 |  | 64 | 3 |  |  |  |  |
| for | 18 | 44 | 6 | 70 | 62 | 9 | 29 | 26 | 7 | 21 | 2 |  |  |  |  |
| Spread element | 39 | 16 | 10 | 7 | 57 | 13 | 29 | 41 | 7 | 16 | 2 |  |  |  |  |
| throw expression | 5 | 7 | 30 | 9 | 96 | 33 | 1 | 7 | 11 | 27 | 1 | 8 |  |  |  |
| Init accessor |  | 2 |  |  |  | 1 | 3 | 223 |  |  |  |  |  |  |  |
| Discard pattern |  | 13 | 1 | 19 | 32 | 37 | 67 | 38 | 10 | 1 |  | 5 |  |  |  |
| Switch expression |  | 11 | 1 | 19 | 32 | 38 | 65 | 38 | 10 | 1 |  | 5 |  |  |  |
| while | 22 | 9 | 5 | 107 | 18 | 3 | 25 | 21 | 1 | 5 |  |  |  |  |  |
| typeof |  | 141 | 5 | 4 |  |  | 3 |  |  | 4 | 38 |  |  |  |  |
| using statement | 37 | 1 | 2 | 135 | 7 | 2 |  |  |  | 1 | 1 | 2 |  |  |  |
| Field-like event |  | 70 |  |  |  | 101 |  |  | 3 |  |  |  |  |  |  |
| Discard _ | 34 | 28 | 49 | 5 | 8 | 3 | 10 | 10 | 7 | 1 | 2 |  |  |  |  |
| Case guard when |  | 6 |  | 1 | 1 | 1 | 100 | 5 |  | 2 |  |  |  |  |  |
| Interface |  | 2 |  |  | 1 | 1 |  | 78 | 29 |  |  |  |  |  |  |
| Index from end ^ | 17 | 5 | 51 | 1 | 4 | 2 | 9 | 8 |  |  |  |  |  |  |  |
| Verbatim string | 9 |  |  |  | 3 |  | 79 |  |  | 2 |  |  |  |  |  |
| Range .. | 11 |  | 12 | 16 | 2 |  | 29 | 21 |  | 1 |  |  |  |  |  |
| Relational pattern |  | 7 |  | 28 | 8 | 1 | 5 | 40 | 1 |  |  |  |  |  |  |
| as cast |  | 56 |  | 3 |  |  | 9 |  |  | 17 | 1 |  |  |  |  |
| Partial type |  |  | 4 |  |  |  | 7 |  |  | 27 |  |  | 45 |  |  |
| is type test | 1 | 22 |  | 2 | 3 |  | 32 |  |  |  |  |  |  |  |  |
| Switch statement |  | 8 |  | 8 | 1 | 2 | 20 | 18 | 1 | 2 |  |  |  |  |  |
| Generic method |  | 25 | 1 |  | 14 | 5 | 1 | 7 | 1 | 5 |  |  |  |  |  |
| Exception filter |  | 3 |  | 33 | 16 |  | 2 |  | 3 |  |  |  |  |  |  |
| Async lambda |  |  | 53 |  | 1 |  |  |  |  | 1 |  |  |  |  |  |
| finally | 7 | 4 |  | 2 | 14 | 6 | 2 |  | 5 | 2 | 4 | 6 |  |  |  |
| yield return / break |  | 9 |  | 7 | 9 |  | 25 | 2 |  |  |  |  |  |  |  |
| Local const | 37 |  | 1 | 4 | 1 | 1 | 6 |  | 1 |  |  |  |  |  |  |
| Enum |  | 2 |  |  |  | 16 |  | 30 |  |  |  |  |  |  |  |
| Nested type | 9 | 7 | 1 |  | 2 | 3 |  |  |  | 10 |  |  |  |  |  |
| ref or out parameter | 2 | 1 | 1 | 1 | 8 |  | 3 | 5 | 4 | 5 |  |  |  |  |  |
| var pattern |  |  |  |  |  | 1 | 28 |  |  |  | 1 |  |  |  |  |
| Null-coalescing assignment ??= | 1 | 10 |  | 1 | 4 | 1 | 3 |  |  |  |  | 1 |  |  |  |
| Collection initializer |  | 2 | 3 | 5 | 3 | 1 |  |  |  | 2 | 2 |  |  |  |  |
| Constraint clause |  | 12 | 1 |  | 1 |  |  |  |  | 4 |  |  |  |  |  |
| Local function | 1 | 4 |  |  |  | 5 | 4 |  |  | 1 | 1 | 1 |  |  |  |
| List pattern | 1 | 4 |  |  |  | 1 | 8 |  | 1 |  |  |  |  |  |  |
| await using | 12 |  |  |  |  | 1 |  |  |  |  |  | 1 |  |  |  |
| Conversion operator |  | 12 |  |  |  |  |  | 1 |  |  |  |  |  |  |  |
| Default interface method body |  |  |  |  |  |  |  |  | 13 |  |  |  |  |  |  |
| Type pattern |  | 1 |  |  |  |  | 10 |  |  |  |  |  |  |  |  |
| Positional pattern |  | 2 |  | 1 | 1 | 4 | 1 |  |  |  |  |  |  |  |  |
| default literal |  | 1 | 1 | 1 |  |  | 5 |  |  |  |  |  |  |  |  |
| Primary constructor on class or struct |  |  |  |  |  | 1 | 4 |  |  | 1 |  |  |  |  |  |
| Required member |  | 2 |  |  |  |  | 3 |  |  |  |  |  |  |  |  |
| field keyword in accessor |  | 4 |  |  |  |  |  |  |  |  |  |  |  |  |  |
| Generic type |  | 2 |  |  |  | 2 |  |  |  |  |  |  |  |  |  |
| Record struct |  | 1 |  |  |  |  | 1 | 1 |  |  |  |  |  |  |  |
| Abstract class |  | 1 |  |  | 1 |  |  |  |  |  |  |  |  |  |  |
| checked / unchecked expression |  | 1 |  |  |  |  |  |  |  |  |  | 1 |  |  |  |
| Static constructor |  | 1 |  |  |  |  |  |  |  | 1 |  |  |  |  |  |
| Static local function |  | 2 |  |  |  |  |  |  |  |  |  |  |  |  |  |
| Using alias |  |  |  |  |  |  |  |  |  | 1 |  |  |  |  | 1 |
| Alias of any type |  |  |  |  |  |  |  |  |  | 1 |  |  |  |  |  |
| Event accessors |  | 1 |  |  |  |  |  |  |  |  |  |  |  |  |  |
| Indexer |  | 1 |  |  |  |  |  |  |  |  |  |  |  |  |  |

## Style A share per project

| Style | Llyn.Tests.Engine | Llyn.UIDeportment | Llyn.Tests.Conduct | Llyn.Infrastructure | Llyn.Application | Llyn.Conduct | Llyn.Tests.Convention | Llyn.Core | Llyn.ShellEngine | Llyn.Tests.Interface | Llyn.Tests.Windows | Llyn.Core.Windows | Llyn.UIVeneer | Llyn.UIDeportment.Capsule | Llyn.Host |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| Null check | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - | - | - |
| Object creation | 52.28 % | 47.76 % | 40.98 % | 24.13 % | 30.98 % | 9.54 % | 42.00 % | 25.69 % | 30.97 % | 33.15 % | 39.34 % | 30.56 % | - | 14.29 % | 19.05 % |
| Collection creation | 99.49 % | 97.35 % | 99.27 % | 98.16 % | 98.78 % | 99.14 % | 99.03 % | 99.02 % | 100.00 % | 99.62 % | 92.43 % | 0.00 % | - | - | - |
| Member body | 2.67 % | 25.78 % | 2.40 % | 1.97 % | 10.40 % | 15.39 % | 3.80 % | 28.89 % | 18.00 % | 78.13 % | 23.27 % | 2.63 % | 0.00 % | 0.00 % | - |
| Local type | 0.04 % | 3.83 % | 0.00 % | 0.00 % | 0.11 % | 0.22 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | - | 0.00 % | 0.00 % |
| Using | 98.47 % | 83.33 % | 99.93 % | 82.82 % | 70.83 % | 0.00 % | 100.00 % | - | - | 95.65 % | 98.51 % | 81.82 % | - | - | - |
| Namespace | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - |
| Text building | 27.75 % | 46.34 % | 13.73 % | 47.50 % | 69.77 % | 2.44 % | 81.70 % | 21.88 % | - | 38.10 % | 85.71 % | 0.00 % | - | 0.00 % | - |
| Branch | - | 57.89 % | 100.00 % | 70.37 % | 96.97 % | 95.00 % | 76.47 % | 67.86 % | 90.91 % | 33.33 % | - | 100.00 % | - | - | - |
| Lambda body | 96.38 % | 87.61 % | 76.31 % | 90.91 % | 93.89 % | 96.38 % | 98.92 % | 98.42 % | 92.50 % | 79.92 % | 32.94 % | 85.71 % | - | - | 77.78 % |
| Type test | 100.00 % | 92.82 % | 100.00 % | 93.88 % | 100.00 % | 100.00 % | 94.58 % | 100.00 % | 100.00 % | 45.16 % | 75.00 % | - | - | - | - |
| Constructor | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 1.15 % | 36.36 % | 0.00 % | 0.00 % | 11.11 % | - | 0.00 % | 0.00 % | - | - |
