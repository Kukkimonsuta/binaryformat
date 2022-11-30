namespace BinaryFormat.EthernetFrame;

public ref struct VLANTagShape
{
    public ushort TPID;
    public byte PCP;
    public bool DEI;
    public ushort VID;
}

public static class VLANTagShapeExtensions
{
    private static bool TryReadVLANTagInternal(ref BinaryFormatReader reader, ref VLANTagShape vlanTag)
    {
        vlanTag.TPID = reader.ReadUInt16();

        var tci = reader.ReadUInt16();

        vlanTag.PCP = (byte)((tci & 0b11100000_00000000) >> 13);
        vlanTag.DEI = (tci & 0b00010000_00000000) != 0;
        vlanTag.VID = (ushort)(tci & 0b00001111_11111111);

        return true;
    }

    public static bool TryReadVLANTag(this ref BinaryFormatReader reader, ref VLANTagShape vlanTag)
    {
        if (reader.Remainder.Length < 4)
        {
            return false;
        }

        var scope = new BinaryFormatReader(reader.Remainder);

        if (!TryReadVLANTagInternal(ref scope, ref vlanTag))
        {
            return false;
        }

        reader.Skip(scope.Index);
        return true;
    }

    public static VLANTagShape ReadVLANTag(this ref BinaryFormatReader reader)
    {
        var vlanTag = new VLANTagShape();

        if (!TryReadVLANTag(ref reader, ref vlanTag))
        {
            throw new InvalidOperationException($"Cannot read {nameof(VLANTagShape)}");
        }

        return vlanTag;
    }
}
