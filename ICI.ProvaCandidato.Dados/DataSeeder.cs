using System;
using System.Collections.Generic;
using System.Linq;
using ICI.ProvaCandidato.Dados;
using ICI.ProvaCandidato.Dados.Entities;

public static class DataSeeder
{
    public static void Initialize(TagDbContext context)
    {
        // Verifica se já existem registros no banco de dados
        if (context.Tags.Any() || context.Noticias.Any() || context.Usuarios.Any())
        {
            return; // O banco de dados já foi inicializado
        }

        // Crie 5 usuários
        var usuarios = new List<Usuario>();
        for (int i = 1; i <= 5; i++)
        {
            usuarios.Add(new Usuario
            {
                Nome = $"Usuário {i}",
                Senha = $"senha{i}",
                Email = $"usuario{i}@example.com"
            });
        }

        context.Usuarios.AddRange(usuarios);
        context.SaveChanges();

        // Crie 10 tags
        var tags = new List<Tag>();
        for (int i = 1; i <= 10; i++)
        {
            tags.Add(new Tag { Descricao = $"Tag {i}" });
        }

        context.Tags.AddRange(tags);
        context.SaveChanges();

        // Crie 10 notícias e relacione aleatoriamente 2 tags e 1 usuário a cada notícia
        var random = new Random();
        var noticias = new List<Noticia>();

        foreach (var _ in Enumerable.Range(1, 10))
        {
            var noticia = new Noticia
            {
                Titulo = $"Título da Notícia {_}",
                Texto = $"Texto da Notícia {_}",
                UsuarioId = usuarios[random.Next(usuarios.Count)].Id,
            };

            // Relacione aleatoriamente 2 tags à notícia
            var tagsRelacionadas = tags.OrderBy(x => random.Next()).Take(2).ToList();
            noticia.NoticiaTags = tagsRelacionadas.Select(tag => new NoticiaTag { TagId = tag.Id }).ToList();

            noticias.Add(noticia);
        }

        context.Noticias.AddRange(noticias);
        context.SaveChanges();
    }
}
