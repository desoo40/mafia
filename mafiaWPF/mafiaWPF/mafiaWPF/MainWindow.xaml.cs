﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace mafiaWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CurrPlayers = new ObservableCollection<Player>();

            playerDG.ItemsSource = CurrPlayers;

            DB = new DataBase();
            AllPlayers = DB.GetPlayersFromDB();
        }

        List<Player> AllPlayers;
        ObservableCollection<Player> CurrPlayers;

        private DataBase DB;

        private void AddLayerBtn_Click(object sender, RoutedEventArgs e)
        {

            var newPlayer = AddPlayerToCurr();
            
            CurrPlayers.Add(newPlayer);
        }

        private Player AddPlayerToCurr()
        {
            Player currPlayer = null;

            AllPlayers.FirstOrDefault(p => p.Nick == playerNikTB.Text);

            return currPlayer;
        }

        private void playerDG_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Nick")
                e.Column.Header = "Ник";

            if (e.PropertyName == "Type")
                e.Column.Header = "Статус";

            if (e.PropertyName == "Mvp")
                e.Column.Header = "MVP";
        }

        private string lastinput = "";
        private void playerNikTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(playerNikTB.Text == "") return;

            if (playerNikTB.Text == lastinput)
            {
                lastinput = "";
                return;
            }

            var player = AllPlayers.FirstOrDefault(p => p.Nick.ToLower().StartsWith(playerNikTB.Text.ToLower()));
            if (player != null)
            {
                lastinput = playerNikTB.Text;
                string addnick = player.Nick;
                playerNikTB.SelectedText = addnick.Replace(lastinput, "");
            }
        }
    }
}
