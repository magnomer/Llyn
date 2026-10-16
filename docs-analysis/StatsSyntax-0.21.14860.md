# Syntax statistics - 0.21.14860

- Generated: 2026-10-16 13:24:03 +09:00
- Source roots: src, tests
- Root-level files: none
- Project segments: 1
- Excluded directories: .git, .vs, artifacts, bin, node_modules, obj, packages, publish
- Excluded suffixes: .g.cs, .g.i.cs, .AssemblyInfo.cs, .GlobalUsings.g.cs, .Designer.cs
- Roslyn: 4.14.0-3.25262.10
- Language version: Preview
- Scanned: 2,178 files, 15 projects, 230,467 lines

## Summary

| Measure | Value |
|---|---:|
| Files | 2,178 |
| Lines | 230,467 |
| Nodes | 1,230,233 |
| Syntax kinds used | 198 |
| Features used | 101 of 138 |
| Minimum C# version | 14 |
| Parse errors | 0 |

## Families

| Family | Features | Used | Count | Per 1k lines |
|---|---:|---:|---:|---:|
| Types | 15 | 12 | 4,516 | 19.59 |
| Namespaces | 6 | 3 | 2,180 | 9.46 |
| Members | 29 | 21 | 10,750 | 46.64 |
| Patterns | 16 | 16 | 7,109 | 30.85 |
| Expressions | 36 | 29 | 30,116 | 130.67 |
| Statements | 20 | 14 | 14,307 | 62.08 |
| Generics | 6 | 2 | 77 | 0.33 |
| Async | 2 | 2 | 682 | 2.96 |
| Nullability | 3 | 2 | 4,473 | 19.41 |
| Unsafe | 2 | 0 | 0 | 0.00 |
| Directives | 3 | 0 | 0 | 0.00 |
| Total | 138 | 101 | 74,210 | 322.00 |

## Projects

| Project | Files | Lines | Nodes | Features used | Newest C# |
|---|---:|---:|---:|---:|---:|
| Llyn.Tests.Engine | 250 | 40,122 | 280,526 | 61 | 12 |
| Llyn.UIDeportment | 335 | 36,872 | 170,508 | 88 | 14 |
| Llyn.Tests.Conduct | 164 | 30,682 | 224,564 | 63 | 12 |
| Llyn.Infrastructure | 193 | 25,970 | 119,749 | 70 | 12 |
| Llyn.Application | 136 | 20,934 | 99,168 | 73 | 12 |
| Llyn.Conduct | 305 | 19,868 | 78,593 | 71 | 12 |
| Llyn.Tests.Convention | 115 | 17,713 | 85,619 | 79 | 12 |
| Llyn.Core | 392 | 13,234 | 54,549 | 60 | 12 |
| Llyn.ShellEngine | 103 | 10,731 | 43,416 | 58 | 12 |
| Llyn.Tests.Interface | 101 | 10,065 | 51,151 | 72 | 12 |
| Llyn.Tests.Windows | 26 | 2,801 | 17,304 | 47 | 12 |
| Llyn.Core.Windows | 7 | 798 | 3,140 | 37 | 10 |
| Llyn.UIVeneer | 45 | 493 | 978 | 4 | 10 |
| Llyn.UIDeportment.Capsule | 5 | 96 | 427 | 15 | 10 |
| Llyn.Host | 1 | 88 | 541 | 4 | 9 |
| Total | 2,178 | 230,467 | 1,230,233 | 101 | 14 |

## Versions

| C# version | Features | Used | Count |
|---|---:|---:|---:|
| 1 | 51 | 35 | 19,118 |
| 2 | 10 | 8 | 2,396 |
| 3 | 7 | 5 | 7,434 |
| 4 | 3 | 2 | 859 |
| 5 | 3 | 3 | 1,577 |
| 6 | 6 | 5 | 5,344 |
| 7 | 11 | 10 | 7,109 |
| 7.1 | 1 | 1 | 8 |
| 7.2 | 1 | 0 | 0 |
| 7.3 | 1 | 0 | 0 |
| 8 | 17 | 13 | 12,053 |
| 9 | 10 | 9 | 7,288 |
| 10 | 3 | 2 | 2,180 |
| 11 | 6 | 3 | 673 |
| 12 | 4 | 4 | 8,167 |
| 13 | 1 | 0 | 0 |
| 14 | 3 | 1 | 4 |

## Features

| Feature | Family | C# | Count | Per 1k lines | Files | First |
|---|---|---|---:|---:|---:|---|
| Collection expression | Expressions | 12 | 7,919 | 34.36 | 980 | src/Llyn.Application/Card/LCardClerk.cs:60 |
| using declaration | Statements | 8 | 6,136 | 26.62 | 459 | src/Llyn.Application/Card/LTranslationClerk.cs:160 |
| Lambda | Expressions | 3 | 5,177 | 22.46 | 805 | src/Llyn.Application/Card/LCardClerk.cs:95 |
| if | Statements | 1 | 4,522 | 19.62 | 759 | src/Llyn.Application/Card/LCardClerk.cs:73 |
| Nullable reference type T? | Nullability | 8 | 3,730 | 16.18 | 1,040 | src/Llyn.Application/Card/LCardClerkField.cs:91 |
| Expression-bodied member | Members | 6 | 2,954 | 12.82 | 548 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Target-typed new | Expressions | 9 | 2,547 | 11.05 | 600 | src/Llyn.Application/Card/LMentionClerk.cs:153 |
| File-scoped namespace | Namespaces | 10 | 2,177 | 9.45 | 2,177 | src/Llyn.Application/Card/LCardClerk.cs:6 |
| Readonly field | Members | 1 | 2,169 | 9.41 | 662 | src/Llyn.Application/Card/LCardClerk.cs:10 |
| Declaration pattern | Patterns | 7 | 2,161 | 9.38 | 428 | src/Llyn.Application/Card/LMeaningClerk.cs:105 |
| Tuple literal or tuple type | Expressions | 7 | 1,816 | 7.88 | 312 | src/Llyn.Application/Card/LMeaningClerk.cs:26 |
| Sealed class | Types | 1 | 1,735 | 7.53 | 1,620 | src/Llyn.Application/Card/LCardClerk.cs:8 |
| Conditional ?: | Expressions | 1 | 1,568 | 6.80 | 583 | src/Llyn.Application/Card/LCardClerk.cs:170 |
| foreach | Statements | 1 | 1,551 | 6.73 | 512 | src/Llyn.Application/Card/LCardClerk.cs:62 |
| Class | Types | 1 | 1,538 | 6.67 | 1,520 | src/Llyn.Application/Card/LCardClerk.cs:8 |
| Static lambda | Expressions | 9 | 1,525 | 6.62 | 363 | src/Llyn.Application/Card/LCardClerk.cs:95 |
| Full property | Members | 1 | 1,290 | 5.60 | 380 | src/Llyn.Application/Citation/LExampleClerk.cs:25 |
| Constant pattern | Patterns | 7 | 1,284 | 5.57 | 223 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Null-conditional ?. ?[ | Expressions | 6 | 1,188 | 5.15 | 422 | src/Llyn.Application/Card/LMentionClerk.cs:47 |
| Null-coalescing ?? | Expressions | 2 | 1,083 | 4.70 | 425 | src/Llyn.Application/Card/LCardClerkField.cs:74 |
| not pattern | Patterns | 9 | 1,021 | 4.43 | 417 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| Auto property | Members | 3 | 971 | 4.21 | 286 | src/Llyn.Application/Citation/LCitationClerk.cs:41 |
| Null-forgiving ! | Expressions | 8 | 953 | 4.14 | 350 | src/Llyn.Application/Draft/Clerk/Card/LDraftClerkCard.cs:124 |
| is null | Patterns | 7 | 916 | 3.97 | 401 | src/Llyn.Application/Card/LCardClerkField.cs:92 |
| with expression | Expressions | 9 | 906 | 3.93 | 289 | src/Llyn.Application/Card/LCardClerk.cs:115 |
| await | Expressions | 5 | 895 | 3.88 | 184 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:139 |
| Extension method | Members | 3 | 756 | 3.28 | 45 | tests/Llyn.Tests.Interface/Atelier/TInterfaceArea.cs:7 |
| Nullable value type T? | Nullability | 2 | 743 | 3.22 | 322 | src/Llyn.Application/Card/LMeaningClerk.cs:104 |
| Interpolated string | Expressions | 6 | 723 | 3.14 | 181 | src/Llyn.Application/Localization/LLocalizationReader.cs:55 |
| Raw string | Expressions | 11 | 653 | 2.83 | 152 | src/Llyn.Infrastructure/Database/Base/LDatabase.cs:51 |
| Constructor | Members | 1 | 642 | 2.79 | 638 | src/Llyn.Application/Card/LCardClerk.cs:21 |
| Async method | Async | 5 | 627 | 2.72 | 184 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:91 |
| Const field | Members | 1 | 615 | 2.67 | 261 | src/Llyn.Application/Card/LMentionClerk.cs:11 |
| Record class | Types | 9 | 600 | 2.60 | 502 | src/Llyn.Application/Draft/Row/LDraftClerkAncestor.cs:263 |
| Cast expression | Expressions | 1 | 595 | 2.58 | 186 | src/Llyn.Application/Markup/LMarkupClerkLink.cs:228 |
| Property pattern | Patterns | 8 | 533 | 2.31 | 144 | src/Llyn.Application/Draft/Clerk/LDraftClerk.cs:83 |
| Object initializer | Expressions | 3 | 512 | 2.22 | 214 | src/Llyn.Application/Portrait/LPortraitClerkLabel.cs:25 |
| Optional parameter | Members | 4 | 493 | 2.14 | 164 | src/Llyn.Application/Draft/Clerk/LDraftClerkPanel.cs:170 |
| nameof | Expressions | 6 | 422 | 1.83 | 154 | src/Llyn.Application/Citation/LAuthorClerk.cs:361 |
| catch | Statements | 1 | 402 | 1.74 | 172 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:142 |
| lock | Statements | 1 | 394 | 1.71 | 63 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:98 |
| Deconstruction | Expressions | 7 | 372 | 1.61 | 162 | src/Llyn.Application/Card/LMeaningClerk.cs:46 |
| try | Statements | 1 | 372 | 1.61 | 191 | src/Llyn.Application/Draft/Court/LChronicleClerk.cs:137 |
| Named argument | Members | 4 | 366 | 1.59 | 105 | src/Llyn.Application/Card/LTranslationClerk.cs:164 |
| and / or pattern | Patterns | 9 | 358 | 1.55 | 108 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| Static class | Types | 2 | 353 | 1.53 | 353 | src/Llyn.Application/Card/LCardClerkField.cs:7 |
| for | Statements | 1 | 297 | 1.29 | 189 | src/Llyn.Application/Card/LCardClerk.cs:280 |
| Spread element | Expressions | 12 | 241 | 1.05 | 135 | src/Llyn.Application/Card/LCardClerk.cs:95 |
| throw expression | Expressions | 7 | 235 | 1.02 | 115 | src/Llyn.Application/Card/LCardClerkField.cs:75 |
| Init accessor | Members | 9 | 230 | 1.00 | 81 | src/Llyn.Conduct/Lexicon/CExample.cs:20 |
| Discard pattern | Patterns | 8 | 224 | 0.97 | 132 | src/Llyn.Application/Citation/LAuthorClerk.cs:243 |
| Switch expression | Patterns | 8 | 221 | 0.96 | 133 | src/Llyn.Application/Citation/LAuthorClerk.cs:239 |
| while | Statements | 1 | 218 | 0.95 | 133 | src/Llyn.Application/Card/LMeaningClerk.cs:44 |
| typeof | Expressions | 1 | 197 | 0.85 | 43 | src/Llyn.Infrastructure/Configuration/LLocalizationLoader.cs:20 |
| using statement | Statements | 1 | 188 | 0.82 | 79 | src/Llyn.Application/Citation/LAuthorCitation.cs:51 |
| Field-like event | Members | 1 | 176 | 0.76 | 109 | src/Llyn.Conduct/Atelier/CWorkspace.cs:40 |
| Discard _ | Expressions | 7 | 162 | 0.70 | 66 | src/Llyn.Application/Draft/Clerk/Card/LDraftClerkCard.cs:127 |
| Case guard when | Patterns | 7 | 116 | 0.50 | 25 | src/Llyn.Application/Citation/LAuthorClerk.cs:241 |
| Interface | Types | 1 | 111 | 0.48 | 111 | src/Llyn.Application/Pronunciation/Relay/LListener.cs:5 |
| Index from end ^ | Expressions | 8 | 105 | 0.46 | 53 | src/Llyn.Application/Draft/Clerk/LDraftClerkList.cs:230 |
| Verbatim string | Expressions | 1 | 95 | 0.41 | 21 | src/Llyn.Application/Localization/LLocalizationReader.cs:13 |
| Range .. | Expressions | 8 | 92 | 0.40 | 49 | src/Llyn.Application/Card/LMentionClerk.cs:159 |
| Relational pattern | Patterns | 9 | 90 | 0.39 | 31 | src/Llyn.Application/Citation/LAuthorCitation.cs:27 |
| as cast | Expressions | 1 | 86 | 0.37 | 43 | src/Llyn.Infrastructure/Database/Base/Workspace/LWorkspaceArchive.cs:132 |
| Partial type | Types | 2 | 84 | 0.36 | 84 | src/Llyn.UIVeneer/Display/View/PDisplay.xaml.cs:5 |
| is type test | Patterns | 1 | 60 | 0.26 | 37 | src/Llyn.Application/Outpost/LCourierClerk.cs:288 |
| Switch statement | Patterns | 1 | 60 | 0.26 | 37 | src/Llyn.Application/Portrait/LPortraitClerkPress.cs:59 |
| Generic method | Generics | 2 | 59 | 0.26 | 31 | src/Llyn.Application/Card/LCardClerkField.cs:183 |
| Exception filter | Statements | 6 | 57 | 0.25 | 39 | src/Llyn.Application/Outpost/LCourierClerk.cs:196 |
| Async lambda | Async | 5 | 55 | 0.24 | 10 | src/Llyn.Application/Outpost/LCourierClerk.cs:251 |
| finally | Statements | 1 | 53 | 0.23 | 38 | src/Llyn.Application/Outpost/LCourierClerk.cs:76 |
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
| default literal | Expressions | 7.1 | 8 | 0.03 | 6 | src/Llyn.Infrastructure/Pronunciation/Loader/LSourceLoader.cs:73 |
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
| Null check | is null / is not null | 916 | == null / != null | 0 | 100.00 % |
| Object creation | Target-typed new() | 2,547 | new T() | 4,473 | 36.28 % |
| Collection creation | Collection expression | 7,919 | Collection initializer or array creation | 74 | 99.07 % |
| Member body | Expression body | 2,954 | Block body | 11,429 | 20.54 % |
| Local type | var | 52 | Explicit type | 24,288 | 0.21 % |
| Using | Declaration | 6,136 | Statement | 188 | 97.03 % |
| Namespace | File-scoped | 2,177 | Block | 0 | 100.00 % |
| Text building | Interpolated string | 723 | string.Format or + with a string literal | 606 | 54.40 % |
| Branch | Switch expression | 221 | Switch statement | 60 | 78.65 % |
| Lambda body | Expression | 4,568 | Block | 609 | 88.24 % |
| Type test | is pattern with designation | 1,273 | as | 86 | 93.67 % |
| Constructor | Primary | 6 | Explicit | 638 | 0.93 % |

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
| IdentifierName | 355,603 | 2,178 |
| Argument | 127,929 | 1,629 |
| SimpleMemberAccessExpression | 117,350 | 1,614 |
| ArgumentList | 79,566 | 1,674 |
| InvocationExpression | 72,713 | 1,632 |
| PredefinedType | 34,866 | 2,017 |
| ExpressionStatement | 34,705 | 1,517 |
| StringLiteralExpression | 28,393 | 1,202 |
| Parameter | 25,398 | 1,952 |
| VariableDeclaration | 25,149 | 1,422 |
| VariableDeclarator | 25,149 | 1,422 |
| EqualsValueClause | 23,915 | 1,479 |
| LocalDeclarationStatement | 21,284 | 1,237 |
| Block | 20,160 | 1,600 |
| ParameterList | 15,370 | 2,091 |
| MethodDeclaration | 12,967 | 1,659 |
| NumericLiteralExpression | 12,852 | 1,197 |
| GenericName | 11,933 | 1,495 |
| TypeArgumentList | 11,933 | 1,495 |
| QualifiedName | 10,289 | 2,178 |
| ExpressionElement | 8,642 | 542 |
| SimpleAssignmentExpression | 8,445 | 1,122 |
| CollectionExpression | 7,919 | 980 |
| UsingDirective | 6,982 | 1,855 |
| ReturnStatement | 6,457 | 1,180 |
| NullLiteralExpression | 4,830 | 990 |
| IfStatement | 4,522 | 759 |
| NullableType | 4,473 | 1,124 |
| ObjectCreationExpression | 4,473 | 980 |
| SimpleLambdaExpression | 4,056 | 728 |
| Attribute | 3,986 | 471 |
| AttributeList | 3,986 | 471 |
| SingleVariableDesignation | 3,655 | 624 |
| BracketedArgumentList | 3,470 | 606 |
| FieldDeclaration | 3,213 | 836 |
| IsPatternExpression | 3,099 | 646 |
| ArrowExpressionClause | 2,956 | 549 |
| ElementAccessExpression | 2,841 | 557 |
| ImplicitObjectCreationExpression | 2,547 | 600 |
| FalseLiteralExpression | 2,299 | 602 |
| PropertyDeclaration | 2,267 | 556 |
| ConstantPattern | 2,200 | 504 |
| CompilationUnit | 2,178 | 2,178 |
| FileScopedNamespaceDeclaration | 2,177 | 2,177 |
| DeclarationPattern | 2,161 | 428 |
| TrueLiteralExpression | 1,907 | 562 |
| EqualsExpression | 1,822 | 652 |
| TupleExpression | 1,757 | 306 |
| AddExpression | 1,744 | 397 |
| InterpolatedStringText | 1,631 | 181 |
| ConditionalExpression | 1,568 | 583 |
| ClassDeclaration | 1,538 | 1,520 |
| LogicalAndExpression | 1,517 | 450 |
| ForEachStatement | 1,460 | 494 |
| Interpolation | 1,429 | 180 |
| AddAssignmentExpression | 1,312 | 303 |
| LogicalNotExpression | 1,302 | 447 |
| AttributeArgument | 1,252 | 63 |
| SwitchExpressionArm | 1,201 | 133 |
| ConditionalAccessExpression | 1,188 | 422 |
| MemberBindingExpression | 1,188 | 422 |
| DeclarationExpression | 1,171 | 306 |
| ParenthesizedLambdaExpression | 1,121 | 318 |
| CoalesceExpression | 1,083 | 425 |
| AccessorList | 1,081 | 313 |
| GetAccessorDeclaration | 1,079 | 312 |
| TupleElement | 1,036 | 231 |
| NotPattern | 1,021 | 417 |
| LogicalOrExpression | 1,016 | 329 |
| SuppressNullableWarningExpression | 953 | 350 |
| WithExpression | 906 | 289 |
| WithInitializerExpression | 906 | 289 |
| AwaitExpression | 895 | 184 |
| GreaterThanExpression | 792 | 341 |
| NameColon | 791 | 229 |
| InterpolatedStringExpression | 723 | 181 |
| ArrayRankSpecifier | 698 | 284 |
| ArrayType | 694 | 284 |
| OmittedArraySizeExpression | 675 | 284 |
| ConstructorDeclaration | 644 | 640 |
| ImplicitElementAccess | 629 | 121 |
| RecordDeclaration | 600 | 502 |
| CastExpression | 595 | 186 |
| PostIncrementExpression | 582 | 272 |
| LessThanExpression | 571 | 258 |
| CharacterLiteralExpression | 552 | 132 |
| RecursivePattern | 542 | 151 |
| PropertyPatternClause | 533 | 144 |
| AttributeArgumentList | 513 | 63 |
| ObjectInitializerExpression | 512 | 214 |
| NotEqualsExpression | 499 | 219 |
| Subpattern | 478 | 132 |
| ParenthesizedExpression | 439 | 215 |
| TupleType | 433 | 231 |
| BaseList | 414 | 313 |
| CatchClause | 402 | 172 |
| CatchDeclaration | 399 | 170 |
| LockStatement | 394 | 63 |
| TryStatement | 372 | 191 |
| ContinueStatement | 340 | 167 |
| SwitchSection | 334 | 37 |
| BreakStatement | 321 | 68 |
| OrPattern | 315 | 102 |
| SimpleBaseType | 315 | 294 |
| ForStatement | 297 | 189 |
| EnumMemberDeclaration | 294 | 48 |
| SubtractExpression | 248 | 115 |
| UnaryMinusExpression | 245 | 99 |
| SpreadElement | 241 | 135 |
| ThrowExpression | 235 | 115 |
| InitAccessorDeclaration | 230 | 81 |
| ThrowStatement | 230 | 124 |
| DiscardPattern | 224 | 132 |
| SwitchExpression | 221 | 133 |
| WhileStatement | 218 | 133 |
| SubtractAssignmentExpression | 213 | 80 |
| TypeOfExpression | 197 | 43 |
| UsingStatement | 188 | 79 |
| CaseSwitchLabel | 183 | 24 |
| ElseClause | 176 | 132 |
| EventFieldDeclaration | 176 | 109 |
| SetAccessorDeclaration | 165 | 82 |
| GreaterThanOrEqualExpression | 141 | 88 |
| ThisExpression | 132 | 88 |
| LessThanOrEqualExpression | 125 | 91 |
| WhenClause | 116 | 25 |
| CasePatternSwitchLabel | 114 | 23 |
| InterfaceDeclaration | 111 | 111 |
| PrimaryConstructorBaseType | 110 | 19 |
| MultiplyExpression | 107 | 46 |
| IndexExpression | 105 | 53 |
| RangeExpression | 92 | 49 |
| ForEachVariableStatement | 91 | 67 |
| RelationalPattern | 90 | 31 |
| AsExpression | 86 | 43 |
| ArrayInitializerExpression | 76 | 42 |
| TypeParameter | 68 | 35 |
| TypeParameterList | 63 | 35 |
| IsExpression | 60 | 37 |
| SwitchStatement | 60 | 37 |
| DivideExpression | 59 | 30 |
| CatchFilterClause | 57 | 39 |
| ImplicitArrayCreationExpression | 54 | 39 |
| FinallyClause | 53 | 38 |
| BitwiseOrExpression | 50 | 26 |
| EnumDeclaration | 48 | 48 |
| ParenthesizedPattern | 45 | 23 |
| YieldReturnStatement | 45 | 21 |
| AndPattern | 43 | 19 |
| DefaultSwitchLabel | 43 | 25 |
| ExpressionColon | 37 | 17 |
| ArrayCreationExpression | 35 | 24 |
| OrAssignmentExpression | 35 | 20 |
| LeftShiftExpression | 34 | 3 |
| VarPattern | 30 | 12 |
| CoalesceAssignmentExpression | 21 | 19 |
| AttributeTargetSpecifier | 19 | 8 |
| CollectionInitializerExpression | 18 | 16 |
| PostDecrementExpression | 18 | 13 |
| TypeParameterConstraintClause | 18 | 10 |
| ClassConstraint | 17 | 9 |
| LocalFunctionStatement | 17 | 11 |
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
| ModuloExpression | 4 | 3 |
| AndAssignmentExpression | 3 | 3 |
| ExclusiveOrExpression | 3 | 2 |
| InterpolationAlignmentClause | 3 | 1 |
| RecordStructDeclaration | 3 | 3 |
| SlicePattern | 3 | 3 |
| ComplexElementInitializerExpression | 2 | 1 |
| DiscardDesignation | 2 | 2 |
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
| Fact | 3,368 | 456 |
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
| Collection expression | 3,357 | 187 | 2,086 | 321 | 407 | 237 | 513 | 309 | 70 | 261 | 171 |  |  |  |  |
| using declaration | 2,389 | 5 | 2,967 | 654 | 17 |  | 7 |  |  | 22 | 66 | 9 |  |  |  |
| Lambda | 976 | 243 | 1,581 | 44 | 480 | 484 | 740 | 196 | 81 | 251 | 85 | 7 |  |  | 9 |
| if | 54 | 1,442 | 6 | 728 | 770 | 459 | 471 | 293 | 231 | 48 | 6 | 11 |  | 3 |  |
| Nullable reference type T? | 190 | 519 | 389 | 393 | 393 | 378 | 300 | 367 | 274 | 469 | 44 | 9 |  | 5 |  |
| Expression-bodied member | 54 | 777 | 37 | 22 | 120 | 208 | 27 | 169 | 169 | 1,333 | 37 | 1 |  |  |  |
| Target-typed new | 208 | 764 | 409 | 153 | 193 | 51 | 189 | 65 | 96 | 307 | 96 | 11 |  | 1 | 4 |
| File-scoped namespace | 250 | 335 | 164 | 193 | 136 | 305 | 115 | 392 | 103 | 101 | 26 | 7 | 45 | 5 |  |
| Readonly field | 45 | 645 | 4 | 128 | 453 | 396 | 215 | 31 | 190 | 46 | 6 | 9 |  | 1 |  |
| Declaration pattern | 7 | 1,101 | 4 | 64 | 227 | 263 | 369 | 33 | 77 | 11 | 3 | 2 |  |  |  |
| Tuple literal or tuple type | 469 | 51 | 303 | 388 | 118 | 91 | 164 | 60 | 99 | 45 | 28 |  |  |  |  |
| Sealed class | 256 | 294 | 164 | 83 | 204 | 283 | 43 | 256 | 74 | 43 | 23 | 7 |  | 5 |  |
| Conditional ?: | 16 | 232 | 19 | 354 | 313 | 190 | 156 | 172 | 76 | 35 | 3 | 2 |  |  |  |
| foreach | 56 | 222 | 20 | 309 | 355 | 31 | 305 | 169 | 32 | 48 | 2 | 2 |  |  |  |
| Class | 259 | 319 | 163 | 191 | 114 | 102 | 108 | 30 | 64 | 109 | 26 | 7 | 45 | 1 |  |
| Static lambda | 146 | 110 | 738 | 11 | 181 | 105 | 15 | 103 | 16 | 91 | 8 |  |  |  | 1 |
| Full property | 7 | 732 | 5 | 6 | 118 | 194 | 2 | 147 | 48 | 30 |  | 1 |  |  |  |
| Constant pattern | 6 | 159 | 8 | 144 | 101 | 208 | 358 | 215 | 49 | 20 |  | 16 |  |  |  |
| Null-conditional ?. ?[ | 132 | 207 | 100 | 63 | 61 | 387 | 91 | 43 | 88 | 9 | 7 |  |  |  |  |
| Null-coalescing ?? | 22 | 83 | 24 | 141 | 235 | 84 | 82 | 274 | 55 | 66 | 11 | 5 |  | 1 |  |
| not pattern | 6 | 376 | 1 | 64 | 95 | 182 | 185 | 40 | 58 | 11 | 1 | 2 |  |  |  |
| Auto property | 5 | 420 | 3 | 8 | 7 | 150 | 23 | 246 | 62 | 47 |  |  |  |  |  |
| Null-forgiving ! | 259 | 165 | 300 | 67 | 16 | 10 | 28 | 5 | 4 | 80 | 19 |  |  |  |  |
| is null | 10 | 161 | 6 | 109 | 196 | 106 | 174 | 67 | 65 | 19 | 2 | 1 |  |  |  |
| with expression | 329 | 8 | 41 | 54 | 298 | 40 | 10 | 30 | 49 | 47 |  |  |  |  |  |
| await | 286 | 63 | 326 | 63 | 69 | 23 | 1 |  | 24 | 14 | 1 | 25 |  |  |  |
| Extension method |  |  |  |  |  |  |  |  |  | 751 | 5 |  |  |  |  |
| Nullable value type T? | 35 | 74 | 31 | 47 | 58 | 163 | 11 | 66 | 141 | 96 | 19 |  |  | 2 |  |
| Interpolated string | 58 | 19 | 8 | 172 | 60 | 1 | 384 | 7 |  | 8 | 6 |  |  |  |  |
| Raw string | 157 | 2 | 7 | 239 |  |  | 235 |  |  | 8 | 5 |  |  |  |  |
| Constructor | 2 | 254 | 1 | 74 | 89 | 90 | 7 | 3 | 63 | 10 |  | 4 | 45 |  |  |
| Async method | 222 | 62 | 204 | 33 | 40 | 21 | 1 |  | 20 | 10 | 1 | 13 |  |  |  |
| Const field | 88 | 75 | 15 | 195 | 17 | 5 | 112 | 82 | 4 | 12 | 6 | 3 |  | 1 |  |
| Record class |  | 18 | 2 | 2 | 114 | 189 | 6 | 253 | 10 | 2 |  |  |  | 4 |  |
| Cast expression | 15 | 132 | 110 | 119 | 4 | 24 | 50 | 5 |  | 53 | 77 | 6 |  |  |  |
| Property pattern | 2 | 186 |  | 10 | 18 | 25 | 274 | 5 | 6 | 5 | 2 |  |  |  |  |
| Object initializer | 67 | 136 | 104 | 32 | 4 | 1 | 59 | 3 | 1 | 38 | 64 | 2 |  | 1 |  |
| Optional parameter | 15 | 8 | 6 | 6 | 12 | 12 | 1 | 295 | 12 | 122 |  |  |  | 4 |  |
| nameof | 1 | 205 | 3 | 13 | 130 | 33 | 17 | 8 | 5 | 2 | 1 | 4 |  |  |  |
| catch | 2 | 21 |  | 139 | 32 | 160 | 7 | 5 | 11 | 1 | 8 | 13 |  | 3 |  |
| lock |  | 14 |  | 2 | 85 | 2 | 5 |  | 281 | 5 |  |  |  |  |  |
| Deconstruction | 77 | 38 | 121 | 35 | 30 | 26 | 30 | 9 | 5 |  | 1 |  |  |  |  |
| try | 9 | 22 | 1 | 86 | 32 | 163 | 9 | 5 | 14 | 3 | 12 | 15 |  | 1 |  |
| Named argument | 174 |  | 39 | 16 | 31 |  | 16 | 23 | 3 | 64 |  |  |  |  |  |
| and / or pattern | 3 | 47 | 3 | 68 | 18 | 3 | 161 | 44 | 2 | 8 |  | 1 |  |  |  |
| Static class | 3 | 42 | 1 | 110 | 23 | 8 | 71 | 27 |  | 65 | 3 |  |  |  |  |
| for | 18 | 45 | 6 | 70 | 62 | 10 | 29 | 27 | 7 | 21 | 2 |  |  |  |  |
| Spread element | 39 | 16 | 10 | 7 | 58 | 14 | 29 | 42 | 8 | 16 | 2 |  |  |  |  |
| throw expression | 5 | 7 | 30 | 9 | 96 | 33 | 1 | 7 | 11 | 27 | 1 | 8 |  |  |  |
| Init accessor |  | 2 |  |  |  | 2 | 3 | 223 |  |  |  |  |  |  |  |
| Discard pattern |  | 14 | 1 | 19 | 32 | 37 | 67 | 38 | 10 | 1 |  | 5 |  |  |  |
| Switch expression |  | 12 | 1 | 19 | 32 | 38 | 65 | 38 | 10 | 1 |  | 5 |  |  |  |
| while | 22 | 9 | 6 | 108 | 18 | 3 | 25 | 21 | 1 | 5 |  |  |  |  |  |
| typeof |  | 143 | 5 | 4 |  |  | 3 |  |  | 4 | 38 |  |  |  |  |
| using statement | 37 | 1 | 2 | 135 | 7 | 2 |  |  |  | 1 | 1 | 2 |  |  |  |
| Field-like event |  | 70 |  |  |  | 103 |  |  | 3 |  |  |  |  |  |  |
| Discard _ | 34 | 28 | 51 | 5 | 8 | 6 | 10 | 10 | 7 | 1 | 2 |  |  |  |  |
| Case guard when |  | 6 |  | 1 | 1 | 1 | 100 | 5 |  | 2 |  |  |  |  |  |
| Interface |  | 2 |  |  | 1 | 1 |  | 78 | 29 |  |  |  |  |  |  |
| Index from end ^ | 24 | 5 | 52 | 1 | 4 | 2 | 9 | 8 |  |  |  |  |  |  |  |
| Verbatim string | 11 |  |  |  | 3 |  | 79 |  |  | 2 |  |  |  |  |  |
| Range .. | 11 |  | 12 | 16 | 2 |  | 29 | 21 |  | 1 |  |  |  |  |  |
| Relational pattern |  | 7 |  | 28 | 8 | 1 | 5 | 40 | 1 |  |  |  |  |  |  |
| as cast |  | 56 |  | 3 |  |  | 9 |  |  | 17 | 1 |  |  |  |  |
| Partial type |  |  | 4 |  |  |  | 7 |  |  | 28 |  |  | 45 |  |  |
| is type test | 1 | 22 |  | 2 | 3 |  | 32 |  |  |  |  |  |  |  |  |
| Switch statement |  | 8 |  | 8 | 1 | 2 | 20 | 18 | 1 | 2 |  |  |  |  |  |
| Generic method |  | 25 | 1 |  | 14 | 5 | 1 | 7 | 1 | 5 |  |  |  |  |  |
| Exception filter |  | 3 |  | 33 | 16 |  | 2 |  | 3 |  |  |  |  |  |  |
| Async lambda |  |  | 53 |  | 1 |  |  |  |  | 1 |  |  |  |  |  |
| finally | 7 | 4 | 1 | 2 | 14 | 6 | 2 |  | 5 | 2 | 4 | 6 |  |  |  |
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
| Object creation | 51.61 % | 47.63 % | 42.34 % | 24.13 % | 30.98 % | 9.44 % | 42.00 % | 25.39 % | 30.77 % | 33.23 % | 39.34 % | 30.56 % | - | 14.29 % | 19.05 % |
| Collection creation | 99.50 % | 97.40 % | 99.29 % | 98.17 % | 98.79 % | 99.16 % | 99.03 % | 99.04 % | 100.00 % | 99.62 % | 92.43 % | 0.00 % | - | - | - |
| Member body | 2.65 % | 26.14 % | 2.34 % | 1.97 % | 10.37 % | 15.32 % | 3.80 % | 28.69 % | 18.53 % | 78.27 % | 23.27 % | 2.63 % | 0.00 % | 0.00 % | - |
| Local type | 0.09 % | 3.78 % | 0.00 % | 0.00 % | 0.16 % | 0.21 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | - | 0.00 % | 0.00 % |
| Using | 98.47 % | 83.33 % | 99.93 % | 82.89 % | 70.83 % | 0.00 % | 100.00 % | - | - | 95.65 % | 98.51 % | 81.82 % | - | - | - |
| Namespace | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | 100.00 % | - |
| Text building | 27.75 % | 46.34 % | 14.55 % | 47.65 % | 69.77 % | 2.38 % | 81.70 % | 21.21 % | - | 38.10 % | 85.71 % | 0.00 % | - | 0.00 % | - |
| Branch | - | 60.00 % | 100.00 % | 70.37 % | 96.97 % | 95.00 % | 76.47 % | 67.86 % | 90.91 % | 33.33 % | - | 100.00 % | - | - | - |
| Lambda body | 96.41 % | 88.07 % | 76.91 % | 90.91 % | 93.96 % | 96.07 % | 98.92 % | 97.96 % | 92.59 % | 80.08 % | 32.94 % | 85.71 % | - | - | 77.78 % |
| Type test | 100.00 % | 93.02 % | 100.00 % | 93.88 % | 100.00 % | 100.00 % | 94.58 % | 100.00 % | 100.00 % | 45.16 % | 75.00 % | - | - | - | - |
| Constructor | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 0.00 % | 1.11 % | 36.36 % | 0.00 % | 0.00 % | 11.11 % | - | 0.00 % | 0.00 % | - | - |
