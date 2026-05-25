using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CuyControl.Controllers;

[Authorize(Roles = "Administrador,Operador")]
public class EnfermedadController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
