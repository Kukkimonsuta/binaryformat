// ReSharper disable InconsistentNaming

using BinaryFormat.IPv4.Tests.Data;
using Xunit;
using Xunit.Abstractions;

namespace BinaryFormat.IPv4.Tests;

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
        var reader = new BinaryFormatReader(IPv4PacketTestData.Frame01);

        var packet = reader.ReadIPv4Packet();

        Assert.Equal(4, packet.Version);
        Assert.Equal(5, packet.IHL);
        Assert.Equal(0, packet.DSCP);
        Assert.Equal(0, packet.ECN);
        Assert.Equal(0, packet.Flags);
        Assert.Equal(0, packet.FragmentOffset);
        Assert.Equal(128, packet.TTL);
        Assert.Equal(17, packet.Protocol);

        Assert.Equal(0, reader.Remainder.Length);
    }

    [Fact]
    public void Cannot_read_invalid_ipv4_packet()
    {
        var reader = new BinaryFormatReader(IPv4PacketTestData.InvalidFrame01);

        var packet = new IPv4PacketShape();
        var result = reader.TryReadIPv4Packet(ref packet);

        Assert.False(result);
        Assert.Equal(0, reader.Index);
        Assert.Equal(IPv4PacketTestData.InvalidFrame01.Length, reader.Remainder.Length);
    }
}
