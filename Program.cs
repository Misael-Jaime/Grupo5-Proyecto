using System;
using System.Windows.Forms;
using Grupo5_Proyecto.Forms;
using MaterialSkin;

namespace Grupo5_Proyecto
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Configuramos el tema oscuro y los colores base (indigo) de los controles MaterialSkin
            var skinManager = MaterialSkinManager.Instance;
            skinManager.EnforceBackcolorOnAllComponents = true;
            skinManager.Theme = MaterialSkinManager.Themes.DARK;
            skinManager.ColorScheme = new ColorScheme(
                Primary.Indigo700,
                Primary.Indigo900,
                Primary.Indigo500,
                Accent.Indigo200,
                TextShade.WHITE
            );
            Application.Run(new frmLogin());
        }
    }
}