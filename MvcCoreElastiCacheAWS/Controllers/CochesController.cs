using Microsoft.AspNetCore.Mvc;
using MvcCoreElastiCacheAWS.Models;
using MvcCoreElastiCacheAWS.Repositories;
using MvcCoreElastiCacheAWS.Services;

namespace MvcCoreElastiCacheAWS.Controllers
{
    public class CochesController : Controller
    {
        private RepositoryCoches repo;
        private ServiceAWSCache service;

        public CochesController(RepositoryCoches repo, ServiceAWSCache service)
        {
            this.repo = repo;
            this.service = service;
        }

        public IActionResult Index()
        {
            List<Coche> coches = this.repo.GetCoches();
            return View(coches);
        }

        public IActionResult Details(int id)
        {
            Coche car = this.repo.FindCoche(id);
            return View(car);
        }

        public async Task<IActionResult> Favoritos()
        {
            List<Coche> coches = await this.service.GetCochesFavoritosAsync();
            return View(coches);
        }

        public async Task<IActionResult> SeleccionarFavorito(int idcoche)
        {
            Coche car = this.repo.FindCoche(idcoche);
            await this.service.AddCocheAsync(car);
            return RedirectToAction("Favoritos");
        }

        public async Task<IActionResult> EliminarFavorito(int idcoche)
        {
            await this.service.DeleteCocheFavoritoAsync(idcoche);
            return RedirectToAction("Favoritos");
        }
    }
}
