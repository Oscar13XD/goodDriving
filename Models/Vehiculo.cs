using System;
using System.Collections.Generic;

namespace GoodDriving.Models
{
    public partial class Vehiculo
    {
        public int Id { get; set; }
        public int? IdMarca { get; set; }
        public int? IdModelo { get; set; }
        public int Cantidad { get; set; }

        public virtual MarcaVehiculo? IdMarcaNavigation { get; set; }
        public virtual ModeloVehiculo? IdModeloNavigation { get; set; }
    }
}
