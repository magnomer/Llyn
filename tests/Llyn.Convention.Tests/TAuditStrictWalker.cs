using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Convention.Tests;

internal static class TAuditStrictWalker
{
    private static readonly CSharpParseOptions TAuditSyntaxOptions = new(
        languageVersion: LanguageVersion.Preview,
        documentationMode: DocumentationMode.None,
        kind: SourceCodeKind.Regular);

    private static readonly SyntaxKind[] TAuditFlowKinds =
    [
        SyntaxKind.IfStatement,
        SyntaxKind.SwitchStatement,
        SyntaxKind.SwitchExpression,
        SyntaxKind.ConditionalExpression,
        SyntaxKind.ForStatement,
        SyntaxKind.ForEachStatement,
        SyntaxKind.ForEachVariableStatement,
        SyntaxKind.WhileStatement,
        SyntaxKind.DoStatement,
        SyntaxKind.LogicalAndExpression,
        SyntaxKind.LogicalOrExpression,
        SyntaxKind.CoalesceExpression,
        SyntaxKind.AddExpression,
        SyntaxKind.SubtractExpression,
        SyntaxKind.MultiplyExpression,
        SyntaxKind.DivideExpression,
        SyntaxKind.ModuloExpression,
        SyntaxKind.LessThanExpression,
        SyntaxKind.LessThanOrEqualExpression,
        SyntaxKind.GreaterThanExpression,
        SyntaxKind.GreaterThanOrEqualExpression,
        SyntaxKind.EqualsExpression,
        SyntaxKind.NotEqualsExpression,
        SyntaxKind.IsPatternExpression,
        SyntaxKind.LocalFunctionStatement,
    ];

    public static IReadOnlyList<TViolation> TAuditRun(IEnumerable<string> sourcePaths, out List<string> veneers)
    {
        List<TViolation> violations = [];
        Dictionary<string, List<ClassDeclarationSyntax>> parts = new(StringComparer.Ordinal);
        foreach (string path in sourcePaths)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText(path), TAuditSyntaxOptions, path);
            SyntaxNode root = tree.GetRoot();
            TAuditTreatScan(root, violations);
            foreach (ClassDeclarationSyntax type in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                string key = type.Identifier.ValueText;
                if (!parts.TryGetValue(key, out List<ClassDeclarationSyntax>? list))
                {
                    list = [];
                    parts[key] = list;
                }

                list.Add(type);
            }
        }

        veneers = [];
        foreach ((string name, List<ClassDeclarationSyntax> type) in parts)
        {
            bool veneer = type.Any(TAuditVeneerCheck);
            if (veneer)
            {
                veneers.Add(name);
            }

            foreach (ClassDeclarationSyntax part in type)
            {
                TAuditStorageScan(part, veneer, violations);
                if (veneer)
                {
                    TAuditFlowScan(part, violations);
                }
            }
        }

        return violations;
    }

    private static bool TAuditVeneerCheck(ClassDeclarationSyntax type)
    {
        string path = type.SyntaxTree.FilePath;
        if (path.EndsWith(".xaml.cs", StringComparison.OrdinalIgnoreCase) && File.Exists(path[..^3]))
        {
            return true;
        }

        return type.BaseList?.Types.Any(baseType =>
            TAuditTypeRead(baseType.Type) is string name
            && TAuditStrictSetting.TAuditVeneerBases.Contains(name, StringComparer.Ordinal)) == true;
    }

    private static void TAuditStorageScan(ClassDeclarationSyntax part, bool veneer, List<TViolation> violations)
    {
        foreach (FieldDeclarationSyntax field in part.Members.OfType<FieldDeclarationSyntax>())
        {
            bool fixture = field.Modifiers.Any(modifier =>
                modifier.IsKind(SyntaxKind.ReadOnlyKeyword) || modifier.IsKind(SyntaxKind.ConstKeyword));
            bool shared = field.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.StaticKeyword));
            if (fixture || (!veneer && !shared))
            {
                continue;
            }

            foreach (VariableDeclaratorSyntax variable in field.Declaration.Variables)
            {
                if (TAuditWiredCheck(variable))
                {
                    continue;
                }

                violations.Add(new TViolation(
                    field.SyntaxTree.FilePath,
                    TAuditLineRead(variable),
                    $"{part.Identifier.ValueText}.{variable.Identifier.ValueText}",
                    "Storage",
                    shared ? "mutable static field" : "mutable field in a veneer class"));
            }
        }
    }

    private static void TAuditFlowScan(ClassDeclarationSyntax part, List<TViolation> violations)
    {
        foreach (MemberDeclarationSyntax member in part.Members)
        {
            if (member is BaseTypeDeclarationSyntax or FieldDeclarationSyntax)
            {
                continue;
            }

            List<string> found = [];
            foreach (SyntaxNode node in member.DescendantNodes())
            {
                SyntaxKind kind = node.Kind();
                if (!TAuditFlowKinds.Contains(kind))
                {
                    continue;
                }

                string label = kind.ToString();
                if (!found.Contains(label, StringComparer.Ordinal))
                {
                    found.Add(label);
                }
            }

            if (found.Count == 0)
            {
                continue;
            }

            violations.Add(new TViolation(
                member.SyntaxTree.FilePath,
                TAuditLineRead(member),
                $"{part.Identifier.ValueText}.{TAuditMemberRead(member)}",
                "Flow",
                string.Join(' ', found)));
        }
    }

    private static void TAuditTreatScan(SyntaxNode root, List<TViolation> violations)
    {
        HashSet<int> seen = [];
        foreach (SyntaxNode node in root.DescendantNodes())
        {
            string? reason = node switch
            {
                BinaryExpressionSyntax binary
                    when !binary.IsKind(SyntaxKind.CoalesceExpression)
                         && !binary.IsKind(SyntaxKind.LogicalAndExpression)
                         && !binary.IsKind(SyntaxKind.LogicalOrExpression)
                         && !TAuditNullCheck(binary.Left) && !TAuditNullCheck(binary.Right)
                         && (TAuditDataCheck(binary.Left) || TAuditDataCheck(binary.Right))
                    => $"logic value in {binary.OperatorToken.ValueText}",
                InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax access } query
                    when TAuditStrictSetting.TAuditTreatVerbs.Contains(
                             access.Name.Identifier.ValueText, StringComparer.Ordinal)
                         && (TAuditDataCheck(access.Expression)
                             || query.ArgumentList.Arguments.Any(argument => TAuditDataCheck(argument)))
                    => $"logic value queried by {access.Name.Identifier.ValueText}",
                IfStatementSyntax branch when TAuditConditionCheck(branch.Condition)
                    => "logic value decides an if",
                ConditionalExpressionSyntax choice when TAuditConditionCheck(choice.Condition)
                    => "logic value decides a ternary",
                SwitchStatementSyntax select when TAuditDataCheck(select.Expression)
                    => "logic value decides a switch",
                SwitchExpressionSyntax arms when TAuditDataCheck(arms.GoverningExpression)
                    => "logic value decides a switch expression",
                _ => null
            };

            if (reason is null)
            {
                continue;
            }

            int line = TAuditLineRead(node);
            if (seen.Add(line))
            {
                string excerpt = node.ToString().Split('\n')[0].Trim();
                violations.Add(new TViolation(root.SyntaxTree.FilePath, line, excerpt, "Treat", reason));
            }
        }
    }

    private static bool TAuditConditionCheck(ExpressionSyntax condition)
    {
        if (condition is IsPatternExpressionSyntax { Pattern: var pattern } && TAuditPatternCheck(pattern))
        {
            return false;
        }

        if (condition is BinaryExpressionSyntax binary
            && (binary.IsKind(SyntaxKind.EqualsExpression) || binary.IsKind(SyntaxKind.NotEqualsExpression))
            && (TAuditNullCheck(binary.Left) || TAuditNullCheck(binary.Right)))
        {
            return false;
        }

        return !TAuditVerdictCheck(condition) && TAuditDataCheck(condition);
    }

    private static bool TAuditVerdictCheck(ExpressionSyntax condition)
    {
        ExpressionSyntax core = condition;
        while (true)
        {
            core = core switch
            {
                ParenthesizedExpressionSyntax wrapped => wrapped.Expression,
                PrefixUnaryExpressionSyntax { RawKind: (int)SyntaxKind.LogicalNotExpression } negated
                    => negated.Operand,
                _ => core
            };
            if (core is not (ParenthesizedExpressionSyntax or PrefixUnaryExpressionSyntax))
            {
                break;
            }
        }

        string? name = core switch
        {
            InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax access }
                => access.Name.Identifier.ValueText,
            InvocationExpressionSyntax { Expression: IdentifierNameSyntax callee } => callee.Identifier.ValueText,
            MemberAccessExpressionSyntax access => access.Name.Identifier.ValueText,
            _ => null
        };
        return name is not null && TAuditLogicCheck(name);
    }

    private static bool TAuditPatternCheck(PatternSyntax pattern)
    {
        return pattern switch
        {
            ConstantPatternSyntax constant => TAuditNullCheck(constant.Expression),
            UnaryPatternSyntax not => TAuditPatternCheck(not.Pattern),
            DeclarationPatternSyntax => true,
            VarPatternSyntax => true,
            TypePatternSyntax => true,
            _ => false
        };
    }

    private static bool TAuditNullCheck(ExpressionSyntax expression)
    {
        return expression.IsKind(SyntaxKind.NullLiteralExpression)
               || expression.IsKind(SyntaxKind.DefaultLiteralExpression);
    }

    private static bool TAuditDataCheck(SyntaxNode node)
    {
        foreach (SyntaxNode child in node.DescendantNodesAndSelf())
        {
            bool logic = child switch
            {
                MemberAccessExpressionSyntax access => TAuditLogicCheck(access.Name.Identifier.ValueText),
                MemberBindingExpressionSyntax binding => TAuditLogicCheck(binding.Name.Identifier.ValueText),
                InvocationExpressionSyntax { Expression: IdentifierNameSyntax callee }
                    => TAuditLogicCheck(callee.Identifier.ValueText),
                _ => false
            };
            if (logic)
            {
                return true;
            }
        }

        return false;
    }

    private static bool TAuditWiredCheck(VariableDeclaratorSyntax variable)
    {
        return variable.Initializer?.Value is PostfixUnaryExpressionSyntax
        {
            RawKind: (int)SyntaxKind.SuppressNullableWarningExpression,
            Operand: LiteralExpressionSyntax { RawKind: (int)SyntaxKind.NullLiteralExpression }
        };
    }

    private static string? TAuditTypeRead(TypeSyntax type)
    {
        return type switch
        {
            SimpleNameSyntax simple => simple.Identifier.ValueText,
            QualifiedNameSyntax qualified => qualified.Right.Identifier.ValueText,
            _ => null
        };
    }

    private static string TAuditMemberRead(MemberDeclarationSyntax member)
    {
        return member switch
        {
            MethodDeclarationSyntax method => method.Identifier.ValueText,
            ConstructorDeclarationSyntax => "ctor",
            PropertyDeclarationSyntax property => property.Identifier.ValueText,
            EventDeclarationSyntax evt => evt.Identifier.ValueText,
            IndexerDeclarationSyntax => "this[]",
            OperatorDeclarationSyntax op => op.OperatorToken.ValueText,
            _ => member.Kind().ToString()
        };
    }

    private static bool TAuditLogicCheck(string name)
    {
        return name.Length >= 2 && name[0] == 'L' && char.IsUpper(name[1]);
    }

    private static int TAuditLineRead(SyntaxNode node)
    {
        return node.SyntaxTree.GetLineSpan(node.Span).StartLinePosition.Line + 1;
    }
}
