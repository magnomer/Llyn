using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LEasel TEaselCreate(this LTenure tenure) => new(tenure);

    internal static void TEaselImageAdd(
        this LEasel easel, long card, LCardKind kind = LCardKind.LCardKindMeaning)
    {
        easel.LEaselImageAdd(kind, card);
    }

    internal static void TEaselImageRemove(
        this LEasel easel, long image, long card = 0, LCardKind kind = LCardKind.LCardKindMeaning)
    {
        easel.LEaselImageRemove(kind, card, image);
    }

    internal static void TEaselImageSet(this LEasel easel, long image, string location, bool deferred)
    {
        easel.LEaselImageSet(image, location, deferred);
    }

    internal static void TEaselVideoAdd(
        this LEasel easel, long card, LCardKind kind = LCardKind.LCardKindMeaning)
    {
        easel.LEaselVideoAdd(kind, card);
    }

    internal static void TEaselVideoRemove(
        this LEasel easel, long video, long card = 0, LCardKind kind = LCardKind.LCardKindMeaning)
    {
        easel.LEaselVideoRemove(kind, card, video);
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
