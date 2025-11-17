using System;
using System.Collections.Generic;

namespace INTERBANK.Model;

public partial class Beneficio
{
    public int IdBeneficio { get; set; }

    public string Tipo { get; set; } = null!;

    public decimal Puntos { get; set; }

    public DateTime FechaActualizacion { get; set; }

    public int IdUsuario { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
