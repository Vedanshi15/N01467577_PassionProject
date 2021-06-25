using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Diagnostics;
using System.Web.Http.Description;
using N01467577_PassionProject.Models;

namespace N01467577_PassionProject.Controllers
{
    public class CartDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// It will return all Carts available in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Carts in the database
        /// </returns>
        /// <example>
        /// GET: api/CartData/ListCart
        /// </example>
        [HttpGet]
        [ResponseType(typeof(CartDto))]
        public IHttpActionResult ListCarts()
        {
            List<Cart> Cart = db.Carts.ToList();
            List<CartDto> CartDtos = new List<CartDto>();

            Cart.ForEach(c => CartDtos.Add(new CartDto()
            {
                CartId = c.CartId,
                CustomerName = c.CustomerName,
                date = c.date
            }));

            return Ok(CartDtos);
        }
        /// <summary>
        /// Add a particular product to the cart
        /// </summary>
        /// <param name="productid">The product Id primary key</param>
        /// <param name="cartid">The cart Id primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/CartData/AddProducttoCart/1/4
        /// </example>
        [HttpPost]
        [Route("api/CartData/AddProducttoCart/{cartid}/{productid}")]
        public IHttpActionResult AddProducttoCart(int cartid, int productid)
        {

            Cart SelectedCart = db.Carts.Include(c => c.Products).Where(c => c.CartId == cartid).FirstOrDefault();
            Product SelectedProduct = db.Products.Find(cartid);

            if (SelectedCart == null || SelectedProduct == null)
            {
                return NotFound();
            }

            Debug.WriteLine("input cart id is: " + cartid);
            Debug.WriteLine("selected cart name is: " + SelectedCart.CustomerName);
            Debug.WriteLine("input product id is: " + productid);
            Debug.WriteLine("selected product name is: " + SelectedProduct.ProductName);

            SelectedCart.Products.Add(SelectedProduct);
            db.SaveChanges();

            return Ok();
        }
        /// <summary>
        /// Returns all customers(cart) who added particular product to the cart.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all added products to the cart by customer
        /// </returns>
        /// <param name="id">Product Primary Key</param>
        /// <example>
        /// GET: api/CartData/ListCartsForProduct/2
        /// </example>
        [HttpGet]
        [ResponseType(typeof(CartDto))]
        public IHttpActionResult ListCartsForProduct(int id)
        {
            List<Cart> Carts = db.Carts.Where(
                c => c.Products.Any(
                    p => p.ProductId == id)
                ).ToList();
            List<CartDto> CartDtos = new List<CartDto>();

            Carts.ForEach(c => CartDtos.Add(new CartDto()
            {
                CartId = c.CartId,
                CustomerName = c.CustomerName,
                date = c.date
            }));

            return Ok(CartDtos);
        }

        /// <summary>
        /// It will return particular cart from the avilable carts in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: Carts in the system matching up to the Cart ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The primary key of the Cart</param>
        /// <example>
        /// GET: api/CartData/FindCart/5
        /// </example>
        [ResponseType(typeof(CartDto))]
        [HttpGet]
        public IHttpActionResult FindCart(int id)
        {
            Cart c = db.Carts.Find(id);
            CartDto CartDto = new CartDto()
            {
                CartId = c.CartId,
                CustomerName = c.CustomerName,
                date = c.date
            };
            if (c == null)
            {
                return NotFound();
            }

            return Ok(CartDto);
        }

        /// <summary>
        /// It will update a particular Cart in the system 
        /// </summary>
        /// <param name="id"> Cart ID primary key</param>
        /// <param name="Cart">JSON FORM DATA of the Cart</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/CartData/UpdateCart/5
        /// FORM DATA: Cart JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateCart(int id, Cart Cart)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Cart.CartId)
            {

                return BadRequest();
            }

            db.Entry(Cart).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartExists(id))
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

        
        [ResponseType(typeof(Cart))]
        [HttpPost]
        public IHttpActionResult AddCart(Cart Cart)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Carts.Add(Cart);
            db.SaveChanges();
            Debug.WriteLine(Cart.date);
            return CreatedAtRoute("DefaultApi", new { id = Cart.CartId }, Cart);
        }
        /// <summary>
        /// Remove specified product from particular cart
        /// </summary>
        /// <param name="cartid">The cart ID primary key</param>
        /// <param name="productid">The product ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/CartData/RemoveProductfromCart/2/1
        /// </example>
        [HttpPost]
        [Route("api/CartData/RemoveProductfromCart/{cartid}/{productid}")]
        public IHttpActionResult RemoveProductfromCart(int cartid, int productid)
        {

            Cart SelectedCart = db.Carts.Include(c => c.Products).Where(c => c.CartId == cartid).FirstOrDefault();
            Product SelectedProduct = db.Products.Find(productid);
            Debug.WriteLine(SelectedProduct);
            if (SelectedCart == null || SelectedProduct == null)
            {
                return NotFound();
            }



            SelectedCart.Products.Remove(SelectedProduct);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// This code will delete the cart from the system by CartId 
        /// </summary>
        /// <param name="id">The primary key of the Cart</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/CartData/DeleteCart/3
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Cart))]
        [HttpPost]
        public IHttpActionResult DeleteCart(int id)
        {
            Cart Cart = db.Carts.Find(id);
            if (Cart == null)
            {
                return NotFound();
            }

            db.Carts.Remove(Cart);
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

        private bool CartExists(int id)
        {
            return db.Carts.Count(e => e.CartId == id) > 0;
        }
    }
}