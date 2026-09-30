namespace Llyn.Conduct;

public sealed record CStateWording(string CStateWordingText, string? CStateWordingKey)
{
    internal static CStateWording LStateWordingRead(CStateValue value, string unset)
    {
        if (value.CStateValueUncertain)
        {
            return new CStateWording(value.CStateValueText, "Display.Unknown");
        }

        return new CStateWording(value.CStateValueText, value.CStateValueShown is null ? unset : null);
    }
}
