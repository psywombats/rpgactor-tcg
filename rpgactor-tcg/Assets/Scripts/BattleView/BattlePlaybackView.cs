using System.Threading.Tasks;
using UnityEngine;

namespace RpgActorTGC
{
    public class BattlePlaybackView : MonoBehaviour
    {
        [SerializeField] private PartyView player1View;
        [SerializeField] private PartyView player2View;

        private BattleModel battle;

        public async Task PlayBattleAsync(Party player1, Party player2)
        {
            battle = new BattleModel(player1, player2);
        }
    }
}