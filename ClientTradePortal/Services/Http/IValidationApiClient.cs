namespace ClientTradePortal.Services.Http;
public interface IValidationApiClient
{
    [Post("/api/validation/order")]
    Task<Refit.ApiResponse<ValidationResponse>> ValidateOrderAsync(
        [Body] ValidationRequest request,
        CancellationToken cancellationToken = default);
}