using HandsComparer.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HandsComparer.Common
{
    public static class EquityResultHelper
    {
        public static Winner Find(this List<Winner> result, int playerId)
        {
            return result.Find(p => p.PlayerId == playerId);
        }
    }

    public static class CombinationHelper
    {
        public static void SetAdditional(this List<CardValues> list, List<Card> cards, List<int> positions, int index)
        {
            list.Add(cards.First(p => p.Position == positions[index]).Value);
        }
    }

    public static class ConvertHelper
    {
        public static T ToEnum<T>(this string value) where T : struct, IConvertible
        {
            var result = (T)(Enum.Parse(typeof(T), value.Trim()));
            return result;
        }
    }

    public static class CopyHelper
    {
        public static Hand Copy(this Hand hand)
        {
            var copyHand = new Hand(hand.PlayerId, hand.CardsCount)
            {
                Cards = hand.Cards.Select(p => new Card(p.Value, p.Suit, p.Position)).ToList()
            };
            
            return copyHand;
        }

        public static List<Hand> Copy(this List<Hand> hands)
        {
            return new List<Hand>(hands.Select(p => p.Copy()));
        }
    }

    public class CardComparer : IEqualityComparer<Card>
    {
        public bool Equals(Card x, Card y)
        {
            return x.Value == y.Value;
        }

        public int GetHashCode(Card obj)
        {
            return base.GetHashCode();
        }
    }

    public static class PMath
    {
        public static int Combination(long n, long k)
        {
           return (int)(Fct(n) / (Fct(k) * Fct(n - k)));
        }

        public static long Fct(long n)
        {
            if (n > 1)
            {
                return Fct(n - 1) * n;
            }
            else
            {
                return 1;
            }
        }
    }
}
