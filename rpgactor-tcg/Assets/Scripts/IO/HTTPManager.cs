using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public class HTTPManager : SingletonBehaviour<HTTPManager>
{
    public HttpClient Client { get; private set; }

    private Dictionary<string, Texture2D> texturesByURL = new();
    private Dictionary<string, Task<Texture2D>> fetchTasks = new();

    protected override void Init()
    {
        base.Init();
        Client = new HttpClient();
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
        var blob = await Client.GetAsync(url);
        if (!blob.IsSuccessStatusCode)
        {
            Debug.LogWarning($"Could not load image from {url}; using fallback");
            return null;
        }

        try
        {
            var imageBytes = await blob.Content.ReadAsByteArrayAsync();
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