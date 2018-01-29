using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataBase_Scr
{
    public static void CreateTable()
    {
        SqliteConnection connection = null;
        try
        {
            string databaseName = Application.persistentDataPath + @"/sqliteDB.db";
            if (!File.Exists(databaseName))
            {
                SqliteConnection.CreateFile(databaseName);
            }
            connection = new SqliteConnection(string.Format("Data Source={0};", databaseName));
            SqliteCommand command = new SqliteCommand(@"CREATE TABLE IF NOT EXISTS record  (
                                                        [id] integer PRIMARY KEY AUTOINCREMENT NOT NULL,
                                                        [name] char(100) NOT NULL,
														[level] 	int NOT NULL,
                                                        [points] 	int NOT NULL)", connection);
            connection.Open();
            command.ExecuteNonQuery();
        }
        finally
        {
            if (connection != null)
                connection.Close();
        }
    }
    public static List<Record_Scr> SelectRecord(int level)
    {
        var listRecord = new List<Record_Scr>();
        SqliteConnection connection = null;
        try
        {
            string databaseName = Application.persistentDataPath + @"/sqliteDB.db";
            connection = new SqliteConnection(string.Format("Data Source={0};", databaseName));
            SqliteCommand command = new SqliteCommand("SELECT name, points " +
                                        "FROM record " +
                                        "WHERE level = '" + level.ToString() +"'" +
                                        " ORDER BY points DESC " + " LIMIT 10" , connection);
            connection.Open();
            SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                listRecord.Add(new Record_Scr(reader.GetValue(0).ToString(),reader.GetInt32(1)));
            }
            reader.Close();
        }
        finally
        {
            if (connection != null)
                connection.Close();
        }
        return listRecord;
    }
    public static void InsertRecord(string name,int level,int points)
    {
        SqliteConnection connection = null;
        try
        {
            string databaseName = Application.persistentDataPath + @"/sqliteDB.db";

            connection = new SqliteConnection(string.Format("Data Source={0};", databaseName));
            SqliteCommand command = new SqliteCommand(@"insert into 
                                          record ('name','level','points') 
                                          values('" + name + "' , '" + level.ToString() + "' , '" + points.ToString() + "' )", connection);
            connection.Open();
            command.ExecuteNonQuery();
        }
        finally
        {
            if (connection != null)
                connection.Close();
        }
    }
}