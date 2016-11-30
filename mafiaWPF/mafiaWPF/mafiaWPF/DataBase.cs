using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.Windows;

namespace mafiaWPF
{
    public class DataBase
    {
        private OleDbConnection conn;

        public DataBase()
        {
            string connetionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Mafia.accdb;";

            conn = new OleDbConnection(connetionString);

            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Нет базы данных");
                return;
            }
        }

        public List<Player> GetPlayersFromDB()
        {
            if (conn.State != ConnectionState.Open) return null;

            OleDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Игроки";

            OleDbDataReader reader = null;
            try
            {
                reader = cmd.ExecuteReader();
            }
            catch (OleDbException ex)
            {
                MessageBox.Show(ex.Message);
            }

            var players = new List<Player>();

            while (reader.Read())
            {
                var player = new Player();
                player.Id = Convert.ToInt32(reader["ID"].ToString());
                player.MainRate = Convert.ToDecimal(reader["Общий Рейтинг"].ToString());
                player.PowerRate = Convert.ToDecimal(reader["Рейтинг Силы"].ToString());
                player.Carma = Convert.ToDecimal(reader["Карма"].ToString());
                player._nick = reader["Ник"].ToString();
                player.Name = reader["Имя"].ToString();
                player.Surname = reader["Фамилия"].ToString();
                player.Games = Convert.ToDecimal(reader["Количество Игр"].ToString());
                player.Wins = Convert.ToDecimal(reader["Победы"].ToString());
                players.Add(player);
            }
            return players;
        }

        public void Update(ObservableCollection<Player> currPlayers)
        {
            if (conn.State != ConnectionState.Open) return ;

            OleDbCommand cmd = conn.CreateCommand();

            foreach (var currPlayer in currPlayers)
            {
                decimal winProc = currPlayer.Wins / currPlayer.Games * 100;

                cmd.CommandText = $"UPDATE Игроки SET [Рейтинг Силы] = {currPlayer.PowerRate.ToString("F2", CultureInfo.InvariantCulture)}, " +
                                  $"[Общий Рейтинг] = {currPlayer.MainRate.ToString("F2", CultureInfo.InvariantCulture)}, " +
                                  $"[Карма] = {currPlayer.Carma.ToString("F2", CultureInfo.InvariantCulture)}, " +
                                  $"[Количество Игр] = {currPlayer.Games}, " +
                                  $"[Победы] = {currPlayer.Wins}, " +
                                  $"[Процент побед] = {winProc.ToString("F2", CultureInfo.InvariantCulture)}, " +
                                  $"[Количество MVP] = {currPlayer.MVPQty.ToString("F2", CultureInfo.InvariantCulture)} Where ID = {currPlayer.Id}";

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (OleDbException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}