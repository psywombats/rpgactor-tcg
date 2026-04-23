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
        public string DisplayName => displayName ??= Data.displayName.Replace(".bsky.social", "");
        
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

            if (Data.stats?.tcg != null)
            {
                // maxes out at 10 if it exists, so all later values must be >10 so as to not penalize those that opt in
                return Data.stats.tcg.DistanceFromCard(card);
            }
            
            var matchingClass = card.Data.actorPreferredClasses.FirstOrDefault(tryClass => Classes.Any(tryClass.Contains));
            if (matchingClass != null)
            {
                // score later classes in the data list higher
                return 10 + card.Data.actorPreferredClasses.IndexOf(matchingClass);
            }

            if (Data.stats?.BestStats != null && Data.stats.BestStats.Contains(card.Data.actorPreferredStat))
            {
                return 11;
            }

            if (Data.stats != null)
            {
                // might as well prioritize people engaged with the rpg.actor system
                // the more stats you have, the closer you are considered to every card
                var dist = 12;
                dist += Data.stats.rmmz == null ? 1 : 0;
                dist += Data.stats.dnd == null ? 1 : 0;
                return dist;
                // TODO: more+better scoring criteria
            }
            
            return 15;
        }
    }
}