/**
 * CoroutineTests.cs
 * Created by: João Borks [joao.borks@gmail.com]
 * Created on: 4/21/2020 (en-US)
 */

using NUnit.Framework;
using Profiling;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;
using Debug = UnityEngine.Debug;

namespace Tests
{
    public class CoroutineTests
    {
        BenchmarkSettings benchmarkManager;
        EmptyBehaviour behaviour;

        [SetUp]
        public void Setup()
        {
            benchmarkManager = Resources.Load<BenchmarkSettings>("BenchmarkSettings");
            behaviour = new GameObject("Coroutine Runner").AddComponent<EmptyBehaviour>();
        }

        [UnityTest]
        public IEnumerator SimpleTest()
        {
            yield return behaviour.StartCoroutine(RunBenchmarkCoroutine());
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
                        behaviour.StartCoroutine(EndNextFrameCoroutine());

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
            JSONWriter.WriteToFile(snapshotResults, "StressCoroutine");
        }

        IEnumerator RunBenchmarkCoroutine()
        {
            int simulationsRan = 0;
            var elapsedMilisecondsResults = new double[benchmarkManager.SimulationCount + benchmarkManager.InitialThreshold];
            var watch = new Stopwatch();

            while (simulationsRan < elapsedMilisecondsResults.Length)
            {
                watch.Restart();
                yield return null;
                watch.Stop();

                elapsedMilisecondsResults[simulationsRan] = watch.Elapsed.TotalMilliseconds;
                simulationsRan++;
            }

            elapsedMilisecondsResults = elapsedMilisecondsResults.Skip(benchmarkManager.InitialThreshold).ToArray();
            var result = elapsedMilisecondsResults.Average();
            Debug.Log($"Average elapsed time: {result:0.00}ms");
            JSONWriter.WriteToFile(elapsedMilisecondsResults, "SimpleCoroutine");
        }

        IEnumerator EndNextFrameCoroutine()
        {
            yield return null;
        }
    }
}