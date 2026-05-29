namespace NonboxingUnion.Tests;

public class PlaceholderTests
{
    [Test]
    public async Task Placeholder()
    {
        var value = 1 + 1;
        await Assert.That(value).IsEqualTo(2);
    }
}
