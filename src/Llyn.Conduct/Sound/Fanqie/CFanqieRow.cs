namespace Llyn.Conduct;

public sealed record CFanqieRow(
    long CFanqieRowId,
    int CFanqieRowRepresentative,
    bool CFanqieRowMarked,
    bool CFanqieRowPrimary,
    string CFanqieRowOrder,
    bool CFanqieRowClosed,
    string CFanqieRowSlashed,
    string CFanqieRowLabel,
    string CFanqieRowInitial,
    string CFanqieRowCell,
    string CFanqieRowBracketed,
    string CFanqieRowKnotted,
    string CFanqieRowMedial,
    string CFanqieRowGraded,
    string CFanqieRowTone,
    string CFanqieRowSpelling,
    string CFanqieRowRemainder);
