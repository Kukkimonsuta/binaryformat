using BinaryFormat.Tests.Data;
using Xunit;
using Xunit.Abstractions;

namespace BinaryFormat.Tests;

public class BinaryFormatReaderTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public BinaryFormatReaderTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Can_read_until_end_index()
    {
        var reader = new BinaryFormatReader(BinaryFormatTestData.Bytes10);

        reader.Skip(4);
        var data = reader.Read(-4);

        Assert.Equal(2, data.Length);
        Assert.Equal(0x05, data[0]);
        Assert.Equal(0x06, data[1]);
    }

    [Fact]
    public void Can_read_until_end_index_zero_size()
    {
        var reader = new BinaryFormatReader(BinaryFormatTestData.Bytes10);

        reader.Skip(4);
        var data = reader.Read(-6);

        Assert.Equal(0, data.Length);
    }

    [Fact]
    public void Cannot_read_until_end_index_negative_size()
    {
        Assert.Throws<IndexOutOfRangeException>(
            () =>
            {
                var reader = new BinaryFormatReader(BinaryFormatTestData.Bytes10);
                reader.Skip(4);
                reader.Read(-8);
            }
        );
    }
}
