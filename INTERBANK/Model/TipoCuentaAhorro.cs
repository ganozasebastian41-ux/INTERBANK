using System;
using System.Collections.Generic;

namespace INTERBANK.Model;

public partial class TipoCuentaAhorro
{
    public int IdTipoCuenta { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal TasaInteres { get; set; }

    public string Moneda { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public virtual ICollection<RequisitoCuentaAhorro> RequisitoCuentaAhorros { get; set; } = new List<RequisitoCuentaAhorro>();
}
