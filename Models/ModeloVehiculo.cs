using System;
using System.Collections.Generic;

namespace GoodDriving.Models
{
    public partial class ModeloVehiculo
    {
        public ModeloVehiculo()
        {
            Vehiculos = new HashSet<Vehiculo>();
        }

        public int Id { get; set; }
        public string? Descripcion { get; set; }

        public virtual ICollection<Vehiculo> Vehiculos { get; set; }
    }
}
