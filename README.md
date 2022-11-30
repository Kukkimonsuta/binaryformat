# binaryformat

Helpers for parsing various binary formats.

## Usage

````csharp
var buffer = new byte[] { 0x81, 0x00, 0x00, 0x7b };

// create reader 
var reader = new BinaryFormatReader(buffer);

// read vlan tag
var tag = reader.ReadVLANTag();

// output parsed information
Console.WriteLine($"TPID={tag.TPID}");
Console.WriteLine($"PCP={tag.PCP}");
Console.WriteLine($"DEI={tag.DEI}");
Console.WriteLine($"VID={tag.VID}");
````

## Parsing new formats

1. declare structure to hold parsed information

````csharp
public ref struct VLANTagShape
{
    public ushort TPID;
    public byte PCP;
    public bool DEI;
    public ushort VID;
}
````

2. write extension to read data from `BinaryFormatReader`

```csharp
public static class VLANTagShapeExtensions
{
    // separating actual reading logic to private method makes it easier not to 
    // read from wrong reader, see `TryReadVLANTag`
    private static bool TryReadVLANTagInternal(ref BinaryFormatReader reader, ref VLANTagShape vlanTag)
    {
        vlanTag.TPID = reader.ReadUInt16();
 
        var tci = reader.ReadUInt16();
        vlanTag.PCP = (byte)((tci & 0b11100000_00000000) >> 13);
        vlanTag.DEI = (tci & 0b00010000_00000000) != 0;
        vlanTag.VID = (ushort)(tci & 0b00001111_11111111);

        return true;
    }

    // main read method 
    public static bool TryReadVLANTag(this ref BinaryFormatReader reader, ref VLANTagShape vlanTag)
    {
        if (reader.Remainder.Length < 4)
        {
            return false;
        }

        // create new reader starting at current positon
        var scope = new BinaryFormatReader(reader.Remainder);

        // try read shape using the new reader
        if (!TryReadVLANTagInternal(ref scope, ref vlanTag))
        {
            return false;
        }

        // when successful, advance original reader by number of bytes read
        reader.Skip(scope.Index);
        return true;
    }

    // helper method for top level reads
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
```
