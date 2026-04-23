using Effekseer;
using UnityEngine;

namespace RpgActorTGC
{
    [DatabaseIndexed]
    [CreateAssetMenu(fileName = "ElementInfo", menuName = "RPG/Element")]
    public class ElementInfo : ScriptableObject, IDatabaseKeyable
    {
        [SerializeField] private Element associatedElement;
        [SerializeField] private string elementName;
        [SerializeField] private string description;
        [Space]
        [SerializeField] private Color primaryColor;
        [SerializeField] private Color cardTint;
        [SerializeField] private Sprite icon;
        [SerializeField] private EffekseerEffectAsset castAnim;

        public string Key => associatedElement.ToString();
        public string Description => description;
        public string ElementName => elementName;
        public Color PrimaryColor => primaryColor;
        public Color CardTint => cardTint;
        public Sprite Icon => icon;
        public EffekseerEffectAsset CastAnim => castAnim;

        public override string ToString() => Key;
    }
}