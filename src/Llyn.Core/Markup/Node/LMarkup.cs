using System;
using System.Collections.Generic;

namespace Llyn.Core;

public static class LMarkup
{
    internal const string LMarkupRoot = "llyn";

    internal const string LMarkupState = "state";

    internal const string LMarkupUnknown = "unknown";

    public const int LMarkupDepthCeiling = 64;

    public const int LMarkupOmissionCeiling = 1000;

    private static readonly IReadOnlyDictionary<string, string> LMarkupUnknownMark =
        new Dictionary<string, string> { [LMarkupState] = LMarkupUnknown };

    public static IReadOnlyList<LMarkupEntry> LMarkupParse(
        LMarkupNode root, out IReadOnlyList<LMarkupOmission> omissions)
    {
        ArgumentNullException.ThrowIfNull(root);

        if (root.LMarkupNodeName != LMarkupRoot)
        {
            throw new LRefusal(LRefusal.LRefusalMarkup);
        }

        List<LMarkupOmission> found = [];
        LMarkupReader.LMarkupAttributeScan(root, found);

        List<LMarkupEntry> entries = [];
        foreach (LMarkupNode child in root.LMarkupNodeChild)
        {
            if (child.LMarkupNodeName == "entry")
            {
                entries.Add(LMarkupReader.LMarkupEntryParse(child, found));
            }
            else
            {
                LMarkupReader.LMarkupOmissionAdd(found, child);
            }
        }

        omissions = found;
        return entries;
    }

    public static LMarkupNode LMarkupFormat(IReadOnlyList<LMarkupEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);

        List<LMarkupNode> children = new(entries.Count);
        foreach (LMarkupEntry entry in entries)
        {
            children.Add(LMarkupWriter.LMarkupEntryFormat(entry));
        }

        return LMarkupNode.LMarkupNodeCreate(LMarkupRoot, children);
    }

    internal static LStateValue LMarkupValueParse(LMarkupNode? element)
    {
        if (element is null)
        {
            return LStateValue.LStateValueUnspecified;
        }

        if (element.LMarkupNodeAttribute.TryGetValue(LMarkupState, out string? state) && state == LMarkupUnknown)
        {
            return LStateValue.LStateValueUnknown;
        }

        return LStateValue.LStateValueRead(element.LMarkupNodeText);
    }

    internal static void LMarkupValueFormat(List<LMarkupNode> parent, string name, LStateValue value)
    {
        switch (value.LStateValueState)
        {
            case LState.LStateUnknown:
                parent.Add(new LMarkupNode(name, LMarkupUnknownMark, [], string.Empty, 0));
                break;
            case LState.LStateSpecified:
                parent.Add(LMarkupNode.LMarkupNodeCreate(name, value.LStateValueText ?? string.Empty));
                break;
            default:
                break;
        }
    }

    internal static string LMarkupTextParse(LMarkupNode? element)
    {
        return element?.LMarkupNodeText ?? string.Empty;
    }

    internal static void LMarkupTextFormat(List<LMarkupNode> parent, string name, string? text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            parent.Add(LMarkupNode.LMarkupNodeCreate(name, text));
        }
    }
}
