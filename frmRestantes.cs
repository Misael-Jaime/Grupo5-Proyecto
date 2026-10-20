using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Grupo5_Proyecto.Database;
using Grupo5_Proyecto.Clases;
using MaterialSkin;
using MaterialSkin.Controls;

namespace Grupo5_Proyecto.Forms
{
    public class frmMascotas : frmBase
    {
        private DataGridView dgv = null!;
        private MaterialTextBox txtBuscar = null!, txtNombre = null!, txtEspecie = null!,
                                txtRaza = null!, txtEdad = null!, txtPeso = null!;
        private ComboBox cmbGenero = null!, cmbDueno = null!;
        private int idSeleccionado = 0;

        public frmMascotas()
        {
            InicializarComponentes();
            RefrescarTabla();
        }

        // Metodo principal para dibujar toda la interfaz visual de esta pantalla
        private void InicializarComponentes()
        {
            Text = "Gestion de Mascotas"; Size = new Size(1050, 680); StartPosition = FormStartPosition.CenterScreen;
            Controls.Add(CrearLabel("🐶 Gestion de Mascotas", 20, 80, true));

            txtBuscar = CrearCampo("🔍 Buscar...", 20, 120, 350);
            txtBuscar.TextChanged += (s, e) => RefrescarTabla();
            Controls.Add(txtBuscar);

            // Creamos y preparamos la tabla visual donde se mostraran los registros al usuario
            dgv = new DataGridView { Location = new Point(20, 185), Size = new Size(580, 420) };
            EstilarTabla(dgv); dgv.SelectionChanged += Dgv_SelectionChanged; Controls.Add(dgv);

            Controls.Add(CrearLabel("📝 Datos de la Mascota", 630, 80, true));
            Controls.Add(CrearLabel("Dueño *", 630, 118));
            cmbDueno = new ComboBox { Location = new Point(630, 138), Size = new Size(300, 30) };
            EstilarComboBox(cmbDueno);
            Controls.Add(cmbDueno);

            txtNombre = CrearCampo("Nombre de la mascota *", 630, 183, 300);
            txtEspecie = CrearCampo("Especie (Perro, Gato...) *", 630, 243, 300);
            txtRaza = CrearCampo("Raza", 630, 303, 145);
            txtEdad = CrearCampo("Edad (años)", 785, 303, 145);
            txtPeso = CrearCampo("Peso (kg)", 630, 363, 145);

            Controls.Add(CrearLabel("Genero", 785, 368));
            cmbGenero = new ComboBox { Location = new Point(785, 388), Size = new Size(145, 30) };
            EstilarComboBox(cmbGenero);
            cmbGenero.Items.AddRange(new object[] { "Macho", "Hembra" }); cmbGenero.SelectedIndex = 0;

            txtEdad.KeyPress += SoloEnteros_KeyPress;
            txtPeso.KeyPress += SoloDecimales_KeyPress;

            Controls.Add(txtNombre); Controls.Add(txtEspecie); Controls.Add(txtRaza);
            Controls.Add(txtEdad); Controls.Add(txtPeso); Controls.Add(cmbGenero);

            var btnG = CrearBoton("💾 Guardar", Color.FromArgb(56, 142, 60), 630, 448, 140);
            var btnE = CrearBoton("🗑 Eliminar", Color.FromArgb(211, 47, 47), 785, 448, 145);
            btnG.Click += BtnGuardar_Click; btnE.Click += BtnEliminar_Click;
            Controls.Add(btnG); Controls.Add(btnE);

            CargarDuenos();
        }

        private void CargarDuenos()
        {
            cmbDueno.Items.Clear(); cmbDueno.Items.Add("-- Seleccionar Dueño --");
            foreach (var d in DataStore.Owners) cmbDueno.Items.Add($"{d.Name} ({d.Identity})");
            cmbDueno.SelectedIndex = 0;
        }

        // Actualiza la informacion de la tabla recargandola con los datos de memoria
        private void RefrescarTabla()
        {
            dgv.SelectionChanged -= Dgv_SelectionChanged;
            // Limpiamos la tabla en pantalla antes de volver a llenarla con los datos mas recientes
            dgv.Rows.Clear(); dgv.Columns.Clear();
            dgv.Columns.Add("Id", "ID"); dgv.Columns.Add("Nombre", "Nombre");
            dgv.Columns.Add("Especie", "Especie"); dgv.Columns.Add("Raza", "Raza");
            dgv.Columns.Add("Edad", "Edad"); dgv.Columns.Add("Peso", "Peso");
            dgv.Columns.Add("Dueno", "Dueño"); dgv.Columns.Add("OwnerId", "OwnerId");
            dgv.Columns["Id"].Width = 40; dgv.Columns["OwnerId"].Visible = false;

            string f = txtBuscar?.Text.ToLower() ?? "";
            foreach (var p in DataStore.Pets.Where(p => string.IsNullOrEmpty(f) || p.Name.ToLower().Contains(f) || p.Species.ToLower().Contains(f)))
            {
                var owner = DataStore.Owners.FirstOrDefault(o => o.Id == p.OwnerId);
                dgv.Rows.Add(p.Id, p.Name, p.Species, p.Breed ?? "", p.Age?.ToString() ?? "",
                    p.Weight?.ToString("F1") ?? "", owner?.Name ?? "", p.OwnerId);
            }
            dgv.ClearSelection();
            dgv.SelectionChanged += Dgv_SelectionChanged;
        }

        private void Dgv_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0 || dgv.CurrentRow == null) return;
            idSeleccionado = Convert.ToInt32(dgv.CurrentRow.Cells["Id"].Value);
            txtNombre.Text = dgv.CurrentRow.Cells["Nombre"].Value?.ToString() ?? "";
            txtEspecie.Text = dgv.CurrentRow.Cells["Especie"].Value?.ToString() ?? "";
            txtRaza.Text = dgv.CurrentRow.Cells["Raza"].Value?.ToString() ?? "";
            txtEdad.Text = dgv.CurrentRow.Cells["Edad"].Value?.ToString() ?? "";
            txtPeso.Text = dgv.CurrentRow.Cells["Peso"].Value?.ToString() ?? "";
            int ownerId = Convert.ToInt32(dgv.CurrentRow.Cells["OwnerId"].Value);
            int idx = DataStore.Owners.FindIndex(d => d.Id == ownerId);
            cmbDueno.SelectedIndex = idx >= 0 ? idx + 1 : 0;
        }

        // Accion cuando se le da clic al boton
        // Obtenemos lo que escribio el usuario y procesamos
        private void BtnGuardar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtEspecie.Text) || cmbDueno.SelectedIndex == 0)
            { MessageBox.Show("Nombre, especie y dueño son obligatorios.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            int ownerId = DataStore.Owners[cmbDueno.SelectedIndex - 1].Id;
            int? edad = int.TryParse(txtEdad.Text, out int ea) ? ea : null;
            float? peso = float.TryParse(txtPeso.Text.Replace(",", "."), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float pe) ? pe : null;

            // Verificamos si estamos creando un registro nuevo (ID en cero) o editando uno que ya existe
            if (idSeleccionado == 0)
            {
                // Como es nuevo, lo anadimos a la lista de registros
                DataStore.Pets.Add(new Pet { Id = DataStore.NextPetId(), Name = txtNombre.Text.Trim(), Species = txtEspecie.Text.Trim(), Breed = txtRaza.Text.Trim(), Age = edad, Weight = peso, Gender = cmbGenero.SelectedItem?.ToString(), OwnerId = ownerId });
                MessageBox.Show("Mascota registrada.", "Exito");
            }
            else
            {
                var p = DataStore.Pets.FirstOrDefault(x => x.Id == idSeleccionado);
                if (p != null) { p.Name = txtNombre.Text.Trim(); p.Species = txtEspecie.Text.Trim(); p.Breed = txtRaza.Text.Trim(); p.Age = edad; p.Weight = peso; p.Gender = cmbGenero.SelectedItem?.ToString(); p.OwnerId = ownerId; }
                MessageBox.Show("Mascota actualizada.", "Exito");
            }
            DataStore.RegistrarLog(idSeleccionado == 0 ? "CREACION" : "ACTUALIZACION", "Mascota", txtNombre.Text);
            RefrescarTabla(); Limpiar();
        }

        // Accion cuando se le da clic al boton
        // Obtenemos lo que escribio el usuario y procesamos
        private void BtnEliminar_Click(object? sender, EventArgs e)
        {
            // Verificamos si estamos creando un registro nuevo (ID en cero) o editando uno que ya existe
            if (idSeleccionado == 0) { MessageBox.Show("Selecciona una mascota."); return; }
            // Le preguntamos al usuario si esta seguro de querer borrar esta informacion para evitar accidentes
            if (MessageBox.Show("¿Eliminar?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            var p = DataStore.Pets.FirstOrDefault(x => x.Id == idSeleccionado);
            if (p != null)
            {
                // Borramos el registro seleccionado de la base de datos temporal
                DataStore.Pets.Remove(p);
                DataStore.RegistrarLog("ELIMINACION", "Mascota", $"Mascota eliminada: {p.Name} ({p.Species})");
                MessageBox.Show("Eliminada.");
            }
            RefrescarTabla(); Limpiar();
        }

        private void Limpiar()
        {
            idSeleccionado = 0; txtNombre.Text = txtEspecie.Text = txtRaza.Text = txtEdad.Text = txtPeso.Text = "";
            cmbDueno.SelectedIndex = 0; cmbGenero.SelectedIndex = 0;
            dgv.SelectionChanged -= Dgv_SelectionChanged;
            dgv.ClearSelection();
            dgv.SelectionChanged += Dgv_SelectionChanged;
        }
    }

    public class frmCitas : frmBase
    {
        private DataGridView dgv = null!;
        private Panel pnlLeftContainer = null!;
        private Panel pnlSemana = null!;
        private Button btnToggleLista = null!, btnToggleSemana = null!;
        private string vistaActiva = "Semana";
        private ComboBox cmbMascota = null!, cmbVeterinario = null!, cmbEstado = null!;
        private DateTimePicker dtpFecha = null!;
        private MaterialTextBox txtMotivo = null!;
        private int idSeleccionado = 0;

        public frmCitas()
        {
            InicializarComponentes();
            SetVistaActiva("Lista");
        }

        // Metodo principal para dibujar toda la interfaz visual de esta pantalla
        private void InicializarComponentes()
        {
            Text = "Gestion de Citas"; Size = new Size(1050, 660); StartPosition = FormStartPosition.CenterScreen;
            Controls.Add(CrearLabel("📅 Calendario de Citas", 20, 80, true));

            var pnlToggle = new Panel
            {
                Location = new Point(280, 77),
                Size = new Size(160, 32),
                BackColor = Color.Transparent
            };
            pnlToggle.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    int radius = 8;
                    path.AddArc(0, 0, radius * 2, radius * 2, 180, 90);
                    path.AddArc(pnlToggle.Width - (radius * 2), 0, radius * 2, radius * 2, 270, 90);
                    path.AddArc(pnlToggle.Width - (radius * 2), pnlToggle.Height - (radius * 2), radius * 2, radius * 2, 0, 90);
                    path.AddArc(0, pnlToggle.Height - (radius * 2), radius * 2, radius * 2, 90, 90);
                    path.CloseAllFigures();

                    using (var brush = new SolidBrush(Color.FromArgb(24, 28, 48)))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                }
            };

            btnToggleLista = new Button
            {
                Text = "Lista",
                Location = new Point(2, 2),
                Size = new Size(76, 28),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnToggleLista.FlatAppearance.BorderSize = 0;
            btnToggleLista.Click += (s, e) => SetVistaActiva("Lista");

            btnToggleSemana = new Button
            {
                Text = "Semana",
                Location = new Point(82, 2),
                Size = new Size(76, 28),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnToggleSemana.FlatAppearance.BorderSize = 0;
            btnToggleSemana.Click += (s, e) => SetVistaActiva("Semana");

            pnlToggle.Controls.Add(btnToggleLista);
            pnlToggle.Controls.Add(btnToggleSemana);
            Controls.Add(pnlToggle);

            pnlLeftContainer = new Panel
            {
                Location = new Point(20, 130),
                Size = new Size(580, 450),
                BackColor = Color.Transparent
            };
            Controls.Add(pnlLeftContainer);

            // Creamos y preparamos la tabla visual donde se mostraran los registros al usuario
            dgv = new DataGridView { Dock = DockStyle.Fill };
            EstilarTabla(dgv);
            dgv.SelectionChanged += Dgv_SelectionChanged;
            pnlLeftContainer.Controls.Add(dgv);

            pnlSemana = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            pnlLeftContainer.Controls.Add(pnlSemana);

            Controls.Add(CrearLabel("📝 Datos de la Cita", 630, 80, true));

            Controls.Add(CrearLabel("Mascota *", 630, 118));
            cmbMascota = new ComboBox { Location = new Point(630, 138), Size = new Size(300, 30) };
            EstilarComboBox(cmbMascota);
            cmbMascota.Items.Add("-- Mascota --");
            foreach (var p in DataStore.Pets) cmbMascota.Items.Add(p.Name);
            cmbMascota.SelectedIndex = 0; Controls.Add(cmbMascota);

            Controls.Add(CrearLabel("Veterinario *", 630, 178));
            cmbVeterinario = new ComboBox { Location = new Point(630, 198), Size = new Size(300, 30) };
            EstilarComboBox(cmbVeterinario);
            cmbVeterinario.Items.Add("-- Veterinario --");
            foreach (var u in DataStore.Users) cmbVeterinario.Items.Add(u.Name);
            cmbVeterinario.SelectedIndex = 0; Controls.Add(cmbVeterinario);

            Controls.Add(CrearLabel("Fecha y Hora *", 630, 238));
            dtpFecha = new DateTimePicker { Location = new Point(630, 258), Size = new Size(300, 30), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm", Font = new Font("Segoe UI", 10) };
            EstilarDateTimePicker(dtpFecha);
            Controls.Add(dtpFecha);

            Controls.Add(CrearLabel("Estado", 630, 298));
            cmbEstado = new ComboBox { Location = new Point(630, 318), Size = new Size(300, 30) };
            EstilarComboBox(cmbEstado);
            cmbEstado.Items.AddRange(new object[] { "PROGRAMADA", "COMPLETADA", "CANCELADA" });
            cmbEstado.SelectedIndex = 0; Controls.Add(cmbEstado);

            txtMotivo = CrearCampo("Motivo de la consulta", 630, 368, 300); Controls.Add(txtMotivo);

            var btnG = CrearBoton("💾 Guardar", Color.FromArgb(56, 142, 60), 630, 430, 140);
            var btnE = CrearBoton("🗑 Eliminar", Color.FromArgb(211, 47, 47), 785, 430, 145);
            btnG.Click += BtnGuardar_Click; btnE.Click += BtnEliminar_Click;
            Controls.Add(btnG); Controls.Add(btnE);
        }

        private void SetVistaActiva(string vista)
        {
            vistaActiva = vista;
            if (vista == "Lista")
            {
                dgv.Visible = true;
                pnlSemana.Visible = false;

                btnToggleLista.BackColor = Color.White;
                btnToggleLista.ForeColor = Color.Black;
                btnToggleLista.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);

                btnToggleSemana.BackColor = Color.Transparent;
                btnToggleSemana.ForeColor = Color.FromArgb(148, 163, 184);
                btnToggleSemana.Font = new Font("Segoe UI", 9F);

                RefrescarTabla();
            }
            else
            {
                dgv.Visible = false;
                pnlSemana.Visible = true;

                btnToggleSemana.BackColor = Color.FromArgb(79, 70, 229);
                btnToggleSemana.ForeColor = Color.White;
                btnToggleSemana.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);

                btnToggleLista.BackColor = Color.Transparent;
                btnToggleLista.ForeColor = Color.FromArgb(148, 163, 184);
                btnToggleLista.Font = new Font("Segoe UI", 9F);

                RefrescarVistaSemanal();
            }
        }

        // Actualiza la informacion de la tabla recargandola con los datos de memoria
        private void RefrescarTabla()
        {
            dgv.SelectionChanged -= Dgv_SelectionChanged;
            // Limpiamos la tabla en pantalla antes de volver a llenarla con los datos mas recientes
            dgv.Rows.Clear(); dgv.Columns.Clear();
            dgv.Columns.Add("Id", "ID"); dgv.Columns.Add("Fecha", "Fecha");
            dgv.Columns.Add("Mascota", "Mascota"); dgv.Columns.Add("Vet", "Veterinario");
            dgv.Columns.Add("Estado", "Estado"); dgv.Columns.Add("Motivo", "Motivo");
            dgv.Columns.Add("PetId", "PetId"); dgv.Columns.Add("UserId", "UserId");
            dgv.Columns["Id"].Width = 40; dgv.Columns["PetId"].Visible = false; dgv.Columns["UserId"].Visible = false;

            foreach (var a in DataStore.Appointments.OrderByDescending(x => x.Date))
            {
                var pet = DataStore.Pets.FirstOrDefault(p => p.Id == a.PetId);
                var user = DataStore.Users.FirstOrDefault(u => u.Id == a.UserId);
                int rowIndex = dgv.Rows.Add(a.Id, a.Date.ToString("dd/MM/yyyy HH:mm"),
                    pet?.Name ?? "", user?.Name ?? "", a.Status, a.Reason ?? "", a.PetId, a.UserId);

                var cellEstado = dgv.Rows[rowIndex].Cells["Estado"];
                switch (a.Status?.ToUpper())
                {
                    case "PROGRAMADA":
                    case "SCHEDULED":
                        cellEstado.Style.ForeColor = Color.FromArgb(251, 191, 36);
                        cellEstado.Style.SelectionForeColor = Color.FromArgb(251, 191, 36);
                        cellEstado.Value = "PROGRAMADA";
                        break;
                    case "COMPLETADA":
                    case "COMPLETED":
                        cellEstado.Style.ForeColor = Color.FromArgb(52, 211, 153);
                        cellEstado.Style.SelectionForeColor = Color.FromArgb(52, 211, 153);
                        cellEstado.Value = "COMPLETADA";
                        break;
                    case "CANCELADA":
                    case "CANCELLED":
                        cellEstado.Style.ForeColor = Color.FromArgb(248, 113, 113);
                        cellEstado.Style.SelectionForeColor = Color.FromArgb(248, 113, 113);
                        cellEstado.Value = "CANCELADA";
                        break;
                }
            }
            dgv.ClearSelection();
            dgv.SelectionChanged += Dgv_SelectionChanged;
        }

        private void RefrescarVistaSemanal()
        {
            pnlSemana.Controls.Clear();

            DateTime[] dias = new DateTime[] {
                DateTime.Today,
                DateTime.Today.AddDays(1),
                DateTime.Today.AddDays(2)
            };

            int xPos = 10;
            foreach (var date in dias)
            {
                var pnlCard = new Panel
                {
                    Location = new Point(xPos, 10),
                    Size = new Size(175, 430),
                    BackColor = Color.Transparent
                };

                pnlCard.Paint += (s, e) =>
                {
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                    {
                        int radius = 12;
                        path.AddArc(0, 0, radius * 2, radius * 2, 180, 90);
                        path.AddArc(pnlCard.Width - (radius * 2), 0, radius * 2, radius * 2, 270, 90);
                        path.AddArc(pnlCard.Width - (radius * 2), pnlCard.Height - (radius * 2), radius * 2, radius * 2, 0, 90);
                        path.AddArc(0, pnlCard.Height - (radius * 2), radius * 2, radius * 2, 90, 90);
                        path.CloseAllFigures();

                        using (var brush = new SolidBrush(Color.FromArgb(24, 28, 48)))
                        {
                            e.Graphics.FillPath(brush, path);
                        }

                        Color borderColor = (date.Date == DateTime.Today) ? Color.FromArgb(99, 102, 241) : Color.FromArgb(30, 41, 59);
                        int borderThickness = (date.Date == DateTime.Today) ? 2 : 1;

                        using (var pen = new Pen(borderColor, borderThickness))
                        {
                            e.Graphics.DrawPath(pen, path);
                        }
                    }
                };

                string dayNameStr = date.ToString("ddd").ToUpper();
                if (date.Date == DateTime.Today) dayNameStr = "HOY";
                else if (date.Date == DateTime.Today.AddDays(1)) dayNameStr = "MAnANA";

                var lblDayName = new Label
                {
                    Text = dayNameStr,
                    ForeColor = Color.FromArgb(148, 163, 184),
                    Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
                    Location = new Point(0, 15),
                    Size = new Size(175, 18),
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Color.Transparent
                };
                pnlCard.Controls.Add(lblDayName);

                var lblDayNum = new Label
                {
                    Text = date.Day.ToString(),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                    Location = new Point(0, 33),
                    Size = new Size(175, 30),
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Color.Transparent
                };
                pnlCard.Controls.Add(lblDayNum);

                var sep = new Panel
                {
                    Location = new Point(15, 75),
                    Size = new Size(145, 1),
                    BackColor = Color.FromArgb(30, 41, 59)
                };
                pnlCard.Controls.Add(sep);

                var flowAppointments = new FlowLayoutPanel
                {
                    Location = new Point(8, 85),
                    Size = new Size(160, 335),
                    FlowDirection = FlowDirection.TopDown,
                    WrapContents = false,
                    AutoScroll = true,
                    BackColor = Color.Transparent
                };

                var appointments = DataStore.Appointments.Where(a => a.Date.Date == date.Date).OrderBy(a => a.Date).ToList();
                if (appointments.Count == 0)
                {
                    var lblLibre = new Label
                    {
                        Text = "Libre",
                        ForeColor = Color.FromArgb(71, 85, 105),
                        Font = new Font("Segoe UI", 10F, FontStyle.Italic),
                        Size = new Size(138, 100),
                        TextAlign = ContentAlignment.MiddleCenter,
                        BackColor = Color.Transparent
                    };
                    flowAppointments.Controls.Add(lblLibre);
                }
                else
                {
                    foreach (var a in appointments)
                    {
                        var pnlAppCard = new Panel
                        {
                            Size = new Size(138, 82),
                            BackColor = Color.Transparent,
                            Margin = new Padding(0, 0, 0, 8),
                            Cursor = Cursors.Hand
                        };

                        string statusDisplay;
                        Color statusColor;
                        switch (a.Status?.ToUpper())
                        {
                            case "COMPLETADA":
                            case "COMPLETED":
                                statusDisplay = "COMPLETADA"; statusColor = Color.FromArgb(52, 211, 153); break;
                            case "CANCELADA":
                            case "CANCELLED":
                                statusDisplay = "CANCELADA"; statusColor = Color.FromArgb(248, 113, 113); break;
                            default:
                                statusDisplay = "PROGRAMADA"; statusColor = Color.FromArgb(251, 191, 36); break;
                        }

                        pnlAppCard.Paint += (s, e) =>
                        {
                            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                            using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                            {
                                int radius = 6;
                                path.AddArc(0, 0, radius * 2, radius * 2, 180, 90);
                                path.AddArc(pnlAppCard.Width - (radius * 2), 0, radius * 2, radius * 2, 270, 90);
                                path.AddArc(pnlAppCard.Width - (radius * 2), pnlAppCard.Height - (radius * 2), radius * 2, radius * 2, 0, 90);
                                path.AddArc(0, pnlAppCard.Height - (radius * 2), radius * 2, radius * 2, 90, 90);
                                path.CloseAllFigures();

                                using (var brush = new SolidBrush(Color.FromArgb(37, 45, 78)))
                                {
                                    e.Graphics.FillPath(brush, path);
                                }

                                if (a.Id == idSeleccionado)
                                {
                                    using (var pen = new Pen(Color.FromArgb(99, 102, 241), 2))
                                    {
                                        e.Graphics.DrawPath(pen, path);
                                    }
                                }
                            }
                        };

                        var lblTime = new Label
                        {
                            Text = a.Date.ToString("hh:mm tt"),
                            ForeColor = Color.FromArgb(56, 189, 248),
                            Font = new Font("Segoe UI Semibold", 8F, FontStyle.Bold),
                            Location = new Point(8, 6),
                            Size = new Size(122, 14),
                            BackColor = Color.Transparent
                        };
                        pnlAppCard.Controls.Add(lblTime);

                        var pet = DataStore.Pets.FirstOrDefault(p => p.Id == a.PetId);
                        var lblPetName = new Label
                        {
                            Text = pet?.Name ?? "Mascota",
                            ForeColor = Color.White,
                            Font = new Font("Segoe UI Bold", 9F, FontStyle.Bold),
                            Location = new Point(8, 22),
                            Size = new Size(122, 18),
                            BackColor = Color.Transparent
                        };
                        pnlAppCard.Controls.Add(lblPetName);

                        var lblReason = new Label
                        {
                            Text = a.Reason ?? "Consulta",
                            ForeColor = Color.FromArgb(148, 163, 184),
                            Font = new Font("Segoe UI", 7.5F, FontStyle.Regular),
                            Location = new Point(8, 42),
                            Size = new Size(122, 16),
                            BackColor = Color.Transparent
                        };
                        pnlAppCard.Controls.Add(lblReason);

                        var lblStatus = new Label
                        {
                            Text = statusDisplay,
                            ForeColor = statusColor,
                            Font = new Font("Segoe UI Semibold", 7F, FontStyle.Bold),
                            Location = new Point(8, 60),
                            Size = new Size(122, 16),
                            BackColor = Color.Transparent
                        };
                        pnlAppCard.Controls.Add(lblStatus);

                        Action selectAppointment = () =>
                        {
                            idSeleccionado = a.Id;
                            dtpFecha.Value = a.Date;
                            txtMotivo.Text = a.Reason ?? "";
                            cmbEstado.SelectedItem = a.Status;
                            int pi = DataStore.Pets.FindIndex(p => p.Id == a.PetId);
                            int ui = DataStore.Users.FindIndex(u => u.Id == a.UserId);
                            cmbMascota.SelectedIndex = pi >= 0 ? pi + 1 : 0;
                            cmbVeterinario.SelectedIndex = ui >= 0 ? ui + 1 : 0;

                            RefrescarTabla();
                            RefrescarVistaSemanal();
                        };

                        pnlAppCard.Click += (s, e) => selectAppointment();
                        lblTime.Click += (s, e) => selectAppointment();
                        lblPetName.Click += (s, e) => selectAppointment();
                        lblReason.Click += (s, e) => selectAppointment();
                        lblStatus.Click += (s, e) => selectAppointment();

                        flowAppointments.Controls.Add(pnlAppCard);
                    }
                }

                pnlCard.Controls.Add(flowAppointments);
                pnlSemana.Controls.Add(pnlCard);
                xPos += 185;
            }
        }

        private void Dgv_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0 || dgv.CurrentRow == null) return;
            idSeleccionado = Convert.ToInt32(dgv.CurrentRow.Cells["Id"].Value);
            if (DateTime.TryParse(dgv.CurrentRow.Cells["Fecha"].Value?.ToString(), out DateTime f)) dtpFecha.Value = f;
            txtMotivo.Text = dgv.CurrentRow.Cells["Motivo"].Value?.ToString() ?? "";
            cmbEstado.SelectedItem = dgv.CurrentRow.Cells["Estado"].Value?.ToString() ?? "PROGRAMADA";
            int petId = Convert.ToInt32(dgv.CurrentRow.Cells["PetId"].Value);
            int userId = Convert.ToInt32(dgv.CurrentRow.Cells["UserId"].Value);
            int pi = DataStore.Pets.FindIndex(p => p.Id == petId);
            int ui = DataStore.Users.FindIndex(u => u.Id == userId);
            cmbMascota.SelectedIndex = pi >= 0 ? pi + 1 : 0;
            cmbVeterinario.SelectedIndex = ui >= 0 ? ui + 1 : 0;
        }

        // Accion cuando se le da clic al boton
        // Obtenemos lo que escribio el usuario y procesamos
        private void BtnGuardar_Click(object? sender, EventArgs e)
        {
            if (cmbMascota.SelectedIndex == 0 || cmbVeterinario.SelectedIndex == 0)
            { MessageBox.Show("Mascota y veterinario son obligatorios.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            int petId = DataStore.Pets[cmbMascota.SelectedIndex - 1].Id;
            int userId = DataStore.Users[cmbVeterinario.SelectedIndex - 1].Id;

            // Verificamos si estamos creando un registro nuevo (ID en cero) o editando uno que ya existe
            if (idSeleccionado == 0)
            {
                // Como es nuevo, lo anadimos a la lista de registros
                DataStore.Appointments.Add(new Appointment { Id = DataStore.NextAppointmentId(), Date = dtpFecha.Value, Reason = txtMotivo.Text.Trim(), Status = cmbEstado.SelectedItem?.ToString() ?? "PROGRAMADA", PetId = petId, UserId = userId });
                MessageBox.Show("Cita registrada.", "Exito");
            }
            else
            {
                var a = DataStore.Appointments.FirstOrDefault(x => x.Id == idSeleccionado);
                if (a != null) { a.Date = dtpFecha.Value; a.Reason = txtMotivo.Text.Trim(); a.Status = cmbEstado.SelectedItem?.ToString() ?? "PROGRAMADA"; a.PetId = petId; a.UserId = userId; }
                MessageBox.Show("Cita actualizada.", "Exito");
            }
            DataStore.RegistrarLog(idSeleccionado == 0 ? "CREACION" : "ACTUALIZACION", "Cita", $"Cita {(idSeleccionado == 0 ? "registrada" : "actualizada")} para mascota ID {petId} con motivo: {txtMotivo.Text.Trim()}");
            Limpiar();
        }

        // Accion cuando se le da clic al boton
        // Obtenemos lo que escribio el usuario y procesamos
        private void BtnEliminar_Click(object? sender, EventArgs e)
        {
            // Verificamos si estamos creando un registro nuevo (ID en cero) o editando uno que ya existe
            if (idSeleccionado == 0) { MessageBox.Show("Selecciona una cita."); return; }
            // Le preguntamos al usuario si esta seguro de querer borrar esta informacion para evitar accidentes
            if (MessageBox.Show("¿Eliminar?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            var a = DataStore.Appointments.FirstOrDefault(x => x.Id == idSeleccionado);
            if (a != null)
            {
                // Borramos el registro seleccionado de la base de datos temporal
                DataStore.Appointments.Remove(a);
                DataStore.RegistrarLog("ELIMINACION", "Cita", $"Cita eliminada para mascota ID {a.PetId}");
                MessageBox.Show("Eliminada.");
            }
            Limpiar();
        }

        private void Limpiar()
        {
            idSeleccionado = 0; txtMotivo.Text = "";
            cmbMascota.SelectedIndex = cmbVeterinario.SelectedIndex = 0; cmbEstado.SelectedIndex = 0;
            dtpFecha.Value = DateTime.Now;
            dgv.SelectionChanged -= Dgv_SelectionChanged;
            dgv.ClearSelection();
            SetVistaActiva(vistaActiva);
            dgv.ClearSelection();
            dgv.SelectionChanged += Dgv_SelectionChanged;
        }
    }

    public class frmHistoriales : frmBase
    {
        private DataGridView dgv = null!;
        private ComboBox cmbMascota = null!, cmbVet = null!;
        private MaterialTextBox txtDiag = null!, txtTrat = null!, txtMed = null!, txtNota = null!;
        private int idSeleccionado = 0;
        private MaterialTextBox txtBuscar = null!;

        public frmHistoriales()
        {
            InicializarComponentes(); RefrescarTabla();
        }

        // Metodo principal para dibujar toda la interfaz visual de esta pantalla
        private void InicializarComponentes()
        {
            Text = "Historiales Medicos"; Size = new Size(1050, 680); StartPosition = FormStartPosition.CenterScreen;
            Controls.Add(CrearLabel("📋 Historiales Medicos", 20, 80, true));

            // Creamos y preparamos la tabla visual donde se mostraran los registros al usuario
            dgv = new DataGridView { Location = new Point(20, 130), Size = new Size(580, 470) };
            EstilarTabla(dgv); dgv.SelectionChanged += Dgv_SelectionChanged; Controls.Add(dgv);

            txtBuscar = CrearCampo("🔍 Buscar...", 20, 110, 580);
            txtBuscar.TextChanged += TxtBuscar_TextChanged;
            Controls.Add(txtBuscar);

            dgv.Location = new Point(20, 155);
            dgv.Size = new Size(580, 445);

            Controls.Add(CrearLabel("📝 Nuevo Historial", 630, 80, true));

            Controls.Add(CrearLabel("Mascota *", 630, 115));
            cmbMascota = new ComboBox { Location = new Point(630, 135), Size = new Size(300, 30) };
            EstilarComboBox(cmbMascota);
            cmbMascota.Items.Add("-- Mascota --");
            foreach (var p in DataStore.Pets) cmbMascota.Items.Add(p.Name);
            cmbMascota.SelectedIndex = 0; Controls.Add(cmbMascota);

            Controls.Add(CrearLabel("Veterinario", 630, 175));
            cmbVet = new ComboBox { Location = new Point(630, 195), Size = new Size(300, 30) };
            EstilarComboBox(cmbVet);
            cmbVet.Items.Add("-- Veterinario --");
            foreach (var u in DataStore.Users) cmbVet.Items.Add(u.Name);
            cmbVet.SelectedIndex = 0; Controls.Add(cmbVet);

            txtDiag = CrearCampo("Diagnostico *", 630, 240, 300);
            txtTrat = CrearCampo("Tratamiento *", 630, 300, 300);
            txtMed = CrearCampo("Medicamentos", 630, 360, 300);
            txtNota = CrearCampo("Notas", 630, 420, 300);
            Controls.Add(txtDiag); Controls.Add(txtTrat); Controls.Add(txtMed); Controls.Add(txtNota);

            var btnG = CrearBoton("💾 Guardar", Color.FromArgb(56, 142, 60), 630, 490, 140);
            var btnE = CrearBoton("🗑 Eliminar", Color.FromArgb(211, 47, 47), 785, 490, 145);
            btnG.Click += BtnGuardar_Click; btnE.Click += BtnEliminar_Click;
            Controls.Add(btnG); Controls.Add(btnE);
        }

        // Actualiza la informacion de la tabla recargandola con los datos de memoria
        private void RefrescarTabla()
        {
            dgv.SelectionChanged -= Dgv_SelectionChanged;
            // Limpiamos la tabla en pantalla antes de volver a llenarla con los datos mas recientes
            dgv.Rows.Clear();
            dgv.Columns.Clear();

            dgv.Columns.Add("Id", "ID");
            dgv.Columns.Add("Fecha", "Fecha");
            dgv.Columns.Add("Mascota", "Mascota");
            dgv.Columns.Add("Diagnostico", "Diagnostico");
            dgv.Columns.Add("Tratamiento", "Tratamiento");
            dgv.Columns.Add("Medicamentos", "Medicamentos");
            dgv.Columns.Add("Notas", "Notas");
            dgv.Columns.Add("PetId", "PetId");
            dgv.Columns.Add("UserId", "UserId");

            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgv.Columns["Id"].Width = 35;
            dgv.Columns["Id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;

            dgv.Columns["Fecha"].Width = 85;
            dgv.Columns["Fecha"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;

            dgv.Columns["Mascota"].FillWeight = 100;
            dgv.Columns["Diagnostico"].FillWeight = 120;
            dgv.Columns["Tratamiento"].FillWeight = 120;
            dgv.Columns["Medicamentos"].FillWeight = 120;
            dgv.Columns["Notas"].FillWeight = 140;

            dgv.Columns["PetId"].Visible = false;
            dgv.Columns["UserId"].Visible = false;

            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgv.Columns["Id"].Width = 35;
            dgv.Columns["Id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgv.Columns["Id"].DisplayIndex = 0;

            dgv.Columns["Fecha"].Width = 85;
            dgv.Columns["Fecha"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgv.Columns["Fecha"].DisplayIndex = 1;

            dgv.Columns["Mascota"].FillWeight = 100;
            dgv.Columns["Mascota"].DisplayIndex = 2;

            dgv.Columns["Diagnostico"].FillWeight = 120;
            dgv.Columns["Diagnostico"].DisplayIndex = 3;

            dgv.Columns["Tratamiento"].FillWeight = 120;
            dgv.Columns["Tratamiento"].DisplayIndex = 4;

            dgv.Columns["Medicamentos"].FillWeight = 120;
            dgv.Columns["Medicamentos"].DisplayIndex = 5;

            dgv.Columns["Notas"].FillWeight = 140;
            dgv.Columns["Notas"].DisplayIndex = 6;

            dgv.Columns["PetId"].Visible = false;
            dgv.Columns["UserId"].Visible = false;

            foreach (var r in DataStore.MedicalRecords.OrderByDescending(x => x.Date))
            {
                var pet = DataStore.Pets.FirstOrDefault(p => p.Id == r.PetId);

                int rowIndex = dgv.Rows.Add(
                    r.Id,
                    r.Date.ToString("dd/MM/yyyy"),
                    pet?.Name ?? "Minino",
                    r.Diagnosis,
                    r.Treatment ?? "",
                    r.Medicines ?? "",
                    r.Notes ?? "",
                    r.PetId,
                    r.UserId
                );

                var row = dgv.Rows[rowIndex];
                var cellFecha = row.Cells["Fecha"];

                double diasTranscurridos = (DateTime.Today - r.Date.Date).TotalDays;

                if (diasTranscurridos <= 7)
                {
                    cellFecha.Style.ForeColor = Color.FromArgb(52, 211, 153);
                    cellFecha.Style.SelectionForeColor = Color.FromArgb(52, 211, 153);
                }
                else
                {
                    cellFecha.Style.ForeColor = Color.FromArgb(251, 191, 36);
                    cellFecha.Style.SelectionForeColor = Color.FromArgb(251, 191, 36);
                }
            }
            dgv.ClearSelection();
            dgv.SelectionChanged += Dgv_SelectionChanged;
        }

        private void Dgv_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0 || dgv.CurrentRow == null) return;
            idSeleccionado = Convert.ToInt32(dgv.CurrentRow.Cells["Id"].Value);
            txtDiag.Text = dgv.CurrentRow.Cells["Diagnostico"].Value?.ToString() ?? "";
            int petId = Convert.ToInt32(dgv.CurrentRow.Cells["PetId"].Value);
            int pi = DataStore.Pets.FindIndex(p => p.Id == petId);
            cmbMascota.SelectedIndex = pi >= 0 ? pi + 1 : 0;
        }

        // Accion cuando se le da clic al boton
        // Obtenemos lo que escribio el usuario y procesamos
        private void BtnGuardar_Click(object? sender, EventArgs e)
        {
            if (cmbMascota.SelectedIndex == 0 || string.IsNullOrWhiteSpace(txtDiag.Text))
            { MessageBox.Show("Mascota y diagnostico son obligatorios."); return; }

            int petId = DataStore.Pets[cmbMascota.SelectedIndex - 1].Id;
            int userId = cmbVet.SelectedIndex > 0 ? DataStore.Users[cmbVet.SelectedIndex - 1].Id : SessionManager.UsuarioActual?.Id ?? 1;

            // Verificamos si estamos creando un registro nuevo (ID en cero) o editando uno que ya existe
            if (idSeleccionado == 0)
            {
                var n = new MedicalRecord { Id = DataStore.NextRecordId(), Date = DateTime.Now, Diagnosis = txtDiag.Text.Trim(), Treatment = txtTrat.Text.Trim(), Medicines = txtMed.Text.Trim(), Notes = txtNota.Text.Trim(), PetId = petId, UserId = userId };
                DataStore.MedicalRecords.Add(n);
                var petName = DataStore.Pets.FirstOrDefault(p => p.Id == petId)?.Name ?? "";
                DataStore.RegistrarLog("CREACION", "Historial", $"Historial medico creado para mascota {petName}. Diag: {n.Diagnosis}");
                MessageBox.Show("Historial guardado.", "Exito");
            }
            else
            {
                var rec = DataStore.MedicalRecords.FirstOrDefault(x => x.Id == idSeleccionado);
                if (rec != null)
                {
                    rec.Diagnosis = txtDiag.Text.Trim();
                    rec.Treatment = txtTrat.Text.Trim();
                    rec.Medicines = txtMed.Text.Trim();
                    rec.Notes = txtNota.Text.Trim();
                    rec.PetId = petId;
                    rec.UserId = userId;
                    var petName = DataStore.Pets.FirstOrDefault(p => p.Id == petId)?.Name ?? "";
                    DataStore.RegistrarLog("ACTUALIZACION", "Historial", $"Historial medico actualizado para mascota {petName}. Diag: {rec.Diagnosis}");
                    MessageBox.Show("Historial actualizado.", "Exito");
                }
            }
            LimpiarFormulario(); RefrescarTabla();
        }

        // Accion cuando se le da clic al boton
        // Obtenemos lo que escribio el usuario y procesamos
        private void BtnEliminar_Click(object? sender, EventArgs e)
        {
            // Verificamos si estamos creando un registro nuevo (ID en cero) o editando uno que ya existe
            if (idSeleccionado == 0) { MessageBox.Show("Selecciona un historial."); return; }
            // Le preguntamos al usuario si esta seguro de querer borrar esta informacion para evitar accidentes
            if (MessageBox.Show("¿Eliminar?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            var r = DataStore.MedicalRecords.FirstOrDefault(x => x.Id == idSeleccionado);
            if (r != null)
            {
                DataStore.MedicalRecords.Remove(r);
                var petName = DataStore.Pets.FirstOrDefault(p => p.Id == r.PetId)?.Name ?? "";
                DataStore.RegistrarLog("ELIMINACION", "Historial", $"Historial medico eliminado para mascota {petName}");
                MessageBox.Show("Eliminado.");
            }
            LimpiarFormulario(); RefrescarTabla();
        }

        private void LimpiarFormulario()
        {
            idSeleccionado = 0;
            dgv.SelectionChanged -= Dgv_SelectionChanged;
            dgv.ClearSelection();
            dgv.SelectionChanged += Dgv_SelectionChanged;
            txtDiag.Text = txtTrat.Text = txtMed.Text = txtNota.Text = "";
            cmbMascota.SelectedIndex = 0;
            cmbVet.SelectedIndex = 0;
        }
        private void TxtBuscar_TextChanged(object? sender, EventArgs e)
        {
            string filtro = txtBuscar.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(filtro))
            {
                RefrescarTabla();
                return;
            }

            dgv.SelectionChanged -= Dgv_SelectionChanged;

            var registrosFiltrados = DataStore.MedicalRecords
                .Where(r =>
                {
                    var pet = DataStore.Pets.FirstOrDefault(p => p.Id == r.PetId);
                    string nombreMascota = pet?.Name?.ToLower() ?? "";
                    string diagnostico = r.Diagnosis?.ToLower() ?? "";
                    string tratamiento = r.Treatment?.ToLower() ?? "";

                    return nombreMascota.Contains(filtro) ||
                           diagnostico.Contains(filtro) ||
                           tratamiento.Contains(filtro);
                })
                .OrderByDescending(x => x.Date)
                .ToList();

            // Limpiamos la tabla en pantalla antes de volver a llenarla con los datos mas recientes
            dgv.Rows.Clear();
            foreach (var r in registrosFiltrados)
            {
                var pet = DataStore.Pets.FirstOrDefault(p => p.Id == r.PetId);
                int rowIndex = dgv.Rows.Add(
                    r.Id,
                    r.Date.ToString("dd/MM/yyyy"),
                    pet?.Name ?? "Minino",
                    r.Diagnosis,
                    r.Treatment ?? "",
                    r.Medicines ?? "",
                    r.Notes ?? "",
                    r.PetId,
                    r.UserId
                );

                var cellFecha = dgv.Rows[rowIndex].Cells["Fecha"];
                double diasTranscurridos = (DateTime.Today - r.Date.Date).TotalDays;

                if (diasTranscurridos <= 7)
                {
                    cellFecha.Style.ForeColor = Color.FromArgb(52, 211, 153);
                    cellFecha.Style.SelectionForeColor = Color.FromArgb(52, 211, 153);
                }
                else
                {
                    cellFecha.Style.ForeColor = Color.FromArgb(251, 191, 36);
                    cellFecha.Style.SelectionForeColor = Color.FromArgb(251, 191, 36);
                }
            }
            dgv.ClearSelection();
            dgv.SelectionChanged += Dgv_SelectionChanged;
        }
    }

    public class frmMedicamentos : frmBase
    {
        private DataGridView dgv = null!;
        private MaterialTextBox txtNombre = null!, txtDesc = null!, txtCat = null!, txtPrecio = null!, txtStock = null!, txtStockMin = null!;
        private int idSeleccionado = 0;

        public frmMedicamentos()
        {
            InicializarComponentes(); RefrescarTabla();
        }

        // Metodo principal para dibujar toda la interfaz visual de esta pantalla
        private void InicializarComponentes()
        {
            Text = "Gestion de Medicamentos"; Size = new Size(1000, 650); StartPosition = FormStartPosition.CenterScreen;
            Controls.Add(CrearLabel("💊 Gestion de Medicamentos", 20, 80, true));

            // Creamos y preparamos la tabla visual donde se mostraran los registros al usuario
            dgv = new DataGridView { Location = new Point(20, 130), Size = new Size(570, 450) };
            EstilarTabla(dgv); dgv.SelectionChanged += Dgv_SelectionChanged; Controls.Add(dgv);

            Controls.Add(CrearLabel("📝 Datos del Medicamento", 615, 80, true));
            txtNombre = CrearCampo("Nombre *", 615, 120, 310);
            txtDesc = CrearCampo("Descripcion", 615, 180, 310);
            txtCat = CrearCampo("Categoria", 615, 240, 310);
            txtPrecio = CrearCampo("Precio (B/.)", 615, 300, 145);
            txtStock = CrearCampo("Stock actual", 775, 300, 150);
            txtStockMin = CrearCampo("Stock minimo", 615, 360, 145);
            txtPrecio.KeyPress += SoloDecimales_KeyPress;
            txtStock.KeyPress += SoloEnteros_KeyPress;
            txtStockMin.KeyPress += SoloEnteros_KeyPress;

            Controls.Add(txtNombre); Controls.Add(txtDesc); Controls.Add(txtCat);
            Controls.Add(txtPrecio); Controls.Add(txtStock); Controls.Add(txtStockMin);

            var btnG = CrearBoton("💾 Guardar", Color.FromArgb(56, 142, 60), 615, 430, 140);
            var btnE = CrearBoton("🗑 Eliminar", Color.FromArgb(211, 47, 47), 770, 430, 155);
            btnG.Click += BtnGuardar_Click; btnE.Click += BtnEliminar_Click;
            Controls.Add(btnG); Controls.Add(btnE);
        }

        // Actualiza la informacion de la tabla recargandola con los datos de memoria
        private void RefrescarTabla()
        {
            dgv.SelectionChanged -= Dgv_SelectionChanged;
            // Limpiamos la tabla en pantalla antes de volver a llenarla con los datos mas recientes
            dgv.Rows.Clear(); dgv.Columns.Clear();
            dgv.Columns.Add("Id", "ID"); dgv.Columns.Add("Nombre", "Nombre");
            dgv.Columns.Add("Cat", "Categoria"); dgv.Columns.Add("Precio", "Precio");
            dgv.Columns.Add("Stock", "Stock"); dgv.Columns.Add("Min", "Min.");
            dgv.Columns["Id"].Width = 40;

            foreach (var m in DataStore.Medicines)
            {
                int rowIndex = dgv.Rows.Add(m.Id, m.Name, m.Category ?? "",
                    $"B/. {m.Price:F2}", m.Stock, m.MinStock);

                var cellStock = dgv.Rows[rowIndex].Cells["Stock"];

                if (m.Stock == 0)
                {
                    cellStock.Style.ForeColor = Color.FromArgb(248, 113, 113);
                    cellStock.Style.SelectionForeColor = Color.FromArgb(248, 113, 113);
                    cellStock.Style.Font = new Font(dgv.Font, FontStyle.Bold);
                }
                else if (m.Stock <= m.MinStock)
                {
                    cellStock.Style.ForeColor = Color.FromArgb(251, 191, 36);
                    cellStock.Style.SelectionForeColor = Color.FromArgb(251, 191, 36);
                }
                else
                {
                    cellStock.Style.ForeColor = Color.FromArgb(52, 211, 153);
                    cellStock.Style.SelectionForeColor = Color.FromArgb(52, 211, 153);
                }
            }
            dgv.ClearSelection();
            dgv.SelectionChanged += Dgv_SelectionChanged;
        }

        private void Dgv_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0 || dgv.CurrentRow == null) return;
            idSeleccionado = Convert.ToInt32(dgv.CurrentRow.Cells["Id"].Value);
            txtNombre.Text = dgv.CurrentRow.Cells["Nombre"].Value?.ToString() ?? "";
            txtCat.Text = dgv.CurrentRow.Cells["Cat"].Value?.ToString() ?? "";
            txtStock.Text = dgv.CurrentRow.Cells["Stock"].Value?.ToString() ?? "";
            txtStockMin.Text = dgv.CurrentRow.Cells["Min"].Value?.ToString() ?? "";
            txtPrecio.Text = dgv.CurrentRow.Cells["Precio"].Value?.ToString()?.Replace("B/. ", "") ?? "";
        }

        // Accion cuando se le da clic al boton
        // Obtenemos lo que escribio el usuario y procesamos
        private void BtnGuardar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text)) { MessageBox.Show("El nombre es obligatorio."); return; }
            float precio = float.TryParse(txtPrecio.Text.Replace(",", "."), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float pp) ? pp : 0;
            int stock = int.TryParse(txtStock.Text, out int ss) ? ss : 0;
            int stockMin = int.TryParse(txtStockMin.Text, out int sm) ? sm : 10;

            // Verificamos si estamos creando un registro nuevo (ID en cero) o editando uno que ya existe
            if (idSeleccionado == 0)
            {
                DataStore.Medicines.Add(new Medicine { Id = DataStore.NextMedicineId(), Name = txtNombre.Text.Trim(), Description = txtDesc.Text.Trim(), Category = txtCat.Text.Trim(), Price = precio, Stock = stock, MinStock = stockMin });
                MessageBox.Show("Medicamento registrado.", "Exito");
            }
            else
            {
                var m = DataStore.Medicines.FirstOrDefault(x => x.Id == idSeleccionado);
                if (m != null) { m.Name = txtNombre.Text.Trim(); m.Description = txtDesc.Text.Trim(); m.Category = txtCat.Text.Trim(); m.Price = precio; m.Stock = stock; m.MinStock = stockMin; }
                MessageBox.Show("Actualizado.", "Exito");
            }
            DataStore.RegistrarLog(idSeleccionado == 0 ? "CREACION" : "ACTUALIZACION", "Medicamento", $"Medicamento {(idSeleccionado == 0 ? "registrado" : "actualizado")}: {txtNombre.Text.Trim()}");
            RefrescarTabla(); Limpiar();
        }

        // Accion cuando se le da clic al boton
        // Obtenemos lo que escribio el usuario y procesamos
        private void BtnEliminar_Click(object? sender, EventArgs e)
        {
            // Verificamos si estamos creando un registro nuevo (ID en cero) o editando uno que ya existe
            if (idSeleccionado == 0) { MessageBox.Show("Selecciona un medicamento."); return; }
            // Le preguntamos al usuario si esta seguro de querer borrar esta informacion para evitar accidentes
            if (MessageBox.Show("¿Eliminar?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            var m = DataStore.Medicines.FirstOrDefault(x => x.Id == idSeleccionado);
            if (m != null)
            {
                DataStore.Medicines.Remove(m);
                DataStore.RegistrarLog("ELIMINACION", "Medicamento", $"Medicamento eliminado: {m.Name}");
                MessageBox.Show("Eliminado.");
            }
            RefrescarTabla(); Limpiar();
        }

        private void Limpiar()
        {
            idSeleccionado = 0;
            txtNombre.Text = txtDesc.Text = txtCat.Text = txtPrecio.Text = txtStock.Text = txtStockMin.Text = "";
            dgv.SelectionChanged -= Dgv_SelectionChanged;
            dgv.ClearSelection();
            dgv.SelectionChanged += Dgv_SelectionChanged;
        }
    }

    public class frmProveedores : frmBase
    {
        private DataGridView dgv = null!;
        private MaterialTextBox txtNombre = null!, txtContacto = null!, txtTel = null!, txtEmail = null!;
        private int idSeleccionado = 0;

        public frmProveedores()
        {
            InicializarComponentes(); RefrescarTabla();
        }

        // Metodo principal para dibujar toda la interfaz visual de esta pantalla
        private void InicializarComponentes()
        {
            Text = "Gestion de Proveedores"; Size = new Size(900, 580); StartPosition = FormStartPosition.CenterScreen;
            Controls.Add(CrearLabel("🏭 Gestion de Proveedores", 20, 80, true));

            // Creamos y preparamos la tabla visual donde se mostraran los registros al usuario
            dgv = new DataGridView { Location = new Point(20, 130), Size = new Size(520, 380) };
            EstilarTabla(dgv); dgv.SelectionChanged += Dgv_SelectionChanged; Controls.Add(dgv);

            Controls.Add(CrearLabel("📝 Datos del Proveedor", 570, 80, true));
            txtNombre = CrearCampo("Nombre *", 570, 120, 280);
            txtContacto = CrearCampo("Contacto", 570, 180, 280);
            txtTel = CrearCampo("Telefono", 570, 240, 280);
            txtEmail = CrearCampo("Email", 570, 300, 280);
            txtTel.KeyPress += SoloTelefono_KeyPress;

            Controls.Add(txtNombre); Controls.Add(txtContacto); Controls.Add(txtTel); Controls.Add(txtEmail);

            var btnG = CrearBoton("💾 Guardar", Color.FromArgb(56, 142, 60), 570, 370, 130);
            var btnE = CrearBoton("🗑 Eliminar", Color.FromArgb(211, 47, 47), 715, 370, 130);
            btnG.Click += BtnGuardar_Click; btnE.Click += BtnEliminar_Click;
            Controls.Add(btnG); Controls.Add(btnE);
        }

        // Actualiza la informacion de la tabla recargandola con los datos de memoria
        private void RefrescarTabla()
        {
            dgv.SelectionChanged -= Dgv_SelectionChanged;
            // Limpiamos la tabla en pantalla antes de volver a llenarla con los datos mas recientes
            dgv.Rows.Clear(); dgv.Columns.Clear();
            dgv.Columns.Add("Id", "ID"); dgv.Columns.Add("Nombre", "Nombre");
            dgv.Columns.Add("Contacto", "Contacto"); dgv.Columns.Add("Tel", "Telefono"); dgv.Columns.Add("Email", "Email");
            dgv.Columns["Id"].Width = 40;
            foreach (var s in DataStore.Suppliers)
                dgv.Rows.Add(s.Id, s.Name, s.Contact ?? "", s.Phone ?? "", s.Email ?? "");
            dgv.ClearSelection();
            dgv.SelectionChanged += Dgv_SelectionChanged;
        }

        private void Dgv_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0 || dgv.CurrentRow == null) return;
            idSeleccionado = Convert.ToInt32(dgv.CurrentRow.Cells["Id"].Value);
            txtNombre.Text = dgv.CurrentRow.Cells["Nombre"].Value?.ToString() ?? "";
            txtContacto.Text = dgv.CurrentRow.Cells["Contacto"].Value?.ToString() ?? "";
            txtTel.Text = dgv.CurrentRow.Cells["Tel"].Value?.ToString() ?? "";
            txtEmail.Text = dgv.CurrentRow.Cells["Email"].Value?.ToString() ?? "";
        }

        // Accion cuando se le da clic al boton
        // Obtenemos lo que escribio el usuario y procesamos
        private void BtnGuardar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text)) { MessageBox.Show("El nombre es obligatorio."); return; }
            // Verificamos si estamos creando un registro nuevo (ID en cero) o editando uno que ya existe
            if (idSeleccionado == 0)
            {
                var n = new Supplier { Id = DataStore.NextSupplierId(), Name = txtNombre.Text.Trim(), Contact = txtContacto.Text.Trim(), Phone = txtTel.Text.Trim(), Email = txtEmail.Text.Trim() };
                DataStore.Suppliers.Add(n);
                DataStore.RegistrarLog("CREACION", "Proveedor", $"Proveedor registrado: {n.Name}");
                MessageBox.Show("Proveedor registrado.", "Exito");
            }
            else
            {
                var s = DataStore.Suppliers.FirstOrDefault(x => x.Id == idSeleccionado);
                if (s != null)
                {
                    s.Name = txtNombre.Text.Trim();
                    s.Contact = txtContacto.Text.Trim();
                    s.Phone = txtTel.Text.Trim();
                    s.Email = txtEmail.Text.Trim();
                    DataStore.RegistrarLog("ACTUALIZACION", "Proveedor", $"Proveedor actualizado: {s.Name}");
                    MessageBox.Show("Actualizado.", "Exito");
                }
            }
            LimpiarFormulario(); RefrescarTabla();
        }

        // Accion cuando se le da clic al boton
        // Obtenemos lo que escribio el usuario y procesamos
        private void BtnEliminar_Click(object? sender, EventArgs e)
        {
            // Verificamos si estamos creando un registro nuevo (ID en cero) o editando uno que ya existe
            if (idSeleccionado == 0) { MessageBox.Show("Selecciona un proveedor."); return; }
            // Le preguntamos al usuario si esta seguro de querer borrar esta informacion para evitar accidentes
            if (MessageBox.Show("¿Eliminar?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            var s = DataStore.Suppliers.FirstOrDefault(x => x.Id == idSeleccionado);
            if (s != null)
            {
                DataStore.Suppliers.Remove(s);
                DataStore.RegistrarLog("ELIMINACION", "Proveedor", $"Proveedor eliminado: {s.Name}");
                MessageBox.Show("Eliminado.");
            }
            LimpiarFormulario(); RefrescarTabla();
        }

        private void LimpiarFormulario()
        {
            idSeleccionado = 0;
            dgv.SelectionChanged -= Dgv_SelectionChanged;
            dgv.ClearSelection();
            dgv.SelectionChanged += Dgv_SelectionChanged;
            txtNombre.Text = txtContacto.Text = txtTel.Text = txtEmail.Text = "";
        }
    }

    public class frmFacturas : frmBase
    {
        private DataGridView dgv = null!;
        private ComboBox cmbDueno = null!, cmbEstado = null!, cmbPago = null!;
        private MaterialTextBox txtTotal = null!, txtImpuesto = null!, txtNotas = null!;

        public frmFacturas()
        {
            InicializarComponentes(); RefrescarTabla();
        }

        // Metodo principal para dibujar toda la interfaz visual de esta pantalla
        private void InicializarComponentes()
        {
            Text = "Gestion de Facturas"; Size = new Size(1000, 650); StartPosition = FormStartPosition.CenterScreen;
            Controls.Add(CrearLabel("🧾 Gestion de Facturas", 20, 80, true));

            // Creamos y preparamos la tabla visual donde se mostraran los registros al usuario
            dgv = new DataGridView { Location = new Point(20, 130), Size = new Size(570, 450) };
            EstilarTabla(dgv); Controls.Add(dgv);

            Controls.Add(CrearLabel("📝 Nueva Factura", 615, 80, true));

            Controls.Add(CrearLabel("Cliente / Dueño", 615, 115));
            cmbDueno = new ComboBox { Location = new Point(615, 135), Size = new Size(310, 30) };
            EstilarComboBox(cmbDueno);
            cmbDueno.Items.Add("-- Seleccionar Cliente --");
            foreach (var o in DataStore.Owners) cmbDueno.Items.Add($"{o.Name} ({o.Identity})");
            cmbDueno.SelectedIndex = 0; Controls.Add(cmbDueno);

            txtTotal = CrearCampo("Total (B/.)", 615, 180, 145);
            txtImpuesto = CrearCampo("Impuesto", 775, 180, 150);
            txtTotal.KeyPress += SoloDecimales_KeyPress;
            txtImpuesto.KeyPress += SoloDecimales_KeyPress;
            Controls.Add(txtTotal); Controls.Add(txtImpuesto);

            Controls.Add(CrearLabel("Metodo de pago", 615, 243));
            cmbPago = new ComboBox { Location = new Point(615, 263), Size = new Size(145, 30) };
            EstilarComboBox(cmbPago);
            cmbPago.Items.AddRange(new object[] { "Efectivo", "Tarjeta", "Yappy", "Transferencia" });
            cmbPago.SelectedIndex = 0; Controls.Add(cmbPago);

            Controls.Add(CrearLabel("Estado", 775, 243));
            cmbEstado = new ComboBox { Location = new Point(775, 263), Size = new Size(150, 30) };
            EstilarComboBox(cmbEstado);
            cmbEstado.Items.AddRange(new object[] { "PAGADA", "PENDIENTE", "ANULADA" });
            cmbEstado.SelectedIndex = 0; Controls.Add(cmbEstado);

            txtNotas = CrearCampo("Notas", 615, 313, 310); Controls.Add(txtNotas);

            var btnG = CrearBoton("💾 Guardar Factura", Color.FromArgb(56, 142, 60), 615, 383, 310);
            btnG.Click += BtnGuardar_Click; Controls.Add(btnG);

            var btnPreview = CrearBoton("🔍 Vista Previa / Imprimir", Color.FromArgb(79, 70, 229), 615, 438, 310);
            btnPreview.Click += BtnPreview_Click; Controls.Add(btnPreview);

            dgv.CellDoubleClick += Dgv_CellDoubleClick;
        }

        private void Dgv_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            int invoiceId = Convert.ToInt32(dgv.Rows[e.RowIndex].Cells["Id"].Value);
            var inv = DataStore.Invoices.FirstOrDefault(x => x.Id == invoiceId);
            if (inv != null)
            {
                new frmFacturaPreview(inv).ShowDialog();
            }
        }

        // Accion cuando se le da clic al boton
        // Obtenemos lo que escribio el usuario y procesamos
        private void BtnPreview_Click(object? sender, EventArgs e)
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Seleccione una factura de la lista.", "Aviso");
                return;
            }
            int invoiceId = Convert.ToInt32(dgv.CurrentRow.Cells["Id"].Value);
            var inv = DataStore.Invoices.FirstOrDefault(x => x.Id == invoiceId);
            if (inv != null)
            {
                new frmFacturaPreview(inv).ShowDialog();
            }
        }

        // Actualiza la informacion de la tabla recargandola con los datos de memoria
        private void RefrescarTabla()
        {
            // Limpiamos la tabla en pantalla antes de volver a llenarla con los datos mas recientes
            dgv.Rows.Clear(); dgv.Columns.Clear();
            dgv.Columns.Add("Id", "ID"); dgv.Columns.Add("Fecha", "Fecha");
            dgv.Columns.Add("Cliente", "Cliente"); dgv.Columns.Add("Total", "Total");
            dgv.Columns.Add("Estado", "Estado"); dgv.Columns.Add("Pago", "Pago");
            dgv.Columns["Id"].Width = 40;

            foreach (var inv in DataStore.Invoices.OrderByDescending(x => x.Date))
            {
                var owner = DataStore.Owners.FirstOrDefault(o => o.Id == inv.OwnerId);
                int row = dgv.Rows.Add(inv.Id, inv.Date.ToString("dd/MM/yyyy"),
                    owner?.Name ?? "N/A", $"B/. {inv.Total:F2}", inv.Status, inv.PaymentMethod ?? "");

                var cellEstado = dgv.Rows[row].Cells["Estado"];
                switch (inv.Status?.ToUpper())
                {
                    case "PAGADA":
                    case "PAID":
                        cellEstado.Style.ForeColor = Color.FromArgb(52, 211, 153);
                        cellEstado.Style.SelectionForeColor = Color.FromArgb(52, 211, 153);
                        cellEstado.Value = "PAGADA";
                        break;
                    case "PENDIENTE":
                    case "PENDING":
                        cellEstado.Style.ForeColor = Color.FromArgb(251, 191, 36);
                        cellEstado.Style.SelectionForeColor = Color.FromArgb(251, 191, 36);
                        cellEstado.Value = "PENDIENTE";
                        break;
                    case "ANULADA":
                    case "VOID":
                        cellEstado.Style.ForeColor = Color.FromArgb(248, 113, 113);
                        cellEstado.Style.SelectionForeColor = Color.FromArgb(248, 113, 113);
                        cellEstado.Value = "ANULADA";
                        break;
                }
            }
            dgv.ClearSelection();
        }

        // Accion cuando se le da clic al boton
        // Obtenemos lo que escribio el usuario y procesamos
        private void BtnGuardar_Click(object? sender, EventArgs e)
        {
            if (cmbDueno.SelectedIndex == 0)
            { MessageBox.Show("Debes seleccionar un cliente / dueño.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            float total = float.TryParse(txtTotal.Text.Replace(",", "."), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float t) ? t : 0;
            if (total <= 0)
            { MessageBox.Show("El total debe ser mayor a cero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            float tax = float.TryParse(txtImpuesto.Text.Replace(",", "."), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float tx) ? tx : 0;
            int ownerId = DataStore.Owners[cmbDueno.SelectedIndex - 1].Id;

            DataStore.Invoices.Add(new Invoice { Id = DataStore.NextInvoiceId(), Date = DateTime.Now, Total = total, Tax = tax, PaymentMethod = cmbPago.SelectedItem?.ToString(), Status = cmbEstado.SelectedItem?.ToString() ?? "PAGADA", Notes = txtNotas.Text.Trim(), OwnerId = ownerId });
            DataStore.RegistrarLog("CREACION", "Factura", $"Factura B/. {total:F2}");
            MessageBox.Show("Factura registrada.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtTotal.Text = txtImpuesto.Text = txtNotas.Text = "";
            cmbDueno.SelectedIndex = 0;
            dgv.ClearSelection();
            RefrescarTabla();
        }
    }

    public class frmFacturaPreview : Form
    {
        private Invoice _invoice;
        private Panel pnlInvoicePage = null!;

        public frmFacturaPreview(Invoice invoice)
        {
            _invoice = invoice;
            InicializarComponentes();
        }

        // Metodo principal para dibujar toda la interfaz visual de esta pantalla
        private void InicializarComponentes()
        {
            Text = "Vista Previa de Factura";
            Size = new Size(800, 850);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.FromArgb(240, 242, 245);

            var pnlTopBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.White
            };

            var lblTitle = new Label
            {
                Text = "Vista Previa de Factura",
                Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(20, 18),
                AutoSize = true
            };

            var btnPrint = new Button
            {
                Text = "🖨️ Imprimir",
                Location = new Point(580, 12),
                Size = new Size(110, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(99, 102, 241),
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold)
            };
            btnPrint.FlatAppearance.BorderSize = 0;
            btnPrint.Click += BtnPrint_Click;

            pnlTopBar.Controls.Add(lblTitle);
            pnlTopBar.Controls.Add(btnPrint);
            Controls.Add(pnlTopBar);

            var scrollContainer = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(240, 242, 245)
            };
            Controls.Add(scrollContainer);

            pnlInvoicePage = new Panel
            {
                Size = new Size(700, 800),
                BackColor = Color.White,
                Location = new Point(40, 20)
            };

            pnlInvoicePage.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(229, 231, 235), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, pnlInvoicePage.Width - 1, pnlInvoicePage.Height - 1);
                }
            };

            scrollContainer.Resize += (s, e) =>
            {
                pnlInvoicePage.Location = new Point(
                    Math.Max(20, (scrollContainer.Width - pnlInvoicePage.Width) / 2),
                    20
                );
            };

            var lblClinica = new Label
            {
                Text = "CLINICA VETERINARIA DR. CEDEÑO",
                Font = new Font("Segoe UI Semibold", 15F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(0, 30),
                Size = new Size(700, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblRuc = new Label
            {
                Text = "RUC: 6-67-83 DV 0",
                Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                Location = new Point(0, 65),
                Size = new Size(700, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblDireccion = new Label
            {
                Text = "Av. Juan Pablo II, Casa 96, Bethania, Panama",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(0, 90),
                Size = new Size(700, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblTels = new Label
            {
                Text = "Tels: 236-9453 / 6614-2391",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(0, 110),
                Size = new Size(700, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lineDivider = new Panel
            {
                Location = new Point(40, 145),
                Size = new Size(620, 1),
                BackColor = Color.FromArgb(203, 213, 225)
            };

            var owner = DataStore.Owners.FirstOrDefault(o => o.Id == _invoice.OwnerId);

            var lblClienteTag = new Label
            {
                Text = "DATOS DEL CLIENTE",
                Font = new Font("Segoe UI Semibold", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(40, 170),
                Size = new Size(300, 18)
            };

            var pnlClienteCard = new Panel
            {
                Location = new Point(40, 190),
                Size = new Size(300, 110),
                BackColor = Color.FromArgb(249, 250, 251)
            };
            pnlClienteCard.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(243, 244, 246), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, pnlClienteCard.Width - 1, pnlClienteCard.Height - 1);
                }
            };

            var lblClienteName = new Label
            {
                Text = owner?.Name ?? "Consumidor Final",
                Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                Location = new Point(15, 15),
                Size = new Size(270, 22)
            };

            var lblClienteId = new Label
            {
                Text = $"ID / Cedula: {owner?.Identity ?? "N/A"}",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(75, 85, 99),
                Location = new Point(15, 45),
                Size = new Size(270, 20)
            };

            var lblClienteDir = new Label
            {
                Text = $"Direccion: {owner?.Address ?? "N/A"}",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(75, 85, 99),
                Location = new Point(15, 70),
                Size = new Size(270, 35)
            };

            pnlClienteCard.Controls.Add(lblClienteName);
            pnlClienteCard.Controls.Add(lblClienteId);
            pnlClienteCard.Controls.Add(lblClienteDir);

            var lblDetallesTag = new Label
            {
                Text = "DETALLES DE FACTURA",
                Font = new Font("Segoe UI Semibold", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(360, 170),
                Size = new Size(300, 18)
            };

            var lblNumFacturaTag = new Label
            {
                Text = "N° Factura:",
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                Location = new Point(360, 195),
                Size = new Size(110, 22)
            };

            var lblNumFacturaVal = new Label
            {
                Text = $"#INV-{_invoice.Id:D4}",
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(79, 70, 229),
                Location = new Point(480, 195),
                Size = new Size(180, 22),
                TextAlign = ContentAlignment.TopRight
            };

            var lblFechaTag = new Label
            {
                Text = "Fecha:",
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                Location = new Point(360, 225),
                Size = new Size(110, 22)
            };

            var lblFechaVal = new Label
            {
                Text = _invoice.Date.ToString("dd/MM/yyyy"),
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(55, 65, 81),
                Location = new Point(480, 225),
                Size = new Size(180, 22),
                TextAlign = ContentAlignment.TopRight
            };

            var pet = DataStore.Pets.FirstOrDefault(p => p.OwnerId == _invoice.OwnerId);
            var lblMascotaTag = new Label
            {
                Text = "Mascota:",
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                Location = new Point(360, 255),
                Size = new Size(110, 22)
            };

            var lblMascotaVal = new Label
            {
                Text = pet?.Name ?? "N/A",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(55, 65, 81),
                Location = new Point(480, 255),
                Size = new Size(180, 22),
                TextAlign = ContentAlignment.TopRight
            };

            var dgvItems = new DataGridView
            {
                Location = new Point(40, 320),
                Size = new Size(620, 280),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(243, 244, 246),
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false,
                RowTemplate = { Height = 36 }
            };

            dgvItems.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(17, 24, 39);
            dgvItems.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvItems.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(17, 24, 39);
            dgvItems.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
            dgvItems.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            dgvItems.ColumnHeadersHeight = 38;

            dgvItems.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgvItems.DefaultCellStyle.ForeColor = Color.FromArgb(55, 65, 81);
            dgvItems.DefaultCellStyle.SelectionBackColor = Color.FromArgb(243, 244, 246);
            dgvItems.DefaultCellStyle.SelectionForeColor = Color.FromArgb(17, 24, 39);

            dgvItems.Columns.Add("Desc", "Descripcion");
            dgvItems.Columns.Add("Cant", "Cant.");
            dgvItems.Columns.Add("Price", "P. Unit");
            dgvItems.Columns.Add("Total", "Total");

            dgvItems.Columns["Cant"].Width = 60;
            dgvItems.Columns["Price"].Width = 80;
            dgvItems.Columns["Total"].Width = 80;
            dgvItems.Columns["Cant"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvItems.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvItems.Columns["Total"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            float subtotalVal = _invoice.Total - _invoice.Tax;
            dgvItems.Rows.Add("Servicios Veterinarios Especializados", 1, $"B/. {subtotalVal:F2}", $"B/. {subtotalVal:F2}");

            if (_invoice.Tax > 0)
            {
                dgvItems.Rows.Add("Impuesto de Servicios (7% ITBMS)", 1, $"B/. {_invoice.Tax:F2}", $"B/. {_invoice.Tax:F2}");
            }

            var lblNotasTag = new Label
            {
                Text = "OBSERVACIONES / NOTAS",
                Font = new Font("Segoe UI Semibold", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(40, 620),
                Size = new Size(300, 18)
            };

            var pnlNotasCard = new Panel
            {
                Location = new Point(40, 640),
                Size = new Size(300, 100),
                BackColor = Color.FromArgb(249, 250, 251)
            };
            pnlNotasCard.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(243, 244, 246), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, pnlNotasCard.Width - 1, pnlNotasCard.Height - 1);
                }
            };

            var lblNotasVal = new Label
            {
                Text = string.IsNullOrWhiteSpace(_invoice.Notes) ? "Sin observaciones adicionales." : _invoice.Notes,
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(12, 12),
                Size = new Size(276, 76)
            };
            pnlNotasCard.Controls.Add(lblNotasVal);

            var lblSubtotalTag = new Label
            {
                Text = "Subtotal",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(360, 630),
                Size = new Size(150, 22)
            };

            var lblSubtotalVal = new Label
            {
                Text = $"B/. {subtotalVal:F2}",
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                Location = new Point(510, 630),
                Size = new Size(150, 22),
                TextAlign = ContentAlignment.TopRight
            };

            var lblTaxTag = new Label
            {
                Text = "ITBMS (7%)",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(360, 660),
                Size = new Size(150, 22)
            };

            var lblTaxVal = new Label
            {
                Text = $"B/. {_invoice.Tax:F2}",
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                Location = new Point(510, 660),
                Size = new Size(150, 22),
                TextAlign = ContentAlignment.TopRight
            };

            var lblTotalTag = new Label
            {
                Text = "Total",
                Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                Location = new Point(360, 695),
                Size = new Size(150, 25)
            };

            var lblTotalVal = new Label
            {
                Text = $"B/. {_invoice.Total:F2}",
                Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(79, 70, 229),
                Location = new Point(510, 695),
                Size = new Size(150, 25),
                TextAlign = ContentAlignment.TopRight
            };

            var lineTotals = new Panel
            {
                Location = new Point(360, 688),
                Size = new Size(300, 1),
                BackColor = Color.FromArgb(229, 231, 235)
            };

            pnlInvoicePage.Controls.Add(lblClinica);
            pnlInvoicePage.Controls.Add(lblRuc);
            pnlInvoicePage.Controls.Add(lblDireccion);
            pnlInvoicePage.Controls.Add(lblTels);
            pnlInvoicePage.Controls.Add(lineDivider);
            pnlInvoicePage.Controls.Add(lblClienteTag);
            pnlInvoicePage.Controls.Add(pnlClienteCard);
            pnlInvoicePage.Controls.Add(lblDetallesTag);
            pnlInvoicePage.Controls.Add(lblNumFacturaTag);
            pnlInvoicePage.Controls.Add(lblNumFacturaVal);
            pnlInvoicePage.Controls.Add(lblFechaTag);
            pnlInvoicePage.Controls.Add(lblFechaVal);
            pnlInvoicePage.Controls.Add(lblMascotaTag);
            pnlInvoicePage.Controls.Add(lblMascotaVal);
            pnlInvoicePage.Controls.Add(dgvItems);
            pnlInvoicePage.Controls.Add(lblNotasTag);
            pnlInvoicePage.Controls.Add(pnlNotasCard);
            pnlInvoicePage.Controls.Add(lblSubtotalTag);
            pnlInvoicePage.Controls.Add(lblSubtotalVal);
            pnlInvoicePage.Controls.Add(lblTaxTag);
            pnlInvoicePage.Controls.Add(lblTaxVal);
            pnlInvoicePage.Controls.Add(lblTotalTag);
            pnlInvoicePage.Controls.Add(lblTotalVal);
            pnlInvoicePage.Controls.Add(lineTotals);

            scrollContainer.Controls.Add(pnlInvoicePage);
        }

        // Accion cuando se le da clic al boton
        // Obtenemos lo que escribio el usuario y procesamos
        private void BtnPrint_Click(object? sender, EventArgs e)
        {
            var pd = new System.Drawing.Printing.PrintDocument();
            pd.PrintPage += (s, ev) =>
            {
                var bmp = new Bitmap(pnlInvoicePage.Width, pnlInvoicePage.Height);
                pnlInvoicePage.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
                ev.Graphics.DrawImage(bmp, 0, 0);
            };

            using (var printDialog = new PrintDialog())
            {
                printDialog.Document = pd;
                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    pd.Print();
                }
            }
        }
    }

    public class frmUsuarios : frmBase
    {
        private DataGridView dgv = null!;
        private MaterialTextBox txtUser = null!, txtNombre = null!, txtEmail = null!, txtPass = null!;
        private ComboBox cmbRol = null!;
        private int idSeleccionado = 0;

        public frmUsuarios()
        {
            if (SessionManager.UsuarioActual == null || Convert.ToInt32(SessionManager.UsuarioActual.RoleId) != 1)
            {
                MessageBox.Show("Acceso denegado. No tienes permisos de administrador para gestionar usuarios.", "Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.Load += (s, e) => this.Close();
                return;
            }
            InicializarComponentes(); RefrescarTabla();
        }

        // Metodo principal para dibujar toda la interfaz visual de esta pantalla
        private void InicializarComponentes()
        {
            Text = "Gestion de Usuarios"; Size = new Size(950, 600); StartPosition = FormStartPosition.CenterScreen;
            Controls.Add(CrearLabel("👤 Gestion de Usuarios", 20, 80, true));

            // Creamos y preparamos la tabla visual donde se mostraran los registros al usuario
            dgv = new DataGridView { Location = new Point(20, 130), Size = new Size(540, 400) };
            EstilarTabla(dgv); dgv.SelectionChanged += Dgv_SelectionChanged; Controls.Add(dgv);

            Controls.Add(CrearLabel("📝 Datos del Usuario", 590, 80, true));
            txtUser = CrearCampo("Username *", 590, 120, 290);
            txtNombre = CrearCampo("Nombre completo *", 590, 180, 290);
            txtEmail = CrearCampo("Email", 590, 240, 290);
            txtPass = CrearCampo("Contraseña *", 590, 300, 290);
            Controls.Add(CrearLabel("Rol *", 590, 355));
            cmbRol = new ComboBox { Location = new Point(590, 375), Size = new Size(290, 30) };
            EstilarComboBox(cmbRol);
            foreach (var r in DataStore.Roles) cmbRol.Items.Add(r.Name);
            if (cmbRol.Items.Count > 0) cmbRol.SelectedIndex = 0;
            Controls.Add(txtUser); Controls.Add(txtNombre); Controls.Add(txtEmail); Controls.Add(txtPass); Controls.Add(cmbRol);

            var btnG = CrearBoton("💾 Guardar", Color.FromArgb(56, 142, 60), 590, 430, 135);
            var btnE = CrearBoton("🗑 Eliminar", Color.FromArgb(211, 47, 47), 740, 430, 140);

            btnG.Click += BtnGuardar_Click;
            btnE.Click += BtnEliminar_Click;

            Controls.Add(btnG); Controls.Add(btnE);
        }

        private void LimpiarFormulario()
        {
            idSeleccionado = 0;
            dgv.SelectionChanged -= Dgv_SelectionChanged;
            dgv.ClearSelection();
            dgv.SelectionChanged += Dgv_SelectionChanged;
            txtUser.Text = "";
            txtNombre.Text = "";
            txtEmail.Text = "";
            txtPass.Text = "";
            if (cmbRol.Items.Count > 0) cmbRol.SelectedIndex = 0;
        }

        // Actualiza la informacion de la tabla recargandola con los datos de memoria
        private void RefrescarTabla()
        {
            dgv.SelectionChanged -= Dgv_SelectionChanged;
            // Limpiamos la tabla en pantalla antes de volver a llenarla con los datos mas recientes
            dgv.Rows.Clear(); dgv.Columns.Clear();
            dgv.Columns.Add("Id", "ID"); dgv.Columns.Add("Usuario", "Usuario");
            dgv.Columns.Add("Nombre", "Nombre"); dgv.Columns.Add("Email", "Email");
            dgv.Columns.Add("RoleId", "RoleId"); dgv.Columns["Id"].Width = 40; dgv.Columns["RoleId"].Visible = false;
            foreach (var u in DataStore.Users)
                dgv.Rows.Add(u.Id, u.Username, u.Name, u.Email ?? "", u.RoleId);
            dgv.ClearSelection();
            dgv.SelectionChanged += Dgv_SelectionChanged;
        }

        private void Dgv_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) return;
            if (dgv.CurrentRow == null) return;
            idSeleccionado = Convert.ToInt32(dgv.CurrentRow.Cells["Id"].Value);
            txtUser.Text = dgv.CurrentRow.Cells["Usuario"].Value?.ToString() ?? "";
            txtNombre.Text = dgv.CurrentRow.Cells["Nombre"].Value?.ToString() ?? "";
            txtEmail.Text = dgv.CurrentRow.Cells["Email"].Value?.ToString() ?? "";
            int roleId = Convert.ToInt32(dgv.CurrentRow.Cells["RoleId"].Value);
            int ri = DataStore.Roles.FindIndex(r => r.Id == roleId);
            if (ri >= 0) cmbRol.SelectedIndex = ri;
        }

        // Accion cuando se le da clic al boton
        // Obtenemos lo que escribio el usuario y procesamos
        private void BtnGuardar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUser.Text) || string.IsNullOrWhiteSpace(txtNombre.Text)) { MessageBox.Show("Username y nombre son obligatorios."); return; }
            if (idSeleccionado == 0 && string.IsNullOrWhiteSpace(txtPass.Text)) { MessageBox.Show("La contraseña es obligatoria para nuevos usuarios."); return; }
            int roleId = DataStore.Roles.Count > 0 ? DataStore.Roles[cmbRol.SelectedIndex].Id : 1;

            string userTrimmed = txtUser.Text.Trim();
            if (DataStore.Users.Any(u => u.Username.Equals(userTrimmed, StringComparison.OrdinalIgnoreCase) && u.Id != idSeleccionado))
            {
                MessageBox.Show("El nombre de usuario ya existe. Por favor, elige otro.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Verificamos si estamos creando un registro nuevo (ID en cero) o editando uno que ya existe
            if (idSeleccionado == 0)
            {
                int nextId = DataStore.NextUserId();
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(txtPass.Text);

                DataStore.Users.Add(new User { Id = nextId, Username = txtUser.Text.Trim(), Password = passwordHash, Name = txtNombre.Text.Trim(), Email = txtEmail.Text.Trim(), RoleId = roleId });

                MessageBox.Show("Usuario registrado.", "Exito");
                DataStore.RegistrarLog("CREACION", "Usuario", $"Usuario registrado: {txtUser.Text.Trim()} ({txtNombre.Text.Trim()})");
            }
            else
            {
                var u = DataStore.Users.FirstOrDefault(x => x.Id == idSeleccionado);
                if (u != null)
                {
                    u.Username = txtUser.Text.Trim();
                    u.Name = txtNombre.Text.Trim();
                    u.Email = txtEmail.Text.Trim();
                    u.RoleId = roleId;

                    if (!string.IsNullOrWhiteSpace(txtPass.Text))
                    {
                        u.Password = BCrypt.Net.BCrypt.HashPassword(txtPass.Text);
                    }
                }
                MessageBox.Show("Usuario actualizado.", "Exito");
                DataStore.RegistrarLog("ACTUALIZACION", "Usuario", $"Usuario actualizado: {u?.Username}");
            }
            idSeleccionado = 0; txtUser.Text = txtNombre.Text = txtEmail.Text = txtPass.Text = "";
            RefrescarTabla();
        }

        // Accion cuando se le da clic al boton
        // Obtenemos lo que escribio el usuario y procesamos
        private void BtnEliminar_Click(object? sender, EventArgs e)
        {
            // Verificamos si estamos creando un registro nuevo (ID en cero) o editando uno que ya existe
            if (idSeleccionado == 0) { MessageBox.Show("Selecciona un usuario."); return; }
            if (MessageBox.Show("¿Eliminar usuario?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            var u = DataStore.Users.FirstOrDefault(x => x.Id == idSeleccionado);
            if (u != null)
            {
                DataStore.Users.Remove(u);

                MessageBox.Show("Eliminado.");
                DataStore.RegistrarLog("ELIMINACION", "Usuario", $"Usuario eliminado: {u.Username}");
            }
            idSeleccionado = 0; RefrescarTabla();
        }
    }

    public class frmAuditoria : frmBase
    {
        private DataGridView dgv = null!;
        private ComboBox cmbRol = null!;
        private ComboBox cmbUsuario = null!;
        private ComboBox cmbEntidad = null!;
        private ComboBox cmbAccion = null!;
        private bool isResetting = false;

        public frmAuditoria()
        {
            if (SessionManager.UsuarioActual == null || Convert.ToInt32(SessionManager.UsuarioActual.RoleId) != 1)
            {
                MessageBox.Show("Acceso denegado. No tienes permisos de administrador para ver el registro de auditoria.", "Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.Load += (s, e) => this.Close();
                return;
            }
            InicializarComponentes(); CargarLogs();
        }

        // Metodo principal para dibujar toda la interfaz visual de esta pantalla
        private void InicializarComponentes()
        {
            Text = "Auditoria del Sistema"; Size = new Size(950, 580); StartPosition = FormStartPosition.CenterScreen;
            Controls.Add(CrearLabel("📊 Registro de Auditoria", 20, 20, true));

            var lblRol = CrearLabel("Rol:", 20, 75); Controls.Add(lblRol);
            cmbRol = new ComboBox { Location = new Point(55, 71), Size = new Size(100, 30) };
            Controls.Add(cmbRol); EstilarComboBox(cmbRol);
            cmbRol.Items.AddRange(new object[] { "Todos", "ADMINISTRADOR", "ASISTENTE" });

            var lblUser = CrearLabel("Usuario:", 170, 75); Controls.Add(lblUser);
            cmbUsuario = new ComboBox { Location = new Point(235, 71), Size = new Size(120, 30) };
            Controls.Add(cmbUsuario); EstilarComboBox(cmbUsuario);
            cmbUsuario.Items.Add("Todos");
            foreach (var u in DataStore.Users)
            {
                if (u.Name != null) cmbUsuario.Items.Add(u.Name);
            }

            var lblEntidad = CrearLabel("Entidad:", 370, 75); Controls.Add(lblEntidad);
            cmbEntidad = new ComboBox { Location = new Point(430, 71), Size = new Size(120, 30) };
            Controls.Add(cmbEntidad); EstilarComboBox(cmbEntidad);
            cmbEntidad.Items.AddRange(new object[] { "Todas", "Sesion", "Dueño", "Mascota", "Cita", "Historial", "Medicamento", "Proveedor", "Factura", "Usuario" });

            var lblAccion = CrearLabel("Accion:", 565, 75); Controls.Add(lblAccion);
            cmbAccion = new ComboBox { Location = new Point(620, 71), Size = new Size(110, 30) };
            Controls.Add(cmbAccion); EstilarComboBox(cmbAccion);
            cmbAccion.Items.AddRange(new object[] { "Todas", "CREACION", "ACTUALIZACION", "ELIMINACION", "INICIO_SESION", "CIERRE_SESION" });

            var btnLimpiar = CrearBoton("🧹 Limpiar filtro", Color.FromArgb(97, 97, 97), 780, 66, 140);
            // Accion para resetear todos los filtros de busqueda y volver a mostrar todo
            btnLimpiar.Click += (s, e) =>
            {
                try
                {
                    isResetting = true;
                    cmbRol.SelectedIndex = 0;
                    cmbUsuario.SelectedIndex = 0;
                    cmbEntidad.SelectedIndex = 0;
                    cmbAccion.SelectedIndex = 0;
                    isResetting = false;
                    CargarLogs();
                    MessageBox.Show("Filtros limpiados.", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al limpiar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            Controls.Add(btnLimpiar);

            // Creamos y preparamos la tabla visual donde se mostraran los registros al usuario
            dgv = new DataGridView { Location = new Point(20, 130), Size = new Size(900, 380) };
            EstilarTabla(dgv); Controls.Add(dgv);

            cmbRol.SelectedIndex = 0;
            cmbUsuario.SelectedIndex = 0;
            cmbEntidad.SelectedIndex = 0;
            cmbAccion.SelectedIndex = 0;

            // Cada vez que el usuario cambia una opcion del filtro, actualizamos la tabla en tiempo real
            cmbRol.SelectedIndexChanged += (s, e) => { if (!isResetting) CargarLogs(); };
            // Cada vez que el usuario cambia una opcion del filtro, actualizamos la tabla en tiempo real
            cmbUsuario.SelectedIndexChanged += (s, e) => { if (!isResetting) CargarLogs(); };
            // Cada vez que el usuario cambia una opcion del filtro, actualizamos la tabla en tiempo real
            cmbEntidad.SelectedIndexChanged += (s, e) => { if (!isResetting) CargarLogs(); };
            // Cada vez que el usuario cambia una opcion del filtro, actualizamos la tabla en tiempo real
            cmbAccion.SelectedIndexChanged += (s, e) => { if (!isResetting) CargarLogs(); };
        }

        // Metodo que lee la base de datos y aplica los filtros seleccionados para mostrar solo lo necesario
        private void CargarLogs()
        {
            if (dgv == null) return;
            // Limpiamos la tabla en pantalla antes de volver a llenarla con los datos mas recientes
            dgv.Rows.Clear(); dgv.Columns.Clear();
            dgv.Columns.Add("Id", "ID"); dgv.Columns.Add("Fecha", "Fecha");
            dgv.Columns.Add("Usuario", "Usuario"); dgv.Columns.Add("Accion", "Accion");
            dgv.Columns.Add("Entidad", "Entidad"); dgv.Columns.Add("Desc", "Descripcion");
            dgv.Columns["Id"].Width = 45;
            dgv.Columns["Fecha"].Width = 120;
            dgv.Columns["Usuario"].Width = 130;
            dgv.Columns["Accion"].Width = 100;
            dgv.Columns["Entidad"].Width = 120;

            string selRol = cmbRol.SelectedItem?.ToString() ?? "Todos";
            string selUsuario = cmbUsuario.SelectedItem?.ToString() ?? "Todos";
            string selEntidad = cmbEntidad.SelectedItem?.ToString() ?? "Todas";
            string selAccion = cmbAccion.SelectedItem?.ToString() ?? "Todas";

            foreach (var log in DataStore.AuditLogs.OrderByDescending(x => x.CreatedAt))
            {
                var user = DataStore.Users.FirstOrDefault(u => u.Id == log.UserId);

                if (selRol != "Todos")
                {
                    if (user == null) continue;
                    string userRoleName = user.RoleId == 1 ? "ADMINISTRADOR" : "ASISTENTE";
                    if (userRoleName != selRol) continue;
                }

                if (selUsuario != "Todos")
                {
                    if (user == null || user.Name != selUsuario) continue;
                }

                if (selEntidad != "Todas")
                {
                    bool match = string.Equals(log.Entity, selEntidad, StringComparison.OrdinalIgnoreCase);
                    if (string.Equals(selEntidad, "Dueño", StringComparison.OrdinalIgnoreCase) && string.Equals(log.Entity, "Dueno", StringComparison.OrdinalIgnoreCase))
                    {
                        match = true;
                    }
                    if (!match) continue;
                }

                if (selAccion != "Todas")
                {
                    string actionMapped = log.Action.ToUpper();
                    if (actionMapped == "INICIO SESIoN" || actionMapped == "INICIO DE SESIoN EXITOSO") actionMapped = "INICIO_SESION";
                    if (actionMapped == "CIERRE SESIoN" || actionMapped == "CIERRE DE SESIoN") actionMapped = "CIERRE_SESION";

                    if (selAccion == "INICIO_SESION" && actionMapped != "INICIO_SESION") continue;
                    else if (selAccion == "CIERRE_SESION" && actionMapped != "CIERRE_SESION") continue;
                    else if (selAccion != "INICIO_SESION" && selAccion != "CIERRE_SESION" && !string.Equals(actionMapped, selAccion, StringComparison.OrdinalIgnoreCase)) continue;
                }

                dgv.Rows.Add(log.Id, log.CreatedAt.ToString("dd/MM/yyyy HH:mm"), user?.Name ?? "Desconocido", log.Action, log.Entity, log.Description ?? "");
            }
        }
    }
}