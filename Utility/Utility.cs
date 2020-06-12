namespace nIS
{
    #region References

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Mail;
    //using Websym.Core.ConfigurationManager;
    //using Websym.Core.ResourceManager;
    //using Websym.Core.EventManager;
    //using Websym.Core.NotificationEngine;
    using System.Reflection;
    using System.Text;
    using Websym.Core.ConfigurationManager;

    //using Microsoft.Practices.Unity;

    #endregion

    public class Utility : IUtility
    {
        /// <summary>
        /// This method will helps to get description of perticular entity of class.
        /// </summary>
        /// <param name="propertyName">
        /// The property name
        /// </param>
        /// <param name="entityType">
        /// The type of entity.
        /// </param>
        /// <returns>
        /// It returns a string.
        /// </returns>
        public string GetDescription(string propertyName, Type entityType)
        {
            var property = entityType.GetProperty(propertyName);
            var attribute = property.GetCustomAttributes(typeof(DescriptionAttribute), true)[0];
            var description = (DescriptionAttribute)attribute;
            return description.Description;
        }


        /// <summary>
        /// This method will helps to get enum key valuee pair of perticular entity of class.
        /// </summary>
        /// <param name="propertyName">
        /// The property name
        /// </param>
        /// <param name="entityType">
        /// The type of entity.
        /// </param>
        /// <returns>
        /// It returns a string.
        /// </returns>
        public List<KeyValuePair<string, int>> GetEnumKeyValue(Enum entityType)
        {
            IList<KeyValuePair<string, int>> keyValuePairEnum = new List<KeyValuePair<string, int>>();
            foreach (var value in Enum.GetValues(entityType.GetType()))
            {
                FieldInfo fi = value.GetType().GetField(value.ToString());
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes != null && attributes.Length > 0)
                    keyValuePairEnum.Add(new KeyValuePair<string, int>(attributes[0].Description, (int)value));
                else
                    keyValuePairEnum.Add(new KeyValuePair<string, int>(value.ToString(), (int)value));
            }
            return keyValuePairEnum.ToList();
        }

        /// <summary>
        /// This method executes the web request using the specified parameters.
        /// </summary>
        /// <param name="instanceURL">The instance URL.</param>
        /// <param name="controller">The controller.</param>
        /// <param name="action">The action.</param>
        /// <param name="objectData">The object data.</param>
        /// <param name="tenantKey">The tenant key.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <param name="toBeSerailzied">This property should be set to be true if passing object data as primitive data type.</param>
        /// <returns>
        /// Returns the response object
        /// </returns>
        public string ExecuteWebRequest(string instanceURL, string controller, string action, string objectData, string tenantKey, string tenantCode, bool toBeSerailzied = false)
        {
            string responseFromServer = string.Empty;
            try
            {
                WebRequest request = WebRequest.Create(instanceURL + "/" + controller + "/" + action);
                HttpWebResponse response = null;
                request.Headers.Add(tenantKey, tenantCode);
                request.Method = "POST";
                string postData = toBeSerailzied ? JsonConvert.SerializeObject(objectData) : objectData;
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request.ContentType = "application/json";
                request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                    dataStream = response.GetResponseStream();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        if (dataStream != null)
                        {
                            StreamReader reader = new StreamReader(dataStream);
                            responseFromServer = reader.ReadToEnd();
                            reader.Close();
                            dataStream.Close();
                        }
                    }
                }
                catch (WebException webException)
                {
                    response = (HttpWebResponse)webException.Response;
                    dataStream = response.GetResponseStream();
                    if (dataStream != null)
                    {
                        StreamReader reader = new StreamReader(dataStream);
                        responseFromServer = reader.ReadToEnd();
                        reader.Close();
                        dataStream.Close();

                        JObject jObject = JsonConvert.DeserializeObject<JObject>(responseFromServer);
                        throw new Exception(jObject["Error"]["Message"].ToString());
                    }
                }

                return responseFromServer;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method gets the configuration values from configuration manager component.
        /// </summary>
        /// <param name="configurationSearchParameter">The configuration search parameter.</param>
        /// <param name="configurationBaseURLKey">The configuration base URL key.</param>
        /// <param name="tenantKey">The tenant key.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns the list of configuration section
        /// </returns>
        public IList<Websym.Core.ConfigurationManager.ConfigurationSection> GetConfigurationValues(ConfigurationSearchParameter configurationSearchParameter, string configurationBaseURLKey, string tenantKey, string tenantCode)
        {

            IList<Websym.Core.ConfigurationManager.ConfigurationSection> configurationSectionList = null;
            try
            {

                return configurationSectionList;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method gets the connection string from configuration manager as per the specified key.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="configurationKey">The configuration key.</param>
        /// <param name="configurationBaseURLKey">The configuration base URL key.</param>
        /// <param name="tenantKey">The tenant key.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns the connection string for the specified configuration key(s).
        /// </returns>
        public string GetConnectionString(string section, string configurationKey, string configurationBaseURLKey, string tenantKey, string tenantCode)
        {
            string sqlConnectionString = string.Empty;
            try
            {
                //return System.Configuration.ConfigurationManager.ConnectionStrings["FMSEntitiesDataContext"].ConnectionString;
                ConfigurationSearchParameter configurationSearchParameter = new ConfigurationSearchParameter();
                configurationSearchParameter.SectionName = section;
                configurationSearchParameter.ConfigurationKey = configurationKey;

                IList<Websym.Core.ConfigurationManager.ConfigurationSection> configurationSectionList = this.GetConfigurationValues(configurationSearchParameter, configurationBaseURLKey, tenantKey, tenantCode);
                if (configurationSectionList != null && configurationSectionList.Count > 0)
                {
                    if (configurationSectionList[0].ConfigurationItems != null && configurationSectionList[0].ConfigurationItems.Count > 0)
                    {
                        sqlConnectionString = configurationSectionList[0].ConfigurationItems[0].Value;
                    }
                }

                sqlConnectionString = sqlConnectionString.EndsWith(";") ? sqlConnectionString : sqlConnectionString + ";";
                // sqlConnectionString = "metadata=res://*/nVidYoDataContext.csdl|res://*/nVidYoDataContext.ssdl|res://*/nVidYoDataContext.msl;provider=System.Data.SqlClient;provider connection string=';Data Source=192.168.100.7;Initial Catalog=nvidyo;User ID=sa;Password=Admin@123;multipleactiveresultsets=True;application name=EntityFramework';";

                sqlConnectionString = @"metadata=res://*/nVidYoDataContext.csdl|res://*/nVidYoDataContext.ssdl|res://*/nVidYoDataContext.msl;provider=System.Data.SqlClient;provider connection string=';" + sqlConnectionString + "multipleactiveresultsets=True;application name=EntityFramework';";
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return sqlConnectionString;
        }

        /// <summary>
        /// This method gets this list of localized resources as per the specified search parameter
        /// </summary>
        /// <param name="resourceSearchParameter">The resource search parameter.</param>
        /// <param name="resourceBaseURLKey">The resource base URL key.</param>
        /// <param name="tenantKey">The tenant key.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns the list of resources for a particular locale
        /// </returns>
        //public IList<Resource> GetResources(ResourceSearchParameter resourceSearchParameter, string resourceBaseURLKey, string tenantKey, string tenantCode)
        //{
        //    IList<Resource> resources = null;
        //    try
        //    {
        //        string resourceBaseURL = System.Configuration.ConfigurationManager.AppSettings[resourceBaseURLKey];
        //        resources = JsonConvert.DeserializeObject<List<Resource>>(this.ExecuteWebRequest(resourceBaseURL, "Resource", "Get", JsonConvert.SerializeObject(resourceSearchParameter), tenantKey, tenantCode.ToString()));

        //        return resources;
        //    }
        //    catch (Exception exception)
        //    {
        //        throw exception;
        //    }
        //}

        /// <summary>
        /// This method gets this list of localized resources for cshtml as per the specified search parameter
        /// </summary>
        /// <param name="culture">The culture name</param>
        /// <param name="sectionName">The UI section name</param>
        /// <param name="resourceBaseUrl">The base url</param>
        /// <param name="tenantKey">The Tenant key</param>
        /// <param name="defaultTenant">The default tenant code.</param>
        /// <returns></returns>
        public Dictionary<string, string> GetResourcesForUI(string culture, string sectionName, string resourceBaseUrl, string tenantKey, string defaultTenant)
        {
            Dictionary<string, string> resourceItems = new Dictionary<string, string>();
            //try
            //{
            //    ResourceSearchParameter resourceSearchParameter = new ResourceSearchParameter();
            //    resourceSearchParameter.Locale = culture;
            //    resourceSearchParameter.SectionName = sectionName;
            //    IList<Resource> resourceList = this.GetResources(resourceSearchParameter, resourceBaseUrl, tenantKey, defaultTenant);
            //    if (resourceList.Count > 0)
            //    {
            //        resourceList.ToList().ForEach(section => section.ResourceSections.ToList()
            //        .ForEach(item => item.ResourceItems.ToList()
            //        .ForEach(data => resourceItems.Add(data.Key, data.Value))));
            //    }
            //}
            //catch (Exception)
            //{
            //    throw;
            //}

            return resourceItems;
        }

        /// <summary>
        /// This method helps to send mail.
        /// </summary>
        /// <param name="mail">MailMessage object</param>
        /// <param name="applicationSMTPClientHost">Application smtp client host</param>
        /// <param name="applicationSMTPClientPort">Application smtp client port</param>
        /// <param name="applicationEmailPassword">Application email password</param>
        /// <param name="tenantCode">The tenant code</param>
        public void SendMail(MailMessage mail, string applicationSMTPClientHost, int applicationSMTPClientPort, string applicationEmailPassword, string tenantCode)
        {
            try
            {
                var emailFromAddress = System.Configuration.ConfigurationManager.AppSettings["FromEmailAddress"];
                var displayName = System.Configuration.ConfigurationManager.AppSettings["MailDisplayName"];
                var password = System.Configuration.ConfigurationManager.AppSettings["FromEmailAddressPassword"];
                bool enableSSL = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["EnableSSL"]);
                var smtpAddress = System.Configuration.ConfigurationManager.AppSettings["SMTPServer"];
                var portNumber = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SMTPPort"]);
                //string smtpAddress = "smtp.gmail.com";
                //int portNumber = 587;
                //bool enableSSL = true;
                //string emailFromAddress = "nvidyo@n4mative.net";
                //string password = "Gauch022";
                //string displayName = "nVidYo Team";

                mail.From = new MailAddress(emailFromAddress, displayName);

                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.EnableSsl = enableSSL;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(emailFromAddress, password);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Send(mail);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        #region Platform 

        /// <summary>
        /// This method implements HTTP posts request.
        /// </summary>
        /// <param name="baseURL">The base URL.</param>
        /// <param name="actionPath">The action path.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="timeout">The Time out in milliseconds.</param>
        /// <param name="contentType">The MIME type for request data.</param>
        /// <returns>HttpResponseMessage.</returns>
        public HttpResponseMessage HttpPostRequest(string baseURL, string actionPath, object parameters = null, IDictionary<string, string> headersDictionary = null, double timeout = 0)
        {
            try
            {
                HttpClient client = new HttpClient()
                {
                    Timeout = timeout > 0 ? TimeSpan.FromMilliseconds(timeout) : TimeSpan.FromMilliseconds(100000)
                };

                if (headersDictionary?.Count > 0)
                {
                    foreach (KeyValuePair<string, string> header in headersDictionary)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                client.BaseAddress = new Uri(baseURL);
                HttpResponseMessage response = null;
                // response = client.PostAsync(baseURL + actionPath, new Content(parameters)).Result;
                //response = client.PostAsJsonAsync(baseURL + actionPath, parameters).Result;
                string responseString = response.Content.ReadAsStringAsync().Result;

                return response;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method implements HTTP posts request.
        /// </summary>
        /// <param name="baseURL">The base URL.</param>
        /// <param name="actionPath">The action path.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="timeout">The Time out in milliseconds.</param>
        /// <param name="contentType">The MIME type for request data.</param>
        /// <returns>HttpResponseMessage.</returns>
        public HttpResponseMessage HttpPostRequestEncodedContent(string baseURL, string actionPath, string parameters = null, IDictionary<string, string> headersDictionary = null, double timeout = 0)
        {
            try
            {
                HttpClient client = new HttpClient()
                {
                    Timeout = timeout > 0 ? TimeSpan.FromMilliseconds(timeout) : TimeSpan.FromMilliseconds(100000)
                };

                if (headersDictionary?.Count > 0)
                {
                    foreach (KeyValuePair<string, string> header in headersDictionary)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                }

                client.BaseAddress = new Uri(baseURL);
                HttpResponseMessage response = null;
                using (StringContent content = new StringContent(parameters, Encoding.Default, "application/x-www-form-urlencoded"))
                {
                    response = client.PostAsync(baseURL + actionPath, content).Result;
                    //response = client.PostAsJsonAsync(baseURL + actionPath, parameters).Result;
                    string responseString = response.Content.ReadAsStringAsync().Result;
                }
                return response;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method implements HTTP posts request.
        /// </summary>
        /// <param name="baseURL">The base URL.</param>
        /// <param name="actionPath">The action path.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="timeout">The Time out in milliseconds.</param>
        /// <param name="contentType">The MIME type for request data.</param>
        /// <returns>HttpResponseMessage.</returns>
        public HttpResponseMessage HttpPutRequest(string baseURL, string actionPath, object parameters = null, IDictionary<string, string> headersDictionary = null, double timeout = 0)
        {
            try
            {
                HttpClient client = new HttpClient()
                {
                    Timeout = timeout > 0 ? TimeSpan.FromMilliseconds(timeout) : TimeSpan.FromMilliseconds(100000)
                };

                if (headersDictionary?.Count > 0)
                {
                    foreach (KeyValuePair<string, string> header in headersDictionary)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                client.BaseAddress = new Uri(baseURL);
                HttpResponseMessage response = null;
                //response = client.PutAsJsonAsync(baseURL + actionPath, parameters).Result;
                return response;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method implements HTTP posts request.
        /// </summary>
        /// <param name="baseURL">The base URL.</param>
        /// <param name="actionPath">The action path.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="timeout">The Time out in milliseconds.</param>
        /// <param name="contentType">The MIME type for request data.</param>
        /// <returns>HttpResponseMessage.</returns>
        public HttpResponseMessage HttpPostRequestByUrlEncodedContent(string baseURL, string actionPath, object parameters = null, IDictionary<string, string> headersDictionary = null, double timeout = 0)
        {
            try
            {
                HttpClient client = new HttpClient()
                {
                    Timeout = timeout > 0 ? TimeSpan.FromMilliseconds(timeout) : TimeSpan.FromMilliseconds(100000)
                };

                IList<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>>();
                if (headersDictionary?.Count > 0)
                {
                    foreach (KeyValuePair<string, string> header in headersDictionary)
                    {
                        pairs.Add(new KeyValuePair<string, string>(header.Key, header.Value));
                        //client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                //var pairs = new List<KeyValuePair<string, string>>
                //{
                //    new KeyValuePair<string, string>("grant_type", "password"),
                //    new KeyValuePair<string, string>("Client_Id", ModelConstants.DEFAULTTENANTVALUE),
                //    new KeyValuePair<string, string>("UserName", model.Email),
                //    new KeyValuePair<string, string>("password", model.Password)
                //};

                FormUrlEncodedContent content = new FormUrlEncodedContent(headersDictionary);
                client.BaseAddress = new Uri(baseURL);
                HttpResponseMessage response = null;
                response = client.PostAsync(baseURL + actionPath, content).Result;
                //response = client.PostAsync(baseURL + actionPath, parameters, new FormUrlEncodedMediaTypeFormatter() { }/*new MediaTypeFormatter() { }*/).Result;

                return response;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method implements HTTP get request.
        /// </summary>
        /// <param name="baseURL">The base URL.</param>
        /// <param name="actionPath">The action path.</param>
        /// <param name="timeout">The Time out in milliseconds.</param>
        /// <returns>HttpResponseMessage.</returns>
        public HttpResponseMessage HttpGetRequest(string baseURL, string actionPath, object parameters = null, IDictionary<string, string> headersDictionary = null, double timeout = 0)
        {
            try
            {
                HttpClient client = new HttpClient()
                {
                    Timeout = timeout > 0 ? TimeSpan.FromMilliseconds(timeout) : TimeSpan.FromMilliseconds(100000)
                };

                if (headersDictionary?.Count > 0)
                {
                    foreach (KeyValuePair<string, string> header in headersDictionary)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                client.BaseAddress = new Uri(baseURL);
                HttpResponseMessage response = client.GetAsync(baseURL + actionPath + ((parameters != null) && (parameters is string) && (!string.IsNullOrWhiteSpace((string)parameters)) ? (string)parameters : "")).Result;
                return response;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        //public IList<Websym.Core.EventManager.Event> AddUserNotificationSubscription(EventSearchParameter eventSearchParameter, DeliveryMode deliveryMode, string userIdentifier, string contactNumber, string emailAddress, string tenantCode)
        //{
        //    try
        //    {
        //        string eventManagerBaseURL = System.Configuration.ConfigurationManager.AppSettings["EventManagerBaseURL"]?.ToString();
        //        string subscriptionManagerBaseURL = System.Configuration.ConfigurationManager.AppSettings["SubscriptionManagerBaseURL"]?.ToString();
        //        IDictionary<string, string> headerValues = new Dictionary<string, string>();
        //        headerValues.Add("TenantCode", tenantCode);

        //        HttpResponseMessage response = this.HttpPostRequest(eventManagerBaseURL, "event/get", eventSearchParameter, headerValues);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            IList<Websym.Core.EventManager.Event> eventList = JsonConvert.DeserializeObject<IList<Websym.Core.EventManager.Event>>(response.Content.ReadAsStringAsync().Result);
        //            if (eventList == null && eventList.Count() == 0)
        //            {
        //                throw new Exception("Event list not found.");
        //            }

        //            IList<Subscription> subscriptions = eventList?.Select(eventDetail => new Subscription()
        //            {
        //                ComponentCode = eventDetail.ComponentCode,
        //                EntityName = eventDetail.EntityName,
        //                EventCode = eventDetail.EventCode,
        //                UserIdentifier = userIdentifier,
        //                MobileNumber = contactNumber,
        //                EmailAddress = emailAddress,
        //                DeliveryMode = deliveryMode,
        //                IsActive = true
        //            })
        //            .ToList();

        //            response = this.HttpPostRequest(subscriptionManagerBaseURL, "subscription/add", subscriptions, headerValues);
        //            if (response.IsSuccessStatusCode)
        //            {
        //                return eventList;
        //            }
        //        }

        //        return null;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public bool SendNotification(EventContext eventContext, DeliveryMode deliveryMode, string tenantCode)
        //{
        //    try
        //    {
        //        string notificationManagerBaseURL = System.Configuration.ConfigurationManager.AppSettings["NotificationEngineApiUrl"]?.ToString();
        //        IDictionary<string, string> headerValues = new Dictionary<string, string>();
        //        headerValues.Add("TenantCode", tenantCode);

        //        HttpResponseMessage response = this.HttpPostRequest(notificationManagerBaseURL, "notification/ProcessNotification?deliveryMode=" + deliveryMode.ToString(), eventContext, headerValues);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            return true;
        //        }

        //        return false;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        #endregion       

    }
}