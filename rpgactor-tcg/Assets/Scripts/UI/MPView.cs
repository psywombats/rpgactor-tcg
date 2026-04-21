using System.Linq;
using UnityEngine;

namespace RpgActorTGC
{
    public class MPView : MonoBehaviour
    {
        [SerializeField] private ListView magList;
        [SerializeField] private TooltipSpawnComponent tooltip;

        public void Populate(int value)
        {
            magList.gameObject.SetActive(value > 0);
            if (value > 0)
            {
                magList.Populate(Enumerable.Range(0, value).ToList(), (_, _) => { });
            }

            tooltip.Message = $"This leader will generate {value} MP at the start of each of their turns";
        }
    }
}