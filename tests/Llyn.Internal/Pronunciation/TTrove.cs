using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TTrove
{
    [Fact]
    public void TroveCandidateSave_SeveralPerOrder_ReadsAllBack()
    {
        LTrove trove = TInterface.TTroveCreate();
        LCandidate[] found =
        [
            TInterface.TCandidateCreate("Cambridge", "təˈmɑːtəʊ", 0, true, "British"),
            TInterface.TCandidateCreate("Cambridge", "təˈmeɪtoʊ", 0, true, "American"),
            TInterface.TCandidateCreate("Longman", null, 1, true, string.Empty),
        ];

        trove.TTroveCandidateSave(7, "tomato", "English", found);

        Assert.Equal(found, trove.TTroveCandidateRead(7, "tomato", "English"));
        Assert.Null(trove.TTroveCandidateRead(7, "potato", "English"));
    }

    [Fact]
    public void TroveCandidateSave_NoPhonetic_KeepsNothing()
    {
        LTrove trove = TInterface.TTroveCreate();

        trove.TTroveCandidateSave(
            7, "tomato", "English", [TInterface.TCandidateCreate("Longman", null, 0, false, string.Empty)]);

        Assert.Null(trove.TTroveCandidateRead(7, "tomato", "English"));
    }

    [Fact]
    public void TroveRecordingRead_SeveralPerOrder_ReadsAllBack()
    {
        LTrove trove = TInterface.TTroveCreate();
        LRecording[] found =
        [
            TInterface.TRecordingCreate("Oxford", "https://example.test/tomato-gb.mp3", 0, true, "British"),
            TInterface.TRecordingCreate("Oxford", "https://example.test/tomato-us.mp3", 0, true, "American"),
        ];

        trove.TTroveRecordingSave(7, "tomato", "English", found);

        Assert.Equal(found, trove.TTroveRecordingRead(7, "tomato", "English"));
        Assert.Null(trove.TTroveRecordingRead(7, "potato", "English"));
    }

    [Fact]
    public void TroveTranscriptionRead_TwoSchemes_ReadsEachBack()
    {
        LTrove trove = TInterface.TTroveCreate();
        LCandidate[] pinyin = [TInterface.TCandidateCreate("Wiktionary", "nǐ hǎo", 0, true, string.Empty)];
        LCandidate[] bopomofo = [TInterface.TCandidateCreate("Wiktionary", "ㄋㄧˇ ㄏㄠˇ", 0, true, string.Empty)];

        trove.TTroveTranscriptionSave(7, "你好", "Mandarin", "Pinyin", pinyin);
        trove.TTroveTranscriptionSave(7, "你好", "Mandarin", "Bopomofo", bopomofo);

        Assert.Equal(pinyin, trove.TTroveTranscriptionRead(7, "你好", "Mandarin", "Pinyin"));
        Assert.Equal(bopomofo, trove.TTroveTranscriptionRead(7, "你好", "Mandarin", "Bopomofo"));
        Assert.Null(trove.TTroveTranscriptionRead(7, "你好", "Mandarin", "Yale"));
        Assert.Null(trove.TTroveTranscriptionRead(7, "再見", "Mandarin", "Pinyin"));
    }

    [Fact]
    public void TroveClear_Session_DropsEveryScheme()
    {
        LTrove trove = TInterface.TTroveCreate();
        LCandidate[] found = [TInterface.TCandidateCreate("Wiktionary", "nǐ hǎo", 0, true, string.Empty)];
        trove.TTroveTranscriptionSave(7, "你好", "Mandarin", "Pinyin", found);
        trove.TTroveTranscriptionSave(8, "你好", "Mandarin", "Pinyin", found);

        trove.TTroveClear(7);

        Assert.Null(trove.TTroveTranscriptionRead(7, "你好", "Mandarin", "Pinyin"));
        Assert.Equal(found, trove.TTroveTranscriptionRead(8, "你好", "Mandarin", "Pinyin"));
    }

    [Fact]
    public void TroveRecordingSave_NoAddress_KeepsNothing()
    {
        LTrove trove = TInterface.TTroveCreate();

        trove.TTroveRecordingSave(
            7, "tomato", "English", [TInterface.TRecordingCreate("Oxford", null, 0, false, string.Empty)]);

        Assert.Null(trove.TTroveRecordingRead(7, "tomato", "English"));
    }
}
