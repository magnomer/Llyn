namespace Llyn.Conduct;

internal sealed class CProfferLantern
{
    private int _cProfferLanternIndex = -1;

    private int _cProfferLanternCount;

    public int CProfferLanternIndex => _cProfferLanternIndex;

    public bool CProfferLanternLit => _cProfferLanternIndex >= 0;

    public void CProfferLanternShow(int count)
    {
        _cProfferLanternIndex = -1;
        _cProfferLanternCount = count;
    }

    public int? CProfferLanternMove(int step)
    {
        int? lit = CLantern.CLanternMove(_cProfferLanternIndex, _cProfferLanternCount, step);
        _cProfferLanternIndex = lit ?? _cProfferLanternIndex;
        return lit;
    }

    public int? CProfferLanternSet(int index)
    {
        int? lit = index >= 0 && index < _cProfferLanternCount ? index : null;
        _cProfferLanternIndex = lit ?? _cProfferLanternIndex;
        return lit;
    }
}
