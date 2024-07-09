using AuthenticationService.Server.Business;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Scrypt;

namespace AuthenticationService.Server.Services
{
    [Authorize]
    public class OrdersService : Orders.OrdersBase
    {
        private readonly ILogger<OrdersService> _logger;
        private readonly ScryptEncoder _encoder;

        public OrdersService(ILogger<OrdersService> logger)
        {
            _logger = logger;
            _encoder = new ScryptEncoder();
        }

        [Authorize]
        public override Task<RegisterReply> Register(RegisterRequest request, ServerCallContext context)
        {
            return Task.FromResult(new RegisterReply { Success = true });
        }

        //[Authorize]
        public override async Task RegisterStreaming(RegisterRequest request, IServerStreamWriter<RegisterStreamingReply> responseStream, ServerCallContext context)
        {
            OrdersBusiness business = new OrdersBusiness();
        
            var responses = business.RegisterOrder(request);
        
            await foreach (var response in responses)
            {
                await responseStream.WriteAsync(response);
            }
        }
    }
}
