using System.Linq;
using UnityEngine;

namespace RpgActorTGC
{
    public class MPView : MonoBehaviour
    {
        [SerializeField] private ListView magList;

        public void Populate(int value)
        {
            magList.gameObject.SetActive(value > 0);
            if (value > 0)
            {
                magList.Populate(Enumerable.Range(0, value).ToList(), (_, _) => { });
            }
        }
    }
}