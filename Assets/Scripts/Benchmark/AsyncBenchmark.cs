/**
* AsyncBenchmark.cs
* Created by: João Borks [joao.borks@gmail.com]
* Created on: 4/20/2020 (en-US)
*/

using System;
using System.Threading;
using System.Threading.Tasks;

public class AsyncBenchmark
{
    public void SimpleAsyncBenchmark(Action onComplete)
    {
        _ = SimpleTaskAsync(onComplete);
    }

    async Task SimpleTaskAsync(Action onComplete, CancellationToken token = default)
    {
        string a = "a";
        a.Replace("a", "b");
        await Task.Yield();
        onComplete.Invoke();
    }
}