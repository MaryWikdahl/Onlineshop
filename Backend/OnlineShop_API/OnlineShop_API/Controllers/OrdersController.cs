﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop_API.Data;
using OnlineShop_API.Dto;
using OnlineShop_API.Identity;
using OnlineShop_API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineShop_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OnlineShopDbContext _context;

        public OrdersController(OnlineShopDbContext context)
        {
            _context = context;
        }

        // POST: api/orders
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<OrderCreatedDto>> CreateOrder([FromBody] OrderCreateDto orderCreateDto)
        {
            if (orderCreateDto == null || orderCreateDto.OrderItems == null || !orderCreateDto.OrderItems.Any())
            {
                return BadRequest("Order items cannot be empty.");
            }

            // Get UserId from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("User not found.");
            }

            var totalAmount = 0m;
            var orderItems = new List<OrderItems>();
            var orderCreatedItems = new List<OrderItemDto>();

            foreach (var item in orderCreateDto.OrderItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                {
                    return BadRequest($"Product with ID {item.ProductId} not found.");
                }

                //if (product.StockQuantity < item.Quantity)
                //{
                //    return BadRequest($"Insufficient stock for product with ID {item.ProductId}. Available: {product.StockQuantity}, requested: {item.Quantity}.");
                //}

                // Minska lagersaldot
                //product.StockQuantity -= item.Quantity;

                var orderItem = new OrderItems
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };

                orderItems.Add(orderItem);
                totalAmount += product.Price * item.Quantity;

                orderCreatedItems.Add(new OrderItemDto
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = product.Price
                });
            }

            // Skapa en order med status "Pending"
            var order = new Orders
            {
                UserId = userId,
                OrderItems = orderItems,
                Status = Status.Pending, // Statusen är Pending initialt
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            // Skicka inte bekräftelsen än. Vänta på Stripe-betalningen
            return Ok(new { Message = "Order created successfully. Please complete the payment to confirm the order." });
        }



        // GET: api/orders
        [HttpGet]
        [Authorize(Policy = IdentityData.AdminUserPolicyName)] // Endast administratörer kan hämta alla ordrar
        public async Task<ActionResult<IEnumerable<OrderCreatedDto>>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ToListAsync();

            var orderDtos = orders.Select(o => new OrderCreatedDto
            {
                OrderId = o.Id,
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity
                }).ToList(),
                Status = o.Status.ToString(),
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount
            }).ToList();

            return Ok(orderDtos);
        }

        // GET: api/orders/{id}
        [HttpGet("{id}")]
        [Authorize(Policy = IdentityData.AdminUserPolicyName)] // Endast administratörer kan hämta specifik order
        public async Task<ActionResult<OrderCreatedDto>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            var orderDto = new OrderCreatedDto
            {
                OrderId = order.Id,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity
                }).ToList(),
                Status = order.Status.ToString(),
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount
            };

            return Ok(orderDto);
        }

        // PUT: api/orders/{id}/status
        [HttpPut("{id}/status")]
        [Authorize(Policy = IdentityData.AdminUserPolicyName)] // Endast administratörer kan uppdatera orderstatus
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            if (!Enum.TryParse(typeof(Status), status, true, out var newStatus))
            {
                return BadRequest("Invalid status value");
            }

            order.Status = (Status)newStatus;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/orders/{id}
        // PUT: api/orders/{id}
        [HttpPut("{id}")]
        [Authorize(Policy = IdentityData.AdminUserPolicyName)] // Endast administratörer kan uppdatera order
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderCreateDto orderUpdateDto)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            // Spåra nuvarande orderitems
            var currentOrderItems = order.OrderItems.ToList();

            // Öka lagersaldot för produkter som tas bort eller vars kvantitet minskar
            foreach (var existingItem in currentOrderItems)
            {
                var matchingItem = orderUpdateDto.OrderItems.FirstOrDefault(i => i.ProductId == existingItem.ProductId);
                var product = await _context.Products.FindAsync(existingItem.ProductId);

                if (product == null) continue;

                if (matchingItem == null)
                {
                    // Produkten tas bort från ordern, återställ hela kvantiteten
                    //product.StockQuantity += existingItem.Quantity;
                }
                else if (existingItem.Quantity > matchingItem.Quantity)
                {
                    // Kvantiteten minskar, återställ skillnaden
                    //product.StockQuantity += (existingItem.Quantity - matchingItem.Quantity);
                }
            }

            // Minska lagersaldot för produkter som läggs till eller vars kvantitet ökar
            foreach (var updatedItem in orderUpdateDto.OrderItems)
            {
                var existingItem = currentOrderItems.FirstOrDefault(i => i.ProductId == updatedItem.ProductId);
                var product = await _context.Products.FindAsync(updatedItem.ProductId);

                if (product == null)
                {
                    return BadRequest($"Product with ID {updatedItem.ProductId} not found.");
                }

                if (existingItem == null)
                {
                    // Produkten är ny i ordern, minska hela kvantiteten
                    //if (product.StockQuantity < updatedItem.Quantity)
                    //{
                    //    return BadRequest($"Insufficient stock for product with ID {updatedItem.ProductId}.");
                    //}

                    //product.StockQuantity -= updatedItem.Quantity;
                }
                else if (updatedItem.Quantity > existingItem.Quantity)
                {
                    // Kvantiteten ökar, minska skillnaden
                    var difference = updatedItem.Quantity - existingItem.Quantity;

                    //if (product.StockQuantity < difference)
                    //{
                    //    return BadRequest($"Insufficient stock for product with ID {updatedItem.ProductId}.");
                    //}

                    //product.StockQuantity -= difference;
                }
            }

            // Uppdatera orderitems
            order.OrderItems.Clear();
            foreach (var updatedItem in orderUpdateDto.OrderItems)
            {
                var newItem = new OrderItems
                {
                    ProductId = updatedItem.ProductId,
                    Quantity = updatedItem.Quantity,
                    OrderId = order.Id
                };
                order.OrderItems.Add(newItem);
            }

            // Uppdatera totalbeloppet
            order.TotalAmount = order.OrderItems.Sum(oi => oi.Quantity * _context.Products.First(p => p.Id == oi.ProductId).Price);

            await _context.SaveChangesAsync();

            return NoContent();
        }


        // DELETE: api/orders/{id}
        // DELETE: api/orders/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = IdentityData.AdminUserPolicyName)] // Endast administratörer kan ta bort order
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems) // Inkludera orderitems för att kunna hantera lager
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            // Återställ lagersaldot för varje produkt i ordern
            foreach (var orderItem in order.OrderItems)
            {
                var product = await _context.Products.FindAsync(orderItem.ProductId);
                if (product != null)
                {
                    //product.StockQuantity += orderItem.Quantity;
                }
            }

            // Ta bort ordern
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}