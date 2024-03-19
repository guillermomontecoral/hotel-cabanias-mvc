
using Hotel.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Hotel.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private static HttpClient _cli = new() { BaseAddress = new Uri("https://localhost:7294/api") };

        private Uri _uriTipoCabanha = new Uri($"{_cli.BaseAddress}/TipoCabanha");
        private Uri _uriCabanha = new Uri($"{_cli.BaseAddress}/Cabanha");
        private Uri _uriMantenimiento = new Uri($"{_cli.BaseAddress}/Mantenimiento");

        JsonSerializerOptions opciones = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true, 
            WriteIndented = true 
        };
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _cli.DefaultRequestHeaders.Accept.Clear();
            _cli.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public ActionResult Index()
        {
            string? nombrelog = HttpContext.Session.GetString("email");
            ViewBag.Email = $"{nombrelog}";
            return View();
        }
        
        [HttpPost]
        public ActionResult Index(UsuarioModel usuario)
        {
            //if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(clave))
            //{
            //    ViewBag.LlenarCampos = "Debe completar los campos para poder iniciar sesión";
            //    return View();                
            //}

            

            try
            {
                usuario.Token = "Sin token";
                var usuarioSerializado = JsonSerializer.Serialize(usuario, opciones);
                var json = new StringContent(usuarioSerializado, Encoding.UTF8, "application/json");

                var response = _cli.PostAsync(_cli.BaseAddress + "/usuario/login", json).Result;
                if (response.IsSuccessStatusCode)
                { //Espera hasta obtener la respuesta; si no lo logra lanza una excepción

                    //Leer el json que viene incluido en el contenido (body) 
                    var jsonRespuesta = response.Content.ReadAsStringAsync().Result;
                    var usrEncontrado = JsonSerializer.Deserialize<UsuarioModel>(jsonRespuesta, opciones);
                    if (usrEncontrado == null || string.IsNullOrEmpty(usrEncontrado.Token))
                        return View();
                    else
                    {
                        _cli.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", usrEncontrado.Token);
                        HttpContext.Session.SetString("token", usrEncontrado.Token);
                        return RedirectToAction("Index");
                    }
                }

                //return View(usuario);

                //_repoUsuarios.ValidarLogin(email, clave);
               return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        public IActionResult Logout()
        {
            if (_cli != null)
                _cli.DefaultRequestHeaders.Remove("Authorization");
            HttpContext.Session.Clear();
            return RedirectToAction("Index");

        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }
}