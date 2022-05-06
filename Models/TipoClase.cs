using System;
using System.Collections.Generic;

namespace GoodDriving.Models
{
    public partial class TipoClase
    {
        public TipoClase()
        {
            Clases = new HashSet<Clase>();
        }

        public int Id { get; set; }
        public string? Descripcion { get; set; }

        public virtual ICollection<Clase> Clases { get; set; }
    }
}
