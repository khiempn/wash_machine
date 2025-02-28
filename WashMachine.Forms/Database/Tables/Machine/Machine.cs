using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using WashMachine.Forms.Database.Context;
using WashMachine.Forms.Modules.Laundry.LaundryItems;

namespace WashMachine.Forms.Database.Tables.Machine
{
    public class Machine : AppDbContext, ICRUD<MachineModel>
    {
        public Machine()
        {
            Initial();
        }

        public int Delete(MachineModel model)
        {
            throw new NotImplementedException();
        }

        public MachineModel Get(MachineModel model)
        {
            var machine = new MachineModel();

            using (var con = CreateConnection())
            {
                var command = con.CreateCommand();
                command.CommandText = $@"SELECT Id, Name, Description, Type, StartAt, EndAt, Time, Temp, IsRunning FROM machine
                    WHERE Name = '{model.Name}'
                ";
                command.CommandType = System.Data.CommandType.Text;
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        machine = new MachineModel()
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            Type = reader.GetInt32(3),
                            StartAt = reader.GetString(4),
                            EndAt = reader.GetString(5),
                            Time = reader.GetInt32(6),
                            Temp = reader.GetInt32(7),
                            IsRunning = reader.GetInt32(8)
                        };
                        return machine;
                    }
                }
            }

            return machine;
        }

        public List<MachineModel> GetAll()
        {
            var machines = new List<MachineModel>();

            using (var con = CreateConnection())
            {
                var command = con.CreateCommand();
                command.CommandText = $@"SELECT Id, Name, Description, Type, StartAt, EndAt FROM machine";
                command.CommandType = System.Data.CommandType.Text;
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var machine = new MachineModel()
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            Type = reader.GetInt32(3),
                            StartAt = reader.GetString(4),
                            EndAt = reader.GetString(5),
                            Time = reader.GetInt32(6),
                            Temp = reader.GetInt32(7),
                            IsRunning = reader.GetInt32(8)
                        };
                        machines.Add(machine);
                    }
                }
            }

            return machines;
        }

        protected override void Initial()
        {
            try
            {
                using (var con = CreateConnection())
                {
                    using (var transaction = con.BeginTransaction())
                    {
                        // 1 Dryer, 2 Wash
                        var script = @"
                            CREATE TABLE IF NOT EXISTS ""machine""(
                                ""Id""  INTEGER NOT NULL,
                                ""Name"" TEXT,
                                ""Description"" TEXT,
                                ""Type"" INTEGER,
                                ""StartAt"" TEXT,
                                ""EndAt"" TEXT,
                                ""Time"" INTEGER,
                                ""Temp"" INTEGER,
                                ""IsRunning"" INTEGER,
                                PRIMARY KEY(""Id"" AUTOINCREMENT)
                            );
                        ";

                        var command = con.CreateCommand();
                        command.CommandText = script;
                        command.CommandType = System.Data.CommandType.Text;
                        command.ExecuteNonQuery();

                        transaction.Commit();
                    }
                }

                List<MachineModel> machines = GetAll();
                if (!machines.Any())
                {
                    List<string> initialMachines = new List<string>()
                    {
                        nameof(Dryer01LaundryItem),
                        nameof(Dryer02LaundryItem),
                        nameof(Dryer03LaundryItem),
                        nameof(Dryer04LaundryItem),

                        nameof(Wash01LaundryItem),
                        nameof(Wash02LaundryItem),
                        nameof(Wash03LaundryItem),
                        nameof(Wash04LaundryItem)
                    };

                    foreach (string initialMachine in initialMachines)
                    {
                        Insert(new MachineModel()
                        {
                            Name = initialMachine,
                            Description = "",
                            StartAt = "0",
                            EndAt = "0",
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public int Insert(MachineModel model)
        {
            try
            {
                using (var con = CreateConnection())
                {
                    using (var transaction = con.BeginTransaction())
                    {
                        var command = con.CreateCommand();

                        command.CommandText =
                        @"
                            INSERT INTO machine (
                                Name,
                                Description,
                                Type,
                                StartAt,
                                EndAt,
                                Time,
                                Temp,
                                IsRunning
                            ) VALUES (
                                $Name,
                                $Description,
                                $Type,
                                $StartAt,
                                $EndAt,
                                $Time,
                                $Temp,
                                $IsRunning
                            );
                        ";

                        var pName = command.CreateParameter();
                        pName.ParameterName = "$Name";
                        pName.Value = model.Name ?? string.Empty;
                        command.Parameters.Add(pName);

                        var pDescription = command.CreateParameter();
                        pDescription.ParameterName = "$Description";
                        pDescription.Value = model.Description ?? string.Empty;
                        command.Parameters.Add(pDescription);

                        var pType = command.CreateParameter();
                        pType.ParameterName = "$Type";
                        pType.Value = model.Type;
                        command.Parameters.Add(pType);

                        var pStartAt = command.CreateParameter();
                        pStartAt.ParameterName = "$StartAt";
                        pStartAt.Value = model.StartAt ?? string.Empty;
                        command.Parameters.Add(pStartAt);

                        var pEndAt = command.CreateParameter();
                        pEndAt.ParameterName = "$EndAt";
                        pEndAt.Value = model.EndAt ?? string.Empty;
                        command.Parameters.Add(pEndAt);

                        var pTime = command.CreateParameter();
                        pTime.ParameterName = "$Time";
                        pTime.Value = model.Time;
                        command.Parameters.Add(pTime);

                        var pTemp = command.CreateParameter();
                        pTemp.ParameterName = "$Temp";
                        pTemp.Value = model.Temp;
                        command.Parameters.Add(pTemp);

                        var pIsRunning = command.CreateParameter();
                        pIsRunning.ParameterName = "$IsRunning";
                        pIsRunning.Value = model.IsRunning;
                        command.Parameters.Add(pIsRunning);

                        command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }

                return 1;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return -1;
            }
        }

        public int Update(MachineModel model)
        {
            try
            {
                using (var con = CreateConnection())
                {
                    using (var transaction = con.BeginTransaction())
                    {
                        var command = con.CreateCommand();
                        command.CommandText =
                        $@"
                            UPDATE machine SET StartAt = $StartAt, EndAt = $EndAt, Time = $Time, Temp = $Temp
                                , IsRunning = $IsRunning
                            WHERE Name = '{model.Name}'
                        ";

                        var pStartAt = command.CreateParameter();
                        pStartAt.ParameterName = "$StartAt";
                        pStartAt.Value = model.StartAt;
                        command.Parameters.Add(pStartAt);

                        var pEndAt = command.CreateParameter();
                        pEndAt.ParameterName = "$EndAt";
                        pEndAt.Value = model.EndAt;
                        command.Parameters.Add(pEndAt);

                        var pTime = command.CreateParameter();
                        pTime.ParameterName = "$Time";
                        pTime.Value = model.Time;
                        command.Parameters.Add(pTime);

                        var pTemp = command.CreateParameter();
                        pTemp.ParameterName = "$Temp";
                        pTemp.Value = model.Temp;
                        command.Parameters.Add(pTemp);

                        var pIsRunning = command.CreateParameter();
                        pIsRunning.ParameterName = "$IsRunning";
                        pIsRunning.Value = model.IsRunning;
                        command.Parameters.Add(pIsRunning);

                        command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }

                return 1;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return -1;
            }
        }

        public void ResetMachine(MachineModel model)
        {
            model.StartAt = "0";
            model.EndAt = "0";
            model.Time = 0;
            model.Type = 0;
            model.IsRunning = 0;
            Update(model);
        }
    }
}
