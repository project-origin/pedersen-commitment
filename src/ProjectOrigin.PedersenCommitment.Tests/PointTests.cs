using ProjectOrigin.PedersenCommitment.Ristretto;
using Xunit;

namespace ProjectOrigin.PedersenCommitment.Tests;

public class PointTests
{
    [Fact]
    public void Elligator()
    {
        var seed = new byte[64];
        seed[0] = 2;
        var p = Ristretto.Point.FromUniformBytes(seed);

        Assert.NotNull(p);
    }

    [Fact]
    public void CreateUsingGenrator()
    {
        var pc_gens = Generator.Default;
        var blinding = Scalar.Random();

        var point = pc_gens.Commit(12, blinding);
        Assert.NotNull(point);

        var compressed = point.Compress();

        var decompressed = compressed.Decompress();
        Assert.Equal(point, decompressed);
    }

    [Fact]
    public void CompressDecompress()
    {
        var seed = new byte[64];
        seed[0] = 2;
        var point = Ristretto.Point.FromUniformBytes(seed);
        var a = point.Compress();
        var b = a.Decompress();
        var c = b.Compress();
        Assert.Equal(a, c);
    }

    [Fact]
    public void Sub()
    {
        var seed = new byte[64];
        seed[0] = 2;
        var g = Ristretto.Point.FromUniformBytes(seed);
        var a = g * new Scalar(2);
        var b = g * new Scalar(7);
        var c = g * new Scalar(5);

        Assert.Equal(c, b - a);
    }

    [Fact]
    public void MulScalar()
    {
        var seed = new byte[64];
        seed[0] = 2;
        var p = Ristretto.Point.FromUniformBytes(seed);
        var p7 = Ristretto.Point.FromUniformBytes(seed);

        var p1 = p * new Scalar(1);

        Assert.Equal(p, p1);
        var p2 = p * new Scalar(2);
        Assert.NotEqual(p1, p2);

        var p3 = p * new Scalar(3);

        var p5 = p2 + p3;

        var p5_ = p * new Scalar(5);

        Assert.Equal(p5, p5_);
    }
}

