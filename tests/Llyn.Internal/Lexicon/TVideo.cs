using Xunit;

namespace Llyn.Tests;

public sealed class TVideo
{
    [Theory]
    [InlineData("https://youtu.be/dQw4w9WgXcQ")]
    [InlineData("https://www.youtube.com/watch?v=dQw4w9WgXcQ&t=30")]
    [InlineData("https://youtube.com/embed/dQw4w9WgXcQ")]
    [InlineData("https://www.youtube.com/shorts/dQw4w9WgXcQ/")]
    [InlineData("https://www.youtube-nocookie.com/embed/dQw4w9WgXcQ")]
    [InlineData("https://YouTube.com/live/dQw4w9WgXcQ")]
    public void VideoFilmRead_HostedShape_AnswersTheFilmId(string address)
    {
        Assert.Equal("dQw4w9WgXcQ", TInterface.TVideoFilmRead(new Uri(address)));
    }

    [Theory]
    [InlineData("https://example.com/watch?v=dQw4w9WgXcQ")]
    [InlineData("https://youtu.be/exemplarvideo")]
    [InlineData("https://www.youtube.com/watch?v=dQw4w9WgX.Q")]
    [InlineData("https://www.youtube.com/channel/dQw4w9WgXcQ")]
    [InlineData("file:///C:/media/kindling.mp4")]
    public void VideoFilmRead_OtherAddress_AnswersNothing(string address)
    {
        Assert.Null(TInterface.TVideoFilmRead(new Uri(address)));
    }

    [Theory]
    [InlineData("00:10-00:40", 10, 40)]
    [InlineData(" 1:02:03 - 1:02:09 ", 3723, 3729)]
    [InlineData("0:05", 5, null)]
    [InlineData("0:05 - soon", 5, null)]
    [InlineData("later - 0:40", 0, 40)]
    [InlineData("5 - 0:40", 0, 40)]
    [InlineData("0:05:07:09", 0, null)]
    [InlineData("-0:05", 5, null)]
    [InlineData("", 0, null)]
    public void VideoSpanRead_WrittenSpan_AnswersItsMomentsOrTheWholeFilm(string span, int from, int? until)
    {
        (TimeSpan start, TimeSpan? end) = TInterface.TVideoSpanRead(span);

        Assert.Equal(TimeSpan.FromSeconds(from), start);
        Assert.Equal(until is int seconds ? TimeSpan.FromSeconds(seconds) : null, end);
    }
}
