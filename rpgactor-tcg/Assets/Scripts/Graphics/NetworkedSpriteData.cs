using System;
using System.Threading.Tasks;
using UnityEngine;

public class NetworkedSpriteData : SpritesheetData
{
    private SpritesheetData fallback;
    private SpritesheetFormatData networkFormat;
    private string url;
    private string spriteName;
    private bool isLoadingInitiated;
    
    public static NetworkedSpriteData Create(SpritesheetFormatData format, string url, string spriteName, 
        SpritesheetData fallback, bool autoLoad = false)
    {
        var instance = CreateInstance<NetworkedSpriteData>();
        instance.fallback = fallback;
        instance.url = url;
        instance.spriteName = spriteName;
        instance.networkFormat = format;
        instance.format = fallback.Format;
        if (autoLoad) instance.LoadAsync().Forget();
        return instance;
    }

    public override Sprite GetSprite(OrthoDir dir, int step)
    {
        return SpritesByName != null ? base.GetSprite(dir, step) : fallback.GetSprite(dir, step);
    }

    public override void OnShow(Action dataChangedCallback = null)
    {
        base.OnShow(dataChangedCallback);
        if (!isLoadingInitiated)
        {
            LoadAndUpdateAsync(dataChangedCallback).Forget();
        }
    }

    private async Task LoadAndUpdateAsync(Action dataChangedCallback)
    {
        await LoadAsync();
        dataChangedCallback?.Invoke();
    }
    
    private async Task LoadAsync()
    {
        isLoadingInitiated = true;
        var tex = await HTTPManager.Instance.FetchNetTextureAsync(url, networkFormat);
        if (tex != null)
        {
            tex.name = spriteName;
            PopulateFromTexture(tex, networkFormat, spriteName);
        }
    }
}