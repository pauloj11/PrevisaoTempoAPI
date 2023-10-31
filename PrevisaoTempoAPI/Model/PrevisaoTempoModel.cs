namespace PrevisaoTempoAPI.Model
{
    public class PrevisaoTempoModel
    {
        public string cidade { get; set; }
        public string estado { get; set; }
        public string atualizado_em { get; set; }
        public List<ClimaModel> clima { get; set; }
    }

    public class ClimaModel
    {
        public string data { get; set; }
        public string condicao { get; set; }
        public string condicao_desc { get; set; }
        public int min { get; set; }
        public int max { get; set; }
        public int indice_uv { get; set; }
    }

}
