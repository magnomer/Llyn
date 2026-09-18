using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Convention.Tests;

internal static partial class TAuditTruthWalker
{
    private static HashSet<string> TAuditRelayNames = [];

    private static HashSet<string> TAuditReaderNames = [];

    private static Dictionary<string, HashSet<int>> TAuditHotNames = [];

    private static HashSet<string> TAuditRelayRead(IReadOnlyList<TypeDeclarationSyntax> type)
    {
        TAuditRelayNames = new HashSet<string>(StringComparer.Ordinal);
        TAuditReaderNames = new HashSet<string>(StringComparer.Ordinal);
        TAuditHotNames = new Dictionary<string, HashSet<int>>(StringComparer.Ordinal);
        List<MemberDeclarationSyntax> members = type
            .SelectMany(part => part.DescendantNodesAndSelf().OfType<TypeDeclarationSyntax>())
            .SelectMany(part => part.Members)
            .ToList();
        bool grown = true;
        while (grown)
        {
            grown = false;
            foreach (MemberDeclarationSyntax member in members)
            {
                string? name = member switch
                {
                    MethodDeclarationSyntax method => method.Identifier.ValueText,
                    PropertyDeclarationSyntax property => property.Identifier.ValueText,
                    _ => null
                };
                if (name is null)
                {
                    continue;
                }

                if (!TAuditRelayNames.Contains(name) && TAuditRequestCheck(member))
                {
                    TAuditRelayNames.Add(name);
                    grown = true;
                }

                if (!TAuditReaderNames.Contains(name) && (TAuditReadCheck(member) || TAuditRequestCheck(member)))
                {
                    TAuditReaderNames.Add(name);
                    grown = true;
                }

                if (member is MethodDeclarationSyntax hot && TAuditHotRead(name, hot))
                {
                    grown = true;
                }
            }
        }

        return TAuditRelayNames;
    }

    private static bool TAuditHotRead(string name, MethodDeclarationSyntax method)
    {
        if (!TAuditHotNames.TryGetValue(name, out HashSet<int>? hot))
        {
            hot = [];
            TAuditHotNames[name] = hot;
        }

        List<string> parameters = method.ParameterList.Parameters.Select(p => p.Identifier.ValueText).ToList();
        bool grown = false;
        foreach (ArgumentSyntax argument in method.DescendantNodes().OfType<ArgumentSyntax>())
        {
            if (argument.Parent?.Parent is not ExpressionSyntax call
                || TAuditCallRead(call) is not string callee
                || !TAuditHotCheck(callee, argument))
            {
                continue;
            }

            foreach (IdentifierNameSyntax used in argument.Expression.DescendantNodesAndSelf()
                         .OfType<IdentifierNameSyntax>())
            {
                int index = parameters.IndexOf(used.Identifier.ValueText);
                if (index >= 0 && hot.Add(index))
                {
                    grown = true;
                }
            }
        }

        return grown;
    }

    private static bool TAuditReadCheck(MemberDeclarationSyntax member)
    {
        return member.DescendantNodes().Any(node => node switch
        {
            MemberAccessExpressionSyntax access => TAuditLogicCheck(access.Name.Identifier.ValueText),
            MemberBindingExpressionSyntax binding => TAuditLogicCheck(binding.Name.Identifier.ValueText),
            InvocationExpressionSyntax call
                => TAuditNameRead(call.Expression) is string callee && TAuditReaderNames.Contains(callee),
            _ => false
        });
    }
}
