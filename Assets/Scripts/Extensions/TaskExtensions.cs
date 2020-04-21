/**
* TaskExtensions.cs
* Created by: João Borks [joao.borks@gmail.com]
* Created on: 1/17/2020 (en-US)
*/

using System;
using System.Threading;
using System.Threading.Tasks;

public static class TaskExtensions
{
    /// <summary>
    /// Fires a "Fire and Forget" Async Task. Use when there's no need to await for the Task's completion.
    /// </summary>
    /// <param name="task">The task to be fired</param>
    public static async void FireAndForgetAsync(this Task task)
    {
        try
        {
            await task;
        }
        catch
        {
            if (!task.IsCanceled)
                throw;
        }
    }

    /// <summary>
    /// Waits until the <paramref name="predicate"/> condition is evaluated to true. Similar to <see cref="UnityEngine.WaitUntil"/> on coroutines.
    /// </summary>
    /// <param name="predicate">Condition to wait for</param>
    /// <param name="token">Async <see cref="CancellationToken"/></param>
    public static async Task WaitUntilAsync(Func<bool> predicate, CancellationToken token = default)
    {
        try
        {
            while (!predicate() && !token.IsCancellationRequested)
                await Task.Yield();
        }
        catch (NullReferenceException)
        {
            throw;
        }
    }

    /// <summary>
    /// Waits while the <paramref name="predicate"/> condition is evaluated to true. Similar to <see cref="UnityEngine.WaitWhile"/> on coroutines.
    /// </summary>
    /// <param name="predicate">Condition to wait for</param>
    /// <param name="token">Async <see cref="CancellationToken"/></param>
    public static async Task WaitWhileAsync(Func<bool> predicate, CancellationToken token = default)
    {
        try
        {
            while (predicate() && !token.IsCancellationRequested)
                await Task.Yield();
        }
        catch (NullReferenceException)
        {
            throw;
        }
    }

    /// <summary>
    /// Delays the <paramref name="action"/> by the amount of <paramref name="milliseconds"/>. Similar to <see cref="UnityEngine.WaitForSeconds"/>
    /// </summary>
    /// <param name="action">Action to be delayed</param>
    /// <param name="milliseconds">Delay amount in milliseconds</param>
    /// <param name="token">Async <see cref="CancellationToken"/></param>
    public static async Task DelayActionAsync(Action action, int milliseconds, CancellationToken token = default)
    {
        try
        {
            await Task.Delay(milliseconds, token);
            action();
        }
        catch (NullReferenceException)
        {
            throw;
        }
    }

    /// <summary>
    /// Delays the <paramref name="action"/> by the amount of <paramref name="frames"/>.
    /// </summary>
    /// <param name="action">Action to be delayed</param>
    /// <param name="frames">Delay amount in frame count</param>
    /// <param name="token">Async <see cref="CancellationToken"/></param>
    public static async Task DelayActionFramesAsync(Action action, int frames, CancellationToken token = default)
    {
        try
        {
            for (int i = 0; i < frames && !token.IsCancellationRequested; i++)
                await Task.Yield();
            action();
        }
        catch
        {
            throw;
        }
    }
}