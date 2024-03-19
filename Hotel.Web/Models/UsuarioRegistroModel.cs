using System.ComponentModel.DataAnnotations;

namespace Hotel.Web.Models
{
    public class UsuarioRegistroModel
    {
        [DataType(DataType.EmailAddress, ErrorMessage = "Formato de correo electronico incorrecto")]
        public string Email { get; set; }

        public string Clave { get; set; }
    }
}
