using System.Transactions;
using backend.config;
using backend.dto;
using backend.model;
using Microsoft.EntityFrameworkCore;

namespace backend.business.products
{
    public class Products : IProducts
    {
        private AppDbContext Context { get; set; }
        private ILogger<Products> _logger { get; set; }

        public Products(AppDbContext context, ILogger<Products> logger)
        {
            Context = context;
            _logger = logger;
        }

        public async Task<List<ProductsDTO>> GetAll()
        {
            var products = await Context.Products.ToListAsync();

            var result = new List<ProductsDTO>();

            foreach (var p in products)
            {
                var prices = await Context.ProductsPrices
                    .Where(w => w.CodeProducts == p.Code)
                    .Select((w) => new dto.ProductsPriceDTO
                    {
                        Id = w.Id,
                        CodeProducts = w.CodeProducts,
                        Stock = w.Stock,
                        Lote = w.Lote,
                        DateLote = w.DateLote,
                        Price = w.Price
                    })
                    .ToListAsync();

                result.Add(new ProductsDTO
                {
                    Code = p.Code,
                    CodeProducts = p.CodeProducts,
                    Name = p.Name,
                    Barcode = p.Barcode,
                    Presentation = p.Presentation,
                    Category = p.Category,
                    IsLote = p.IsLote,
                    IsActive = p.IsActive,
                    Prices = prices,
                    User = p.User
                });
            }

            return result;
        }

        public async Task<ProductsDTO> GetByCode(int code)
        {
            var product = await Context.Products.FindAsync(code);

            if (product == null) return null;

            var prices = await Context.ProductsPrices.Where(p => p.CodeProducts == product.Code)
                                                .Select(s => new
                                                dto.ProductsPriceDTO
                                                {
                                                    Id = s.Id,
                                                    CodeProducts = s.CodeProducts,
                                                    Stock = s.Stock,
                                                    Lote = s.Lote,
                                                    DateLote = s.DateLote,
                                                    Price = s.Price
                                                })
                                                .ToListAsync();

            return new ProductsDTO
            {
                Code = product.Code,
                CodeProducts = product.CodeProducts,
                Name = product.Name,
                Barcode = product.Barcode,
                Presentation = product.Presentation,
                Category = product.Category,
                IsLote = product.IsLote,
                IsActive = product.IsActive,
                Prices = prices,
                User = product.User
            };
        }

        public async Task<ProductsDTO> GetByCodeProduct(string code)
        {
            var product = await Context.Products.FirstOrDefaultAsync(p => p.CodeProducts == code);

            if (product == null) return null;

            var prices = await Context.ProductsPrices.Where(p => p.CodeProducts == product.Code)
                                                .Select(s => new ProductsPriceDTO
                                                {
                                                    Id = s.Id,
                                                    CodeProducts = s.CodeProducts,
                                                    Stock = s.Stock,
                                                    Lote = s.Lote,
                                                    DateLote = s.DateLote,
                                                    Price = s.Price
                                                })
                                                .ToListAsync();

            return new ProductsDTO
            {
                Code = product.Code,
                CodeProducts = product.CodeProducts,
                Name = product.Name,
                Barcode = product.Barcode,
                Presentation = product.Presentation,
                Category = product.Category,
                IsLote = product.IsLote,
                IsActive = product.IsActive,
                Prices = prices,
                User = product.User
            };
        }

        public async Task<ProductsDTO> Insert(ProductsDTO product)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();

            try
            {
                // verificar si existe
                var exists = await Context.Products.FirstOrDefaultAsync(p => p.CodeProducts == product.CodeProducts);

                // si no existes registra uno nuevo
                if (exists == null)
                {
                    var newProduct = new ProductsModel
                    {
                        CodeProducts = product.CodeProducts,
                        Name = product.Name,
                        Barcode = product.Barcode,
                        Presentation = product.Presentation,
                        Category = product.Category,
                        IsService = product.IsService,
                        IsLote = product.IsLote,
                        IsActive = product.IsActive,
                        State = "A",
                        CreatedAt = DateTime.Now,
                        User = product.User
                    };

                    Context.Products.Add(newProduct);
                    await Context.SaveChangesAsync();

                    var price = new model.ProductsPriceModel
                    {
                        CodeProducts = newProduct.Code,
                        Price = product.Price,
                        Lote = product.Lote,
                        DateLote = product.DateLote,
                        Stock = product.Stock,
                    };

                    Context.ProductsPrices.Add(price);
                    await Context.SaveChangesAsync();
                }
                else // si ya existes, verficamos si el lote y la fecha estan ingresados
                {
                    var prices = await Context.ProductsPrices.Where(p => p.CodeProducts == exists.Code).ToListAsync();
                    var existsLote = prices.Find(p => p.Lote == product.Lote && p.DateLote == product.DateLote);

                    // si no hay registramos nuevo precio
                    if (existsLote == null)
                    {
                        var price = new model.ProductsPriceModel
                        {
                            CodeProducts = exists.Code,
                            Price = product.Price,
                            Lote = product.Lote,
                            DateLote = product.DateLote,
                            Stock = product.Stock,
                        };

                        Context.ProductsPrices.Add(price);
                    }
                    else if (existsLote != null) // Si ya existes actualizamos el stock
                    {
                        existsLote.Stock = existsLote.Stock + product.Stock;

                        Context.ProductsPrices.Update(existsLote);
                    }

                    await Context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                return product;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al insertar producto: {message}", ex.Message);
                throw new Exception("Error inserting product", ex);
            }
        }

        public async Task<ProductsDTO> Update(ProductsDTO product)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();
            try
            {
                var _product = await Context.Products.FindAsync(product.Code);

                if (_product == null) return null;

                // Actualizar los datos del producto
                _product.CodeProducts = product.CodeProducts;
                _product.Name = product.Name;
                _product.Barcode = product.Barcode;
                _product.Presentation = product.Presentation;
                _product.Category = product.Category;
                _product.IsLote = product.IsLote;
                _product.IsActive = product.IsActive;
                _product.IsService = product.IsService;
                _product.UpdatedAt = DateTime.Now;
                _product.User = product.User;

                // Elimina los almacenes existentes relacionados
                var wareHouseList = await Context.ProductsPrices
                                        .Where(w => w.CodeProducts == product.Code)
                                        .ToListAsync();

                Context.ProductsPrices.RemoveRange(wareHouseList);

                // Agrega los nuevos almacenes del DTO
                if (product.Prices != null)
                {
                    foreach (var price in product.Prices)
                    {
                        var newWareHouse = new model.ProductsPriceModel
                        {
                            CodeProducts = product.Code,
                            Price = price.Price,
                            Lote = price.Lote,
                            DateLote = price.DateLote,
                            Stock = price.Stock
                        };
                        Context.ProductsPrices.Add(newWareHouse);
                    }
                }

                await Context.SaveChangesAsync();
                await transaction.CommitAsync();

                return product;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "error al actualizar producto: {message}", ex.Message);
                throw new Exception("Error updating product", ex);
            }
        }

        public async Task<bool> Delete(int code)
        {
            using var trasaction = await Context.Database.BeginTransactionAsync();
            try
            {
                var product = await Context.Products.FindAsync(code);

                if (product == null) return false;

                var wareHouseList = await Context.ProductsPrices
                                            .Where(w => w.CodeProducts == product.Code)
                                            .ToListAsync();

                Context.Products.Remove(product);
                Context.ProductsPrices.RemoveRange(wareHouseList);

                await Context.SaveChangesAsync();
                await trasaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await trasaction.RollbackAsync();
                _logger.LogError(ex, "error al eliminar producto: {message}", ex.Message);
                throw;
            }
        }

        public async Task<bool> DeletePriceProduct(int code)
        {
            using var trasaction = await Context.Database.BeginTransactionAsync();
            try
            {
                var price = await Context.ProductsPrices.FindAsync(code);
                Context.ProductsPrices.Remove(price);
                await trasaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await trasaction.RollbackAsync();
                _logger.LogError(ex, "Error al eliminar el precio del producto: {message}", ex.Message);
                throw;
            }
        }
    }
}
