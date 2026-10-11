using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;
using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TPressProof(LPressPaper paper) : LPressProof
{
    internal bool TPressProofDisposed { get; private set; }

    internal TaskCompletionSource? TPressProofHold { get; set; }

    public int LPressProofCount => 3;

    public LPressPaper LPressProofRead(int page)
    {
        return paper;
    }

    public async Task<LPressImage> LPressProofDraw(int page, int resolution)
    {
        if (TPressProofHold is { } hold)
        {
            await hold.Task;
        }

        return new LPressImage(TPressProofCreate(
            Math.Max(1, (int)Math.Round(paper.LPressPaperWidth * resolution)),
            Math.Max(1, (int)Math.Round(paper.LPressPaperHeight * resolution))));
    }

    public void Dispose()
    {
        TPressProofDisposed = true;
    }

    private static byte[] TPressProofCreate(int across, int down)
    {
        byte[] header = new byte[13];
        BinaryPrimitives.WriteInt32BigEndian(header, across);
        BinaryPrimitives.WriteInt32BigEndian(header.AsSpan(4), down);
        header[8] = 1;
        using MemoryStream rows = new();
        using (ZLibStream zlib = new(rows, CompressionLevel.Fastest, leaveOpen: true))
        {
            byte[] row = new byte[1 + ((across + 7) / 8)];
            for (int line = 0; line < down; line++)
            {
                zlib.Write(row);
            }
        }

        using MemoryStream png = new();
        png.Write([137, 80, 78, 71, 13, 10, 26, 10]);
        TPressProofAdd(png, "IHDR", header);
        TPressProofAdd(png, "IDAT", rows.ToArray());
        TPressProofAdd(png, "IEND", []);
        return png.ToArray();
    }

    private static void TPressProofAdd(Stream png, string kind, byte[] data)
    {
        byte[] head = new byte[8];
        BinaryPrimitives.WriteInt32BigEndian(head, data.Length);
        Encoding.ASCII.GetBytes(kind, head.AsSpan(4));
        uint sum = uint.MaxValue;
        foreach (byte value in (byte[])[.. head[4..], .. data])
        {
            sum ^= value;
            for (int bit = 0; bit < 8; bit++)
            {
                sum = (sum & 1) != 0 ? (sum >> 1) ^ 0xEDB88320u : sum >> 1;
            }
        }

        byte[] tail = new byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(tail, ~sum);
        png.Write(head);
        png.Write(data);
        png.Write(tail);
    }
}
