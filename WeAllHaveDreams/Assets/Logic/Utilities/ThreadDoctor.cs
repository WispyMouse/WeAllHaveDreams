using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class ThreadDoctor
{
    public static IEnumerator YieldTask(Task toYield)
    {
        if (toYield.Status == TaskStatus.Created)
        {
            toYield.Start();
        }

        while (!toYield.IsCompleted)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    public static IEnumerator YieldTask<T>(Task<T> toYield)
    {
        if (toYield.Status == TaskStatus.Created)
        {
            toYield.Start();
        }

        while (!toYield.IsCompleted)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    public static async Task AwaitIEnumerator(IEnumerator toAwait)
    {
        while (toAwait.MoveNext())
        {
            await Task.Delay(0);
        }
    }

    public static IEnumerator YieldAsyncOperation(AsyncOperation toYield)
    {
        while (!toYield.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
    }
}
