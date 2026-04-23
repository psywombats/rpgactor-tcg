using System;
using System.Threading.Tasks;
using UnityEngine;

public static class AsyncUtils
{
    public static async void Forget(this Task task)
    {
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public static async Task RunInBackgroundIfPossible(Func<Task> task)
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            await task.Invoke();
        }
        else
        {
            await Task.Run(() => task);
        }
    }
    
    public static async Task RunInBackgroundIfPossible(Action action)
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            action.Invoke();
        }
        else
        {
            await Task.Run(action);
        }
    }
}