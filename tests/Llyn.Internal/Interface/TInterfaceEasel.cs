using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LEasel TEaselCreate(this LTenure tenure) => new(tenure);

    internal static void TEaselImageAdd(this LEasel easel, int position)
    {
        easel.LEaselImageAdd(0, position);
    }

    internal static void TEaselImageRemove(this LEasel easel, long image)
    {
        easel.LEaselImageRemove(0, image);
    }

    internal static void TEaselImageSet(this LEasel easel, long image, string location, bool deferred)
    {
        easel.LEaselImageSet(image, location, deferred);
    }

    internal static void TEaselVideoAdd(this LEasel easel, int position)
    {
        easel.LEaselVideoAdd(0, position);
    }

    internal static void TEaselVideoRemove(this LEasel easel, long video)
    {
        easel.LEaselVideoRemove(0, video);
    }

    internal static void TEaselVideoSet(this LEasel easel, long video, string location, bool deferred)
    {
        easel.LEaselVideoSet(video, location, deferred);
    }

    internal static void TEaselSpanSet(this LEasel easel, long video, string span)
    {
        easel.LEaselSpanSet(video, span);
    }
}
