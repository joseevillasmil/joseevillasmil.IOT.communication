using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace joseevillasmil.IOT.Communication
{
    public class LanClient
    {
        private string _endpoint = "";
        private string _privateKey = "";
        private string _token = "";

        public LanClient(string endpoint, string privateKey)
        {
            this._privateKey = privateKey;
            this._endpoint = $"http://{endpoint}";

            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = client.GetAsync($"{_endpoint}/token?key={_privateKey}").GetAwaiter().GetResult();
                string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Response obj = JsonSerializer.Deserialize<Response>(responseBody);
                this._token = obj.response;

            }
            catch (Exception e)
            {

            }
        }

        public void updateToken()
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = client.GetAsync($"{_endpoint}/token?key={_privateKey}").GetAwaiter().GetResult();
                string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Response obj = JsonSerializer.Deserialize<Response>(responseBody);
                this._token = obj.response;

            }
            catch (Exception e)
            {

            }
        }

        public bool TurnOn()
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = client.GetAsync($"{_endpoint}/accion/encender?token={_token}").GetAwaiter().GetResult();
                string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Response obj = JsonSerializer.Deserialize<Response>(responseBody);
                if(String.Equals(obj.response, "ok"))
                {
                    return true;
                }

            }
            catch (Exception e)
            {
            }

            return false;
        }

        public bool TurnOff()
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = client.GetAsync($"{_endpoint}/accion/apagar?token={_token}").GetAwaiter().GetResult();
                string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Response obj = JsonSerializer.Deserialize<Response>(responseBody);

                if (String.Equals(obj.response, "ok"))
                {
                    return true;
                }

            }
            catch (Exception e)
            {

            }

            return false;
        }

        public string State()
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = client.GetAsync($"{_endpoint}/accion/estado?token={_token}").GetAwaiter().GetResult();
                string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Response obj = JsonSerializer.Deserialize<Response>(responseBody);
                return obj.response;
            }
            catch (Exception e)
            {

            }

            return "";
        }

    }

    public class Response {
        public string response { get; set; }
    }
}
