/**
* AsyncTests.cs
* Created by: João Borks [joao.borks@gmail.com]
* Created on: 4/21/2020 (en-US)
*/

using NUnit.Framework;
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

        async Task SimpleTaskAsync(CancellationToken token = default)
        {
            int simulationsRan = 0;
            var elapsedMilisecondsResults = new double[benchmarkManager.SimulationCount + benchmarkManager.InitialThreshold];
            var watch = new Stopwatch();

            try
            {
                while (simulationsRan < elapsedMilisecondsResults.Length && !token.IsCancellationRequested)
                {
                    watch.Reset();
                    watch.Start();
                    await Task.Yield();
                    watch.Stop();

                    elapsedMilisecondsResults[simulationsRan] = watch.Elapsed.TotalMilliseconds;
                    simulationsRan++;
                }

                elapsedMilisecondsResults = elapsedMilisecondsResults.Skip(benchmarkManager.InitialThreshold).ToArray();
                var result = elapsedMilisecondsResults.Average();
                Debug.Log($"Average elapsed time: {result:0.00}ms");
                JSONWriter.Write(elapsedMilisecondsResults, GetType().Name);
            }
            catch (Exception e)
            {
                // Swallow if task got cancelled
                if (e.GetType().IsAssignableFrom(typeof(TaskCanceledException)))
                    return;
                throw e;
            }
        }
    }
}
