using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Web.Models
{
    public class TipoCabanhaModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        public string Descripcion { get; set; }


        //Configuro el tipo de dato decimal para que en la BD quede como corresponde
        [Column(TypeName = "decimal(10,2)")]
        [Required(ErrorMessage = "El costo por huesped es requerido")]
        public decimal? CostoPorHuesped { get; set; }
    }
}
