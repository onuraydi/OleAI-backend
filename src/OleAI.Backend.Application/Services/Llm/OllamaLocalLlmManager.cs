using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Application.Services.Llm
{
    public class OllamaLocalLlmManager : ILocalLlmService
    {
        private readonly HttpClient _httpClient;
        private readonly OllamaSettings _settings;
        private readonly IIngredientRepository _ingredientRepository;

        public OllamaLocalLlmManager(HttpClient httpClient, IOptions<OllamaSettings> settings, IIngredientRepository ingredientRepository)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
            _httpClient.Timeout = TimeSpan.FromMinutes(2);
            _ingredientRepository = ingredientRepository;
        }

        public async Task<string> AnalyzeIngredientRequestAsync(string requestedName, CancellationToken cancellationToken = default)
        {
            QueryOptions<Ingredient> query = new QueryOptions<Ingredient> { Size = 1000, EnableTracking = false };
            var existingIngredientResult = await _ingredientRepository.GetListAsync(query, cancellationToken);
            var existingIngredientNames = string.Join(", ", existingIngredientResult.Items.Select(i => i.Name));

            var userPrompt = $@"
                Kullanıcı dolabına şu malzemeyi eklemek istiyor: '{requestedName}'
                Sistemimizde kayıtlı bazı temel malzemeler şunlar: [{existingIngredientNames}]

                Görevler:
                1. Eğer kullanıcının girdiği kelime, sistemdeki temel malzemelerden birinin eşanlamlısı, yanlış yazılmış hali veya alt türüyse (örn: 'Çeri Domates' -> 'Domates' ya da 'Domates' -> 'Domaset'), IsNew=false yap ve SuggestedAliasFor alanına sistemdeki o temel malzemenin adını yaz.
                2. Eğer bu tamamen yeni, geçerli ve yenilebilir bir malzemeyse (örn: 'Ejder Meyvesi'), IsNew=true yap, SuggestedAliasFor=null yap. Buna uygun bir Kategori (Sebze, Meyve, Süt Ürünleri, Kuru Gıda vb.) ve BaseUnitType (Gr, Adet, Litre) belirle.
                3. Eğer bu anlamsız bir metinse (örn: 'asdfg') veya yiyecek değilse, IsNew=false yap, Reason kısmına 'Geçersiz yiyecek maddesi' yaz.

                SADECE geçerli bir JSON objesi döndür. Başka hiçbir metin veya markdown (```json) kullanma.";

            try
            {
                var llmOutput = await ExecuteOllamaRequestAsync(userPrompt, cancellationToken);
                if (string.IsNullOrEmpty(llmOutput)) return GenerateErrorJson("LLM boş yanıt döndü");
                JsonDocument.Parse(llmOutput);
                return llmOutput;
            }
            catch (Exception ex)
            {
                return GenerateErrorJson($"Hata: {ex.Message}");
            }
        }

        public async Task<bool> CheckRecipeContentForSpamAsync(string title, string description, CancellationToken cancellationToken = default)
        {
            var userPrompt = $@"
                Sen bir içerik moderatörüsün. Aşağıdaki yemek tarifini incele. Küfür, spam, anlamsız tuşlamalar (örn: 'asdfg') veya yiyecek dışı tehlikeli bir içerik barındırıp barındırmadığını kontrol et.

                Başlık: '{title}'
                Açıklama: '{description}'

                Görevler:
                1. Eğer içerik normal, geçerli bir yemek tarifiyse veya zararsız bir yiyecek anlatımıysa IsClean=true yap.
                2. Eğer içerik spam, küfür, anlamsız metin veya yiyecekle alakasız tehlikeli bir madde içeriyorsa IsClean=false yap.
                3. Reason alanına kararının nedenini kısaca yaz.

                SADECE geçerli bir JSON objesi döndür. Başka hiçbir metin veya markdown (```json) kullanma.
                Örnek Format: {{ ""IsClean"": true, ""Reason"": ""Geçerli yemek tarifi."" }}";

            try
            {
                var llmOutput = await ExecuteOllamaRequestAsync(userPrompt, cancellationToken);
                if (string.IsNullOrEmpty(llmOutput)) return false;

                using var outputDoc = JsonDocument.Parse(llmOutput);
                return outputDoc.RootElement.GetProperty("IsClean").GetBoolean();
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<string?> ExecuteOllamaRequestAsync(string prompt, CancellationToken cancellationToken)
        {
            var requestPayload = new
            {
                model = _settings.ModelName,
                system = _settings.SystemPrompt,
                prompt = prompt,
                stream = false,
                format = "json"
            };

            var content = new StringContent(JsonSerializer.Serialize(requestPayload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/generate", content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);
            using var jsonDocument = JsonDocument.Parse(responseString);

            return jsonDocument.RootElement.GetProperty("response").GetString();
        }

        private string GenerateErrorJson(string errorMessage)
        {
            // LLM çökse bile sistemimizin (Admin panelinin) patlamaması için Fallback (Yedek) JSON dönüyoruz.
            var errorObject = new
            {
                IsNew = true, // Admin manuel incelesin diye true bırakıyoruz
                SuggestedAliasFor = (string?)null,
                Category = "Belirsiz",
                BaseUnitType = "Adet",
                Reason = $"SİSTEM HATASI: {errorMessage}. Lütfen manuel karar verin."
            };

            return JsonSerializer.Serialize(errorObject);
        }
    }
}
