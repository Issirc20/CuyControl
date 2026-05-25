using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CuyControl.Controllers;

[Authorize(Roles = "Administrador,Operador")]
public class InventarioAlimentoController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
