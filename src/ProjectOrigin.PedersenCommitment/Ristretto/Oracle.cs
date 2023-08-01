namespace ProjectOrigin.PedersenCommitment.Ristretto;
using System.Text;
using System.Runtime.InteropServices;
using System;

public partial class Oracle
{

    private partial class NativeTranscript
    {

        [LibraryImport("rust_ffi", EntryPoint = "transcript_new")]
        internal static partial IntPtr New(byte[] label, int len);

        [LibraryImport("rust_ffi", EntryPoint = "transcript_append_point")]
        internal static partial void AppendPoint(IntPtr self, byte[] label, int len, IntPtr point);

        [LibraryImport("rust_ffi", EntryPoint = "transcript_challenge_scalar")]
        internal static partial IntPtr ChallengeScalar(IntPtr self, byte[] label, int len);
    }

    private readonly IntPtr _ptr;

    public Oracle(byte[] label)
    {
        _ptr = NativeTranscript.New(label, label.Length);

    }

    public Oracle(String label)
    {
        var bytes = Encoding.UTF8.GetBytes(label);
        _ptr = NativeTranscript.New(bytes, bytes.Length);
    }

    public void Add(String label, params Point[] points)
    {
        var bytes = Encoding.UTF8.GetBytes(label);
        foreach (Point p in points)
        {
            NativeTranscript.AppendPoint(this._ptr, bytes, bytes.Length, p._ptr);
        }

    }

    public Scalar Challenge(String label)
    {
        var bytes = Encoding.UTF8.GetBytes(label);
        var scalar_ptr = NativeTranscript.ChallengeScalar(this._ptr, bytes, bytes.Length);
        return new Scalar(scalar_ptr);
    }
}
