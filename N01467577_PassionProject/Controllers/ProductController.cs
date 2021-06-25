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
    public class ProductController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        static ProductController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                UseCookies = false
            };

            client = new HttpClient(handler);
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

        // GET: Product/List  
        public ActionResult List()
        {         
            string url = "productdata/listproducts";
            HttpResponseMessage response = client.GetAsync(url).Result;          
            IEnumerable<ProductDto> p = response.Content.ReadAsAsync<IEnumerable<ProductDto>>().Result;          
            return View(p);
        }
        // GET: product/Details/2
        public ActionResult Details(int id)
        {
            DetailsProduct ViewModel = new DetailsProduct();

            string url = "Productdata/findProduct/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            Debug.WriteLine("The response code is ");
            Debug.WriteLine(response.StatusCode);
            ProductDto SelectedProduct = response.Content.ReadAsAsync<ProductDto>().Result;
            Debug.WriteLine("Product received : ");
            Debug.WriteLine(SelectedProduct.ProductName);
            ViewModel.SelectedProduct = SelectedProduct;
            url = "cartdata/listcartsforproduct/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<CartDto> RelatedCart = response.Content.ReadAsAsync<IEnumerable<CartDto>>().Result;
            ViewModel.RelatedCart = RelatedCart;   
            return View(ViewModel);
        }  
        public ActionResult Error()
        {
            return View();        }

        // GET: Product/New
        [Authorize]
        public ActionResult New()
        {            
            return View();
        }
        // POST: Product/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(Product p, HttpPostedFileBase ProductPic)
        {
            GetApplicationCookie();
            string url = "productdata/addproduct";
            Debug.WriteLine(p.Qty);
            string jsonpayload = jss.Serialize(p);          
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            Debug.WriteLine(jsonpayload);
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(ProductPic); Debug.WriteLine(jsonpayload);
            if (response.IsSuccessStatusCode && ProductPic != null)
            {
                url = "ProductData/UploadProductPic/" + p.ProductId;
                MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                HttpContent imagecontent = new StreamContent(ProductPic.InputStream);
                requestcontent.Add(imagecontent, "ProductPic", ProductPic.FileName);
                response = client.PostAsync(url, requestcontent).Result;
                return RedirectToAction("List");
            }
            else if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }

        }
        // GET: Product/Edit/2
        [Authorize]
        public ActionResult Edit(int id)
        {            
            string url = "productdata/findproduct/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ProductDto selectedProduct = response.Content.ReadAsAsync<ProductDto>().Result;
            return View(selectedProduct);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Update(int id, Product p, HttpPostedFileBase ProductPic)
        {
            GetApplicationCookie();
            string url = "productdata/updateproduct/" + id;
            string jsonpayload = jss.Serialize(p);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(ProductPic);Debug.WriteLine(jsonpayload);
            
            if (response.IsSuccessStatusCode && ProductPic != null)
            {
                url = "ProductData/UploadProductPic/" + id;                
                MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                HttpContent imagecontent = new StreamContent(ProductPic.InputStream);
                requestcontent.Add(imagecontent, "ProductPic", ProductPic.FileName);
                response = client.PostAsync(url, requestcontent).Result;

                return RedirectToAction("List");
            }
            else if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
        // GET: Product/Delete/2
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "productdata/findproduct/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ProductDto selectedProduct = response.Content.ReadAsAsync<ProductDto>().Result;
            return View(selectedProduct);
        }
        // GET: Product/Delete/2
        [Authorize]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();
            string url = "productdata/deleteproduct/" + id;
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
