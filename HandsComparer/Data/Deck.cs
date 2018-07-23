using HandsComparer.Common;
using HandsComparer.Logic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HandsComparer.Data
{
    public class Deck
    {
        private List<Card> _cards;
       
        public Deck(List<Hand> hands = null)
        {
            _cards = new List<Card>();

            int position = 0;
            foreach (CardValues value in Enum.GetValues(typeof(CardValues)))
            {
                foreach (CardSuits suit in Enum.GetValues(typeof(CardSuits)))
                {
                    if (hands == null || !IsCardOnTable(hands, suit, value))
                    {
                        _cards.Add(new Card(value, suit, position));
                        position++;
                    }
                }
            }
        }

        public void QuickCalculate(WinEquity equity, List<Hand> hands, Hand table, int needToOpen, int calculationCount = 1000)
        {
            var startPosition = 5 - needToOpen;
            Queue<int> cardIdQueue = new Queue<int>();
            Random rand = new Random();

            for (int i = 0; i < calculationCount; i++)
            {   
                for (int j = 0; j < needToOpen; j++)
                {
                    int nmb;
                    do
                    {
                        nmb = rand.Next(0, _cards.Count);
                    }
                    while (cardIdQueue.Contains(nmb));
                    cardIdQueue.Enqueue(nmb);
                }

                for (int j = startPosition; j < 5; j++)
                {
                    table.Cards[j].Edit(_cards[cardIdQueue.Dequeue()]);
                }

                var result = HandComparer.CompareFullHands(hands.Select(p => p.Copy()).ToList(), table);
                equity.ProcessResult(result);
            }
        }

        public void Interate(WinEquity equity, List<Hand> hands, Hand table, int needToOpen, int current = 0, int startCard = 0)
        {
            var currentPosition = 5 - needToOpen + current;
            foreach (var card in GetCards(currentPosition, startCard))
            {
                table.Cards[currentPosition].Edit(card);

                if (needToOpen == current + 1)
                {
                    var result = HandComparer.CompareFullHands(hands.Select(p => p.Copy()).ToList(), table);
                    equity.ProcessResult(result);
                }
                else
                {
                    Interate(equity, hands, table, needToOpen, current + 1, card.Position + 1);
                }
            }
        }

        public IEnumerable<Card> GetCards(int position, int start)
        {
            int end = _cards.Count - (4 - position);

            for (int i = start; i < end; i++)
            {
                yield return _cards[i];
            }
        }

        private static bool IsCardOnTable(List<Hand> hands, CardSuits suit, CardValues value)
        {
            foreach (var hand in hands)
            {
                foreach (var card in hand.Cards)
                {
                    if (card.Suit == suit && card.Value == value)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}