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
    public class frmDuenos : frmBase
    {
        private DataGridView dgvDuenos = null!;
        private MaterialTextBox txtBuscar = null!;
        private MaterialTextBox txtNombre = null!;
        private MaterialTextBox txtCedula = null!;
        private MaterialTextBox txtTelefono = null!;
        private MaterialTextBox txtEmail = null!;
        private MaterialTextBox txtDireccion = null!;
        private int idSeleccionado = 0;

        public frmDuenos()
        {
            InicializarComponentes();
            RefrescarTabla();
        }

        // Metodo principal para dibujar toda la interfaz visual de esta pantalla
        private void InicializarComponentes()
        {
            Text = "Gestion de Dueños";
            Size = new Size(1000, 650);
            StartPosition = FormStartPosition.CenterScreen;

            Controls.Add(CrearLabel("👥 Gestion de Dueños", 20, 80, true));

            txtBuscar = CrearCampo("🔍 Buscar por nombre o cedula...", 20, 120, 350);
            txtBuscar.TextChanged += (s, e) => RefrescarTabla();
            Controls.Add(txtBuscar);

            dgvDuenos = new DataGridView { Location = new Point(20, 185), Size = new Size(580, 400) };
            EstilarTabla(dgvDuenos);
            dgvDuenos.SelectionChanged += Dgv_SelectionChanged;
            Controls.Add(dgvDuenos);

            Controls.Add(CrearLabel("📝 Datos del Dueño", 630, 80, true));
            txtNombre = CrearCampo("Nombre completo *", 630, 120, 300);
            txtCedula = CrearCampo("Cedula / Identidad *", 630, 180, 300);
            txtTelefono = CrearCampo("Telefono", 630, 240, 300);
            txtEmail = CrearCampo("Email", 630, 300, 300);
            txtDireccion = CrearCampo("Direccion", 630, 360, 300);

            txtTelefono.KeyPress += SoloTelefono_KeyPress;

            Controls.Add(txtNombre); Controls.Add(txtCedula);
            Controls.Add(txtTelefono); Controls.Add(txtEmail); Controls.Add(txtDireccion);

            var btnGuardar = CrearBoton("💾 Guardar", Color.FromArgb(56, 142, 60), 630, 430, 140);
            var btnEliminar = CrearBoton("🗑 Eliminar", Color.FromArgb(211, 47, 47), 785, 430, 140);

            btnGuardar.Click += BtnGuardar_Click;
            btnEliminar.Click += BtnEliminar_Click;

            Controls.Add(btnGuardar); Controls.Add(btnEliminar);
        }

        private void RefrescarTabla()
        {
            dgvDuenos.SelectionChanged -= Dgv_SelectionChanged;

            dgvDuenos.Rows.Clear();
            dgvDuenos.Columns.Clear();

            dgvDuenos.Columns.Add("Id", "ID");
            dgvDuenos.Columns.Add("Nombre", "Nombre");
            dgvDuenos.Columns.Add("Cedula", "Cedula");
            dgvDuenos.Columns.Add("Telefono", "Telefono");
            dgvDuenos.Columns.Add("Email", "Email");
            dgvDuenos.Columns.Add("Direccion", "Direccion");
            dgvDuenos.Columns["Id"].Width = 40;

            string filtro = txtBuscar?.Text.ToLower().Trim() ?? "";

            var lista = DataStore.Owners
                .Where(o => string.IsNullOrEmpty(filtro) ||
                            o.Name.ToLower().Contains(filtro) ||
                            o.Identity.ToLower().Contains(filtro))
                .ToList();

            foreach (var o in lista)
                dgvDuenos.Rows.Add(o.Id, o.Name, o.Identity,
                    o.Phone ?? "", o.Email ?? "", o.Address ?? "");

            dgvDuenos.ClearSelection();

            dgvDuenos.SelectionChanged += Dgv_SelectionChanged;
        }

        private void Dgv_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvDuenos.SelectedRows.Count == 0 || dgvDuenos.CurrentRow == null) return;
            idSeleccionado = Convert.ToInt32(dgvDuenos.CurrentRow.Cells["Id"].Value);
            txtNombre.Text = dgvDuenos.CurrentRow.Cells["Nombre"].Value?.ToString() ?? "";
            txtCedula.Text = dgvDuenos.CurrentRow.Cells["Cedula"].Value?.ToString() ?? "";
            txtTelefono.Text = dgvDuenos.CurrentRow.Cells["Telefono"].Value?.ToString() ?? "";
            txtEmail.Text = dgvDuenos.CurrentRow.Cells["Email"].Value?.ToString() ?? "";
            txtDireccion.Text = dgvDuenos.CurrentRow.Cells["Direccion"].Value?.ToString() ?? "";
        }

        // Accion cuando se le da clic al boton
        // Obtenemos lo que escribio el usuario y procesamos
        private void BtnGuardar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtCedula.Text))
            {
                MessageBox.Show("Nombre y Cedula son obligatorios.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (idSeleccionado == 0)
            {
                var nuevo = new Owner
                {
                    Id = DataStore.NextOwnerId(),
                    Name = txtNombre.Text.Trim(),
                    Identity = txtCedula.Text.Trim(),
                    Phone = txtTelefono.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Address = txtDireccion.Text.Trim()
                };
                DataStore.Owners.Add(nuevo);
                DataStore.RegistrarLog("CREACION", "Dueno", $"Nuevo dueño: {nuevo.Name}");
                MessageBox.Show("Dueño registrado.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                var o = DataStore.Owners.FirstOrDefault(x => x.Id == idSeleccionado);
                if (o != null)
                {
                    o.Name = txtNombre.Text.Trim();
                    o.Identity = txtCedula.Text.Trim();
                    o.Phone = txtTelefono.Text.Trim();
                    o.Email = txtEmail.Text.Trim();
                    o.Address = txtDireccion.Text.Trim();
                    DataStore.RegistrarLog("ACTUALIZACION", "Dueno", $"Actualizado dueño: {o.Name}");
                    MessageBox.Show("Dueño actualizado.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            RefrescarTabla(); LimpiarFormulario();
        }

        // Accion cuando se le da clic al boton
        // Obtenemos lo que escribio el usuario y procesamos
        private void BtnEliminar_Click(object? sender, EventArgs e)
        {
            if (idSeleccionado == 0) { MessageBox.Show("Selecciona un dueño primero."); return; }
            if (MessageBox.Show("¿Eliminar este dueño?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            var o = DataStore.Owners.FirstOrDefault(x => x.Id == idSeleccionado);
            if (o != null)
            {
                DataStore.Owners.Remove(o);
                DataStore.RegistrarLog("ELIMINACION", "Dueno", $"Eliminado dueño: {o.Name}");
                MessageBox.Show("Dueño eliminado.", "Exito");
            }
            RefrescarTabla(); LimpiarFormulario();
        }

        private void LimpiarFormulario()
        {
            idSeleccionado = 0;
            txtNombre.Text = txtCedula.Text = txtTelefono.Text = txtEmail.Text = txtDireccion.Text = "";
            dgvDuenos.SelectionChanged -= Dgv_SelectionChanged;
            dgvDuenos.ClearSelection();
            dgvDuenos.SelectionChanged += Dgv_SelectionChanged;
        }
    }
}