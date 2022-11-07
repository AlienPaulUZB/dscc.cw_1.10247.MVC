using courseworkMVC.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace courseworkMVC.Controllers
{
    public class ProductController : Controller
    {

        Uri baseAddress = new Uri("http://apifor10247.us-west-1.elasticbeanstalk.com/");
        HttpClient client;
        // GET: Product

        public ProductController()
        {
            client = new HttpClient();
            client.BaseAddress = baseAddress;
        }

        
        public async Task<ActionResult> Index()
        {
            //Hosted web API REST Service Base url
            string Baseurl = "http://apifor10247.us-west-1.elasticbeanstalk.com/";
            List<Product> ProdInfo = new List<Product>();
            using (var client=new HttpClient())
            {
                //Passing service base url
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();

                //Define request data format
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource using HttpClient
                HttpResponseMessage Res = await client.GetAsync("api/Product");

                //Checking the response is successful or not which is sent HttpClient
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details received from web api
                    var PrResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response received from web api and storing the Product List
                    ProdInfo = JsonConvert.DeserializeObject<List<Product>>(PrResponse);

                }
                //returning the Product list to view
                return View(ProdInfo);
            }
        }

        

        // GET: Product/Details/5
        public async Task<ActionResult> DetailsAsync(int id)
    {         
            string Baseurl= "http://apifor10247.us-west-1.elasticbeanstalk.com/";
            Product productDetails = new Product();
            using(var client=new HttpClient())
            {
                //Passing service base url
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();

                //Define request data format
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));


                //Sending request to find web api REST service resource using HttpClient
                HttpResponseMessage Res = await client.GetAsync("api/Product/"+id);
                
                //Checking the response is successful or not which is sent HttpClient
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details received from web api
                    var PrResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response received from web api and storing the Product 
                    productDetails = JsonConvert.DeserializeObject<Product>(PrResponse);
                    

                }

                //returning the Product list to view
                return View(productDetails);
            }

           
    }

    // GET: Product/Create
    public async Task<ActionResult> Create()
    {
            HttpResponseMessage Res = await client.GetAsync("api/Category");
            //Checking the response is successful or not which is sent HttpClient
            if (Res.IsSuccessStatusCode)
            {
                //Storing the response details received from web api
                var PrResponse = Res.Content.ReadAsStringAsync().Result;

                //Deserializing the response received from web api and storing the Product List
                var categoriesList = JsonConvert.DeserializeObject<List<Category>>(PrResponse);
                ViewBag.categories = categoriesList;

            }
            return View();
    }

    // POST: Product/Create
    [HttpPost]
    public async Task<ActionResult> Create(Product product)
    {
            /*
                        Product insertProd = new Product();
                        insertProd.Id = new Random().Next(10,12000);
                        insertProd.Name = product.Name;
                        insertProd.Description = product.Description;
                        insertProd.Price = product.Price;
                        insertProd.ProductCategory = null;

                        Console.WriteLine(insertProd);
                        string json = JsonConvert.SerializeObject(insertProd);

                        */

            HttpResponseMessage Res = await client.GetAsync("api/Category/"+product.ProductCategory.Id);

            if (Res.IsSuccessStatusCode)
            {
                //Storing the response details received from web api
                var PrResponse = Res.Content.ReadAsStringAsync().Result;

                //Deserializing the response 
                var categoryProd = JsonConvert.DeserializeObject<Category>(PrResponse);
                product.ProductCategory = categoryProd;
                
                
            }

            

            string data = JsonConvert.SerializeObject(product);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(client.BaseAddress + "api/Product", content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View();
    }

    // GET: Product/Edit/5
    public async Task<ActionResult> Edit(int id)
    {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://apifor10247.us-west-1.elasticbeanstalk.com/");
            Product product = new Product();
            HttpResponseMessage Res = client.GetAsync(client.BaseAddress + "api/Product/" + id).Result;
            if (Res.IsSuccessStatusCode)
            {
                string data = Res.Content.ReadAsStringAsync().Result;
                product = JsonConvert.DeserializeObject<Product>(data);
            }

            HttpResponseMessage Resp = await client.GetAsync("api/Category");
            //Checking the response is successful or not which is sent HttpClient
            if (Resp.IsSuccessStatusCode)
            {
                //Storing the response details received from web api
                var PrResponse = Resp.Content.ReadAsStringAsync().Result;

                //Deserializing the response received from web api and storing the Product List
                var categoriesList = JsonConvert.DeserializeObject<List<Category>>(PrResponse);
                ViewBag.categories = categoriesList;

            }
            return View("Edit",product);
    }

    // POST: Product/Edit/5
    [HttpPost]
    public async Task<ActionResult> Edit(Product product)
    {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://apifor10247.us-west-1.elasticbeanstalk.com/");

            //get categories list for viewbag
            HttpResponseMessage Res = await client.GetAsync("api/Category/" + product.ProductCategory.Id);
            if (Res.IsSuccessStatusCode)
            {
                //Storing the response details received from web api
                var PrResponse = Res.Content.ReadAsStringAsync().Result;

                //Deserializing the response 
                var categoryProd = JsonConvert.DeserializeObject<Category>(PrResponse);
                product.ProductCategory = categoryProd;


            }


            string data = JsonConvert.SerializeObject(product);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PutAsync(client.BaseAddress + "api/Product/" + product.Id, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View();

    }

    // GET: Product/Delete/5
    public ActionResult Delete(int id)
    {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://apifor10247.us-west-1.elasticbeanstalk.com/");
                var deleteProduct = client.DeleteAsync("api/Product/" + id.ToString());

               HttpResponseMessage response = deleteProduct.Result;
                if (response.IsSuccessStatusCode)
                    return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        
    }

    // POST: Product/Delete/5
    [HttpPost]
    public ActionResult Delete(int id, FormCollection collection)
    {
                try
                {
                RedirectToAction("Index");
                }
                catch
                {
                    return View();
                }

            return View();
           
        }

    
    }
}


