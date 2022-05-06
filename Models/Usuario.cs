using System;
using System.Collections.Generic;

namespace GoodDriving.Models
{
    public partial class Usuario
    {
        public Usuario()
        {
            ClaseIdTutorNavigations = new HashSet<Clase>();
            ClaseIdUsuarioNavigations = new HashSet<Clase>();
            HorarioTutors = new HashSet<HorarioTutor>();
        }

        public int Id { get; set; }
        public string NoDocumento { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Apellido1 { get; set; } = null!;
        public string? Apellido2 { get; set; }
        public string Nombre1 { get; set; } = null!;
        public string? Nombre2 { get; set; }
        public long Telefono { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Direccion { get; set; } = null!;
        public int IdTipo { get; set; }
        public int IdEstado { get; set; }
        public int? IdTipoDocumento { get; set; }
        public string? TokenRecovery { get; set; }

        public virtual EstadoUsuario IdEstadoNavigation { get; set; } = null!;
        public virtual TipoDocumento? IdTipoDocumentoNavigation { get; set; }
        public virtual TipoUsuario IdTipoNavigation { get; set; } = null!;
        public virtual ICollection<Clase> ClaseIdTutorNavigations { get; set; }
        public virtual ICollection<Clase> ClaseIdUsuarioNavigations { get; set; }
        public virtual ICollection<HorarioTutor> HorarioTutors { get; set; }
    }
}
