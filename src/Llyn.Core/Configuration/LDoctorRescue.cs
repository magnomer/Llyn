namespace Llyn.Core;

public sealed record LDoctorRescue(
    bool LDoctorRescueDone,
    string? LDoctorRescueBackup,
    string? LDoctorRescueReason)
{
    public static LDoctorRescue LDoctorRescueHealthy { get; } = new(false, null, null);
}
