using KingmakerKingdomSheet.Domain.ValueObjects;

namespace KingmakerKingdomSheet.Domain.Tests;

public sealed class HexCoordinateTests
{
    [Fact]
    public void ToString_ReturnsColumnCommaRow()
    {
        var coordinate = new HexCoordinate(3, 7);

        var result = coordinate.ToString();

        Assert.Equal("3,7", result);
    }
}
