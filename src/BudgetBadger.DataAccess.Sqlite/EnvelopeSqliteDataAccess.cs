﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using BudgetBadger.Core.DataAccess;
using BudgetBadger.Core.Files;
using BudgetBadger.Core.LocalizedResources;
using BudgetBadger.Models;
using Microsoft.Data.Sqlite;

namespace BudgetBadger.DataAccess.Sqlite
{
    public class EnvelopeSqliteDataAccess : SqliteDataAccess, IEnvelopeDataAccess
    {
        public EnvelopeSqliteDataAccess(string connectionString) : base(connectionString)
        {
        }

        public async Task CreateBudgetAsync(Budget budget)
        {
            

            using (await MultiThreadLock.UseWaitAsync())
            {
                await Task.Run(() =>
                {
                    using (var db = new SqliteConnection(_connectionString))
                    {
                        db.Open();
                        var command = db.CreateCommand();

                        command.CommandText = @"INSERT INTO Budget
                                                (Id, 
                                                 Amount, 
                                                 IgnoreOverspend,
                                                 BudgetScheduleId, 
                                                 EnvelopeId, 
                                                 CreatedDateTime, 
                                                 ModifiedDateTime)  
                                    VALUES     (@Id, 
                                                @Amount,
                                                @IgnoreOverspend, 
                                                @BudgetScheduleId, 
                                                @EnvelopeId, 
                                                @CreatedDateTime, 
                                                @ModifiedDateTime)";

                        command.Parameters.AddWithValue("@Id", budget.Id.ToByteArray());
                        command.Parameters.AddWithValue("@Amount", budget.Amount);
                        command.Parameters.AddWithValue("@IgnoreOverspend", budget.IgnoreOverspend);
                        command.Parameters.AddWithValue("@BudgetScheduleId", budget.Schedule?.Id.ToByteArray());
                        command.Parameters.AddWithValue("@EnvelopeId", budget.Envelope?.Id.ToByteArray());
                        command.Parameters.AddWithValue("@CreatedDateTime", budget.CreatedDateTime);
                        command.Parameters.AddWithValue("@ModifiedDateTime", budget.ModifiedDateTime);

                        command.ExecuteNonQuery();
                    }
                });
            }
            
        }

        public async Task CreateBudgetScheduleAsync(BudgetSchedule budgetSchedule)
        {
            

            using (await MultiThreadLock.UseWaitAsync())
            {
                await Task.Run(() =>
                {
                    using (var db = new SqliteConnection(_connectionString))
                    {
                        db.Open();
                        var command = db.CreateCommand();

                        command.CommandText = @"INSERT INTO BudgetSchedule
                                                (Id, 
                                                 BeginDate,
                                                 EndDate, 
                                                 CreatedDateTime, 
                                                 ModifiedDateTime) 
                                    VALUES     (@Id, 
                                                @BeginDate, 
                                                @EndDate, 
                                                @CreatedDateTime, 
                                                @ModifiedDateTime)";

                        command.Parameters.AddWithValue("@Id", budgetSchedule.Id.ToByteArray());
                        command.Parameters.AddWithValue("@BeginDate", budgetSchedule.BeginDate);
                        command.Parameters.AddWithValue("@EndDate", budgetSchedule.EndDate);
                        command.Parameters.AddWithValue("@CreatedDateTime", budgetSchedule.CreatedDateTime);
                        command.Parameters.AddWithValue("@ModifiedDateTime", budgetSchedule.ModifiedDateTime);

                        command.ExecuteNonQuery();
                    }
                });
            }
            
        }

        public async Task CreateEnvelopeAsync(Envelope envelope)
        {
            

            using (await MultiThreadLock.UseWaitAsync())
            {
                await Task.Run(() =>
                {
                    using (var db = new SqliteConnection(_connectionString))
                    {
                        db.Open();
                        var command = db.CreateCommand();

                        command.CommandText = @"INSERT INTO Envelope 
                                                (Id, 
                                                 Description, 
                                                 EnvelopeGroupId, 
                                                 Notes, 
                                                 IgnoreOverspend,
                                                 CreatedDateTime, 
                                                 ModifiedDateTime, 
                                                 DeletedDateTime,
                                                 HiddenDateTime) 
                                    VALUES     (@Id, 
                                                @Description, 
                                                @EnvelopeGroupId,
                                                @Notes, 
                                                @IgnoreOverspend,
                                                @CreatedDateTime, 
                                                @ModifiedDateTime, 
                                                @DeletedDateTime,
                                                @HiddenDateTime)";

                        command.Parameters.AddWithValue("@Id", envelope.Id.ToByteArray());
                        command.Parameters.AddWithValue("@Description", envelope.Description);
                        command.Parameters.AddWithValue("@EnvelopeGroupId", envelope.Group?.Id.ToByteArray());
                        command.Parameters.AddWithValue("@Notes", envelope.Notes ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@IgnoreOverspend", envelope.IgnoreOverspend);
                        command.Parameters.AddWithValue("@CreatedDateTime", envelope.CreatedDateTime);
                        command.Parameters.AddWithValue("@ModifiedDateTime", envelope.ModifiedDateTime);
                        command.Parameters.AddWithValue("@DeletedDateTime", envelope.DeletedDateTime ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@HiddenDateTime", envelope.HiddenDateTime ?? (object)DBNull.Value);

                        command.ExecuteNonQuery();
                    }
                });
            }
            
        }

        public async Task CreateEnvelopeGroupAsync(EnvelopeGroup envelopeGroup)
        {
            

            using (await MultiThreadLock.UseWaitAsync())
            {
                await Task.Run(() =>
                {
                    using (var db = new SqliteConnection(_connectionString))
                    {
                        db.Open();
                        var command = db.CreateCommand();

                        command.CommandText = @"INSERT INTO EnvelopeGroup
                                                (Id, 
                                                 Description, 
                                                 Notes, 
                                                 CreatedDateTime, 
                                                 ModifiedDateTime, 
                                                 DeletedDateTime,
                                                 HiddenDateTime) 
                                    VALUES     (@Id, 
                                                @Description, 
                                                @Notes, 
                                                @CreatedDateTime, 
                                                @ModifiedDateTime, 
                                                @DeletedDateTime,
                                                @HiddenDateTime)";

                        command.Parameters.AddWithValue("@Id", envelopeGroup.Id.ToByteArray());
                        command.Parameters.AddWithValue("@Description", envelopeGroup.Description);
                        command.Parameters.AddWithValue("@Notes", envelopeGroup.Notes ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CreatedDateTime", envelopeGroup.CreatedDateTime);
                        command.Parameters.AddWithValue("@ModifiedDateTime", envelopeGroup.ModifiedDateTime);
                        command.Parameters.AddWithValue("@DeletedDateTime", envelopeGroup.DeletedDateTime ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@HiddenDateTime", envelopeGroup.HiddenDateTime ?? (object)DBNull.Value);

                        command.ExecuteNonQuery();
                    }
                });
            }
            
        }

        public async Task DeleteBudgetAsync(Guid id)
        {
            

            using (await MultiThreadLock.UseWaitAsync())
            {
                await Task.Run(() =>
                {
                    using (var db = new SqliteConnection(_connectionString))
                    {
                        db.Open();
                        var command = db.CreateCommand();

                        command.CommandText = @"DELETE FROM Budget WHERE Id = @Id";

                        command.Parameters.AddWithValue("@Id", id.ToByteArray());

                        command.ExecuteNonQuery();
                    }
                });
            }
            
        }

        public async Task DeleteBudgetScheduleAsync(Guid id)
        {
            

            using (await MultiThreadLock.UseWaitAsync())
            {
                await Task.Run(() =>
                {
                    using (var db = new SqliteConnection(_connectionString))
                    {
                        db.Open();
                        var command = db.CreateCommand();

                        command.CommandText = @"DELETE BudgetSchedule WHERE Id = @Id";

                        command.Parameters.AddWithValue("@Id", id.ToByteArray());

                        command.ExecuteNonQuery();
                    }
                });
            }
            
        }

        public async Task DeleteEnvelopeAsync(Guid id)
        {
            

            using (await MultiThreadLock.UseWaitAsync())
            {
                await Task.Run(() =>
                {
                    using (var db = new SqliteConnection(_connectionString))
                    {
                        db.Open();
                        var command = db.CreateCommand();

                        command.CommandText = @"DELETE Envelope WHERE Id = @Id";

                        command.Parameters.AddWithValue("@Id", id.ToByteArray());

                        command.ExecuteNonQuery();
                    }
                });
            }
            
        }

        public async Task DeleteEnvelopeGroupAsync(Guid id)
        {
            

            using (await MultiThreadLock.UseWaitAsync())
            {
                await Task.Run(() =>
                {
                    using (var db = new SqliteConnection(_connectionString))
                    {
                        db.Open();
                        var command = db.CreateCommand();

                        command.CommandText = @"DELETE EnvelopeGroup WHERE Id = @Id";

                        command.Parameters.AddWithValue("@Id", id.ToByteArray());

                        command.ExecuteNonQuery();
                    }
                });
            }
            
        }

        public async Task<Budget> ReadBudgetAsync(Guid id)
        {
            

            using (await MultiThreadLock.UseWaitAsync())
            {
                return await Task.Run(() =>
                {
                    var budget = new Budget();
                    using (var db = new SqliteConnection(_connectionString))
                    {
                        db.Open();
                        var command = db.CreateCommand();

                        command.CommandText = @"SELECT B.Id, 
                                           B.Amount, 
                                           B.IgnoreOverspend,
                                           B.CreatedDateTime, 
                                           B.ModifiedDateTime, 
                                           B.BudgetScheduleId, 
                                           BS.BeginDate        AS BudgetScheduleBeginDate, 
                                           BS.EndDate          AS BudgetScheduleEndDate,
                                           BS.CreatedDateTime  AS BudgetScheduleCreatedDateTime, 
                                           BS.ModifiedDateTime AS BudgetScheduleModifiedDateTime, 
                                           B.EnvelopeId, 
                                           E.Description       AS EnvelopeDescription, 
                                           E.Notes             AS EnvelopeNotes, 
                                           E.IgnoreOverspend   AS EnvelopeIgnoreOverspend,
                                           E.CreatedDateTime   AS EnvelopeCreatedDateTime, 
                                           E.ModifiedDateTime  AS EnvelopeModifiedDateTime, 
                                           E.DeletedDateTime   AS EnvelopeDeletedDateTime,
                                           E.HiddenDateTime    AS EnvelopeHiddenDateTime,
                                           EG.Id               AS EnvelopeGroupId, 
                                           EG.Description      AS EnvelopeGroupDescription,
                                           EG.Notes            AS EnvelopeGroupNotes, 
                                           EG.CreatedDateTime  AS EnvelopeGroupCreatedDateTime, 
                                           EG.ModifiedDateTime AS EnvelopeGroupModifiedDateTime, 
                                           EG.DeletedDateTime  AS EnvelopeGroupDeletedDateTime,
                                           EG.HiddenDateTime   AS EnvelopeGroupHiddenDateTime
                                    FROM   Budget AS B 
                                    JOIN   BudgetSchedule AS BS ON B.BudgetScheduleId = BS.Id
                                    JOIN   Envelope AS E ON B.EnvelopeId = E.Id
                                    JOIN   EnvelopeGroup EG ON E.EnvelopeGroupId = EG.Id
                                    WHERE  B.Id = @Id";

                        command.Parameters.AddWithValue("@Id", id.ToByteArray());

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                budget = new Budget()
                                {
                                    Id = new Guid(reader["Id"] as byte[]),
                                    Amount = Convert.ToDecimal(reader["Amount"], CultureInfo.InvariantCulture),
                                    IgnoreOverspend = Convert.ToBoolean(reader["IgnoreOverspend"], CultureInfo.InvariantCulture),
                                    CreatedDateTime = Convert.ToDateTime(reader["CreatedDateTime"], CultureInfo.InvariantCulture),
                                    ModifiedDateTime = Convert.ToDateTime(reader["ModifiedDateTime"], CultureInfo.InvariantCulture),
                                    Schedule = new BudgetSchedule
                                    {
                                        Id = new Guid(reader["BudgetScheduleId"] as byte[]),
                                        BeginDate = Convert.ToDateTime(reader["BudgetScheduleBeginDate"], CultureInfo.InvariantCulture),
                                        EndDate = Convert.ToDateTime(reader["BudgetScheduleEndDate"], CultureInfo.InvariantCulture),
                                        CreatedDateTime = Convert.ToDateTime(reader["BudgetScheduleCreatedDateTime"], CultureInfo.InvariantCulture),
                                        ModifiedDateTime = Convert.ToDateTime(reader["BudgetScheduleModifiedDateTime"], CultureInfo.InvariantCulture),
                                    },
                                    Envelope = new Envelope
                                    {
                                        Id = new Guid(reader["EnvelopeId"] as byte[]),
                                        Description = reader["EnvelopeDescription"].ToString(),
                                        Notes = reader["EnvelopeNotes"].ToString(),
                                        IgnoreOverspend = Convert.ToBoolean(reader["EnvelopeIgnoreOverspend"], CultureInfo.InvariantCulture),
                                        CreatedDateTime = Convert.ToDateTime(reader["EnvelopeCreatedDateTime"], CultureInfo.InvariantCulture),
                                        ModifiedDateTime = Convert.ToDateTime(reader["EnvelopeModifiedDateTime"], CultureInfo.InvariantCulture),
                                        DeletedDateTime = reader["EnvelopeDeletedDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EnvelopeDeletedDateTime"], CultureInfo.InvariantCulture),
                                        HiddenDateTime = reader["EnvelopeHiddenDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EnvelopeHiddenDateTime"], CultureInfo.InvariantCulture),
                                        Group = new EnvelopeGroup
                                        {
                                            Id = new Guid(reader["EnvelopeGroupId"] as byte[]),
                                            Description = reader["EnvelopeGroupDescription"].ToString(),
                                            Notes = reader["EnvelopeGroupNotes"].ToString(),
                                            CreatedDateTime = Convert.ToDateTime(reader["EnvelopeGroupCreatedDateTime"], CultureInfo.InvariantCulture),
                                            ModifiedDateTime = Convert.ToDateTime(reader["EnvelopeGroupModifiedDateTime"], CultureInfo.InvariantCulture),
                                            DeletedDateTime = reader["EnvelopeGroupDeletedDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EnvelopeGroupDeletedDateTime"], CultureInfo.InvariantCulture),
                                            HiddenDateTime = reader["EnvelopeGroupHiddenDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EnvelopeGroupHiddenDateTime"], CultureInfo.InvariantCulture)
                                        }
                                    }
                                };
                            }
                        }
                    }

                    return budget;
                });
            }
            
        }

        public async Task<IReadOnlyList<Budget>> ReadBudgetsAsync()
        {
            

            using (await MultiThreadLock.UseWaitAsync())
            {
                return await Task.Run(() =>
                {
                    var budgets = new List<Budget>();

                    using (var db = new SqliteConnection(_connectionString))
                    {
                        db.Open();
                        var command = db.CreateCommand();

                        command.CommandText = @"SELECT B.Id, 
                                           B.Amount, 
                                           B.IgnoreOverspend,
                                           B.CreatedDateTime, 
                                           B.ModifiedDateTime, 
                                           B.BudgetScheduleId, 
                                           BS.BeginDate        AS BudgetScheduleBeginDate, 
                                           BS.EndDate          AS BudgetScheduleEndDate,
                                           BS.CreatedDateTime  AS BudgetScheduleCreatedDateTime, 
                                           BS.ModifiedDateTime AS BudgetScheduleModifiedDateTime, 
                                           B.EnvelopeId, 
                                           E.Description       AS EnvelopeDescription, 
                                           E.Notes             AS EnvelopeNotes, 
                                           E.IgnoreOverspend   AS EnvelopeIgnoreOverspend,
                                           E.CreatedDateTime   AS EnvelopeCreatedDateTime, 
                                           E.ModifiedDateTime  AS EnvelopeModifiedDateTime, 
                                           E.DeletedDateTime   AS EnvelopeDeletedDateTime,
                                           E.HiddenDateTime    AS EnvelopeHiddenDateTime,
                                           EG.Id               AS EnvelopeGroupId, 
                                           EG.Description      AS EnvelopeGroupDescription,
                                           EG.Notes            AS EnvelopeGroupNotes, 
                                           EG.CreatedDateTime  AS EnvelopeGroupCreatedDateTime, 
                                           EG.ModifiedDateTime AS EnvelopeGroupModifiedDateTime, 
                                           EG.DeletedDateTime  AS EnvelopeGroupDeletedDateTime,
                                           EG.HiddenDateTime   AS EnvelopeGroupHiddenDateTime
                                    FROM   Budget AS B 
                                    JOIN   BudgetSchedule AS BS ON B.BudgetScheduleId = BS.Id
                                    JOIN   Envelope AS E ON B.EnvelopeId = E.Id
                                    JOIN   EnvelopeGroup EG ON E.EnvelopeGroupId = EG.Id";

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                budgets.Add(new Budget()
                                {
                                    Id = new Guid(reader["Id"] as byte[]),
                                    Amount = Convert.ToDecimal(reader["Amount"], CultureInfo.InvariantCulture),
                                    IgnoreOverspend = Convert.ToBoolean(reader["IgnoreOverspend"], CultureInfo.InvariantCulture),
                                    CreatedDateTime = Convert.ToDateTime(reader["CreatedDateTime"], CultureInfo.InvariantCulture),
                                    ModifiedDateTime = Convert.ToDateTime(reader["ModifiedDateTime"], CultureInfo.InvariantCulture),
                                    Schedule = new BudgetSchedule
                                    {
                                        Id = new Guid(reader["BudgetScheduleId"] as byte[]),
                                        BeginDate = Convert.ToDateTime(reader["BudgetScheduleBeginDate"], CultureInfo.InvariantCulture),
                                        EndDate = Convert.ToDateTime(reader["BudgetScheduleEndDate"], CultureInfo.InvariantCulture),
                                        CreatedDateTime = Convert.ToDateTime(reader["BudgetScheduleCreatedDateTime"], CultureInfo.InvariantCulture),
                                        ModifiedDateTime = Convert.ToDateTime(reader["BudgetScheduleModifiedDateTime"], CultureInfo.InvariantCulture),
                                    },
                                    Envelope = new Envelope
                                    {
                                        Id = new Guid(reader["EnvelopeId"] as byte[]),
                                        Description = reader["EnvelopeDescription"].ToString(),
                                        Notes = reader["EnvelopeNotes"].ToString(),
                                        IgnoreOverspend = Convert.ToBoolean(reader["EnvelopeIgnoreOverspend"], CultureInfo.InvariantCulture),
                                        CreatedDateTime = Convert.ToDateTime(reader["EnvelopeCreatedDateTime"], CultureInfo.InvariantCulture),
                                        ModifiedDateTime = Convert.ToDateTime(reader["EnvelopeModifiedDateTime"], CultureInfo.InvariantCulture),
                                        DeletedDateTime = reader["EnvelopeDeletedDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EnvelopeDeletedDateTime"], CultureInfo.InvariantCulture),
                                        HiddenDateTime = reader["EnvelopeHiddenDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EnvelopeHiddenDateTime"], CultureInfo.InvariantCulture),
                                        Group = new EnvelopeGroup
                                        {
                                            Id = new Guid(reader["EnvelopeGroupId"] as byte[]),
                                            Description = reader["EnvelopeGroupDescription"].ToString(),
                                            Notes = reader["EnvelopeGroupNotes"].ToString(),
                                            CreatedDateTime = Convert.ToDateTime(reader["EnvelopeGroupCreatedDateTime"], CultureInfo.InvariantCulture),
                                            ModifiedDateTime = Convert.ToDateTime(reader["EnvelopeGroupModifiedDateTime"], CultureInfo.InvariantCulture),
                                            DeletedDateTime = reader["EnvelopeGroupDeletedDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EnvelopeGroupDeletedDateTime"], CultureInfo.InvariantCulture),
                                            HiddenDateTime = reader["EnvelopeGroupHiddenDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EnvelopeGroupHiddenDateTime"], CultureInfo.InvariantCulture)
                                        }
                                    }
                                });
                            }
                        }
                    }

                    return budgets;
                });
            }
            
        }

        public async Task<IReadOnlyList<Budget>> ReadBudgetsFromScheduleAsync(Guid scheduleId)
        {
            

            using (await MultiThreadLock.UseWaitAsync())
            {
                return await Task.Run(() =>
                {
                    var budgets = new List<Budget>();
                    using (var db = new SqliteConnection(_connectionString))
                    {
                        db.Open();
                        var command = db.CreateCommand();

                        command.CommandText = @"SELECT B.Id, 
                                           B.Amount, 
                                           B.IgnoreOverspend,
                                           B.CreatedDateTime, 
                                           B.ModifiedDateTime, 
                                           B.BudgetScheduleId, 
                                           BS.BeginDate        AS BudgetScheduleBeginDate, 
                                           BS.EndDate          AS BudgetScheduleEndDate,
                                           BS.CreatedDateTime  AS BudgetScheduleCreatedDateTime, 
                                           BS.ModifiedDateTime AS BudgetScheduleModifiedDateTime, 
                                           B.EnvelopeId, 
                                           E.Description       AS EnvelopeDescription, 
                                           E.Notes             AS EnvelopeNotes, 
                                           E.IgnoreOverspend   AS EnvelopeIgnoreOverspend,
                                           E.CreatedDateTime   AS EnvelopeCreatedDateTime, 
                                           E.ModifiedDateTime  AS EnvelopeModifiedDateTime, 
                                           E.DeletedDateTime   AS EnvelopeDeletedDateTime,
                                           E.HiddenDateTime    AS EnvelopeHiddenDateTime,
                                           EG.Id               AS EnvelopeGroupId, 
                                           EG.Description      AS EnvelopeGroupDescription,
                                           EG.Notes            AS EnvelopeGroupNotes, 
                                           EG.CreatedDateTime  AS EnvelopeGroupCreatedDateTime, 
                                           EG.ModifiedDateTime AS EnvelopeGroupModifiedDateTime, 
                                           EG.DeletedDateTime  AS EnvelopeGroupDeletedDateTime,
                                           EG.HiddenDateTime   AS EnvelopeGroupHiddenDateTime
                                    FROM   Budget AS B 
                                    JOIN   BudgetSchedule BS ON B.BudgetScheduleId = BS.Id
                                    JOIN   Envelope E ON B.EnvelopeId = E.Id
                                    JOIN   EnvelopeGroup EG ON E.EnvelopeGroupId = EG.Id
                                    WHERE  BS.Id = @ScheduleId";

                        command.Parameters.AddWithValue("@ScheduleId", scheduleId.ToByteArray());

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                budgets.Add(new Budget()
                                {
                                    Id = new Guid(reader["Id"] as byte[]),
                                    Amount = Convert.ToDecimal(reader["Amount"], CultureInfo.InvariantCulture),
                                    IgnoreOverspend = Convert.ToBoolean(reader["IgnoreOverspend"], CultureInfo.InvariantCulture),
                                    CreatedDateTime = Convert.ToDateTime(reader["CreatedDateTime"], CultureInfo.InvariantCulture),
                                    ModifiedDateTime = Convert.ToDateTime(reader["ModifiedDateTime"], CultureInfo.InvariantCulture),
                                    Schedule = new BudgetSchedule
                                    {
                                        Id = new Guid(reader["BudgetScheduleId"] as byte[]),
                                        BeginDate = Convert.ToDateTime(reader["BudgetScheduleBeginDate"], CultureInfo.InvariantCulture),
                                        EndDate = Convert.ToDateTime(reader["BudgetScheduleEndDate"], CultureInfo.InvariantCulture),
                                        CreatedDateTime = Convert.ToDateTime(reader["BudgetScheduleCreatedDateTime"], CultureInfo.InvariantCulture),
                                        ModifiedDateTime = Convert.ToDateTime(reader["BudgetScheduleModifiedDateTime"], CultureInfo.InvariantCulture),
                                    },
                                    Envelope = new Envelope
                                    {
                                        Id = new Guid(reader["EnvelopeId"] as byte[]),
                                        Description = reader["EnvelopeDescription"].ToString(),
                                        Notes = reader["EnvelopeNotes"].ToString(),
                                        IgnoreOverspend = Convert.ToBoolean(reader["EnvelopeIgnoreOverspend"], CultureInfo.InvariantCulture),
                                        CreatedDateTime = Convert.ToDateTime(reader["EnvelopeCreatedDateTime"], CultureInfo.InvariantCulture),
                                        ModifiedDateTime = Convert.ToDateTime(reader["EnvelopeModifiedDateTime"], CultureInfo.InvariantCulture),
                                        DeletedDateTime = reader["EnvelopeDeletedDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EnvelopeDeletedDateTime"], CultureInfo.InvariantCulture),
                                        HiddenDateTime = reader["EnvelopeHiddenDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EnvelopeHiddenDateTime"], CultureInfo.InvariantCulture),
                                        Group = new EnvelopeGroup
                                        {
                                            Id = new Guid(reader["EnvelopeGroupId"] as byte[]),
                                            Description = reader["EnvelopeGroupDescription"].ToString(),
                                            Notes = reader["EnvelopeGroupNotes"].ToString(),
                                            CreatedDateTime = Convert.ToDateTime(reader["EnvelopeGroupCreatedDateTime"], CultureInfo.InvariantCulture),
                                            ModifiedDateTime = Convert.ToDateTime(reader["EnvelopeGroupModifiedDateTime"], CultureInfo.InvariantCulture),
                                            DeletedDateTime = reader["EnvelopeGroupDeletedDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EnvelopeGroupDeletedDateTime"], CultureInfo.InvariantCulture),
                                            HiddenDateTime = reader["EnvelopeGroupHiddenDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EnvelopeGroupHiddenDateTime"], CultureInfo.InvariantCulture)
                                        }
                                    }
                                });
                            }
                        }
                    }
                    return budgets;
                });
            }
            
        }

        public async Task<IReadOnlyList<Budget>> ReadBudgetsFromEnvelopeAsync(Guid envelopeId)
        {
            

            using (await MultiThreadLock.UseWaitAsync())
            {
                return await Task.Run(() =>
                {
                    var budgets = new List<Budget>();
                    using (var db = new SqliteConnection(_connectionString))
                    {
                        db.Open();
                        var command = db.CreateCommand();

                        command.CommandText = @"SELECT B.Id, 
                                           B.Amount, 
                                           B.IgnoreOverspend,
                                           B.CreatedDateTime, 
                                           B.ModifiedDateTime, 
                                           B.BudgetScheduleId, 
                                           BS.BeginDate        AS BudgetScheduleBeginDate, 
                                           BS.EndDate          AS BudgetScheduleEndDate,
                                           BS.CreatedDateTime  AS BudgetScheduleCreatedDateTime, 
                                           BS.ModifiedDateTime AS BudgetScheduleModifiedDateTime, 
                                           B.EnvelopeId, 
                                           E.Description       AS EnvelopeDescription, 
                                           E.Notes             AS EnvelopeNotes, 
                                           E.IgnoreOverspend   AS EnvelopeIgnoreOverspend,
                                           E.CreatedDateTime   AS EnvelopeCreatedDateTime, 
                                           E.ModifiedDateTime  AS EnvelopeModifiedDateTime, 
                                           E.DeletedDateTime   AS EnvelopeDeletedDateTime,
                                           E.HiddenDateTime    AS EnvelopeHiddenDateTime,
                                           EG.Id               AS EnvelopeGroupId, 
                                           EG.Description      AS EnvelopeGroupDescription,
                                           EG.Notes            AS EnvelopeGroupNotes, 
                                           EG.CreatedDateTime  AS EnvelopeGroupCreatedDateTime, 
                                           EG.ModifiedDateTime AS EnvelopeGroupModifiedDateTime, 
                                           EG.DeletedDateTime  AS EnvelopeGroupDeletedDateTime,
                                           EG.HiddenDateTime   AS EnvelopeGroupHiddenDateTime
                                    FROM   Budget AS B 
                                    JOIN   BudgetSchedule BS ON B.BudgetScheduleId = BS.Id
                                    JOIN   Envelope E ON B.EnvelopeId = E.Id
                                    JOIN   EnvelopeGroup EG ON E.EnvelopeGroupId = EG.Id
                                    WHERE  E.Id = @EnvelopeId";

                        command.Parameters.AddWithValue("@EnvelopeId", envelopeId.ToByteArray());

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                budgets.Add(new Budget()
                                {
                                    Id = new Guid(reader["Id"] as byte[]),
                                    Amount = Convert.ToDecimal(reader["Amount"], CultureInfo.InvariantCulture),
                                    IgnoreOverspend = Convert.ToBoolean(reader["IgnoreOverspend"], CultureInfo.InvariantCulture),
                                    CreatedDateTime = Convert.ToDateTime(reader["CreatedDateTime"], CultureInfo.InvariantCulture),
                                    ModifiedDateTime = Convert.ToDateTime(reader["ModifiedDateTime"], CultureInfo.InvariantCulture),
                                    Schedule = new BudgetSchedule
                                    {
                                        Id = new Guid(reader["BudgetScheduleId"] as byte[]),
                                        BeginDate = Convert.ToDateTime(reader["BudgetScheduleBeginDate"], CultureInfo.InvariantCulture),
                                        EndDate = Convert.ToDateTime(reader["BudgetScheduleEndDate"], CultureInfo.InvariantCulture),
                                        CreatedDateTime = Convert.ToDateTime(reader["BudgetScheduleCreatedDateTime"], CultureInfo.InvariantCulture),
                                        ModifiedDateTime = Convert.ToDateTime(reader["BudgetScheduleModifiedDateTime"], CultureInfo.InvariantCulture),
                                    },
                                    Envelope = new Envelope
                                    {
                                        Id = new Guid(reader["EnvelopeId"] as byte[]),
                                        Description = reader["EnvelopeDescription"].ToString(),
                                        Notes = reader["EnvelopeNotes"].ToString(),
                                        IgnoreOverspend = Convert.ToBoolean(reader["EnvelopeIgnoreOverspend"], CultureInfo.InvariantCulture),
                                        CreatedDateTime = Convert.ToDateTime(reader["EnvelopeCreatedDateTime"], CultureInfo.InvariantCulture),
                                        ModifiedDateTime = Convert.ToDateTime(reader["EnvelopeModifiedDateTime"], CultureInfo.InvariantCulture),
                                        DeletedDateTime = reader["EnvelopeDeletedDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EnvelopeDeletedDateTime"], CultureInfo.InvariantCulture),
                                        HiddenDateTime = reader["EnvelopeHiddenDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EnvelopeHiddenDateTime"], CultureInfo.InvariantCulture),
                                        Group = new EnvelopeGroup
                                        {
                                            Id = new Guid(reader["EnvelopeGroupId"] as byte[]),
                                            Description = reader["EnvelopeGroupDescription"].ToString(),
                                            Notes = reader["EnvelopeGroupNotes"].ToString(),
                                            CreatedDateTime = Convert.ToDateTime(reader["EnvelopeGroupCreatedDateTime"], CultureInfo.InvariantCulture),
                                            ModifiedDateTime = Convert.ToDateTime(reader["EnvelopeGroupModifiedDateTime"], CultureInfo.InvariantCulture),
                                            DeletedDateTime = reader["EnvelopeGroupDeletedDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EnvelopeGroupDeletedDateTime"], CultureInfo.InvariantCulture),
                                            HiddenDateTime = reader["EnvelopeGroupHiddenDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EnvelopeGroupHiddenDateTime"], CultureInfo.InvariantCulture)
                                        }
                                    }
                                });
                            }
                        }
                    }

                    return budgets;
                });
            }
            
        }

        public async Task<Budget> ReadBudgetFromScheduleAndEnvelopeAsync(Guid scheduleId, Guid envelopeId)
        {
            

            using (await MultiThreadLock.UseWaitAsync())
            {
                return await Task.Run(() =>
                {
                    var budget = new Budget();
                    using (var db = new SqliteConnection(_connectionString))
                    {
                        db.Open();
                        var command = db.CreateCommand();

                        command.CommandText = @"SELECT B.Id, 
                                           B.Amount, 
                                           B.IgnoreOverspend,
                                           B.CreatedDateTime, 
                                           B.ModifiedDateTime, 
                                           B.BudgetScheduleId, 
                                           BS.BeginDate        AS BudgetScheduleBeginDate, 
                                           BS.EndDate          AS BudgetScheduleEndDate,
                                           BS.CreatedDateTime  AS BudgetScheduleCreatedDateTime, 
                                           BS.ModifiedDateTime AS BudgetScheduleModifiedDateTime, 
                                           B.EnvelopeId, 
                                           E.Description       AS EnvelopeDescription, 
                                           E.Notes             AS EnvelopeNotes, 
                                           E.IgnoreOverspend   AS EnvelopeIgnoreOverspend,
                                           E.CreatedDateTime   AS EnvelopeCreatedDateTime, 
                                           E.ModifiedDateTime  AS EnvelopeModifiedDateTime, 
                                           E.DeletedDateTime   AS EnvelopeDeletedDateTime,
                                           E.HiddenDateTime    AS EnvelopeHiddenDateTime,
                                           EG.Id               AS EnvelopeGroupId, 
                                           EG.Description      AS EnvelopeGroupDescription,
                                           EG.Notes            AS EnvelopeGroupNotes, 
                                           EG.CreatedDateTime  AS EnvelopeGroupCreatedDateTime, 
                                           EG.ModifiedDateTime AS EnvelopeGroupModifiedDateTime, 
                                           EG.DeletedDateTime  AS EnvelopeGroupDeletedDateTime,
                                           EG.HiddenDateTime   AS EnvelopeGroupHiddenDateTime
                                    FROM   Budget AS B 
                                    JOIN   BudgetSchedule BS ON B.BudgetScheduleId = BS.Id
                                    JOIN   Envelope E ON B.EnvelopeId = E.Id
                                    JOIN   EnvelopeGroup EG ON E.EnvelopeGroupId = EG.Id
                                    WHERE  E.Id = @EnvelopeId AND BS.Id = @ScheduleId";

                        command.Parameters.AddWithValue("@EnvelopeId", envelopeId.ToByteArray());
                        command.Parameters.AddWithValue("@ScheduleId", scheduleId.ToByteArray());

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                budget = new Budget()
                                {
                                    Id = new Guid(reader["Id"] as byte[]),
                                    Amount = Convert.ToDecimal(reader["Amount"], CultureInfo.InvariantCulture),
                                    IgnoreOverspend = Convert.ToBoolean(reader["IgnoreOverspend"], CultureInfo.InvariantCulture),
                                    CreatedDateTime = Convert.ToDateTime(reader["CreatedDateTime"], CultureInfo.InvariantCulture),
                                    ModifiedDateTime = Convert.ToDateTime(reader["ModifiedDateTime"], CultureInfo.InvariantCulture),
                                    Schedule = new BudgetSchedule
                                    {
                                        Id = new Guid(reader["BudgetScheduleId"] as byte[]),
                                        BeginDate = Convert.ToDateTime(reader["BudgetScheduleBeginDate"], CultureInfo.InvariantCulture),
                                        EndDate = Convert.ToDateTime(reader["BudgetScheduleEndDate"], CultureInfo.InvariantCulture),
                                        CreatedDateTime = Convert.ToDateTime(reader["BudgetScheduleCreatedDateTime"], CultureInfo.InvariantCulture),
                                        ModifiedDateTime = Convert.ToDateTime(reader["BudgetScheduleModifiedDateTime"], CultureInfo.InvariantCulture),
                                    },
                                    Envelope = new Envelope
                                    {
                                        Id = new Guid(reader["EnvelopeId"] as byte[]),
                                        Description = reader["EnvelopeDescription"].ToString(),
                                        Notes = reader["EnvelopeNotes"].ToString(),
                                        IgnoreOverspend = Convert.ToBoolean(reader["EnvelopeIgnoreOverspend"], CultureInfo.InvariantCulture),
                                        CreatedDateTime = Convert.ToDateTime(reader["EnvelopeCreatedDateTime"], CultureInfo.InvariantCulture),
                                        ModifiedDateTime = Convert.ToDateTime(reader["EnvelopeModifiedDateTime"], CultureInfo.InvariantCulture),
                                        DeletedDateTime = reader["EnvelopeDeletedDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EnvelopeDeletedDateTime"], CultureInfo.InvariantCulture),
                                        HiddenDateTime = reader["EnvelopeHiddenDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EnvelopeHiddenDateTime"], CultureInfo.InvariantCulture),
                                        Group = new EnvelopeGroup
                                        {
                                            Id = new Guid(reader["EnvelopeGroupId"] as byte[]),
                                            Description = reader["EnvelopeGroupDescription"].ToString(),
                                            Notes = reader["EnvelopeGroupNotes"].ToString(),
                                            CreatedDateTime = Convert.ToDateTime(reader["EnvelopeGroupCreatedDateTime"], CultureInfo.InvariantCulture),
                                            ModifiedDateTime = Convert.ToDateTime(reader["EnvelopeGroupModifiedDateTime"], CultureInfo.InvariantCulture),
                                            DeletedDateTime = reader["EnvelopeGroupDeletedDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EnvelopeGroupDeletedDateTime"], CultureInfo.InvariantCulture),
                                            HiddenDateTime = reader["EnvelopeGroupHiddenDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EnvelopeGroupHiddenDateTime"], CultureInfo.InvariantCulture)
                                        }
                                    }
                                };
                            }
                        }
                    }

                    return budget;
                });
            }
        }

        public async Task<BudgetSchedule> ReadBudgetScheduleAsync(Guid id)
        {
            

            using (await MultiThreadLock.UseWaitAsync())
            {
                return await Task.Run(() =>
                {
                    var budgetSchedule = new BudgetSchedule();
                    using (var db = new SqliteConnection(_connectionString))
                    {
                        db.Open();
                        var command = db.CreateCommand();

                        command.CommandText = @"SELECT Id,
                                           BeginDate, 
                                           EndDate,
                                           CreatedDateTime,
                                           ModifiedDateTime
                                    FROM   BudgetSchedule
                                    WHERE  Id = @Id";

                        command.Parameters.AddWithValue("@Id", id.ToByteArray());

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                budgetSchedule = new BudgetSchedule
                                {
                                    Id = new Guid(reader["Id"] as byte[]),
                                    BeginDate = Convert.ToDateTime(reader["BeginDate"], CultureInfo.InvariantCulture),
                                    EndDate = Convert.ToDateTime(reader["EndDate"], CultureInfo.InvariantCulture),
                                    CreatedDateTime = Convert.ToDateTime(reader["CreatedDateTime"], CultureInfo.InvariantCulture),
                                    ModifiedDateTime = Convert.ToDateTime(reader["ModifiedDateTime"], CultureInfo.InvariantCulture),
                                };
                            }
                        }
                    }

                    return budgetSchedule;
                });
            }
            
        }

        public async Task<IReadOnlyList<BudgetSchedule>> ReadBudgetSchedulesAsync()
        {
            

            using (await MultiThreadLock.UseWaitAsync())
            {
                return await Task.Run(() =>
                {
                    var budgetSchedules = new List<BudgetSchedule>();
                    using (var db = new SqliteConnection(_connectionString))
                    {
                        db.Open();
                        var command = db.CreateCommand();

                        command.CommandText = @"SELECT Id,
                                           BeginDate, 
                                           EndDate,
                                           CreatedDateTime,
                                           ModifiedDateTime
                                    FROM   BudgetSchedule";

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                budgetSchedules.Add(new BudgetSchedule
                                {
                                    Id = new Guid(reader["Id"] as byte[]),
                                    BeginDate = Convert.ToDateTime(reader["BeginDate"], CultureInfo.InvariantCulture),
                                    EndDate = Convert.ToDateTime(reader["EndDate"], CultureInfo.InvariantCulture),
                                    CreatedDateTime = Convert.ToDateTime(reader["CreatedDateTime"], CultureInfo.InvariantCulture),
                                    ModifiedDateTime = Convert.ToDateTime(reader["ModifiedDateTime"], CultureInfo.InvariantCulture),
                                });
                            }
                        }
                    }

                    return budgetSchedules;
                });
            }
            
        }

        public async Task<Envelope> ReadEnvelopeAsync(Guid id)
        {
            

            using (await MultiThreadLock.UseWaitAsync())
            {
                return await Task.Run(() =>
                {
                    var envelope = new Envelope();

                    using (var db = new SqliteConnection(_connectionString))
                    {
                        db.Open();
                        var command = db.CreateCommand();

                        command.CommandText = @"SELECT E.Id, 
                                           E.Description, 
                                           E.Notes, 
                                           E.IgnoreOverspend,
                                           E.CreatedDateTime, 
                                           E.ModifiedDateTime, 
                                           E.DeletedDateTime, 
                                           E.EnvelopeGroupId,
                                           E.HiddenDateTime,
                                           EG.Description      AS EnvelopeGroupDescription, 
                                           EG.Notes            AS EnvelopeGroupNotes, 
                                           EG.CreatedDateTime  AS EnvelopeGroupCreatedDateTime, 
                                           EG.ModifiedDateTime AS EnvelopeGroupModifiedDateTime, 
                                           EG.DeletedDateTime  AS EnvelopeGroupDeletedDateTime,
                                           EG.HiddenDateTime   AS EnvelopeGroupHiddenDateTime
                                    FROM   Envelope AS E
                                    JOIN   EnvelopeGroup AS EG ON E.EnvelopeGroupId = EG.Id
                                    WHERE  E.Id = @Id";

                        command.Parameters.AddWithValue("@Id", id.ToByteArray());

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                envelope = new Envelope
                                {
                                    Id = new Guid(reader["Id"] as byte[]),
                                    Description = reader["Description"].ToString(),
                                    Notes = reader["Notes"].ToString(),
                                    IgnoreOverspend = Convert.ToBoolean(reader["IgnoreOverspend"], CultureInfo.InvariantCulture),
                                    Group = new EnvelopeGroup
                                    {
                                        Id = new Guid(reader["EnvelopeGroupId"] as byte[]),
                                        Description = reader["EnvelopeGroupDescription"].ToString(),
                                        Notes = reader["EnvelopeGroupNotes"].ToString(),
                                        CreatedDateTime = Convert.ToDateTime(reader["EnvelopeGroupCreatedDateTime"], CultureInfo.InvariantCulture),
                                        ModifiedDateTime = Convert.ToDateTime(reader["EnvelopeGroupModifiedDateTime"], CultureInfo.InvariantCulture),
                                        DeletedDateTime = reader["EnvelopeGroupDeletedDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EnvelopeGroupDeletedDateTime"], CultureInfo.InvariantCulture),
                                        HiddenDateTime = reader["EnvelopeGroupHiddenDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EnvelopeGroupHiddenDateTime"], CultureInfo.InvariantCulture)
                                    },
                                    CreatedDateTime = Convert.ToDateTime(reader["CreatedDateTime"], CultureInfo.InvariantCulture),
                                    ModifiedDateTime = Convert.ToDateTime(reader["ModifiedDateTime"], CultureInfo.InvariantCulture),
                                    DeletedDateTime = reader["DeletedDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["DeletedDateTime"], CultureInfo.InvariantCulture),
                                    HiddenDateTime = reader["HiddenDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["HiddenDateTime"], CultureInfo.InvariantCulture)
                                };
                            }
                        }
                    }

                    return envelope;
                });
            }
            
        }

        public async Task<EnvelopeGroup> ReadEnvelopeGroupAsync(Guid id)
        {
            

            using (await MultiThreadLock.UseWaitAsync())
            {
                return await Task.Run(() =>
                {
                    var envelopeGroup = new EnvelopeGroup();

                    using (var db = new SqliteConnection(_connectionString))
                    {
                        db.Open();
                        var command = db.CreateCommand();

                        command.CommandText = @"SELECT Id,
                                               Description,
                                               Notes, 
                                               CreatedDateTime, 
                                               ModifiedDateTime, 
                                               DeletedDateTime,
                                               HiddenDateTime
                                        FROM   EnvelopeGroup
                                        WHERE  Id = @Id";

                        command.Parameters.AddWithValue("@Id", id.ToByteArray());

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                envelopeGroup = new EnvelopeGroup
                                {
                                    Id = new Guid(reader["Id"] as byte[]),
                                    Description = reader["Description"].ToString(),
                                    Notes = reader["Notes"].ToString(),
                                    CreatedDateTime = Convert.ToDateTime(reader["CreatedDateTime"], CultureInfo.InvariantCulture),
                                    ModifiedDateTime = Convert.ToDateTime(reader["ModifiedDateTime"], CultureInfo.InvariantCulture),
                                    DeletedDateTime = reader["DeletedDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["DeletedDateTime"], CultureInfo.InvariantCulture),
                                    HiddenDateTime = reader["HiddenDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["HiddenDateTime"], CultureInfo.InvariantCulture)
                                };
                            }
                        }
                    }

                    return envelopeGroup;
                });
            }
            
        }

        public async Task<IReadOnlyList<EnvelopeGroup>> ReadEnvelopeGroupsAsync()
        {
            

            using (await MultiThreadLock.UseWaitAsync())
            {
                return await Task.Run(() =>
                {
                    var envelopeGroups = new List<EnvelopeGroup>();

                    using (var db = new SqliteConnection(_connectionString))
                    {
                        db.Open();
                        var command = db.CreateCommand();

                        command.CommandText = @"SELECT Id,
                                               Description,
                                               Notes, 
                                               CreatedDateTime, 
                                               ModifiedDateTime, 
                                               DeletedDateTime,
                                               HiddenDateTime
                                        FROM   EnvelopeGroup";

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                envelopeGroups.Add(new EnvelopeGroup
                                {
                                    Id = new Guid(reader["Id"] as byte[]),
                                    Description = reader["Description"].ToString(),
                                    Notes = reader["Notes"].ToString(),
                                    CreatedDateTime = Convert.ToDateTime(reader["CreatedDateTime"], CultureInfo.InvariantCulture),
                                    ModifiedDateTime = Convert.ToDateTime(reader["ModifiedDateTime"], CultureInfo.InvariantCulture),
                                    DeletedDateTime = reader["DeletedDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["DeletedDateTime"], CultureInfo.InvariantCulture),
                                    HiddenDateTime = reader["HiddenDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["HiddenDateTime"], CultureInfo.InvariantCulture)
                                });
                            }
                        }
                    }

                    return envelopeGroups;
                });
            }
            
        }

        public async Task<IReadOnlyList<Envelope>> ReadEnvelopesAsync()
        {
            

            using (await MultiThreadLock.UseWaitAsync())
            {
                return await Task.Run(() =>
                {
                    var envelopes = new List<Envelope>();

                    using (var db = new SqliteConnection(_connectionString))
                    {
                        db.Open();
                        var command = db.CreateCommand();

                        command.CommandText = @"SELECT E.Id, 
                                               E.Description, 
                                               E.Notes, 
                                               E.IgnoreOverspend,
                                               E.CreatedDateTime, 
                                               E.ModifiedDateTime, 
                                               E.DeletedDateTime,
                                               E.HiddenDateTime,
                                               E.EnvelopeGroupId, 
                                               EG.Description      AS EnvelopeGroupDescription, 
                                               EG.Notes            AS EnvelopeGroupNotes, 
                                               EG.CreatedDateTime  AS EnvelopeGroupCreatedDateTime, 
                                               EG.ModifiedDateTime AS EnvelopeGroupModifiedDateTime, 
                                               EG.DeletedDateTime  AS EnvelopeGroupDeletedDateTime,
                                               EG.HiddenDateTime   AS EnvelopeGroupHiddenDateTime
                                        FROM   Envelope AS E
                                        JOIN   EnvelopeGroup AS EG ON E.EnvelopeGroupId = EG.Id";

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                envelopes.Add(new Envelope
                                {
                                    Id = new Guid(reader["Id"] as byte[]),
                                    Description = reader["Description"].ToString(),
                                    Notes = reader["Notes"].ToString(),
                                    IgnoreOverspend = Convert.ToBoolean(reader["IgnoreOverspend"], CultureInfo.InvariantCulture),
                                    Group = new EnvelopeGroup
                                    {
                                        Id = new Guid(reader["EnvelopeGroupId"] as byte[]),
                                        Description = reader["EnvelopeGroupDescription"].ToString(),
                                        Notes = reader["EnvelopeGroupNotes"].ToString(),
                                        CreatedDateTime = Convert.ToDateTime(reader["EnvelopeGroupCreatedDateTime"], CultureInfo.InvariantCulture),
                                        ModifiedDateTime = Convert.ToDateTime(reader["EnvelopeGroupModifiedDateTime"], CultureInfo.InvariantCulture),
                                        DeletedDateTime = reader["EnvelopeGroupDeletedDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EnvelopeGroupDeletedDateTime"], CultureInfo.InvariantCulture),
                                        HiddenDateTime = reader["EnvelopeGroupHiddenDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["EnvelopeGroupHiddenDateTime"], CultureInfo.InvariantCulture)
                                    },
                                    CreatedDateTime = Convert.ToDateTime(reader["CreatedDateTime"], CultureInfo.InvariantCulture),
                                    ModifiedDateTime = Convert.ToDateTime(reader["ModifiedDateTime"], CultureInfo.InvariantCulture),
                                    DeletedDateTime = reader["DeletedDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["DeletedDateTime"], CultureInfo.InvariantCulture),
                                    HiddenDateTime = reader["HiddenDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["HiddenDateTime"], CultureInfo.InvariantCulture)
                                });
                            }
                        }
                    }

                    return envelopes;
                });
            }
            
        }

        public async Task UpdateBudgetAsync(Budget budget)
        {
            

            using (await MultiThreadLock.UseWaitAsync())
            {
                await Task.Run(() =>
                {
                    using (var db = new SqliteConnection(_connectionString))
                    {
                        db.Open();
                        var command = db.CreateCommand();

                        command.CommandText = @"UPDATE Budget 
                                        SET    Amount = @Amount, 
                                               IgnoreOverspend = @IgnoreOverspend,
                                               EnvelopeId = @EnvelopeId, 
                                               BudgetScheduleId = @BudgetScheduleId,
                                               CreatedDateTime = @CreatedDateTime,
                                               ModifiedDateTime = @ModifiedDateTime
                                        WHERE  Id = @Id";

                        command.Parameters.AddWithValue("@Id", budget.Id.ToByteArray());
                        command.Parameters.AddWithValue("@Amount", budget.Amount);
                        command.Parameters.AddWithValue("@IgnoreOverspend", budget.IgnoreOverspend);
                        command.Parameters.AddWithValue("@EnvelopeId", budget.Envelope?.Id.ToByteArray());
                        command.Parameters.AddWithValue("@BudgetScheduleId", budget.Schedule?.Id.ToByteArray());
                        command.Parameters.AddWithValue("@CreatedDateTime", budget.CreatedDateTime);
                        command.Parameters.AddWithValue("@ModifiedDateTime", budget.ModifiedDateTime);

                        command.ExecuteNonQuery();
                    }
                });
            }
            
        }

        public async Task UpdateBudgetScheduleAsync(BudgetSchedule budgetSchedule)
        {
            

            using (await MultiThreadLock.UseWaitAsync())
            {
                await Task.Run(() =>
                {
                    using (var db = new SqliteConnection(_connectionString))
                    {
                        db.Open();
                        var command = db.CreateCommand();

                        command.CommandText = @"UPDATE BudgetSchedule
                                        SET    BeginDate = @BeginDate, 
                                               EndDate = @EndDate,
                                               CreatedDateTime = @CreatedDateTime,
                                               ModifiedDateTime = @ModifiedDateTime
                                        WHERE  Id = @Id";

                        command.Parameters.AddWithValue("@Id", budgetSchedule.Id.ToByteArray());
                        command.Parameters.AddWithValue("@BeginDate", budgetSchedule.BeginDate);
                        command.Parameters.AddWithValue("@EndDate", budgetSchedule.EndDate);
                        command.Parameters.AddWithValue("@CreatedDateTime", budgetSchedule.CreatedDateTime);
                        command.Parameters.AddWithValue("@ModifiedDateTime", budgetSchedule.ModifiedDateTime);

                        command.ExecuteNonQuery();
                    }
                });
            }
            
        }

        public async Task UpdateEnvelopeAsync(Envelope envelope)
        {
            

            using (await MultiThreadLock.UseWaitAsync())
            {
                await Task.Run(() =>
                {
                    using (var db = new SqliteConnection(_connectionString))
                    {
                        db.Open();
                        var command = db.CreateCommand();

                        command.CommandText = @"UPDATE Envelope 
                                        SET    Description = @Description,
                                               EnvelopeGroupId = @EnvelopeGroupId,
                                               Notes = @Notes,
                                               IgnoreOverspend = @IgnoreOverspend,
                                               CreatedDateTime = @CreatedDateTime, 
                                               ModifiedDateTime = @ModifiedDateTime, 
                                               DeletedDateTime = @DeletedDateTime,
                                               HiddenDateTime = @HiddenDateTime
                                        WHERE  Id = @Id ";

                        command.Parameters.AddWithValue("@Id", envelope.Id.ToByteArray());
                        command.Parameters.AddWithValue("@Description", envelope.Description);
                        command.Parameters.AddWithValue("@EnvelopeGroupId", envelope.Group?.Id.ToByteArray());
                        command.Parameters.AddWithValue("@Notes", envelope.Notes ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@IgnoreOverspend", envelope.IgnoreOverspend);
                        command.Parameters.AddWithValue("@CreatedDateTime", envelope.CreatedDateTime);
                        command.Parameters.AddWithValue("@ModifiedDateTime", envelope.ModifiedDateTime);
                        command.Parameters.AddWithValue("@DeletedDateTime", envelope.DeletedDateTime ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@HiddenDateTime", envelope.HiddenDateTime ?? (object)DBNull.Value);

                        command.ExecuteNonQuery();
                    }
                });
            }
            
        }

        public async Task UpdateEnvelopeGroupAsync(EnvelopeGroup envelopeGroup)
        {
            

            using (await MultiThreadLock.UseWaitAsync())
            {
                await Task.Run(() =>
                {
                    using (var db = new SqliteConnection(_connectionString))
                    {
                        db.Open();
                        var command = db.CreateCommand();

                        command.CommandText = @"UPDATE EnvelopeGroup
                                        SET    Description = @Description,
                                               Notes = @Notes,
                                               CreatedDateTime = @CreatedDateTime, 
                                               ModifiedDateTime = @ModifiedDateTime, 
                                               DeletedDateTime = @DeletedDateTime,
                                               HiddenDateTime = @HiddenDateTime
                                        WHERE  Id = @Id ";

                        command.Parameters.AddWithValue("@Id", envelopeGroup.Id.ToByteArray());
                        command.Parameters.AddWithValue("@Description", envelopeGroup.Description);
                        command.Parameters.AddWithValue("@Notes", envelopeGroup.Notes ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@CreatedDateTime", envelopeGroup.CreatedDateTime);
                        command.Parameters.AddWithValue("@ModifiedDateTime", envelopeGroup.ModifiedDateTime);
                        command.Parameters.AddWithValue("@DeletedDateTime", envelopeGroup.DeletedDateTime ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@HiddenDateTime", envelopeGroup.HiddenDateTime ?? (object)DBNull.Value);

                        command.ExecuteNonQuery();
                    }
                });
            }
            
        }

        public async Task<int> GetEnvelopesCountAsync()
        {
            

            using (await MultiThreadLock.UseWaitAsync())
            {
                return await Task.Run(() =>
                {
                    var count = 0;
                    using (var db = new SqliteConnection(_connectionString))
                    {
                        db.Open();
                        var command = db.CreateCommand();

                        command.CommandText = @"SELECT COUNT(*)
                                    FROM   Envelope";

                        object result = command.ExecuteScalar();
                        result = (result == DBNull.Value) ? null : result;
                        count = Convert.ToInt32(result, CultureInfo.InvariantCulture);
                    }

                    return count;
                });
            }
        }

        public async Task<int> GetEnvelopeGroupsCountAsync()
        {
            

            using (await MultiThreadLock.UseWaitAsync())
            {
                return await Task.Run(() =>
                {
                    var count = 0;
                    using (var db = new SqliteConnection(_connectionString))
                    {
                        db.Open();
                        var command = db.CreateCommand();

                        command.CommandText = @"SELECT COUNT(*)
                                    FROM   EnvelopeGroup";

                        object result = command.ExecuteScalar();
                        result = (result == DBNull.Value) ? null : result;
                        count = Convert.ToInt32(result, CultureInfo.InvariantCulture);
                    }

                    return count;
                });
            }
        }
    }
}
