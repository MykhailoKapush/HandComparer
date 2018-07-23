using System.Collections.Generic;

namespace HandsComparer.Data
{
    public class Combination
    {
        public int PlayerId { get; set; }

        public CombinationTypes Type { get; set; }

        public List<CardValues> Additionals { get; set; }

        public Combination()
        {
            Additionals = new List<CardValues>();
        }

        public Combination(int id)
        {
            PlayerId = id;
            Additionals = new List<CardValues>();
        }
    }
}
