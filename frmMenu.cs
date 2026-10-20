using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MaterialSkin;
using Grupo5_Proyecto.Database;
using Grupo5_Proyecto.CapaDatos;

namespace Grupo5_Proyecto.Forms
{
    public class frmMenu : MaterialSkin.Controls.MaterialForm
    {
        private Panel panelContenido = null!;
        private Button? btnActivo = null;
        private Form? vistaActual = null;

        public frmMenu()
        {
            MaterialSkinManager.Instance.AddFormToManage(this);
            InicializarComponentes();
        }

        // Metodo principal para dibujar toda la interfaz visual de esta pantalla
        private void InicializarComponentes()
        {
            Text = "Clinica Veterinaria Dr. Cedeño";
            Size = new Size(1200, 750);
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1200, 750);
            MaximumSize = new Size(1200, 750);
            MaximizeBox = false;
            MinimizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;

            bool esAdmin = SessionManager.UsuarioActual != null &&
                           Convert.ToInt32(SessionManager.UsuarioActual.RoleId) == 1;

            var panelLateral = new Panel
            {
                Dock = DockStyle.Left,
                Width = 230,
                BackColor = Color.FromArgb(17, 20, 36)
            };

            var pnlUserProfile = new Panel
            {
                Location = new Point(12, 12),
                Size = new Size(206, 68),
                BackColor = Color.Transparent
            };

            pnlUserProfile.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    int radius = 12;
                    path.AddArc(0, 0, radius * 2, radius * 2, 180, 90);
                    path.AddArc(pnlUserProfile.Width - (radius * 2), 0, radius * 2, radius * 2, 270, 90);
                    path.AddArc(pnlUserProfile.Width - (radius * 2), pnlUserProfile.Height - (radius * 2), radius * 2, radius * 2, 0, 90);
                    path.AddArc(0, pnlUserProfile.Height - (radius * 2), radius * 2, radius * 2, 90, 90);
                    path.CloseAllFigures();

                    using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                        new Point(0, 0), new Point(pnlUserProfile.Width, pnlUserProfile.Height),
                        Color.FromArgb(28, 33, 58), Color.FromArgb(20, 24, 44)))
                    {
                        e.Graphics.FillPath(brush, path);
                    }

                    using (var pen = new Pen(Color.FromArgb(60, 99, 102, 241), 1.5f))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            };

            var pnlAvatar = new Panel
            {
                Location = new Point(10, 12),
                Size = new Size(44, 44),
                BackColor = Color.Transparent
            };
            pnlAvatar.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    path.AddEllipse(0, 0, pnlAvatar.Width - 1, pnlAvatar.Height - 1);

                    using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                        new Point(0, 0), new Point(pnlAvatar.Width, pnlAvatar.Height),
                        Color.FromArgb(99, 102, 241), Color.FromArgb(67, 56, 202)))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                }
            };

            var lblAvatarIcon = new Label
            {
                Text = "👤",
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.White,
                Size = new Size(44, 44),
                Location = new Point(0, 0),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            pnlAvatar.Controls.Add(lblAvatarIcon);
            pnlUserProfile.Controls.Add(pnlAvatar);

            var lblUserName = new Label
            {
                Text = SessionManager.UsuarioActual?.Name ?? "Usuario",
                ForeColor = Color.FromArgb(241, 245, 249),
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                Location = new Point(62, 12),
                Size = new Size(138, 20),
                BackColor = Color.Transparent
            };

            string roleName = esAdmin ? "ADMINISTRADOR" : "ASISTENTE";
            Color roleColor = esAdmin ? Color.FromArgb(34, 197, 94) : Color.FromArgb(56, 189, 248);
            Color roleBgColor = esAdmin ? Color.FromArgb(20, 83, 45) : Color.FromArgb(8, 51, 68);

            int roleBadgeWidth = esAdmin ? 120 : 80;
            var pnlRoleBadge = new Panel
            {
                Location = new Point(62, 36),
                Size = new Size(roleBadgeWidth, 18),
                BackColor = Color.Transparent
            };
            pnlRoleBadge.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    int r = 5;
                    path.AddArc(0, 0, r * 2, r * 2, 180, 90);
                    path.AddArc(pnlRoleBadge.Width - r * 2, 0, r * 2, r * 2, 270, 90);
                    path.AddArc(pnlRoleBadge.Width - r * 2, pnlRoleBadge.Height - r * 2, r * 2, r * 2, 0, 90);
                    path.AddArc(0, pnlRoleBadge.Height - r * 2, r * 2, r * 2, 90, 90);
                    path.CloseAllFigures();
                    using (var brush = new SolidBrush(roleBgColor))
                        e.Graphics.FillPath(brush, path);
                    using (var pen = new Pen(roleColor, 1))
                        e.Graphics.DrawPath(pen, path);
                }
            };
            var lblRoleText = new Label
            {
                Text = roleName,
                ForeColor = roleColor,
                Font = new Font("Segoe UI", 7F, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            pnlRoleBadge.Controls.Add(lblRoleText);

            pnlUserProfile.Controls.Add(lblUserName);
            pnlUserProfile.Controls.Add(pnlRoleBadge);
            panelLateral.Controls.Add(pnlUserProfile);

            panelLateral.Controls.Add(new Panel
            {
                Location = new Point(15, 92),
                Size = new Size(200, 1),
                BackColor = Color.FromArgb(40, 99, 102, 241)
            });

            var btnInicio = new Button
            {
                Text = "  🏠  Inicio",
                Location = new Point(0, 95),
                Size = new Size(230, 46),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(79, 70, 229),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 0, 0),
                Cursor = Cursors.Hand,
                Tag = "Inicio"
            };
            btnInicio.FlatAppearance.BorderSize = 0;
            btnInicio.Click += (s, e) =>
            {
                if (btnActivo != null)
                {
                    btnActivo.BackColor = Color.Transparent;
                    btnActivo.ForeColor = Color.FromArgb(148, 163, 184);
                }
                btnActivo = btnInicio;
                btnInicio.BackColor = Color.FromArgb(79, 70, 229);
                btnInicio.ForeColor = Color.White;
                vistaActual?.Dispose();
                MostrarBienvenida();
            };
            btnActivo = btnInicio;
            panelLateral.Controls.Add(btnInicio);

            var itemsDeNavegacion = new System.Collections.Generic.List<(string Icono, string Texto)>
            {
                ("👥", "Dueños"),
                ("🐶", "Mascotas"),
                ("📅", "Citas"),
                ("📋", "Historiales"),
                ("📊", "Reportes"),
                ("💊", "Medicamentos"),
                ("🏭", "Proveedores"),
                ("🧾", "Facturas")
            };

            if (esAdmin)
            {
                itemsDeNavegacion.Add(("👤", "Usuarios"));
                itemsDeNavegacion.Add(("📊", "Auditoria"));
            }

            int yPos = 142;
            foreach (var navItem in itemsDeNavegacion)
            {
                var btn = new Button
                {
                    Text = $"  {navItem.Icono}  {navItem.Texto}",
                    Location = new Point(0, yPos),
                    Size = new Size(230, 46),
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.FromArgb(148, 163, 184),
                    Font = new Font("Segoe UI", 10),
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(12, 0, 0, 0),
                    Cursor = Cursors.Hand,
                    Tag = navItem.Texto
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(35, 33, 90);
                btn.Click += BtnNavegar_Click;
                panelLateral.Controls.Add(btn);
                yPos += 47;
            }

            var btnCerrar = new Button
            {
                Text = "  🚪  Cerrar Sesion",
                Dock = DockStyle.Bottom,
                Height = 46,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.FromArgb(255, 130, 130),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.FlatAppearance.MouseOverBackColor = Color.FromArgb(100, 30, 30);
            btnCerrar.Click += BtnCerrar_Click;
            panelLateral.Controls.Add(btnCerrar);

            panelContenido = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(12, 14, 23),
                Padding = new Padding(0)
            };

            Controls.Add(panelContenido);
            Controls.Add(panelLateral);

            this.Load += (s, e) => MostrarBienvenida();
        }

        // Muestra el texto de saludo y las estadisticas principales del sistema en el inicio
        private void MostrarBienvenida()
        {
            panelContenido.Controls.Clear();

            var scroll = new ScrollPanelV
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(12, 14, 23)
            };

            var lblBienvenida = new Label
            {
                Text = $"🐾  Bienvenido, {SessionManager.UsuarioActual?.Name ?? ""}",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(148, 163, 210),
                Location = new Point(30, 25),
                Size = new Size(800, 38)
            };

            var lblFecha = new Label
            {
                Text = $"📅  {DateTime.Now.ToString("dddd, dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("es-PA"))}",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Gray,
                Location = new Point(30, 63),
                Size = new Size(800, 25)
            };

            int totalDuenos = DataStore.Owners.Count;
            int totalMascotas = DataStore.Pets.Count;
            int citasHoy = DataStore.Appointments.Count(a => a.Date.Date == DateTime.Today);
            int citasPendientes = DataStore.Appointments.Count(a => a.Status == "SCHEDULED" || a.Status == "PROGRAMADA");
            int stockBajo = DataStore.Medicines.Count(m => m.Stock <= m.MinStock);

            float ingresosMes = 0;
            try
            {
                string mesActual = DateTime.Now.ToString("yyyy-MM");
                ingresosMes = DataStore.Invoices
                    .Where(i => (i.Status == "PAID" || i.Status == "PAGADA") && i.Date.ToString("yyyy-MM") == mesActual)
                    .Sum(i => i.Total);
            }
            catch { ingresosMes = 0; }

            var tarjetas = new[]
            {
                ("👥", "Dueños",           totalDuenos.ToString(),     Color.FromArgb(63,  81,  181)),
                ("🐶", "Mascotas",         totalMascotas.ToString(),   Color.FromArgb(30,  136, 229)),
                ("📅", "Citas Hoy",        citasHoy.ToString(),        Color.FromArgb(0,   150, 136)),
                ("⏳", "Pendientes",       citasPendientes.ToString(), Color.FromArgb(251, 140, 0  )),
                ("⚠️", "Stock Bajo",       stockBajo.ToString(),       Color.FromArgb(229, 57,  53 )),
                ("💰", "Ingresos del Mes", $"B/. {ingresosMes:F2}",    Color.FromArgb(46,  125, 50 )),
            };

            int x = 30, y = 100;
            foreach (var (icono, titulo, valor, color) in tarjetas)
            {
                var card = new Panel
                {
                    Location = new Point(x, y),
                    Size = new Size(270, 105),
                    BackColor = color,
                    Cursor = Cursors.Default
                };

                card.Controls.Add(new Label
                {
                    Text = icono,
                    Font = new Font("Segoe UI", 20),
                    ForeColor = Color.White,
                    Location = new Point(15, 8),
                    Size = new Size(50, 42),
                    TextAlign = ContentAlignment.MiddleCenter
                });

                card.Controls.Add(new Label
                {
                    Text = valor,
                    Font = new Font("Segoe UI", titulo == "Ingresos del Mes" ? 13 : 20, FontStyle.Bold),
                    ForeColor = Color.White,
                    Location = new Point(70, 8),
                    Size = new Size(190, 42),
                    TextAlign = ContentAlignment.MiddleLeft
                });

                card.Controls.Add(new Label
                {
                    Text = titulo,
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.FromArgb(220, 235, 255),
                    Location = new Point(0, 62),
                    Size = new Size(270, 28),
                    TextAlign = ContentAlignment.MiddleCenter
                });

                card.Paint += (s, e) =>
                {
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    e.Graphics.DrawLine(new System.Drawing.Pen(Color.FromArgb(100, 255, 255, 255), 1), 1, 0, card.Width - 2, 0);
                    e.Graphics.DrawLine(new System.Drawing.Pen(Color.FromArgb(60, 255, 255, 255), 1), 0, 0, 0, card.Height - 1);
                    e.Graphics.DrawLine(new System.Drawing.Pen(Color.FromArgb(60, 0, 0, 0), 2), 0, card.Height - 1, card.Width, card.Height - 1);
                };

                scroll.Controls.Add(card);
                x += 295;
                if (x > 800) { x = 30; y += 120; }
            }

            y += 15;

            scroll.Controls.Add(new Label
            {
                Text = "📋  Proximas Citas Pendientes",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(148, 163, 210),
                Location = new Point(30, y),
                Size = new Size(500, 30)
            });

            var dgvCitas = new DataGridView
            {
                Location = new Point(30, y + 35),
                Size = new Size(540, 160),
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.FromArgb(17, 20, 36),
                GridColor = Color.FromArgb(35, 42, 70),
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 9.5F),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvCitas.DefaultCellStyle.BackColor = Color.FromArgb(17, 20, 36);
            dgvCitas.DefaultCellStyle.ForeColor = Color.FromArgb(226, 232, 240);
            dgvCitas.DefaultCellStyle.SelectionBackColor = Color.FromArgb(79, 70, 229);
            dgvCitas.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvCitas.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(24, 28, 48);

            dgvCitas.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 27, 75);
            dgvCitas.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(203, 213, 225);
            dgvCitas.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(30, 27, 75);
            dgvCitas.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(203, 213, 225);
            dgvCitas.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            dgvCitas.EnableHeadersVisualStyles = false;
            dgvCitas.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvCitas.ColumnHeadersHeight = 35;
            dgvCitas.RowTemplate.Height = 32;
            dgvCitas.Columns.Add("Fecha", "Fecha");
            dgvCitas.Columns.Add("Mascota", "Mascota");
            dgvCitas.Columns.Add("Veterinario", "Veterinario");
            dgvCitas.Columns.Add("Motivo", "Motivo");

            foreach (var cita in DataStore.Appointments.Where(a => a.Status == "SCHEDULED" || a.Status == "PROGRAMADA").OrderBy(a => a.Date).Take(4))
            {
                var pet = DataStore.Pets.FirstOrDefault(p => p.Id == cita.PetId);
                var user = DataStore.Users.FirstOrDefault(u => u.Id == cita.UserId);
                int row = dgvCitas.Rows.Add(cita.Date.ToString("dd/MM/yyyy HH:mm"), pet?.Name ?? "", user?.Name ?? "", cita.Reason ?? "");
                dgvCitas.Rows[row].Cells[1].Style.ForeColor = Color.FromArgb(251, 191, 36);
            }

            scroll.Controls.Add(new Label
            {
                Text = "⚠️  Medicamentos con Stock Bajo",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(148, 163, 210),
                Location = new Point(590, y),
                Size = new Size(320, 30)
            });

            var dgvStock = new DataGridView
            {
                Location = new Point(590, y + 35),
                Size = new Size(300, 160),
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                BackgroundColor = Color.FromArgb(17, 20, 36),
                GridColor = Color.FromArgb(35, 42, 70),
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 9.5F),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvStock.DefaultCellStyle.BackColor = Color.FromArgb(17, 20, 36);
            dgvStock.DefaultCellStyle.ForeColor = Color.FromArgb(226, 232, 240);
            dgvStock.DefaultCellStyle.SelectionBackColor = Color.FromArgb(79, 70, 229);
            dgvStock.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvStock.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(24, 28, 48);

            dgvStock.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 27, 75);
            dgvStock.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(203, 213, 225);
            dgvStock.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(30, 27, 75);
            dgvStock.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(203, 213, 225);
            dgvStock.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            dgvStock.EnableHeadersVisualStyles = false;
            dgvStock.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvStock.ColumnHeadersHeight = 35;
            dgvStock.RowTemplate.Height = 32;
            dgvStock.Columns.Add("Nombre", "Medicamento");
            dgvStock.Columns.Add("Stock", "Stock");
            dgvStock.Columns.Add("Minimo", "Minimo");

            foreach (var med in DataStore.Medicines.Where(m => m.Stock <= m.MinStock))
            {
                int row = dgvStock.Rows.Add(med.Name, med.Stock, med.MinStock);
                dgvStock.Rows[row].Cells[1].Style.ForeColor = med.Stock == 0
                    ? Color.FromArgb(248, 113, 113)
                    : Color.FromArgb(251, 191, 36);
                dgvStock.Rows[row].Cells[1].Style.SelectionForeColor = med.Stock == 0
                    ? Color.FromArgb(248, 113, 113)
                    : Color.FromArgb(251, 191, 36);
            }

            scroll.Controls.Add(lblBienvenida);
            scroll.Controls.Add(lblFecha);
            scroll.Controls.Add(dgvCitas);
            scroll.Controls.Add(dgvStock);
            panelContenido.Controls.Add(scroll);
        }

        private void CargarVista(Form form)
        {
            panelContenido.Controls.Clear();
            vistaActual?.Dispose();

            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            form.Visible = true;
            form.Location = new Point(0, 0);

            panelContenido.Controls.Add(form);
            vistaActual = form;

            if (SessionManager.UsuarioActual == null || Convert.ToInt32(SessionManager.UsuarioActual.RoleId) != 1)
            {
                DeshabilitarBotonesEliminarRecursivo(form.Controls);
            }
        }

        private void DeshabilitarBotonesEliminarRecursivo(Control.ControlCollection controles)
        {
            foreach (Control ctrl in controles)
            {
                if (ctrl is Button btn)
                {
                    string nombreLower = btn.Name.ToLower();
                    string textoLower = btn.Text.ToLower();
                    if (nombreLower.Contains("eliminar") || nombreLower.Contains("borrar") ||
                        textoLower.Contains("eliminar") || textoLower.Contains("borrar"))
                    {
                        btn.Visible = false;
                    }
                }
                if (ctrl.HasChildren)
                    DeshabilitarBotonesEliminarRecursivo(ctrl.Controls);
            }
        }

        // Accion cuando se le da clic al boton
        // Obtenemos lo que escribio el usuario y procesamos
        private void BtnNavegar_Click(object? sender, EventArgs e)
        {
            if (sender is not Button btn) return;

            if (btnActivo != null)
            {
                btnActivo.BackColor = Color.Transparent;
                btnActivo.ForeColor = Color.FromArgb(148, 163, 184);
            }
            btn.BackColor = Color.FromArgb(79, 70, 229);
            btn.ForeColor = Color.White;
            btnActivo = btn;

            Form? vista = btn.Tag?.ToString() switch
            {
                "Dueños" => new frmDuenos(),
                "Mascotas" => new frmMascotas(),
                "Citas" => new frmCitas(),
                "Historiales" => new frmHistoriales(),
                "Medicamentos" => new frmMedicamentos(),
                "Proveedores" => new frmProveedores(),
                "Facturas" => new frmFacturas(),
                "Reportes" => new frmReportes(),
                "Usuarios" => new frmUsuarios(),
                "Auditoria" => new frmAuditoria(),
                _ => null
            };

            if (vista != null) CargarVista(vista);
        }

        // Accion cuando se le da clic al boton
        // Obtenemos lo que escribio el usuario y procesamos
        private void BtnCerrar_Click(object? sender, EventArgs e)
        {
            if (MessageBox.Show("¿Cerrar sesion?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (SessionManager.UsuarioActual != null)
                {
                    DataStore.RegistrarLog("CIERRE_SESION", "Sesion", $"Cierre de sesion de {SessionManager.UsuarioActual.Name} ({SessionManager.UsuarioActual.Username})");
                }
                SessionManager.CerrarSesion();
                vistaActual?.Dispose();
                new frmLogin().Show();
                Close();
            }
        }
    }
}