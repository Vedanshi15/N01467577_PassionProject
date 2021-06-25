using System;
using System.IO;
using System.Web;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using N01467577_PassionProject.Models;
using System.Diagnostics;

namespace N01467577_PassionProject.Controllers
{
    public class ProductDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// This code will return all Products in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Products in the database
        /// </returns>
        /// <example>
        /// GET: api/ProductData/ListProduct
        /// </example>
        [HttpGet]
        [ResponseType(typeof(ProductDto))]
        public IHttpActionResult ListProducts()
        {
            List<Product> Products = db.Products.ToList();
            List<ProductDto> ProductDtos = new List<ProductDto>();

            Products.ForEach(p => ProductDtos.Add(new ProductDto()
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Description = p.Description,
                Qty = p.Qty,
                ProductHasPic = p.ProductHasPic,
                PicExtension = p.PicExtension,
                Price = p.Price
            }));

            return Ok(ProductDtos);
        }

        /// <summary>
        /// Gathers information about products related to a particular cart
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all products in the system added by customers to the cart
        /// </returns>
        /// <param name="id">Cart ID.</param>
        /// <example>
        /// GET: api/CartData/ListProductsForCart/1
        /// </example>
        [HttpGet]
        [ResponseType(typeof(ProductDto))]
        public IHttpActionResult ListProductsForCart(int id)
        {
            
            List<Product> Products = db.Products.Where(
                p => p.Carts.Any(
                    c => c.CartId == id
                )).ToList();
            List<ProductDto> ProductDtos = new List<ProductDto>();

            Products.ForEach(p => ProductDtos.Add(new ProductDto()
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductHasPic = p.ProductHasPic,
                PicExtension = p.PicExtension,
                Description = p.Description,
                Qty = p.Qty,
                Price = p.Price
            }));

            return Ok(ProductDtos);
        }
        /// <summary>
        /// Returns products in the system not added to the cart
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all available products not added to the cart
        /// </returns>
        /// <param name="id">CartId Primary Key</param>
        /// <example>
        /// GET: api/ProductData/ListProductsNotaddedtoCart/1
        /// </example>
        [HttpGet]
        [ResponseType(typeof(ProductDto))]
        public IHttpActionResult ListProductsNotaddedtoCart(int id)
        {
            List<Product> Products = db.Products.Where(
                p => !p.Carts.Any(
                    c => c.CartId == id)
                ).ToList();
            List<ProductDto> ProductDtos = new List<ProductDto>();

            Products.ForEach(p => ProductDtos.Add(new ProductDto()
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductHasPic = p.ProductHasPic,
                PicExtension = p.PicExtension,
                Description = p.Description,
                Qty = p.Qty,
                Price = p.Price
            }));

            return Ok(ProductDtos);
        }


        

        [ResponseType(typeof(ProductDto))]
        [HttpGet]
        public IHttpActionResult FindProduct(int id)
        {
            Product p  = db.Products.Find(id);
            ProductDto ProductDto = new ProductDto()
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Description = p.Description,
                Qty = p.Qty,
                ProductHasPic = p.ProductHasPic,
                PicExtension = p.PicExtension,
                Price = p.Price
            };
            if (p == null)
            {
                return NotFound();
            }

            return Ok(ProductDto);
        }
        /// <summary>
        /// Receives product picture data, uploads it to the webserver and updates the product's HasPic option
        /// </summary>
        /// <param name="id">the product id</param>
        /// <returns>status code 200 if successful.</returns>
        /// <example>
        /// POST: api/productData/UpdateproductPic/3
        /// HEADER: enctype=multipart/form-data
        /// FORM-DATA: image
        /// </example>
       
        [HttpPost]
        public IHttpActionResult UploadProductPic(int id)
        {

            bool haspic = false;
            string picextension;
            Console.WriteLine(Request.Content.IsMimeMultipartContent());
            
            if (Request.Content.IsMimeMultipartContent())
            {
                int numfiles = HttpContext.Current.Request.Files.Count;
              
                if (numfiles == 1 && HttpContext.Current.Request.Files[0] != null)
                {
                    
                    var productPic = HttpContext.Current.Request.Files[0];
                   
                    if (productPic.ContentLength > 0)
                    {
                        var valtypes = new[] { "jpeg", "jpg", "png", "gif" };
                        var extension = Path.GetExtension(productPic.FileName).Substring(1);
                     
                        if (valtypes.Contains(extension))
                        {
                            try
                            {
                                
                                string fn = id + "." + extension;

                                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Images/Products/"), fn);

                                productPic.SaveAs(path);

                                haspic = true;
                                picextension = extension;

                                Product Selectedproduct = db.Products.Find(id);
                                Selectedproduct.ProductHasPic = haspic;
                                Selectedproduct.PicExtension = extension;
                                db.Entry(Selectedproduct).State = EntityState.Modified;

                                db.SaveChanges();

                            }
                            catch (Exception ex)
                            {
                               return BadRequest();
                            }
                        }
                    }

                }

                return Ok();
            }
            else
            {
              
                return BadRequest();

            }

        }
     
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateProduct(int id, Product p)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != p.ProductId)
            {

                return BadRequest();
            }

            db.Entry(p).State = EntityState.Modified;
            db.Entry(p).Property(pp => pp.ProductHasPic).IsModified = false;
            db.Entry(p).Property(pp => pp.PicExtension).IsModified = false;
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
       

        [ResponseType(typeof(Product))]
        [HttpPost]
        public IHttpActionResult AddProduct(Product p)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Products.Add(p);
            db.SaveChanges();
            Debug.WriteLine(p.Qty);
            return CreatedAtRoute("DefaultApi", new { id = p.ProductId }, p);
        }

       
        [ResponseType(typeof(Product))]
        [HttpPost]
        public IHttpActionResult DeleteProduct(int id)
        {
            Product p = db.Products.Find(id);
            if (p == null)
            {
                return NotFound();
            }
            if (p.ProductHasPic && p.PicExtension != "")
            {
               
                string path = HttpContext.Current.Server.MapPath("~/Content/Images/Products/" + id + "." + p.PicExtension);
                if (System.IO.File.Exists(path))
                {
                    Debug.WriteLine("File exists... preparing to delete!");
                    System.IO.File.Delete(path);
                }
            }

            db.Products.Remove(p);
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return db.Products.Count(e => e.ProductId == id) > 0;
        }
    }
}
