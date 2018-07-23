using System.Collections.Generic;

namespace HandsComparer.Data
{
    public class Hand
    {
        public int CardsCount { get; set; }

        public int PlayerId { get; set; }

        public List<Card> Cards { get; set; }

        public Hand(int playerId, int count = 0)
        {
            CardsCount = count;
            PlayerId = playerId;
            Cards = new List<Card>();
        }

        public Hand AddCard(Card card)
        {
            card.Position = ++CardsCount;
            Cards.Add(card);
            return this;
        }

        public Hand AddCards(Hand hand)
        {
            foreach (var card in hand.Cards)
            {
                this.AddCard(card);
            }
            return this;
        }
    }
}
