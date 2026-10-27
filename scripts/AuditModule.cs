using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

return LAuditModule.LAuditModuleRun(args);

internal static class LAuditModule
{
    private static readonly StringComparer _lAuditModuleOrdinal = StringComparer.Ordinal;
    private static readonly StringComparer _lAuditModuleIdentity = StringComparer.OrdinalIgnoreCase;

    public static int LAuditModuleRun(string[] arguments)
    {
        if (arguments.Length != 8)
        {
            Console.Error.WriteLine("usage: <config> <root> <version> <report> <binder> <page> <fresh> <freshness>");
            return 2;
        }

        string configPath = arguments[0];
        string root = Path.GetFullPath(arguments[1]);
        string version = arguments[2];
        string reportPath = arguments[3];
        string binderPath = arguments[4];
        string pagePath = arguments[5];
        bool fresh = arguments[6] == "fresh";
        string freshness = arguments[7];
        JsonElement config = JsonDocument.Parse(File.ReadAllText(configPath)).RootElement;
        LAuditConfigurationCheck(config, root);
        LAuditBinder binder = LAuditBinder.LAuditBinderRead(root, binderPath);
        List<LAuditModuleRow> modules = LAuditModuleRead(config, binder);
        List<LAuditModuleFinding> findings = LAuditFindingRead(binder, modules);
        List<string> evidence = LAuditEvidenceRead(modules);
        List<LAuditModuleCandidate> candidates = [];
        if (fresh)
        {
            try { candidates = LAuditCandidateRead(root, config.GetProperty("suggestions"), ref freshness); }
            catch (Exception error) { freshness = "unavailable/incomplete: " + error.Message; }
        }
        List<string> gates = LAuditGateRead(config, modules, findings);
        LAuditReportSave(config, version, reportPath, pagePath, modules, findings, evidence, candidates, freshness, gates);
        LAuditConsoleShow(config, modules, findings, evidence, candidates, freshness, gates, reportPath, pagePath);
        return config.GetProperty("enforced").GetBoolean() && gates.Count > 0 ? 3 : 0;
    }

    private static void LAuditConfigurationCheck(JsonElement config, string root)
    {
        LAuditKeysCheck(config, "root", ["generation", "project", "enforced", "helper", "source", "modules", "suggestions", "ceilings", "console", "report"]);
        LAuditKeysCheck(config.GetProperty("helper"), "helper", ["framework"]);
        LAuditKeysCheck(config.GetProperty("suggestions"), "suggestions", ["wiring", "minimumFiles"]);
        LAuditKeysCheck(config.GetProperty("console"), "console", ["top"]);
        LAuditKeysCheck(config.GetProperty("report"), "report", ["directory", "versionFile", "versionKey", "prefix"]);
        if (config.GetProperty("generation").GetInt32() != 27) throw new InvalidOperationException("AuditModule configuration must be generation 27.");
        if (config.GetProperty("suggestions").GetProperty("minimumFiles").GetInt32() < 2) throw new InvalidOperationException("suggestions.minimumFiles must be at least 2.");
        string source = config.GetProperty("source").GetString() ?? "";
        LAuditRelativeCheck(source, "source");
        HashSet<string> keys = new(_lAuditModuleOrdinal);
        HashSet<string> files = new(_lAuditModuleIdentity);
        foreach (JsonElement module in config.GetProperty("modules").EnumerateArray())
        {
            LAuditKeysCheck(module, "module", ["project", "name", "entrance", "files", "contracts"]);
            string project = module.GetProperty("project").GetString() ?? "";
            string name = module.GetProperty("name").GetString() ?? "";
            if (!keys.Add(project + "/" + name)) throw new InvalidOperationException($"Duplicate module key: {project}/{name}");
            HashSet<string> contracts = new(_lAuditModuleOrdinal);
            foreach (JsonElement contract in module.GetProperty("contracts").EnumerateArray())
                if (!contracts.Add(contract.GetString() ?? "")) throw new InvalidOperationException($"Duplicate contract in {project}/{name}.");
            foreach (JsonElement fileNode in module.GetProperty("files").EnumerateArray())
            {
                string file = fileNode.GetString() ?? "";
                LAuditRelativeCheck(file, "module file");
                if (!file.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) || !file.StartsWith(source, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException($"Module file is not C# under source: {file}");
                if (!files.Add(file)) throw new InvalidOperationException($"Module file belongs to more than one module: {file}");
                if (!File.Exists(Path.Combine(root, file.Replace('/', Path.DirectorySeparatorChar)))) throw new InvalidOperationException($"Module file does not exist: {file}");
                LAuditProjectCheck(root, project, file);
            }
        }
        foreach (JsonProperty ceiling in config.GetProperty("ceilings").EnumerateObject())
            if (ceiling.Value.GetInt32() < 0) throw new InvalidOperationException($"Negative ceiling: {ceiling.Name}");
    }
    private static void LAuditProjectCheck(string root, string project, string file)
    {
        string projectPath = Path.Combine(root, "src", project, project + ".csproj"); if (!File.Exists(projectPath)) throw new InvalidOperationException($"Module project does not exist: {project}");
        ProcessStartInfo info = new("dotnet") { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false, WorkingDirectory = root }; foreach (string argument in new[] { "msbuild", projectPath, "--nologo", "-getItem:Compile" }) info.ArgumentList.Add(argument);
        using Process process = Process.Start(info) ?? throw new InvalidOperationException("dotnet could not evaluate module project membership."); string output = process.StandardOutput.ReadToEnd(); string error = process.StandardError.ReadToEnd(); process.WaitForExit();
        if (process.ExitCode != 0) throw new InvalidOperationException($"Module project could not be evaluated: {project}\n{error}");
        string full = Path.GetFullPath(Path.Combine(root, file.Replace('/', Path.DirectorySeparatorChar))); using JsonDocument result = JsonDocument.Parse(output); bool included = result.RootElement.GetProperty("Items").GetProperty("Compile").EnumerateArray().Any(item => _lAuditModuleIdentity.Equals(Path.GetFullPath(item.GetProperty("FullPath").GetString()!), full));
        if (!included) throw new InvalidOperationException($"Module file is not compiled by project {project}: {file}");
    }
    private static void LAuditKeysCheck(JsonElement node, string owner, string[] expected)
    {
        HashSet<string> actual = node.EnumerateObject().Select(property => property.Name).ToHashSet(_lAuditModuleOrdinal);
        foreach (string key in expected) if (!actual.Remove(key)) throw new InvalidOperationException($"Missing {owner} key: {key}");
        if (actual.Count > 0) throw new InvalidOperationException($"Unknown {owner} key: {actual.OrderBy(key => key, _lAuditModuleOrdinal).First()}");
    }

    private static void LAuditRelativeCheck(string path, string owner)
    {
        string normalized = path.Replace('\\', '/').TrimEnd('/');
        if (normalized.Length == 0 || Path.IsPathRooted(path) || normalized.Split('/').Any(part => part is "" or "." or "..")) throw new InvalidOperationException($"{owner} path must be relative and contained: {path}");
    }

    private static List<LAuditModuleRow> LAuditModuleRead(JsonElement config, LAuditBinder binder)
    {
        Dictionary<string, SyntaxTree> trees = binder.LAuditTrees.ToDictionary(tree => binder.LAuditRelativeRead(tree.FilePath), _lAuditModuleIdentity);
        List<LAuditModuleRow> rows = [];
        foreach (JsonElement node in config.GetProperty("modules").EnumerateArray())
        {
            string project = node.GetProperty("project").GetString()!;
            string name = node.GetProperty("name").GetString()!;
            string[] files = node.GetProperty("files").EnumerateArray().Select(value => value.GetString()!).OrderBy(value => value, _lAuditModuleOrdinal).ToArray();
            if (files.Any(file => !trees.ContainsKey(file))) throw new InvalidOperationException($"Module file is not tracked bound source: {files.First(file => !trees.ContainsKey(file))}");
            INamedTypeSymbol entrance = LAuditTypeResolve(binder, node.GetProperty("entrance").GetString()!, project, files, "entrance");
            List<INamedTypeSymbol> contracts = node.GetProperty("contracts").EnumerateArray()
                .Select(value => LAuditTypeResolve(binder, value.GetString()!, project, files, "contract")).ToList();
            rows.Add(new(project, name, files, entrance, contracts));
        }
        Dictionary<string, LAuditModuleRow> owners = rows.SelectMany(row => row.LAuditFiles.Select(file => (file, row))).ToDictionary(pair => pair.file, pair => pair.row, _lAuditModuleIdentity);
        foreach (LAuditModuleRow row in rows)
        foreach (string file in row.LAuditFiles)
        {
            SyntaxTree tree = trees[file];
            SemanticModel model = binder.LAuditCompilation.GetSemanticModel(tree);
            foreach (BaseTypeDeclarationSyntax declaration in tree.GetRoot().DescendantNodes().OfType<BaseTypeDeclarationSyntax>().Where(value => value.Parent is not BaseTypeDeclarationSyntax))
            {
                INamedTypeSymbol symbol = (INamedTypeSymbol)model.GetDeclaredSymbol(declaration)!;
                HashSet<LAuditModuleRow> parts = symbol.DeclaringSyntaxReferences.Select(reference => binder.LAuditRelativeRead(reference.SyntaxTree.FilePath))
                    .Select(path => owners.GetValueOrDefault(path)).Where(owner => owner is not null).Cast<LAuditModuleRow>().ToHashSet();
                int declaredParts = symbol.DeclaringSyntaxReferences.Length;
                if (parts.Count != 1 || symbol.DeclaringSyntaxReferences.Any(reference => !owners.ContainsKey(binder.LAuditRelativeRead(reference.SyntaxTree.FilePath))))
                    throw new InvalidOperationException($"Top-level type declarations cross module membership: {symbol.ToDisplayString()}");
                if (declaredParts > 1 && !ReferenceEquals(parts.Single(), row)) throw new InvalidOperationException($"Partial top-level type crosses modules: {symbol.ToDisplayString()}");
            }
        }
        return rows.OrderBy(row => row.LAuditKey, _lAuditModuleOrdinal).ToList();
    }

    private static INamedTypeSymbol LAuditTypeResolve(LAuditBinder binder, string metadataName, string project, string[] files, string role)
    {
        INamedTypeSymbol symbol = binder.LAuditCompilation.GetTypeByMetadataName(metadataName)
            ?? throw new InvalidOperationException($"Module {role} does not resolve exactly once: {metadataName}");
        INamedTypeSymbol top = LAuditTopRead(symbol);
        string[] locations = top.Locations.Where(location => location.IsInSource).Select(location => binder.LAuditRelativeRead(location.SourceTree!.FilePath)).Distinct(_lAuditModuleIdentity).ToArray();
        if (!SymbolEqualityComparer.Default.Equals(top, symbol) || locations.Length == 0 || locations.Any(path => !files.Contains(path, _lAuditModuleIdentity)))
            throw new InvalidOperationException($"Module {role} is not a top-level type in its listed files: {metadataName}");
        return symbol;
    }

    private static List<LAuditModuleFinding> LAuditFindingRead(LAuditBinder binder, List<LAuditModuleRow> modules)
    {
        Dictionary<string, LAuditModuleRow> byFile = modules.SelectMany(row => row.LAuditFiles.Select(file => (file, row))).ToDictionary(pair => pair.file, pair => pair.row, _lAuditModuleIdentity);
        Dictionary<INamedTypeSymbol, LAuditModuleRow> boundary = new(SymbolEqualityComparer.Default);
        foreach (LAuditModuleRow row in modules) { boundary[row.LAuditEntrance] = row; foreach (INamedTypeSymbol contract in row.LAuditContracts) boundary[contract] = row; }
        HashSet<string> seen = new(_lAuditModuleOrdinal);
        List<LAuditModuleFinding> findings = [];
        Dictionary<string, HashSet<string>> edges = modules.ToDictionary(row => row.LAuditKey, _ => new HashSet<string>(_lAuditModuleOrdinal), _lAuditModuleOrdinal);
        foreach (SyntaxTree tree in binder.LAuditTrees)
        {
            string path = binder.LAuditRelativeRead(tree.FilePath);
            SemanticModel model = binder.LAuditCompilation.GetSemanticModel(tree);
            foreach (SimpleNameSyntax name in tree.GetRoot().DescendantNodes().OfType<SimpleNameSyntax>())
            {
                ISymbol? referenced = model.GetAliasInfo(name)?.Target ?? model.GetSymbolInfo(name).Symbol;
                INamedTypeSymbol? targetType = LAuditTargetRead(referenced);
                if (targetType is null) continue;
                INamedTypeSymbol? containing = model.GetEnclosingSymbol(name.SpanStart)?.ContainingType;
                if (containing is null) continue;
                LAuditModuleRow? sourceModule = LAuditOwnerRead(LAuditTopRead(containing), binder, byFile);
                LAuditModuleRow? targetModule = LAuditOwnerRead(targetType, binder, byFile);
                if (targetModule is null || ReferenceEquals(sourceModule, targetModule)) continue;
                if (sourceModule is not null && boundary.TryGetValue(targetType, out LAuditModuleRow? boundaryModule) && ReferenceEquals(boundaryModule, targetModule)) edges[sourceModule.LAuditKey].Add(targetModule.LAuditKey);
                if (boundary.ContainsKey(targetType)) continue;
                string symbol = referenced!.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
                string key = $"Bypassing:{targetModule.LAuditKey}:{path}:{name.Span.Start}-{name.Span.End}:{symbol}";
                if (seen.Add(key)) findings.Add(new("Bypassing", targetModule.LAuditKey, key, $"{path}:{name.GetLocation().GetLineSpan().StartLinePosition.Line + 1}: {symbol}"));
            }
        }
        foreach (LAuditModuleRow row in modules)
        foreach (ISymbol member in row.LAuditEntrance.GetMembers()
            .Where(member => !member.IsImplicitlyDeclared && member is not IMethodSymbol { AssociatedSymbol: not null })
            .Cast<ISymbol>().Prepend(row.LAuditEntrance).Where(LAuditAccessibleCheck))
        foreach (INamedTypeSymbol leaked in LAuditSignatureRead(member).Select(LAuditTopRead).Distinct(SymbolEqualityComparer.Default))
        {
            if (!ReferenceEquals(LAuditOwnerRead(leaked, binder, byFile), row) || boundary.ContainsKey(leaked)) continue;
            Location location = member.Locations.First(candidate => candidate.IsInSource);
            string symbol = $"{member.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)}->{leaked.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)}";
            string path = binder.LAuditRelativeRead(location.SourceTree!.FilePath);
            string key = $"Exposing:{row.LAuditKey}:{path}:{location.SourceSpan.Start}-{location.SourceSpan.End}:{symbol}";
            if (seen.Add(key)) findings.Add(new("Exposing", row.LAuditKey, key, $"{symbol}"));
        }
        foreach (string[] cycle in LAuditCycleRead(edges))
        {
            string project = modules.First(row => row.LAuditKey == cycle[0]).LAuditProject;
            string owner = project + "/" + string.Join("+", cycle.Select(key => key[(key.IndexOf('/') + 1)..]));
            findings.Add(new("Cycling", owner, "Cycling:" + owner, string.Join(" -> ", cycle)));
        }
        return findings.OrderBy(finding => finding.LAuditKey, _lAuditModuleOrdinal).ToList();
    }

    private static INamedTypeSymbol? LAuditTargetRead(ISymbol? symbol)
    {
        if (symbol is null) return null;
        INamedTypeSymbol? type = symbol as INamedTypeSymbol ?? symbol.ContainingType;
        return type is null ? null : LAuditTopRead(type.OriginalDefinition);
    }

    private static INamedTypeSymbol LAuditTopRead(INamedTypeSymbol symbol)
    {
        while (symbol.ContainingType is not null) symbol = symbol.ContainingType;
        return symbol.OriginalDefinition;
    }

    private static LAuditModuleRow? LAuditOwnerRead(INamedTypeSymbol symbol, LAuditBinder binder, Dictionary<string, LAuditModuleRow> byFile)
    {
        foreach (Location location in LAuditTopRead(symbol).Locations.Where(location => location.IsInSource))
            if (byFile.TryGetValue(binder.LAuditRelativeRead(location.SourceTree!.FilePath), out LAuditModuleRow? row)) return row;
        return null;
    }

    private static bool LAuditAccessibleCheck(ISymbol symbol) => symbol.DeclaredAccessibility is Accessibility.Public or Accessibility.Internal or Accessibility.Protected or Accessibility.ProtectedAndInternal or Accessibility.ProtectedOrInternal;

    private static IEnumerable<INamedTypeSymbol> LAuditSignatureRead(ISymbol member)
    {
        IEnumerable<ITypeSymbol> roots = member switch
        {
            IMethodSymbol method => method.Parameters.Select(parameter => parameter.Type).Append(method.ReturnType).Concat(method.TypeParameters.SelectMany(parameter => parameter.ConstraintTypes)),
            IPropertySymbol property => property.Parameters.Select(parameter => parameter.Type).Append(property.Type),
            IEventSymbol eventSymbol => [eventSymbol.Type],
            IFieldSymbol field => [field.Type],
            INamedTypeSymbol type => type.Interfaces.Cast<ITypeSymbol>().Append(type.BaseType!).Where(value => value is not null).Concat(type.TypeParameters.SelectMany(parameter => parameter.ConstraintTypes)),
            _ => []
        };
        HashSet<ITypeSymbol> visited = new(SymbolEqualityComparer.Default);
        Stack<ITypeSymbol> pending = new(roots.Where(type => type is not null));
        while (pending.TryPop(out ITypeSymbol? type) && visited.Add(type))
        {
            if (type is IArrayTypeSymbol array) pending.Push(array.ElementType);
            else if (type is IPointerTypeSymbol pointer) pending.Push(pointer.PointedAtType);
            else if (type is ITypeParameterSymbol parameter) foreach (ITypeSymbol constraint in parameter.ConstraintTypes) pending.Push(constraint);
            else if (type is INamedTypeSymbol named) { yield return named.OriginalDefinition; foreach (ITypeSymbol argument in named.TypeArguments) pending.Push(argument); }
        }
    }

    private static List<string[]> LAuditCycleRead(Dictionary<string, HashSet<string>> edges)
    {
        HashSet<string> unused = edges.Keys.ToHashSet(_lAuditModuleOrdinal);
        List<string[]> cycles = [];
        foreach (string start in edges.Keys.OrderBy(key => key, _lAuditModuleOrdinal))
        {
            if (!unused.Contains(start)) continue;
            HashSet<string> forward = LAuditReachRead(start, edges);
            string[] group = unused.Where(vertex => forward.Contains(vertex) && LAuditReachRead(vertex, edges).Contains(start)).OrderBy(vertex => vertex, _lAuditModuleOrdinal).ToArray();
            foreach (string vertex in group) unused.Remove(vertex);
            if (group.Length > 1) cycles.Add(group);
        }
        return cycles;
    }

    private static HashSet<string> LAuditReachRead(string start, Dictionary<string, HashSet<string>> edges)
    {
        HashSet<string> reached = new(_lAuditModuleOrdinal); Stack<string> pending = new(); pending.Push(start);
        while (pending.TryPop(out string? current) && reached.Add(current)) foreach (string next in edges[current]) pending.Push(next);
        return reached;
    }

    private static List<string> LAuditEvidenceRead(List<LAuditModuleRow> modules)
    {
        List<string> evidence = [];
        foreach (LAuditModuleRow row in modules)
        foreach (ISymbol member in row.LAuditEntrance.GetMembers().Where(LAuditAccessibleCheck))
        {
            if (member is IMethodSymbol method && (method.ReturnsByRef || method.ReturnsByRefReadonly || method.Parameters.Any(parameter => parameter.RefKind != RefKind.None))) evidence.Add($"{row.LAuditKey}: review evidence: ref-shaped entrance member {method.ToDisplayString()}");
            IEnumerable<ITypeSymbol> types = member switch { IMethodSymbol methodType => methodType.Parameters.Select(parameter => parameter.Type).Append(methodType.ReturnType), IPropertySymbol property => property.Parameters.Select(parameter => parameter.Type).Append(property.Type), IEventSymbol eventSymbol => [eventSymbol.Type], _ => [] };
            if (types.Any(type => type is IArrayTypeSymbol)) evidence.Add($"{row.LAuditKey}: review evidence: array-shaped entrance member {member.ToDisplayString()}");
            if (types.SelectMany(LAuditShapeRead).Any(type => type.OriginalDefinition.ToDisplayString() is "System.Collections.IList" or "System.Collections.IDictionary" or "System.Collections.Generic.ICollection<T>" or "System.Collections.Generic.IList<T>" or "System.Collections.Generic.IDictionary<TKey, TValue>" or "System.Collections.Generic.ISet<T>")) evidence.Add($"{row.LAuditKey}: review evidence: mutable collection-shaped entrance member {member.ToDisplayString()}");
            if (member is IPropertySymbol propertySymbol && propertySymbol.SetMethod is not null && LAuditAccessibleCheck(propertySymbol.SetMethod)) evidence.Add($"{row.LAuditKey}: review evidence: settable entrance property {member.ToDisplayString()}");
            if (member is IEventSymbol) evidence.Add($"{row.LAuditKey}: review evidence: entrance event {member.ToDisplayString()}");
        }
        foreach (LAuditModuleRow row in modules) foreach (INamedTypeSymbol contract in row.LAuditContracts) foreach (IMethodSymbol method in contract.GetMembers().OfType<IMethodSymbol>().Where(method => method.MethodKind == MethodKind.Ordinary && LAuditAccessibleCheck(method))) evidence.Add($"{row.LAuditKey}: review evidence: explicit contract method {method.ToDisplayString()}");
        return evidence.Distinct(_lAuditModuleOrdinal).OrderBy(value => value, _lAuditModuleOrdinal).ToList();
    }

    private static IEnumerable<INamedTypeSymbol> LAuditShapeRead(ITypeSymbol root)
    {
        Stack<ITypeSymbol> pending = new(); pending.Push(root);
        while (pending.TryPop(out ITypeSymbol? type)) { if (type is IArrayTypeSymbol array) pending.Push(array.ElementType); else if (type is INamedTypeSymbol named) { yield return named; foreach (ITypeSymbol argument in named.TypeArguments) pending.Push(argument); } }
    }

    private static List<LAuditModuleCandidate> LAuditCandidateRead(string root, JsonElement settings, ref string freshness)
    {
        string path = Path.Combine(root, settings.GetProperty("wiring").GetString()!.Replace('/', Path.DirectorySeparatorChar));
        using JsonDocument graph = JsonDocument.Parse(File.ReadAllText(path));
        JsonElement rootNode = graph.RootElement;
        if (!rootNode.TryGetProperty("meta", out JsonElement meta) || meta.GetProperty("version").GetInt32() != 1) throw new InvalidOperationException("Suggestions unavailable/incomplete: unsupported wiring schema.");
        Dictionary<string, string> nodes = rootNode.GetProperty("nodes").EnumerateArray().ToDictionary(node => node.GetProperty("id").GetString()!, node => node.GetProperty("path").GetString()!, _lAuditModuleOrdinal);
        Dictionary<(string, string), int> weights = [];
        Dictionary<(string, string), int> directed = [];
        int unknownRelations = 0, unknownConfidence = 0, unresolved = 0;
        foreach (JsonElement edge in rootNode.GetProperty("edges").EnumerateArray())
        {
            string relation = edge.GetProperty("relation").GetString()!;
            if (relation != "calls") { unknownRelations++; continue; }
            string confidence = edge.GetProperty("confidence").GetString()!;
            if (confidence is not ("extracted" or "inferred")) { unknownConfidence++; continue; }
            if (!nodes.TryGetValue(edge.GetProperty("source").GetString()!, out string? left) || !nodes.TryGetValue(edge.GetProperty("target").GetString()!, out string? right)) { unresolved++; continue; }
            left = left.Replace('\\', '/'); right = right.Replace('\\', '/');
            if (!left.StartsWith("src/", StringComparison.Ordinal) || !right.StartsWith("src/", StringComparison.Ordinal) || left == right) continue;
            string[] a = left.Split('/'); string[] b = right.Split('/'); if (a.Length < 3 || b.Length < 3 || a[1] != b[1]) continue;
            int weight = confidence == "extracted" ? 2 : 1;
            directed[(left, right)] = directed.GetValueOrDefault((left, right)) + weight;
            (string, string) pair = _lAuditModuleOrdinal.Compare(left, right) < 0 ? (left, right) : (right, left);
            weights[pair] = weights.GetValueOrDefault(pair) + weight;
        }
        if (unknownRelations + unknownConfidence + unresolved > 0) throw new InvalidOperationException($"Suggestions unavailable/incomplete: relation={unknownRelations}, confidence={unknownConfidence}, endpoint={unresolved}.");
        List<HashSet<string>> communities = weights.Keys.SelectMany(pair => new[] { pair.Item1, pair.Item2 }).Distinct(_lAuditModuleOrdinal).OrderBy(file => file, _lAuditModuleOrdinal).Select(file => new HashSet<string>([file], _lAuditModuleOrdinal)).ToList();
        long total = weights.Values.Sum();
        while (true)
        {
            Dictionary<string, int> membership = communities.SelectMany((group, index) => group.Select(file => (file, index))).ToDictionary(pair => pair.file, pair => pair.index, _lAuditModuleOrdinal);
            long[] degrees = new long[communities.Count];
            Dictionary<(int, int), int> between = [];
            foreach (KeyValuePair<(string, string), int> edge in weights)
            {
                int a = membership[edge.Key.Item1], b = membership[edge.Key.Item2];
                degrees[a] += edge.Value; degrees[b] += edge.Value;
                if (a != b) { (int, int) pair = a < b ? (a, b) : (b, a); between[pair] = between.GetValueOrDefault(pair) + edge.Value; }
            }
            (int, int, long, string, string) best = (-1, -1, 0, "", "");
            foreach (KeyValuePair<(int, int), int> link in between)
            {
                int a = link.Key.Item1, b = link.Key.Item2;
                long gain = 2 * total * link.Value - degrees[a] * degrees[b];
                string left = communities[a].Min(_lAuditModuleOrdinal)!; string right = communities[b].Min(_lAuditModuleOrdinal)!;
                if (_lAuditModuleOrdinal.Compare(left, right) > 0) (left, right) = (right, left);
                if (gain > best.Item3 || gain == best.Item3 && gain > 0 && (_lAuditModuleOrdinal.Compare(left, best.Item4) < 0 || left == best.Item4 && _lAuditModuleOrdinal.Compare(right, best.Item5) < 0)) best = (a, b, gain, left, right);
            }
            if (best.Item3 <= 0) break;
            communities[best.Item1].UnionWith(communities[best.Item2]); communities.RemoveAt(best.Item2);
        }
        int minimum = settings.GetProperty("minimumFiles").GetInt32();
        return communities.Where(group => group.Count >= minimum).Select(group =>
        {
            string[] files = group.OrderBy(file => file, _lAuditModuleOrdinal).ToArray();
            int inside = weights.Where(pair => group.Contains(pair.Key.Item1) && group.Contains(pair.Key.Item2)).Sum(pair => pair.Value);
            int boundary = weights.Where(pair => group.Contains(pair.Key.Item1) ^ group.Contains(pair.Key.Item2)).Sum(pair => pair.Value);
            int maximum = files.Max(file => directed.Where(pair => pair.Key.Item2 == file && !group.Contains(pair.Key.Item1)).Sum(pair => pair.Value));
            string[] entrances = files.Where(file => directed.Where(pair => pair.Key.Item2 == file && !group.Contains(pair.Key.Item1)).Sum(pair => pair.Value) == maximum).ToArray();
            return new LAuditModuleCandidate(files, entrances, inside, boundary);
        }).OrderByDescending(candidate => candidate.LAuditInternalWeight).ThenBy(candidate => candidate.LAuditBoundaryWeight).ThenBy(candidate => candidate.LAuditFiles[0], _lAuditModuleOrdinal).ToList();
    }

    private static List<string> LAuditGateRead(JsonElement config, List<LAuditModuleRow> modules, List<LAuditModuleFinding> findings)
    {
        Dictionary<string, int> counts = findings.GroupBy(finding => finding.LAuditKind + ":" + finding.LAuditOwner, _lAuditModuleOrdinal).ToDictionary(group => group.Key, group => group.Count(), _lAuditModuleOrdinal);
        HashSet<string> recognized = modules.SelectMany(row => new[] { "Bypassing:" + row.LAuditKey, "Exposing:" + row.LAuditKey }).Concat(findings.Where(finding => finding.LAuditKind == "Cycling").Select(finding => "Cycling:" + finding.LAuditOwner)).ToHashSet(_lAuditModuleOrdinal);
        Dictionary<string, int> ceilings = config.GetProperty("ceilings").EnumerateObject().ToDictionary(property => property.Name, property => property.Value.GetInt32(), _lAuditModuleOrdinal);
        string? unknown = ceilings.Keys.FirstOrDefault(key => !recognized.Contains(key)); if (unknown is not null) throw new InvalidOperationException($"Ceiling has no live recognized pair: {unknown}");
        if (!config.GetProperty("enforced").GetBoolean()) return [];
        List<string> gates = [];
        foreach (string key in recognized.OrderBy(key => key, _lAuditModuleOrdinal)) { int count = counts.GetValueOrDefault(key); int ceiling = ceilings.GetValueOrDefault(key); if (count > ceiling) gates.Add($"above ceiling: {key} {count}>{ceiling}"); else if (ceilings.ContainsKey(key) && count < ceiling) gates.Add($"stale ceiling: {key} {count}<{ceiling}"); }
        return gates;
    }

    private static void LAuditReportSave(JsonElement config, string version, string reportPath, string pagePath, List<LAuditModuleRow> modules, List<LAuditModuleFinding> findings, List<string> evidence, List<LAuditModuleCandidate> candidates, string freshness, List<string> gates)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);
        foreach (string old in Directory.GetFiles(Path.GetDirectoryName(reportPath)!, config.GetProperty("report").GetProperty("prefix").GetString()! + "*")) if (!_lAuditModuleIdentity.Equals(old, reportPath) && !_lAuditModuleIdentity.Equals(old, pagePath)) File.Delete(old);
        List<string> text = [$"# Module audit - {version}", "", $"- Modules: {modules.Count}", $"- Findings: {findings.Count}", $"- Graft: {freshness}", $"- Result: {(gates.Count == 0 ? "PASS" : "FAIL")}", "", "## Findings", ""];
        text.AddRange(findings.Count == 0 ? ["None."] : findings.Select(finding => $"- `{finding.LAuditKey}` - {finding.LAuditDetail}"));
        text.AddRange(["", "## Review evidence", ""]); text.AddRange(evidence.Count == 0 ? ["None."] : evidence.Select(item => "- " + item));
        text.AddRange(["", "## Advisory call-cohesion candidates", "", "These are call-cohesion candidates, not semantic module or ownership proof.", ""]);
        text.AddRange(candidates.Count == 0 ? ["None available."] : candidates.Select(candidate => $"- {string.Join(", ", candidate.LAuditFiles)}; entrance candidates: {string.Join(", ", candidate.LAuditEntrances)}; internal {candidate.LAuditInternalWeight}; boundary {candidate.LAuditBoundaryWeight}"));
        File.WriteAllText(reportPath, string.Join('\n', text) + "\n", new UTF8Encoding(false));
        Dictionary<string, object?> data = new(StringComparer.Ordinal) { ["version"] = version, ["freshness"] = freshness, ["modules"] = modules.Select(row => row.LAuditKey), ["findings"] = findings.Select(finding => new Dictionary<string, object?> { ["Kind"] = finding.LAuditKind, ["Owner"] = finding.LAuditOwner, ["Key"] = finding.LAuditKey, ["Detail"] = finding.LAuditDetail }), ["evidence"] = evidence, ["candidates"] = candidates.Select(candidate => new Dictionary<string, object?> { ["Files"] = candidate.LAuditFiles, ["Entrances"] = candidate.LAuditEntrances, ["InternalWeight"] = candidate.LAuditInternalWeight, ["BoundaryWeight"] = candidate.LAuditBoundaryWeight }), ["gates"] = gates };
        File.WriteAllText(pagePath, JsonSerializer.Serialize(data), new UTF8Encoding(false));
    }

    private static void LAuditConsoleShow(JsonElement config, List<LAuditModuleRow> modules, List<LAuditModuleFinding> findings, List<string> evidence, List<LAuditModuleCandidate> candidates, string freshness, List<string> gates, string reportPath, string pagePath)
    {
        int top = config.GetProperty("console").GetProperty("top").GetInt32();
        Console.WriteLine($"Scanned: {modules.Count} announced module(s)"); Console.WriteLine($"Suggestions: {freshness}");
        foreach (LAuditModuleFinding finding in findings.Take(top)) Console.WriteLine($"{finding.LAuditKind}: {finding.LAuditDetail}");
        Console.WriteLine($"Review evidence: {evidence.Count}"); Console.WriteLine($"Candidates: {candidates.Count}");
        foreach (string gate in gates) Console.WriteLine($"Gate: {gate}");
        List<(string LAuditGate, int LAuditCount, string LAuditMeaning)> rows =
        [
            ("Bypassing over ceiling", gates.Count(gate => gate.StartsWith("above ceiling: Bypassing:", StringComparison.Ordinal)), "modules whose bypassing findings exceed their ceiling"),
            ("Exposing over ceiling", gates.Count(gate => gate.StartsWith("above ceiling: Exposing:", StringComparison.Ordinal)), "modules whose exposing findings exceed their ceiling"),
            ("Cycling over ceiling", gates.Count(gate => gate.StartsWith("above ceiling: Cycling:", StringComparison.Ordinal)), "module cycles whose findings exceed their ceiling"),
            ("Stale ceilings", gates.Count(gate => gate.StartsWith("stale ceiling: ", StringComparison.Ordinal)), "ceilings above their live finding count"),
        ];
        System.Globalization.CultureInfo invariant = System.Globalization.CultureInfo.InvariantCulture;
        Console.WriteLine(); Console.WriteLine("Result"); Console.WriteLine("------");
        int countWidth = Math.Max(5, rows.Max(row => row.LAuditCount.ToString("N0", invariant).Length));
        int gateWidth = Math.Max(4, rows.Max(row => row.LAuditGate.Length));
        int meaningWidth = Math.Max(7, rows.Max(row => row.LAuditMeaning.Length));
        Console.WriteLine($"{"Status",-6}  {"Count".PadLeft(countWidth)}  {"Gate".PadRight(gateWidth)}  Meaning");
        Console.WriteLine($"{new string('-', 6)}  {new string('-', countWidth)}  {new string('-', gateWidth)}  {new string('-', meaningWidth)}");
        foreach ((string gate, int count, string meaning) in rows) Console.WriteLine($"{(count > 0 ? "FAIL" : "OK"),-6}  {count.ToString("N0", invariant).PadLeft(countWidth)}  {gate.PadRight(gateWidth)}  {meaning}");
        List<string> failed = rows.Where(row => row.LAuditCount > 0).Select(row => $"\"{row.LAuditGate}\"").ToList();
        Console.WriteLine();
        Console.WriteLine(failed.Count == 0 ? $"PASS: all {rows.Count} gates at 0." : $"FAIL: {failed.Count} of {rows.Count} gates above 0. See {string.Join(", ", failed)}.");
        Console.WriteLine($"Report: {reportPath}");
    }
}

internal sealed record LAuditModuleRow(string LAuditProject, string LAuditName, string[] LAuditFiles, INamedTypeSymbol LAuditEntrance, List<INamedTypeSymbol> LAuditContracts)
{
    public string LAuditKey => LAuditProject + "/" + LAuditName;
}
internal sealed record LAuditModuleFinding(string LAuditKind, string LAuditOwner, string LAuditKey, string LAuditDetail);
internal sealed record LAuditModuleCandidate(string[] LAuditFiles, string[] LAuditEntrances, int LAuditInternalWeight, int LAuditBoundaryWeight);

