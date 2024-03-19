using Hotel.Web.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Azure;

namespace Hotel.Web.Controllers
{
    public class CabanhaController : Controller
    {

        //Foto
        private readonly ILogger<CabanhaController> _logger;
        private IWebHostEnvironment _environment;

        private static HttpClient _cli = new() { BaseAddress = new Uri("https://localhost:7294/api") };

        private Uri _uriCabanha = new Uri($"{_cli.BaseAddress}/Cabanha");
        private Uri _uriTipoCabanha = new Uri($"{_cli.BaseAddress}/TipoCabanha");

        JsonSerializerOptions opciones = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        public CabanhaController(ILogger<CabanhaController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;

            _cli.DefaultRequestHeaders.Accept.Clear();
            _cli.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // GET: CabanhaController
        public IActionResult Index(string buscarTexto, int? tipoCabanha, int? numMaxPersonas, bool cabHabilitada, bool? reset)
        {
            if (HttpContext.Session.GetString("token") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                
                string? jsonTipoCabanha = GetRespuesta(_uriTipoCabanha);
                if (jsonTipoCabanha == null)
                {
                    ViewBag.TipoCabanhas = "No existen tipos de cabañas";
                    return View();
                }
                var listaTiposCabanhas = JsonSerializer.Deserialize<IEnumerable<TipoCabanhaModel>>(jsonTipoCabanha, opciones);
                ViewBag.TipoCabanhas = listaTiposCabanhas;

                string? json = GetRespuesta(_uriCabanha);

                if (json == null)
                {
                    ViewBag.ListaVacia = $"La lista de cabañas se encuentra vacía.";
                    return View();
                }

             
                var listaCabanhas = JsonSerializer.Deserialize<IEnumerable<CabanhaViewModel>>(json, opciones);

                if (!string.IsNullOrEmpty(buscarTexto))
                {
                    var buscado = GetCabanhaByName(buscarTexto);
            
                    return View(buscado);
                }

                if (tipoCabanha != null)
                {
                    var buscado = GetTipoCabanhas(tipoCabanha.Value);

                    return View(buscado);
                }

                if (numMaxPersonas != null)
                {
                    var buscado = GetCabanhaPorCantHuespedes(numMaxPersonas);

                    return View(buscado);
                }

                if (cabHabilitada)
                {
                    var buscado = GetCabanhaHabilitadas();

                    return View(buscado);
                }

                if (reset != null)
                    return View(listaCabanhas);

                return View(listaCabanhas);
           
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();

            }
        }

        // GET: CabanhaController/Details/5
        public IActionResult Details(int? id)
        {
            if (HttpContext.Session.GetString("token") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {

                if (id == null)
                {
                    ViewBag.Error = $"Debe de ingresar un id.";
                    return View();
                }

                var cabanha = GetCabanhaById(id.Value);
                if (cabanha == null)
                {
                    ViewBag.Error = $"No hay una cabaña con el id {id}";
                    return RedirectToAction("Index");
                }

                var tipoCabanha = GetTipoCabanhaById(cabanha.IdTipoCabanha);
                ViewBag.TipoCabanha = tipoCabanha;

                return View(cabanha);
            }
            catch
            {
                ViewBag.Error = $"No se puede obtener la cabaña con el id {id}";
                return View();
            }
        }


        // GET: CabanhaController/Create
        public ActionResult Create()
        {
            if (HttpContext.Session.GetString("token") == null)
            {
                return RedirectToAction("Index", "Home");
            }


            string? jsonTipoCabanha = GetRespuesta(_uriTipoCabanha);
            if (jsonTipoCabanha == null)
            {
                ViewBag.TipoCabanhas = "No existen tipos de cabañas";
                return View();
            }
            var listaTiposCabanhas = JsonSerializer.Deserialize<IEnumerable<TipoCabanhaModel>>(jsonTipoCabanha, opciones);

            
            if (!listaTiposCabanhas.Any())
            {
                TempData["TiposCabanhaVacios"] = "No hay tipos cabañas, por eso ha sido redirigido a esta vista para poder agregar un tipo de cabaña.";
                return RedirectToAction("Create", "TipoCabanha");
            }
            
            ViewBag.TipoCabanhas = listaTiposCabanhas;

            return View();
        }

        // POST: CabanhaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CabanhaViewModel nuevaCabanha, IFormFile imagen)
        {
            if (HttpContext.Session.GetString("token") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                if (nuevaCabanha == null)
                {
                    return BadRequest("La cabaña es nulo, no se puede dar de alta una cabaña nulo.");
                }

                string? jsonTipoCabanha = GetRespuesta(_uriTipoCabanha);
                if (jsonTipoCabanha == null)
                {
                    ViewBag.TipoCabanhas = "No existen tipos de cabañas registrados.";
                    return View();
                }
                var listaTiposCabanhas = JsonSerializer.Deserialize<IEnumerable<TipoCabanhaModel>>(jsonTipoCabanha, opciones);
                ViewBag.TipoCabanhas = listaTiposCabanhas;

                GuardarImagen(imagen, nuevaCabanha);

                //Consumir la api para crear un tipo de cabaña
                var tipoCabanhaSerializado = JsonSerializer.Serialize(nuevaCabanha);
                //Crear el body
                var body = new StringContent(tipoCabanhaSerializado, Encoding.UTF8, "application/json");
                var respuesta = _cli.PostAsync(_uriCabanha, body).Result;
                if (respuesta.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                if (respuesta.ReasonPhrase == "Conflict")
                {
                    ViewBag.Error = $"No fue posible dar de alta la cabaña con el nombre '{nuevaCabanha.Nombre}'. Ya existe una registrada con ese nombre.";
                    return View(nuevaCabanha);
                }
                return View(nuevaCabanha);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException.InnerException is SqlException)
                {
                    SqlException sql = (SqlException)ex.InnerException.InnerException;
                    if (sql.Number == 2627)
                    {
                        ViewBag.Error = ex.Message;
                        return View();
                    }
                }
                ViewBag.Error = ex.Message;
                return View();

            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }


        private void GuardarImagen(IFormFile imgCabanha, CabanhaViewModel nuevaCabanha)
        {
            if (imgCabanha == null)
                throw new Exception("Debe seleccionar una imagen.");

            if (nuevaCabanha == null)
                throw new Exception("Los datos de la cabaña no pueden estar vacios.");

            // SUBIR LA IMAGEN
            //ruta fisica de wwwroot
            string rutaFisicaWwwRoot = _environment.WebRootPath;
            string nombreImagen;
            if (Path.GetExtension(imgCabanha.FileName) == ".png"
                || Path.GetExtension(imgCabanha.FileName) == ".jpg"
                || Path.GetExtension(imgCabanha.FileName) == ".jpeg")
            {

                nombreImagen = nuevaCabanha.Nombre.Replace(" ", "_") + "_001" + Path.GetExtension(imgCabanha.FileName);
            }
            else
            {
                throw new Exception("Error de formato, el formato de imagen debe ser png, jpg o jpeg.");
            }
            string rutaFisicaFoto = Path.Combine(rutaFisicaWwwRoot, "img", "fotos", nombreImagen);

            //FileStream permite manejar archivos
            try
            {
                //el método using libera los recursos del objeto FileStream al finalizar 
                using (FileStream f = new FileStream(rutaFisicaFoto, FileMode.Create))
                {
                    //si fueran archivos grandes o si fueran varios, deberíamos usar la versión
                    //asincrónica de CopyTo, aquí no es necesario.
                    //sería: await foto.CopyToAsync (f);
                    imgCabanha.CopyTo(f);
                }
                //GUARDAR EL NOMBRE DE LA IMAGEN SUBIDA EN EL OBJETO
                nuevaCabanha.NombreFoto = nombreImagen.ToLower();
                //return true;
            }
            catch (Exception ex)
            {
                throw;
            }

        }


        // GET: CabanhaController/Edit/5
        public IActionResult Edit(int? id)
        {
            if (HttpContext.Session.GetString("token") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                if (id == null)
                {
                    ViewBag.Error = "El id de la cabaña no puede ser nulo.";
                    return View();
                }

                string? jsonTipoCabanha = GetRespuesta(_uriTipoCabanha);
                if (jsonTipoCabanha == null)
                {
                    ViewBag.TipoCabanhas = "No existen tipos de cabañas registrados.";
                    return View();
                }
                var listaTiposCabanhas = JsonSerializer.Deserialize<IEnumerable<TipoCabanhaModel>>(jsonTipoCabanha, opciones);
                ViewBag.TipoCabanhas = listaTiposCabanhas;

                var cabanha = GetCabanhaById(id.Value);
                if (cabanha == null)
                {
                    ViewBag.Error = $"No existe una cabaña con el id {id}";
                    return RedirectToAction("Index");
                }

                return View(cabanha);

            }
            catch (Exception ex)
            {
                ViewBag.Error = $"No se puede obtener la cabaña con el id {id}. Error {ex.Message}";
                return View();
            }
        }

        // POST: CabanhaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int? id, CabanhaViewModel unaCabanha)
        {
            if (HttpContext.Session.GetString("token") == null)
            {
                return RedirectToAction("Index", "Home");
            }


            try
            {

                if (id == null)
                {
                    ViewBag.Error = "El id del tipo de cabaña no puede ser nulo.";
                    return View();
                }

                if (unaCabanha == null)
                {
                    ViewBag.Error = "Los datos de la cabaña son nulos.";
                    return View();
                }

                //Busco los tipos de cabaña para poder usarlos en las vistas
                string? jsonTipoCabanha = GetRespuesta(_uriTipoCabanha);
                if (jsonTipoCabanha == null)
                {
                    ViewBag.TipoCabanhas = "No existen tipos de cabañas registrados.";
                    return View();
                }
                var listaTiposCabanhas = JsonSerializer.Deserialize<IEnumerable<TipoCabanhaModel>>(jsonTipoCabanha, opciones);
                ViewBag.TipoCabanhas = listaTiposCabanhas;

                var cabanhaSerializado = JsonSerializer.Serialize(unaCabanha, opciones);
                var json = new StringContent(cabanhaSerializado, Encoding.UTF8, "application/json");
                var respuesta = _cli.PutAsync(_uriCabanha + "/" + id, json);
                if (respuesta.Result.IsSuccessStatusCode)
                    return RedirectToAction("Index");

                return View(unaCabanha);

            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error en la cabaña con el id {id}. {ex.Message}";
                return View();
            }
        }

        // GET: CabanhaController/Delete/5
        public ActionResult Delete(int? id)
        {
            if (HttpContext.Session.GetString("token") == null)
            {
                return RedirectToAction("Index", "Home");
            }


            try
            {
                if (id == null)
                {
                    ViewBag.Error = "El id del tipo de cabaña no puede ser nulo.";
                    return View();
                }

                var cabanha = GetCabanhaById(id.Value);
                if (cabanha == null)
                {
                    ViewBag.Error = $"No existe una cabaña con el id {id}";
                    return RedirectToAction("Index");
                }

                var tipoCabanha = GetTipoCabanhaById(cabanha.IdTipoCabanha);
                ViewBag.TipoCabanha = tipoCabanha;

                return View(cabanha);

            }
            catch (Exception ex)
            {
                ViewBag.Error = $"No se puede obtener la cabaña con el id {id}. Error {ex.Message}";
                return View();
            }
        }

        // POST: CabanhaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id, CabanhaViewModel cabanha)
        {
            if (HttpContext.Session.GetString("token") == null)
            {
                return RedirectToAction("Index", "Home");
            }


            try
            {
                if (cabanha == null || id == null)
                {
                    ViewBag.Error = "Debe indicar el id y la cabaña a eliminar";
                    return RedirectToAction("Index");
                }

                
                var respuesta = _cli.DeleteAsync($"{_uriCabanha}/{id.Value}");
                if (((int)respuesta.Result.StatusCode) == StatusCodes.Status204NoContent)
                    return RedirectToAction(nameof(Index));

                ViewBag.Error = $"No fue posible eliminar la cabaña con id: {id.Value}";
                return View(cabanha);
            }
            catch (DbUpdateException e)
            {
                ViewBag.Error = $"Error al eliminar el tipo de cabaña {cabanha.Nombre}. Este error ocurrio ya que se encuentra asociado con alguna cabaña.";
                return View(cabanha);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error inesperado. {ex.Message}";
                return View(cabanha);
            }
        }

        #region Métodos auxiliares
        /// <summary>
        /// Método auxiliar para retornar el json obtenido a partir de la api.
        /// </summary>
        /// <param name="uri">Url donde está ubicado el recurso de la API a consumir</param>
        /// <returns>Un string con el contenido del body obtenido en la respuesta; habitualmente contendrá la info de un objeto o de una lista de objetos y se formateará como Json</returns>
        private string? GetRespuesta(Uri uri)
        {
            var response = _cli.GetAsync(uri).Result;
            response.EnsureSuccessStatusCode();

            if (response.ReasonPhrase == "Not Found" || response.ReasonPhrase == "Conflict")
            {
                return response.ReasonPhrase;
            }

            var json = response.Content.ReadAsStringAsync().Result;
            return json;
        }

        /// <summary>
        /// Método auxiliar para obtener una cabaña a partir del consumo de la Api. 
        /// </summary>
        /// <param name="id">El id de la cabaña a buscar</param>
        /// <returns>El objeto obtenido a partir del json incluido en el body (Content) de la respuesta</returns>
        /// <remarks>Dado que este método se utiliza en varias de las acciones, se separó para no repetir código </remarks>
        private CabanhaViewModel GetCabanhaById(int id)
        {
            try
            {
                var json = GetRespuesta(new Uri(_uriCabanha + "/" + id));
                if (json == null)
                    return null;

                var cabanha = JsonSerializer.Deserialize<CabanhaViewModel>(json, opciones);

                if (cabanha == null)
                    throw new ArgumentNullException("Devolvio null el tipo de cabaña.");

                return cabanha;
            }
            catch (Exception)
            {
                throw;
            }


        }

        /// <summary>
        /// Método auxiliar para obtener un tipo de cabaña a partir del consumo de la Api. 
        /// </summary>
        /// <param name="id">El id de un tipo de cabaña a buscar</param>
        /// <returns>El objeto obtenido a partir del json incluido en el body (Content) de la respuesta</returns>
        /// <remarks>Dado que este método se utiliza en varias de las acciones, se separó para no repetir código </remarks>
        private TipoCabanhaModel GetTipoCabanhaById(int id)
        {
            try
            {
                var json = GetRespuesta(new Uri(_uriTipoCabanha + "/" + id));
                if (json == null)
                    return null;

                var tipoCabanha = JsonSerializer.Deserialize<TipoCabanhaModel>(json, opciones);

                if (tipoCabanha == null)
                    throw new ArgumentNullException("Devolvio null el tipo de cabaña.");

                return tipoCabanha;
            }
            catch (Exception)
            {
                throw;
            }


        }

        private IEnumerable<CabanhaViewModel> GetTipoCabanhas(int id)
        {
            try
            {
                var json = GetRespuesta(new Uri(_uriCabanha + "/tipoCabanha/" + id));
                if (json == null)
                    return null;

                var tipoCabanhas = JsonSerializer.Deserialize<IEnumerable<CabanhaViewModel>>(json, opciones);

                if (tipoCabanhas == null)
                    throw new ArgumentNullException("Devolvio null el tipo de cabaña.");

                return tipoCabanhas;
            }
            catch (Exception)
            {
                throw;
            }


        }


        /// <summary>
        /// Método auxiliar para obtener una cabaña a partir del consumo de la Api. 
        /// </summary>
        /// <param name="nombre">El nombre de la cabaña a buscar</param>
        /// <returns>El objeto obtenido a partir del json incluido en el body (Content) de la respuesta</returns>
        private IEnumerable<CabanhaViewModel> GetCabanhaByName(string nombre)
        {
            try
            {
                var json = GetRespuesta(new Uri(_uriCabanha + "/nombre/" + nombre));
                if (json == null)
                    return null;

                var cabanhas = JsonSerializer.Deserialize<IEnumerable<CabanhaViewModel>>(json, opciones);

                if (cabanhas.Count() == 0)
                    throw new ArgumentNullException("No hay cabañas con ese nombre.");

                return cabanhas;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private IEnumerable<CabanhaViewModel> GetCabanhaPorCantHuespedes(int? cantHuespedes)
        {
            try
            {
                var json = GetRespuesta(new Uri(_uriCabanha + "/cantidadHuespedes/" + cantHuespedes));
                if (json == null)
                    return null;

                var cabanhas = JsonSerializer.Deserialize<IEnumerable<CabanhaViewModel>>(json, opciones);

                if (cabanhas.Count() == 0)
                    throw new ArgumentNullException("No hay cabañas con esa cantidad de huespedes.");

                return cabanhas;
            }
            catch (Exception ex)
            {
                throw;
            }


        }

        private IEnumerable<CabanhaViewModel> GetCabanhaHabilitadas()
        {
            try
            {
                var json = GetRespuesta(new Uri(_uriCabanha + "/habilitadas/"));
                if (json == null)
                    return null;

                var cabanhas = JsonSerializer.Deserialize<IEnumerable<CabanhaViewModel>>(json, opciones);

                if (cabanhas.Count() == 0)
                    throw new ArgumentNullException("No hay cabañas habilitadas.");

                return cabanhas;
            }
            catch (Exception ex)
            {
                throw;
            }


        }
        
        #endregion

    }
}
