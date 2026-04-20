using System;
using UnityEngine;

public class CharaModelView : MonoBehaviour
{
    private const float BaseStepsPerSecond = 2f;
    
    [SerializeField] private SpritesheetData defaultSprite;
    [SerializeField] private bool defaultAnimates = true;
    [SerializeField] private OrthoDir defaultDir = OrthoDir.South;

    private float StepsPerSecond => BaseStepsPerSecond;

    public event Action<Sprite> OnSpriteUpdated;

    private SpritesheetData sprite;
    public SpritesheetData Sprite
    {
        get => sprite;
        set
        {
            stepIndex = 0;
            sprite = value;
            sprite?.OnShow(UpdateForNewSprite);
            UpdateForNewSprite();
        }
    }

    private OrthoDir facing;
    public OrthoDir Facing
    {
        get => facing;
        set
        {
            facing = value; 
            UpdateForNewSprite();
        }
    }
    
    private int stepIndex;
    public int StepIndex
    {
        get => stepIndex;
        private set
        {
            stepIndex = value;
            UpdateForNewSprite();
        }
    }

    private bool animates;
    public bool Animates
    {
        get => animates;
        set
        {
            animates = value;
            if (!animates) StepIndex = 0;
        }
    }
    
    public void Awake()
    {
        Sprite = defaultSprite;
        Facing = defaultDir;
        Animates = defaultAnimates;
        StepIndex = 0;
    }

    public void SetSpritesheetByTag(string key)
    {
        Sprite = DBManager.Instance.Get<SpritesheetData>(key);
    }
    
    public void Update() 
    {
        if (Sprite != null)
        {
            var newX = 0;
            if (Animates) 
            {
                var elapsed = Time.time;
                newX = Mathf.FloorToInt(elapsed * StepsPerSecond) % Sprite.StepCount;
            }
            if (StepIndex != newX) 
            {
                StepIndex = newX;
            }
        }
    }

    public Sprite GetSpriteForCurrentFrame()
    {
        if (Sprite == null) return null;
        return Sprite.GetSprite(Facing, StepIndex);
    }

    private void UpdateForNewSprite()
    {
        OnSpriteUpdated?.Invoke(GetSpriteForCurrentFrame());
    }
}
