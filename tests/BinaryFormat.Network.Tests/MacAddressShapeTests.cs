using BinaryFormat.Network.Tests.Data;
using Xunit;
using Xunit.Abstractions;

namespace BinaryFormat.Network.Tests;

public class IPv4PacketShapeTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public IPv4PacketShapeTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Can_read_ipv4_packet()
    {
        var reader = new BinaryFormatReader(MacAddressTestData.MacAddress01);

        var macAddress = reader.ReadMacAddress();

        Assert.Equal(6, macAddress.Data.Length);
        Assert.True(MacAddressTestData.MacAddress01.SequenceEqual(macAddress.Data.ToArray()));

        Assert.Equal(0, reader.Remainder.Length);
    }
}
