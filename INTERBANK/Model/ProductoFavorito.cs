using System;
using System.Collections.Generic;

namespace INTERBANK.Model;

public partial class ProductoFavorito
{
    public int IdFavorito { get; set; }

    public int IdUsuario { get; set; }

    public int IdProducto { get; set; }

    public DateTime FechaAgregado { get; set; }

    public virtual ProductoFinanciero IdProductoNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
