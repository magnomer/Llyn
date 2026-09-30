namespace Llyn.Conduct;

public sealed record CStateWording(string CStateWordingText, string? CStateWordingKey, bool CStateWordingMuted)
{
    internal static CStateWording LStateWordingRead(CStateValue value, string? unset)
    {
        if (value.CStateValueUncertain)
        {
            return new CStateWording(value.CStateValueText, "Display.Unknown", false);
        }

        bool muted = value.CStateValueShown is null;
        return new CStateWording(value.CStateValueText, muted ? unset : null, muted);
    }
}
