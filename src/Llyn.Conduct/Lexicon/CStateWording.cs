namespace Llyn.Conduct;

public sealed record CStateWording(
    string CStateWordingText, string? CStateWordingKey, bool CStateWordingMuted, string? CStateWordingHint)
{
    internal static CStateWording LStateWordingRead(CStateValue value, string? unset, string? hint = null)
    {
        if (value.CStateValueUncertain)
        {
            const string unknown = "Display.Unknown";
            return new CStateWording(value.CStateValueText, unknown, false, unknown);
        }

        bool muted = value.CStateValueShown is null;
        return new CStateWording(value.CStateValueText, muted ? unset : null, muted, hint);
    }
}
