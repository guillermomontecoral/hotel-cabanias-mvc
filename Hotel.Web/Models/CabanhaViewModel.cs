
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Web.Models
{
    public class CabanhaViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un tipo de cabaña")]
        public int IdTipoCabanha { get; set; }
        //public TipoCabanha TipoCabanha { get; set; }

        [Required(ErrorMessage = "La descripción es requerido")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Es requerido")]
        public bool Jacuzzi { get; set; }

        [Required(ErrorMessage = "Es requerido")]
        public bool HabilitadaParaReservas { get; set; }

        [Required(ErrorMessage = "Es requerido el número de habitación")]
        public int NumHabitacion { get; set; }

        [Required(ErrorMessage = "La cantidad de personas es requerido")]
        public int CantMaxPersonas { get; set; }

        [Required(ErrorMessage = "La foto es requerida")]
        public string NombreFoto { get; set; }
        //public int Sec { get; set; } = 001;


        //public Cabanha MandarDatosCabanha()
        //{
        //    Cabanha c = new Cabanha()
        //    {
        //        //Lado izquierdo propeties de Cabanha
        //        //Lado derecho properties de CabanhaModel
        //        CantMaxPersonas = CantMaxPersonas,
        //        Descripcion = Descripcion,
        //        HabilitadaParaReservas = HabilitadaParaReservas,
        //        IdTipoCabanha = IdTipoCabanha,
        //        Jacuzzi = Jacuzzi,
        //        Nombre = Nombre,
        //        NumHabitacion = NumHabitacion,
        //        MisFotos = new List<Foto>()
        //            {
        //                new Foto()
        //                {
        //                    NombreFoto = NombreFoto,
        //                    Secuenciador = Sec

        //                }
        //            }
        //    };

        //    return c;
        //}
    }
}
