using System;
using System.Collections.Generic;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

internal sealed class CClip
{
    private readonly List<CClipItem> _cClipRows = [];

    private bool _cClipSearching;

    private CRecording? _cClipPreview;

    internal void LClipStart()
    {
        _cClipRows.Clear();
        _cClipSearching = true;
        _cClipPreview = null;
    }

    internal void LClipFinish()
    {
        _cClipSearching = false;
    }

    internal CClipRoll LClipRead()
    {
        return new CClipRoll(
            [.. _cClipRows],
            _cClipRows.Count == 0,
            _cClipSearching,
            _cClipSearching ? "Downloader.Searching" : "Downloader.Empty");
    }

    internal int LClipPlace(string source, int order)
    {
        int position = 0;
        while (position < _cClipRows.Count && _cClipRows[position].CClipItemOrder < order)
        {
            position++;
        }

        if (position < _cClipRows.Count && _cClipRows[position].CClipItemOrder == order)
        {
            return position;
        }

        _cClipRows.Insert(position, new CClipItem(source, order, [], "Downloader.Searching", false));
        return position;
    }

    internal void LClipRecordingAdd(CRecording recording, LForay? foray, LTenure? tenure)
    {
        int position = LClipPlace(recording.CRecordingSource, recording.CRecordingOrder);
        CClipItem row = _cClipRows[position];
        if (recording.CRecordingAddressed)
        {
            _cClipRows[position] = row with
            {
                CClipItemReading = [.. row.CClipItemReading, LClipReadingRead(foray, tenure, recording)],
                CClipItemNotice = string.Empty,
                CClipItemReady = true,
            };
            return;
        }

        if (row.CClipItemReading.Count > 0)
        {
            return;
        }

        _cClipRows[position] = row with
        {
            CClipItemNotice = recording.CRecordingReached ? "Downloader.Missing" : "Downloader.Broken",
            CClipItemReady = false,
        };
    }

    private static CClipReading LClipReadingRead(LForay? foray, LTenure? tenure, CRecording recording)
    {
        return new CClipReading(
            recording,
            CSounding.CSoundingVarietyRead(
                foray?.LForayLanguage ?? tenure?.LTenureLanguageRead() ?? string.Empty, recording.CRecordingVariety),
            foray?.LForayFlagged ?? tenure?.LTenureFlaggedCheck() ?? false,
            "Downloader.Use",
            true,
            false,
            false,
            false);
    }

    internal void LClipPreviewStart(CRecording recording)
    {
        LClipPreviewFinish();
        _cClipPreview = recording;
        LClipReadingSet(recording, static reading => reading with
        {
            CClipReadingRefused = false,
            CClipReadingFetching = true,
        });
    }

    internal bool LClipPreviewPlay(CRecording recording)
    {
        if (!recording.Equals(_cClipPreview))
        {
            return false;
        }

        LClipReadingSet(recording, static reading => reading with
        {
            CClipReadingFetching = false,
            CClipReadingPlaying = true,
        });
        return true;
    }

    internal void LClipRefusedSet(CRecording recording)
    {
        LClipReadingSet(recording, static reading => reading with
        {
            CClipReadingFetching = false,
            CClipReadingRefused = true,
        });
    }

    internal bool LClipPreviewFinish()
    {
        if (_cClipPreview is not CRecording previewed)
        {
            return false;
        }

        _cClipPreview = null;
        LClipReadingSet(previewed, static reading => reading with
        {
            CClipReadingFetching = false,
            CClipReadingPlaying = false,
        });
        return true;
    }

    internal bool LClipSaveStart(CRecording recording)
    {
        if (LClipReadingFind(recording) is not { CClipReadingReady: true })
        {
            return false;
        }

        LClipReadingSet(recording, static reading => reading with
        {
            CClipReadingAction = "Downloader.Saving",
            CClipReadingReady = false,
        });
        return true;
    }

    internal void LClipSaveFinish(CRecording recording, bool saved)
    {
        LClipReadingSet(recording, reading => reading with
        {
            CClipReadingAction = saved ? "Downloader.Saved" : "Downloader.Retry",
            CClipReadingReady = !saved,
        });
    }

    private CClipReading? LClipReadingFind(CRecording recording)
    {
        foreach (CClipItem row in _cClipRows)
        {
            foreach (CClipReading reading in row.CClipItemReading)
            {
                if (reading.CClipReadingRecording.Equals(recording))
                {
                    return reading;
                }
            }
        }

        return null;
    }

    private void LClipReadingSet(CRecording recording, Func<CClipReading, CClipReading> change)
    {
        for (int position = 0; position < _cClipRows.Count; position++)
        {
            CClipItem row = _cClipRows[position];
            List<CClipReading> readings = [.. row.CClipItemReading];
            int found = readings.FindIndex(reading => reading.CClipReadingRecording.Equals(recording));
            if (found < 0)
            {
                continue;
            }

            readings[found] = change(readings[found]);
            _cClipRows[position] = row with { CClipItemReading = readings };
            return;
        }
    }
}
