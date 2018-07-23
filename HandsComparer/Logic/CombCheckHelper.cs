using HandsComparer.Common;
using HandsComparer.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HandsComparer.Logic
{
    public static class CombinationChecker
    {
        public static bool CheckRoyalFlush(this Hand hand, ref List<int> positions,
            out bool isStraightFlush, out bool isFlush)
        {
            isStraightFlush = hand.CheckStraightFlush(ref positions, out isFlush);
            if (!isStraightFlush)
            {
                return false;
            }

            var condition = hand.Cards
                .Where(Contains(positions))
                .Where(p => p.Value == CardValues.Ace || p.Value == CardValues.Ten)
                .Count() == 2;

            if (!condition)
            {
                return false;
            }
            return true;
        }
        
        public static bool CheckStraightFlush(this Hand hand, ref List<int> positions, out bool isFlush)
        {
            isFlush = hand.CheckFlush(ref positions);
            if (!isFlush)
            {
                return false;
            }

            var straightPositions = new List<int>();
            var straightCards = new Hand(0)
            {
                Cards = hand.Cards
                    .Where(Contains(positions))
                    .Select(p => new Card(p.Value, p.Suit, p.Position))
                    .ToList()
            };

            bool isStraight = straightCards.CheckStraight(ref straightPositions);
            if (!isStraight)
            {
                positions.Clear();
                return false;
            }

            if (positions.All(p => straightPositions.Contains(p)))
            {
                return true;
            }
            return false;
        }

        public static bool CheckKare(this Hand hand, ref List<int> positions)
        {
            positions = hand.Cards
                .GroupBy(p => p.Value)
                .FirstOrDefault(p => p.Count() == 4)?
                .Select(p => p.Position).ToList();

            if (positions == null)
            {
                positions = new List<int>();
                return false;
            }

            var notCombCards = hand.Cards.Where(NotContains(positions));
            GetHighestCards(notCombCards, ref positions, 1);

            return true;
        }

        public static bool CheckFullHouse(this Hand hand, ref List<int> positions, out bool isSet, out bool isPair)
        {
            isSet = hand.CheckSet(ref positions, false);
            if (!isSet)
            {
                isPair = true; // IsNeedCheckPair
                return false;
            }

            var notCombCards = hand.Cards.Where(NotContains(positions));
            isPair = CheckPair(notCombCards, ref positions, false);
            if (!isPair)
            {
                positions.Clear();
                return false;
            }

            return true;
        }

        public static bool CheckFlush(this Hand hand, ref List<int> positions)
        {
            positions = hand.Cards
                .GroupBy(p => p.Suit)
                .FirstOrDefault(p => p.Count() == 5)?
                .OrderByDescending(p => p.Value)
                .Select(p => p.Position).ToList();

            if (positions == null)
            {
                positions = new List<int>();
                return false;
            }
            return true;
        }

        public static bool CheckStraight(this Hand hand, ref List<int> positions)
        {
            var cards = hand.Cards
                .Distinct(new CardComparer())
                .OrderByDescending(p => p.Value)
                .ToArray();

            positions.Add(cards.First().Position);
            for (int i = 1; i < cards.Length; i++)
            {
                if (positions.Count == 5)
                {
                    break;
                }

                if ((int)cards[i - 1].Value - (int)cards[i].Value == 1)
                {
                    positions.Add(cards[i].Position);
                }
                else
                {
                    positions.Clear();
                    positions.Add(cards[i].Position);
                }
            }

            //Check [Two - Ace]
            var last = positions.Last();
            var condition = positions.Count == 4
                && cards.First(p => p.Position == last).Value == CardValues.Two
                && cards.First().Value == CardValues.Ace;

            if (condition)
            {
                positions.Add(cards.First().Position);
            }

            if (positions.Count < 5)
            {
                positions.Clear();
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool CheckSet(this Hand hand, ref List<int> positions, bool fullable = true)
        {
            var pos = hand.Cards
                .GroupBy(p => p.Value)
                .Where(p => p.Count() == 3)
                .OrderByDescending(p => p.Key)
                .FirstOrDefault()?
                .Select(p => p.Position)
                .ToList();
            
            if (pos == null)
            {
                return false;
            }
            positions.AddRange(pos);

            if (fullable)
            {
                var notCombCards = hand.Cards.Where(Contains(positions));
                GetHighestCards(notCombCards, ref positions, 2);
            }
            return true;
        }

        public static bool CheckTwoPairs(this Hand hand, ref List<int> positions, bool fullable = true)
        {
            var isFirstPair = CheckPair(hand.Cards, ref positions, false);
            if (!isFirstPair)
            {
                return false;
            }

            var notCombCards = hand.Cards.Where(NotContains(positions));
            var isSecondPair = CheckPair(notCombCards, ref positions, false);
            if (!isSecondPair)
            {
                positions.Clear();
                return false;
            }

            if (fullable)
            {
                notCombCards = hand.Cards.Where(NotContains(positions));
                GetHighestCards(notCombCards, ref positions, 1);
            }
            return true;
        }

        public static bool CheckPair(IEnumerable<Card> cards, ref List<int> positions, bool fullable = true)
        {
            var pos = cards
                .GroupBy(p => p.Value)
                .Where(p => p.Count() >= 2)
                .OrderByDescending(p => p.Key)
                .FirstOrDefault()?
                .Select(p => p.Position)
                .Take(2)
                .ToList();

            if (pos == null)
            {
                return false;
            }
            positions.AddRange(pos);

            if (fullable)
            {
                var notCombCards = cards.Where(NotContains(positions));
                GetHighestCards(notCombCards, ref positions, 3);
            }
            return true;
        }

        public static List<CardValues> GetHighestCards(this IEnumerable<Card> cards, ref List<int> positions, int count = 5)
        {
            var list = cards
                .OrderByDescending(p => p.Value).Take(count);

            positions.AddRange(list.Select(p => p.Position));

            return list.Select(p => p.Value).ToList();
        }

        private static Func<Card, bool> Contains(List<int> positions)
        {
            return p => positions.Contains(p.Position);
        }

        private static Func<Card, bool> NotContains(List<int> positions)
        {
            return p => !positions.Contains(p.Position);
        }
    }
}
