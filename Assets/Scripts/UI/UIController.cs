/**
 * UIController.cs
 * Created by: João Borks [joao.borks@gmail.com]
 * Created on: 4/20/2020 (en-US)
 */

using System;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField]
    BenchmarkManager benchmarkManager;

    public void Awake()
    {
        benchmarkManager.Init();
    }

    public void Action()
    {
        benchmarkManager.RunSimpleBenchmarkAsync();
    }
}