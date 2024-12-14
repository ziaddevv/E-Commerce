﻿using E_Commerce;
using E_Commerce.Data;
using E_Commerce.Models;
using System.Collections.Generic;
using System.Linq;

public class ProductRepository : IProductRepository
{
	private readonly ApplicationDbContext _context;

	public ProductRepository(ApplicationDbContext context)
	{
		_context = context;
	}

	public IEnumerable<Product> GetAllProducts()
	{
		return _context.Products.ToList();
	}

	public Product GetProductById(int id)
	{
		return _context.Products.FirstOrDefault(p => p.Id == id);
	}

	public void AddProduct(Product product)
	{
		_context.Products.Add(product);
		_context.SaveChanges();
	}

	public void UpdateProduct(Product product)
	{
		_context.Products.Update(product);
		_context.SaveChanges();
	}

	public void DeleteProduct(int id)
	{
		var product = _context.Products.FirstOrDefault(p => p.Id == id);
		if (product != null)
		{
			_context.Products.Remove(product);
			_context.SaveChanges();
		}
	}
}
