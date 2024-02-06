using ICI.ProvaCandidato.Dados;
using ICI.ProvaCandidato.Dados.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ICI.ProvaCandidato.Negocio
{
    public class NoticiaService
    {
        private readonly TagDbContext _context;

        public NoticiaService(TagDbContext context)
        {
            _context = context;
        }

        public async Task<List<Noticia>> GetAllNoticiasAsync()
        {
            return await _context.Noticias
                .Include(n => n.Usuario)
                .Include(n => n.NoticiaTags)
                .ThenInclude(nt => nt.Tag)
                .OrderBy(n => n.Id)
                .ThenBy(n => n.Titulo)
                .ToListAsync();
        }

        public async Task<List<int>> GetTagsSelecionadasAsync(int? noticiaId)
        {
            var noticiaTags = await _context.NoticiaTags
                .Where(nt => nt.NoticiaId == noticiaId)
                .Select(nt => nt.TagId)
                .ToListAsync();

            return noticiaTags;
        }
        public async Task<List<Usuario>> GetAllUsuariosAsync()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            return usuarios;
        }


        public async Task<Noticia> GetNoticiaAsync(int? id)
        {
            return await _context.Noticias.FindAsync(id);
        }

        public async Task CreateOrUpdateNoticiaAsync(Noticia noticia, int[] TagsSelecionadas)
        {
            if (noticia.Id == 0)
            {
                // Lógica para criar uma nova notícia
                _context.Noticias.Add(noticia);
                await _context.SaveChangesAsync();

                // Associe as Tags selecionadas à Notícia
                if (TagsSelecionadas != null)
                {
                    foreach (var tagId in TagsSelecionadas)
                    {
                        var noticiaTag = new NoticiaTag
                        {
                            NoticiaId = noticia.Id,
                            TagId = tagId
                        };
                        _context.NoticiaTags.Add(noticiaTag);
                    }
                    await _context.SaveChangesAsync();
                }
            }
            else if (noticia.Id != 0)
            {
                // Lógica para editar a notícia existente
                _context.Noticias.Update(noticia);
                await _context.SaveChangesAsync();

                // Atualize os vínculos com as Tags selecionadas
                if (TagsSelecionadas != null)
                {
                    var noticiaTagsExistentes = _context.NoticiaTags.Where(nt => nt.NoticiaId == noticia.Id).ToList();

                    // Remova quaisquer vínculos existentes que não estejam na lista de Tags selecionadas
                    foreach (var noticiaTag in noticiaTagsExistentes)
                    {
                        if (!TagsSelecionadas.Contains(noticiaTag.TagId))
                        {
                            _context.NoticiaTags.Remove(noticiaTag);
                        }
                    }

                    // Adicione novos vínculos para Tags selecionadas que não existem nos vínculos existentes
                    foreach (var tagId in TagsSelecionadas)
                    {
                        if (!noticiaTagsExistentes.Any(nt => nt.TagId == tagId))
                        {
                            var noticiaTag = new NoticiaTag
                            {
                                NoticiaId = noticia.Id,
                                TagId = tagId
                            };
                            _context.NoticiaTags.Add(noticiaTag);
                        }
                    }
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task<Noticia> DeleteNoticiaAsync(int? id)
        {
            var noticia = await _context.Noticias.FindAsync(id);
            if (noticia != null)
            {
                _context.Noticias.Remove(noticia);
                await _context.SaveChangesAsync();
            }
            return noticia;
        }
    }
}
