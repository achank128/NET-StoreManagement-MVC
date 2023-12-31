﻿using Microsoft.EntityFrameworkCore;
using StoreManagement.Models;
using StoreManagement.Repositories.RepositoryBase;

namespace StoreManagement.Repositories.ProductRepository
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        private readonly StoreManagementContext _context;

        public ProductRepository(StoreManagementContext context) : base(context) 
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllProducts()
        {
            List<Product> products = await _context.Products.Include(s => s.Category).ToListAsync();
            return products;
        }
    }
}
