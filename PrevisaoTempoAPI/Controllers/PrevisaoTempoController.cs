using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PrevisaoTempoAPI.Model;

[Route("api/previsaotempo")]
[ApiController]
public class PrevisaoTempoController : ControllerBase
{
    private readonly IHttpClientFactory _clientFactory;

    public PrevisaoTempoController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [HttpGet]
    public async Task<IActionResult> ObterPrevisaoTempo(string nomeCidade, int? numeroDias)
    {
        try
        {
            // Etapa 1: Faz a primeira chamada para obter o ID da cidade informada
            var cidadeId = await ObterCidadeId(nomeCidade);

            if (cidadeId.HasValue)
            {
                // Etapa 2: Faz a segunda chamada com o ID da cidade obtido
                // e o número de dias para obter a previsão do tempo.
                // Número de dias também pode não ser informado, neste caso será retornado um dia por padrão
                var previsaoTempo = await ObterPrevisaoTempo(cidadeId.Value, numeroDias);

                if (previsaoTempo != null)
                {
                    // Etapa 3: Retorna a previsão do tempo para o usuário
                    return Ok(previsaoTempo);
                }
            }

            return NotFound("Cidade não encontrada ou previsão do tempo não disponível.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
        }
    }

    private async Task<int?> ObterCidadeId(string nomeCidade)
    {
        try
        {
            // Construção da URL da primeira chamada com o nome da cidade
            string cidadeApiUrl = $"https://brasilapi.com.br/api/cptec/v1/cidade/{nomeCidade}";

            // Execução da primeira chamada para obter o ID da cidade
            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync(cidadeApiUrl);

            //Caso o resultado seja positivo
            if (response.IsSuccessStatusCode)
            {
                // Lê o conteúdo da resposta como uma string
                var cidadeDataString = await response.Content.ReadAsStringAsync();

                // Analise do JSON da resposta para obter o ID
                var cidadeData = JsonConvert.DeserializeObject<dynamic>(cidadeDataString);
                var cidadeId = cidadeData[0].id;

                return cidadeId;
            }

            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private async Task<PrevisaoTempoModel> ObterPrevisaoTempo(int cidadeId, int? numeroDias)
    {
        try
        {
            // Construção da URL da segunda chamada com o ID da cidade e o número de dias (caso informado)
            string previsaoTempoApiUrl = $"https://brasilapi.com.br/api/cptec/v1/clima/previsao/{cidadeId}";

            // Verificação se foi informado a quantidade de dias como parâmetro
            if (numeroDias.HasValue)
            {
                previsaoTempoApiUrl += $"/{numeroDias}";
            }

            // Execução da segunda chamada para obter a previsão do tempo
            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync(previsaoTempoApiUrl);

            if (response.IsSuccessStatusCode)
            {
                // Leia o conteúdo da resposta como uma string
                var previsaoTempoDataString = await response.Content.ReadAsStringAsync();

                // Analise do JSON da resposta no modelo de PrevisaoTempoModel
                var previsaoTempoData = JsonConvert.DeserializeObject<PrevisaoTempoModel>(previsaoTempoDataString);
                return previsaoTempoData;
            }

            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

}
