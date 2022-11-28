using BinaryFormat.EthernetFrame.Tests.Data;
using Xunit;
using Xunit.Abstractions;

namespace BinaryFormat.EthernetFrame.Tests;

public class EthernetFrameShapeTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public EthernetFrameShapeTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Can_read_l2_ethernet_frame()
    {
        var reader = new BinaryFormatReader(EthernetFrameTestData.Frame01);

        var frame = reader.ReadL2EthernetFrame();

        Assert.True(new ReadOnlySpan<byte>(EthernetFrameTestData.Frame01).Slice(0, 6).SequenceEqual(frame.Destination.Data));
        Assert.True(new ReadOnlySpan<byte>(EthernetFrameTestData.Frame01).Slice(6, 6).SequenceEqual(frame.Source.Data));
        Assert.Equal(0x800, frame.EtherTypeOrSize);
        Assert.True(new ReadOnlySpan<byte>(EthernetFrameTestData.Frame01).Slice(14).SequenceEqual(frame.Payload));

        Assert.Equal(0, reader.Remainder.Length);
    }
}
