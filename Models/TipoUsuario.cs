using System;
using System.Collections.Generic;

namespace GoodDriving.Models
{
    public partial class TipoUsuario
    {
        public TipoUsuario()
        {
            Usuarios = new HashSet<Usuario>();
        }

        public int Id { get; set; }
        public string Tipo { get; set; } = null!;

        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}
