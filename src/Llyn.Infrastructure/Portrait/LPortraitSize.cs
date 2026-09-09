using System;

namespace Llyn.Infrastructure;

public static class LPortraitSize
{
    public static (int LPortraitWidth, int LPortraitHeight) LPortraitSizeRead(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (data.Length > 24
            && data[0] == 0x89 && data[1] == 0x50 && data[2] == 0x4E && data[3] == 0x47)
        {
            return (LPortraitSizeRead(data, 16, true), LPortraitSizeRead(data, 20, true));
        }

        if (data.Length > 10 && data[0] == 0x47 && data[1] == 0x49 && data[2] == 0x46)
        {
            return (data[6] | (data[7] << 8), data[8] | (data[9] << 8));
        }

        if (data.Length > 26 && data[0] == 0x42 && data[1] == 0x4D)
        {
            return (LPortraitSizeRead(data, 18, false), Math.Abs(LPortraitSizeRead(data, 22, false)));
        }

        if (data.Length > 4 && data[0] == 0xFF && data[1] == 0xD8)
        {
            return LPortraitSizeScan(data);
        }

        return (640, 360);
    }

    private static (int LPortraitWidth, int LPortraitHeight) LPortraitSizeScan(byte[] data)
    {
        int place = 2;

        while (place + 9 < data.Length)
        {
            if (data[place] != 0xFF)
            {
                place++;
                continue;
            }

            int mark = data[place + 1];
            if (mark is 0xD8 or 0x01 or (>= 0xD0 and <= 0xD7))
            {
                place += 2;
                continue;
            }

            int span = (data[place + 2] << 8) | data[place + 3];
            if (span < 2)
            {
                break;
            }

            bool frame = mark is (>= 0xC0 and <= 0xC3)
                or (>= 0xC5 and <= 0xC7)
                or (>= 0xC9 and <= 0xCB)
                or (>= 0xCD and <= 0xCF);

            if (frame && place + 9 < data.Length)
            {
                int height = (data[place + 5] << 8) | data[place + 6];
                int width = (data[place + 7] << 8) | data[place + 8];
                return (width, height);
            }

            place += 2 + span;
        }

        return (640, 360);
    }

    private static int LPortraitSizeRead(byte[] data, int place, bool big)
    {
        return big
            ? (data[place] << 24) | (data[place + 1] << 16)
                | (data[place + 2] << 8) | data[place + 3]
            : data[place] | (data[place + 1] << 8)
                | (data[place + 2] << 16) | (data[place + 3] << 24);
    }
}
