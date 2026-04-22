using RpgActorTGC;
using UnityEngine;

[CreateAssetMenu(fileName = "Constants", menuName = "Data/Constants")]
public class ConstantsData : ScriptableObject
{
    public static ConstantsData Instance => DBManager.Instance.Constants;
    
    [SerializeField] public int deckCount = 3;
    [SerializeField] public int npcCount = 30;
    [SerializeField, Tooltip("Comma-separated list")] public string rarityUnlockThresholds = "1,2";
    [Space]
    [SerializeField] public SpritesheetFormatData rpgactorSpriteFormat;
    [SerializeField] public SpritesheetData defaultNetworkFallbackSprite;
    [Space]
    [SerializeField] public OpponentNameData oppoNames;
    [Space]
    [SerializeField] public EvolutionRunner<DeckSolution>.EvolutionSettings evolutionSettings;
}