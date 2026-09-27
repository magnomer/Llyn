using System;
using System.Collections.Generic;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QUnion
{
    private const int QUnionLimit = 8;

    private readonly LDesk _qUnionDesk;

    private readonly Func<long?> _qUnionAuthorSeam;

    private readonly Func<string, long, int, IReadOnlyList<CCatalogAuthor>> _qUnionFindSeam;

    private readonly Func<long, string> _qUnionNameSeam;

    private readonly Action<long, long> _qUnionAbsorbSeam;

    private readonly Func<string, string, bool> _qUnionSeam;

    private readonly Action<long> _qUnionOpenSeam;

    internal QUnion(
        LDesk desk,
        Func<long?> authorSeam,
        Func<string, long, int, IReadOnlyList<CCatalogAuthor>> findSeam,
        Func<long, string> nameSeam,
        Action<long, long> absorbSeam,
        Func<string, string, bool> unionSeam,
        Action<long> openSeam)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(authorSeam);
        ArgumentNullException.ThrowIfNull(findSeam);
        ArgumentNullException.ThrowIfNull(nameSeam);
        ArgumentNullException.ThrowIfNull(absorbSeam);
        ArgumentNullException.ThrowIfNull(unionSeam);
        ArgumentNullException.ThrowIfNull(openSeam);

        _qUnionDesk = desk;
        _qUnionAuthorSeam = authorSeam;
        _qUnionFindSeam = findSeam;
        _qUnionNameSeam = nameSeam;
        _qUnionAbsorbSeam = absorbSeam;
        _qUnionSeam = unionSeam;
        _qUnionOpenSeam = openSeam;
    }

    public event Action<string, Exception>? QUnionFailed;

    public bool QUnionShown => _qUnionDesk.LDeskStored;

    public IReadOnlyList<CCatalogAuthor> QUnionRead(string typed)
    {
        ArgumentNullException.ThrowIfNull(typed);

        if (string.IsNullOrWhiteSpace(typed))
        {
            return [];
        }

        if (_qUnionAuthorSeam() is not long author)
        {
            return [];
        }

        return _qUnionFindSeam(typed, author, QUnionLimit);
    }

    public void QUnionSelect(long? id)
    {
        if (id is not long kept)
        {
            return;
        }

        if (_qUnionAuthorSeam() is not long author)
        {
            return;
        }

        if (!QUnionConfirm(kept))
        {
            return;
        }

        try
        {
            _qUnionAbsorbSeam(kept, author);
        }
        catch (Exception exception)
        {
            QUnionFailed?.Invoke("Guild.MergeFailed", exception);
            return;
        }

        _qUnionOpenSeam(kept);
    }

    private bool QUnionConfirm(long kept)
    {
        return _qUnionSeam(
            _qUnionDesk.LDeskRead()?.LDraftAuthorName ?? string.Empty,
            _qUnionNameSeam(kept));
    }
}
