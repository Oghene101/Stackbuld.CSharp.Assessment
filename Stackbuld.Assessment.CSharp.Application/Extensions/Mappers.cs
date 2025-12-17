using Stackbuld.Assessment.CSharp.Application.Common.Contracts;
using Stackbuld.Assessment.CSharp.Application.Common.Filters;
using Stackbuld.Assessment.CSharp.Application.Features.Auth.Commands;
using Stackbuld.Assessment.CSharp.Application.Features.Cart.Commands;
using Stackbuld.Assessment.CSharp.Application.Features.Merchant.Queries;
using Stackbuld.Assessment.CSharp.Application.Features.Product.Commands;
using Stackbuld.Assessment.CSharp.Application.Features.Product.Queries;
using Stackbuld.Assessment.CSharp.Domain.Entities;
using Cart = Stackbuld.Assessment.CSharp.Application.Common.Contracts.Cart;
using CreateProductRequest = Stackbuld.Assessment.CSharp.Application.Common.Contracts.Product.CreateProductRequest;
using UpdateProductRequest = Stackbuld.Assessment.CSharp.Application.Common.Contracts.Product.UpdateProductRequest;
using GetProductsRequest = Stackbuld.Assessment.CSharp.Application.Common.Contracts.Product.GetProductsRequest;
using GetProductByIdResponse = Stackbuld.Assessment.CSharp.Application.Common.Contracts.Product.GetProductByIdResponse;
using GetProductsResponse = Stackbuld.Assessment.CSharp.Application.Common.Contracts.Product.GetProductsResponse;
using Product = Stackbuld.Assessment.CSharp.Domain.Entities.Product;
using RefreshToken = Stackbuld.Assessment.CSharp.Application.Features.Auth.Commands.RefreshToken;

namespace Stackbuld.Assessment.CSharp.Application.Extensions;

public static class Mappers
{
    #region To Entity

    public static User ToEntity(this SignUp.Command dto)
        => new()
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            UserName = dto.Email,
            CreatedBy = $"{dto.FirstName} {dto.LastName}",
            UpdatedBy = $"{dto.FirstName} {dto.LastName}"
        };

    public static User ToEntity(this SignUpMerchant.Command dto)
        => new()
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            UserName = dto.Email,
            CreatedBy = $"{dto.FirstName} {dto.LastName}",
            UpdatedBy = $"{dto.FirstName} {dto.LastName}"
        };


    public static Product ToEntity(this CreateProduct.Command dto, Guid merchantId)
        => new()
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            MerchantId = merchantId
        };

    public static CartItem ToCartItem(this Product dto, int quantity, Guid cartId)
        => new()
        {
            Quantity = quantity,
            CartId = cartId,
            ProductId = dto.Id,
        };

    #endregion

    #region To Command

    public static SignUp.Command ToCommand(this Auth.SignUpRequest dto)
        => new(dto.FirstName, dto.LastName, dto.Email, dto.Password);

    public static SignUpMerchant.Command ToCommand(this Auth.SignUpMerchantRequest dto)
        => new(dto.FirstName, dto.LastName, dto.Email, dto.BusinessName, dto.Password);

    public static ConfirmEmail.Command ToCommand(this Auth.ConfirmEmailRequest dto)
        => new(dto.Email, dto.Token);

    public static SignIn.Command ToCommand(this Auth.SignInRequest dto)
        => new(dto.Email, dto.Password);

    public static RefreshToken.Command ToCommand(this Auth.RefreshTokenRequest dto)
        => new(dto.AccessToken, dto.RefreshToken);

    public static ChangePassword.Command ToCommand(this Auth.ChangePasswordRequest dto)
        => new(dto.OldPassword, dto.NewPassword);

    public static CreateProduct.Command ToCommand(this CreateProductRequest dto)
        => new(dto.Name, dto.Description, dto.Price, dto.StockQuantity);

    public static UpdateProduct.Command ToCommand(this UpdateProductRequest dto,
        Guid productId)
        => new(productId, dto.ProductName, dto.Description, dto.Price, dto.StockQuantity);

    public static AddToCart.Command ToCommand(this Cart.AddToCartRequest dto,
        Guid productId)
        => new(productId, dto.Quantity);

    #endregion

    #region To Queries

    public static GetProducts.Query ToQuery(this GetProductsRequest dto)
        => new(
            new PaginationFilter(dto.PageNumber, dto.PageSize),
            new DateRangeFilter(dto.StartDate, dto.EndDate));

    public static GetProductsByMerchantId.Query ToQuery(
        this Common.Contracts.Product.GetProductsByMerchantIdRequest dto)
        => new(
            new PaginationFilter(dto.PageNumber, dto.PageSize),
            new DateRangeFilter(dto.StartDate, dto.EndDate));

    #endregion

    #region To View Model

    public static GetProductByIdResponse ToVm(this Product dto, string businessName)
        => new(
            dto.MerchantId,
            businessName,
            dto.Id,
            dto.Name,
            dto.Description,
            dto.Price,
            dto.StockQuantity);

    public static PaginatorVm<IEnumerable<GetProductsResponse>> ToVm(
        this IEnumerable<Product> dto, int totalCount, int pageNumber, int pageSize)
    {
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var getProductsResponses =
            dto.Select(x => new GetProductsResponse(x.Id, x.Name, x.Price));

        return new PaginatorVm<IEnumerable<GetProductsResponse>>(pageSize, pageNumber,
            totalPages, totalCount, getProductsResponses);
    }

    public static Cart.GetCartByUserIdResponse ToVm(
        this Domain.Entities.Cart dto, Dictionary<Guid, Product> products)
    {
        var cartItems = dto.CartItems.Select(x =>
        {
            var product = products[x.ProductId];
            return new Cart.CartItemVm
            (
                x.Id,
                x.ProductId,
                product.Name,
                product.Price,
                x.Quantity
            );
        }).ToArray();

        return new Cart.GetCartByUserIdResponse
        (
            dto.Id,
            cartItems
        );
    }

    #endregion
}