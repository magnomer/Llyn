using Llyn.Conduct;
using Xunit;

namespace Llyn.Tests;

public sealed class TStudy
{
    [Fact]
    public void StudyEmptyKey_Placeholder_ReturnsStudyEmpty()
    {
        CStudy study = new();

        Assert.Equal("Study.Empty", study.CStudyEmptyKey);
    }
}
