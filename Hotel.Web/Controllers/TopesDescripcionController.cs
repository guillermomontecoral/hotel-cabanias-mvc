
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Web.Controllers
{
    public class TopesDescripcionController : Controller
    {

/*
        // GET: TopesDescripcionController1cs
        public ActionResult Index()
        {
            if (HttpContext.Session.GetString("email") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            IEnumerable<TopesDescripcion> topes = _repoTopeDescripcion.FindAll();
            try
            {
                if (!topes.Any())
                {
                    ViewBag.ListaVacia = $"La lista se encuentra vacía.";
                    return View();
                }
                return View(topes);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
            
        }

        // GET: TopesDescripcionController1cs/Details/5
        public ActionResult Details(int id)
        {
            if (HttpContext.Session.GetString("email") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // GET: TopesDescripcionController1cs/Create
        public ActionResult Create()
        {
            if (HttpContext.Session.GetString("email") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: TopesDescripcionController1cs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TopesDescripcion tope)
        {
            if (HttpContext.Session.GetString("email") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                if (tope == null)
                {
                    return BadRequest();
                }

                _repoTopeDescripcion.Add(tope);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(tope);
            }
        }

        // GET: TopesDescripcionController1cs/Edit/5
        public ActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("email") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var tope = _repoTopeDescripcion.FindById(id);
            return View(tope);
        }

        // POST: TopesDescripcionController1cs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, TopesDescripcion topes)
        {
            if (HttpContext.Session.GetString("email") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                if (topes != null)
                {
                    _repoTopeDescripcion.Update(topes);
                    return RedirectToAction(nameof(Index));
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error {ex.Message}";
                return View();
            }
        }

        // GET: TopesDescripcionController1cs/Delete/5
        public ActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("email") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: TopesDescripcionController1cs/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            if (HttpContext.Session.GetString("email") == null)
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
     
        }
*/
    }
}
