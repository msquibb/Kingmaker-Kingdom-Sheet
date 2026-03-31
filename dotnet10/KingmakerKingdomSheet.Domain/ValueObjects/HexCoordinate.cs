namespace KingmakerKingdomSheet.Domain.ValueObjects;

public readonly record struct HexCoordinate(int Column, int Row)
{
    public override string ToString() => $"{Column},{Row}";
}
