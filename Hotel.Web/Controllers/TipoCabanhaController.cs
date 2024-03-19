using Azure;
using Hotel.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Hotel.Web.Controllers
{
    public class TipoCabanhaController : Controller
    {
        private static HttpClient _cli = new() { BaseAddress = new Uri("https://localhost:7294/api") };

        private Uri _uriTipoCabanha = new Uri($"{_cli.BaseAddress}/TipoCabanha");

        JsonSerializerOptions opciones = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        public TipoCabanhaController()
        {
            _cli.DefaultRequestHeaders.Accept.Clear();
            _cli.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }



        // GET: TipoCabanhaController
        public IActionResult Index(string buscarNombre, bool? reset)
        {
            //if (HttpContext.Session.GetString("email") == null)
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            try
            {

                string? json = GetRespuesta(_uriTipoCabanha);

                if (json == null)
                {
                    ViewBag.ListaVacia = $"La lista de tipos de cabañas se encuentra vacía.";
                    return View();
                }

                var listaTipoCabanhas = JsonSerializer.Deserialize<IEnumerable<TipoCabanhaModel>>(json, opciones);

                if (!string.IsNullOrEmpty(buscarNombre))
                {
                    var buscado = GetTipoCabanhaByName(buscarNombre);

                    if (buscado == null)
                    {
                        ViewBag.ListaVacia = $"No existe registrado un tipo de cabaña con el nombre {buscarNombre}";
                        return View();
                    }

                    var b = new[] { buscado };

                    return View(b);
                }

                if (reset != null)
                    return View(listaTipoCabanhas);

                return View(listaTipoCabanhas);

            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();

            }
        }

        // GET: TipoCabanhaController/Details/5
        public IActionResult Details(int? id)
        {
            //if (HttpContext.Session.GetString("email") == null)
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            if (id == null)
            {

                ViewBag.Error = "El id del tipo de cabaña no puede ser nulo.";
                return View();
            }

            var tipoCabanha = GetTipoCabanhaById(id.Value);
            if (tipoCabanha == null)
            {
                ViewBag.Error = $"No se puede obtener el tipo de cabaña con el id {id}";
                return RedirectToAction("Index");
            }

            return View(tipoCabanha);
        }


        // GET: TipoCabanhaController/Create
        public IActionResult Create()
        {
            //if (HttpContext.Session.GetString("email") == null)
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            return View();
        }


        // POST: TipoCabanhaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TipoCabanhaModel nuevoTipo)
        {
            //if (HttpContext.Session.GetString("email") == null)
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            try
            {
                if (nuevoTipo == null)
                {
                    ViewBag.Error = "El tipo de cabaña es nulo, no se puede dar de alta un tipo de cabaña nulo";
                    return View();
                }
                if (!ModelState.IsValid)
                {
                    return View(nuevoTipo);
                }

                var tipoCabanhaSerializado = JsonSerializer.Serialize(nuevoTipo);
                var body = new StringContent(tipoCabanhaSerializado, Encoding.UTF8, "application/json");
                var respuesta = _cli.PostAsync(_uriTipoCabanha, body).Result;

                if (respuesta.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }

                if (((int)respuesta.StatusCode) == StatusCodes.Status400BadRequest)
                {
                    ViewBag.Error = $"No fue posible crear el tipo de cabaña con el nombre '{nuevoTipo.Nombre}'. Ya existe una registrada con ese nombre.";
                    return View(nuevoTipo);
                }

                //if (respuesta.ReasonPhrase == "Conflict")
                //{
                //    ViewBag.Error = $"No fue posible crear el tipo de cabaña con el nombre '{nuevoTipo.Nombre}'. Ya existe una registrada con ese nombre.";
                //    return View(nuevoTipo);
                //}
                return View(nuevoTipo);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(nuevoTipo);
            }
        }


        // GET: TipoCabanhaController/Edit/5
        public IActionResult Edit(int? id)
        {
            //if (HttpContext.Session.GetString("email") == null)
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            try
            {
                if (id == null)
                {
                    ViewBag.Error = "El id del tipo de cabaña no puede ser nulo.";
                    return View();
                }

                var tipoCabanha = GetTipoCabanhaById(id.Value);
                if (tipoCabanha == null)
                {
                    ViewBag.Error = $"No existe un tipo de cabaña con el id {id}";
                    return RedirectToAction("Index");
                }

                return View(tipoCabanha);

            }
            catch (Exception ex)
            {
                ViewBag.Error = $"No se puede obtener el tipo de cabaña con el id {id}. Error {ex.Message}";
                return View();
            }


        }

        // POST: TipoCabanhaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int? id, TipoCabanhaModel tipoCabanha)
        {
            //if (HttpContext.Session.GetString("email") == null)
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            try
            {
                if (!ModelState.IsValid)
                {
                    return View(tipoCabanha);
                }
                if (id == null)
                {
                    ViewBag.Error = "El id del tipo de cabaña no puede ser nulo.";
                    return View();
                }

                var autorSerializado = JsonSerializer.Serialize(tipoCabanha, opciones);
                var json = new StringContent(autorSerializado, Encoding.UTF8, "application/json");
                var respuesta = _cli.PutAsync(_uriTipoCabanha + "/" + id, json);



                if (respuesta.Result.IsSuccessStatusCode)
                    return RedirectToAction("Index");

                return View(tipoCabanha);

            }
            catch (Exception ex)
            {
                ViewBag.Error = $"No se puede editar el tipo de cabaña con el {id}. Error {ex.Message}";
                return View(tipoCabanha);
            }
        }

        // GET: TipoCabanhaController/Delete/5
        public IActionResult Delete(int? id)
        {
            //if (HttpContext.Session.GetString("email") == null)
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            try
            {
                if (id == null)
                {
                    ViewBag.Error = "El id del tipo de cabaña no puede ser nulo.";
                    return View();
                }

                var tipoCabanha = GetTipoCabanhaById(id.Value);
                if (tipoCabanha == null)
                {
                    ViewBag.Error = $"No existe un tipo de cabaña con el id {id}";
                    return RedirectToAction("Index");
                }

                return View(tipoCabanha);

            }
            catch (Exception ex)
            {
                ViewBag.Error = $"No se puede obtener el tipo de cabaña con el id {id}. Error {ex.Message}";
                return View();
            }
        }

        // POST: TipoCabanhaController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int? id, TipoCabanhaModel tipoCabanha)
        {
            //if (HttpContext.Session.GetString("email") == null)
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            try
            {
                if (tipoCabanha == null || id == null)
                {
                    ViewBag.Error = "Debe indicar el id y el tipo de cabaña a eliminar";
                    return RedirectToAction("Index");
                }

                var respuesta = _cli.DeleteAsync($"{_uriTipoCabanha}/{id.Value}");
                if (((int)respuesta.Result.StatusCode) == StatusCodes.Status204NoContent)
                    return RedirectToAction(nameof(Index));

                ViewBag.Error = $"No fue posible eliminar el tipo de cabaña con id: {id.Value}. Se encuentra asociado con alguna cabaña.";
                return View(tipoCabanha);
            }
            catch (DbUpdateException e)
            {
                ViewBag.Error = $"Error al eliminar el tipo de cabaña {tipoCabanha.Nombre}. Este error ocurrio ya que se encuentra asociado con alguna cabaña.";
                return View(tipoCabanha);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error inesperado. {ex.Message}";
                return View(tipoCabanha);
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

            //if (response.StatusCode.Equals(400))
            //{
            //    throw new Exception("ERRRRROR");
            //}

            var json = response.Content.ReadAsStringAsync().Result;
            return json;
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


        /// <summary>
        /// Método auxiliar para obtener un tipo de cabaña a partir del consumo de la Api. 
        /// </summary>
        /// <param name="nombre">El nombre de un tipo de cabaña a buscar</param>
        /// <returns>El objeto obtenido a partir del json incluido en el body (Content) de la respuesta</returns>
        private TipoCabanhaModel GetTipoCabanhaByName(string nombre)
        {
            try
            {
                var json = GetRespuesta(new Uri(_uriTipoCabanha + "/nombre/" + nombre));
                if (json == null)
                    return null;

                var tipoCabanha = JsonSerializer.Deserialize<TipoCabanhaModel>(json, opciones);

                if (tipoCabanha == null)
                    throw new ArgumentNullException("Devolvio null el tipo de cabaña.");

                return tipoCabanha;
            }
            catch (Exception ex)
            {
                throw;
            }


        }

        #endregion
    }
}
