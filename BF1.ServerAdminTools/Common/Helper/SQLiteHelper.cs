﻿using BF1.ServerAdminTools.Common.Utils;
using BF1.ServerAdminTools.Features.Data;

using Microsoft.Data.Sqlite;

namespace BF1.ServerAdminTools.Common.Helper;

public static class SQLiteHelper
{
    private static string KickDbFile = FileUtil.D_Data_Path + @"\AdminLog.db";

    private static SqliteConnection connection = null;

    /// <summary>
    /// 数据库初始化
    /// </summary>
    public static void Initialize()
    {
        var connStr = new SqliteConnectionStringBuilder("Data Source=" + KickDbFile)
        {
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();

        connection = new SqliteConnection(connStr);
        connection.Open();

        string selectSheet1 = @"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='kick_ok'";
        if (ExecuteScalar(selectSheet1) == 0)
        {
            string creatSheet1 = "CREATE TABLE kick_ok ( id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, name TEXT, personaId TEXT, reason TEXT, status TEXT, date TEXT )";
            ExecuteNonQuery(creatSheet1);
        }

        string selectSheet2 = @"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='kick_no'";
        if (ExecuteScalar(selectSheet2) == 0)
        {
            string creatSheet2 = "CREATE TABLE kick_no ( id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, name TEXT, personaId TEXT, reason TEXT, status TEXT, date TEXT )";
            ExecuteNonQuery(creatSheet2);
        }

        string selectSheet3 = @"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='change_team'";
        if (ExecuteScalar(selectSheet3) == 0)
        {
            string creatSheet3 = "CREATE TABLE change_team ( id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, rank TEXT, name TEXT, personaId TEXT, status TEXT, date TEXT )";
            ExecuteNonQuery(creatSheet3);
        }


        string selectSheet4 = @"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='balancers'";
        if (ExecuteScalar(selectSheet4) == 0)
        {
            string creatSheet4 = "CREATE TABLE balancers ( id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, rank TEXT, name TEXT, personaId TEXT, status TEXT, date TEXT )";
            ExecuteNonQuery(creatSheet4);
        }
    }

    /// <summary>
    /// 关闭数据库连接
    /// </summary>
    public static void CloseConnection()
    {
        if (connection != null)
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
    }

    /// <summary>
    /// 执行SQL命令，执行对数据表的增加、删除、修改操作（有SQL注入风险）
    /// </summary>
    /// <param name="sqlStr"></param>
    public static void ExecuteNonQuery(string sqlStr)
    {
        using var cmd = new SqliteCommand(sqlStr, connection);
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// 执行SQL命令，返回查询结果中第 1 行第 1 列的值（有SQL注入风险）
    /// </summary>
    /// <param name="sqlStr"></param>
    /// <returns></returns>
    public static int ExecuteScalar(string sqlStr)
    {
        using var cmd = new SqliteCommand(sqlStr, connection);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    /// <summary>
    /// 增加数据库记录
    /// </summary>
    /// <param name="sheetName"></param>
    /// <param name="info"></param>
    public static void AddLog2SQLite(string sheetName, BreakRuleInfo info)
    {
        switch (sheetName)
        {
            case "kick_ok":
                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                    @"
                        INSERT INTO kick_ok
                        ( name, personaId, reason, status, date ) 
                        VALUES
                        ( $name, $personaId, $reason, $status, $date )
                    ";
                    command.Parameters.AddWithValue("$name", info.Name);
                    command.Parameters.AddWithValue("$personaId", info.PersonaId);
                    command.Parameters.AddWithValue("$reason", info.Reason);
                    //command.Parameters.AddWithValue("$status", "kicked");
                    command.Parameters.AddWithValue("$status", info.Status);
                    command.Parameters.AddWithValue("$date", DateTime.Now.ToString());
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Log.Ex(ex);
                    }
                    
                }
                break;
            case "kick_no":
                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                    @"
                        INSERT INTO kick_no
                        ( name, personaId, reason, status, date )
                        VALUES
                        ( $name, $personaId, $reason, $status, $date )
                    ";
                    command.Parameters.AddWithValue("$name", info.Name);
                    command.Parameters.AddWithValue("$personaId", info.PersonaId);
                    command.Parameters.AddWithValue("$reason", info.Reason);
                    //command.Parameters.AddWithValue("$status", "not kicked");
                    command.Parameters.AddWithValue("$status", info.Status);
                    command.Parameters.AddWithValue("$date", DateTime.Now.ToString());

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Log.Ex(ex);
                    }
                    
                }
                break;
        }
    }

    /// <summary>
    /// 更换队伍记录日志
    /// </summary>
    /// <param name="info"></param>
    public static void AddLog2SQLite(ChangeTeamInfo info)
    {
        using (var command = connection.CreateCommand())
        {
            command.CommandText =
            @"
                INSERT INTO change_team
                ( rank, name, personaId, status, date ) 
                VALUES
                ( $rank, $name, $personaId, $status, $date )
            ";
            command.Parameters.AddWithValue("$rank", info.Rank);
            command.Parameters.AddWithValue("$name", info.Name);
            command.Parameters.AddWithValue("$personaId", info.PersonaId);
            command.Parameters.AddWithValue("$status", info.Status);
            command.Parameters.AddWithValue("$date", DateTime.Now.ToString());

            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.Ex(ex);
            }         
        }
    }

    public static void AddLog2SQLiteBalancersNex(ChangeTeamInfo info)
    {
        using (var command = connection.CreateCommand())
        {
            command.CommandText =
            @"
                INSERT INTO balancers
                ( rank, name, personaId, status, date ) 
                VALUES
                ( $rank, $name, $personaId, $status, $date )
            ";
            command.Parameters.AddWithValue("$rank", info.Rank);
            command.Parameters.AddWithValue("$name", info.Name);
            command.Parameters.AddWithValue("$personaId", info.PersonaId);
            command.Parameters.AddWithValue("$status", info.Status);
            command.Parameters.AddWithValue("$date", DateTime.Now.ToString());

            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.Ex(ex);
            }            
        }
    }
}
