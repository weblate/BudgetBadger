﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BudgetBadger.Core.DataAccess;
using BudgetBadger.Core.Files;
using BudgetBadger.Models;
using Microsoft.Data.Sqlite;

namespace BudgetBadger.DataAccess.Sqlite
{
    public class AccountSqliteDataAccess : IAccountDataAccess
    {
        readonly string _connectionString;

        public AccountSqliteDataAccess(string connectionString)
        {
            _connectionString = connectionString;

            Initialize();
        }

        void Initialize()
        {
            using(var db = new SqliteConnection(_connectionString))
            {
                db.Open();
                var command = db.CreateCommand();

                command.CommandText = @"CREATE TABLE IF NOT EXISTS Account 
                                          ( 
                                             Id               BLOB PRIMARY KEY NOT NULL, 
                                             Description      TEXT NOT NULL, 
                                             OnBudget         INTEGER NOT NULL, 
                                             Notes            TEXT, 
                                             CreatedDateTime  TEXT NOT NULL, 
                                             ModifiedDateTime TEXT NOT NULL, 
                                             DeletedDateTime  TEXT
                                          );
                                        ";
                
                command.ExecuteNonQuery();
            }
        }

        public async Task CreateAccountAsync(Account account)
        {
            using (var db = new SqliteConnection(_connectionString))
            {
                await db.OpenAsync();
                var command = db.CreateCommand();

                command.CommandText = @"INSERT INTO Account 
                                                    (Id, 
                                                     Description, 
                                                     OnBudget, 
                                                     Notes, 
                                                     CreatedDateTime, 
                                                     ModifiedDateTime, 
                                                     DeletedDateTime) 
                                        VALUES     (@Id, 
                                                    @Description, 
                                                    @OnBudget, 
                                                    @Notes, 
                                                    @CreatedDateTime, 
                                                    @ModifiedDateTime, 
                                                    @DeletedDateTime)";
                
                command.Parameters.AddWithValue("@Id", account.Id);
                command.Parameters.AddWithValue("@Description", account.Description);
                command.Parameters.AddWithValue("@OnBudget", account.OnBudget);
                command.Parameters.AddWithValue("@Notes", account.Notes ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@CreatedDateTime", account.CreatedDateTime);
                command.Parameters.AddWithValue("@ModifiedDateTime", account.ModifiedDateTime);
                command.Parameters.AddWithValue("@DeletedDateTime", account.DeletedDateTime ?? (object)DBNull.Value);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAccountAsync(Guid id)
        {
            using (var db = new SqliteConnection(_connectionString))
            {
                await db.OpenAsync();
                var command = db.CreateCommand();

                command.CommandText = @"DELETE Account WHERE Id = @Id";

                command.Parameters.AddWithValue("@Id", id);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<Account> ReadAccountAsync(Guid id)
        {
            var account = new Account();

            using (var db = new SqliteConnection(_connectionString))
            {
                await db.OpenAsync();
                var command = db.CreateCommand();

                command.CommandText = @"SELECT AC.Id, 
                                               AC.Description, 
                                               AC.OnBudget, 
                                               AC.Notes, 
                                               AC.CreatedDateTime, 
                                               AC.ModifiedDateTime, 
                                               AC.DeletedDateTime
                                        FROM   Account AS AC 
                                        WHERE  AC.Id = @Id";

                command.Parameters.AddWithValue("@Id", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        account = new Account
                        {
                            Id = new Guid(reader["Id"] as byte[]),
                            Description = reader["Description"].ToString(),
                            OnBudget = Convert.ToBoolean(reader["OnBudget"]),
                            Notes = reader["Notes"].ToString(),
                            CreatedDateTime = Convert.ToDateTime(reader["CreatedDateTime"]),
                            ModifiedDateTime = Convert.ToDateTime(reader["ModifiedDateTime"]),
                            DeletedDateTime = reader["DeletedDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["DeletedDateTime"])
                        };
                    }
                }
            }

            return account;
        }

        public async Task<IReadOnlyList<Account>> ReadAccountsAsync()
        {
            var accounts = new List<Account>();

            using (var db = new SqliteConnection(_connectionString))
            {
                await db.OpenAsync();
                var command = db.CreateCommand();

                command.CommandText = @"SELECT A.Id, 
                                               A.Description, 
                                               A.OnBudget, 
                                               A.Notes, 
                                               A.CreatedDateTime, 
                                               A.ModifiedDateTime, 
                                               A.DeletedDateTime
                                        FROM   Account AS A";

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        accounts.Add(new Account
                        {
                            Id = new Guid(reader["Id"] as byte[]),
                            Description = reader["Description"].ToString(),
                            OnBudget = Convert.ToBoolean(reader["OnBudget"]),
                            Notes = reader["Notes"].ToString(),
                            CreatedDateTime = Convert.ToDateTime(reader["CreatedDateTime"]),
                            ModifiedDateTime = Convert.ToDateTime(reader["ModifiedDateTime"]),
                            DeletedDateTime = reader["DeletedDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["DeletedDateTime"])
                        });
                    }
                }
            }

            return accounts;
        }

        public async Task UpdateAccountAsync(Account account)
        {
            using (var db = new SqliteConnection(_connectionString))
            {
                await db.OpenAsync();
                var command = db.CreateCommand();

                command.CommandText = @"UPDATE Account 
                                        SET    Description = @Description, 
                                               OnBudget = @OnBudget, 
                                               Notes = @Notes, 
                                               CreatedDateTime = @CreatedDateTime, 
                                               ModifiedDateTime = @ModifiedDateTime, 
                                               DeletedDateTime = @DeletedDateTime 
                                        WHERE  Id = @Id ";

                command.Parameters.AddWithValue("@Id", account.Id);
                command.Parameters.AddWithValue("@Description", account.Description);
                command.Parameters.AddWithValue("@OnBudget", account.OnBudget);
                command.Parameters.AddWithValue("@Notes", account.Notes);
                command.Parameters.AddWithValue("@CreatedDateTime", account.CreatedDateTime);
                command.Parameters.AddWithValue("@ModifiedDateTime", account.ModifiedDateTime);
                command.Parameters.AddWithValue("@DeletedDateTime", account.DeletedDateTime ?? (object)DBNull.Value);

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
