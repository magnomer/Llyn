namespace Llyn.Core;

public sealed record LAncestorRelation(
    long LAncestorRelationId,
    string LAncestorRelationName,
    bool LAncestorRelationBuiltin = false,
    int LAncestorRelationUsage = 0)
{
    public string LAncestorRelationName { get; init; } = LAncestorRelationName ?? string.Empty;

    public string LAncestorRelationKey =>
        !LAncestorRelationBuiltin
            ? string.Empty
            : LAncestorRelationName switch
            {
                "borrowed" => "Relation.Borrowed",
                "compound" => "Relation.Compound",
                "derived" => "Relation.Derived",
                "calque" => "Relation.Calque",
                "blend" => "Relation.Blend",
                _ => string.Empty,
            };
}
