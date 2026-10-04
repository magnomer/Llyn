using System;
using Llyn.Conduct;

namespace Llyn.Tests;

internal static class TInterfaceNavigation
{
    internal static void TNavigationTabOpen(this CNavigation navigation) => navigation.LNavigationTabOpen();

    internal static void TNavigationTabAdd(
        this CNavigation navigation,
        string tab,
        Func<bool> leave,
        Func<long> station,
        Action<bool> scribe,
        Action<long> arrival,
        Func<bool>? allowed = null) =>
        navigation.LNavigationTabAdd(tab, leave, station, scribe, arrival, allowed);

    internal static void TNavigationStationAdd(this CNavigation navigation) => navigation.LNavigationStationAdd();

    internal static void TNavigationDiweiAttach(this CNavigation navigation, Action<string, string, string> open) =>
        navigation.LNavigationDiweiAttach(open);

    internal static void TNavigationStemAttach(this CNavigation navigation, Action<string, string?> open) =>
        navigation.LNavigationStemAttach(open);

    internal static bool TNavigationDiweiOpen(
        this CNavigation navigation, string language, string kind, string key) =>
        navigation.LNavigationDiweiOpen(language, kind, key);

    internal static bool TNavigationStemOpen(this CNavigation navigation, string language, string? key) =>
        navigation.LNavigationStemOpen(language, key);

    internal static void TCorpusExampleOpen(this CCorpus corpus, long id) => corpus.LCorpusExampleOpen(id);

    internal static void TGuildAuthorOpen(this CGuild guild, long id) => guild.LGuildAuthorOpen(id);

    internal static void TRepertoireSituationOpen(this CRepertoire repertoire, long id) =>
        repertoire.LRepertoireSituationOpen(id);

    internal static CSituationDraft? TRepertoireScenarioRead(this CRepertoire repertoire) =>
        repertoire.CRepertoirePlaywright.LPlaywrightScenarioRead();

    internal static void TShelfReferenceOpen(this CShelf shelf, long id) => shelf.LShelfReferenceOpen(id);

    internal static void TXieshengStemOpen(this CXiesheng xiesheng, string language, string? key) =>
        xiesheng.LXieshengStemOpen(language, key);

    internal static void TYunjingDiweiOpen(this CYunjing yunjing, string language, string kind, string key) =>
        yunjing.LYunjingDiweiOpen(language, kind, key);

    internal static void TTaxonomyTagOpen(this CTaxonomy taxonomy, long id) => taxonomy.LTaxonomyTagOpen(id);

    internal static void TTenorRegisterOpen(this CTenor tenor, long id) => tenor.LTenorRegisterOpen(id);
}
