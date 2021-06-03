using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CoroutineTaskReturnWrapper<T>
{
    public Task<T> ProcessingTask { get; set; }
    public T Result { get; set; }

    public CoroutineTaskReturnWrapper(Task<T> task)
    {
        ProcessingTask = task;
    }

    public IEnumerator YieldRoutine()
    {
        yield return ThreadDoctor.YieldTask<T>(ProcessingTask);
        Result = ProcessingTask.Result;
    }
}
