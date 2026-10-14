using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Tests;

internal sealed class TVaultFakeFolder : LDeckFolderVault
{
    private readonly TVaultFakeDeck _tVaultFakeDecks;
    private readonly List<LDeckFolder> _tVaultFakeFolders = [];
    private long _tVaultFakeNext = 100;

    public TVaultFakeFolder(TVaultFakeDeck decks)
    {
        _tVaultFakeDecks = decks;
    }

    public LDeckFolder LDeckFolderCreate(string name)
    {
        int position = _tVaultFakeFolders.Count == 0
            ? 0
            : _tVaultFakeFolders.Max(static folder => folder.LDeckFolderPosition) + 1;
        LDeckFolder created = new(++_tVaultFakeNext, name, position);
        _tVaultFakeFolders.Add(created);
        return created;
    }

    public IReadOnlyList<LDeckFolder> LDeckFolderRead() =>
        _tVaultFakeFolders
            .OrderBy(static folder => folder.LDeckFolderPosition)
            .ThenBy(static folder => folder.LDeckFolderId)
            .ToList();

    public void LDeckFolderUpdate(LDeckFolder folder)
    {
        int index = _tVaultFakeFolders.FindIndex(held => held.LDeckFolderId == folder.LDeckFolderId);
        if (index < 0)
        {
            throw new InvalidOperationException($"No deck folder carries the id '{folder.LDeckFolderId}'.");
        }

        _tVaultFakeFolders[index] = folder;
    }

    public void LDeckFolderDelete(long folderId)
    {
        if (_tVaultFakeFolders.RemoveAll(folder => folder.LDeckFolderId == folderId) == 0)
        {
            return;
        }

        foreach (LDeck deck in _tVaultFakeDecks.LDeckRead().Where(deck => deck.LDeckFolderId == folderId))
        {
            _tVaultFakeDecks.LDeckUpdate(deck with { LDeckFolderId = null });
        }
    }
}
