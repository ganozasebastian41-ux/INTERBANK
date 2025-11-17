using System;
using System.Collections.Generic;

namespace INTERBANK.Model;

public partial class Reclamo
{
    public int IdReclamo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public string Descripcion { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public string? Respuesta { get; set; }

    public int IdUsuario { get; set; }

    public string TipoReclamo { get; set; } = null!;

    public DateTime? FechaRespuesta { get; set; }

    public virtual ICollection<HistorialReclamo> HistorialReclamos { get; set; } = new List<HistorialReclamo>();

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
