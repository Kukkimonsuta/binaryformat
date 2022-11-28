using System.Buffers;
using BinaryFormat;

namespace BinaryFormat.IPv4;

public ref struct IPv4PacketShape
{
    public byte Version;
    public byte IHL;
    public byte DSCP;
    public byte ECN;
    public ushort Length;
    public ushort Identification;
    public byte Flags;
    public ushort FragmentOffset;
    public byte TTL;
    public byte Protocol;
    public ushort HeaderChecksum;
    public uint SourceIpAddress;
    public uint DestinationIpAddress;
    public ReadOnlySpan<byte> Options;

    public ReadOnlySpan<byte> Payload;
}

public static class IPv4PacketShapeExtensions
{
    private static bool TryReadIPv4PacketInternal(ref BinaryFormatReader reader, ref IPv4PacketShape ipv4Packet)
    {
        var versionIhl = reader.ReadByte();
        ipv4Packet.Version = (byte)((versionIhl & 0b11110000) >> 4);
        if (ipv4Packet.Version != 4)
        {
            return false;
        }
        ipv4Packet.IHL = (byte)(versionIhl & 0b1111);
        if (ipv4Packet.IHL < 5)
        {
            return false;
        }

        var dscpEcn = reader.ReadByte();
        ipv4Packet.DSCP = (byte)((dscpEcn & 0b11111100) >> 2);
        ipv4Packet.ECN = (byte)(dscpEcn & 0b11);

        ipv4Packet.Length = reader.ReadUInt16();
        ipv4Packet.Identification = reader.ReadUInt16();

        var flagsFragmentOffset = reader.ReadUInt16();
        ipv4Packet.Flags = (byte)((flagsFragmentOffset & 0b11100000_00000000) >> 13);
        if ((ipv4Packet.Flags & 0b100) != 0)
        {
            return false;
        }
        ipv4Packet.FragmentOffset = (ushort)(flagsFragmentOffset & 0b00011111_11111111);

        ipv4Packet.TTL = reader.ReadByte();
        ipv4Packet.Protocol = reader.ReadByte();

        ipv4Packet.HeaderChecksum = reader.ReadUInt16();

        ipv4Packet.SourceIpAddress = reader.ReadUInt32();
        ipv4Packet.DestinationIpAddress = reader.ReadUInt32();

        var payloadLength = ipv4Packet.Length - ipv4Packet.IHL * 4;
        if (reader.Remainder.Length < payloadLength)
        {
            return false;
        }

        ipv4Packet.Payload = reader.Read(payloadLength);

        return true;
    }

    public static bool TryReadIPv4Packet(this ref BinaryFormatReader reader, ref IPv4PacketShape ipv4Packet)
    {
        if (reader.Remainder.Length < 20)
        {
            return false;
        }

        var scope = new BinaryFormatReader(reader.Remainder);

        if (!TryReadIPv4PacketInternal(ref scope, ref ipv4Packet))
        {
            return false;
        }

        reader.Skip(scope.Index);
        return true;
    }

    public static IPv4PacketShape ReadIPv4Packet(this ref BinaryFormatReader reader)
    {
        var ipv4Packet = new IPv4PacketShape();

        if (!TryReadIPv4Packet(ref reader, ref ipv4Packet))
        {
            throw new InvalidOperationException($"Cannot read {nameof(IPv4PacketShape)}");
        }

        return ipv4Packet;
    }
}
