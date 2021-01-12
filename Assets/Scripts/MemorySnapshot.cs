using System;

namespace Profiling
{
    public struct MemorySnapshot
    {
        public long TotalMemory;
        public long GCMemory;
        public long GCAlloc;

        public static MemorySnapshot operator -(MemorySnapshot a, MemorySnapshot b)
        {
            return new MemorySnapshot
            {
                TotalMemory = Math.Abs(a.TotalMemory - b.TotalMemory),
                GCMemory = Math.Abs(a.GCMemory - b.GCMemory),
                GCAlloc = Math.Abs(a.GCAlloc - b.GCAlloc)
            };
        }

        public override string ToString() => $"Total Memory: {TotalMemory} bytes | GC Used Memory: {GCMemory} bytes | GC Allocated In Frame: {GCAlloc} bytes";
    }
}