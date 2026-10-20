using System.Drawing;
using System.Windows.Forms;

namespace Grupo5_Proyecto.Forms
{
    public class frmBase : System.Windows.Forms.Form
    {
        public frmBase()
        {
            BackColor = Color.FromArgb(12, 14, 23);
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer, true);
            MaximizeBox = false;
            MinimizeBox = false;
            FormBorderStyle = FormBorderStyle.None;
        }

        // Configuramos los colores y el estilo general de las tablas para que se vean modernas
        protected static void EstilarTabla(DataGridView dgv)
        {
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(35, 42, 70);
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 27, 75);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(203, 213, 225);
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(30, 27, 75);
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(203, 213, 225);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold);
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersHeight = 45;
            dgv.EnableHeadersVisualStyles = false;

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.DefaultCellStyle.BackColor = Color.FromArgb(17, 20, 36);
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(226, 232, 240);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(79, 70, 229);
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(24, 28, 48);
            dgv.AlternatingRowsDefaultCellStyle.ForeColor = Color.FromArgb(226, 232, 240);
            dgv.BackgroundColor = Color.FromArgb(17, 20, 36);
            dgv.RowTemplate.Height = 38;
        }

        // Metodo para crear botones estandarizados en todo el sistema
        protected static Button CrearBoton(string texto, Color color, int x, int y, int ancho = 130)
        {
            var btn = new Button
            {
                Text = texto,
                Location = new Point(x, y),
                Size = new Size(ancho, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold)
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        protected static Panel CrearCampoPanel(string placeholder, int x, int y, int ancho = 200)
        {
            var pnl = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(ancho, 42),
                BackColor = Color.FromArgb(24, 28, 48)
            };
            pnl.Paint += (s, e) =>
            {
                using (var pen = new System.Drawing.Pen(Color.FromArgb(51, 65, 85), 1))
                    e.Graphics.DrawRectangle(pen, 0, 0, pnl.Width - 1, pnl.Height - 1);
            };
            var tb = new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(24, 28, 48),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10.5F),
                Location = new Point(10, 11),
                Width = ancho - 22,
                Tag = placeholder
            };
            tb.Text = placeholder;
            tb.ForeColor = Color.FromArgb(100, 116, 139);
            tb.GotFocus += (s, e) =>
            {
                if (tb.Text == placeholder) { tb.Text = ""; tb.ForeColor = Color.White; }
            };
            tb.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(tb.Text)) { tb.Text = placeholder; tb.ForeColor = Color.FromArgb(100, 116, 139); }
            };
            pnl.Controls.Add(tb);
            return pnl;
        }

        protected static MaterialSkin.Controls.MaterialTextBox CrearCampo(string hint, int x, int y, int ancho = 200)
        {
            return new MaterialSkin.Controls.MaterialTextBox
            {
                Hint = hint,
                Location = new Point(x, y),
                Size = new Size(ancho, 48)
            };
        }

        // Metodo auxiliar para crear etiquetas de texto rapido
        protected static Label CrearLabel(string texto, int x, int y, bool negrita = false)
        {
            return new Label
            {
                Text = texto,
                Location = new Point(x, y),
                AutoSize = true,
                Font = negrita
                    ? new Font("Segoe UI Semibold", 11, FontStyle.Bold)
                    : new Font("Segoe UI", 10),
                ForeColor = negrita
                    ? Color.White
                    : Color.FromArgb(148, 163, 184)
            };
        }

        protected static void SoloEnteros_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        protected static void SoloDecimales_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;
            if (char.IsDigit(e.KeyChar)) return;

            if (e.KeyChar == '.' || e.KeyChar == ',')
            {
                var tb = sender as TextBox;
                if (tb != null)
                {
                    if (tb.Text.Contains(".") || tb.Text.Contains(","))
                    {
                        e.Handled = true;
                    }
                }
                else
                {
                    var mtb = sender as MaterialSkin.Controls.MaterialTextBox;
                    if (mtb != null)
                    {
                        if (mtb.Text.Contains(".") || mtb.Text.Contains(","))
                        {
                            e.Handled = true;
                        }
                    }
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        protected static void SoloTelefono_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '-' && e.KeyChar != '+' && e.KeyChar != ' ')
            {
                e.Handled = true;
            }
        }

        // Le damos estilo visual a las listas desplegables
        protected static void EstilarComboBox(ComboBox cmb)
        {
            cmb.DropDownStyle = ComboBoxStyle.DropDownList;
            cmb.FlatStyle = FlatStyle.Flat;
            cmb.BackColor = Color.FromArgb(24, 28, 48);
            cmb.ForeColor = Color.White;
            cmb.Font = new Font("Segoe UI", 10);
            cmb.DrawMode = DrawMode.OwnerDrawFixed;
            cmb.ItemHeight = 28;

            cmb.DrawItem += (s, e) =>
            {
                if (e.Index < 0) return;
                bool isHovered = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
                Color bg = isHovered ? Color.FromArgb(79, 70, 229) : Color.FromArgb(24, 28, 48);
                Color fg = Color.White;

                using (var brushBg = new SolidBrush(bg))
                {
                    e.Graphics.FillRectangle(brushBg, e.Bounds);
                }

                string itemText = cmb.Items[e.Index]?.ToString() ?? "";
                using (var brushFg = new SolidBrush(fg))
                {
                    var sf = new StringFormat
                    {
                        LineAlignment = StringAlignment.Center,
                        Alignment = StringAlignment.Near
                    };
                    var textRect = new Rectangle(e.Bounds.X + 8, e.Bounds.Y, e.Bounds.Width - 8, e.Bounds.Height);
                    e.Graphics.DrawString(itemText, cmb.Font, brushFg, textRect, sf);
                }
                e.DrawFocusRectangle();
            };

            var pnlContainer = new Panel
            {
                BackColor = Color.FromArgb(24, 28, 48),
                Cursor = Cursors.Hand
            };

            var pnlClip = new Panel
            {
                BackColor = Color.FromArgb(24, 28, 48),
                Cursor = Cursors.Hand
            };

            pnlContainer.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(51, 65, 85), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, pnlContainer.Width - 1, pnlContainer.Height - 1);
                }
            };

            var lblArrow = new Label
            {
                Text = "▾",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(148, 163, 184),
                BackColor = Color.FromArgb(24, 28, 48),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(24, 26),
                Cursor = Cursors.Hand
            };

            void AlinearControles()
            {
                pnlClip.Location = new Point(1, 1);
                pnlClip.Width = pnlContainer.Width - 2;
                pnlClip.Height = pnlContainer.Height - 2;

                cmb.Location = new Point(-3, -3);
                cmb.Width = pnlClip.Width + 6;
                cmb.Height = pnlClip.Height + 6;

                lblArrow.Height = pnlContainer.Height - 2;
                lblArrow.Location = new Point(pnlContainer.Width - 25, 1);
            }

            cmb.ParentChanged += (s, e) =>
            {
                var parent = cmb.Parent;
                if (parent != null && parent != pnlClip)
                {
                    pnlContainer.Location = cmb.Location;
                    pnlContainer.Size = cmb.Size;

                    parent.Controls.Remove(cmb);
                    parent.Controls.Add(pnlContainer);

                    pnlContainer.Controls.Add(pnlClip);
                    pnlClip.Controls.Add(cmb);
                    pnlContainer.Controls.Add(lblArrow);

                    AlinearControles();
                    lblArrow.BringToFront();
                }
            };

            pnlContainer.SizeChanged += (s, e) => AlinearControles();

            lblArrow.Click += (s, e) =>
            {
                cmb.Focus();
                cmb.DroppedDown = !cmb.DroppedDown;
            };
        }

        protected static void EstilarDateTimePicker(DateTimePicker dtp)
        {
            dtp.Format = DateTimePickerFormat.Custom;
            dtp.CustomFormat = "dd/MM/yyyy HH:mm";
            dtp.ShowUpDown = false;
            dtp.TabStop = false;

            dtp.CalendarMonthBackground = Color.FromArgb(24, 28, 48);
            dtp.CalendarForeColor = Color.White;
            dtp.CalendarTitleBackColor = Color.FromArgb(79, 70, 229);
            dtp.CalendarTitleForeColor = Color.White;
            dtp.CalendarTrailingForeColor = Color.FromArgb(148, 163, 184);

            var pnlContainer = new Panel { BackColor = Color.FromArgb(24, 28, 48), Cursor = Cursors.Hand };
            pnlContainer.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(51, 65, 85), 1);
                e.Graphics.DrawRectangle(pen, 0, 0, pnlContainer.Width - 1, pnlContainer.Height - 1);
            };

            var pnlClip = new Panel { BackColor = Color.FromArgb(24, 28, 48) };

            var txtValue = new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(24, 28, 48),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                ReadOnly = true,
                Cursor = Cursors.Hand,
                TabStop = false
            };

            var lblIcon = new Label
            {
                Text = "📅",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(148, 163, 184),
                BackColor = Color.FromArgb(24, 28, 48),
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand
            };

            lblIcon.MouseEnter += (s, e) => lblIcon.ForeColor = Color.White;
            lblIcon.MouseLeave += (s, e) => lblIcon.ForeColor = Color.FromArgb(148, 163, 184);

            void AlinearControles()
            {
                pnlClip.Location = new Point(1, 1);
                pnlClip.Width = pnlContainer.Width - 2;
                pnlClip.Height = pnlContainer.Height - 2;

                txtValue.Location = new Point(8, (pnlClip.Height - txtValue.Height) / 2);
                txtValue.Width = pnlClip.Width - 34;

                lblIcon.Size = new Size(28, pnlClip.Height);
                lblIcon.Location = new Point(pnlClip.Width - 28, 0);

                dtp.Size = new Size(1, 1);
                dtp.Location = new Point(pnlClip.Width - 2, pnlClip.Height - 2);
            }

            dtp.ValueChanged += (s, e) => txtValue.Text = dtp.Value.ToString("dd/MM/yyyy HH:mm");
            txtValue.Text = dtp.Value.ToString("dd/MM/yyyy HH:mm");

            void AbrirCalendario()
            {
                dtp.Focus();
                SendKeys.Send("%{DOWN}");
            }

            txtValue.Click += (s, e) => AbrirCalendario();
            lblIcon.Click += (s, e) => AbrirCalendario();
            pnlContainer.Click += (s, e) => AbrirCalendario();
            pnlClip.Click += (s, e) => AbrirCalendario();

            dtp.ParentChanged += (s, e) =>
            {
                var parent = dtp.Parent;
                if (parent != null && parent != pnlClip)
                {
                    pnlContainer.Location = dtp.Location;
                    pnlContainer.Size = dtp.Size;

                    parent.Controls.Remove(dtp);
                    parent.Controls.Add(pnlContainer);

                    pnlContainer.Controls.Add(pnlClip);
                    pnlClip.Controls.Add(txtValue);
                    pnlClip.Controls.Add(dtp);
                    pnlClip.Controls.Add(lblIcon);

                    AlinearControles();
                    lblIcon.BringToFront();
                }
            };

            pnlContainer.SizeChanged += (s, e) => AlinearControles();
        }
    }

    // Clase auxiliar para crear cuadros de dialogo de alerta (exito o error) con nuestro propio estilo visual
    public class frmAlerta : Form
    {
        public frmAlerta(string mensaje, string titulo, bool esError)
        {
            Text = titulo;
            Size = new Size(380, 180);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.FromArgb(17, 20, 36);

            this.Paint += (s, e) =>
            {
                var borderColor = esError ? Color.FromArgb(239, 68, 68) : Color.FromArgb(99, 102, 241);
                using (var pen = new Pen(borderColor, 2))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
                }
            };

            var lblIcon = new Label
            {
                Text = esError ? "❌" : "✨",
                Font = new Font("Segoe UI Emoji", 24),
                ForeColor = esError ? Color.FromArgb(239, 68, 68) : Color.FromArgb(253, 224, 71),
                Location = new Point(20, 25),
                Size = new Size(50, 50),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            var lblTitulo = new Label
            {
                Text = titulo,
                Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(80, 22),
                Size = new Size(280, 25),
                BackColor = Color.Transparent
            };

            var lblMensaje = new Label
            {
                Text = mensaje,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(80, 52),
                Size = new Size(280, 60),
                TextAlign = ContentAlignment.TopLeft,
                BackColor = Color.Transparent
            };

            var btnAceptar = new Button
            {
                Text = "Aceptar",
                Location = new Point(125, 125),
                Size = new Size(130, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = esError ? Color.FromArgb(239, 68, 68) : Color.FromArgb(79, 70, 229),
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold)
            };
            btnAceptar.FlatAppearance.BorderSize = 0;
            btnAceptar.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            Controls.Add(lblIcon);
            Controls.Add(lblTitulo);
            Controls.Add(lblMensaje);
            Controls.Add(btnAceptar);

            AcceptButton = btnAceptar;
        }

        public static void Mostrar(string mensaje, string titulo = "Aviso", bool esError = false)
        {
            using (var alert = new frmAlerta(mensaje, titulo, esError))
            {
                alert.ShowDialog();
            }
        }
    }

    // Clase auxiliar para crear cuadros de dialogo que piden confirmacion (Si/No) al usuario
    public class frmConfirmacion : Form
    {
        public frmConfirmacion(string mensaje, string titulo)
        {
            Text = titulo;
            Size = new Size(380, 180);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.FromArgb(17, 20, 36);

            this.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(245, 158, 11), 2))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
                }
            };

            var lblIcon = new Label
            {
                Text = "❓",
                Font = new Font("Segoe UI Emoji", 24),
                ForeColor = Color.FromArgb(96, 165, 250),
                Location = new Point(20, 25),
                Size = new Size(50, 50),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            var lblTitulo = new Label
            {
                Text = titulo,
                Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(80, 22),
                Size = new Size(280, 25),
                BackColor = Color.Transparent
            };

            var lblMensaje = new Label
            {
                Text = mensaje,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(80, 52),
                Size = new Size(280, 60),
                TextAlign = ContentAlignment.TopLeft,
                BackColor = Color.Transparent
            };

            var btnSi = new Button
            {
                Text = "Si",
                Location = new Point(90, 125),
                Size = new Size(80, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(79, 70, 229),
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold)
            };
            btnSi.FlatAppearance.BorderSize = 0;
            btnSi.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Yes;
                this.Close();
            };

            var btnNo = new Button
            {
                Text = "No",
                Location = new Point(190, 125),
                Size = new Size(80, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(55, 65, 81),
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold)
            };
            btnNo.FlatAppearance.BorderSize = 0;
            btnNo.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.No;
                this.Close();
            };

            Controls.Add(lblIcon);
            Controls.Add(lblTitulo);
            Controls.Add(lblMensaje);
            Controls.Add(btnSi);
            Controls.Add(btnNo);

            AcceptButton = btnSi;
            CancelButton = btnNo;
        }
    }

    public static class MessageBox
    {
        public static DialogResult Show(string text)
        {
            frmAlerta.Mostrar(text, "Aviso", false);
            return DialogResult.OK;
        }

        public static DialogResult Show(string text, string caption)
        {
            bool esError = caption.ToLower().Contains("error") || caption.ToLower().Contains("denegado");
            frmAlerta.Mostrar(text, caption, esError);
            return DialogResult.OK;
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
        {
            if (buttons == MessageBoxButtons.YesNo || buttons == MessageBoxButtons.YesNoCancel)
            {
                using (var confirm = new frmConfirmacion(text, caption))
                {
                    return confirm.ShowDialog();
                }
            }
            bool esError = caption.ToLower().Contains("error") || caption.ToLower().Contains("denegado");
            frmAlerta.Mostrar(text, caption, esError);
            return DialogResult.OK;
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            bool esError = icon == MessageBoxIcon.Error || icon == MessageBoxIcon.Hand || icon == MessageBoxIcon.Stop ||
                           caption.ToLower().Contains("error") || caption.ToLower().Contains("denegado");

            if (buttons == MessageBoxButtons.YesNo || buttons == MessageBoxButtons.YesNoCancel)
            {
                using (var confirm = new frmConfirmacion(text, caption))
                {
                    return confirm.ShowDialog();
                }
            }
            frmAlerta.Mostrar(text, caption, esError);
            return DialogResult.OK;
        }
    }

    public class ScrollPanelV : System.Windows.Forms.Panel
    {
        private const int WS_HSCROLL = 0x0010_0000;

        protected override System.Windows.Forms.CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.Style &= ~WS_HSCROLL;
                return cp;
            }
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == 0x114) return;
            base.WndProc(ref m);
        }
    }
}