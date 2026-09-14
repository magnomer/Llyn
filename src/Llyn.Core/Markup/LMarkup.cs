using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Llyn.Core;

public static class LMarkup
{
    internal const string LMarkupRoot = "llyn";

    internal const string LMarkupState = "state";

    internal const string LMarkupUnknown = "unknown";

    public const int LMarkupDepthCeiling = 64;

    public const int LMarkupOmissionCeiling = 1000;

    public static IReadOnlyList<LMarkupEntry> LMarkupParse(string text)
    {
        return LMarkupParse(text, out _);
    }

    public static IReadOnlyList<LMarkupEntry> LMarkupParse(string text, out IReadOnlyList<LMarkupOmission> omissions)
    {
        ArgumentNullException.ThrowIfNull(text);

        LMarkupReader.LMarkupDepthValidate(text);

        XDocument document;
        try
        {
            document = XDocument.Parse(text, LoadOptions.SetLineInfo);
        }
        catch (XmlException)
        {
            throw new LRefusal(LRefusal.LRefusalMarkup);
        }

        XElement? root = document.Root;
        if (root is null || root.Name.LocalName != LMarkupRoot)
        {
            throw new LRefusal(LRefusal.LRefusalMarkup);
        }

        List<LMarkupOmission> found = [];
        LMarkupReader.LMarkupAttributeScan(root, found);

        List<LMarkupEntry> entries = [];
        foreach (XElement child in root.Elements())
        {
            if (child.Name.LocalName == "entry")
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

    public static string LMarkupFormat(IReadOnlyList<LMarkupEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);

        XElement root = new(LMarkupRoot);
        foreach (LMarkupEntry entry in entries)
        {
            root.Add(LMarkupWriter.LMarkupEntryFormat(entry));
        }

        XmlWriterSettings settings = new()
        {
            Indent = true,
            IndentChars = "  ",
            NewLineChars = "\n",
            OmitXmlDeclaration = true,
            Encoding = new UTF8Encoding(false),
        };

        StringBuilder builder = new();
        using (XmlWriter writer = XmlWriter.Create(builder, settings))
        {
            root.Save(writer);
        }

        return builder.ToString();
    }

    internal static LStateValue LMarkupValueParse(XElement? element)
    {
        if (element is null)
        {
            return LStateValue.LStateValueUnspecified;
        }

        if (element.Attribute(LMarkupState)?.Value == LMarkupUnknown)
        {
            return LStateValue.LStateValueUnknown;
        }

        return LStateValue.LStateValueRead(element.Value);
    }

    internal static void LMarkupValueFormat(XElement parent, string name, LStateValue value)
    {
        switch (value.LStateValueState)
        {
            case LState.LStateUnknown:
                parent.Add(new XElement(name, new XAttribute(LMarkupState, LMarkupUnknown)));
                break;
            case LState.LStateSpecified:
                parent.Add(new XElement(name, LMarkupTextNormalize(value.LStateValueText ?? string.Empty)));
                break;
            default:
                break;
        }
    }

    internal static string LMarkupTextParse(XElement? element)
    {
        return element?.Value ?? string.Empty;
    }

    internal static void LMarkupTextFormat(XElement parent, string name, string? text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            parent.Add(new XElement(name, LMarkupTextNormalize(text)));
        }
    }

    internal static string LMarkupTextNormalize(string text)
    {
        StringBuilder? builder = null;
        for (int index = 0; index < text.Length; index++)
        {
            char current = text[index];
            bool pair = char.IsHighSurrogate(current)
                && index + 1 < text.Length
                && XmlConvert.IsXmlSurrogatePair(text[index + 1], current);
            if (pair)
            {
                builder?.Append(current).Append(text[index + 1]);
                index++;
                continue;
            }

            if (XmlConvert.IsXmlChar(current))
            {
                builder?.Append(current);
                continue;
            }

            builder ??= new StringBuilder(text, 0, index, text.Length);
        }

        return builder?.ToString() ?? text;
    }
}
