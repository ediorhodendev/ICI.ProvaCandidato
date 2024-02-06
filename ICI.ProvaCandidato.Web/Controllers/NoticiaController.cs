using ICI.ProvaCandidato.Dados.Entities;
using ICI.ProvaCandidato.Negocio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

public class NoticiasController : Controller
{
    private readonly NoticiaService _noticiaService;
    private readonly TagService _tagService;
    private readonly UsuarioService _usuarioService;

    public NoticiasController(NoticiaService noticiaService, TagService tagService, UsuarioService usuarioService)
    {
        _noticiaService = noticiaService;
        _tagService = tagService;
        _usuarioService = usuarioService;
    }

    public async Task<IActionResult> Noticias()
    {
        List<Noticia> noticias = await _noticiaService.GetAllNoticiasAsync();
        return View(noticias);
    }

    public async Task<IActionResult> Criar(int? id)
    {
        // Carregue as listas de tags e usuários para as combobox
        var tags = await _tagService.GetAllTagsAsync();
        var usuarios = await _noticiaService.GetAllUsuariosAsync();

        ViewBag.Tags = new SelectList(tags, "Id", "Descricao");
        ViewBag.Usuarios = new SelectList(usuarios, "Id", "Nome");

        if (id == null || id == 0)
        {
            // Ação de criação
            ViewBag.Acao = "Criacao"; // Defina a ação como "Criacao" usando ViewBag
            return View(new Noticia());
        }
        else
        {
            // Ação de edição
            ViewBag.Acao = "Edicao"; // Defina a ação como "Edicao" usando ViewBag

            // Lógica para buscar a notícia com o ID especificado e passá-la para a view
            var noticia = await _noticiaService.GetNoticiaAsync(id);

            if (noticia == null)
            {
                // Trate o caso em que a notícia não foi encontrada
                return NotFound();
            }

            // Passe o ID do usuário e os IDs das tags relacionadas à notícia para a view
            ViewBag.UsuarioId = noticia.UsuarioId;
            ViewBag.TagsSelecionadas = await _noticiaService.GetTagsSelecionadasAsync(id);

            return View(noticia);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Criar(Noticia noticia, int[] TagsSelecionadas)
    {
        if (ModelState.IsValid)
        {
            await _noticiaService.CreateOrUpdateNoticiaAsync(noticia, TagsSelecionadas);
            return RedirectToAction("Noticias", "Noticias");
        }

        // Se houver erros de validação, volte para a página de criação ou edição com erros
        var tags = await _tagService.GetAllTagsAsync();
        var usuarios = await _noticiaService.GetAllUsuariosAsync();

        ViewBag.Tags = new SelectList(tags, "Id", "Descricao");
        ViewBag.Usuarios = new SelectList(usuarios, "Id", "Nome");

        return View(noticia);
    }

    public async Task<IActionResult> ConfirmDelete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var noticia = await _noticiaService.GetNoticiaAsync(id);
        if (noticia == null)
        {
            return NotFound();
        }

        // Realiza a exclusão diretamente
        var deletedNoticia = await _noticiaService.DeleteNoticiaAsync(id);

        TempData["Message"] = $"Tag {deletedNoticia.Titulo} excluída com sucesso.";

        return RedirectToAction("Noticias", "Noticias");
    }
}
