using UnityEngine;

[CreateAssetMenu(fileName = "Constants", menuName = "Data/Constants")]
public class ConstantsData : ScriptableObject
{
    public static ConstantsData Instance => DBManager.Instance.Constants;
    
    [SerializeField] public int deckCount = 3;
    [Space]
    [SerializeField] public SpritesheetFormatData rpgactorSpriteFormat;
    [SerializeField] public SpritesheetData defaultNetworkFallbackSprite;
}