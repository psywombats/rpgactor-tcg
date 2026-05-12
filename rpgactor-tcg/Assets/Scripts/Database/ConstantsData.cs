using MapElitesTGC;
using RpgActorTGC;
using UnityEngine;

[CreateAssetMenu(fileName = "Constants", menuName = "Data/Constants")]
public class ConstantsData : ScriptableObject
{
    public static ConstantsData Instance => DBManager.Instance.Constants;
    
    [SerializeField] public int deckCount = 3;
    [SerializeField] public int winningDeckCount = 4;
    [Space]
    [SerializeField] public int frontrunnerCount = 2;
    [SerializeField] public int scientistCount = 2;
    [SerializeField] public int copycatCount = 5;
    [SerializeField] public int contentCount = 12;
    [SerializeField] public int chaffCount = 9;
    [SerializeField, Tooltip("Comma-separated list")] public string rarityUnlockThresholds = "1,2";
    [Space]
    [SerializeField] public SpritesheetFormatData rpgactorSpriteFormat;
    [SerializeField] public SpritesheetData defaultNetworkFallbackSprite;
    [Space]
    [SerializeField] public OpponentNameData oppoNames;
    [Space]
    [SerializeField] public MapEliteRunner.GenerationSettings evolutionSettings;
}