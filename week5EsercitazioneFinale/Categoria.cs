using System.Collections.Generic;

namespace week5EsercitazioneFinale
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public virtual IList<Spesa> Spese { get; set; }
    }
}