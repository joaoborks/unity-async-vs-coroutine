/**
 * AsyncVsCoroutineTests.cs
 * Created by: João Borks [joao.borks@gmail.com]
 * Created on: 4/21/2020 (en-US)
 */

using Cysharp.Threading.Tasks;
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
    public class AsyncVsCoroutineTests
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
        public IEnumerator SimpleCoroutineTest()
        {
            yield return behaviour.StartCoroutine(simpleCoroutine());

            IEnumerator simpleCoroutine()
            {
                int simulationsRan = 0;
                CreateSimulationArray<double>(out var elapsedMilisecondsResults);
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
                ConcludeTest(elapsedMilisecondsResults, "SimpleCoroutine");
            }
        }

        [UnityTest]
        public IEnumerator SimpleAsyncTest()
        {
            var task = simpleTaskAsync();
            yield return new WaitUntil(() => task.IsCompleted);

            async Task simpleTaskAsync(CancellationToken token = default)
            {
                int simulationsRan = 0;
                CreateSimulationArray<double>(out var elapsedMilisecondsResults);
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
                    ConcludeTest(elapsedMilisecondsResults, "SimpleAsync");
                }
                catch (Exception e) when (!(e is TaskCanceledException))
                {
                    throw e;
                }
            }
        }

        [UnityTest]
        public IEnumerator SimpleUniTaskTest()
        {
            return simpleUniTaskAsync().ToCoroutine();

            async UniTask simpleUniTaskAsync()
            {
                int simulationsRan = 0;
                CreateSimulationArray<double>(out var elapsedMilisecondsResults);
                var watch = new Stopwatch();

                try
                {
                    while (simulationsRan < elapsedMilisecondsResults.Length)
                    {
                        watch.Restart();
                        await UniTask.Yield();
                        watch.Stop();

                        elapsedMilisecondsResults[simulationsRan] = watch.Elapsed.TotalMilliseconds;
                        simulationsRan++;
                    }

                    elapsedMilisecondsResults = elapsedMilisecondsResults.Skip(benchmarkManager.InitialThreshold).ToArray();
                    var result = elapsedMilisecondsResults.Average();
                    Debug.Log($"Average elapsed time: {result:0.00}ms");
                    ConcludeTest(elapsedMilisecondsResults, "SimpleUniTask");
                }
                catch (Exception e) when (!(e is OperationCanceledException))
                {
                    throw e;
                }
            }
        }

        [UnityTest]
        public IEnumerator StressCoroutineTest()
        {
            yield return behaviour.StartCoroutine(stressTestRoutine());

            static IEnumerator endNextFrameCoroutine()
            {
                yield return null;
            }

            IEnumerator stressTestRoutine()
            {
                int simulationsRan = 0;
                CreateSimulationArray<MemorySnapshot>(out var snapshotResults);
                var length = snapshotResults.Length;
                var coroutines = new Coroutine[100];
                int i;

                using (var profiler = new MemoryProfiler())
                {
                    while (simulationsRan < length)
                    {
                        yield return null;
                        profiler.GetMemorySnapshot(out var first);

                        for (i = 0; i < 100; i++)
                            coroutines[i] = behaviour.StartCoroutine(endNextFrameCoroutine());

                        profiler.GetMemorySnapshot(out var final);
                        yield return null;
                        for (i = 0; i < 100; i++)
                            behaviour.StopCoroutine(coroutines[i]);
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
                ConcludeTest(snapshotResults, "StressCoroutine");
            }
        }

        [UnityTest]
        public IEnumerator StressAsyncTest()
        {
            yield return behaviour.StartCoroutine(stressTestRoutine());

            static async Task endNextFrameAsync() => await Task.Yield();

            IEnumerator stressTestRoutine()
            {
                int simulationsRan = 0;
                CreateSimulationArray<MemorySnapshot>(out var snapshotResults);
                var length = snapshotResults.Length;
                var tasks = new Task[100];
                int i;

                using (var profiler = new MemoryProfiler())
                {
                    while (simulationsRan < length)
                    {
                        yield return null;
                        profiler.GetMemorySnapshot(out var first);

                        for (i = 0; i < 100; i++)
                            tasks[i] = endNextFrameAsync();

                        profiler.GetMemorySnapshot(out var final);
                        yield return null;
                        Task.WaitAll(tasks);
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
                ConcludeTest(snapshotResults, "StressAsync");
            }
        }

        [UnityTest]
        public IEnumerator StressUniTaskTest()
        {
            yield return behaviour.StartCoroutine(stressTestRoutine());

            static async UniTask endNextFrameAsync() => await UniTask.Yield();

            IEnumerator stressTestRoutine()
            {
                int simulationsRan = 0;
                CreateSimulationArray<MemorySnapshot>(out var snapshotResults);
                var length = snapshotResults.Length;
                var uniTasks = new UniTask[100];

                using (var profiler = new MemoryProfiler())
                {
                    while (simulationsRan < length)
                    {
                        yield return null;
                        profiler.GetMemorySnapshot(out var first);

                        for (int i = 0; i < 100; i++)
                            uniTasks[i] = endNextFrameAsync();

                        profiler.GetMemorySnapshot(out var final);
                        yield return null;
                        UniTask.WhenAll(uniTasks).GetAwaiter().GetResult();
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
                ConcludeTest(snapshotResults, "StressUniTask");
            }
        }

        void CreateSimulationArray<T>(out T[] simulationArray) => simulationArray = new T[benchmarkManager.SimulationCount + benchmarkManager.InitialThreshold];

        void ConcludeTest<T>(T[] results, string testLabel) => JSONWriter.WriteToFile(results, testLabel);
    }
}