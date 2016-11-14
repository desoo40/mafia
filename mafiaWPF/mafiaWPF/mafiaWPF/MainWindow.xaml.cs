using System;
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
            
            if (newPlayer != null)
            CurrPlayers.Add(newPlayer);
        }

        private Player AddPlayerToCurr()
        {
            if (CurrPlayers.Count == 10)
            {
                MessageBox.Show("В игре уже 10 игроков");
                return null;
            }

            Player currPlayer = null;

            currPlayer = AllPlayers.FirstOrDefault(p => p.Nick == playerNikCB.Text);
            if (currPlayer == null)
            {
                MessageBox.Show("Такого игрока нет в базе");
                return null;
            }
            if (CurrPlayers.Contains(currPlayer))
            {
                MessageBox.Show("Уже добавили в игру");
                return null;
            }
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

            if (e.PropertyName == "DeltaCarma")
                e.Column.Header = "Карма";

            if (e.PropertyName == "IsWinner")
                e.Cancel = true;
        }

        private void playerNikCB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (playerNikCB.Text == "") return;

            var players = AllPlayers.FindAll(p => p.Nick.ToLower().StartsWith(playerNikCB.Text.ToLower()));
            if (players.Count>0)
            {
                playerNikCB.ItemsSource = players;
                playerNikCB.IsDropDownOpen = true;
            }
        }

        private void CalcButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrPlayers.Count < 10)
            {
                MessageBox.Show("Мало игроков");
                return;
            }
            if (!CorrectTypes())
            {
                MessageBox.Show("Верно расставьте типы игроков:\n" +
                                "Мирные - 6\n" +
                                "Мафия - 3\n" +
                                "Коммисар - 1\n");
                return;
            }

            int qtyMvp = IsOneMvp();

            if (qtyMvp != 1)
            {
                MessageBox.Show(qtyMvp > 0 ? "Лучший игрок только один" : "Выберите лучшего игрока");

                return;
            }

            if (IsCitizenCheckBox.IsChecked == true || IsMafiaCheckBox.IsChecked == true)
            {
                if (IsCitizenCheckBox.IsChecked == true && IsMafiaCheckBox.IsChecked == true)
                {
                    MessageBox.Show("Может победить лишь одна команда!");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Выберите победителей"); // сделать отмену выбранных победетелей с последующей отменой инициализации полей IsWinner
                return;
            }

            CalculateStat();
        }

        private void CalculateStat()
        {
            var winner = IsCitizenCheckBox.IsChecked;

            if (winner == true)
            {
                foreach (var currPlayer in CurrPlayers)
                {
                    if (currPlayer.Type != WhoIs.Мафия)
                    {
                        currPlayer.IsWinner = true;
                    }
                }
            }
            else
            {
                foreach (var currPlayer in CurrPlayers)
                {
                    if (currPlayer.Type == WhoIs.Мафия)
                    {
                        currPlayer.IsWinner = true;
                    }
                }
            }
        }

        private int IsOneMvp()
        {
            int cnt = 0;
            foreach (var currPlayer in CurrPlayers)
            {
                if (currPlayer.Mvp == true)
                    ++cnt;
            }
            return cnt;
        }

        private bool CorrectTypes()
        {
            int komm = 0;
            int cit = 0;
            int maf = 0;

            for (int i = 0; i < CurrPlayers.Count; i++)
            {
                if (CurrPlayers[i].Type == WhoIs.Комиссар)
                    ++komm;
                if (CurrPlayers[i].Type == WhoIs.Мафия)
                    ++maf;
                if (CurrPlayers[i].Type == WhoIs.Мирный)
                    ++cit;
            }

            if (komm == 1 && maf == 3 && cit == 6)
            {
                return true;
            }

            return false;
        }

        private void OneTeamMessage()
        {
            MessageBox.Show("Лишь одна команда может выиграть...");
        }

        private void IsCitizenCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void IsMafiaCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                CurrPlayers.Add(AllPlayers[i]);
            }

            CurrPlayers[0].Type = WhoIs.Мафия;
            CurrPlayers[1].Type = WhoIs.Мафия;
            CurrPlayers[2].Type = WhoIs.Мафия;
            CurrPlayers[3].Type = WhoIs.Комиссар;
        }
    }
}
