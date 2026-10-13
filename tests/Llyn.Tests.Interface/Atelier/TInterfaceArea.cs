using Llyn.Conduct;

namespace Llyn.Tests;

internal static class TInterfaceArea
{
    internal static void TCorpusVistaRestore(this CCorpus corpus) => corpus.LCorpusVistaRestore();

    internal static void TFavoriteVistaRestore(this CFavorite favorite) => favorite.LFavoriteVistaRestore();

    internal static void TGuildVistaRestore(this CGuild guild) => guild.LGuildVistaRestore();

    internal static void TLibraryVistaRestore(this CLibrary library) => library.LLibraryVistaRestore();

    internal static void TPhonologyVistaRestore(this CPhonology phonology) => phonology.LPhonologyVistaRestore();

    internal static void TRepertoireVistaRestore(this CRepertoire repertoire) =>
        repertoire.LRepertoireVistaRestore();

    internal static void TShelfVistaRestore(this CShelf shelf) => shelf.LShelfVistaRestore();

    internal static void TTaxonomyVistaRestore(this CTaxonomy taxonomy) => taxonomy.LTaxonomyVistaRestore();

    internal static void TTenorVistaRestore(this CTenor tenor) => tenor.LTenorVistaRestore();

    internal static void TXieshengVistaRestore(this CXiesheng xiesheng) => xiesheng.LXieshengVistaRestore();

    internal static void TYunjingVistaRestore(this CYunjing yunjing) => yunjing.LYunjingVistaRestore();
}
