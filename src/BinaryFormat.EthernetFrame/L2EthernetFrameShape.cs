using BinaryFormat.Network;

namespace BinaryFormat.EthernetFrame;

public ref struct L2EthernetFrameShape
{
    public MacAddressShape Destination;
    public MacAddressShape Source;
    public VLANTagShape STag;
    public VLANTagShape CTag;
    public ushort EtherTypeOrSize;
    public ReadOnlySpan<byte> Payload;
    public uint FrameCheckSequence;
}

public static class L2EthernetFrameShapeExtensions
{
    private static bool TryReadL2EthernetFrameInternal(ref BinaryFormatReader reader, ref L2EthernetFrameShape l2EthernetFrame, bool hasFrameCheckSequence)
    {
        if (!reader.TryReadMacAddress(ref l2EthernetFrame.Destination))
        {
            return false;
        }
        if (!reader.TryReadMacAddress(ref l2EthernetFrame.Source))
        {
            return false;
        }
        l2EthernetFrame.EtherTypeOrSize = reader.PeekUInt16();

        if (l2EthernetFrame.EtherTypeOrSize == 0x88a8)
        {
            if (!reader.TryReadVLANTag(ref l2EthernetFrame.STag))
            {
                return false;
            }
            if (!reader.TryReadVLANTag(ref l2EthernetFrame.CTag) || l2EthernetFrame.CTag.TPID != 0x8100)
            {
                return false;
            }

            l2EthernetFrame.EtherTypeOrSize = reader.ReadUInt16();
        }
        else if (l2EthernetFrame.EtherTypeOrSize == 0x8100)
        {
            if (!reader.TryReadVLANTag(ref l2EthernetFrame.CTag))
            {
                return false;
            }

            l2EthernetFrame.EtherTypeOrSize = reader.ReadUInt16();
        }
        else
        {
            reader.Skip(2);
        }

        if (hasFrameCheckSequence)
        {
            l2EthernetFrame.Payload = reader.Read(-4);
            l2EthernetFrame.FrameCheckSequence = reader.ReadUInt32();
        }
        else
        {
            l2EthernetFrame.Payload = reader.Read(0);
        }

        return true;
    }

    public static bool TryReadL2EthernetFrame(this ref BinaryFormatReader reader, ref L2EthernetFrameShape l2EthernetFrame, bool hasFrameCheckSequence = false)
    {
        if (reader.Remainder.Length < 18)
        {
            return false;
        }

        var scope = new BinaryFormatReader(reader.Remainder);

        if (!TryReadL2EthernetFrameInternal(ref scope, ref l2EthernetFrame, hasFrameCheckSequence: hasFrameCheckSequence))
        {
            return false;
        }

        reader.Skip(scope.Index);
        return true;
    }

    public static L2EthernetFrameShape ReadL2EthernetFrame(this ref BinaryFormatReader reader, bool hasFrameCheckSequence = false)
    {
        var l2EthernetFrame = new L2EthernetFrameShape();

        if (!TryReadL2EthernetFrame(ref reader, ref l2EthernetFrame, hasFrameCheckSequence: hasFrameCheckSequence))
        {
            throw new InvalidOperationException($"Cannot read {nameof(L2EthernetFrameShape)}");
        }

        return l2EthernetFrame;
    }
}
