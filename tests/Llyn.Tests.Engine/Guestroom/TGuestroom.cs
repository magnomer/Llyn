using Xunit;

namespace Llyn.Tests;

public sealed class TGuestroom
{
    [Theory]
    [InlineData(@"C:\Program Files\Llyn\Llyn.com")]
    [InlineData(@"C:\Program Files\Llyn\LLYN.COM")]
    public void GuestroomRun_ConsoleLauncher_ReturnsConsoleCode(string path)
    {
        int code = TInterfaceGuestroom.TGuestroomRun(path, () => 3, () => 7);

        Assert.Equal(3, code);
    }

    [Fact]
    public void GuestroomRun_WindowLauncher_ReturnsWindowCode()
    {
        int code = TInterfaceGuestroom.TGuestroomRun(@"C:\Program Files\Llyn\Llyn.exe", () => 3, () => 7);

        Assert.Equal(7, code);
    }

    [Fact]
    public void GuestroomRun_NoPath_ReturnsWindowCode()
    {
        int code = TInterfaceGuestroom.TGuestroomRun(null, () => 3, () => 7);

        Assert.Equal(7, code);
    }
}
