using HandsComparer.Common;
using System;

namespace HandsComparer.Data
{
    public class Card
    {
        public int Position { get; set; }

        public CardSuits Suit { get; set; }

        public CardValues Value { get; set; }

        public Card()
        { }

        public Card(string valueSuitString)
        {
            try
            {
                var splited = valueSuitString.Split(" ");

                Value = splited[0].ToEnum<CardValues>();
                Suit = splited[1].ToEnum<CardSuits>();
            }
            catch
            {
                throw new Exception("Value or suit is not correct");
            }
        }

        public Card(CardValues value, CardSuits suit)
        {
            Suit = suit;
            Value = value;
        }

        public Card(CardValues value, CardSuits suit, int position)
        {
            Suit = suit;
            Value = value;
            Position = position;
        }
        
        public void Edit(Card card)
        {
            Suit = card.Suit;
            Value = card.Value;
        }
    }
}
