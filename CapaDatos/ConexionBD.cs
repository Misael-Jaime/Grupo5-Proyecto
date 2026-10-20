using System;
using System.IO;
using System.Data;
using System.Data.SQLite;

namespace Grupo5_Proyecto.CapaDatos
{
    public class ConexionBD
    {
        // Ruta donde se guardara fisicamente el archivo de nuestra base de datos local (SQLite)
        private static readonly string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "vetnexus.db");
        // Esta es la 'llave' o cadena de conexion que el sistema usa para comunicarse con SQLite
        public static string Cadena => $"Data Source={dbPath};Version=3;";

        private SQLiteConnection conexion;

        static ConexionBD()
        {
            InicializarBaseDeDatos();
        }

        // Metodo super importante: Crea la base de datos y todas sus tablas si es la primera vez que se abre el sistema
        private static void InicializarBaseDeDatos()
        {
            if (File.Exists(dbPath))
            {
                bool necesitaRecreacion = false;
                try
                {
                    using (var con = new SQLiteConnection(Cadena))
                    {
                        con.Open();
                        using (var cmd = new SQLiteCommand("PRAGMA table_info(Owner);", con))
                        using (var reader = cmd.ExecuteReader())
                        {
                            bool tieneEmail = false;
                            while (reader.Read())
                            {
                                if (reader["name"].ToString()?.ToLower() == "email")
                                {
                                    tieneEmail = true;
                                    break;
                                }
                            }
                            if (!tieneEmail) necesitaRecreacion = true;
                        }
                    }
                }
                catch { necesitaRecreacion = true; }

                if (necesitaRecreacion)
                {
                    try { File.Delete(dbPath); } catch { }
                }
            }

            if (!File.Exists(dbPath))
            {
                string dir = Path.GetDirectoryName(dbPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                using (var connection = new SQLiteConnection(Cadena))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            string createRole = @"
                                CREATE TABLE IF NOT EXISTS Role (
                                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    name TEXT NOT NULL,
                                    description TEXT,
                                    permissions TEXT
                                );";
                            new SQLiteCommand(createRole, connection).ExecuteNonQuery();

                            string createUser = @"
                                CREATE TABLE IF NOT EXISTS User (
                                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    username TEXT UNIQUE NOT NULL,
                                    email TEXT,
                                    password TEXT NOT NULL,
                                    name TEXT,
                                    roleId INTEGER,
                                    createdAt TEXT,
                                    FOREIGN KEY (roleId) REFERENCES Role(id)
                                );";
                            new SQLiteCommand(createUser, connection).ExecuteNonQuery();

                            string createOwner = @"
                                CREATE TABLE IF NOT EXISTS Owner (
                                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    name TEXT NOT NULL,
                                    identity TEXT UNIQUE NOT NULL,
                                    phone TEXT,
                                    email TEXT,
                                    address TEXT
                                );";
                            new SQLiteCommand(createOwner, connection).ExecuteNonQuery();

                            string createPet = @"
                                CREATE TABLE IF NOT EXISTS Pet (
                                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    name TEXT NOT NULL,
                                    species TEXT,
                                    breed TEXT,
                                    age INTEGER,
                                    gender TEXT,
                                    weight REAL,
                                    ownerId INTEGER,
                                    nextVaccineDate TEXT,
                                    FOREIGN KEY (ownerId) REFERENCES Owner(id)
                                );";
                            new SQLiteCommand(createPet, connection).ExecuteNonQuery();

                            string createSupplier = @"
                                CREATE TABLE IF NOT EXISTS Supplier (
                                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    name TEXT NOT NULL,
                                    contact TEXT,
                                    phone TEXT,
                                    email TEXT
                                );";
                            new SQLiteCommand(createSupplier, connection).ExecuteNonQuery();

                            string createMedicine = @"
                                CREATE TABLE IF NOT EXISTS Medicine (
                                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    name TEXT NOT NULL,
                                    description TEXT,
                                    category TEXT,
                                    price REAL,
                                    stock INTEGER,
                                    minStock INTEGER,
                                    supplierId INTEGER,
                                    FOREIGN KEY (supplierId) REFERENCES Supplier(id)
                                );";
                            new SQLiteCommand(createMedicine, connection).ExecuteNonQuery();

                            string createAppointment = @"
                                CREATE TABLE IF NOT EXISTS Appointment (
                                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    date TEXT NOT NULL,
                                    reason TEXT,
                                    status TEXT,
                                    petId INTEGER,
                                    userId INTEGER,
                                    FOREIGN KEY (petId) REFERENCES Pet(id),
                                    FOREIGN KEY (userId) REFERENCES User(id)
                                );";
                            new SQLiteCommand(createAppointment, connection).ExecuteNonQuery();

                            string createRecord = @"
                                CREATE TABLE IF NOT EXISTS MedicalRecord (
                                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    date TEXT NOT NULL,
                                    diagnosis TEXT,
                                    treatment TEXT,
                                    medicines TEXT,
                                    notes TEXT,
                                    petId INTEGER,
                                    userId INTEGER,
                                    FOREIGN KEY (petId) REFERENCES Pet(id),
                                    FOREIGN KEY (userId) REFERENCES User(id)
                                );";
                            new SQLiteCommand(createRecord, connection).ExecuteNonQuery();

                            string createInvoice = @"
                                CREATE TABLE IF NOT EXISTS Invoice (
                                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    date TEXT NOT NULL,
                                    total REAL,
                                    tax REAL,
                                    paymentMethod TEXT,
                                    status TEXT,
                                    notes TEXT,
                                    ownerId INTEGER,
                                    FOREIGN KEY (ownerId) REFERENCES Owner(id)
                                );";
                            new SQLiteCommand(createInvoice, connection).ExecuteNonQuery();

                            string createInvoiceItem = @"
                                CREATE TABLE IF NOT EXISTS InvoiceItem (
                                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    quantity INTEGER,
                                    unitPrice REAL,
                                    invoiceId INTEGER,
                                    medicineId INTEGER,
                                    FOREIGN KEY (invoiceId) REFERENCES Invoice(id),
                                    FOREIGN KEY (medicineId) REFERENCES Medicine(id)
                                );";
                            new SQLiteCommand(createInvoiceItem, connection).ExecuteNonQuery();

                            string createAuditLog = @"
                                CREATE TABLE IF NOT EXISTS AuditLog (
                                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    userId INTEGER,
                                    action TEXT NOT NULL,
                                    entity TEXT,
                                    description TEXT,
                                    createdAt TEXT,
                                    FOREIGN KEY (userId) REFERENCES User(id)
                                );";
                            new SQLiteCommand(createAuditLog, connection).ExecuteNonQuery();

                            string insertRoleAdmin = "INSERT INTO Role (id, name, description, permissions) VALUES (1, 'ADMINISTRADOR', 'Administrador del sistema', 'ALL');";
                            string insertRoleAssistant = "INSERT INTO Role (id, name, description, permissions) VALUES (2, 'ASISTENTE', 'Asistente veterinario', 'READ,WRITE')";
                            new SQLiteCommand(insertRoleAdmin, connection).ExecuteNonQuery();
                            new SQLiteCommand(insertRoleAssistant, connection).ExecuteNonQuery();

                            string adminPasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123");
                            string assistantPasswordHash = BCrypt.Net.BCrypt.HashPassword("1234");

                            var cmdAdmin = new SQLiteCommand("INSERT INTO User (id, username, password, name, roleId) VALUES (1, 'admin', @pwd, 'Dr. Cedeño', 1);", connection);
                            cmdAdmin.Parameters.AddWithValue("@pwd", adminPasswordHash);
                            cmdAdmin.ExecuteNonQuery();

                            var cmdAsist = new SQLiteCommand("INSERT INTO User (id, username, password, name, roleId) VALUES (2, 'asist1', @pwd, 'Ana Martinez', 2);", connection);
                            cmdAsist.Parameters.AddWithValue("@pwd", assistantPasswordHash);
                            cmdAsist.ExecuteNonQuery();

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception("Error al inicializar la base de datos sqlite: " + ex.Message, ex);
                        }
                    }
                }
            }
        }

        public ConexionBD()
        {
            conexion = new SQLiteConnection(Cadena);
        }

        // Abre el puente de comunicacion entre nuestro programa y la base de datos
        public SQLiteConnection AbrirConexion()
        {
            if (conexion.State == ConnectionState.Closed)
            {
                conexion.Open();
            }
            return conexion;
        }

        // Cierra la comunicacion para ahorrar memoria y no dejar archivos bloqueados
        public void CerrarConexion()
        {
            if (conexion.State == ConnectionState.Open)
            {
                conexion.Close();
            }
        }
    }
}