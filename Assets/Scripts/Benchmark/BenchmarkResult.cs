/**
* BenchmarkResult.cs
* Created by: João Borks [joao.borks@gmail.com]
* Created on: 4/21/2020 (en-US)
*/

using System;

public readonly struct BenchmarkResult : IEquatable<BenchmarkResult>
{
    public static BenchmarkResult Zero => new BenchmarkResult();

    public double ElapsedMiliseconds { get; }
    public double AllocatedMemory { get; }

    public BenchmarkResult(double elapsedMiliseconds, double allocatedMemory)
    {
        ElapsedMiliseconds = elapsedMiliseconds;
        AllocatedMemory = allocatedMemory;
    }

    public override string ToString() => $"ElapsedMiliseconds: {ElapsedMiliseconds}, AllocatedMemory: {AllocatedMemory}";

    public override bool Equals(object obj) => obj is BenchmarkResult result && Equals(result);

    public override int GetHashCode()
    {
        int hashCode = 619404756;
        hashCode = hashCode * -1521134295 + ElapsedMiliseconds.GetHashCode();
        hashCode = hashCode * -1521134295 + AllocatedMemory.GetHashCode();
        return hashCode;
    }

    public bool Equals(BenchmarkResult other) => ElapsedMiliseconds == other.ElapsedMiliseconds && AllocatedMemory == other.AllocatedMemory;

    public static bool operator ==(BenchmarkResult left, BenchmarkResult right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(BenchmarkResult left, BenchmarkResult right)
    {
        return !(left == right);
    }
}