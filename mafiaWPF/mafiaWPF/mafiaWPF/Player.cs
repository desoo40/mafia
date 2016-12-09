using System.ComponentModel;

namespace mafiaWPF
{
    public class  Player
    {
        public int Id { get; set; }

        public string Nick
        {
            get
            {
                if (string.IsNullOrEmpty(_nick))
                {
                    return Name + " " + Surname;
                }
                return _nick;
            }
        }
        public WhoIs Type { get; set; }
        public bool Mvp { get; set; }
        public decimal DeltaCarma { get; set; }
        public bool IsWinner { get; set; }

        public decimal MainRate;
        public decimal PowerRate;
        public decimal Carma;
        public string Name;
        public string Surname;
        public string _nick;
        public decimal Games;
        public decimal Wins;
        public decimal MVPQty;

        public override string ToString()
        {
            return Nick;
        }
    }

    public enum WhoIs
    {
        Мирный,
        Комиссар,
        Мафия
    }
}