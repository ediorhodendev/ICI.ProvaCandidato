using System.ComponentModel.DataAnnotations.Schema;

namespace ICI.ProvaCandidato.Dados.Entities
{
    public class NoticiaTag
    {
        public int Id { get; set; }

        [ForeignKey("Noticia")]
        public int NoticiaId { get; set; }
        public Noticia Noticia { get; set; }

        [ForeignKey("Tag")]
        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
