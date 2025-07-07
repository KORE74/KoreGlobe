using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace KoreCommon;

public class KoreBinaryDataManager : IDisposable
{
    private string _connectionString;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private bool _disposed = false;

    // Centralized lock for one-time schema creation per instance.
    private readonly object _schemaLock = new object();
    private bool _schemaCreated = false;

    // ----------------------------------------------------------------------------------------
    // Constructor: Initializes the connection and ensures schema exists.
    // ----------------------------------------------------------------------------------------
    public KoreBinaryDataManager(string dbFilePath)
    {
        _connectionString = $"Data Source={dbFilePath};Version=3;";
        EnsureSchemaCreated();
    }

    // ----------------------------------------------------------------------------------------
    // One-time schema creation using a static lock.
    // ----------------------------------------------------------------------------------------
    private void EnsureSchemaCreated()
    {
        if (!_schemaCreated)
        {
            lock (_schemaLock)
            {
                if (!_schemaCreated)
                {
                    using (var connection = new SQLiteConnection(_connectionString))
                    {
                        connection.Open();
                        string createTableQuery = @"
                            CREATE TABLE IF NOT EXISTS BlobTable (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                DataName  TEXT UNIQUE,
                                DataBytes BLOB
                            )";
                        using (var command = new SQLiteCommand(createTableQuery, connection))
                        {
                            command.ExecuteNonQuery();
                        }

                        string createIndexQuery = "CREATE INDEX IF NOT EXISTS idx_DataName ON BlobTable (DataName);";
                        using (var command = new SQLiteCommand(createIndexQuery, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    _schemaCreated = true;
                }
            }
        }
    }

    // ----------------------------------------------------------------------------------------
    // Add
    // ----------------------------------------------------------------------------------------
    public bool Add(string dataName, byte[] dataBytes)
    {
        if (dataBytes == null || dataBytes.Length == 0 || string.IsNullOrEmpty(dataName))
            return false;

        _semaphore.Wait();
        try
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string insertQuery = "INSERT OR REPLACE INTO BlobTable (DataName, DataBytes) VALUES (@dataName, @dataBytes)";
                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@dataName", dataName);
                    command.Parameters.AddWithValue("@dataBytes", dataBytes);
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    // ----------------------------------------------------------------------------------------
    // Delete
    // ----------------------------------------------------------------------------------------

    public bool Delete(string dataName)
    {
        _semaphore.Wait();
        try
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string deleteQuery = "DELETE FROM BlobTable WHERE DataName = @dataName";
                using (var command = new SQLiteCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@dataName", dataName);
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    // ----------------------------------------------------------------------------------------
    // Get
    // ----------------------------------------------------------------------------------------
    public byte[] Get(string dataName)
    {
        _semaphore.Wait();
        try
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string selectQuery = "SELECT DataBytes FROM BlobTable WHERE DataName = @dataName";
                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@dataName", dataName);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return (byte[])reader["DataBytes"];
                        }
                    }
                }
            }
            return Array.Empty<byte>();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    // ----------------------------------------------------------------------------------------
    // List & Exists
    // ----------------------------------------------------------------------------------------
    public List<string> List()
    {
        var fileList = new List<string>();

        _semaphore.Wait();
        try
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string selectQuery = "SELECT DataName FROM BlobTable";
                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            fileList.Add(reader.GetString(0));
                        }
                    }
                }
            }
        }
        finally
        {
            _semaphore.Release();
        }

        return fileList;
    }

    public bool DataExists(string dataName)
    {
        _semaphore.Wait();
        try
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string selectQuery = "SELECT COUNT(*) FROM BlobTable WHERE DataName = @dataName";
                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@dataName", dataName);
                    long count = (long)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    // ----------------------------------------------------------------------------------------
    // Dispose
    // ----------------------------------------------------------------------------------------
    public void Dispose()
    {
        if (!_disposed)
        {
            _semaphore.Dispose();
            _disposed = true;
        }
    }
}
