﻿using System.ComponentModel;

namespace mafiaWPF
{
    public class  Player
    {
        public int Id;

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

        public bool Mvp { get; set; }
        public WhoIs Type { get; set; }
        public decimal MainRate;
        public decimal PowerRate;
        public decimal Carma;
        public string Name;
        public string Surname;
        public string _nick;

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