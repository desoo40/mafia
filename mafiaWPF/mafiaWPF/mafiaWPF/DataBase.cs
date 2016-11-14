﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
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
                MessageBox.Show("ХУЙ0");
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
                Console.WriteLine(ex.Message);
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


                players.Add(player);
            }
            return players;
        }
    }
}