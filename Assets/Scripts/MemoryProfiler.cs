using System;
using Unity.Profiling;

namespace Profiling
{
    public class MemoryProfiler : IDisposable
    {
        public readonly ProfilerRecorder TotalMemoryRecorder;
        public readonly ProfilerRecorder GCMemoryRecorder;
        public readonly ProfilerRecorder GCAllocRecorder;

        bool disposedValue;

        public MemoryProfiler()
        {
            TotalMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Used Memory", 1, ProfilerRecorderOptions.Default | ProfilerRecorderOptions.StartImmediately);
            GCMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Used Memory", 1, ProfilerRecorderOptions.Default | ProfilerRecorderOptions.StartImmediately);
            GCAllocRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocated In Frame", 1, ProfilerRecorderOptions.Default | ProfilerRecorderOptions.StartImmediately);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void GetMemorySnapshot(out MemorySnapshot snapshot)
        {
            snapshot = new MemorySnapshot()
            {
                TotalMemory = TotalMemoryRecorder.CurrentValue,
                GCMemory = GCMemoryRecorder.CurrentValue,
                GCAlloc = GCAllocRecorder.CurrentValue
            };
        }

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    TotalMemoryRecorder.Dispose();
                    GCMemoryRecorder.Dispose();
                    GCAllocRecorder.Dispose();
                }
                disposedValue = true;
            }
        }
    }
}