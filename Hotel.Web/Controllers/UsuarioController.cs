
using Azure;
using Hotel.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Hotel.Web.Controllers
{
    public class UsuarioController : Controller
    {
        private static HttpClient _cli = new() { BaseAddress = new Uri("https://localhost:7294/api") };

        private Uri _uriUsuario = new Uri($"{_cli.BaseAddress}/Usuario");

        JsonSerializerOptions opciones = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        public UsuarioController()
        {
            _cli.DefaultRequestHeaders.Accept.Clear();
            _cli.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /*
        // GET: UsuarioController
        public ActionResult Index()
        {
            try
            {
                PrecargarUsuarios();
                return RedirectToAction("Index", "Home");
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorUsuarios"] = $"Se ha producido un error. {ex.InnerException.Message} Verifique la base de datos.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                //ViewBag.Error = ex.Message;
                TempData["ErrorUsuarios"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }

        }

        // GET: UsuarioController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        */

        // GET: UsuarioController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UsuarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UsuarioModel unUsuario)
        {
            try
            {
                if(unUsuario == null)
                {
                    ViewBag.Error = "Los datos no pueden ser nulos";
                    return View();
                }

                
                var usuSerializado = JsonSerializer.Serialize(unUsuario);
                var body = new StringContent(usuSerializado, Encoding.UTF8, "application/json");
                var respuesta = _cli.PostAsync(_uriUsuario, body).Result;
                if (respuesta.IsSuccessStatusCode)
                {
                    var jsonRespuesta = respuesta.Content.ReadAsStringAsync().Result;
                    var usrEncontrado = JsonSerializer.Deserialize<UsuarioModel>(jsonRespuesta, opciones);
                    if (usrEncontrado == null || string.IsNullOrEmpty(usrEncontrado.Token))
                        return View();
                    else
                    {
                        _cli.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", usrEncontrado.Token);
                        HttpContext.Session.SetString("token", usrEncontrado.Token);

                        return RedirectToAction("Index", "Home");
                    }
                }
                ViewBag.Error = $"No fue posible registrarse.";
                return View(unUsuario);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        private string? GetRespuesta(Uri uri)
        {
            var response = _cli.GetAsync(uri).Result;

            response.EnsureSuccessStatusCode();

            var json = response.Content.ReadAsStringAsync().Result;
            return json;
        }

    }
}
