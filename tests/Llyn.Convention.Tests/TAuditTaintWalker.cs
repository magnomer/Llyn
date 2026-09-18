using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Convention.Tests;

internal static class TAuditTaintWalker
{
    private const string TAuditLogicColour = "logic value";

    private const string TAuditTextColour = "control input";

    private static readonly CSharpParseOptions TAuditSyntaxOptions = new(
        languageVersion: LanguageVersion.Preview,
        documentationMode: DocumentationMode.None,
        kind: SourceCodeKind.Regular);

    private static IReadOnlySet<string> TAuditReaderNames = new HashSet<string>();

    private static IReadOnlySet<string> TAuditControlNames = new HashSet<string>();

    public static IReadOnlyList<TViolation> TAuditRun(
        IEnumerable<string> sourcePaths, IReadOnlySet<string> readers, IReadOnlySet<string> controls)
    {
        TAuditReaderNames = readers;
        TAuditControlNames = controls;
        List<TViolation> violations = [];
        foreach (string path in sourcePaths)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText(path), TAuditSyntaxOptions, path);
            foreach (MemberDeclarationSyntax member in tree.GetRoot().DescendantNodes()
                         .OfType<MemberDeclarationSyntax>())
            {
                if (member is BaseTypeDeclarationSyntax or FieldDeclarationSyntax or BaseNamespaceDeclarationSyntax)
                {
                    continue;
                }

                TAuditMemberScan(member, violations);
            }
        }

        return violations;
    }

    private static void TAuditMemberScan(MemberDeclarationSyntax member, List<TViolation> violations)
    {
        Dictionary<string, string> tainted = TAuditTaintRead(member);
        HashSet<string> seen = new(StringComparer.Ordinal);
        foreach (SyntaxNode node in member.DescendantNodes())
        {
            if (node is MemberDeclarationSyntax)
            {
                continue;
            }

            (ExpressionSyntax TViolationSource, string TViolationReason)? sink = node switch
            {
                BinaryExpressionSyntax binary
                    when !binary.IsKind(SyntaxKind.CoalesceExpression)
                         && !binary.IsKind(SyntaxKind.LogicalAndExpression)
                         && !binary.IsKind(SyntaxKind.LogicalOrExpression)
                         && !TAuditNullCheck(binary.Left) && !TAuditNullCheck(binary.Right)
                    => (binary, $"in {binary.OperatorToken.ValueText}"),
                InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax access } query
                    when TAuditStrictSetting.TAuditTreatVerbs.Contains(
                        access.Name.Identifier.ValueText, StringComparer.Ordinal)
                    => (query, $"queried by {access.Name.Identifier.ValueText}"),
                IfStatementSyntax branch when TAuditConditionCheck(branch.Condition)
                    => (branch.Condition, "decides an if"),
                ConditionalExpressionSyntax choice when TAuditConditionCheck(choice.Condition)
                    => (choice.Condition, "decides a ternary"),
                SwitchStatementSyntax select => (select.Expression, "decides a switch"),
                SwitchExpressionSyntax arms => (arms.GoverningExpression, "decides a switch expression"),
                _ => null
            };
            if (sink is null || TAuditDataCheck(sink.Value.TViolationSource))
            {
                continue;
            }

            if (TAuditTaintFind(sink.Value.TViolationSource, tainted) is not (string name, string colour))
            {
                continue;
            }

            int line = TAuditLineRead(node);
            string reason = $"{colour} {sink.Value.TViolationReason} through '{name}'";
            if (seen.Add($"{line}:{reason}"))
            {
                string excerpt = node.ToString().Split('\n')[0].Trim();
                violations.Add(new TViolation(node.SyntaxTree.FilePath, line, excerpt, "Taint", reason));
            }
        }
    }

    private static Dictionary<string, string> TAuditTaintRead(MemberDeclarationSyntax member)
    {
        Dictionary<string, string> tainted = new(StringComparer.Ordinal);
        foreach (ParameterSyntax parameter in member.DescendantNodesAndSelf().OfType<ParameterSyntax>())
        {
            if (parameter.Type is not null && TAuditDataCheck(parameter.Type))
            {
                tainted[parameter.Identifier.ValueText] = TAuditLogicColour;
            }
        }

        foreach (SyntaxNode node in member.DescendantNodes())
        {
            switch (node)
            {
                case VariableDeclaratorSyntax { Initializer.Value: var value } declarator
                    when TAuditColourRead(value, tainted) is string colour:
                    tainted[declarator.Identifier.ValueText] = colour;
                    break;
                case AssignmentExpressionSyntax { Left: IdentifierNameSyntax local } assignment
                    when !local.Identifier.ValueText.StartsWith('_')
                         && TAuditColourRead(assignment.Right, tainted) is string colour:
                    tainted[local.Identifier.ValueText] = colour;
                    break;
                case ForEachStatementSyntax loop when TAuditColourRead(loop.Expression, tainted) is string colour:
                    tainted[loop.Identifier.ValueText] = colour;
                    break;
                case IsPatternExpressionSyntax pattern
                    when TAuditColourRead(pattern.Expression, tainted) is string colour:
                    foreach (SingleVariableDesignationSyntax designation in
                             pattern.Pattern.DescendantNodesAndSelf().OfType<SingleVariableDesignationSyntax>())
                    {
                        tainted[designation.Identifier.ValueText] = colour;
                    }

                    break;
            }
        }

        return tainted;
    }

    private static string? TAuditColourRead(ExpressionSyntax value, Dictionary<string, string> tainted)
    {
        return TAuditTaintFind(value, tainted)?.TViolationColour;
    }

    private static (string TViolationName, string TViolationColour)? TAuditTaintFind(
        SyntaxNode node, Dictionary<string, string> tainted)
    {
        foreach (SyntaxNode child in node.DescendantNodesAndSelf())
        {
            switch (child)
            {
                case IdentifierNameSyntax name when tainted.TryGetValue(name.Identifier.ValueText, out string? colour):
                    return (name.Identifier.ValueText, colour);
                case IdentifierNameSyntax name when TAuditLogicCheck(name.Identifier.ValueText):
                    return (name.Identifier.ValueText, TAuditLogicColour);
                case IdentifierNameSyntax name when TAuditReaderNames.Contains(name.Identifier.ValueText):
                    return (name.Identifier.ValueText, TAuditLogicColour);
                case MemberBindingExpressionSyntax binding when TAuditLogicCheck(binding.Name.Identifier.ValueText):
                    return (binding.Name.Identifier.ValueText, TAuditLogicColour);
                case MemberAccessExpressionSyntax input
                    when TAuditTruthSetting.TAuditInputMembers.Contains(
                             input.Name.Identifier.ValueText, StringComparer.Ordinal)
                         && TAuditOwnerRead(input.Expression) is string owner && TAuditControlNames.Contains(owner):
                    return (owner, TAuditTextColour);
            }
        }

        return null;
    }

    private static string? TAuditOwnerRead(ExpressionSyntax owner)
    {
        return owner switch
        {
            IdentifierNameSyntax name => name.Identifier.ValueText,
            MemberAccessExpressionSyntax { Expression: ThisExpressionSyntax } own => own.Name.Identifier.ValueText,
            _ => null
        };
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

        return !TAuditVerdictCheck(condition);
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
            IdentifierNameSyntax bare => bare.Identifier.ValueText,
            _ => null
        };
        return name is not null && (TAuditLogicCheck(name) || TAuditReaderNames.Contains(name));
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
        return node.DescendantNodesAndSelf().Any(child => child switch
        {
            IdentifierNameSyntax name => TAuditLogicCheck(name.Identifier.ValueText),
            MemberBindingExpressionSyntax binding => TAuditLogicCheck(binding.Name.Identifier.ValueText),
            _ => false
        });
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
