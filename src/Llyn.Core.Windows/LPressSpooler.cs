using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Core.Windows;

public sealed class LPressSpooler : LPressQueue
{
    public Task<IReadOnlyList<LPressPrinter>> LPressQueueRead()
    {
        TaskCompletionSource<IReadOnlyList<LPressPrinter>> done =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        Thread worker = new(() =>
        {
            string? marked = null;

            try
            {
                using PrintQueue chosen = LocalPrintServer.GetDefaultPrintQueue();
                marked = chosen.FullName;
            }
            catch (PrintSystemException)
            {
                marked = null;
            }

            try
            {
                List<LPressPrinter> printers = new();

                using LocalPrintServer server = new();
                using PrintQueueCollection queues = server.GetPrintQueues(new[]
                {
                    EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections,
                });

                foreach (PrintQueue queue in queues)
                {
                    using (queue)
                    {
                        bool chosen = string.Equals(queue.FullName, marked, StringComparison.OrdinalIgnoreCase);
                        printers.Add(LPressSpoolerScan(queue, chosen));
                    }
                }

                done.SetResult(printers);
            }
            catch (Exception exception)
            {
                done.SetException(exception);
            }
        });

        worker.IsBackground = true;
        worker.SetApartmentState(ApartmentState.STA);
        worker.Start();
        return done.Task;
    }

    private static LPressPrinter LPressSpoolerScan(PrintQueue queue, bool marked)
    {
        string name = queue.FullName;
        PrintCapabilities capability;

        try
        {
            capability = queue.GetPrintCapabilities();
        }
        catch (PrintQueueException)
        {
            return new LPressPrinter(
                name,
                marked,
                Array.Empty<LPressPaper>(),
                Array.Empty<LPressSide>(),
                Array.Empty<LPressColor>(),
                false,
                null,
                false);
        }

        List<LPressPaper> papers = new();

        foreach (PageMediaSize size in capability.PageMediaSizeCapability)
        {
            if (size.Width is not double width || size.Height is not double height || width <= 0.0 || height <= 0.0)
            {
                continue;
            }

            papers.Add(new LPressPaper(width / 96.0, height / 96.0));
        }

        LPressSide[] sides = capability.DuplexingCapability
            .Select(duplexing => duplexing switch
            {
                Duplexing.OneSided => LPressSide.LPressSideSingle,
                Duplexing.TwoSidedLongEdge => LPressSide.LPressSideLong,
                Duplexing.TwoSidedShortEdge => LPressSide.LPressSideShort,
                _ => LPressSide.LPressSideDefault,
            })
            .Where(side => side != LPressSide.LPressSideDefault)
            .Distinct()
            .ToArray();

        LPressColor[] colors = capability.OutputColorCapability
            .Select(color => color switch
            {
                OutputColor.Color => LPressColor.LPressColorFull,
                OutputColor.Grayscale or OutputColor.Monochrome => LPressColor.LPressColorGray,
                _ => LPressColor.LPressColorDefault,
            })
            .Where(color => color != LPressColor.LPressColorDefault)
            .Distinct()
            .ToArray();

        return new LPressPrinter(
            name,
            marked,
            papers,
            sides,
            colors,
            capability.CollationCapability.Contains(Collation.Collated),
            capability.MaxCopyCount,
            true);
    }
}
