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
}