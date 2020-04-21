/**
* BenchmarkManager.cs
* Created by: João Borks [joao.borks@gmail.com]
* Created on: 4/20/2020 (en-US)
*/

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

[CreateAssetMenu(fileName = "BenchmarkManager", menuName = "Benchmark/Manager")]
public class BenchmarkManager : ScriptableObject
{
    [SerializeField, Range(100, 10000)]
    int simulationCount = 1000;

    CoroutineBenchmark coroutineBenchmark;
    AsyncBenchmark asyncBenchmark;

    public void Init()
    {
        coroutineBenchmark = new CoroutineBenchmark();
        asyncBenchmark = new AsyncBenchmark();
    }

    public async void RunSimpleBenchmarkAsync(CancellationToken token = default)
    {
        var asyncResult = await RunBenchmarkAsync(asyncBenchmark.SimpleAsyncBenchmark);
        var coroutineResult = await RunBenchmarkAsync(coroutineBenchmark.SimpleCoroutineBenchmark);
        Debug.Log($"Async results: {asyncResult}\nCoroutine results: {coroutineResult}");
    }

    public async Task<BenchmarkResult> RunBenchmarkAsync(Action<Action> benchmarkAction, CancellationToken token = default)
    {
        if (benchmarkAction == null)
            return BenchmarkResult.Zero;

        int simulationsRan = 0;
        bool moveNext;
        long initMem;
        var elapsedMilisecondsResults = new long[simulationCount];
        var allocatedMemoryResults = new long[simulationCount];
        var watch = new Stopwatch();

        // Used to track the action's completion
        void onComplete() => moveNext = true;

        try
        {
            while (simulationsRan < simulationCount && !token.IsCancellationRequested)
            {
                // Reset execution state
                moveNext = false;
                watch.Reset();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                initMem = GC.GetTotalMemory(false);
                watch.Start();

                // Execute benchmark action
                benchmarkAction.Invoke(onComplete);

                while (!moveNext && !token.IsCancellationRequested)
                    await Task.Yield();

                watch.Stop();
                // Collect results
                elapsedMilisecondsResults[simulationsRan] = watch.ElapsedMilliseconds;
                allocatedMemoryResults[simulationsRan] = GC.GetTotalMemory(false) - initMem;

                simulationsRan++;
            }

            return new BenchmarkResult(elapsedMilisecondsResults.Average(), allocatedMemoryResults.Average());
        }
        catch (Exception e)
        {
            // Swallow if task got cancelled
            if (e.GetType().IsAssignableFrom(typeof(TaskCanceledException)))
                return BenchmarkResult.Zero;
            throw e;
        }
        finally
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}