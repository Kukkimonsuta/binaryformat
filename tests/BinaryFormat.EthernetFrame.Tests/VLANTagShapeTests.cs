using BinaryFormat.EthernetFrame.Tests.Data;
using Xunit;
using Xunit.Abstractions;

namespace BinaryFormat.EthernetFrame.Tests;

public class VLANTagShapeTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public VLANTagShapeTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Can_read_vlan_tag()
    {
        var reader = new BinaryFormatReader(EthernetFrameTestData.VLANTag01);

        var frame = reader.ReadVLANTag();

        Assert.Equal(0x8100, frame.TPID);
        Assert.Equal(0, frame.PCP);
        Assert.False(frame.DEI);
        Assert.Equal(123, frame.VID);

        Assert.Equal(0, reader.Remainder.Length);
    }
}
