using System.ComponentModel.DataAnnotations;

namespace Hotel.Web.Models
{
    public class UsuarioModel
    {
        public int Id { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessage = "Formato de correo electronico incorrecto")]
        //[RegularExpression(@"/[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?/g")]
        public string Email { get; set; }

        public string Clave { get; set; }

        public string Token { get; set; } = "";
    }
}
