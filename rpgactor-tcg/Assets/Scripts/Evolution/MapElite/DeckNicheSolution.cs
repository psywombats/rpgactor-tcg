using System.Collections.Generic;

namespace RpgActorTGC
{
    /// <summary>
    /// A wrapper around a deck solution containing additional data about its performance within this problem space
    /// </summary>
    public class DeckNicheSolution
    {
        public Deck Deck { get; private set; }
        
        private Party party;
        private bool isPartyInUse;
        
        public DeckNicheSolution(Deck deck)
        {
            Deck = deck;
        }

        public Party LockParty()
        {
            if (isPartyInUse)
            {
                return new Party(Deck);
            }
            else
            {
                isPartyInUse = true;
                return GetFreshParty();
            }
        }

        public void UnlockParty(Party returnedParty)
        {
            if (returnedParty == party)
            {
                isPartyInUse = false;
            }
        }

        private Party GetFreshParty()
        {
            if (party == null)
            {
                party = new Party(Deck);
            }
            else
            {
                party.Reset();
            }

            return party;
        }
    }
}