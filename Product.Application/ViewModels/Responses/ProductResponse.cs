using Product.Application.DTOs;

namespace Product.Application.ViewModels.Responses
{
    public record ProductResponse(int ProductId, string Name, string Description, string ProductType, string UnitOfMeasure, string PrimaryProductImageUrl, NameAndCode ProductStatus, DateTime? DateModified);
}
