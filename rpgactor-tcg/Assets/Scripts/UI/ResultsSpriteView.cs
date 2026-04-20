using UnityEngine;

namespace RpgActorTGC
{
    public class ResultsSpriteView : MonoBehaviour
    {
        [SerializeField] private CharaModelView charaModel;
        [SerializeField] private GameObject leaderArea;

        public void Populate(CharacterCard card)
        {
            charaModel.Sprite = card.Sprite;
            leaderArea.SetActive(card.IsLeader);
        }
    }
}