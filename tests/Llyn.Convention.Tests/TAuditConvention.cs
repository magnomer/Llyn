using Xunit;

namespace Convention.Tests;

public sealed class TAuditConvention
{
    public const int TAuditGeneration = 8;

    [Fact]
    public void AuditConvention_SidecarGenerations_MatchTheTooling()
    {
        (string TSidecarName, int TSidecarGeneration)[] sidecars =
        [
            (nameof(TAuditNameSetting), TAuditNameSetting.TAuditGeneration),
            (nameof(TAuditLineSetting), TAuditLineSetting.TAuditGeneration),
            (nameof(TAuditCommentSetting), TAuditCommentSetting.TAuditGeneration),
        ];

        string[] stale = sidecars
            .Where(sidecar => sidecar.TSidecarGeneration != TAuditGeneration)
            .Select(sidecar => $"  {sidecar.TSidecarName} is generation {sidecar.TSidecarGeneration}")
            .ToArray();

        Assert.True(stale.Length == 0,
            $"This tooling is generation {TAuditGeneration}, but {stale.Length} sidecar(s) differ. Rerun the audit that generates each.\n"
            + string.Join('\n', stale));
    }
}
