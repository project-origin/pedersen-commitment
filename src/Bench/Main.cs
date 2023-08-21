using ProjectOrigin.PedersenCommitment.Ristretto;
using ProjectOrigin.PedersenCommitment;
using System.Threading.Tasks;
using System.Text;
using System;

class ProgramEntry
{

    static int Main(string[] args)
    {
        uint iters = 100;
        System.Console.WriteLine($"Running {iters} iterations sequentially");
        var rangeproofs = new RangeProof[iters];
        var commitments = new Commitment[iters];

        // Sequential
        byte[] last = new byte[0];
        var watch = System.Diagnostics.Stopwatch.StartNew();
        for (uint i = 0; i < iters; i++) {
            var s = new SecretCommitmentInfo(i);
            rangeproofs[i] = RangeProof.FromBytes(s.CreateRangeProof("bench").ToArray());
            commitments[i] = s.Commitment;
            last = rangeproofs[i].ToBytes();
        }
        watch.Stop();
        System.Console.WriteLine($"Took {watch.ElapsedMilliseconds} ms");
        System.Console.WriteLine($"And the bytes {Convert.ToBase64String(last)}");

        var label = Encoding.ASCII.GetBytes("bench");
        var failed = (int) iters;
        for (int i = 0; i < iters; i++) {
            var cp = new CompressedPoint(commitments[i].C.ToArray());
            var res = rangeproofs[i].VerifySingle(BulletProofGen.Default, Generator.Default, cp, 32, label);
            failed -= res ? 1 : 0;
        }
        System.Console.WriteLine($"{failed} failed");


        watch.Reset();
        watch.Start();
        System.Console.WriteLine($"Running {iters} iterations in parallel");
        var par = Parallel.For(0, iters, (i) => {
                var s = new SecretCommitmentInfo((uint) i);
                rangeproofs[i] = RangeProof.FromBytes(s.CreateRangeProof("bench").ToArray());
                commitments[i] = s.Commitment;
                last = rangeproofs[i].ToBytes();
            });
        System.Console.WriteLine($"And the bytes {Convert.ToBase64String(last)}");
        System.Console.WriteLine($"Is loop complete? {par.IsCompleted}");
        watch.Stop();
        System.Console.WriteLine($"Took {watch.ElapsedMilliseconds} ms");

        failed = (int) iters;
        for (int i = 0; i < iters; i++) {
            var cp = new CompressedPoint(commitments[i].C.ToArray());
            var res = rangeproofs[i].VerifySingle(BulletProofGen.Default, Generator.Default, cp, 32, label);
            failed -= res ? 1 : 0;
        }
        System.Console.WriteLine($"{failed} failed");
        return 0;
    }

}
