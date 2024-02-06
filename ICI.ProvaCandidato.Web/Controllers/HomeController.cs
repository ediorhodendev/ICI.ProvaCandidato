using ICI.ProvaCandidato.Dados;
using ICI.ProvaCandidato.Dados.Entities;
using ICI.ProvaCandidato.Negocio;
using ICI.ProvaCandidato.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ICI.ProvaCandidato.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TagService _tagService;

        public HomeController(ILogger<HomeController> logger, TagService tagService)
        {
            _logger = logger;
            _tagService = tagService;
        }

        public async Task<IActionResult> Index()
        {
            var tags = await _tagService.GetAllTagsAsync();
            return View(tags);
        }

        public async Task<IActionResult> Cadastro(int? id)
        {
            if (id == null)
            {
                // Ação de criação
                ViewData["Acao"] = "Cadastro";
                return View("CadastroTag", new Tag());
            }
            else
            {
                // Ação de edição
                var tag = await _tagService.GetTagByIdAsync(id.Value); // Aguarde a operação assíncrona

                if (tag == null)
                {
                    return NotFound();
                }

                ViewData["Acao"] = "Edicao";
                ViewData["Descricao"] = tag.Descricao; // Agora você pode acessar a descrição corretamente

                return View("CadastroTag", tag);
            }
        }






        [HttpPost]
        public async Task<IActionResult> Cadastro(Tag tag)
        {
            if (ModelState.IsValid)
            {
                await _tagService.CreateOrUpdateTagAsync(tag);
                TempData["SuccessMessage"] = "Tag cadastrada/atualizada com sucesso.";
                return RedirectToAction("Index");
            }
            ViewData["Acao"] = (tag.Id == 0) ? "Cadastro" : "Edicao";
            return View("CadastroTag", tag);
        }

        public async Task<IActionResult> ConfirmDelete(int id)
        {
            bool tagDeleted = await _tagService.DeleteTagAsync(id);
            if (!tagDeleted)
            {
                TempData["ErrorMessage"] = "Não é possível excluir a Tag, pois ela está vinculada a uma ou mais notícias.";
            }
            else
            {
                TempData["SuccessMessage"] = "Tag excluída com sucesso.";
            }

            return RedirectToAction("Index");
        }

        public IActionResult Leiame()
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
