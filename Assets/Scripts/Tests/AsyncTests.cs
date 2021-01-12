/**
* AsyncTests.cs
* Created by: João Borks [joao.borks@gmail.com]
* Created on: 4/21/2020 (en-US)
*/

using NUnit.Framework;
using Profiling;
using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TestTools;
using Debug = UnityEngine.Debug;

namespace Tests
{
    public class AsyncTests
    {
        BenchmarkSettings benchmarkManager;

        [SetUp]
        public void Setup() => benchmarkManager = Resources.Load<BenchmarkSettings>("BenchmarkSettings");

        [UnityTest]
        public IEnumerator SimpleTest()
        {
            var a = SimpleTaskAsync();
            while (!a.IsCompleted)
                yield return null;
        }

        [UnityTest]
        public IEnumerator StressTest()
        {
            int simulationsRan = 0;
            var snapshotResults = new MemorySnapshot[benchmarkManager.SimulationCount + benchmarkManager.InitialThreshold];
            var length = snapshotResults.Length;
            
            using (var profiler = new MemoryProfiler())
            {
                while (simulationsRan < length)
                {
                    yield return null;
                    profiler.GetMemorySnapshot(out var first);

                    for (int i = 0; i < 100; i++)
                        _ = EndNextFrameAsync();

                    profiler.GetMemorySnapshot(out var final);
                    yield return null;
                    yield return null;
                    snapshotResults[simulationsRan] = final - first;
                    simulationsRan++;
                }
            }

            snapshotResults = snapshotResults.Skip(benchmarkManager.InitialThreshold).ToArray();
            var averageSnapshot = new MemorySnapshot
            {
                TotalMemory = (long)snapshotResults.Select(s => s.TotalMemory).Average(),
                GCMemory = (long)snapshotResults.Select(s => s.GCMemory).Average(),
                GCAlloc = (long)snapshotResults.Select(s => s.GCAlloc).Average()
            };
            Debug.Log(averageSnapshot);
            JSONWriter.WriteToFile(snapshotResults, "StressAsync");
        }

        async Task SimpleTaskAsync(CancellationToken token = default)
        {
            int simulationsRan = 0;
            var elapsedMilisecondsResults = new double[benchmarkManager.SimulationCount + benchmarkManager.InitialThreshold];
            var watch = new Stopwatch();

            try
            {
                while (simulationsRan < elapsedMilisecondsResults.Length && !token.IsCancellationRequested)
                {
                    watch.Restart();
                    await Task.Yield();
                    watch.Stop();

                    elapsedMilisecondsResults[simulationsRan] = watch.Elapsed.TotalMilliseconds;
                    simulationsRan++;
                }

                elapsedMilisecondsResults = elapsedMilisecondsResults.Skip(benchmarkManager.InitialThreshold).ToArray();
                var result = elapsedMilisecondsResults.Average();
                Debug.Log($"Average elapsed time: {result:0.00}ms");
                JSONWriter.WriteToFile(elapsedMilisecondsResults, "SimpleAsync");
            }
            catch (Exception e)
            {
                // Swallow if task got cancelled
                if (e.GetType().IsAssignableFrom(typeof(TaskCanceledException)))
                    return;
                throw e;
            }
        }

        async Task EndNextFrameAsync()
        {
            await Task.Yield();
        }
    }
}
