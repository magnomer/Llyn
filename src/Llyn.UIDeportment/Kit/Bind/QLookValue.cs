using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Llyn.UIDeportment;

internal abstract record QLookValue
{
    public static implicit operator QLookValue(string key) => new QLookKey(key);

    public static implicit operator QLookValue(DependencyProperty source) => new QLookRelay(source);

    public static implicit operator QLookValue(AnimationTimeline timeline) => new QLookMotion(timeline);

    public static implicit operator QLookValue(Freezable setting) => new QLookFix<Freezable>(setting);

    public static implicit operator QLookValue(double setting) => new QLookFix<double>(setting);

    public static implicit operator QLookValue(bool setting) => new QLookFix<bool>(setting);

    public static implicit operator QLookValue(Visibility setting) => new QLookFix<Visibility>(setting);

    public static implicit operator QLookValue(HorizontalAlignment setting) =>
        new QLookFix<HorizontalAlignment>(setting);

    public static implicit operator QLookValue(FontWeight setting) => new QLookFix<FontWeight>(setting);

    public static implicit operator QLookValue(Point setting) => new QLookFix<Point>(setting);

    public static implicit operator QLookValue(Cursor setting) => new QLookFix<Cursor>(setting);

    public static implicit operator QLookValue(RoutedCommand setting) => new QLookFix<RoutedCommand>(setting);

    internal abstract void QLookValueApply(
        FrameworkElement element, FrameworkElement target, DependencyProperty property);

    internal virtual void QLookValueClear(FrameworkElement target, DependencyProperty property, object saved)
    {
        if (saved is BindingExpressionBase expression)
        {
            BindingOperations.SetBinding(target, property, expression.ParentBindingBase);
            return;
        }

        if (saved == DependencyProperty.UnsetValue)
        {
            target.ClearValue(property);
            return;
        }

        target.SetValue(property, saved);
    }

    internal sealed record QLookKey(string QLookKeyName) : QLookValue
    {
        internal override void QLookValueApply(
            FrameworkElement element, FrameworkElement target, DependencyProperty property)
        {
            target.SetResourceReference(property, QLookKeyName);
        }
    }

    internal sealed record QLookRelay(DependencyProperty QLookRelaySource) : QLookValue
    {
        internal override void QLookValueApply(
            FrameworkElement element, FrameworkElement target, DependencyProperty property)
        {
            target.SetBinding(property, new Binding { Source = element, Path = new PropertyPath(QLookRelaySource) });
        }
    }

    internal sealed record QLookMotion(AnimationTimeline QLookMotionTimeline) : QLookValue
    {
        internal override void QLookValueApply(
            FrameworkElement element, FrameworkElement target, DependencyProperty property)
        {
            if (target.RenderTransform.HasAnimatedProperties)
            {
                return;
            }

            Transform shift = target.RenderTransform.CloneCurrentValue();
            target.RenderTransform = shift;
            shift.BeginAnimation(property, QLookMotionTimeline);
        }

        internal override void QLookValueClear(FrameworkElement target, DependencyProperty property, object saved)
        {
            target.RenderTransform.BeginAnimation(property, null);
        }
    }

    internal sealed record QLookFix<QLookFixValue>(QLookFixValue QLookFixSetting) : QLookValue
    {
        internal override void QLookValueApply(
            FrameworkElement element, FrameworkElement target, DependencyProperty property)
        {
            target.SetValue(property, QLookFixSetting);
        }
    }
}
