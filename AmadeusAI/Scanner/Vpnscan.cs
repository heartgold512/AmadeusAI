using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Management; //?
using System.Windows;
using System.Windows.Controls;
using AmadeusAI.Scanner;
using System.Collections.ObjectModel;
using System.Management;
using System.Windows;
using System.Windows.Controls;
using AmadeusAI; //?
namespace AmadeusAI.Scanner
{ //vpn mode contains literally most things in ipscanner 
  //but is detectable as a vpn upon first login
  //31st Jan yes i took a haiatus due to college and exams for a while i suppose i also found a pretty intresting stack post
  //man its Febuary tomorrow.... i havnt even got a girlfriend nevermind a irl friend who isnt family or considered family to me
  //people are very stranger like now, even my nanan has noticed that things and people have become worse and withdrawn
  //I am the autistic one but everyone else seems very withdrawn i attempt to talk to people , girls included but i get shunned
  //yes this is sad though i suppose this itself is some form of memory for me having written down these comments to look back on in the future whatever state i find my self in
  // though im more worried of the world being uninhabitable by the late 2030s than anything else.

    //thank you very much: https://stackoverflow.com/questions/42435970/detect-what-vpn-im-connected-to
    public class Vpnscan 
    {

        //we will inherit the method
        public ListView list_route;
        public ObservableCollection<string[]> RouteData { get; private set; }
        public bool IsVpnPresent { get; private set; }
        


        public Vpnscan(ListView listView)
        {
            list_route = listView;
            RouteData = new ObservableCollection<string[]>();
            list_route.ItemsSource = RouteData;
            //if no data is present 
            IsVpnPresent = LoadData();
        }



        public bool LoadData()
        {
            try
            {
                //this preerable should be rand every so often lets say 5 or so minutes this function is called from the main or something
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_IP4RouteTable");

                foreach (ManagementObject queryObj in searcher.Get())
                {
                    string destination = queryObj["Destination"].ToString();
                    string mask = queryObj["Mask"].ToString();
                    string metric = queryObj["Metric1"].ToString();
                    string interfaceIndex = queryObj["InterfaceIndex"].ToString();
                    string nexthop = queryObj["NextHop"].ToString();
                    string protocol = queryObj["Protocol"].ToString();
                    string type = queryObj["Type"].ToString();
                    string status = queryObj["Status"] != null ? queryObj["Status"].ToString() : string.Empty;

                    RouteData.Add(new string[] { destination, mask, metric, interfaceIndex, nexthop, protocol, status, type });
                }
                return true;
            }
            catch (ManagementException ex)
            {
                
                return false;
            }
            catch (Exception ex)
            {
                // Handle other types of exceptions...
                System.Windows.MessageBox.Show("An unexpected error occurred: " + ex.Message);
                return false;
            }
        }
    }
}
