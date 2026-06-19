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
                1. Eğer kullanıcının girdiği kelime, sistemdeki temel malzemelerden birinin eşanlamlısı, yanlış yazılmış hali veya alt türüyse (örn: 'Çeri Domates' -> 'Domates' ya da 'Domates' -> 'Domaset'), IsNew=false yap ve SuggestedAliasFor alanına sistemdeki o temel malzemenin adını yaz. Ayrıca EstimatedShelfLifeInDays değerini (gün cinsinden raf ömrü) tahmin et (Örn: Süt ise 4).
                2. Eğer bu tamamen yeni, geçerli ve yenilebilir bir malzemeyse (örn: 'Ejder Meyvesi'), IsNew=true yap, SuggestedAliasFor=null yap. Buna uygun bir Kategori (Sebze, Meyve, Süt Ürünleri, Kuru Gıda vb.), BaseUnitType (Gr, Adet, Litre) belirle ve EstimatedShelfLifeInDays değerini (gün cinsinden) tahmin et.
                3. Eğer bu anlamsız bir metinse (örn: 'asdfg') veya yiyecek değilse, IsNew=false yap, Reason kısmına 'Geçersiz yiyecek maddesi' yaz.

                SADECE geçerli bir JSON objesi döndür. JSON içinde 'EstimatedShelfLifeInDays' adında integer (sayısal) bir alan daha olsun. Başka hiçbir metin veya markdown (```json) kullanma.";

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
                // Eğer LLM kapalıysa veya hata verirse, otomatik olarak reddetmek yerine admin'in onayına sunmak için
                // temiz (clean) kabul edip geçiyoruz. Daha sonra admin inceleyecektir.
                return true;
            }
        }

        public async Task<string> AnalyzeRecipeContentAsync(string description, List<string> steps, CancellationToken cancellationToken = default)
        {
            var phase1SystemPrompt = @"You are a senior Turkish culinary editor. Your task is to analyze and correct the grammar of a recipe description.

CRITICAL RULES:
1. Fix Turkish grammar, spelling, capitalization, and punctuation.
2. CULINARY DICTIONARY (Fix these common typos): 'suvan'->'soğan', 'imce'->'ince', 'yımırta'->'yumurta', 'domatiz'->'domates', 'püskevit'->'bisküvi', 'sarmısak'->'sarımsak'.
3. DO NOT hallucinate, change the meaning, or replace culinary equipment/ingredients with unrelated words.
4. Pay special attention to Turkish characters 'I/ı' and 'İ/i'.
5. If it contains advertisements, links, or spam, set 'isApproved' to false and state 'Spam'.
6. If it contains profanity or complete gibberish (e.g. 'asdfg'), set 'isApproved' to false and 'isToxic' to true.
7. RESPOND STRICTLY IN JSON FORMAT using the structure below. Do NOT output markdown formatting like ```json.

EXPECTED JSON:
{
  ""reasoning"": ""Explain what typos and grammatical errors you found and why you are correcting them."",
  ""isApproved"": boolean,
  ""isToxic"": boolean,
  ""correctedDescription"": ""The grammatically corrected description, or null if rejected"",
  ""rejectionReason"": ""Reason if rejected, or null""
}

--- EXAMPLES ---

INPUT:
Lütfen açıklamayı analiz et ve düzelt:
Açıklama:
Bugun size harıka bir suvanli yımırta tarifi verecegim. cok leziz oluyor.

JSON OUTPUT:
{
  ""reasoning"": ""The text contains common typos like 'harıka' (harika), 'suvanli' (soğanlı), 'yımırta' (yumurta), 'verecegim' (vereceğim), 'cok' (çok). I will fix these based on the culinary dictionary and correct the punctuation by capitalizing the first letters of sentences."",
  ""isApproved"": true,
  ""isToxic"": false,
  ""correctedDescription"": ""Bugün size harika bir soğanlı yumurta tarifi vereceğim. Çok leziz oluyor."",
  ""rejectionReason"": null
}

INPUT:
Lütfen açıklamayı analiz et ve düzelt:
Açıklama:
asdfghjkl www.kumar.com a girin

JSON OUTPUT:
{
  ""reasoning"": ""The text contains gibberish 'asdfghjkl' and a spam link. It must be rejected."",
  ""isApproved"": false,
  ""isToxic"": true,
  ""correctedDescription"": null,
  ""rejectionReason"": ""Spam and gibberish content detected.""
}";

            var phase1UserPrompt = $"Lütfen açıklamayı analiz et ve düzelt:\nAçıklama:\n{description}";

            try
            {
                var phase1Output = await ExecuteOllamaRequestAsync(phase1UserPrompt, cancellationToken, systemPromptOverride: phase1SystemPrompt);
                if (string.IsNullOrEmpty(phase1Output)) return "{\"isApproved\":false,\"isToxic\":false,\"correctedDescription\":null,\"correctedSteps\":[],\"rejectionReason\":\"LLM boş yanıt döndü\"}";

                using var doc = JsonDocument.Parse(phase1Output);
                var isApproved = doc.RootElement.GetProperty("isApproved").GetBoolean();

                if (!isApproved)
                {
                    // Eğer onaylanmadıysa direkt bu sonucu correctedSteps ekleyerek dön
                    var isToxic = doc.RootElement.TryGetProperty("isToxic", out var toxicProp) && toxicProp.GetBoolean();
                    var rejectionReason = doc.RootElement.TryGetProperty("rejectionReason", out var reasonProp) ? reasonProp.GetString() : "Bilinmeyen Sebep";

                    var rejectedResult = new
                    {
                        isApproved = false,
                        isToxic = isToxic,
                        correctedDescription = (string?)null,
                        correctedSteps = new List<string>(),
                        rejectionReason = rejectionReason
                    };
                    return JsonSerializer.Serialize(rejectedResult);
                }

                var correctedDescription = doc.RootElement.GetProperty("correctedDescription").GetString();

                // Phase 2: Adımları tek tek analiz et
                var correctedSteps = new List<string>();
                var stepSystemPrompt = @"You are a senior Turkish culinary editor. Your job is to correct a single recipe step.

CRITICAL RULES:
1. ONLY fix grammar, spelling, and punctuation. Convert verbs to professional imperative mood (e.g., 'doğra' -> 'doğrayın', 'koyuyorum' -> 'koyun').
2. CULINARY DICTIONARY (Fix these common typos): 'suvan'->'soğan', 'imce'->'ince', 'yımırta'->'yumurta', 'domatiz'->'domates', 'püskevit'->'bisküvi', 'sarmısak'->'sarımsak', 'pampiş'->'pembe'.
3. DO NOT change the vocabulary, meaning, or cooking method.
4. DO NOT hallucinate or change objects. A pan ('tava') must stay a pan, it must NOT become a plate ('tabak').
5. Pay special attention to correct Turkish characters 'I/ı' and 'İ/i'.
6. RESPOND STRICTLY IN JSON FORMAT using the structure below. Do NOT output markdown formatting like ```json.

EXPECTED JSON:
{
  ""reasoning"": ""Explain the grammatical errors and typos you found, and how you will correct them to an imperative mood."",
  ""correctedStep"": ""The grammatically corrected step text""
}

--- EXAMPLES ---

INPUT:
Lütfen adımı dilbilgisi kurallarına göre düzelt:
suvanları imce imce doğrayıp zeytinyaginda hafif pembeleşene kadar kavuruyorum.

JSON OUTPUT:
{
  ""reasoning"": ""I identified typos: 'suvanları' (soğanları), 'imce imce' (ince ince), 'zeytinyaginda' (zeytinyağında). The verb 'kavuruyorum' is in first-person present continuous; I will change it to the professional imperative 'kavurun'. I will capitalize the first letter."",
  ""correctedStep"": ""Soğanları ince ince doğrayıp zeytinyağında hafif pembeleşene kadar kavurun.""
}

INPUT:
Lütfen adımı dilbilgisi kurallarına göre düzelt:
sonra onlari yagi koydugunuz tavaya alip karamelize edin.

JSON OUTPUT:
{
  ""reasoning"": ""Typos identified: 'onlari' (onları), 'yagi' (yağı), 'koydugunuz' (koyduğunuz), 'alip' (alıp). The verb 'karamelize edin' is already in the correct imperative form. I will fix the Turkish characters and capitalize the first letter. I will not change 'tavaya' to another word."",
  ""correctedStep"": ""Sonra onları yağı koyduğunuz tavaya alıp karamelize edin.""
}";
                foreach (var step in steps)
                {
                    var stepUserPrompt = $"Lütfen adımı dilbilgisi kurallarına göre düzelt:\n{step}";
                    try
                    {
                        var stepOutput = await ExecuteOllamaRequestAsync(stepUserPrompt, cancellationToken, systemPromptOverride: stepSystemPrompt);

                        if (!string.IsNullOrEmpty(stepOutput))
                        {
                            using var stepDoc = JsonDocument.Parse(stepOutput);
                            if (stepDoc.RootElement.TryGetProperty("correctedStep", out var prop) && prop.ValueKind == JsonValueKind.String)
                            {
                                correctedSteps.Add(prop.GetString()!);
                            }
                            else
                            {
                                correctedSteps.Add(step);
                            }
                        }
                        else
                        {
                            correctedSteps.Add(step);
                        }
                    }
                    catch
                    {
                        // Fallback to original step if LLM fails for this specific step
                        correctedSteps.Add(step);
                    }
                }

                // Final JSON
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
                return $"{{\"isApproved\":false,\"isToxic\":false,\"correctedDescription\":null,\"correctedSteps\":[],\"rejectionReason\":\"Hata: {ex.Message.Replace("\"", "'")}\"}}";
            }
        }

        private async Task<string?> ExecuteOllamaRequestAsync(string prompt, CancellationToken cancellationToken, string? systemPromptOverride = null)
        {
            var requestPayload = new
            {
                model = _settings.ModelName,
                system = systemPromptOverride ?? _settings.SystemPrompt,
                prompt = prompt,
                stream = false,
                format = "json",
                options = new
                {
                    temperature = 0.1,
                    top_p = 0.9,
                    top_k = 40
                }
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
                EstimatedShelfLifeInDays = (int?)null,
                Reason = $"SİSTEM HATASI: {errorMessage}. Lütfen manuel karar verin."
            };

            return JsonSerializer.Serialize(errorObject);
        }
    }
}
