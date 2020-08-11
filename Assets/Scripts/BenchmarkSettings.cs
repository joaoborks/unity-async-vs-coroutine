/**
* BenchmarkManager.cs
* Created by: João Borks [joao.borks@gmail.com]
* Created on: 4/20/2020 (en-US)
*/

using UnityEngine;

[CreateAssetMenu(fileName = "BenchmarkSettings", menuName = "Benchmark/Settings")]
public class BenchmarkSettings : ScriptableObject
{
    public int InitialThreshold => initialThreshold;
    public int SimulationCount => simulationCount;

    [SerializeField, Range(100, 10000)]
    int simulationCount = 1000;
    [SerializeField, Range(0, 20)]
    int initialThreshold = 10;
}