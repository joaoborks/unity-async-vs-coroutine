/**
* CoroutineBenchmark.cs
* Created by: João Borks [joao.borks@gmail.com]
* Created on: 4/20/2020 (en-US)
*/

using System;
using System.Collections;
using UnityEngine;

public class CoroutineBenchmark
{
    MonoBehaviour monoBehaviour;

    public CoroutineBenchmark()
    {
        monoBehaviour = new GameObject("Coroutine Runner").AddComponent<EmptyBehaviour>();
    }

    ~CoroutineBenchmark()
    {
        GameObject.Destroy(monoBehaviour.gameObject);
    }

    public void SimpleCoroutineBenchmark(Action onComplete)
    {
        monoBehaviour.StartCoroutine(SimpleRoutine(onComplete));
    }

    IEnumerator SimpleRoutine(Action onComplete)
    {
        string a = "a";
        a.Replace("a", "b");
        yield return null;
        onComplete.Invoke();
    }
}