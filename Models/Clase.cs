using System;
using System.Collections.Generic;

namespace GoodDriving.Models
{
    public partial class Clase
    {
        public int Id { get; set; }
        public int? IdTutor { get; set; }
        public int? IdUsuario { get; set; }
        public int? IdEstado { get; set; }
        public int? IdVehiculo { get; set; }
        public int? IdLicencia { get; set; }
        public int? IdTipo { get; set; }
        public DateTime? FechaSolicitud { get; set; }
        public DateTime? FechaFinalizacion { get; set; }
        public string? ReportePdf { get; set; }

        public virtual EstadoClase? IdEstadoNavigation { get; set; }
        public virtual Licencium? IdLicenciaNavigation { get; set; }
        public virtual TipoClase? IdTipoNavigation { get; set; }
        public virtual Usuario? IdTutorNavigation { get; set; }
        public virtual Usuario? IdUsuarioNavigation { get; set; }
        public virtual Vehiculo? IdVehiculoNavigation { get; set; }
    }
}
