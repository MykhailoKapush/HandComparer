using HandsComparer.Common;
using System.Collections.Generic;
using System.Linq;

namespace HandsComparer.Data
{
    public class WinEquity
    {
        public List<Winner> Win { get; set; }

        public int Draw { get; set; }

        public int Count { get; set; }

        public WinEquity(List<Hand> hands, int count = 0)
        {
            Win = InitResult(hands);
            Count = count;
        }

        private List<Winner> InitResult(List<Hand> hands)
        {
            var result = new List<Winner>();

            foreach (var hand in hands)
            {
                result.Add(new Winner
                {
                    PlayerId = hand.PlayerId
                });
            }

            return result;
        }

        public void ProcessResult(IEnumerable<int> winners)
        {
            Count++;
            if (winners.Count() > 1)
            {
                Draw++;
            }
            else
            {
                Win.Find(winners.First()).Wins++;
            }
        }
    }
}
