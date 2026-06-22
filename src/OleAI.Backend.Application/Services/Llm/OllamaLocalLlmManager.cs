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
            _httpClient.Timeout = TimeSpan.FromMinutes(30); // Artırıldı (CPU'da uzun sürebilir)
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
                1. Eğer kullanıcının girdiği kelime, sistemdeki temel malzemelerden birinin eşanlamlısı, yanlış yazılmış hali veya alt türüyse (örn: 'Çeri Domates' -> 'Domates' ya da 'Domates' -> 'Domaset'), IsNew=false yap ve SuggestedAliasFor alanına sistemdeki o temel malzemenin adını yaz. Ayrıca EstimatedShelfLifeInDays değerini (gün cinsinden raf ömrü) tahmin et (Örn: Süt ise 4).
                2. Eğer bu tamamen yeni, geçerli ve yenilebilir bir malzemeyse (örn: 'Ejder Meyvesi'), IsNew=true yap, SuggestedAliasFor=null yap. Buna uygun bir Kategori (Sebze, Meyve, Süt Ürünleri, Kuru Gıda vb.), BaseUnitType (Gr, Adet, Litre) belirle ve EstimatedShelfLifeInDays değerini (gün cinsinden) tahmin et.
                3. Eğer bu anlamsız bir metinse (örn: 'asdfg') veya yiyecek değilse, IsNew=false yap, Reason kısmına 'Geçersiz yiyecek maddesi' yaz.

                SADECE geçerli bir JSON objesi döndür. JSON içinde 'EstimatedShelfLifeInDays' adında integer (sayısal) bir alan daha olsun. Başka hiçbir metin veya markdown (```json) kullanma.";

            try
            {
                var systemPrompt = "Sen kıdemli bir mutfak ve veri tabanı asistanısın. Kullanıcının girdiği malzeme adını analiz edip SADECE JSON formatında yanıt vermelisin.";
                var llmOutput = await ExecuteOllamaRequestAsync(userPrompt, cancellationToken, systemPromptOverride: systemPrompt);
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
                var systemPrompt = "Sen bir içerik moderatörüsün. Küfür, spam, anlamsız tuşlamalar veya yiyecek dışı tehlikeli bir içerik barındırıp barındırmadığını kontrol et.";
                var llmOutput = await ExecuteOllamaRequestAsync(userPrompt, cancellationToken, systemPromptOverride: systemPrompt);
                if (string.IsNullOrEmpty(llmOutput)) return false;

                using var outputDoc = JsonDocument.Parse(llmOutput);
                return outputDoc.RootElement.GetProperty("IsClean").GetBoolean();
            }
            catch (Exception)
            {
                // Eğer LLM kapalıysa veya hata verirse, otomatik olarak reddetmek yerine admin'in onayına sunmak için
                // temiz (clean) kabul edip geçiyoruz. Daha sonra admin inceleyecektir.
                return true;
            }
        }

        public async Task<string> AnalyzeRecipeContentAsync(string description, List<string> steps, CancellationToken cancellationToken = default)
        {
            // Tüm tarifi (açıklama + adımlar) TEK BİR istekte gönder.
            // Böylece 8 ayrı istek yerine 1 istek atılır ve ~80 dk yerine ~12 dk sürer.
            var stepsBlock = new StringBuilder();
            for (int i = 0; i < steps.Count; i++)
            {
                stepsBlock.AppendLine($"Adım {i + 1}: {steps[i]}");
            }

            var userPrompt = $@"Lütfen aşağıdaki tarif açıklamasını ve tüm adımlarını analiz edip düzelt.

Açıklama:
{description}

Adımlar:
{stepsBlock}

YANIT FORMATI (SADECE JSON, başka hiçbir şey yazma):
{{""isApproved"": true/false, ""isToxic"": true/false, ""correctedDescription"": ""düzeltilmiş açıklama"", ""correctedSteps"": [""düzeltilmiş adım 1"", ""düzeltilmiş adım 2"", ...], ""rejectionReason"": null veya ""red sebebi""}}";

            try
            {
                var llmOutput = await ExecuteOllamaRequestAsync(userPrompt, cancellationToken, systemPromptOverride: null);
                if (string.IsNullOrEmpty(llmOutput))
                {
                    var emptyObj = new { isApproved = false, isToxic = false, correctedDescription = (string?)null, correctedSteps = new List<string>(), rejectionReason = "LLM boş yanıt döndü" };
                    return JsonSerializer.Serialize(emptyObj);
                }

                // Markdown bloklarını temizle
                llmOutput = llmOutput.Replace("```json", "").Replace("```", "").Trim();

                using var doc = JsonDocument.Parse(llmOutput);

                bool isApproved = true;
                if (doc.RootElement.TryGetProperty("isApproved", out var approvedProp) && (approvedProp.ValueKind == JsonValueKind.True || approvedProp.ValueKind == JsonValueKind.False))
                {
                    isApproved = approvedProp.GetBoolean();
                }

                bool isToxic = false;
                if (doc.RootElement.TryGetProperty("isToxic", out var toxicProp) && (toxicProp.ValueKind == JsonValueKind.True || toxicProp.ValueKind == JsonValueKind.False))
                {
                    isToxic = toxicProp.GetBoolean();
                }

                string? rejectionReason = doc.RootElement.TryGetProperty("rejectionReason", out var reasonProp) && reasonProp.ValueKind == JsonValueKind.String ? reasonProp.GetString() : null;

                if (!isApproved || isToxic)
                {
                    var rejectedResult = new
                    {
                        isApproved = false,
                        isToxic = isToxic,
                        correctedDescription = (string?)null,
                        correctedSteps = new List<string>(),
                        rejectionReason = rejectionReason ?? "Yapay zeka bu içeriği onaylamadı."
                    };
                    return JsonSerializer.Serialize(rejectedResult);
                }

                string? correctedDescription = doc.RootElement.TryGetProperty("correctedDescription", out var descProp) && descProp.ValueKind == JsonValueKind.String ? descProp.GetString() : description;

                // Düzeltilmiş adımları JSON dizisinden oku
                var correctedSteps = new List<string>();
                if (doc.RootElement.TryGetProperty("correctedSteps", out var stepsProp) && stepsProp.ValueKind == JsonValueKind.Array)
                {
                    foreach (var stepElement in stepsProp.EnumerateArray())
                    {
                        correctedSteps.Add(stepElement.GetString() ?? "");
                    }
                }

                // Eğer LLM adım sayısını yanlış döndürdüyse, orijinal adımları kullan
                if (correctedSteps.Count != steps.Count)
                {
                    correctedSteps = steps;
                }

                var finalResult = new
                {
                    isApproved = true,
                    isToxic = false,
                    correctedDescription = correctedDescription,
                    correctedSteps = correctedSteps,
                    rejectionReason = (string?)null
                };

                return JsonSerializer.Serialize(finalResult);
            }
            catch (Exception ex)
            {
                var errorObj = new
                {
                    isApproved = false,
                    isToxic = false,
                    correctedDescription = (string?)null,
                    correctedSteps = new List<string>(),
                    rejectionReason = $"Hata: {ex.Message}"
                };
                return JsonSerializer.Serialize(errorObj);
            }
        }

        private async Task<string?> ExecuteOllamaRequestAsync(string prompt, CancellationToken cancellationToken, string? systemPromptOverride = null)
        {
            object requestPayload;

            if (!string.IsNullOrWhiteSpace(systemPromptOverride))
            {
                requestPayload = new
                {
                    model = _settings.ModelName,
                    system = systemPromptOverride,
                    prompt = prompt,
                    stream = false,
                    format = "json",
                    options = new { temperature = 0.0, top_p = 0.9, top_k = 40 }
                };
            }
            else
            {
                requestPayload = new
                {
                    model = _settings.ModelName,
                    prompt = prompt,
                    stream = false,
                    format = "json",
                    options = new { temperature = 0.0, top_p = 0.9, top_k = 40 }
                };
            }

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
                EstimatedShelfLifeInDays = (int?)null,
                Reason = $"SİSTEM HATASI: {errorMessage}. Lütfen manuel karar verin."
            };

            return JsonSerializer.Serialize(errorObject);
        }
    }
}
