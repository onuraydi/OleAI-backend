using MediatR;
using OleAI.Backend.Application.Features.Auth.Dtos.Requests;
using OleAI.Backend.Application.Features.Auth.Dtos.Responses;

namespace OleAI.Backend.Application.Features.Auth.Commands.ResendOtp
{
    public class ResendOtpCommand : IRequest<ResendedOtpCodeResponse>
    {
        public ResendOtpCodeRequest Request { get; set; } = default!;
    }
}
