using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

namespace AmadeusAI.Scanner
{    //we will have to make another designer for this


    internal class IpScanner
    { //this is generated code i had no idea where to start but as i gain experience this will change
        public void Onscan(string onfeed, int re_enterbit)
        { //if ip4 selected option in xaml popup from tray brings up a new grid which can be opened closed (weather menu)
          //then a combobox is dropped
          //re_enterbit int32; //reeneter bit is outr user defined parameter to reeneter for ip4
          //ipv6 also
          //refresh scan to attempt to reconnect scanner for information
          //idk if this should be declared public
          //here for now we will incorportate a refresh method every so often so an attempt to regain info is possible

        }
        
        public async Task<string> GetLocalIPAddressAsync()
        {
                
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {

                    //here we pass the country method i specifically used Newtronsoft Json to do this
                    string ipAddress = ip.ToString();
                    string country = await GetCountryFromIPAsync(ipAddress);
                    return ipAddress;

                    //pass the ip as a constructor to another file
                    //if we check for a vpn we will issue warning but still pass the ip to get exclusive information
                    //we will also allow the user to input a set random ip addresses to find weather realted information round the world
                }
            }
            throw new Exception("No network adapters with an IPv4 address" +
                "in the system, cannot display correct information");
            //setting {weather} to null in AmadeusAISettings.settings
            //we will scan for an ipv6 also as this is higher bit 128

        }

        public async Task<string> GetCountryFromIPAsync(string ipAddress)
        {
            using (var client = new HttpClient())
            {
                string url = $"https://ipgeolocationapi.com/api/{ipAddress}"; //maybe try another link
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<dynamic>(json);
                    return data.country_name;
                }
                else
                {
                    return "Unknown Location";
                }
            }
        }
        
    }
}

   

//if this file fails basically garbage collect it till the damn vpn goes


