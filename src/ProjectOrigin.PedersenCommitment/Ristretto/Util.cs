using System;
using System.Runtime.InteropServices;
namespace ProjectOrigin.PedersenCommitment.Ristretto;

public static partial class Extensions
{

    public const String LIBRARY = "rust_ffi";
    internal static byte[] ToByteArray(this ulong value, uint arrayLength)
    {
        var inputBytes = BitConverter.GetBytes(value);
        var length = inputBytes.Length;

        if (arrayLength < length)
        {
            throw new ArgumentException("Wanted Length is smaller that source.");
        }

        var outputArray = new byte[arrayLength];
        Array.Copy(inputBytes, outputArray, inputBytes.Length);
        return outputArray;
    }

    [LibraryImport(LIBRARY, EntryPoint = "fill_bytes")]
    internal static partial void FillBytes(RawVec raw, byte[] dst);

    [LibraryImport(LIBRARY, EntryPoint = "free_vec")]
    internal static partial void FreeVec(RawVec raw);

    [StructLayout(LayoutKind.Sequential)]
    internal struct RawVec
    {
        internal IntPtr data;
        internal nuint size;
        internal nuint cap;
    }
}
