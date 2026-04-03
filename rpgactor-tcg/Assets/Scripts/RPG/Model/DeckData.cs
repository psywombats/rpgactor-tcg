using UnityEngine;

namespace RpgActorTGC
{
    [CreateAssetMenu(fileName = "Deck", menuName = "Data/DeckData")]
    public class DeckData : ScriptableObject
    {
        [SerializeField] public CharacterData backChara;
        [Space]
        [SerializeField] public CharacterData leftChara;
        [SerializeField] public CharacterData centerChara;
        [SerializeField] public CharacterData rightChara;
    }
}
