using Grupo5_Proyecto.Clases;

namespace Grupo5_Proyecto
{
    // Clase estatica para mantener la informacion del usuario activo (el que inicio sesion) en todo el sistema
    public static class SessionManager
    {
        // Variable global que guarda los datos del usuario logueado en este momento
        public static User? UsuarioActual { get; set; }
        public static bool EstaLogueado => UsuarioActual != null;
        public static void CerrarSesion() => UsuarioActual = null;
    }
}