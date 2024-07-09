using AuthenticationService.Server.Services;

namespace AuthenticationService.Server.Business
{
    public class OrdersBusiness
    {
        public async IAsyncEnumerable<RegisterStreamingReply> RegisterOrder(RegisterRequest request)
        {
            yield return new RegisterStreamingReply
            {
                Success = true,
                Finished = false,
                Message = "A validar pedido"
            };

            await Task.Delay(TimeSpan.FromMilliseconds(500));

            yield return new RegisterStreamingReply
            {
                Success = true,
                Finished = false,
                Message = "A registar pedido"
            };

            await Task.Delay(TimeSpan.FromMilliseconds(500));

            yield return new RegisterStreamingReply
            {
                Success = true,
                Finished = false,
                Message = "A processar pedido"
            };

            await Task.Delay(TimeSpan.FromMilliseconds(500));

            yield return new RegisterStreamingReply
            {
                Success = true,
                Finished = false,
                Message = "A enviar pedido"
            };

            await Task.Delay(TimeSpan.FromMilliseconds(1500));

            yield return new RegisterStreamingReply
            {
                Success = true,
                Finished = true,
                Message = "Pedido concluido"
            };
        }
    }
}
