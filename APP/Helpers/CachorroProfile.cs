using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ApiVet.Helpers
{
    public class CachorroProfile

    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Temperament { get; set; }
        public string Life_Span { get; set; }
        public string Origin { get; set; }
        public PesoRaca Weight { get; set; }
        public AlturaRaca Height { get; set; }
        public ImagemRaca Image { get; set; }

    }
    public class PesoRaca
    {
        public string imperial { get; set; }
        public string metric { get; set; }
    }
    public class ImagemRaca
    {
        public string id { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string url { get; set; }
    }
    public class AlturaRaca
    {
        public string imperial { get; set; }
        public string metric { get; set; }
    }

    public class CachorroProfile2

    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Temperament { get; set; }
        public string Life_Span { get; set; }
        public string Origin { get; set; }


    }

    public class GetApi
    {
        public readonly HttpClient _httpClient;

        public GetApi()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.thedogapi.com/v1/"),
            };

            _httpClient.DefaultRequestHeaders.Add("x-api-key", "dca98e62-2fbc-4eac-8cea-e4c4b335f4bb");
        }


        public async Task<List<CachorroProfile>> GetBreeds()
        {
            var response = await _httpClient.GetAsync("breeds?limit=20");
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<CachorroProfile>>(jsonString);
        }

        public async Task<List<CachorroProfile2>> GetBreedByName(string nome)
        {
            var response = await _httpClient.GetAsync($"breeds/search?q={nome}");
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<CachorroProfile2>>(jsonString);
        }

        public async Task<List<CachorroProfile>> GetPublicImages()
        {
            var response = await _httpClient.GetAsync("search?limit=20");
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<CachorroProfile>>(jsonString);
        }


    }


}
