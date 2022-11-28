namespace BinaryFormat.UDP;

public ref struct UdpPacketShape
{
    public ushort SourcePort;
    public ushort DestinationPort;
    public ushort Length;
    public ushort Checksum;

    public ReadOnlySpan<byte> Payload;
}

public static class UdpPacketShapeExtensions
{
    private static bool TryReadUdpPacketInternal(ref BinaryFormatReader reader, ref UdpPacketShape udpPacket)
    {
        udpPacket.SourcePort = reader.ReadUInt16();
        udpPacket.DestinationPort = reader.ReadUInt16();
        udpPacket.Length = reader.ReadUInt16();
        if (udpPacket.Length < 8)
        {
            return false;
        }
        udpPacket.Checksum = reader.ReadUInt16();

        var payloadLength = udpPacket.Length - 8;
        if (reader.Remainder.Length < payloadLength)
        {
            return false;
        }

        udpPacket.Payload = reader.Read(payloadLength);

        return true;
    }

    public static bool TryReadUdpPacket(this ref BinaryFormatReader reader, ref UdpPacketShape udpPacket)
    {
        if (reader.Remainder.Length < 16)
        {
            return false;
        }

        var scope = new BinaryFormatReader(reader.Remainder);

        if (!TryReadUdpPacketInternal(ref scope, ref udpPacket))
        {
            return false;
        }

        reader.Skip(scope.Index);
        return true;
    }

    public static UdpPacketShape ReadUdpPacket(this ref BinaryFormatReader reader)
    {
        var udpPacket = new UdpPacketShape();

        if (!TryReadUdpPacket(ref reader, ref udpPacket))
        {
            throw new InvalidOperationException($"Cannot read {nameof(UdpPacketShape)}");
        }

        return udpPacket;
    }
}
