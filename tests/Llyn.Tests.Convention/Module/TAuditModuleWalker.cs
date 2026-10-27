using System.Diagnostics;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Convention.Tests;

internal static class TAuditModuleWalker
{
    public static IReadOnlyList<TAuditModuleFinding> TAuditRun(IReadOnlyList<TAuditModuleRow> rows)
    {
        IReadOnlyList<SyntaxTree> trees = TAuditBinder.TAuditTrees;
        IReadOnlyList<(INamedTypeSymbol, string)> declarations = TAuditDeclarationRead(trees);
        IReadOnlyList<TAuditModuleType> modules = TAuditConfigurationRead(rows, trees, declarations);
        Dictionary<INamedTypeSymbol, TAuditModuleType> owners = new(SymbolEqualityComparer.Default);
        foreach (TAuditModuleType module in modules)
        {
            foreach (INamedTypeSymbol declaration in module.TAuditDeclarations)
            {
                owners[declaration] = module;
            }
        }

        Dictionary<string, HashSet<string>> edges = modules.ToDictionary(
            module => module.TAuditRow.TAuditKey,
            _ => new HashSet<string>(StringComparer.Ordinal),
            StringComparer.Ordinal);
        Dictionary<string, TAuditModuleType> keyed = modules.ToDictionary(
            module => module.TAuditRow.TAuditKey, StringComparer.Ordinal);
        List<TAuditModuleFinding> findings = [];
        IEnumerable<SyntaxTree> orderedTrees = trees.OrderBy(
            tree => TAuditBinder.TAuditRelativeRead(tree.FilePath),
            StringComparer.Ordinal);
        foreach (SyntaxTree tree in orderedTrees)
        {
            SemanticModel model = TAuditBinder.TAuditModelRead(tree);
            string path = TAuditBinder.TAuditRelativeRead(tree.FilePath);
            foreach (SimpleNameSyntax name in tree.GetRoot().DescendantNodes().OfType<SimpleNameSyntax>())
            {
                ISymbol? symbol = name is IdentifierNameSyntax identifier
                    ? model.GetAliasInfo(identifier)?.Target
                    : null;
                symbol ??= model.GetSymbolInfo(name).Symbol;
                if (symbol is null || !TAuditTargetRead(
                    symbol,
                    owners,
                    out INamedTypeSymbol target,
                    out TAuditModuleType destination))
                {
                    continue;
                }

                INamedTypeSymbol? containing = model.GetEnclosingSymbol(name.SpanStart)?.ContainingType;
                if (containing is null)
                {
                    continue;
                }

                INamedTypeSymbol source = TAuditModuleType.TAuditTopRead(containing);
                owners.TryGetValue(source, out TAuditModuleType? origin);
                if (origin is not null && !ReferenceEquals(origin, destination)
                    && destination.TAuditContractCheck(target))
                {
                    edges[origin.TAuditRow.TAuditKey].Add(destination.TAuditRow.TAuditKey);
                }

                if (!ReferenceEquals(origin, destination) && !destination.TAuditContractCheck(target))
                {
                    findings.Add(new TAuditModuleFinding(
                        "Bypassing",
                        destination.TAuditRow.TAuditProject,
                        destination.TAuditRow.TAuditName,
                        path,
                        $"{name.SpanStart}-{name.Span.End}",
                        symbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)));
                }
            }
        }

        findings.AddRange(TAuditExposureRead(modules));
        findings.AddRange(TAuditCycleRead(edges, keyed));
        return findings
            .DistinctBy(finding => finding.TAuditKey, StringComparer.Ordinal)
            .OrderBy(finding => finding.TAuditKey, StringComparer.Ordinal)
            .ToList();
    }

    private static IReadOnlyList<(INamedTypeSymbol, string)> TAuditDeclarationRead(
        IReadOnlyList<SyntaxTree> trees)
    {
        List<(INamedTypeSymbol, string)> declarations = [];
        foreach (SyntaxTree tree in trees)
        {
            SemanticModel model = TAuditBinder.TAuditModelRead(tree);
            IEnumerable<MemberDeclarationSyntax> nodes = tree.GetRoot().DescendantNodes()
                .Where(node => node is BaseTypeDeclarationSyntax or DelegateDeclarationSyntax)
                .Cast<MemberDeclarationSyntax>()
                .Where(node => !node.Ancestors().Any(owner =>
                    owner is BaseTypeDeclarationSyntax or DelegateDeclarationSyntax));
            foreach (MemberDeclarationSyntax node in nodes)
            {
                if (model.GetDeclaredSymbol(node) is INamedTypeSymbol symbol)
                {
                    declarations.Add((symbol.OriginalDefinition, TAuditBinder.TAuditRelativeRead(tree.FilePath)));
                }
            }
        }

        return declarations;
    }

    private static IReadOnlyList<TAuditModuleType> TAuditConfigurationRead(
        IReadOnlyList<TAuditModuleRow> rows,
        IReadOnlyList<SyntaxTree> trees,
        IReadOnlyList<(INamedTypeSymbol, string)> declarations)
    {
        List<string> errors = [];
        Dictionary<string, SyntaxTree> tracked = trees.ToDictionary(
            tree => TAuditBinder.TAuditRelativeRead(tree.FilePath),
            StringComparer.OrdinalIgnoreCase);
        foreach (IGrouping<string, TAuditModuleRow> duplicate in rows.GroupBy(
            row => row.TAuditKey, StringComparer.Ordinal).Where(group => group.Count() > 1))
        {
            errors.Add($"Duplicate module key {duplicate.Key}.");
        }

        Dictionary<string, TAuditModuleRow> fileOwners = new(StringComparer.OrdinalIgnoreCase);
        foreach (TAuditModuleRow row in rows)
        {
            foreach (string duplicate in row.TAuditFiles.GroupBy(path => path, StringComparer.Ordinal)
                .Where(group => group.Count() > 1).Select(group => group.Key))
            {
                errors.Add($"Duplicate file {duplicate} in {row.TAuditKey}.");
            }
            foreach (string duplicate in row.TAuditContracts.GroupBy(name => name, StringComparer.Ordinal)
                .Where(group => group.Count() > 1).Select(group => group.Key))
            {
                errors.Add($"Duplicate contract {duplicate} in {row.TAuditKey}.");
            }

            foreach (string file in row.TAuditFiles)
            {
                if (!TAuditPathCheck(file, row.TAuditProject) || !tracked.ContainsKey(file))
                {
                    errors.Add($"Missing or invalid tracked source {file} in {row.TAuditKey}.");
                    continue;
                }
                TAuditProjectCheck(file, row.TAuditProject);
                if (!fileOwners.TryAdd(file, row))
                {
                    errors.Add($"File {file} belongs to more than one module.");
                }
            }
        }

        IEqualityComparer<INamedTypeSymbol> symbolComparer = SymbolEqualityComparer.Default;
        foreach (IGrouping<INamedTypeSymbol, (INamedTypeSymbol, string)> parts in
            declarations.GroupBy(declaration => declaration.Item1, symbolComparer))
        {
            if (parts.Count() < 2)
            {
                continue;
            }
            List<TAuditModuleRow?> owners = parts.Select(part =>
                fileOwners.GetValueOrDefault(part.Item2)).Distinct().ToList();
            if (owners.Any(owner => owner is not null) && (owners.Count != 1 || owners[0] is null))
            {
                errors.Add($"Partial type {TAuditModuleType.TAuditNameRead(parts.Key)} crosses module ownership.");
            }
        }

        List<TAuditModuleType> modules = [];
        foreach (TAuditModuleRow row in rows)
        {
            HashSet<string> files = new(row.TAuditFiles, StringComparer.OrdinalIgnoreCase);
            HashSet<INamedTypeSymbol> unique = new(SymbolEqualityComparer.Default);
            List<INamedTypeSymbol> declared = declarations.Where(item => files.Contains(item.Item2))
                .Select(item => item.Item1)
                .Where(unique.Add)
                .ToList();
            List<INamedTypeSymbol> entrances = declared
                .Where(type => string.Equals(
                    TAuditModuleType.TAuditNameRead(type),
                    row.TAuditEntrance,
                    StringComparison.Ordinal))
                .ToList();
            if (entrances.Count != 1)
            {
                errors.Add($"Entrance {row.TAuditEntrance} does not resolve exactly once in {row.TAuditKey}.");
                continue;
            }

            List<INamedTypeSymbol> contracts = [];
            foreach (string contractName in row.TAuditContracts)
            {
                List<INamedTypeSymbol> matches = declared.Where(type => string.Equals(
                    TAuditModuleType.TAuditNameRead(type), contractName, StringComparison.Ordinal)).ToList();
                if (matches.Count != 1)
                {
                    errors.Add($"Contract {contractName} does not resolve exactly once in {row.TAuditKey}.");
                }
                else
                {
                    contracts.Add(matches[0]);
                }
            }

            modules.Add(new TAuditModuleType(row, entrances[0], contracts, declared));
        }

        if (errors.Count > 0)
        {
            throw new InvalidOperationException(string.Join('\n', errors.Order(StringComparer.Ordinal)));
        }

        return modules;
    }

    private static bool TAuditPathCheck(string path, string project)
    {
        if (Path.IsPathRooted(path) || path.Contains('\\') || path.Split('/').Contains("..", StringComparer.Ordinal)
            || !path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
            return false;

        string[] parts = path.Split('/');
        return parts.Length >= 3 && string.Equals(parts[0], "src", StringComparison.Ordinal);
    }

    private static void TAuditProjectCheck(string file, string project)
    {
        string projectPath = Path.Combine(TAuditBinder.TAuditRoot, "src", project, project + ".csproj");
        if (!File.Exists(projectPath))
        {
            string[] parts = file.Split('/');
            if (parts.Length >= 3 && string.Equals(parts[1], project, StringComparison.Ordinal)) return;
            throw new InvalidOperationException($"Module project does not exist: {project}");
        }
        ProcessStartInfo info = new("dotnet")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            WorkingDirectory = TAuditBinder.TAuditRoot,
        };
        foreach (string argument in new[] { "msbuild", projectPath, "--nologo", "-getItem:Compile" })
            info.ArgumentList.Add(argument);
        using Process process = Process.Start(info)
            ?? throw new InvalidOperationException("dotnet could not evaluate module project membership.");
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd(); process.WaitForExit();
        if (process.ExitCode != 0)
            throw new InvalidOperationException(
                $"Module project could not be evaluated: {project}\n{error}");
        string full = Path.GetFullPath(Path.Combine(
            TAuditBinder.TAuditRoot,
            file.Replace('/', Path.DirectorySeparatorChar)));
        using JsonDocument result = JsonDocument.Parse(output);
        bool included = result.RootElement.GetProperty("Items").GetProperty("Compile").EnumerateArray()
            .Any(item => StringComparer.OrdinalIgnoreCase.Equals(
                Path.GetFullPath(item.GetProperty("FullPath").GetString()!),
                full));
        if (!included) throw new InvalidOperationException(
            $"Module file is not compiled by project {project}: {file}");
    }

    private static bool TAuditTargetRead(
        ISymbol symbol,
        IReadOnlyDictionary<INamedTypeSymbol, TAuditModuleType> owners,
        out INamedTypeSymbol target,
        out TAuditModuleType module)
    {
        INamedTypeSymbol? resolved = symbol is IAliasSymbol alias
            ? alias.Target as INamedTypeSymbol
            : symbol as INamedTypeSymbol;
        resolved ??= symbol.ContainingType;
        if (resolved is null)
        {
            target = null!;
            module = null!;
            return false;
        }

        target = TAuditModuleType.TAuditTopRead(resolved);
        return owners.TryGetValue(target, out module!);
    }

    private static IReadOnlyList<TAuditModuleFinding> TAuditExposureRead(IReadOnlyList<TAuditModuleType> modules)
    {
        List<TAuditModuleFinding> findings = [];
        foreach (TAuditModuleType module in modules)
        {
            IEnumerable<ISymbol> members = new ISymbol[] { module.TAuditEntrance }
                .Concat(module.TAuditEntrance.GetMembers().Where(member =>
                    !member.IsImplicitlyDeclared && member is not IMethodSymbol { AssociatedSymbol: not null }));
            foreach (ISymbol member in members.Where(member =>
                SymbolEqualityComparer.Default.Equals(member, module.TAuditEntrance)
                || TAuditModuleType.TAuditVisibleCheck(member)))
            {
                Location? location = member.Locations.FirstOrDefault(candidate => candidate.IsInSource);
                if (location is null)
                {
                    continue;
                }
                foreach (INamedTypeSymbol leaked in TAuditModuleType.TAuditShapeRead(
                    TAuditModuleType.TAuditSignatureRead(member)))
                {
                    if (!module.TAuditDeclarationCheck(leaked) || module.TAuditContractCheck(leaked))
                    {
                        continue;
                    }

                    findings.Add(new TAuditModuleFinding(
                        "Exposing",
                        module.TAuditRow.TAuditProject,
                        module.TAuditRow.TAuditName,
                        TAuditBinder.TAuditRelativeRead(location.SourceTree!.FilePath),
                        $"{location.SourceSpan.Start}-{location.SourceSpan.End}",
                        $"{member.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)}"
                            + $"->{TAuditModuleType.TAuditNameRead(leaked)}"));
                }
            }
        }

        return findings;
    }

    private static IReadOnlyList<TAuditModuleFinding> TAuditCycleRead(
        IReadOnlyDictionary<string, HashSet<string>> edges,
        IReadOnlyDictionary<string, TAuditModuleType> modules)
    {
        Dictionary<string, int> indices = new(StringComparer.Ordinal);
        Dictionary<string, int> lows = new(StringComparer.Ordinal);
        HashSet<string> stacked = new(StringComparer.Ordinal);
        Stack<string> stack = new();
        List<IReadOnlyList<string>> components = [];
        int index = 0;
        foreach (string vertex in edges.Keys.Order(StringComparer.Ordinal))
        {
            if (!indices.ContainsKey(vertex))
            {
                TAuditComponentRead(vertex, edges, indices, lows, stacked, stack, components, ref index);
            }
        }

        return components.Where(component => component.Count > 1).Select(component =>
        {
            string[] keys = component.Order(StringComparer.Ordinal).ToArray();
            string project = string.Join("+", keys.Select(key => modules[key].TAuditRow.TAuditProject)
                .Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal));
            string names = string.Join("+", keys.Select(key => modules[key].TAuditRow.TAuditName)
                .Order(StringComparer.Ordinal));
            return new TAuditModuleFinding("Cycling", project, names, "", "", "");
        }).ToList();
    }

    private static void TAuditComponentRead(
        string vertex,
        IReadOnlyDictionary<string, HashSet<string>> edges,
        IDictionary<string, int> indices,
        IDictionary<string, int> lows,
        ISet<string> stacked,
        Stack<string> stack,
        ICollection<IReadOnlyList<string>> components,
        ref int index)
    {
        indices[vertex] = index;
        lows[vertex] = index++;
        stack.Push(vertex);
        stacked.Add(vertex);
        foreach (string target in edges[vertex].Order(StringComparer.Ordinal))
        {
            if (!indices.ContainsKey(target))
            {
                TAuditComponentRead(target, edges, indices, lows, stacked, stack, components, ref index);
                lows[vertex] = Math.Min(lows[vertex], lows[target]);
            }
            else if (stacked.Contains(target))
            {
                lows[vertex] = Math.Min(lows[vertex], indices[target]);
            }
        }

        if (lows[vertex] != indices[vertex])
        {
            return;
        }

        List<string> component = [];
        string member;
        do
        {
            member = stack.Pop();
            stacked.Remove(member);
            component.Add(member);
        }
        while (!string.Equals(member, vertex, StringComparison.Ordinal));
        components.Add(component);
    }
}

