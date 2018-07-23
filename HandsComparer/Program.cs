using HandsComparer.Common;
using HandsComparer.Data;
using HandsComparer.Logic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HandsComparer
{
    class Program
    {
        static void Main(string[] args)
        {
            var table = new Hand(0);
                //.AddCard(new Card(CardValues.Two, CardSuits.Diamonds))
                //.AddCard(new Card(CardValues.Seven, CardSuits.Clubs))
                //.AddCard(new Card(CardValues.Four, CardSuits.Diamonds))
                //.AddCard(new Card(CardValues.Ace, CardSuits.Diamonds))
                //.AddCard(new Card(CardValues.Two, CardSuits.Hearts));

            var firstHand = new Hand(1)
                .AddCard(new Card(CardValues.Six, CardSuits.Diamonds))
                .AddCard(new Card(CardValues.Six, CardSuits.Spades));

            var secondHand = new Hand(2)
                .AddCard(new Card(CardValues.Eight, CardSuits.Diamonds))
                .AddCard(new Card(CardValues.Queen, CardSuits.Spades));

            var hands = new List<Hand>
            {
                firstHand,
                secondHand
            };

            #region timer
            //var timer = new Stopwatch();
            //timer.Start();
            #endregion

            for (int i = 0; i <= 10; i++)
            {

                var equity = HandComparer.GetWinEquity(hands.Copy(), table.Copy(), i != 10);

                #region timer
                //timer.Stop();
                #endregion

                foreach (var player in equity.Win)
                {
                    Console.WriteLine($"Player #{player.PlayerId} : {Math.Round((double)player.Wins / equity.Count * 100, 1)}%  [{player.Wins} out(s)]");
                }
                Console.WriteLine($"Draw      : {Math.Round((double)equity.Draw / equity.Count * 100, 1)}%   [{equity.Count - equity.Win.Select(p => p.Wins).Sum()} out(s)]");
                Console.WriteLine($"Total count : {equity.Count}");

                Console.WriteLine($"{Environment.NewLine}----------------------------------------------------------------{Environment.NewLine}");
            }

            #region timer
            //Console.WriteLine($"{Environment.NewLine}Calculation takes {timer.ElapsedMilliseconds} ms.");
            #endregion

            Console.ReadKey();
        }
    }
}
