using ICI.ProvaCandidato.Dados;
using ICI.ProvaCandidato.Dados.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ICI.ProvaCandidato.Negocio;
using System.Linq;

public class UsuariosController : Controller
{
    private readonly TagDbContext _context;
    private readonly UsuarioService _usuarioService;

    public UsuariosController(UsuarioService usuarioService,TagDbContext context)
    {
        _context = context;
        _usuarioService = usuarioService;
    }

    public IActionResult Usuarios()
    {
        var usuarios = _usuarioService.ObterTodosUsuarios();
        return View(usuarios);
    }

    public IActionResult Criar(int? id)
    {
        

        if (id == null || id == 0)
        {
            // Ação de criação
            ViewBag.Acao = "Criar";
            return View(new Usuario());
        }
        else
        {
            // Ação de edição
            ViewBag.Acao = "Editar";
            var usuario = _usuarioService.ObterUsuarioPorId(id.Value);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }
    }

    [HttpPost]
    public IActionResult Criar(Usuario usuario)
    {
        if (ModelState.IsValid)
        {
            if (usuario.Id == 0)
            {
                _usuarioService.AdicionarUsuario(usuario);
            }
            else
            {
                _usuarioService.AtualizarUsuario(usuario);
            }

            return RedirectToAction("Usuarios");
        }

        ViewBag.Acao = (usuario.Id == 0) ? "Cadastrar Usuário" : "Editar Usuário";
        return View(usuario);
    }

    public IActionResult ConfirmDelete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        _usuarioService.ExcluirUsuario(id.Value);

        TempData["Message"] = "Usuário excluído com sucesso.";

        return RedirectToAction("Usuarios");
    }



}
