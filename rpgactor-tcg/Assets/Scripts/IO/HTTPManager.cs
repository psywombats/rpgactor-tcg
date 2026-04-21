using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class HTTPManager : SingletonBehaviour<HTTPManager>
{
    private Dictionary<string, Texture2D> texturesByURL = new();
    private Dictionary<string, Task<Texture2D>> fetchTasks = new();

    public async Task<(UnityWebRequest.Result result, string jsonString)> FetchJSONStringAsync(string url)
    {
        var req = UnityWebRequest.Get(url);
        await req.SendWebRequest();
        return req.result != UnityWebRequest.Result.Success 
            ? (req.result, null) 
            : (req.result, req.downloadHandler.text);
    }
    
    public Task<Texture2D> FetchNetTextureAsync(string url, SpritesheetFormatData networkFormat)
    {
        if (!texturesByURL.TryGetValue(url, out var tex))
        {
            if (!fetchTasks.ContainsKey(url))
            {
                fetchTasks[url] = FetchAndCacheRemoteNetTextureAsync(url, networkFormat);
            }
            return fetchTasks[url];
        }
        return Task.FromResult(tex);
    }

    private async Task<Texture2D> FetchAndCacheRemoteNetTextureAsync(string url, SpritesheetFormatData networkFormat)
    {
        var result = await FetchRemoteNetTextureAsync(url, networkFormat);
        texturesByURL.Add(url, result);
        return result;
    }

    private async Task<Texture2D> FetchRemoteNetTextureAsync(string url, SpritesheetFormatData networkFormat)
    {
        var req = UnityWebRequest.Get(url);
        await req.SendWebRequest();
 
        if (req.result != UnityWebRequest.Result.Success) 
        {
            Debug.LogWarning($"Could not load image from {url}; using fallback");
            return null;
        }

        try
        {
            var imageBytes = req.downloadHandler.data;
            var tex = new Texture2D(networkFormat.SheetSize.x, networkFormat.SheetSize.y, TextureFormat.RGBA32, false);
            tex.LoadImage(imageBytes);
            tex.filterMode = FilterMode.Point;
            return tex;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return null;
        }
    }
}