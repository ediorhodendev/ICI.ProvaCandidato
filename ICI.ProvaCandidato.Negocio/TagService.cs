using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ICI.ProvaCandidato.Dados;
using ICI.ProvaCandidato.Dados.Entities;
using Microsoft.EntityFrameworkCore;

namespace ICI.ProvaCandidato.Negocio
{
    public class TagService
    {
        private readonly TagDbContext _context;

        public TagService(TagDbContext context)
        {
            _context = context;
        }

        public async Task<List<Tag>> GetAllTagsAsync()
        {
            return await _context.Tags.ToListAsync();
        }

        public async Task<Tag> GetTagByIdAsync(int id)
        {
            return await _context.Tags.FindAsync(id);
        }

        public async Task CreateOrUpdateTagAsync(Tag tag)
        {
            if (tag.Id == 0)
            {
                // Lógica para criar uma nova tag
                _context.Tags.Add(tag);
            }
            else
            {
                // Lógica para editar uma tag existente
                _context.Tags.Update(tag);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteTagAsync(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                return false;
            }

            // Verifica se a tag está vinculada a alguma notícia
            var isTagLinkedToNoticia = await _context.NoticiaTags.AnyAsync(nt => nt.TagId == tag.Id);
            if (isTagLinkedToNoticia)
            {
                return false;
            }

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
