using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace Llyn.UIDemeanor;

public abstract class BBlueprint
{
    private static readonly Dictionary<string, string[]> BBlueprintVocabulary = new(StringComparer.Ordinal)
    {
        ["Blueprint"] = ["x:Name", "x:Class", "Source"],
        ["Split"] = ["Rows", "Columns", "Margin"],
        ["Frame"] = ["x:Name", "Theme", "Glyphs", "Rows", "Margin"],
        ["Text"] = ["x:Name", "Theme", "Margin"],
        ["Legend"] = ["x:Name", "Theme", "Margin"],
        ["Glyphs"] =
        [
            "x:Key", "Frame", "Rule", "Favorite", "Clip", "Order", "Pending", "History",
            "Submenu", "Ellipsis", "Dot", "Arrow", "Scroll", "Check", "Radio", "Grasp", "Fold", "Media", "Tree",
        ],
    };

    protected BBlueprint()
    {
    }

    internal Dictionary<string, BBlueprintPart> BBlueprintParts { get; } = new(StringComparer.Ordinal);

    internal Dictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>> BBlueprintGlyphs { get; } =
        new(StringComparer.Ordinal);

    protected static void BBlueprintLoad(BBlueprint shell)
    {
        ArgumentNullException.ThrowIfNull(shell);

        Type type = shell.GetType();
        Assembly assembly = type.Assembly;
        string owner = type.Name;
        string[] own = [.. assembly.GetManifestResourceNames()
            .Where(name => name.EndsWith($".{owner}.xaml", StringComparison.Ordinal))];
        if (own.Length != 1)
        {
            throw new InvalidDataException($"The blueprint of '{owner}' is missing.");
        }

        using Stream list = assembly.GetManifestResourceStream(own[0])!;
        BBlueprintPart root = BBlueprintRead(list);
        if (root.BBlueprintPartClass != type.FullName)
        {
            throw new InvalidDataException($"The blueprint of '{owner}' takes the x:Class '{type.FullName}'.");
        }

        foreach (BBlueprintPart listed in root.BBlueprintPartChildren)
        {
            if (listed is { BBlueprintPartKey: string key, BBlueprintPartMarks: { } marks })
            {
                if (!shell.BBlueprintGlyphs.TryAdd(key, marks))
                {
                    throw new InvalidDataException($"The glyphs '{key}' are listed twice.");
                }

                continue;
            }

            if (listed.BBlueprintPartSource is not string source)
            {
                throw new InvalidDataException($"The blueprint of '{owner}' holds a part; it lists blueprints only.");
            }

            string resourceName = $"{assembly.GetName().Name}.{source.Replace('/', '.')}";
            using Stream markup = assembly.GetManifestResourceStream(resourceName)
                ?? throw new InvalidDataException($"The blueprint '{source}' is missing.");
            BBlueprintPart blueprint = BBlueprintRead(markup);
            IReadOnlyList<BBlueprintPart> children = blueprint.BBlueprintPartChildren;
            string? fault = children.Any(child => child.BBlueprintPartSource is not null) ? "lists blueprints"
                : children.Any(child => child.BBlueprintPartElement == "Glyphs") ? "lists glyphs"
                : blueprint.BBlueprintPartClass is not null ? "takes an x:Class"
                : null;
            if (fault is not null)
            {
                throw new InvalidDataException($"The blueprint '{source}' {fault}; only the shell's blueprint does.");
            }

            if (!shell.BBlueprintParts.TryAdd(blueprint.BBlueprintPartName!, blueprint))
            {
                throw new InvalidDataException($"The blueprint '{blueprint.BBlueprintPartName}' is listed twice.");
            }
        }

        foreach ((string name, BBlueprintPart blueprint) in shell.BBlueprintParts)
        {
            Stack<BBlueprintPart> parts = new([blueprint]);
            while (parts.TryPop(out BBlueprintPart? part))
            {
                if (part.BBlueprintPartGlyphs is string set && !shell.BBlueprintGlyphs.ContainsKey(set))
                {
                    throw new InvalidDataException(
                        $"The blueprint '{name}' draws the Glyphs '{set}', which are unknown.");
                }

                foreach (BBlueprintPart child in part.BBlueprintPartChildren)
                {
                    parts.Push(child);
                }
            }
        }
    }

    internal static BBlueprintPart BBlueprintRead(Stream markup)
    {
        ArgumentNullException.ThrowIfNull(markup);

        XmlReaderSettings settings = new()
        {
            DtdProcessing = DtdProcessing.Prohibit,
            IgnoreComments = true,
            IgnoreProcessingInstructions = true,
            IgnoreWhitespace = true,
        };
        using XmlReader reader = XmlReader.Create(markup, settings);
        reader.MoveToContent();
        BBlueprintPart blueprint = BBlueprintElementRead(reader, new HashSet<string>(StringComparer.Ordinal));
        if (blueprint.BBlueprintPartElement != "Blueprint"
            || (blueprint.BBlueprintPartName ?? blueprint.BBlueprintPartClass) is null)
        {
            throw new InvalidDataException("A blueprint file holds one Blueprint with an x:Name or an x:Class.");
        }

        return blueprint;
    }

    private static BBlueprintPart BBlueprintElementRead(XmlReader reader, HashSet<string> names)
    {
        string element = reader.Name;
        int line = reader is IXmlLineInfo info ? info.LineNumber : 0;
        string site = $"Blueprint line {line}";
        if (!BBlueprintVocabulary.TryGetValue(element, out string[]? allowed))
        {
            throw new InvalidDataException($"{site}: the element '{element}' is unknown.");
        }

        Dictionary<string, string> values = new(StringComparer.Ordinal);
        if (reader.MoveToFirstAttribute())
        {
            do
            {
                if (reader.NamespaceURI == "http://www.w3.org/2000/xmlns/")
                {
                    continue;
                }

                string attribute = reader.NamespaceURI switch
                {
                    "" => reader.LocalName,
                    "http://schemas.microsoft.com/winfx/2006/xaml" => $"x:{reader.LocalName}",
                    _ => $"{{{reader.NamespaceURI}}}{reader.LocalName}",
                };
                if (!allowed.Contains(attribute, StringComparer.Ordinal))
                {
                    throw new InvalidDataException($"{site}: the attribute '{reader.Name}' is unknown on '{element}'.");
                }

                values[attribute] = reader.Value;
            }
            while (reader.MoveToNextAttribute());
            reader.MoveToElement();
        }

        List<BBlueprintPart> children = [];
        bool empty = reader.IsEmptyElement;
        reader.Read();
        if (!empty)
        {
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.NodeType != XmlNodeType.Element)
                {
                    throw new InvalidDataException($"{site}: '{element}' holds text; a blueprint holds only parts.");
                }

                children.Add(BBlueprintElementRead(reader, names));
            }

            reader.Read();
        }

        string? name = values.GetValueOrDefault("x:Name");
        string? source = values.GetValueOrDefault("Source");
        string? key = values.GetValueOrDefault("x:Key");
        string? glyphs = values.GetValueOrDefault("Glyphs");
        Dictionary<string, IReadOnlyList<string>>? marks = null;
        if (element == "Glyphs")
        {
            marks = new(StringComparer.Ordinal);
            foreach ((string mark, string value) in values)
            {
                if (mark != "x:Key")
                {
                    marks[mark] = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                }
            }
        }

        string? rows = values.GetValueOrDefault("Rows");
        string? columns = values.GetValueOrDefault("Columns");
        string? inset = values.GetValueOrDefault("Margin");
        IReadOnlyList<BBlueprintTrack> tracks = rows is not null ? BBlueprintTrack.BBlueprintTrackParse(rows)
            : columns is not null ? BBlueprintTrack.BBlueprintTrackParse(columns)
            : [];
        int[] sides = inset is null
            ? [0]
            : [.. inset.Split(',', StringSplitOptions.TrimEntries).Select(side =>
                int.TryParse(side, NumberStyles.None, CultureInfo.InvariantCulture, out int cells) ? cells : -1)];
        int[] margin = sides.Length switch
        {
            1 => [sides[0], sides[0], sides[0], sides[0]],
            2 => [sides[0], sides[1], sides[0], sides[1]],
            4 => sides,
            _ => [-1],
        };
        int room = element switch
        {
            "Blueprint" => 1,
            "Split" or "Frame" => Math.Max(tracks.Count, 1),
            _ => 0,
        };
        int parts = children.Count(child => child.BBlueprintPartSource is null && child.BBlueprintPartMarks is null);

        string? fault =
            name is not null && !names.Add(name) ? $"the Name '{name}' is used twice"
            : element == "Blueprint" && new[] { "x:Name", "x:Class", "Source" }.Count(values.ContainsKey) != 1
                ? "a Blueprint takes an x:Name, an x:Class or a Source"
            : children.Any(child => child.BBlueprintPartClass is not null) ? "only the root takes an x:Class"
            : source is not null && children.Count > 0 ? "a Blueprint with a Source holds no parts"
            : element == "Split" && (rows is null) == (columns is null) ? "a Split takes Rows or Columns"
            : element == "Frame" && glyphs is null ? "a Frame takes Glyphs"
            : marks is not null && (values.Count != allowed.Length || marks.Values.Any(set => set.Count == 0))
                ? "a Glyphs takes a Key and every mark"
            : marks is not null && (marks["Frame"].Count != 6 || marks["Rule"].Count != 3)
                ? $"the Glyphs '{key}' take six frame and three rule marks"
            : element != "Blueprint" && children.Any(child => child.BBlueprintPartElement == "Glyphs")
                ? "only a Blueprint lists glyphs"
            : margin.Length != 4 || margin.Min() < 0 ? $"the Margin '{inset}' is not one, two or four cell counts"
            : element != "Blueprint" && children.Any(child => child.BBlueprintPartSource is not null)
                ? "only a Blueprint lists blueprints"
            : parts > room ? $"'{element}' holds more parts than it has room for"
            : null;
        if (fault is not null)
        {
            throw new InvalidDataException($"{site}: {fault}.");
        }

        return new BBlueprintPart
        {
            BBlueprintPartElement = element,
            BBlueprintPartName = name,
            BBlueprintPartClass = values.GetValueOrDefault("x:Class"),
            BBlueprintPartSource = source,
            BBlueprintPartTheme = values.GetValueOrDefault("Theme") ?? "text",
            BBlueprintPartGlyphs = glyphs,
            BBlueprintPartKey = key,
            BBlueprintPartMarks = marks,
            BBlueprintPartMargin = margin,
            BBlueprintPartTracks = tracks,
            BBlueprintPartAcross = columns is not null,
            BBlueprintPartChildren = children,
        };
    }

    internal static void BBlueprintPlace(BBlueprintPart part, int row, int column, int width, int height)
    {
        ArgumentNullException.ThrowIfNull(part);

        IReadOnlyList<int> margin = part.BBlueprintPartMargin;
        part.BBlueprintPartRow = row + margin[1];
        part.BBlueprintPartColumn = column + margin[0];
        part.BBlueprintPartWidth = Math.Max(width - margin[0] - margin[2], 0);
        part.BBlueprintPartHeight = Math.Max(height - margin[1] - margin[3], 0);
        (row, column, width, height) =
            (part.BBlueprintPartRow, part.BBlueprintPartColumn, part.BBlueprintPartWidth, part.BBlueprintPartHeight);
        int rule = 0;
        if (part.BBlueprintPartElement == "Frame")
        {
            (row, column, width, height) = (row + 1, column + 1, Math.Max(width - 2, 0), Math.Max(height - 2, 0));
            rule = 1;
        }

        IReadOnlyList<BBlueprintTrack> tracks = part.BBlueprintPartTracks;
        IReadOnlyList<BBlueprintPart> children = part.BBlueprintPartChildren;
        part.BBlueprintPartRules = [];
        if (tracks.Count == 0)
        {
            foreach (BBlueprintPart child in children)
            {
                BBlueprintPlace(child, row, column, width, height);
            }

            return;
        }

        bool across = part.BBlueprintPartAcross;
        int[] sizes = BBlueprintTrack.BBlueprintTrackDivide(
            tracks, (across ? width : height) - (rule * (tracks.Count - 1)));
        int at = across ? column : row;
        List<int> rules = [];
        for (int index = 0; index < sizes.Length; index++)
        {
            if (index < children.Count)
            {
                if (across)
                {
                    BBlueprintPlace(children[index], row, at, sizes[index], height);
                }
                else
                {
                    BBlueprintPlace(children[index], at, column, width, sizes[index]);
                }
            }

            at += sizes[index];
            if (rule == 1 && index < sizes.Length - 1)
            {
                rules.Add(at);
            }

            at += rule;
        }

        part.BBlueprintPartRules = rules;
    }

    internal static IEnumerable<BCanvasText> BBlueprintDraw(
        BBlueprintPart part, IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>> glyphs)
    {
        ArgumentNullException.ThrowIfNull(part);
        ArgumentNullException.ThrowIfNull(glyphs);

        (int row, int column, int width, int height) =
            (part.BBlueprintPartRow, part.BBlueprintPartColumn, part.BBlueprintPartWidth, part.BBlueprintPartHeight);
        if (part.BBlueprintPartElement == "Frame" && part.BBlueprintPartGlyphs is string set && width > 0 && height > 0)
        {
            IReadOnlyList<string> frame = glyphs[set]["Frame"];
            IReadOnlyList<string> divider = glyphs[set]["Rule"];
            string theme = part.BBlueprintPartTheme;
            int inner = Math.Max(width - 2, 0);
            string top = string.Concat(Enumerable.Repeat(frame[1], inner));
            string middle = string.Concat(Enumerable.Repeat(divider[1], inner));
            IReadOnlyList<int> rules = part.BBlueprintPartRules;
            yield return new BCanvasText(row, column, width, $"{frame[0]}{top}{frame[2]}", theme);
            for (int line = row + 1; line < row + height - 1; line++)
            {
                string body = rules.Contains(line)
                    ? $"{divider[0]}{middle}{divider[2]}"
                    : $"{frame[3]}{new string(' ', inner)}{frame[3]}";
                yield return new BCanvasText(line, column, width, body, theme);
            }

            yield return new BCanvasText(row + height - 1, column, width, $"{frame[4]}{top}{frame[5]}", theme);
        }

        foreach (BBlueprintPart child in part.BBlueprintPartChildren)
        {
            foreach (BCanvasText text in BBlueprintDraw(child, glyphs))
            {
                yield return text;
            }
        }
    }
}
