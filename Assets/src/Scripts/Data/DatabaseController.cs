using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using SQLite4Unity3d;
using System;

public class DatabaseController : IDisposable
{
    public SQLiteConnection DBConnection;

    public DatabaseController()
    {
        // ToDo: handle DB in editor.
        if (!Application.isPlaying)
        {
            return;
        }

        // Path to database.
        string dbPath = Application.dataPath + "/Resources/Data/PKU-Database.db";
        DBConnection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

        if (DBConnection == null)
            Debug.LogError("Error initializing the database.");
    }

    public void Dispose()
    {
        DBConnection.Close();

        DBConnection.Dispose();
    }
}
