using BinaryFormat.UDP.Tests.Data;
using Xunit;
using Xunit.Abstractions;

namespace BinaryFormat.UDP.Tests;

public class UdpPacketShapeTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public UdpPacketShapeTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Can_read_ipv4_packet()
    {
        var reader = new BinaryFormatReader(UdpPacketTestData.Frame01);

        var packet = reader.ReadUdpPacket();

        Assert.Equal(67, packet.SourcePort);
        Assert.Equal(67, packet.DestinationPort);
        Assert.Equal(384, packet.Length);
        Assert.Equal(0x720e, packet.Checksum);

        Assert.Equal(0, reader.Remainder.Length);
    }
}
