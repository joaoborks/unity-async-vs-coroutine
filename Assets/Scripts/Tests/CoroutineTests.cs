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
            yield return behaviour.StartCoroutine(RunBenchmarkCoroutine(SimpleTaskRoutine(), behaviour));
        }

        IEnumerator SimpleTaskRoutine()
        {
            yield return null;
        }

        IEnumerator RunBenchmarkCoroutine(IEnumerator benchmarkCoroutine, MonoBehaviour behaviour)
        {
            if (benchmarkCoroutine == null)
                yield break;

            int simulationsRan = 0;
            var elapsedMilisecondsResults = new double[benchmarkManager.SimulationCount];
            var watch = new Stopwatch();

            while (simulationsRan < benchmarkManager.SimulationCount)
            {
                watch.Reset();
                watch.Start();
                yield return behaviour.StartCoroutine(benchmarkCoroutine);
                watch.Stop();

                elapsedMilisecondsResults[simulationsRan] = watch.Elapsed.TotalMilliseconds;
                simulationsRan++;
            }

            var result = elapsedMilisecondsResults.Average();
            Debug.Log($"Average elapsed time: {result:0.00}ms");
        }
    }
}