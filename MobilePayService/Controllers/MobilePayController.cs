using MobilePayService.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace MobilePayService.Controllers
{
   
    public class MobilePayController : ApiController
    {
        // GET: Test        
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("MobilePayIndex/PostInvoice")]
        [Authentication.BasicAuthentication]
        public IHttpActionResult PostInvoice([FromBody]List<InvoiceModel> lstInvoice)
        {
            string key = null;
            //string returnmsg = "";
            StringBuilder returnmsg = new StringBuilder();
            MobilePayIndexController mobilepayindex = new MobilePayIndexController();


            if (lstInvoice == null)
                return Content(HttpStatusCode.BadRequest,"Invalid parameters!");
            try
            {
                var settings = new JsonSerializerSettings();
                settings.NullValueHandling = NullValueHandling.Ignore;
                settings.DefaultValueHandling = DefaultValueHandling.Ignore;
                var responsebody = JsonConvert.SerializeObject(lstInvoice, settings);
                var highestDate = lstInvoice.Max(T => T.Date);

                lstInvoice.ForEach(invoice => {
                    if (highestDate == invoice.Date)
                    {
                        returnmsg.Append("InvoiceId: ").Append(invoice.InvoiceId).Append("  ReturnMessage: ");
                        returnmsg.Append(mobilepayindex.PostInvoice(invoice, responsebody));
                        returnmsg.AppendLine();
                    }
                    
                });
            }
            catch (WebException ex)
            {                
                returnmsg.Append(ex.Message);           
                if (((System.Net.HttpWebResponse)ex.Response).StatusCode.ToString().Equals("Unauthorized"))
                    return Content(HttpStatusCode.Unauthorized, returnmsg.ToString());

            }
            catch (Exception ex)
            {
                returnmsg.Append(ex.Message);
                return Content(HttpStatusCode.BadRequest, returnmsg);
            }

            
            return Content(HttpStatusCode.OK, returnmsg);
        }
    }
}