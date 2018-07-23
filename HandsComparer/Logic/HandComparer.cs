using HandsComparer.Common;
using HandsComparer.Data;
using System.Collections.Generic;
using System.Linq;

namespace HandsComparer.Logic
{
    public class HandComparer
    {
        public static WinEquity GetWinEquity(List<Hand> hands, Hand table, bool isQuick = false)
        {
            var inDeck = 52 - (hands.Count * 2 + table.CardsCount);
            var needToOpen = 5 - table.CardsCount;

            var equity = new WinEquity(hands/*, PMath.Combination(inDeck, needToOpen)*/);

            if (table.CardsCount == 5)
            {
                var result = CompareFullHands(hands, table);
                equity.ProcessResult(result);
                return equity;
            }

            var deck = new Deck(new List<Hand>(hands) { table });

            for (int i = 0; i < needToOpen; i++)
            {
                table.AddCard(new Card());
            }

            if (isQuick)
            {
                deck.QuickCalculate(equity, hands, table, needToOpen);
            }
            else
            {
                deck.Interate(equity, hands, table, needToOpen);
            }

            return equity;
        }

        public static IEnumerable<int> CompareFullHands(List<Hand> hands, Hand table)
        {
            var combinations = new List<Combination>();

            foreach (var hand in hands)
            {
                hand.AddCards(table);

                var combination = GetHighestCombination(hand);
                combinations.Add(combination);
            }

            var winners = Compare(combinations);
            return winners;
        }

        private static Combination GetHighestCombination(Hand hand)
        {
            var positions = new List<int>();
            bool isStaightFlush, isFlush, isSet, IsPair;

            var combination = new Combination(hand.PlayerId);
            
            if (hand.CheckRoyalFlush(ref positions, out isStaightFlush, out isFlush))
            {
                combination.Type = CombinationTypes.RoyalFlush;
            }
            else if (isStaightFlush && hand.CheckStraightFlush(ref positions, out isFlush))
            {
                combination.Type = CombinationTypes.StraightFlush;
                combination.Additionals.SetAdditional(hand.Cards, positions, 0);
            }
            else if (hand.CheckKare(ref positions))
            {
                combination.Type = CombinationTypes.Kare;
                combination.Additionals.SetAdditional(hand.Cards, positions, 0);
                combination.Additionals.SetAdditional(hand.Cards, positions, 4);
            }
            else if (hand.CheckFullHouse(ref positions, out isSet, out IsPair))
            {
                combination.Type = CombinationTypes.FullHouse;
                combination.Additionals.SetAdditional(hand.Cards, positions, 0);
                combination.Additionals.SetAdditional(hand.Cards, positions, 4);
            }
            else if (isFlush && hand.CheckFlush(ref positions))
            {
                combination.Type = CombinationTypes.Flush;
                combination.Additionals.SetAdditional(hand.Cards, positions, 0);
            }
            else if (hand.CheckStraight(ref positions))
            {
                combination.Type = CombinationTypes.Straight;
                combination.Additionals.SetAdditional(hand.Cards, positions, 0);
            }
            else if (isSet && hand.CheckSet(ref positions))
            {
                combination.Type = CombinationTypes.Set;
                combination.Additionals.SetAdditional(hand.Cards, positions, 0);
                combination.Additionals.SetAdditional(hand.Cards, positions, 3);
                combination.Additionals.SetAdditional(hand.Cards, positions, 4);
            }
            else if (IsPair && hand.CheckTwoPairs(ref positions))
            {
                combination.Type = CombinationTypes.TwoPairs;
                combination.Additionals.SetAdditional(hand.Cards, positions, 0);
                combination.Additionals.SetAdditional(hand.Cards, positions, 2);
                combination.Additionals.SetAdditional(hand.Cards, positions, 4);
            }
            else if (IsPair && CombinationChecker.CheckPair(hand.Cards, ref positions))
            {
                combination.Type = CombinationTypes.Pair;
                combination.Additionals.SetAdditional(hand.Cards, positions, 0);
                combination.Additionals.SetAdditional(hand.Cards, positions, 2);
                combination.Additionals.SetAdditional(hand.Cards, positions, 3);
                combination.Additionals.SetAdditional(hand.Cards, positions, 4);
            }
            else
            {
                combination.Additionals.AddRange(CombinationChecker.GetHighestCards(hand.Cards, ref positions));
                combination.Type = CombinationTypes.HightKiker;
            }

            return combination;
        }

        private static IEnumerable<int> Compare(IEnumerable<Combination> combinations)
        {
            var highestComb = combinations.Max(p => p.Type);
            combinations = combinations.Where(p => p.Type == highestComb).ToList();

            if (combinations.Count() == 1)
            {
                return combinations.Select(p => p.PlayerId);
            }

            var additionalCount = combinations.First().Additionals.Count;

            for (int i = 0; i < additionalCount; i++)
            {
                var highestAdditional = combinations.Max(p => p.Additionals[i]);
                combinations = combinations.Where(p => p.Additionals[i] == highestAdditional).ToList();

                if (combinations.Count() == 1)
                {
                    return combinations.Select(p => p.PlayerId);
                }
            }

            return combinations.Select(p => p.PlayerId);
        }
    }
}
