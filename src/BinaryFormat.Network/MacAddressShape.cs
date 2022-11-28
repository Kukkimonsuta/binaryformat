using System.Runtime.CompilerServices;
using BinaryFormat;

namespace BinaryFormat.Network;

public ref struct MacAddressShape
{
    public ReadOnlySpan<byte> Data;
}

public static class MacAddressShapeExtensions
{
    private static bool TryReadMacAddressInternal(ref BinaryFormatReader reader, ref MacAddressShape macAddress)
    {
        macAddress.Data = reader.Read(6);

        return true;
    }

    public static bool TryReadMacAddress(this ref BinaryFormatReader reader, ref MacAddressShape macAddress)
    {
        if (reader.Remainder.Length < 6)
        {
            macAddress = default;
            return false;
        }

        var scope = new BinaryFormatReader(reader.Remainder);

        if (!TryReadMacAddressInternal(ref scope, ref macAddress))
        {
            return false;
        }

        reader.Skip(scope.Index);
        return true;
    }

    public static MacAddressShape ReadMacAddress(this ref BinaryFormatReader reader)
    {
        var macAddress = new MacAddressShape();

        if (!TryReadMacAddress(ref reader, ref macAddress))
        {
            throw new InvalidOperationException($"Cannot read {nameof(MacAddressShape)}");
        }

        return macAddress;
    }
}
