
using Hotel.Web.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Hotel.Web.Controllers
{
    public class MantenimientoController : Controller
    {

        private static HttpClient _cli = new() { BaseAddress = new Uri("https://localhost:7294/api") };

        private Uri _uriMantenimiento = new Uri($"{_cli.BaseAddress}/Mantenimiento");
        private Uri _uriCabanha = new Uri($"{_cli.BaseAddress}/Cabanha");
        private Uri _uriTipoCabanha = new Uri($"{_cli.BaseAddress}/TipoCabanha");

        JsonSerializerOptions opciones = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        public MantenimientoController()
        {
            _cli.DefaultRequestHeaders.Accept.Clear();
            _cli.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }



        // GET: MantenimientoController
        public IActionResult Index(int idCabanha, DateTime? fecha1, DateTime? fecha2, bool? reset)
        {
            //if (HttpContext.Session.GetString("email") == null)
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            try
            {
                TempData["idCabanha"] = idCabanha;

                if (fecha1 != null && fecha2 != null)
                {
                    var m = GetMantCabanhaEntreFechas(fecha1, fecha2, idCabanha);
                    if (!m.Any())
                    {
                        ViewBag.ListaVacia = $"No hay mantenimientos entre el {fecha1:dd/MM/yyyy} y el {fecha2:dd/MM/yyyy}";
                        return View();
                    }
                    return View(m);
                }

                var mant = GetMantCabanha(idCabanha);
                if (reset != null)
                { 
                    return View(mant);
                }

                return View(mant);

            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();

            }

        }

        /*
        // GET: MantenimientoController/Details/5
        public ActionResult Details(int id)
        {
            if (HttpContext.Session.GetString("email") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
        */

        // GET: MantenimientoController/Create
        public ActionResult Create(int cabanhaId)
        {
            //if (HttpContext.Session.GetString("email") == null)
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            //Busco los tipos de cabaña para poder usarlos en las vistas
            string? jsonTipoCabanha = GetRespuesta(_uriTipoCabanha);
            if (jsonTipoCabanha == null)
            {
                ViewBag.TipoCabanhas = "No existen tipos de cabañas";
                return View();
            }
            var listaTiposCabanhas = JsonSerializer.Deserialize<IEnumerable<TipoCabanhaModel>>(jsonTipoCabanha, opciones);

            //var q = _repoTipoCabanha.FindAll();
            if (!listaTiposCabanhas.Any())
            {
                TempData["TiposCabanhaVacios"] = "No hay tipos cabañas, por eso ha sido redirigido a esta vista para poder agregar un tipo de cabaña.";
                return RedirectToAction("Create", "TipoCabanha");
            }

            ViewBag.TipoCabanhas = listaTiposCabanhas;

            var tipoCabanha = GetCabanhaById(cabanhaId);
            ViewBag.Cabanha = tipoCabanha;

            return View();
        }


        // POST: MantenimientoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MantenimientoModel m)
        {
            //if (HttpContext.Session.GetString("email") == null)
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            try
            {
                if (m == null)
                {
                    return BadRequest("El objeto mantnimiento no puede ser nulo");
                }

                
                var mantenimientoSerializado = JsonSerializer.Serialize(m);
                //Crear el body
                var body = new StringContent(mantenimientoSerializado, Encoding.UTF8, "application/json");
                var respuesta = _cli.PostAsync(_uriMantenimiento, body).Result;
                if (respuesta.IsSuccessStatusCode)
                {                    
                    return RedirectToAction("Index", new { idCabanha = m.IdCabanha });
                }
                ViewBag.Error = $"No fue crear el mantenimiento. Error {respuesta.StatusCode} {respuesta.ReasonPhrase}";
                return View(m);



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

        /*
        // GET: MantenimientoController/Edit/5
        public ActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("email") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var editarM = _repoMantenimeinto.FindById(id);

            if (editarM != null)
            {
                
                ViewData["Fecha"] = editarM.Fecha;

                return View(editarM);
            }


            ViewBag.Error = $"No se puede obtener el mantenimiento con el id {id}";
            return View();
        }

        // POST: MantenimientoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Mantenimiento m)
        {
            if (HttpContext.Session.GetString("email") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                if (m != null)
                {
                    var valores = _repoTopeDescripcion.FindByNameObject(m.GetType().Name);
                    if (valores == null)
                    {
                        ViewBag.ErrorTope = $"No hay topes de caracteres registrados, por favor ingrese topes de caracteres y luego puede dar de alta una cabaña.";
                        return View(m);
                    }
                    if (m.Descripcion.Trim().Length < valores.TopeMin || m.Descripcion.Trim().Length > valores.TopeMax)
                    {
                        ViewBag.ErrorTope = $"La descripción debe de contener entre 10 y 200 caracteres. Usted escribio {m.Descripcion.Length} caracteres.";
                        return View(m);
                    }

                    _repoMantenimeinto.Update(m);
                    return RedirectToAction("Index", new { id = m.IdCabanha });
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error en el mantenimiento con el id {id}. {ex.Message}";
                return View();
            }
        }

        // GET: MantenimientoController/Delete/5
        public ActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("email") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            Mantenimiento m = _repoMantenimeinto.FindById(id);
            if (m != null)
                return View(m);

            ViewBag.Error = $"No se puede obtener el mantenimiento con el id {id}";
            return View();
        }

        // POST: MantenimientoController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Mantenimiento m)
        {
            if (HttpContext.Session.GetString("email") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var mantenimiento = _repoMantenimeinto.FindById(id);

            try
            {
                if (mantenimiento == null)
                    return BadRequest("El id y el tipo cabaña no deben ser nulos");

                _repoMantenimeinto.Delete(id);
                return RedirectToAction("Index", new { id = mantenimiento.IdCabanha });
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error id {id}. {ex.Message}";
                return View(mantenimiento);
            }
        }
        */

        #region Métodos auxiliares
        /// <summary>
        /// Método auxiliar para retornar el json obtenido a partir de la api.
        /// </summary>
        /// <param name="uri">Url donde está ubicado el recurso de la API a consumir</param>
        /// <returns>Un string con el contenido del body obtenido en la respuesta; habitualmente contendrá la info de un objeto o de una lista de objetos y se formateará como Json</returns>
        private string? GetRespuesta(Uri uri)
        {
            var response = _cli.GetAsync(uri).Result;

            if (response.ReasonPhrase == "Not Found")
            {
                return response.ReasonPhrase;
            }

            response.EnsureSuccessStatusCode();

            var json = response.Content.ReadAsStringAsync().Result;
            return json;
        }

        /// <summary>
        /// Método auxiliar para obtener un tipo de cabaña a partir del consumo de la Api. 
        /// </summary>
        /// <param name="idCabanha">El id de un tipo de cabaña a buscar</param>
        /// <returns>El objeto obtenido a partir del json incluido en el body (Content) de la respuesta</returns>
        /// <remarks>Dado que este método se utiliza en varias de las acciones, se separó para no repetir código </remarks>
        private IEnumerable<MantenimientoModel> GetMantCabanha(int? idCabanha)
        {
            try
            {
                var json = GetRespuesta(new Uri(_uriMantenimiento + "/mantCabanhas/" + idCabanha));
                if (json == null)
                    return null;

                if (json == "Not Found")
                {
                    throw new Exception($"No hay mantenimientos para la cabaña con id: {idCabanha}");
                    //ViewBag.ListaVacia = $"No hay mantenimientos para la cabaña con id: {idCabanha}";
                }

                var mant = JsonSerializer.Deserialize<IEnumerable<MantenimientoModel>>(json, opciones);

                if (mant == null)
                    throw new ArgumentNullException("Devolvio null el mantenimiento.");

                return mant;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        /// <summary>
        /// Método auxiliar para obtener un tipo de cabaña a partir del consumo de la Api. 
        /// </summary>
        /// <param name="f1">El id de un tipo de cabaña a buscar</param> 
        /// <param name="f2">El id de un tipo de cabaña a buscar</param>
        /// <param name="idCab">El id de un tipo de cabaña a buscar</param>
        /// <returns>El objeto obtenido a partir del json incluido en el body (Content) de la respuesta</returns>
        /// <remarks>Dado que este método se utiliza en varias de las acciones, se separó para no repetir código </remarks>
        private IEnumerable<MantenimientoModel> GetMantCabanhaEntreFechas(DateTime? f1, DateTime? f2, int? idCab)
        {
            try
            {
                //Convertir en string las fechas
                string f_1 = f1.Value.ToString("yyyy-MM-dd");
                string f_2 = f2.Value.ToString("yyyy-MM-dd");

                var json = GetRespuesta(new Uri(_uriMantenimiento + "/mantenimientosPorFecha/fecha1/" + f_1 + "/fecha2/" + f_2 + "/cabanha/" + idCab));
                if (json == null)
                    return null;

                if (json == "Not Found")
                {
                    throw new Exception($"No hay mantenimientos entre las fechas {f_1} y {f_2}");
                    //ViewBag.ListaVacia = $"No hay mantenimientos para la cabaña con id: {idCabanha}";
                }

                var mant = JsonSerializer.Deserialize<IEnumerable<MantenimientoModel>>(json, opciones);

                if (mant == null)
                    throw new ArgumentNullException("Devolvio null el mantenimiento.");

                return mant;
            }
            catch (Exception ex)
            {
                throw;
            }


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
            catch (Exception ex)
            {
                throw;
            }


        }

        /// <summary>
        /// Método auxiliar para obtener un tipo de cabaña a partir del consumo de la Api. 
        /// </summary>
        /// <param name="nombre">El nombre de un tipo de cabaña a buscar</param>
        /// <returns>El objeto obtenido a partir del json incluido en el body (Content) de la respuesta</returns>
        //private TipoCabanhaModel GetTipoCabanhaByName(string nombre)
        //{
        //    try
        //    {
        //        var json = GetRespuesta(new Uri(_uriTipoCabanha + "/nombre/" + nombre));
        //        if (json == null)
        //            return null;

        //        var tipoCabanha = JsonSerializer.Deserialize<TipoCabanhaModel>(json, opciones);

        //        if (tipoCabanha == null)
        //            throw new ArgumentNullException("Devolvio null el tipo de cabaña.");

        //        return tipoCabanha;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }


        //}

        #endregion
    }
}
