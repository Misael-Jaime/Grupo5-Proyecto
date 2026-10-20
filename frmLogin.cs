using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Grupo5_Proyecto.Database;
using MaterialSkin;
using MaterialSkin.Controls;

namespace Grupo5_Proyecto.Forms
{
    public class frmLogin : MaterialSkin.Controls.MaterialForm
    {
        private TextBox txtUsername = null!;
        private TextBox txtPassword = null!;
        private Button btnLogin = null!;

        public frmLogin()
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
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.FromArgb(12, 14, 23);

            var pnlBackground = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(12, 14, 23)
            };

            var cardContainer = new Panel
            {
                Size = new Size(400, 480),
                BackColor = Color.FromArgb(17, 20, 36)
            };

            pnlBackground.Resize += (s, e) =>
            {
                cardContainer.Location = new Point(
                    (pnlBackground.Width - cardContainer.Width) / 2,
                    (pnlBackground.Height - cardContainer.Height) / 2
                );
            };

            cardContainer.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(35, 42, 70), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, cardContainer.Width - 1, cardContainer.Height - 1);
                }
            };

            var pnlCircleIcon = new Panel
            {
                Size = new Size(80, 80),
                Location = new Point((400 - 80) / 2, 40),
                BackColor = Color.Transparent
            };
            pnlCircleIcon.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                using (var brush = new SolidBrush(Color.FromArgb(28, 32, 60)))
                {
                    e.Graphics.FillEllipse(brush, 2, 2, 76, 76);
                }

                using (var pen = new Pen(Color.FromArgb(99, 102, 241), 2))
                {
                    e.Graphics.DrawEllipse(pen, 2, 2, 76, 76);
                }

                using (var brush = new SolidBrush(Color.FromArgb(129, 140, 248)))
                {
                    e.Graphics.FillEllipse(brush, 28, 38, 24, 18);

                    e.Graphics.FillEllipse(brush, 17, 30, 8, 11);
                    e.Graphics.FillEllipse(brush, 27, 20, 8, 11);
                    e.Graphics.FillEllipse(brush, 45, 20, 8, 11);
                    e.Graphics.FillEllipse(brush, 55, 30, 8, 11);
                }
            };

            var lblTitle = new Label
            {
                Text = "Clinica Veterinaria Dr. Cedeño",
                Font = new Font("Segoe UI Semibold", 13F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(10, 135),
                Size = new Size(380, 28)
            };

            var lblSubtitle = new Label
            {
                Text = "Ingresa tu usuario y contraseña",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(148, 163, 184),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(10, 168),
                Size = new Size(380, 22)
            };

            var lblUserTag = new Label
            {
                Text = "Usuario",
                Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(203, 213, 225),
                BackColor = Color.Transparent,
                Location = new Point(40, 205),
                Size = new Size(100, 18)
            };

            var pnlUserField = new Panel
            {
                Location = new Point(40, 228),
                Size = new Size(320, 42),
                BackColor = Color.FromArgb(24, 28, 48)
            };
            pnlUserField.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(51, 65, 85), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, pnlUserField.Width - 1, pnlUserField.Height - 1);
                }
            };
            txtUsername = new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(24, 28, 48),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11),
                Location = new Point(12, 11),
                Width = 296
            };
            pnlUserField.Controls.Add(txtUsername);

            var lblPassTag = new Label
            {
                Text = "Contraseña",
                Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(203, 213, 225),
                BackColor = Color.Transparent,
                Location = new Point(40, 290),
                Size = new Size(100, 18)
            };

            var pnlPassField = new Panel
            {
                Location = new Point(40, 313),
                Size = new Size(320, 42),
                BackColor = Color.FromArgb(24, 28, 48)
            };
            pnlPassField.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(51, 65, 85), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, pnlPassField.Width - 1, pnlPassField.Height - 1);
                }
            };
            txtPassword = new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(24, 28, 48),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11),
                Location = new Point(12, 11),
                Width = 215,
                UseSystemPasswordChar = true
            };

            var btnShowPassword = new Button
            {
                Text = "VER",
                Size = new Size(75, 26),
                Location = new Point(235, 8),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.FromArgb(129, 140, 248),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                TabStop = false
            };
            btnShowPassword.FlatAppearance.BorderSize = 0;
            btnShowPassword.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnShowPassword.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnShowPassword.Click += (s, e) =>
            {
                txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
                btnShowPassword.Text = txtPassword.UseSystemPasswordChar ? "VER" : "OCULTAR";
                txtPassword.Focus();
            };

            pnlPassField.Controls.Add(txtPassword);
            pnlPassField.Controls.Add(btnShowPassword);

            btnLogin = new Button
            {
                Text = "Iniciar Sesion",
                Location = new Point(40, 385),
                Size = new Size(130, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(79, 70, 229),
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold)
            };
            btnLogin.FlatAppearance.BorderSize = 0;

            btnLogin.Paint += (s, e) =>
            {
                using (var brush = new SolidBrush(Color.FromArgb(79, 70, 229)))
                {
                    e.Graphics.FillRectangle(brush, 0, 0, btnLogin.Width, btnLogin.Height);
                }
                using (var brush = new SolidBrush(Color.White))
                {
                    var sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    e.Graphics.DrawString(btnLogin.Text, btnLogin.Font, brush, new RectangleF(0, 0, btnLogin.Width, btnLogin.Height), sf);
                }
            };
            btnLogin.Click += BtnLogin_Click;

            cardContainer.Controls.Add(pnlCircleIcon);
            cardContainer.Controls.Add(lblTitle);
            cardContainer.Controls.Add(lblSubtitle);
            cardContainer.Controls.Add(lblUserTag);
            cardContainer.Controls.Add(pnlUserField);
            cardContainer.Controls.Add(lblPassTag);
            cardContainer.Controls.Add(pnlPassField);
            cardContainer.Controls.Add(btnLogin);

            pnlBackground.Controls.Add(cardContainer);
            Controls.Add(pnlBackground);

            AcceptButton = btnLogin;
        }

        // Accion cuando se le da clic al boton
        // Obtenemos lo que escribio el usuario y procesamos
        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Ingresa usuario y contraseña.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var usuarioDb = DataStore.Users.FirstOrDefault(u => u.Username.Equals(user, StringComparison.OrdinalIgnoreCase));

                if (usuarioDb == null)
                {
                    MessageBox.Show("El usuario no existe.", "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPassword.Text = "";
                    return;
                }

                bool passwordCorrecta = BCrypt.Net.BCrypt.Verify(pass, usuarioDb.Password);

                if (passwordCorrecta)
                {
                    SessionManager.UsuarioActual = usuarioDb;
                    DataStore.RegistrarLog("INICIO_SESION", "Sesion", $"Inicio de sesion exitoso de {usuarioDb.Name} ({usuarioDb.Username})");
                    MessageBox.Show($"¡Bienvenido al sistema, {usuarioDb.Name}!", "Acceso Concedido", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    new frmMenu().Show();
                    Hide();
                }
                else
                {
                    MessageBox.Show("Contraseña incorrecta. Verifica tus datos.", "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPassword.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrio un error al iniciar sesion: " + ex.Message, "Error del Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Text = "";
            }
        }

    }

}