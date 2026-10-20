using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Grupo5_Proyecto.CapaDatos;
using Grupo5_Proyecto.Clases;

namespace Grupo5_Proyecto.Database
{
    public static class DataStore
    {
        // Contadores internos para ir generando numeros de ID unicos automaticamente para cada nuevo registro
        private static int _nextUserId = 3;
        private static int _nextOwnerId = 4;
        private static int _nextPetId = 4;
        private static int _nextSupplierId = 3;
        private static int _nextMedicineId = 4;
        private static int _nextAppointmentId = 3;
        private static int _nextRecordId = 3;
        private static int _nextInvoiceId = 3;
        private static int _nextLogId = 1;

        static DataStore()
        {
            CargarDatos();
        }

        // Toma todo lo que esta en la memoria RAM y lo escribe permanentemente en la base de datos SQLite
        public static void GuardarDatos()
        {
            try
            {
                using (var con = new SQLiteConnection(ConexionBD.Cadena))
                {
                    con.Open();
                    using (var cmdPragma = new SQLiteCommand("PRAGMA foreign_keys = OFF;", con)) cmdPragma.ExecuteNonQuery();
                    using (var trans = con.BeginTransaction())
                    {
                        try
                        {
                            using (var cmdDel = new SQLiteCommand("DELETE FROM Role", con)) cmdDel.ExecuteNonQuery();
                            foreach (var r in Roles)
                            {
                                var cmd = new SQLiteCommand("INSERT INTO Role (id, name, description, permissions) VALUES (@id, @name, @desc, @perm)", con);
                                cmd.Parameters.AddWithValue("@id", r.Id);
                                cmd.Parameters.AddWithValue("@name", r.Name);
                                cmd.Parameters.AddWithValue("@desc", (object)r.Description ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@perm", r.Permissions);
                                cmd.ExecuteNonQuery();
                            }

                            using (var cmdDel = new SQLiteCommand("DELETE FROM User", con)) cmdDel.ExecuteNonQuery();
                            foreach (var u in Users)
                            {
                                var cmd = new SQLiteCommand("INSERT INTO User (id, username, email, password, name, roleId, createdAt) VALUES (@id, @username, @email, @password, @name, @roleId, @createdAt)", con);
                                cmd.Parameters.AddWithValue("@id", u.Id);
                                cmd.Parameters.AddWithValue("@username", u.Username);
                                cmd.Parameters.AddWithValue("@email", (object)u.Email ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@password", u.Password);
                                cmd.Parameters.AddWithValue("@name", u.Name);
                                cmd.Parameters.AddWithValue("@roleId", u.RoleId);
                                cmd.Parameters.AddWithValue("@createdAt", u.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                                cmd.ExecuteNonQuery();
                            }

                            using (var cmdDel = new SQLiteCommand("DELETE FROM Owner", con)) cmdDel.ExecuteNonQuery();
                            foreach (var o in Owners)
                            {
                                var cmd = new SQLiteCommand("INSERT INTO Owner (id, name, identity, phone, email, address) VALUES (@id, @name, @identity, @phone, @email, @address)", con);
                                cmd.Parameters.AddWithValue("@id", o.Id);
                                cmd.Parameters.AddWithValue("@name", o.Name);
                                cmd.Parameters.AddWithValue("@identity", o.Identity);
                                cmd.Parameters.AddWithValue("@phone", (object)o.Phone ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@email", (object)o.Email ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@address", (object)o.Address ?? DBNull.Value);
                                cmd.ExecuteNonQuery();
                            }

                            using (var cmdDel = new SQLiteCommand("DELETE FROM Pet", con)) cmdDel.ExecuteNonQuery();
                            foreach (var p in Pets)
                            {
                                var cmd = new SQLiteCommand("INSERT INTO Pet (id, name, species, breed, age, gender, weight, ownerId, nextVaccineDate) VALUES (@id, @name, @species, @breed, @age, @gender, @weight, @ownerId, @nextVaccine)", con);
                                cmd.Parameters.AddWithValue("@id", p.Id);
                                cmd.Parameters.AddWithValue("@name", p.Name);
                                cmd.Parameters.AddWithValue("@species", p.Species);
                                cmd.Parameters.AddWithValue("@breed", (object)p.Breed ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@age", (object)p.Age ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@gender", (object)p.Gender ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@weight", (object)p.Weight ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@ownerId", p.OwnerId);
                                cmd.Parameters.AddWithValue("@nextVaccine", p.NextVaccineDate.HasValue ? p.NextVaccineDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : DBNull.Value);
                                cmd.ExecuteNonQuery();
                            }

                            using (var cmdDel = new SQLiteCommand("DELETE FROM Supplier", con)) cmdDel.ExecuteNonQuery();
                            foreach (var s in Suppliers)
                            {
                                var cmd = new SQLiteCommand("INSERT INTO Supplier (id, name, contact, phone, email) VALUES (@id, @name, @contact, @phone, @email)", con);
                                cmd.Parameters.AddWithValue("@id", s.Id);
                                cmd.Parameters.AddWithValue("@name", s.Name);
                                cmd.Parameters.AddWithValue("@contact", (object)s.Contact ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@phone", (object)s.Phone ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@email", (object)s.Email ?? DBNull.Value);
                                cmd.ExecuteNonQuery();
                            }

                            using (var cmdDel = new SQLiteCommand("DELETE FROM Medicine", con)) cmdDel.ExecuteNonQuery();
                            foreach (var m in Medicines)
                            {
                                var cmd = new SQLiteCommand("INSERT INTO Medicine (id, name, description, category, price, stock, minStock, supplierId) VALUES (@id, @name, @desc, @cat, @price, @stock, @minStock, @supplierId)", con);
                                cmd.Parameters.AddWithValue("@id", m.Id);
                                cmd.Parameters.AddWithValue("@name", m.Name);
                                cmd.Parameters.AddWithValue("@desc", (object)m.Description ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@cat", (object)m.Category ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@price", m.Price);
                                cmd.Parameters.AddWithValue("@stock", m.Stock);
                                cmd.Parameters.AddWithValue("@minStock", m.MinStock);
                                cmd.Parameters.AddWithValue("@supplierId", (object)m.SupplierId ?? DBNull.Value);
                                cmd.ExecuteNonQuery();
                            }

                            using (var cmdDel = new SQLiteCommand("DELETE FROM Appointment", con)) cmdDel.ExecuteNonQuery();
                            foreach (var a in Appointments)
                            {
                                var cmd = new SQLiteCommand("INSERT INTO Appointment (id, date, reason, status, petId, userId) VALUES (@id, @date, @reason, @status, @petId, @userId)", con);
                                cmd.Parameters.AddWithValue("@id", a.Id);
                                cmd.Parameters.AddWithValue("@date", a.Date.ToString("yyyy-MM-dd HH:mm:ss"));
                                cmd.Parameters.AddWithValue("@reason", (object)a.Reason ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@status", a.Status);
                                cmd.Parameters.AddWithValue("@petId", a.PetId);
                                cmd.Parameters.AddWithValue("@userId", a.UserId);
                                cmd.ExecuteNonQuery();
                            }

                            using (var cmdDel = new SQLiteCommand("DELETE FROM MedicalRecord", con)) cmdDel.ExecuteNonQuery();
                            foreach (var r in MedicalRecords)
                            {
                                var cmd = new SQLiteCommand("INSERT INTO MedicalRecord (id, date, diagnosis, treatment, medicines, notes, petId, userId) VALUES (@id, @date, @diag, @treat, @meds, @notes, @petId, @userId)", con);
                                cmd.Parameters.AddWithValue("@id", r.Id);
                                cmd.Parameters.AddWithValue("@date", r.Date.ToString("yyyy-MM-dd HH:mm:ss"));
                                cmd.Parameters.AddWithValue("@diag", r.Diagnosis);
                                cmd.Parameters.AddWithValue("@treat", r.Treatment);
                                cmd.Parameters.AddWithValue("@meds", (object)r.Medicines ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@notes", (object)r.Notes ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@petId", r.PetId);
                                cmd.Parameters.AddWithValue("@userId", r.UserId);
                                cmd.ExecuteNonQuery();
                            }

                            using (var cmdDel = new SQLiteCommand("DELETE FROM Invoice", con)) cmdDel.ExecuteNonQuery();
                            foreach (var inv in Invoices)
                            {
                                var cmd = new SQLiteCommand("INSERT INTO Invoice (id, date, total, tax, paymentMethod, status, notes, ownerId) VALUES (@id, @date, @total, @tax, @pay, @status, @notes, @ownerId)", con);
                                cmd.Parameters.AddWithValue("@id", inv.Id);
                                cmd.Parameters.AddWithValue("@date", inv.Date.ToString("yyyy-MM-dd HH:mm:ss"));
                                cmd.Parameters.AddWithValue("@total", inv.Total);
                                cmd.Parameters.AddWithValue("@tax", inv.Tax);
                                cmd.Parameters.AddWithValue("@pay", (object)inv.PaymentMethod ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@status", inv.Status);
                                cmd.Parameters.AddWithValue("@notes", (object)inv.Notes ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@ownerId", (object)inv.OwnerId ?? DBNull.Value);
                                cmd.ExecuteNonQuery();
                            }

                            using (var cmdDel = new SQLiteCommand("DELETE FROM AuditLog", con)) cmdDel.ExecuteNonQuery();
                            foreach (var log in AuditLogs)
                            {
                                var cmd = new SQLiteCommand("INSERT INTO AuditLog (id, userId, action, entity, description, createdAt) VALUES (@id, @userId, @action, @entity, @desc, @created)", con);
                                cmd.Parameters.AddWithValue("@id", log.Id);
                                cmd.Parameters.AddWithValue("@userId", log.UserId);
                                cmd.Parameters.AddWithValue("@action", log.Action);
                                cmd.Parameters.AddWithValue("@entity", log.Entity);
                                cmd.Parameters.AddWithValue("@desc", (object)log.Description ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@created", log.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                                cmd.ExecuteNonQuery();
                            }

                            trans.Commit();
                        }
                        catch (Exception)
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error al guardar en SQLite: " + ex.ToString(), "Error de Base de Datos", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        // Lee toda la base de datos de disco duro y la carga a la memoria RAM de la aplicacion
        public static void CargarDatos()
        {
            try
            {
                using (var con = new SQLiteConnection(ConexionBD.Cadena))
                {
                    con.Open();

                    var tempRoles = new List<Role>();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Role", con))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tempRoles.Add(new Role
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Name = reader["name"].ToString() ?? "",
                                Description = reader["description"]?.ToString(),
                                Permissions = reader["permissions"]?.ToString() ?? ""
                            });
                        }
                    }

                    var tempUsers = new List<User>();
                    using (var cmd = new SQLiteCommand("SELECT * FROM User", con))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tempUsers.Add(new User
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Username = reader["username"].ToString() ?? "",
                                Email = reader["email"]?.ToString(),
                                Password = reader["password"].ToString() ?? "",
                                Name = reader["name"].ToString() ?? "",
                                RoleId = Convert.ToInt32(reader["roleId"]),
                                CreatedAt = reader["createdAt"] != DBNull.Value && !string.IsNullOrEmpty(reader["createdAt"].ToString()) ? DateTime.Parse(reader["createdAt"].ToString()!) : DateTime.Now
                            });
                        }
                    }

                    var tempOwners = new List<Owner>();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Owner", con))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tempOwners.Add(new Owner
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Name = reader["name"].ToString() ?? "",
                                Identity = reader["identity"].ToString() ?? "",
                                Phone = reader["phone"]?.ToString(),
                                Email = reader["email"]?.ToString(),
                                Address = reader["address"]?.ToString()
                            });
                        }
                    }

                    var tempPets = new List<Pet>();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Pet", con))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tempPets.Add(new Pet
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Name = reader["name"].ToString() ?? "",
                                Species = reader["species"].ToString() ?? "",
                                Breed = reader["breed"]?.ToString(),
                                Age = reader["age"] != DBNull.Value ? Convert.ToInt32(reader["age"]) : null,
                                Gender = reader["gender"]?.ToString(),
                                Weight = reader["weight"] != DBNull.Value ? Convert.ToSingle(reader["weight"]) : null,
                                OwnerId = Convert.ToInt32(reader["ownerId"]),
                                NextVaccineDate = reader["nextVaccineDate"] != DBNull.Value && !string.IsNullOrEmpty(reader["nextVaccineDate"].ToString()) ? DateTime.Parse(reader["nextVaccineDate"].ToString()!) : null
                            });
                        }
                    }

                    var tempSuppliers = new List<Supplier>();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Supplier", con))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tempSuppliers.Add(new Supplier
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Name = reader["name"].ToString() ?? "",
                                Contact = reader["contact"]?.ToString(),
                                Phone = reader["phone"]?.ToString(),
                                Email = reader["email"]?.ToString()
                            });
                        }
                    }

                    var tempMedicines = new List<Medicine>();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Medicine", con))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tempMedicines.Add(new Medicine
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Name = reader["name"].ToString() ?? "",
                                Description = reader["description"]?.ToString(),
                                Category = reader["category"]?.ToString(),
                                Price = Convert.ToSingle(reader["price"]),
                                Stock = Convert.ToInt32(reader["stock"]),
                                MinStock = reader["minStock"] != DBNull.Value ? Convert.ToInt32(reader["minStock"]) : 10,
                                SupplierId = reader["supplierId"] != DBNull.Value ? Convert.ToInt32(reader["supplierId"]) : null
                            });
                        }
                    }

                    var tempAppointments = new List<Appointment>();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Appointment", con))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tempAppointments.Add(new Appointment
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Date = DateTime.Parse(reader["date"].ToString()!),
                                Reason = reader["reason"]?.ToString(),
                                Status = reader["status"].ToString() ?? "SCHEDULED",
                                PetId = Convert.ToInt32(reader["petId"]),
                                UserId = Convert.ToInt32(reader["userId"])
                            });
                        }
                    }

                    var tempRecords = new List<MedicalRecord>();
                    using (var cmd = new SQLiteCommand("SELECT * FROM MedicalRecord", con))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tempRecords.Add(new MedicalRecord
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Date = DateTime.Parse(reader["date"].ToString()!),
                                Diagnosis = reader["diagnosis"].ToString() ?? "",
                                Treatment = reader["treatment"].ToString() ?? "",
                                Medicines = reader["medicines"]?.ToString(),
                                Notes = reader["notes"]?.ToString(),
                                PetId = Convert.ToInt32(reader["petId"]),
                                UserId = Convert.ToInt32(reader["userId"])
                            });
                        }
                    }

                    var tempInvoices = new List<Invoice>();
                    using (var cmd = new SQLiteCommand("SELECT * FROM Invoice", con))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tempInvoices.Add(new Invoice
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Date = DateTime.Parse(reader["date"].ToString()!),
                                Total = Convert.ToSingle(reader["total"]),
                                Tax = Convert.ToSingle(reader["tax"]),
                                PaymentMethod = reader["paymentMethod"]?.ToString(),
                                Status = reader["status"].ToString() ?? "PAID",
                                Notes = reader["notes"]?.ToString(),
                                OwnerId = reader["ownerId"] != DBNull.Value ? Convert.ToInt32(reader["ownerId"]) : null
                            });
                        }
                    }

                    var tempLogs = new List<AuditLog>();
                    using (var cmd = new SQLiteCommand("SELECT * FROM AuditLog", con))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tempLogs.Add(new AuditLog
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                UserId = Convert.ToInt32(reader["userId"]),
                                Action = reader["action"].ToString() ?? "",
                                Entity = reader["entity"].ToString() ?? "",
                                Description = reader["description"]?.ToString(),
                                CreatedAt = DateTime.Parse(reader["createdAt"].ToString()!)
                            });
                        }
                    }

                    if (tempRoles.Count > 0) Roles = tempRoles;
                    if (tempUsers.Count > 0) Users = tempUsers;
                    Owners = tempOwners;
                    Pets = tempPets;
                    Suppliers = tempSuppliers;
                    Medicines = tempMedicines;
                    Appointments = tempAppointments;
                    MedicalRecords = tempRecords;
                    Invoices = tempInvoices;
                    AuditLogs = tempLogs;

                    _nextUserId = Users.Count > 0 ? System.Linq.Enumerable.Max(Users, x => x.Id) + 1 : 3;
                    _nextOwnerId = Owners.Count > 0 ? System.Linq.Enumerable.Max(Owners, x => x.Id) + 1 : 1;
                    _nextPetId = Pets.Count > 0 ? System.Linq.Enumerable.Max(Pets, x => x.Id) + 1 : 1;
                    _nextSupplierId = Suppliers.Count > 0 ? System.Linq.Enumerable.Max(Suppliers, x => x.Id) + 1 : 1;
                    _nextMedicineId = Medicines.Count > 0 ? System.Linq.Enumerable.Max(Medicines, x => x.Id) + 1 : 1;
                    _nextAppointmentId = Appointments.Count > 0 ? System.Linq.Enumerable.Max(Appointments, x => x.Id) + 1 : 1;
                    _nextRecordId = MedicalRecords.Count > 0 ? System.Linq.Enumerable.Max(MedicalRecords, x => x.Id) + 1 : 1;
                    _nextInvoiceId = Invoices.Count > 0 ? System.Linq.Enumerable.Max(Invoices, x => x.Id) + 1 : 1;
                    _nextLogId = AuditLogs.Count > 0 ? System.Linq.Enumerable.Max(AuditLogs, x => x.Id) + 1 : 1;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error al cargar de SQLite: " + ex.ToString(), "Error de Base de Datos", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public static int NextUserId() => _nextUserId++;
        public static int NextOwnerId() => _nextOwnerId++;
        public static int NextPetId() => _nextPetId++;
        public static int NextSupplierId() => _nextSupplierId++;
        public static int NextMedicineId() => _nextMedicineId++;
        public static int NextAppointmentId() => _nextAppointmentId++;
        public static int NextRecordId() => _nextRecordId++;
        public static int NextInvoiceId() => _nextInvoiceId++;
        public static int NextLogId() => _nextLogId++;

        public static List<Role> Roles = new()
        {
            new Role { Id = 1, Name = "ADMINISTRADOR",     Description = "Administrador del sistema", Permissions = "ALL" },
            new Role { Id = 2, Name = "ASISTENTE",  Description = "Asistente veterinario",     Permissions = "READ,WRITE" }
        };

        public static List<User> Users = new()
        {
            new User { Id = 1, Username = "admin",  Password = "admin123", Name = "Dr. Cedeño",    RoleId = 1 },
            new User { Id = 2, Username = "asist1", Password = "1234",     Name = "Ana Martinez",  RoleId = 2 }
        };

        // Lista temporal para guardar a los Owner
        public static List<Owner> Owners = new();

        // Lista temporal para guardar a los Pet
        public static List<Pet> Pets = new();

        // Lista temporal para guardar a los Supplier
        public static List<Supplier> Suppliers = new();

        // Lista temporal para guardar a los Medicine
        public static List<Medicine> Medicines = new();

        // Lista temporal para guardar a los Appointment
        public static List<Appointment> Appointments = new();

        // Lista temporal para guardar a los MedicalRecord
        public static List<MedicalRecord> MedicalRecords = new();

        // Lista temporal para guardar a los Invoice
        public static List<Invoice> Invoices = new();

        // Lista temporal para guardar a los AuditLog
        public static List<AuditLog> AuditLogs = new();

        // Metodo que usamos para guardar el historial de todo lo que hacen
        // Paso 1: Nos mandan los datos de que acaba de pasar
        // Paso 2: Lo guardamos en nuestra lista de auditoria en memoria
        public static void RegistrarLog(string accion, string entidad, string descripcion)
        {
            AuditLogs.Add(new AuditLog
            {
                Id = NextLogId(),
                UserId = SessionManager.UsuarioActual?.Id ?? 0,
                Action = accion,
                Entity = entidad,
                Description = descripcion,
                CreatedAt = DateTime.Now
            });
            // Paso 3: Lo guardamos en la base de datos para que no se borre
            GuardarDatos();
        }
    }
}