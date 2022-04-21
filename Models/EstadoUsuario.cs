using System;
using System.Collections.Generic;

namespace GoodDriving.Models
{
    public partial class EstadoUsuario
    {
        public EstadoUsuario()
        {
            Usuarios = new HashSet<Usuario>();
        }

        public int Id { get; set; }
        public string Estado { get; set; } = null!;

        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}
