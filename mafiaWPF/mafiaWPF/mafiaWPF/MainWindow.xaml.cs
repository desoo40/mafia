using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
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
            if (DB.conn == null || DB.conn.State != ConnectionState.Open)
                Environment.Exit(0);
            AllPlayers = DB.GetPlayersFromDB();
        }

        List<Player> AllPlayers;
        ObservableCollection<Player> CurrPlayers;

        private DataBase DB;

        private void AddPlayerBtn_Click(object sender, RoutedEventArgs e)
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
            var input = playerNikCB.Text;

            currPlayer = AllPlayers.FirstOrDefault(p => p.Nick == input || p.Id.ToString() == input);
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
            if (e.PropertyName == "Id")
                e.Column.Header = "ID";

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
            if (playerNikCB.Text == "")
            {
                playerNikCB.IsDropDownOpen = false;
                playerNikCB.ItemsSource = null;
                return;
            }

            var players = AllPlayers.FindAll(p =>
            p.Id.ToString() == playerNikCB.Text || 
            p.Nick.ToLower().StartsWith(playerNikCB.Text.ToLower()));

            if (players.Count ==0)
            {
                playerNikCB.IsDropDownOpen = false;
                playerNikCB.ItemsSource = null;
            }
            else
            {
                //bool clearselect = false;

                playerNikCB.ItemsSource = players;
                //if (!playerNikCB.IsDropDownOpen) clearselect = true;
                //playerNikCB.IsDropDownOpen = true;
                //if (clearselect)
                //{
                //    var edit = (TextBox) playerNikCB.Template.FindName("PART_EditableTextBox", playerNikCB);
                //    edit.SelectedText = null;
                //}
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

            int qtyMvp = CurrPlayers.Count(currPlayer => currPlayer.Mvp);

            if (qtyMvp != 1)
            {
                MessageBox.Show(qtyMvp > 0 ? "Лучший игрок только один" : "Выберите лучшего игрока");

                return;
            }

            if(IsCitizenCheckBox.IsChecked ==false && IsMafiaCheckBox.IsChecked == false)
            { 
                MessageBox.Show("Выберите победителей"); // сделать отмену выбранных победетелей с последующей отменой инициализации полей IsWinner
                return;
            }

            CalculateStat();
        }

        private void CalculateStat()
        {
            WinnerLoserCheck();

            decimal carmaKoef = 0.25M;
            decimal gamesKoef = 0.3M;

            var avgWinners = CurrPlayers.Where(p => p.IsWinner).Average(p => p.PowerRate);
            var avgLosers = CurrPlayers.Where(p => !p.IsWinner).Average(p => p.PowerRate);

            var deltaPowerRate = 0M;

            if (avgWinners == 0)
                deltaPowerRate = 60;
            else
                deltaPowerRate = (avgLosers / avgWinners) * 30; 

            if (deltaPowerRate > 60)
                deltaPowerRate = 60;
            else if (deltaPowerRate < 15)
                deltaPowerRate = 15;

            foreach (var currPlayer in CurrPlayers)
            {
                if (currPlayer.Type == WhoIs.Мафия)
                    ++currPlayer.Mafia;

                if (currPlayer.Type == WhoIs.Мирный)
                    ++currPlayer.Citizen;

                if (currPlayer.Type == WhoIs.Комиссар)
                    ++currPlayer.Comiss;

                if (currPlayer.IsWinner)
                {
                    if (currPlayer.Type == WhoIs.Мафия)
                        ++currPlayer.WinsMafia;

                    if (currPlayer.Type == WhoIs.Мирный)
                        ++currPlayer.WinsCitizen;

                    if (currPlayer.Type == WhoIs.Комиссар)
                        ++currPlayer.WinsComiss;

                    if (currPlayer.Mvp)
                    {
                        currPlayer.PowerRate += deltaPowerRate*2;
                        ++currPlayer.MVPQty;
                    }
                    else
                        currPlayer.PowerRate += deltaPowerRate;

                    ++currPlayer.Wins;
                }

                else
                {
                    if (currPlayer.Mvp)
                    {
                        currPlayer.PowerRate += 30;
                        ++currPlayer.MVPQty;
                    }
                    else
                        currPlayer.PowerRate -= deltaPowerRate;

                    if (currPlayer.PowerRate < 0)
                        currPlayer.PowerRate = 0;
                }

                currPlayer.Carma += currPlayer.DeltaCarma;
                ++currPlayer.Games;

                currPlayer.MainRate = currPlayer.PowerRate + currPlayer.Carma * carmaKoef + currPlayer.Games * gamesKoef;
            }

            DB.Update(CurrPlayers);
            AllPlayers = DB.GetPlayersFromDB();
            ClearStatus();

            MessageBox.Show("База данных обновлена!");

        }

        private void ClearStatus()
        {
            foreach (var currPlayer in CurrPlayers)
            {
                currPlayer.IsWinner = false;
            }
        }

        private void WinnerLoserCheck()
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

        private bool CorrectTypes()
        {
            int komm = 0;
            int cit = 0;
            int maf = 0;

            foreach (Player p in CurrPlayers)
            {
                if (p.Type == WhoIs.Комиссар)
                    ++komm;
                if (p.Type == WhoIs.Мафия)
                    ++maf;
                if (p.Type == WhoIs.Мирный)
                    ++cit;
            }

            if (komm == 1 && maf == 3 && cit == 6)
            {
                return true;
            }

            return false;
        }

        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)

        {

            DataGridCell cell = sender as DataGridCell;

            if (!cell.IsEditing)

            {
                // enables editing on single click

                if (!cell.IsFocused)

                    cell.Focus();

                if (!cell.IsSelected)

                    cell.IsSelected = true;
            }

        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            CurrPlayers.Clear();
        }

        private void playerNikCB_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddPlayerBtn_Click(sender, e);
            }
        }

        private void playerDG_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var currPlayer = AllPlayers.FirstOrDefault(p => p.Nick == playerDG.CurrentItem.ToString());

                if (currPlayer != null)
                    CurrPlayers.Remove(currPlayer);
            }
        }
    }
}
