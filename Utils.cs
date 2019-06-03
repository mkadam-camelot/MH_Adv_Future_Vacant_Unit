using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using Newtonsoft.Json;
using System.Net;
using GetAdvRoomFutureVacancy.Classes;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;

namespace GetAdvRoomFutureVacancy
{
    public class Utils
    {
        public static int step = 0;
        #region connstring
        string connstr = "server=camws002.camelot.lan;database=CONTRACT_STREAMLINING;uid=crm_user;pwd=hellocrm;";
        #endregion

        #region Login
        //Log in to MH : Tested : OK
        public string Login()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            LoginCamelot lgnOK = new LoginCamelot();
            try
            {
                string webAddr = "https://mh.trimble-app.uk:443/camelot_prod/wrd/run/SPDEDJSONSERVICE.LOGIN";
                //string webAddr = "https://mh-uat.trimble-app.uk:443/camelot_uat/wrd/run/SPDEDJSONSERVICE.LOGIN";
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                httpWebRequest.ContentType = "text/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.Accept = "text/json";

                userLogin lgn = new userLogin
                {
                    method = "login",
                    username = "camelot_api",
                    password = "C@melot!"
                };
                //   var jsonscrpt = new JavaScriptSerializer().Serialize(lgn);

                var jsonscrpt = JsonConvert.SerializeObject(lgn);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(jsonscrpt);
                    streamWriter.Flush();
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    Console.WriteLine(responseText);

                    try
                    {
                        lgnOK = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginCamelot>(responseText);
                    }
                    catch (Exception exc) { Console.WriteLine(exc.Message); }
                }
                return lgnOK.sessionID;
            }
            catch (WebException ex)
            {
                using(StreamWriter sw = File.AppendText(ConfigurationSettings.AppSettings["logfile"].ToString()))
                {
                    sw.WriteLine("\nexception message :  " + ex.Message + Environment.NewLine + "Exception : " + ex.StackTrace);
                    sw.Close();
                }
                return "";
            }
        }
        #endregion

        #region Logout

        //Log out of MH : Tested : OK
        public void Logout(string session_id)
        {
            try
            {
                string webAddr = "https://mh.trimble-app.uk:443/camelot_prod/wrd/run/SPDEDJSONSERVICE.LOGOUT";
                //string webAddr = "https://mh-uat.trimble-app.uk:443/camelot_uat/wrd/run/SPDEDJSONSERVICE.LOGOUT";
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                httpWebRequest.ContentType = "text/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.Accept = "text/json";

                userLogout lgo = new userLogout
                {
                    method = "logout",
                    sessionID = session_id
                };
                var jsonscrpt = JsonConvert.SerializeObject(lgo);
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(jsonscrpt);
                    streamWriter.Flush();
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    Console.WriteLine(responseText);
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion

        #region clearLocalRecords

        // Clear All Records

        public void ClearAllRecords()
        {
            SqlConnection conn = new SqlConnection();
            SqlCommand cmd = new SqlCommand();
            try
            {
                conn.ConnectionString = connstr;
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "DELETE_RECORD_FOR_ADVRT_ROOM_VACANCY";
                cmd.ExecuteNonQuery();

                using (StreamWriter sw = File.AppendText(ConfigurationSettings.AppSettings["logfile"].ToString()))
                { 
                    sw.WriteLine("All Records Cleared");
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                using(StreamWriter sw = File.AppendText(ConfigurationSettings.AppSettings["logfile"].ToString()))
                {
                    sw.WriteLine("\nexception message :  " + ex.Message + Environment.NewLine + "Exception : " + ex.StackTrace);
                    sw.Close();
                }
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
        }

        #endregion

        #region GetUCamUnitFromMH
        //Get Rooms from MH for vacancy flag
        public int GetUnitsVacancy(int Records)
        {
            
            int id = 1;
            MHUCamUnit.PROFILE prf = new MHUCamUnit.PROFILE();
            MHUCamUnit.getUnitsVacancy lgo = new MHUCamUnit.getUnitsVacancy();
            List<MHUCamUnit.PROFILE> propLst = new List<MHUCamUnit.PROFILE>();
            string[] visibles = { "UN_BREF", "UN_UREF", "VACANCY_FLAG", "TL_LEND" };
            prf.VACANCY_FLAG = "A";
            propLst.Add(prf);

            lgo.sessionID = SessionID.sessionID;
            lgo.FORMAT = "payload";
            lgo.GRIDID = "U_CAM_UNIT";
            lgo.GRIDVIEW = "2";
            lgo.FROM = Records;
            lgo.HITS = 100;
            lgo.PROFILE = propLst;
            lgo.ORDERBY = "UN_UREF";
            lgo.VISIBLE = visibles;

            TrimbleCall t = new TrimbleCall();

            string response = t.trimbleCall(lgo, "SPDEDMHAPI.GRIDGET", "POST");

            MHUCamUnit.Rootobject oRootobject = new MHUCamUnit.Rootobject();
            MHUCamUnit.ErrorResult errorResult = new MHUCamUnit.ErrorResult();
            string oldval = "\"UN_GFA\":\"\"";
            string newVal = "\"UN_GFA\":0";

            response = response.Replace(oldval, newVal);
            try
            {
                oRootobject = JsonConvert.DeserializeObject<MHUCamUnit.Rootobject>(response);
            }
            catch (Exception ex)
            {
                using(StreamWriter sw = File.AppendText(ConfigurationSettings.AppSettings["logfile"].ToString()))
                {
                    sw.WriteLine("\nException at get vacancy room (U_CAM_UNIT)  :  " + ex.Message + Environment.NewLine + "Exception at " + ex.StackTrace + Environment.NewLine + "Response : " + response);
                    sw.Close();
                }
                errorResult = JsonConvert.DeserializeObject<MHUCamUnit.ErrorResult>(response);
            }
            if (oRootobject.Hits > 0)
            {
                foreach (MHUCamUnit.Payload payload in oRootobject.Payload)
                {
                    saveUnitsVacancy(payload);
                }
                using(StreamWriter sw = File.AppendText(ConfigurationSettings.AppSettings["logfile"].ToString()))
                {
                    sw.WriteLine("All data saved in table U_CAM_UNIT (API U_CAM_UNIT) for step " + (++step));
                    sw.Close();
                }
            }
            return oRootobject.Hits;
        }

        public void saveUnitsVacancy(MHUCamUnit.Payload payload)
        {
            SqlConnection conn = new SqlConnection();
            SqlCommand cmd = new SqlCommand();
            try
            {
                conn.ConnectionString = connstr;
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "INSERT_UNIT_VACANCY";

                cmd.Parameters.Add(
                    new SqlParameter("@UN_BREF", payload.UN_BREF));
                cmd.Parameters.Add(
                    new SqlParameter("@UN_UREF", payload.UN_UREF));
                cmd.Parameters.Add(
                    new SqlParameter("@VACANCY_FLAG", payload.VACANCY_FLAG));
                cmd.Parameters.Add(
                    new SqlParameter("@TL_LEND", payload.TL_LEND));

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                using(StreamWriter sw = File.AppendText(ConfigurationSettings.AppSettings["logfile"].ToString()))
                {
                    sw.WriteLine("\nexception message for unit vacancy:  " + payload.UN_UREF + "Message : " + ex.Message + Environment.NewLine + "Exception : " + ex.StackTrace);
                    sw.Close();
                }
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
        }

        #endregion

        #region sendErrMail

        //send Error Email
        public void SendErrorEmail(string err, string desc)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress("m.kadam@cameloteurope.com");
            message.To.Add(ConfigurationSettings.AppSettings["email"].ToString());
            message.CC.Add(new MailAddress("m.kadam@cameloteurope.com"));
            message.Body = "Error Message :" + err + Environment.NewLine + "Error Description :" + desc;
            message.Subject = "Error at vacancy grid get";
            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient("relay.cameloteurope.com");
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Send(message);
        }

        #endregion
    }
}
