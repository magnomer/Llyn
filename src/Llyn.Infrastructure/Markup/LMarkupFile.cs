using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LMarkupFile : LMarkupVault
{
    public const long LMarkupFileCeiling = 64L * 1024 * 1024;

    public LMarkupNode LMarkupRead(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        using FileStream stream = new(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        if (stream.Length > LMarkupFileCeiling)
        {
            throw new LRefusal(LRefusal.LRefusalMarkup);
        }

        using StreamReader reader = new(stream, Encoding.UTF8, true);
        return LMarkupFileParse(reader.ReadToEnd());
    }

    public void LMarkupSave(string path, LMarkupNode root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(root);

        File.WriteAllText(path, LMarkupFileFormat(root), new UTF8Encoding(false));
    }

    public static LMarkupNode LMarkupFileParse(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        LMarkupFileValidate(text);

        XDocument document;
        try
        {
            document = XDocument.Parse(text, LoadOptions.SetLineInfo);
        }
        catch (XmlException)
        {
            throw new LRefusal(LRefusal.LRefusalMarkup);
        }

        XElement root = document.Root ?? throw new LRefusal(LRefusal.LRefusalMarkup);
        return LMarkupFileRead(root);
    }

    public static string LMarkupFileFormat(LMarkupNode root)
    {
        ArgumentNullException.ThrowIfNull(root);

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
            LMarkupFileCreate(root).Save(writer);
        }

        return builder.ToString();
    }

    private static void LMarkupFileValidate(string text)
    {
        XmlReaderSettings settings = new()
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
        };

        try
        {
            using XmlReader reader = XmlReader.Create(new StringReader(text), settings);
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Depth >= LMarkup.LMarkupDepthCeiling)
                {
                    throw new LRefusal(LRefusal.LRefusalMarkup);
                }
            }
        }
        catch (XmlException)
        {
            throw new LRefusal(LRefusal.LRefusalMarkup);
        }
    }

    private static LMarkupNode LMarkupFileRead(XElement element)
    {
        Dictionary<string, string> attributes = [];
        foreach (XAttribute attribute in element.Attributes())
        {
            attributes.TryAdd(attribute.Name.LocalName, attribute.Value);
        }

        List<LMarkupNode> children = [];
        foreach (XElement child in element.Elements())
        {
            children.Add(LMarkupFileRead(child));
        }

        IXmlLineInfo info = element;
        return new LMarkupNode(
            element.Name.LocalName,
            attributes,
            children,
            element.Value,
            info.HasLineInfo() ? info.LineNumber : 0);
    }

    private static XElement LMarkupFileCreate(LMarkupNode node)
    {
        XElement element = new(node.LMarkupNodeName);
        foreach ((string name, string value) in node.LMarkupNodeAttribute)
        {
            element.SetAttributeValue(name, value);
        }

        foreach (LMarkupNode child in node.LMarkupNodeChild)
        {
            element.Add(LMarkupFileCreate(child));
        }

        if (node.LMarkupNodeChild.Count == 0 && node.LMarkupNodeText.Length > 0)
        {
            element.Add(LMarkupFileNormalize(node.LMarkupNodeText));
        }

        return element;
    }

    private static string LMarkupFileNormalize(string text)
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
