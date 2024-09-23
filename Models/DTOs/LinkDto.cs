namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.DTOs
{
    public class LinkDto
    {
        public int Id { get; set; }
        public string Href { get; set; }
        public string Rel { get; set; }
        public string Metodo { get; set; }

        // Metodo para montar o link 
        public LinkDto(int id, string href, string rel, string metodo)
        {
            Id = id;
            Href = href;
            Rel = rel;
            Metodo = metodo;
        }
    }
    public class LinksHATEOS
    {
        public List<LinkDto> Links { get; set; } = new List<LinkDto>();
    }
}
