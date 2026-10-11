using System;
using System.IO;
using System.Threading.Tasks;
using Llyn.Core;
using Windows.Data.Pdf;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace Llyn.Core.Windows;

internal sealed class LPressDocument : LPressProof
{
    private readonly PdfDocument _lPressDocumentPdf;

    private readonly Stream _lPressDocumentStream;

    private LPressDocument(PdfDocument pdf, Stream stream)
    {
        _lPressDocumentPdf = pdf;
        _lPressDocumentStream = stream;
    }

    public int LPressProofCount => (int)_lPressDocumentPdf.PageCount;

    internal static async Task<LPressDocument> LPressDocumentOpen(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        MemoryStream buffer = new();

        try
        {
            using (stream)
            {
                await stream.CopyToAsync(buffer);
            }

            buffer.Position = 0;
            PdfDocument pdf = await PdfDocument.LoadFromStreamAsync(buffer.AsRandomAccessStream());
            return new LPressDocument(pdf, buffer);
        }
        catch (Exception)
        {
            buffer.Dispose();
            throw;
        }
    }

    public LPressPaper LPressProofRead(int page)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(page);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(page, LPressProofCount);

        using PdfPage sheet = _lPressDocumentPdf.GetPage((uint)page);
        Size size = sheet.Size;
        return new LPressPaper(size.Width / 96.0, size.Height / 96.0);
    }

    public async Task<LPressImage> LPressProofDraw(int page, int resolution)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(page);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(page, LPressProofCount);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(resolution);

        using PdfPage sheet = _lPressDocumentPdf.GetPage((uint)page);
        Size size = sheet.Size;
        double scale = resolution / 96.0;

        PdfPageRenderOptions options = new()
        {
            DestinationWidth = (uint)Math.Max(1.0, Math.Round(size.Width * scale)),
            DestinationHeight = (uint)Math.Max(1.0, Math.Round(size.Height * scale)),
            BitmapEncoderId = BitmapEncoder.PngEncoderId,
        };

        using InMemoryRandomAccessStream image = new();
        await sheet.RenderToStreamAsync(image, options).AsTask().ConfigureAwait(false);

        image.Seek(0);
        using Stream read = image.AsStreamForRead();
        using MemoryStream copy = new();
        await read.CopyToAsync(copy).ConfigureAwait(false);

        return new LPressImage(copy.ToArray());
    }

    public void Dispose()
    {
        _lPressDocumentStream.Dispose();
    }
}
