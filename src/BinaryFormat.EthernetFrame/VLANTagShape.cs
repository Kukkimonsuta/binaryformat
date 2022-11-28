// ReSharper disable InconsistentNaming

using BinaryFormat;

namespace BinaryFormat.EthernetFrame;

public ref struct VLANTagShape
{
    public ReadOnlySpan<byte> Data;
}

public static class VLANTagShapeExtensions
{
    private static bool TryReadVLANTagInternal(ref BinaryFormatReader reader, ref VLANTagShape vlanTag)
    {
        vlanTag.Data = reader.Read(4);

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
