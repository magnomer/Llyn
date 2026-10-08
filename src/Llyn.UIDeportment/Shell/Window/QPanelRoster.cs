using System;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QPanelRoster
{
    private readonly Window _qPanelRosterSurface;

    private readonly QLayout _qLayout;

    private readonly QInput _qInput;

    private readonly QTaxonomy _qTaxonomy;

    private readonly QTenor _qTenor;

    private readonly QRepertoire _qRepertoire;

    private readonly QCorpus _qCorpus;

    private readonly QReference _qReference;

    private readonly QLibrary _qLibrary;

    private readonly QPhonology _qPhonology;

    private readonly QXiesheng _qXiesheng;

    private readonly QYunjing _qYunjing;

    private readonly QFavorite _qFavorite;

    private readonly QDuplex _qDuplex;

    private readonly QGuild _qGuild;

    private readonly QSettings _qSettings;

    internal QPanelRoster(Window window, QPosture posture)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(posture);

        _qPanelRosterSurface = window;
        _qLayout = new QLayout(posture);

        _qInput = new QInput(QInputSurface);
        _qTaxonomy = new QTaxonomy(QTaxonomySurface);
        _qTenor = new QTenor(QTenorSurface);
        _qRepertoire = new QRepertoire(QRepertoireSurface);
        _qCorpus = new QCorpus(QCorpusSurface);
        _qReference = new QReference(QReferenceSurface);
        _qLibrary = new QLibrary(QLibrarySurface);
        _qPhonology = new QPhonology(QPhonologySurface);
        _qXiesheng = new QXiesheng(QXieshengSurface);
        _qYunjing = new QYunjing(QYunjingSurface);
        _qFavorite = new QFavorite(QFavoriteSurface);
        _qDuplex = new QDuplex(QDuplexSurface);
        _qGuild = new QGuild(QGuildSurface);
        _qSettings = new QSettings(QSettingsSurface, _qLayout, posture);
    }

    private UserControl QInputSurface => QContract.QContractFind<UserControl>(_qPanelRosterSurface, "PInput");

    private UserControl QLibrarySurface => QContract.QContractFind<UserControl>(_qPanelRosterSurface, "PLibrary");

    private UserControl QPhonologySurface => QContract.QContractFind<UserControl>(_qPanelRosterSurface, "PPhonology");

    private UserControl QXieshengSurface => QContract.QContractFind<UserControl>(_qPanelRosterSurface, "PXiesheng");

    private UserControl QYunjingSurface => QContract.QContractFind<UserControl>(_qPanelRosterSurface, "PYunjing");

    private UserControl QTaxonomySurface => QContract.QContractFind<UserControl>(_qPanelRosterSurface, "PTaxonomy");

    private UserControl QTenorSurface => QContract.QContractFind<UserControl>(_qPanelRosterSurface, "PTenor");

    private UserControl QRepertoireSurface => QContract.QContractFind<UserControl>(_qPanelRosterSurface, "PRepertoire");

    private UserControl QCorpusSurface => QContract.QContractFind<UserControl>(_qPanelRosterSurface, "PCorpus");

    private UserControl QReferenceSurface => QContract.QContractFind<UserControl>(_qPanelRosterSurface, "PReference");

    private UserControl QGuildSurface => QContract.QContractFind<UserControl>(_qPanelRosterSurface, "PGuild");

    private UserControl QFavoriteSurface => QContract.QContractFind<UserControl>(_qPanelRosterSurface, "PFavorite");

    private UserControl QDuplexSurface => QContract.QContractFind<UserControl>(_qPanelRosterSurface, "PDuplex");

    private UserControl QSettingsSurface => QContract.QContractFind<UserControl>(_qPanelRosterSurface, "PSettings");

    internal void QPanelRosterIntroduce(CAtelier atelier, CEnvoy envoy, QVolume volume, QMentionMenu mentionMenu)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _qInput.QInputIntroduce(atelier, envoy, volume, mentionMenu);
        _qLibrary.QLibraryIntroduce(atelier, envoy, volume, mentionMenu);
        _qPhonology.QPhonologyIntroduce(atelier, envoy, volume, mentionMenu);
        _qXiesheng.QXieshengIntroduce(atelier, envoy, volume, mentionMenu);
        _qYunjing.QYunjingIntroduce(atelier, envoy, volume, mentionMenu);
        _qTaxonomy.QTaxonomyIntroduce(atelier, envoy, volume, mentionMenu);
        _qTenor.QTenorIntroduce(atelier, envoy, volume, mentionMenu);
        _qRepertoire.QRepertoireIntroduce(atelier, envoy, volume, mentionMenu);
        _qCorpus.QCorpusIntroduce(atelier, envoy, volume, mentionMenu);
        _qReference.QReferenceIntroduce(atelier, envoy, volume, mentionMenu);
        _qGuild.QGuildIntroduce(atelier, envoy);
        _qFavorite.QFavoriteIntroduce(atelier, envoy, volume, mentionMenu);
        _qDuplex.QDuplexIntroduce(atelier, envoy, volume, mentionMenu);
        _qSettings.QSettingsIntroduce(atelier, envoy);

        CWorkspace workspace = atelier.CAtelierWorkspace;
        workspace.CWorkspaceOpened += _qLibrary.QLibraryVistaRefine;
        workspace.CWorkspaceOpened += _qPhonology.QPhonologyVistaRefine;
        workspace.CWorkspaceOpened += _qFavorite.QFavoriteVistaRefine;
        workspace.CWorkspaceOpened += _qTaxonomy.QTaxonomyVistaRefine;
        workspace.CWorkspaceOpened += _qTenor.QTenorVistaRefine;
        workspace.CWorkspaceOpened += _qRepertoire.QRepertoireVistaRefine;
        workspace.CWorkspaceOpened += _qReference.QReferenceVistaRefine;
        workspace.CWorkspaceOpened += _qCorpus.QCorpusVistaRefine;
        workspace.CWorkspaceOpened += _qGuild.QGuildVistaRefine;
        workspace.CWorkspaceOpened += _qGuild.QTallyRefine;
        workspace.CWorkspaceOpened += _qXiesheng.QXieshengVistaRefine;
        workspace.CWorkspaceOpened += _qXiesheng.QGroveRefine;
        workspace.CWorkspaceOpened += _qXiesheng.QXieshengStemRefine;
        workspace.CWorkspaceOpened += _qYunjing.QYunjingVistaRefine;
        workspace.CWorkspaceOpened += _qYunjing.QYunjingDiweiRefine;
        workspace.CWorkspaceOpened += _qLayout.QLayoutResetRefine;
        workspace.CWorkspaceOpened += _qLayout.QLayoutRefine;
    }

    internal void QPanelRosterAttach()
    {
        _qLayout.QLayoutAttach((Grid)QLibrarySurface.Content, "library");
        _qLayout.QLayoutAttach((Grid)QPhonologySurface.Content, "phonology");
        _qLayout.QLayoutAttach((Grid)QXieshengSurface.Content, "xiesheng");
        _qLayout.QLayoutAttach((Grid)QYunjingSurface.Content, "yunjing");
        _qLayout.QLayoutAttach((Grid)QTaxonomySurface.Content, "taxonomy");
        _qLayout.QLayoutAttach((Grid)QTenorSurface.Content, "tenor");
        _qLayout.QLayoutAttach((Grid)QRepertoireSurface.Content, "repertoire");
        _qLayout.QLayoutAttach((Grid)QCorpusSurface.Content, "corpus");
        _qLayout.QLayoutAttach((Grid)QReferenceSurface.Content, "reference");
        _qLayout.QLayoutAttach((Grid)QGuildSurface.Content, "guild");
        _qLayout.QLayoutAttach((Grid)QFavoriteSurface.Content, "favorite");
    }

    internal void QPanelRosterClose()
    {
        _qInput.QInputExitRefine();
        _qLibrary.QLibraryExitRefine();
        _qPhonology.QPhonologyExitRefine();
        _qXiesheng.QXieshengExitRefine();
        _qYunjing.QYunjingExitRefine();
        _qTaxonomy.QTaxonomyExitRefine();
        _qTenor.QTenorExitRefine();
        _qRepertoire.QRepertoireExitRefine();
        _qCorpus.QCorpusExitRefine();
        _qReference.QReferenceExitRefine();
        _qGuild.QGuildClose();
        _qFavorite.QFavoriteExitRefine();
    }
}
