using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Configuration;

namespace GetAdvRoomFutureVacancy
{
    public static class SessionID
    {
        public static string sessionID { get; set; }
    }

    public class TrimbleCall
    {
        public string trimbleCall(object jsonObj, string method, string postGet)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                //string webAddr = "https://mh-uat.trimble-app.uk:443/camelot_uat/wrd/run/" + method;
                string webAddr = "https://mh.trimble-app.uk:443/camelot_prod/wrd/run/" + method;
                string responseText = "";
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                httpWebRequest.ContentType = "text/json; charset=utf-8";
                httpWebRequest.Method = postGet;
                httpWebRequest.Accept = "text/json";
                //httpWebRequest.Timeout = 6000000;
                //httpWebRequest.ContinueTimeout = 6000000;
                var jsonscrpt = JsonConvert.SerializeObject(jsonObj);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(jsonscrpt);
                    streamWriter.Flush();
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    responseText = streamReader.ReadToEnd();
                    //Console.WriteLine(responseText);
                }
                return responseText;
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                return "Error";
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Utils utls = new Utils();
            int Records = 0;
            using (StreamWriter sw = File.AppendText(ConfigurationSettings.AppSettings["logfile"].ToString()))
            {
                sw.WriteLine("********************Execution Starts***********************");
                sw.WriteLine("\nExecution Starts at :  " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                sw.Close();
            }
            try
            {
                //Log in to MH
                SessionID.sessionID = utls.Login();
                if (SessionID.sessionID != "")
                {
                    using (StreamWriter sw = File.AppendText(ConfigurationSettings.AppSettings["logfile"].ToString()))
                    {
                        sw.WriteLine("Sessio ID : " + SessionID.sessionID);
                        sw.Close();
                    }
                    
                    // Clear all Records
                    utls.ClearAllRecords();

                    //Get unit vacancy details
                    for (int i = 0; i < 10000; i = i + 100)
                    {
                        Records = utls.GetUnitsVacancy(i);
                        if (Records != 100)
                            break;
                    }
                    
                    //Log out from MH
                    utls.Logout(SessionID.sessionID);

                    using (StreamWriter sw = File.AppendText(ConfigurationSettings.AppSettings["logfile"].ToString()))
                    {
                        sw.WriteLine("\nExecution Ends at :  " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                        sw.WriteLine("*************************Execution Ends***********************");
                        sw.Close();
                    }
                }
            }
            catch(Exception e)
            {
                using(StreamWriter sw = File.AppendText(ConfigurationSettings.AppSettings["logfile"].ToString()))
                {
                    sw.WriteLine("\nexception at :  " + e.StackTrace + Environment.NewLine + "Exception Description : " + e.Message);
                    sw.Close();
                }
                
                utls.SendErrorEmail(e.StackTrace, e.Message);
            }
        }
    }
}
