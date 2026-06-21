using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OleAI.Backend.Application.Features.Pantry.Events;
using OleAI.Backend.Application.Services.Llm;
using OleAI.Backend.Application.Services.Repositories;

namespace OleAI.Backend.Application.Features.Pantry.EventHandlers
{
    public class IngredientRequestedEventHandler : INotificationHandler<IngredientRequestedEvent>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public IngredientRequestedEventHandler(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task Handle(IngredientRequestedEvent notification, CancellationToken cancellationToken)
        {
            // 2. LLM'e bağlan ve analiz et (Bu işlem birkaç saniye veya dakika sürebilir.
            // Nginx Gateway Timeout (504) yememek için MediatR handler'ını bloklamayıp gerçek bir arkaplan thread'ine atıyoruz)
            _ = Task.Run(async () =>
            {
                using var scope = _scopeFactory.CreateScope();
                var requestRepository = scope.ServiceProvider.GetRequiredService<IIngredientRequestRepository>();
                var localLlmService = scope.ServiceProvider.GetRequiredService<ILocalLlmService>();

                var request = await requestRepository.GetAsync(r => r.Id == notification.RequestId);
                if (request == null) return;

                try
                {
                    // CancellationToken.None veriyoruz çünkü HTTP Request'i iptal olsa bile bu arkaplan işlemi devam etmeli
                    var llmResultJson = await localLlmService.AnalyzeIngredientRequestAsync(notification.RequestedName, CancellationToken.None);
                    
                    // 3. Domain entity'sinin iş kuralını (Behavior) tetikle
                    request.UpdateLlmAnalysis(llmResultJson);
                    
                    // 4. Analiz sonucunu veritabanına kaydet
                    await requestRepository.UpdateAsync(request);
                }
                catch (Exception ex)
                {
                    // LLM servisi çökmüş olabilir, talebin Pending statüsünde kalmasını sağla ama logla
                    request.UpdateLlmAnalysis($"{{\"Error\": \"LLM Analizi başarısız oldu: {ex.Message}\"}}");
                    await requestRepository.UpdateAsync(request);
                }
            });

            return Task.CompletedTask;
        }
    }
}
