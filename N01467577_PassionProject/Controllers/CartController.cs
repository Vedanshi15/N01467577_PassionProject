using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Diagnostics;
using N01467577_PassionProject.Models;
using N01467577_PassionProject.Models.ViewModels;
using System.Web.Script.Serialization;

namespace N01467577_PassionProject.Controllers
{
    public class CartController : Controller
    {
            private static readonly HttpClient client;
            private JavaScriptSerializer jss = new JavaScriptSerializer();

            static CartController()
            {
                client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:44374/api/");
            }
        private void GetApplicationCookie()
        {
            string token = "";
            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;
            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);
            return;
        }
        public ActionResult List()
        {
            
            string url = "cartdata/listcarts";
            HttpResponseMessage response = client.GetAsync(url).Result;      
            IEnumerable<CartDto> Cart = response.Content.ReadAsAsync<IEnumerable<CartDto>>().Result;           
            return View(Cart);
        }

        // GET: Cart/Details/5
        public ActionResult Details(int id)
        {
           
            DetailsCart ViewModel = new DetailsCart();

            string url = "cartdata/findCart/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            Debug.WriteLine("The response code is ");
            Debug.WriteLine(response.StatusCode);
            CartDto SelectedCart = response.Content.ReadAsAsync<CartDto>().Result;
            Debug.WriteLine("Received : ");
            Debug.WriteLine(SelectedCart.CustomerName);
            ViewModel.SelectedCart = SelectedCart;
            url = "productdata/listproductsforCart/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<ProductDto> AddedItems = response.Content.ReadAsAsync<IEnumerable<ProductDto>>().Result;
            ViewModel.AddedItems = AddedItems;
            url = "productdata/ListProductsNotaddedtoCart/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<ProductDto> AvailableItems = response.Content.ReadAsAsync<IEnumerable<ProductDto>>().Result;
            ViewModel.AvailableItems = AvailableItems;
            return View(ViewModel);
        }
       
        [HttpGet]
        [Authorize]
        public ActionResult Remove(int id, int ProductId)
        {
            GetApplicationCookie();
            Debug.WriteLine("Attempting to unassociate  :" + id + " with Product: " + ProductId);        
            string url = "cartdata/RemoveProductfromCart/" + id + "/" + ProductId;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;            
            return RedirectToAction("Details/" + id);
        }

        public ActionResult Error()
        {

            return View();
        }
        [Authorize]
        // GET: Cart/New
        public ActionResult New()
        {
            return View();
        }
        [Authorize]
        // POST: Cart/Create
        [HttpPost]
        public ActionResult Create(Cart Cart)
        {
            GetApplicationCookie();
            Debug.WriteLine("the json payload is :");
            Debug.WriteLine(Cart.date);          
            string url = "cartdata/addcart";
            string jsonpayload = jss.Serialize(Cart);
            Debug.WriteLine(jsonpayload);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }


        }
        //POST: Cart/Add/{cartid}
        [HttpPost]
        [Authorize]
        public ActionResult Add(int id, int ProductId)
        {
            GetApplicationCookie();           
            string url = "cartdata/AddProducttoCart/" + id + "/" + ProductId;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;           
            return RedirectToAction("Details/" + id);
        }
        [Authorize]
        // GET: Cart/Edit/5
        public ActionResult Edit(int id)
        {
            string url = "Cartdata/findCart/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            CartDto selectedCart = response.Content.ReadAsAsync<CartDto>().Result;
            Debug.WriteLine(selectedCart.CustomerName);
            return View(selectedCart);
        }
        [Authorize]
        // POST: Cart/Update/5
        [HttpPost]
        public ActionResult Update(int id, Cart Cart)
        {
            GetApplicationCookie();
            string url = "Cartdata/updateCart/" + id;
            string jsonpayload = jss.Serialize(Cart);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
        [Authorize]
        // GET: Cart/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "Cartdata/findCart/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            CartDto selectedCart = response.Content.ReadAsAsync<CartDto>().Result;
            return View(selectedCart);
        }
        [Authorize]
        // POST: Cart/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();
            string url = "Cartdata/deleteCart/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}
