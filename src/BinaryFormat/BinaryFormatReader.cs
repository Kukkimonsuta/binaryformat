using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace BinaryFormat;

public ref struct BinaryFormatReader
{
    public BinaryFormatReader(ReadOnlySpan<byte> data, Endianness endianness = Endianness.Big)
    {
        Data = data;
        Endianness = endianness;
    }

    public ReadOnlySpan<byte> Data { get; }
    public Endianness Endianness { get; private set; }

    public int Index { get; set; }
    public ReadOnlySpan<byte> Remainder => Data[Index..];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> Peek(int length)
    {
        if (length == 0)
        {
            return Remainder;
        }
        else if (length > 0)
        {
            if (Remainder.Length < length)
                throw new IndexOutOfRangeException();

            return Remainder[..length];
        }
        else
        {
            if (Remainder.Length < -length)
                throw new IndexOutOfRangeException();

            return Remainder[..^-length];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> Read(int length)
    {
        var result = Peek(length);
        Index += result.Length;
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Skip(int count)
    {
        if (Remainder.Length < count)
            throw new IndexOutOfRangeException(nameof(count));

        Index += count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte PeekByte()
    {
        if (Remainder.Length < 1)
            throw new IndexOutOfRangeException();

        return Remainder[0];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte ReadByte()
    {
        var result = PeekByte();
        Index += 1;
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short PeekInt16()
    {
        if (Remainder.Length < 2)
            throw new IndexOutOfRangeException();

        return Endianness == Endianness.Big ? BinaryPrimitives.ReadInt16BigEndian(Remainder) : BinaryPrimitives.ReadInt16LittleEndian(Remainder);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short ReadInt16()
    {
        var result = PeekInt16();
        Index += 2;
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort PeekUInt16()
    {
        if (Remainder.Length < 2)
            throw new IndexOutOfRangeException();

        return Endianness == Endianness.Big ? BinaryPrimitives.ReadUInt16BigEndian(Remainder) : BinaryPrimitives.ReadUInt16LittleEndian(Remainder);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort ReadUInt16()
    {
        var result = PeekUInt16();
        Index += 2;
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int PeekInt32()
    {
        if (Remainder.Length < 4)
            throw new IndexOutOfRangeException();

        return Endianness == Endianness.Big ? BinaryPrimitives.ReadInt32BigEndian(Remainder) : BinaryPrimitives.ReadInt32LittleEndian(Remainder);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadInt32()
    {
        var result = PeekInt32();
        Index += 4;
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint PeekUInt32()
    {
        if (Remainder.Length < 4)
            throw new IndexOutOfRangeException();

        return Endianness == Endianness.Big ? BinaryPrimitives.ReadUInt32BigEndian(Remainder) : BinaryPrimitives.ReadUInt32LittleEndian(Remainder);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadUInt32()
    {
        var result = PeekUInt32();
        Index += 4;
        return result;
    }
}
