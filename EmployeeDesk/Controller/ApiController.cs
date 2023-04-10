using EmployeeDesk.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDesk.Controller
{
    public class ApiController
    {
        public static Task<HttpResponseMessage> GetData(string url)
        {
            try
            {

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                //string apiUrl = ApiUrls.baseURI + url;
                //using (HttpClient client = new HttpClient())
                //{
                //    client.BaseAddress = new Uri(apiUrl);
                //    client.Timeout = TimeSpan.FromSeconds(900);
                //    client.DefaultRequestHeaders.Accept.Clear();
                //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigurationManager.AppSettings["apiToken"]);
                //    var response = client.GetAsync(apiUrl);
                //    response.Wait();
                //    return response;

                //}
                
                using(HttpClient client= JsonHelper.GetHttpClient(url))
                {
                    var response = client.GetAsync(ApiUrls.baseURI + url);
                    response.Wait();
                    return response;
                }


            }
            catch (Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// creates a new employee
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> PostData<T>(string url, T model)
        {
            try
            {
                using (HttpClient client = JsonHelper.GetHttpClient(url))                
                using (var request = new HttpRequestMessage(HttpMethod.Post, ApiUrls.baseURI + url))
                using (var httpContent = JsonHelper.CreateHttpContent(model))
                {                    
                    var response = client.PostAsync(ApiUrls.baseURI + url, httpContent);
                    response.Wait();
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        /// <summary>
        /// Updates emplyees details
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> PutData<T>(string url, T model)
        {
            try
            {
                using (HttpClient client = JsonHelper.GetHttpClient(url))
                using (var request = new HttpRequestMessage(HttpMethod.Post, ApiUrls.baseURI + url))
                using (var httpContent = JsonHelper.CreateHttpContent(model))
                {                   
                    var response = client.PutAsync(ApiUrls.baseURI + url, httpContent);
                    response.Wait();
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Delete employee's record
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> DeleteData(string url)
        {
            try
            {               
                string apiUrl = ApiUrls.baseURI + url;
                using (HttpClient client = JsonHelper.GetHttpClient(url))
                {                   
                    var response = client.DeleteAsync(apiUrl);
                    response.Wait();
                    return response;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }        

    }
}
