using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using System.Text;
using Grupo5_Proyecto.Database;
using SelectPdf;

namespace Grupo5_Proyecto.Forms
{
    public class frmReportes : frmBase
    {
        private Panel cardInventario = null!, cardIngresos = null!, cardPacientes = null!, cardCitas = null!;
        private Label lblValInventario = null!, lblValIngresos = null!, lblValPacientes = null!, lblValCitas = null!;
        private Panel panelLinea = null!, panelPastel = null!;
        private Point? pieMousePos = null;
        private Point? lineMousePos = null;

        public frmReportes()
        {
            InicializarComponentes();
            CargarMetricasYAnaliticas();
        }

        private void InitializeComponent() { }

        private void InicializarComponentes()
        {
            Text = "Analisis de Negocio";
            Size = new Size(1100, 700);
            BackColor = Color.FromArgb(10, 11, 18);

            var lblTitulo = new Label
            {
                Text = "Analisis de Negocio",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(30, 20),
                Size = new Size(300, 40)
            };
            Controls.Add(lblTitulo);

            var btnExportar = new Button
            {
                Text = "📥  Exportar Reporte",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(24, 28, 48),
                Location = new Point(885, 20),
                Size = new Size(155, 35),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnExportar.FlatAppearance.BorderSize = 0;
            btnExportar.Click += BtnExportar_Click;
            Controls.Add(btnExportar);

            var tlpCards = new TableLayoutPanel
            {
                Location = new Point(30, 75),
                Size = new Size(1035, 90),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                ColumnCount = 4,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            for (int i = 0; i < 4; i++) tlpCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));

            cardInventario = CrearTarjeta("Valor Inventario", "📦", Color.FromArgb(30, 35, 65));
            cardIngresos = CrearTarjeta("Ingresos Anuales", "💵", Color.FromArgb(20, 45, 35));
            cardPacientes = CrearTarjeta("Total Pacientes", "❤️", Color.FromArgb(45, 25, 35));
            cardCitas = CrearTarjeta("Citas Totales", "👤", Color.FromArgb(45, 35, 20));

            tlpCards.Controls.Add(cardInventario, 0, 0);
            tlpCards.Controls.Add(cardIngresos, 1, 0);
            tlpCards.Controls.Add(cardPacientes, 2, 0);
            tlpCards.Controls.Add(cardCitas, 3, 0);
            Controls.Add(tlpCards);

            lblValInventario = CrearLabelValor(cardInventario);
            lblValIngresos = CrearLabelValor(cardIngresos);
            lblValPacientes = CrearLabelValor(cardPacientes);
            lblValCitas = CrearLabelValor(cardCitas);

            panelLinea = new Panel { Location = new Point(30, 185), Size = new Size(650, 430), BackColor = Color.FromArgb(16, 18, 27), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };
            var lblT = new Label { Text = "📈 Tendencia de Ingresos", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.FromArgb(148, 163, 210), Location = new Point(20, 15), Size = new Size(300, 25) };
            panelLinea.Controls.Add(lblT);
            panelLinea.Paint += DibujarGraficoLineas_Paint;
            panelLinea.MouseMove += (s, e) => { lineMousePos = e.Location; panelLinea.Invalidate(); };
            panelLinea.MouseLeave += (s, e) => { lineMousePos = null; panelLinea.Invalidate(); };
            typeof(Panel).InvokeMember("DoubleBuffered", System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, panelLinea, new object[] { true });
            Controls.Add(panelLinea);

            panelPastel = new Panel { Location = new Point(700, 185), Size = new Size(365, 430), BackColor = Color.FromArgb(16, 18, 27), Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom };
            var lblD = new Label { Text = "📊 Distribucion por Especie", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.FromArgb(148, 163, 210), Location = new Point(20, 15), Size = new Size(300, 25) };
            panelPastel.Controls.Add(lblD);
            panelPastel.Paint += DibujarGraficoPastel_Paint;
            panelPastel.MouseMove += (s, e) => { pieMousePos = e.Location; panelPastel.Invalidate(); };
            panelPastel.MouseLeave += (s, e) => { pieMousePos = null; panelPastel.Invalidate(); };
            typeof(Panel).InvokeMember("DoubleBuffered", System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, panelPastel, new object[] { true });
            Controls.Add(panelPastel);

            this.Resize += (s, e) => { panelLinea.Invalidate(); panelPastel.Invalidate(); };
        }

        private Panel CrearTarjeta(string titulo, string icono, Color iconBg)
        {
            var card = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 15, 0), BackColor = Color.FromArgb(16, 18, 27) };

            var pnlIcono = new Panel { Location = new Point(15, 20), Size = new Size(45, 45), BackColor = iconBg };
            var lblIcono = new Label { Text = icono, Font = new Font("Segoe UI", 14), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            pnlIcono.Controls.Add(lblIcono);
            card.Controls.Add(pnlIcono);

            var lblTitle = new Label { Text = titulo, Font = new Font("Segoe UI", 8), ForeColor = Color.FromArgb(148, 163, 210), Location = new Point(70, 20), Size = new Size(150, 18) };
            card.Controls.Add(lblTitle);

            return card;
        }

        private Label CrearLabelValor(Panel parent)
        {
            var lbl = new Label { Text = "$0", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.White, Location = new Point(70, 38), Size = new Size(160, 30), TextAlign = ContentAlignment.MiddleLeft };
            parent.Controls.Add(lbl);
            return lbl;
        }

        private void CargarMetricasYAnaliticas()
        {
            double valorInventario = DataStore.Medicines.Sum(m => m.Price * m.Stock);
            double ingresosAnuales = DataStore.Invoices.Where(i => i.Date.Year == DateTime.Now.Year && (i.Status == "PAGADA" || i.Status == "PAID")).Sum(i => i.Total);

            lblValInventario.Text = "$" + valorInventario.ToString("N0");
            lblValIngresos.Text = "$" + ingresosAnuales.ToString("N0");
            lblValPacientes.Text = DataStore.Pets.Count.ToString();
            lblValCitas.Text = DataStore.Appointments.Count.ToString();
        }

        // Accion cuando se le da clic al boton
        // Obtenemos lo que escribio el usuario y procesamos
        private void BtnExportar_Click(object? sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Documento PDF (*.pdf)|*.pdf|Archivos CSV (*.csv)|*.csv";
                sfd.FileName = $"Reporte_Negocio_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
                sfd.Title = "Exportar Reporte";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Preparamos un constructor de textos (StringBuilder) para armar nuestro reporte pedazo a pedazo
                        var sb = new StringBuilder();

                        if (sfd.FileName.EndsWith(".csv"))
                        {
                            sb.AppendLine("REPORTE DE ANALISIS DE NEGOCIO");
                            sb.AppendLine($"Fecha de exportacion:;{DateTime.Now}");
                            sb.AppendLine();
                            sb.AppendLine("METRICAS GLOBALES");
                            sb.AppendLine("Metrica;Valor");
                            sb.AppendLine($"Valor Inventario;{lblValInventario.Text}");
                            sb.AppendLine($"Ingresos Anuales;{lblValIngresos.Text}");
                            sb.AppendLine($"Total Pacientes;{lblValPacientes.Text}");
                            sb.AppendLine($"Citas Totales;{lblValCitas.Text}");
                            sb.AppendLine();
                            sb.AppendLine("TENDENCIA DE INGRESOS (Anio Actual)");
                            sb.AppendLine("Mes;Ingresos");
                            string[] meses = { "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" };
                            // Filtramos las facturas del año actual y las agrupamos por mes para sumar los totales
                            var facturasPorMes = DataStore.Invoices.Where(i => i.Date.Year == DateTime.Now.Year && (i.Status == "PAGADA" || i.Status == "PAID"))
                                .GroupBy(i => i.Date.Month).ToDictionary(g => g.Key, g => g.Sum(x => x.Total));
                            // Hacemos un ciclo del 1 al 12 (los doce meses del año) para mostrar sus resultados
                            for (int i = 1; i <= 12; i++)
                            {
                                double total = facturasPorMes.ContainsKey(i) ? facturasPorMes[i] : 0;
                                sb.AppendLine($"{meses[i - 1]};{total}");
                            }
                            sb.AppendLine();
                            sb.AppendLine("DISTRIBUCION POR ESPECIE");
                            sb.AppendLine("Especie;Cantidad");
                            // Agrupamos todas las mascotas segun su especie para contar cuantas hay de cada tipo
                            var especies = DataStore.Pets.GroupBy(p => p.Species ?? "Otros")
                                .Select(gr => new { Especie = gr.Key, Cantidad = gr.Count() });
                            foreach (var esp in especies) sb.AppendLine($"{esp.Especie};{esp.Cantidad}");
                        }
                        else
                        {
                            sb.AppendLine("<!DOCTYPE html>");
                            sb.AppendLine("<html lang='es'>");
                            sb.AppendLine("<head>");
                            sb.AppendLine("<meta charset='UTF-8' />");
                            sb.AppendLine("<meta name='viewport' content='width=device-width, initial-scale=1.0' />");
                            sb.AppendLine("<title>Reporte de Analisis de Negocio</title>");
                            sb.AppendLine("<style>");
                            sb.AppendLine("html { min-height: 100vh; background-color: #0a0b12 !important; }");
                            sb.AppendLine("body { min-height: 100vh; margin: 0; padding: 40px; background-color: #0a0b12 !important; font-family: Helvetica, Arial, sans-serif; color: #e2e8f0; box-sizing: border-box; }");
                            sb.AppendLine(".container { max-width: 800px; margin: 0 auto; background: #10121b; padding: 30px; border-radius: 10px; border: 1px solid #1e2338; }");
                            sb.AppendLine("h1 { color: #ffffff; text-align: center; border-bottom: 2px solid #4f46e5; padding-bottom: 10px; margin-bottom: 30px; }");
                            sb.AppendLine("h2 { color: #a5b4fc; margin-top: 30px; font-size: 1.2rem; }");
                            sb.AppendLine("p.date { text-align: center; color: #94a3b8; font-size: 0.9rem; margin-top: -20px; margin-bottom: 30px; }");
                            sb.AppendLine("table { width: 100%; border-collapse: collapse; margin-top: 10px; }");
                            sb.AppendLine("th, td { padding: 12px 15px; text-align: left; border-bottom: 1px solid #1e2338; }");
                            sb.AppendLine("th { background-color: #1e2338; color: #ffffff; font-weight: 600; }");
                            sb.AppendLine("tr:nth-child(even) { background-color: #151824; }");
                            sb.AppendLine("tr:hover { background-color: #3730a3; }");
                            sb.AppendLine("td.num { text-align: right; font-weight: 600; color: #ffffff; }");
                            sb.AppendLine("</style>");
                            sb.AppendLine("</head>");
                            sb.AppendLine("<body>");
                            sb.AppendLine("<div style='position: fixed; top: 0; left: 0; bottom: 0; right: 0; width: 100%; height: 100%; background-color: #0a0b12; z-index: -1;'></div>");
                            sb.AppendLine("<div class='container'>");
                            sb.AppendLine("<h1>Reporte de Analisis de Negocio</h1>");
                            sb.AppendLine($"<p class='date'>Generado el: {DateTime.Now:dd/MM/yyyy hh:mm:ss tt}</p>");

                            sb.AppendLine("<h2>Metricas Globales</h2>");
                            sb.AppendLine("<table>");
                            sb.AppendLine("<tr><th>Metrica</th><th style='text-align:right;'>Valor</th></tr>");
                            sb.AppendLine($"<tr><td>Valor del Inventario</td><td class='num'>{lblValInventario.Text}</td></tr>");
                            sb.AppendLine($"<tr><td>Ingresos Anuales Estimados</td><td class='num'>{lblValIngresos.Text}</td></tr>");
                            sb.AppendLine($"<tr><td>Pacientes Registrados</td><td class='num'>{lblValPacientes.Text}</td></tr>");
                            sb.AppendLine($"<tr><td>Citas Totales Procesadas</td><td class='num'>{lblValCitas.Text}</td></tr>");
                            sb.AppendLine("</table>");

                            sb.AppendLine("<h2>Tendencia de Ingresos (Año Actual)</h2>");
                            sb.AppendLine("<table>");
                            sb.AppendLine("<tr><th>Mes</th><th style='text-align:right;'>Ingresos Generados</th></tr>");
                            string[] meses = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
                            // Filtramos las facturas del año actual y las agrupamos por mes para sumar los totales
                            var facturasPorMes = DataStore.Invoices.Where(i => i.Date.Year == DateTime.Now.Year && (i.Status == "PAGADA" || i.Status == "PAID"))
                                .GroupBy(i => i.Date.Month).ToDictionary(g => g.Key, g => g.Sum(x => x.Total));
                            // Hacemos un ciclo del 1 al 12 (los doce meses del año) para mostrar sus resultados
                            for (int i = 1; i <= 12; i++)
                            {
                                double total = facturasPorMes.ContainsKey(i) ? facturasPorMes[i] : 0;
                                sb.AppendLine($"<tr><td>{meses[i - 1]}</td><td class='num'>${total:N2}</td></tr>");
                            }
                            sb.AppendLine("</table>");

                            sb.AppendLine("<h2>Distribucion de Pacientes por Especie</h2>");
                            sb.AppendLine("<table>");
                            sb.AppendLine("<tr><th>Especie</th><th style='text-align:right;'>Cantidad de Pacientes</th></tr>");
                            // Agrupamos todas las mascotas segun su especie para contar cuantas hay de cada tipo
                            var especies = DataStore.Pets.GroupBy(p => string.IsNullOrWhiteSpace(p.Species) ? "Otros" : p.Species)
                                .Select(gr => new { Especie = gr.Key, Cantidad = gr.Count() });
                            foreach (var esp in especies)
                            {
                                sb.AppendLine($"<tr><td>{esp.Especie}</td><td class='num'>{esp.Cantidad}</td></tr>");
                            }
                            sb.AppendLine("</table>");

                            sb.AppendLine("</div>");
                            sb.AppendLine("</body>");
                            sb.AppendLine("</html>");
                        }

                        if (sfd.FileName.EndsWith(".pdf"))
                        {
                            var converter = new HtmlToPdf();
                            converter.Options.MarginBottom = 0;
                            converter.Options.MarginTop = 0;
                            converter.Options.MarginLeft = 0;
                            converter.Options.MarginRight = 0;
                            converter.Options.PdfPageSize = PdfPageSize.A4;
                            converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;
                            converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
                            var doc = // Le pasamos todo nuestro texto armado al conversor para que genere el PDF en memoria
                            converter.ConvertHtmlString(sb.ToString());
                            // Finalmente, guardamos el archivo PDF en la ruta que eligio el usuario en su computadora
                            doc.Save(sfd.FileName);
                            doc.Close();

                            if (MessageBox.Show("Reporte exportado exitosamente a PDF.\n¿Deseas abrirlo ahora?", "Exportacion Exitosa", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                            {
                                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                                {
                                    FileName = sfd.FileName,
                                    UseShellExecute = true
                                });
                            }
                        }
                        else
                        {
                            File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                            MessageBox.Show("Reporte exportado exitosamente a CSV.", "Exportacion Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al exportar: " + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void DibujarGraficoLineas_Paint(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            string[] meses = { "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" };
            int paddingX = 50, paddingY = 50;
            int graphWidth = panelLinea.Width - paddingX - 30;
            int graphHeight = panelLinea.Height - paddingY - 60;

            double[] ingresosMensuales = new double[12];
            // Filtramos las facturas del año actual y las agrupamos por mes para sumar los totales
            var facturasPorMes = DataStore.Invoices.Where(i => i.Date.Year == DateTime.Now.Year && (i.Status == "PAGADA" || i.Status == "PAID"))
.GroupBy(i => i.Date.Month).Select(gr => new { Mes = gr.Key, Total = gr.Sum(x => x.Total) });
            foreach (var f in facturasPorMes) if (f.Mes >= 1 && f.Mes <= 12) ingresosMensuales[f.Mes - 1] = f.Total;

            double maxIngreso = ingresosMensuales.Max();
            if (maxIngreso == 0) maxIngreso = 1;

            int maxEjeY = ((int)maxIngreso / 500 + 1) * 500;
            if (maxIngreso < 500) maxEjeY = ((int)maxIngreso / 100 + 1) * 100;

            using (var penGuia = new Pen(Color.FromArgb(28, 30, 43), 1))
            using (var fEjes = new Font("Segoe UI", 8))
            using (var bEjes = new SolidBrush(Color.FromArgb(90, 95, 115)))
            {
                for (int i = 0; i <= 5; i++)
                {
                    int y = panelLinea.Height - paddingY - (graphHeight * i / 5);
                    g.DrawLine(penGuia, paddingX, y, panelLinea.Width - 30, y);
                    int valorEje = maxEjeY * i / 5;
                    g.DrawString(valorEje.ToString(), fEjes, bEjes, paddingX - 45, y - 7);
                }

                int barWidth = Math.Max(10, (graphWidth / 12) - 20);
                Rectangle hoveredBar = Rectangle.Empty;
                string hoveredMonth = "";
                double hoveredValue = 0;

                for (int i = 0; i < 12; i++)
                {
                    int x = paddingX + (graphWidth * i / 12) + (graphWidth / 24) - (barWidth / 2);
                    double proporcion = ingresosMensuales[i] / maxEjeY;

                    int barHeight = (int)(graphHeight * proporcion);
                    int y = (panelLinea.Height - paddingY) - barHeight;

                    g.DrawString(meses[i], fEjes, bEjes, x + barWidth / 2 - 12, panelLinea.Height - paddingY + 10);

                    if (barHeight > 0)
                    {
                        var rect = new Rectangle(x, y, barWidth, barHeight);
                        bool isHovered = lineMousePos.HasValue && rect.Contains(lineMousePos.Value);

                        Color barColor = isHovered ? Color.FromArgb(129, 140, 248) : Color.FromArgb(67, 56, 202);

                        using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                        {
                            int r = 5;
                            path.AddArc(rect.X, rect.Y, r * 2, r * 2, 180, 90);
                            path.AddArc(rect.Right - r * 2, rect.Y, r * 2, r * 2, 270, 90);
                            path.AddLine(rect.Right, rect.Bottom, rect.X, rect.Bottom);
                            path.CloseFigure();

                            using (var bBar = new SolidBrush(barColor))
                            {
                                g.FillPath(bBar, path);
                            }
                        }

                        if (isHovered)
                        {
                            hoveredBar = rect;
                            hoveredMonth = meses[i];
                            hoveredValue = ingresosMensuales[i];
                        }
                    }
                }

                if (!hoveredBar.IsEmpty && lineMousePos.HasValue)
                {
                    string txtLine1 = hoveredMonth;
                    string txtLine2 = $"Ingresos Mensuales ($): {hoveredValue:F0}";

                    using (var fBold = new Font("Segoe UI", 9, FontStyle.Bold))
                    using (var fNorm = new Font("Segoe UI", 9))
                    {
                        var sz1 = g.MeasureString(txtLine1, fBold);
                        var sz2 = g.MeasureString(txtLine2, fNorm);

                        float w = Math.Max(sz1.Width, sz2.Width + 15) + 20;
                        float h = sz1.Height + sz2.Height + 15;

                        float tipX = lineMousePos.Value.X + 15;
                        float tipY = lineMousePos.Value.Y + 15;

                        if (tipX + w > panelLinea.Width) tipX = lineMousePos.Value.X - w - 5;
                        if (tipY + h > panelLinea.Height) tipY = lineMousePos.Value.Y - h - 5;

                        using (var bTooltip = new SolidBrush(Color.FromArgb(15, 15, 20)))
                        using (var pBorder = new Pen(Color.FromArgb(40, 40, 50), 1))
                        using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                        {
                            int r = 5;
                            path.AddArc(tipX, tipY, r * 2, r * 2, 180, 90);
                            path.AddArc(tipX + w - r * 2, tipY, r * 2, r * 2, 270, 90);
                            path.AddArc(tipX + w - r * 2, tipY + h - r * 2, r * 2, r * 2, 0, 90);
                            path.AddArc(tipX, tipY + h - r * 2, r * 2, r * 2, 90, 90);
                            path.CloseFigure();

                            g.FillPath(bTooltip, path);
                            g.DrawPath(pBorder, path);
                        }

                        using (var bWhite = new SolidBrush(Color.White))
                        {
                            g.DrawString(txtLine1, fBold, bWhite, tipX + 10, tipY + 7);

                            using (var bColor = new SolidBrush(Color.FromArgb(129, 140, 248)))
                            {
                                g.FillRectangle(bColor, tipX + 10, tipY + 7 + sz1.Height + 2, 10, 10);
                            }

                            g.DrawString(txtLine2, fNorm, bWhite, tipX + 22, tipY + 7 + sz1.Height);
                        }
                    }
                }
            }
        }

        private void DibujarGraficoPastel_Paint(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int posX = 50, posY = 90, size = 250;
            var rectPastel = new Rectangle(posX, posY, size, size);

            int total = DataStore.Pets.Count;
            if (total == 0)
            {
                using (var brushFigma = new SolidBrush(Color.FromArgb(40, 42, 54)))
                {
                    g.FillPie(brushFigma, rectPastel, 0, 360);
                }

                using (var f = new Font("Segoe UI", 9))
                using (var b = new SolidBrush(Color.LightGray))
                {
                    g.DrawString("Sin datos", f, b, posX + 95, posY + 115);
                }
            }
            else
            {
                // Agrupamos todas las mascotas segun su especie para contar cuantas hay de cada tipo
                var especies = DataStore.Pets.GroupBy(p => p.Species ?? "Otros")
        .Select(gr => new { Especie = gr.Key, Cantidad = gr.Count() }).ToList();

                Color[] paleta = { Color.FromArgb(99, 102, 241), Color.FromArgb(52, 211, 153), Color.FromArgb(251, 191, 36), Color.FromArgb(248, 113, 113) };
                float anguloInicial = 0;

                for (int i = 0; i < especies.Count; i++)
                {
                    float sweep = (especies[i].Cantidad * 360f) / total;
                    using (var brush = new SolidBrush(paleta[i % paleta.Length]))
                    {
                        g.FillPie(brush, rectPastel, anguloInicial, sweep);
                    }
                    anguloInicial += sweep;
                }

                if (pieMousePos.HasValue)
                {
                    float cx = posX + size / 2f;
                    float cy = posY + size / 2f;
                    float mx = pieMousePos.Value.X;
                    float my = pieMousePos.Value.Y;
                    float distSq = (mx - cx) * (mx - cx) + (my - cy) * (my - cy);
                    float radius = size / 2f;

                    if (distSq <= radius * radius)
                    {
                        float anguloRaton = (float)(Math.Atan2(my - cy, mx - cx) * 180 / Math.PI);
                        if (anguloRaton < 0) anguloRaton += 360;

                        float tempAngulo = 0;
                        for (int i = 0; i < especies.Count; i++)
                        {
                            float sweep = (especies[i].Cantidad * 360f) / total;
                            if (anguloRaton >= tempAngulo && anguloRaton <= tempAngulo + sweep)
                            {
                                string especieNombre = especies[i].Especie;
                                int especieCantidad = especies[i].Cantidad;
                                Color colorEspecie = paleta[i % paleta.Length];

                                string txtLine1 = especieNombre;
                                string txtLine2 = especieCantidad.ToString();

                                using (var fBold = new Font("Segoe UI", 9, FontStyle.Bold))
                                using (var fNorm = new Font("Segoe UI", 9))
                                {
                                    var sz1 = g.MeasureString(txtLine1, fBold);
                                    var sz2 = g.MeasureString(txtLine2, fNorm);
                                    float w = Math.Max(sz1.Width, sz2.Width + 15) + 20;
                                    float h = sz1.Height + sz2.Height + 15;

                                    float tipX = mx + 15;
                                    float tipY = my + 15;

                                    if (tipX + w > panelPastel.Width) tipX = mx - w - 5;
                                    if (tipY + h > panelPastel.Height) tipY = my - h - 5;

                                    using (var bTooltip = new SolidBrush(Color.FromArgb(15, 15, 20)))
                                    using (var pBorder = new Pen(Color.FromArgb(40, 40, 50), 1))
                                    using (var path = new GraphicsPath())
                                    {
                                        int r = 5;
                                        path.AddArc(tipX, tipY, r * 2, r * 2, 180, 90);
                                        path.AddArc(tipX + w - r * 2, tipY, r * 2, r * 2, 270, 90);
                                        path.AddArc(tipX + w - r * 2, tipY + h - r * 2, r * 2, r * 2, 0, 90);
                                        path.AddArc(tipX, tipY + h - r * 2, r * 2, r * 2, 90, 90);
                                        path.CloseFigure();

                                        g.FillPath(bTooltip, path);
                                        g.DrawPath(pBorder, path);
                                    }

                                    using (var bWhite = new SolidBrush(Color.White))
                                    {
                                        g.DrawString(txtLine1, fBold, bWhite, tipX + 10, tipY + 7);

                                        using (var bColor = new SolidBrush(colorEspecie))
                                        {
                                            g.FillRectangle(bColor, tipX + 10, tipY + 7 + sz1.Height + 2, 10, 10);
                                        }

                                        g.DrawString(txtLine2, fNorm, bWhite, tipX + 22, tipY + 7 + sz1.Height);
                                    }
                                }
                                break;
                            }
                            tempAngulo += sweep;
                        }
                    }
                }
            }

            if (DataStore.Pets.Count > 0)
            {
                // Agrupamos todas las mascotas segun su especie para contar cuantas hay de cada tipo
                var especies = DataStore.Pets.GroupBy(p => p.Species ?? "Otros")
        .Select(gr => new { Especie = gr.Key, Cantidad = gr.Count() }).ToList();
                Color[] paleta = { Color.FromArgb(99, 102, 241), Color.FromArgb(52, 211, 153), Color.FromArgb(251, 191, 36), Color.FromArgb(248, 113, 113) };

                using (var f = new Font("Segoe UI", 9))
                using (var b = new SolidBrush(Color.LightGray))
                {
                    int legendX = posX + 20;
                    int legendY = posY - 40;
                    for (int i = 0; i < especies.Count; i++)
                    {
                        using (var brushColor = new SolidBrush(paleta[i % paleta.Length]))
                        {
                            g.FillCircle(brushColor, legendX, legendY + 5, 5);
                        }
                        g.DrawString(especies[i].Especie, f, b, legendX + 10, legendY - 2);

                        var sz = g.MeasureString(especies[i].Especie, f);
                        legendX += (int)sz.Width + 30;
                        if (legendX > posX + size - 30)
                        {
                            legendX = posX + 20;
                            legendY += 20;
                        }
                    }
                }
            }
        }
    }

    public static class GraphicsExtensions
    {
        public static void FillCircle(this Graphics g, Brush brush, float x, float y, float radius)
        {
            g.FillEllipse(brush, x - radius, y - radius, radius * 2, radius * 2);
        }
    }
}