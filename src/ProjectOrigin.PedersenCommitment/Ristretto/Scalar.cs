using System;
using System.Runtime.InteropServices;

namespace ProjectOrigin.PedersenCommitment.Ristretto;

/// <summary>
/// Scalar referencing a Rust object, guaranteed to always be in the field.
/// </summary>
public sealed partial class Scalar
{
    private partial class Native
    {
        [LibraryImport("rust_ffi", EntryPoint = "scalar_new")]
        internal static partial IntPtr New(byte[] bytes);

        [LibraryImport("rust_ffi", EntryPoint = "scalar_random")]
        internal static partial IntPtr Random();

        [LibraryImport("rust_ffi", EntryPoint = "scalar_spill_guts")]
        internal static partial void SpillGuts(IntPtr self);

        [LibraryImport("rust_ffi", EntryPoint = "scalar_to_bytes")]
        internal static partial void ToBytes(IntPtr self, byte[] output);

        [LibraryImport("rust_ffi", EntryPoint = "scalar_free")]
        internal static partial void Free(IntPtr self);

        [LibraryImport("rust_ffi", EntryPoint = "scalar_add")]
        internal static partial IntPtr Add(IntPtr lhs, IntPtr rhs);

        [LibraryImport("rust_ffi", EntryPoint = "scalar_sub")]
        internal static partial IntPtr Sub(IntPtr lhs, IntPtr rhs);

        [LibraryImport("rust_ffi", EntryPoint = "scalar_negate")]
        internal static partial IntPtr Negate(IntPtr self);

        [LibraryImport("rust_ffi", EntryPoint = "scalar_mul")]
        internal static partial IntPtr Mul(IntPtr lhs, IntPtr rhs);

        [LibraryImport("rust_ffi", EntryPoint = "scalar_sum")]
        internal static partial IntPtr Sum(IntPtr[] args, int len);

        [LibraryImport("rust_ffi", EntryPoint = "scalar_equals")]
        [return: MarshalAs(UnmanagedType.U1)]
        internal static partial bool Equals(IntPtr lhs, IntPtr rhs);

        [LibraryImport("rust_ffi", EntryPoint = "scalar_hash_from_bytes")]
        internal static partial IntPtr HashFromBytes(byte[] bytes, int len);
    }

    internal readonly IntPtr _ptr;

    internal Scalar(IntPtr ptr)
    {
        _ptr = ptr;
    }

    ~Scalar()
    {
        Native.Free(_ptr);
    }

    /// <summary>
    /// Construct a new Scalar from a unsigned long
    /// </summary>
    /// <param name="value">value to map into the field</param>
    /// <returns>Scalar representing the value</returns>
    public Scalar(ulong value)
    {
        var bytes = value.ToByteArray(32);
        _ptr = Native.New(bytes);
    }

    /// <summary>
    /// Construct a new Scalar from a byte span of size 32
    /// </summary>
    /// <param name="bytes">value to map into the field</param>
    /// <returns>Scalar representing the value</returns>
    public Scalar(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != 32)
        {
            throw new ArgumentException("Length has to be 32");
        }
        _ptr = Native.New(bytes.ToArray());
    }

    /// <summary>
    /// Construct a new Scalar from a byte array of size 32
    /// </summary>
    /// <param name="bytes">value to map into the field</param>
    /// <returns>Scalar representing the value</returns>
    public Scalar(byte[] bytes)
    {
        if (bytes.Length != 32)
        {
            throw new ArgumentException("Byte length has to 32");
        }
        _ptr = Native.New(bytes);
    }

    public static Scalar Random()
    {
        return new Scalar(Native.Random());
    }

    /// <summary>
    /// Construct a new scalar by hashing with sha3-512 an arbitrary length byte array
    /// </summary>
    /// <param name="bytes">byte array to be hashed</param>
    /// <returns>a new scalar from the hashed bytes</returns>
    public static Scalar HashFromBytes(byte[] bytes)
    {
        return new Scalar(Native.HashFromBytes(bytes, bytes.Length));
    }

    public void SpillGuts()
    {
        Native.SpillGuts(_ptr);
    }

    /// <summary>
    /// Construct a 32-byte array from the Scalar
    /// </summary>
    /// <returns>a 32-byte array</returns>
    public byte[] ToBytes()
    {
        var bytes = new byte[32];
        Native.ToBytes(_ptr, bytes);
        return bytes;
    }

    public static Scalar operator +(Scalar left, Scalar right)
    {
        var ptr = Native.Add(left._ptr, right._ptr);
        return new Scalar(ptr);
    }

    public static Scalar operator -(Scalar left, Scalar right)
    {
        var ptr = Native.Sub(left._ptr, right._ptr);
        return new Scalar(ptr);
    }

    public static Scalar operator -(Scalar self)
    {
        var ptr = Native.Negate(self._ptr);
        return new Scalar(ptr);
    }

    public static Scalar operator *(Scalar left, Scalar right)
    {
        var ptr = Native.Mul(left._ptr, right._ptr);
        return new Scalar(ptr);
    }

    public override bool Equals(object? obj)
    {
        if (obj is Scalar)
        {
            return this == (Scalar)obj;
        }
        else
        {
            return false;
        }
    }

    public static bool operator ==(Scalar left, Scalar right)
    {
        if (left._ptr == right._ptr)
        {
            return true;
        }
        return Native.Equals(left._ptr, right._ptr);
    }

    public static bool operator !=(Scalar left, Scalar right)
    {
        return !Native.Equals(left._ptr, right._ptr);
    }

    public override int GetHashCode() => base.GetHashCode();

    public static Scalar Sum(params Scalar[] args)
    {
        var ptrs = new IntPtr[args.Length];
        for (int i = 0; i < args.Length; i++)
            ptrs[i] = args[i]._ptr;

        var resPtr = Native.Sum(ptrs, ptrs.Length);
        return new Scalar(resPtr);
    }
}
