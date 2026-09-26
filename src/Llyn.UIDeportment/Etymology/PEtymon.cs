using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal sealed class PEtymon : DependencyObject
{
    public static readonly DependencyProperty PEtymonTextProperty = DependencyProperty.Register(
        nameof(PEtymonText),
        typeof(string),
        typeof(PEtymon),
        new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty PEtymonShownProperty = DependencyProperty.Register(
        nameof(PEtymonShown),
        typeof(bool),
        typeof(PEtymon),
        new PropertyMetadata(false));

    internal PEtymon()
    {
        PEtymonHeadword = string.Empty;
        PEtymonLanguage = string.Empty;
        PEtymonCaret = true;
    }

    private PEtymon(long id, string headword, string language)
    {
        PEtymonId = id;
        PEtymonHeadword = headword;
        PEtymonLanguage = language;
        PEtymonFlag = LEnsignImage.LEnsignFind(language);
    }

    public long PEtymonId { get; }

    public string PEtymonHeadword { get; }

    public string PEtymonLanguage { get; }

    public ImageSource? PEtymonFlag { get; }

    public bool PEtymonCaret { get; }

    public string PEtymonText
    {
        get => (string)GetValue(PEtymonTextProperty);
        set => SetValue(PEtymonTextProperty, value);
    }

    public bool PEtymonShown
    {
        get => (bool)GetValue(PEtymonShownProperty);
        set => SetValue(PEtymonShownProperty, value);
    }

    internal static IReadOnlyList<PEtymon> PEtymonBuild(IReadOnlyList<LTranslationTarget> targets, PEtymon caret)
    {
        List<PEtymon> items = [];
        foreach (LTranslationTarget target in targets)
        {
            items.Add(new PEtymon(
                target.LTranslationTargetId, target.LTranslationTargetHeadword, target.LTranslationTargetLanguage));
        }

        items.Add(caret);
        return items;
    }
}
