using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop_API.Data;
using OnlineShop_API.DTOs;
using OnlineShop_API.Identity;
using OnlineShop_API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace OnlineShop_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly OnlineShopDbContext _context;

        public ProductsController(OnlineShopDbContext context)
        {
            _context = context;
        }

        // GET: api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductsDto>>> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.SubCategory) // Inkludera subkategori om relevant
                .Include(p => p.ProductInfo)
                .ToListAsync();

            // Skapa en lista av DTO:er
            var productDtos = products.Select(product => new ProductsDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                EncodedImage = product.EncodedImage,
                SubCategoryId = product.SubCategoryId,
                ProductInfo = product.ProductInfo.Select(pi => new ProductInfoDto() 
                { 
                    StockQuantity = pi.StockQuantity, 
                    Size = pi.Size,
                    Color = pi.Color, 
                }).ToList()
            }).ToList();

            return Ok(productDtos);
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductsDto>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.SubCategory) // Inkludera subkategori om relevant
                .Include(p => p.ProductInfo)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Skapa DTO-svar
            var responseDto = new ProductsDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                EncodedImage = product.EncodedImage,
                SubCategoryId = product.SubCategoryId,
                ProductInfo = product.ProductInfo.Select(pi => new ProductInfoDto()
                {
                    StockQuantity = pi.StockQuantity,
                    Size = pi.Size,
                    Color = pi.Color,
                }).ToList()
            };

            return Ok(responseDto);
        }

        // POST: api/products
        [HttpPost]
        [Authorize(Policy = IdentityData.AdminUserPolicyName)]
        public async Task<ActionResult<ProductsDto>> PostProduct([FromBody] ProductsDto productDto)
        {
            if (productDto == null)
            {
                return BadRequest("Product data is required.");
            }

            // Kontrollera att subkategorin finns
            var subCategoryExists = await _context.SubCategories.AnyAsync(sc => sc.Id == productDto.SubCategoryId);
            if (!subCategoryExists)
            {
                return NotFound("Subcategory not found.");
            }

            // Skapa en ny produkt
            var product = new Products
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                EncodedImage = productDto.EncodedImage,
                SubCategoryId = productDto.SubCategoryId,
                ProductInfo = productDto.ProductInfo.Select(pi => new ProductInfo()
                {
                    StockQuantity = pi.StockQuantity,
                    Size = pi.Size,
                    Color = pi.Color,
                }).ToList(),
            };

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            // Skapa ett svar som inkluderar all information om produkten
            var responseDto = new ProductsDto
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                EncodedImage = product.EncodedImage,
                SubCategoryId = product.SubCategoryId,
                //StockQuantity = product.StockQuantity
            };

            // Returnera det skapade objektet
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, responseDto);
        }

        // PUT: api/products/{id}
        [HttpPut("{id}")]
        [Authorize(Policy = IdentityData.AdminUserPolicyName)]
        public async Task<IActionResult> PutProduct(int id, [FromBody] ProductsDto productDto)
        {
            if (productDto == null)
            {
                return BadRequest("Product data is required.");
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Kontrollera att subkategorin finns
            var subCategoryExists = await _context.SubCategories.AnyAsync(sc => sc.Id == productDto.SubCategoryId);
            if (!subCategoryExists)
            {
                return NotFound("Subcategory not found.");
            }

            // Ta bort gamla productInfos
            var productInfos = await _context.ProductInfo.Where(pi => pi.ProductId == id).ToListAsync();
            _context.ProductInfo.RemoveRange(productInfos);

            // Uppdatera produktens information
            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.EncodedImage = productDto.EncodedImage;
            product.SubCategoryId = productDto.SubCategoryId;
            product.ProductInfo = productDto.ProductInfo.Select(pi => new ProductInfo()
            {
                StockQuantity = pi.StockQuantity,
                Size = pi.Size,
                Color = pi.Color,
            }).ToList();


            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Returnera det uppdaterade objektet
            return Ok(productDto);
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = IdentityData.AdminUserPolicyName)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Get productInfos related to this product
            var productInfos = await _context.ProductInfo.Where(pi => pi.ProductId == id).ToListAsync();

            _context.Products.Remove(product);
            _context.ProductInfo.RemoveRange(productInfos);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
