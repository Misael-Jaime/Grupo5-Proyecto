# 🐾 Sistema de Gestión de Clínica Veterinaria

[![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/)
[![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![SQLite](https://img.shields.io/badge/SQLite-07405E?style=for-the-badge&logo=sqlite&logoColor=white)](https://www.sqlite.org/)
[![UI](https://img.shields.io/badge/UI-MaterialSkin%202-blue?style=for-the-badge)](https://github.com/leocms/MaterialSkin)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge)](https://opensource.org/licenses/MIT)

> Aplicación de escritorio desarrollada en **C# .NET** con interfaz moderna en **MaterialSkin** y persistencia en **SQLite**, diseñada para centralizar y optimizar la administración clínica, operativa y financiera de una veterinaria.

---

## 📖 Descripción del Proyecto

El **Sistema de Gestión Veterinaria** resuelve la necesidad de automatizar el control de pacientes (mascotas), dueños, expedientes médicos y facturación en clínicas y consultorios veterinarios. 

Ofrece un entorno centralizado, seguro y con autenticación por roles que permite:
* Registrar y gestionar información completa de propietarios y mascotas.
* Agendar y administrar citas veterinarias con estado en tiempo real.
* Llevar el historial clínico digital y registro de diagnósticos/tratamientos.
* Controlar inventario de medicamentos y gestión de proveedores.
* Generar facturas y reportes estadísticos de ingresos y servicios.
* Registro de auditoría (Audit Logs) para trazabilidad de operaciones.

---

## 🛠️ Tecnologías Utilizadas

* **Lenguaje de Programación:** C# (.NET 8 / Windows Forms)
* **Interfaz de Usuario:** MaterialSkin 2 (Tema oscuro/claro y componentes Material Design)
* **Persistencia de Datos:** SQLite / ADO.NET con arquitectura modular
* **Control de Versiones:** Git y GitHub
* **Entorno de Desarrollo:** Visual Studio 2022

---

## 🏛️ Arquitectura del Sistema

El proyecto implementa una arquitectura por capas para garantizar mantenibilidad y separación de responsabilidades:
* **UI / Formularios (`Forms`):** Vistas interactivas con MaterialSkin (`frmLogin`, `frmMenu`, `frmDuenos`, `frmReportes`, etc.).
* **Capa de Negocio / Modelos (`Clases`):** Entidades del dominio (`Pet`, `Owner`, `Appointment`, `MedicalRecord`, `Invoice`, `Medicine`, `User`, `AuditLog`).
* **Capa de Datos (`CapaDatos` & `DataStore`):** Gestión de conexiones, consultas SQL y operaciones CRUD.

---

## ⚙️ Instrucciones de Instalación y Ejecución

### Prerrequisitos
* Sistema Operativo: Windows 10 / 11
* [.NET SDK](https://dotnet.microsoft.com/download) o [Visual Studio 2022](https://visualstudio.microsoft.com/) con la carga de trabajo *Desarrollo de escritorio de .NET*.

### Pasos para ejecutar:
1. **Clonar el repositorio:**
   ```bash
   git clone https://github.com/Misael-Jaime/Grupo5-Proyecto.git
   cd Grupo5-Proyecto
   ```

2. **Abrir la solución:**
   * Haz doble clic en el archivo `Grupo5-Proyecto.sln` para abrirlo en Visual Studio.

3. **Restaurar paquetes y compilar:**
   * En Visual Studio: `Compilar` -> `Compilar solución` (o presiona `Ctrl + Shift + B`).
   * Por consola:
     ```bash
     dotnet restore
     dotnet build
     ```

4. **Ejecutar la aplicación:**
   * Presiona `F5` o ejecuta:
     ```bash
     dotnet run --project Grupo5-Proyecto.csproj
     ```

---

## 👤 Autor

* **Misael Jaime** - [GitHub](https://github.com/Misael-Jaime) • [LinkedIn](https://www.linkedin.com/in/misael-jaime/)
* **Equipo de Desarrollo:** Grupo 5 - Universidad Tecnológica de Panamá (UTP)

---

## 📄 Licencia

Este proyecto está bajo la Licencia **MIT** - consulta el archivo `LICENSE` para más detalles.
