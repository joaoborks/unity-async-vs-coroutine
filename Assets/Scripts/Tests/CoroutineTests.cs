/**
 * CoroutineTests.cs
 * Created by: João Borks [joao.borks@gmail.com]
 * Created on: 4/21/2020 (en-US)
 */

using NUnit.Framework;
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

        IEnumerator RunBenchmarkCoroutine()
        {
            int simulationsRan = 0;
            var elapsedMilisecondsResults = new double[benchmarkManager.SimulationCount + benchmarkManager.InitialThreshold];
            var watch = new Stopwatch();

            while (simulationsRan < elapsedMilisecondsResults.Length)
            {
                watch.Reset();
                watch.Start();
                yield return null;
                watch.Stop();

                elapsedMilisecondsResults[simulationsRan] = watch.Elapsed.TotalMilliseconds;
                simulationsRan++;
            }

            elapsedMilisecondsResults = elapsedMilisecondsResults.Skip(benchmarkManager.InitialThreshold).ToArray();
            var result = elapsedMilisecondsResults.Average();
            Debug.Log($"Average elapsed time: {result:0.00}ms");
            JSONWriter.Write(elapsedMilisecondsResults, GetType().Name);
        }
    }
}