using System;
using System.Collections.Generic;

namespace GoodDriving.Models
{
    public partial class HorarioTutor
    {
        public int Id { get; set; }
        public int? IdTutor { get; set; }
        public string? Dia { get; set; }
        public string? Hora { get; set; }
        public int? Cupo { get; set; }

        public virtual Usuario? IdTutorNavigation { get; set; }
    }
}
