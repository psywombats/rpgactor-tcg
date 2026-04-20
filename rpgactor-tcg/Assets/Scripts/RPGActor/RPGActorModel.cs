using System.Collections.Generic;
using System.Linq;

namespace RpgActorTGC
{
    public class RPGActorModel
    {
        public RPGActorData Data { get; }

        public string DID => Data.did;
        public IEnumerable<string> Classes => Data.Classes;
        
        private string asciiName;
        public string AsciiName => asciiName ??= DisplayName.ToAscii();

        private string displayName;
        public string DisplayName => displayName ??= Data.displayName.Replace(".bluesky.social", "");
        
        public CharacterCard Card { get; set; }
        public NetworkedSpriteData Sprite { get; private set; }

        public RPGActorModel(RPGActorData data)
        {
            Data = data;
            Sprite = NetworkedSpriteData.Create(
                format: DBManager.Instance.Constants.rpgactorSpriteFormat,
                url: data.sprite.url, 
                spriteName: AsciiName, 
                fallback: DBManager.Instance.Constants.defaultNetworkFallbackSprite);
        }
        
        public int DistanceFromCard(CharacterCard card)
        {
            if (Data.did == card.Data.actorPreferredDID)
            {
                return 0;
            }
            if (AsciiName == card.Data.actorPreferredName)
            {
                return 1;
            }
            var matchingClass = card.Data.actorPreferredClasses.FirstOrDefault(@class => Classes.Contains(@class));
            if (matchingClass != null)
            {
                // score later classes in the data list higher
                return 2 + card.Data.actorPreferredClasses.IndexOf(matchingClass);
            }
            
            // TODO: better scoring criteria
            return 10;
        }
    }
}