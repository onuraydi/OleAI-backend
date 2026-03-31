using MediatR;
using OleAI.Backend.Application.Features.Pantry.Events;
using OleAI.Backend.Application.Services.Llm;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.Pantry.EventHandlers
{
    public class IngredientRequestedEventHandler : INotificationHandler<IngredientRequestedEvent>
    {
        private readonly IIngredientRequestRepository _requestRepository;
        private readonly ILocalLlmService _localLlmService;

        public IngredientRequestedEventHandler(IIngredientRequestRepository requestRepository, ILocalLlmService localLlmService)
        {
            _requestRepository = requestRepository;
            _localLlmService = localLlmService;
        }

        public async Task Handle(IngredientRequestedEvent notification, CancellationToken cancellationToken)
        {
            var request = await _requestRepository.GetAsync(r => r.Id == notification.RequestId, cancellationToken: cancellationToken);
            if (request == null) return;

            try
            {
                // 2. LLM'e bağlan ve analiz et (Bu işlem birkaç saniye sürebilir, o yüzden arkaplanda yapıyoruz)
                // Not: Gerçek LLM entegrasyonunda buraya prompt bağlamını da (mevcut kategoriler vb.) gönderebilirsin.
                var llmResultJson = await _localLlmService.AnalyzeIngredientRequestAsync(notification.RequestedName, cancellationToken);

                // 3. Domain entity'sinin iş kuralını (Behavior) tetikle
                request.UpdateLlmAnalysis(llmResultJson);

                // 4. Analiz sonucunu veritabanına kaydet
                await _requestRepository.UpdateAsync(request);
            }
            catch (Exception ex)
            {
                // LLM servisi çökmüş olabilir, talebin Pending statüsünde kalmasını sağla ama logla
                request.UpdateLlmAnalysis($"{{\"Error\": \"LLM Analizi başarısız oldu: {ex.Message}\"}}");
                await _requestRepository.UpdateAsync(request);
            }
        }
    }
}
