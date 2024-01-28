namespace nIS
{
    #region References

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using SelectPdf;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
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
    using PuppeteerSharp;
    using System.Threading.Tasks;

    //using Microsoft.Practices.Unity;


    #endregion

    public class Utility : IUtility
    {
        private static int CreateAndWriteToZipFileCount = 0;
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

        public static string FormatCurrency(string input)
        {
            if(input.Contains("R"))
                return input;

            decimal amount = decimal.Parse(input, System.Globalization.CultureInfo.InvariantCulture); // Parse the string to a decimal

            string formattedAmount = amount.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-ZA"));
            return (amount < 0 ? "-" : "") + formattedAmount;
        }

        public static string FormatCurrency(double amount)
        {
            string formattedAmount = amount.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-ZA"));
            return (amount < 0 ? "-" : "") + formattedAmount;
        }

        //ILog _log = log4net.LogManager.GetLogger(typeof(Utility));

        //ILog _schdeuleLog = log4net.LogManager.GetLogger("RunSchedule");

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

                //sqlConnectionString = @"metadata=res://*/nVidYoDataContext.csdl|res://*/nVidYoDataContext.ssdl|res://*/nVidYoDataContext.msl;provider=System.Data.SqlClient;provider connection string=';" + sqlConnectionString + "multipleactiveresultsets=True;application name=EntityFramework';";
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
        /// <summary>
        /// This method help to write html string to actual file
        /// </summary>
        /// <param name="Message"> the message string </param>
        /// <param name="fileName"> the file name </param>
        /// <param name="batchId"> the batch identifier </param>
        /// <param name="customerId"> the customer identifier </param>
        public string WriteToFile(string Message, string fileName, string scheduleName, string batchName, long customerId, string baseURL, string outputLocation, bool printPdf = false, string headerHtml = "", string footerHtml = "", string segment = "", string language = "")
        {
            string resourceFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\Resources";
            //string statementDestPath = outputLocation + "\\Statements" + "\\" + batchId;
            //string statementPath = baseURL + "\\Statements" + "\\" + batchId + "\\" + customerId + "\\" + fileName;
            string statementDestPath = outputLocation + "\\Statements" + "\\" + scheduleName + "\\" + batchName;
            string statementPath = baseURL + "\\Statements" + "\\" + scheduleName + "\\" + batchName + "\\" + customerId + "\\" + fileName;
            if (!Directory.Exists(statementDestPath))
            {
                Directory.CreateDirectory(statementDestPath);
            }
            string path = statementDestPath + "\\" + customerId + "\\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = Path.Combine(path, fileName);
            if (!File.Exists(filepath))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }

            //To move js, css and other assets contents which are common to each statment file
            if (!Directory.Exists(statementDestPath + "\\common"))
            {
                DirectoryCopy(resourceFilePath, (statementDestPath + "\\common"), true);
            }

           // string pdfGenerationError = null;
            //Printing PDF
            if (printPdf)
            {
                var deleteHtmlAfterPdfGenerate = false;
                bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["DeleteHtmlAfterPdfGenerate"], out deleteHtmlAfterPdfGenerate);
                var outputPdfPath = Path.Combine(path, Path.GetFileNameWithoutExtension(fileName) + ".pdf");



                //  pdfGenerationError =  GeneratePdf(filepath, outputPdfPath, segment, language);

                Task.Run(async () =>
                {
                    var result = await GeneratePdf(filepath, outputPdfPath, headerHtml, language);
                    if (string.IsNullOrEmpty(result)) {
                        if (deleteHtmlAfterPdfGenerate)
                        {
                            File.Delete(filepath);
                        }
                    };
                }).Wait();

                //if ( await GeneratePdf(filepath, outputPdfPath, segment, language)!=null)
                //{

                //}
                //else
                //{
                //    //if (deleteHtmlAfterPdfGenerate)
                //    //{
                //    //    File.Delete(filepath);
                //    //}
                //}

               
            }

            return filepath;
        }

        /// <summary>
        /// This method help to write json stringin to actual file
        /// </summary>
        /// <param name="Message"> the message string </param>
        /// <param name="fileName"> the file name </param>
        /// <param name="batchId"> the batch identifier </param>
        /// <param name="customerId"> the customer identifier </param>
        public void WriteToJsonFile(string Message, string fileName, long batchId, long customerId, string baseURL)
        {
            string jsonFileDestPath = baseURL + "\\Statements" + "\\" + batchId;
            if (!Directory.Exists(jsonFileDestPath))
            {
                Directory.CreateDirectory(jsonFileDestPath);
            }
            string path = jsonFileDestPath + "\\" + customerId + "\\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = path + fileName;
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(filepath))
            {
                sw.WriteLine(Message);
            }
        }

        /// <summary>
        /// This method help to copy files from one directory to another directory
        /// </summary>
        /// <param name="sourceDirName"> the path of source directory </param>
        /// <param name="destDirName"> the path of destinaation diretory </param>
        /// <param name="copySubDirs"> the bool value of is want to copy sub directory of source directory </param>
        public void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                if (!File.Exists(temppath))
                {
                    file.CopyTo(temppath, false);
                }
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    if (subdir.Name.ToLower() != "sampledata")
                    {
                        string temppath = Path.Combine(destDirName, subdir.Name);
                        DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                    }
                }
            }
        }

        /// <summary>
        /// This method help to create zip file of common html with js, css, and image files
        /// </summary>
        /// <param name="htmlstr"> the html string </param>
        /// <param name="fileName"> the filename </param>
        /// <param name="batchId"> the batch id </param>
        // public string CreateAndWriteToZipFile(string htmlstr, string fileName, long batchId, string baseURL, string outputLocation, IDictionary<string, string> filesDictionary = null)
        public string CreateAndWriteToZipFile(string htmlstr, string fileName, string scheduleName, string batchName, string baseURL, string outputLocation, IDictionary<string, string> filesDictionary = null)
        {
            CreateAndWriteToZipFileCount += 1;
            var callerName = (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name;
            //  _log.Info($"CreateAndWriteToZipFile is being called {CreateAndWriteToZipFileCount} time. Called by {callerName}");

            ////create folder to store the html statement files for current batch customers
            //string path = outputLocation + "\\Statements" + "\\" + batchId + "\\";
            //if (!Directory.Exists(path))
            //{
            //    Directory.CreateDirectory(path);
            //}

            ////create media folder for common images and videos files of asset library
            //string mediaPath = path + "\\common\\media\\";
            //if (!Directory.Exists(mediaPath))
            //{
            //    Directory.CreateDirectory(mediaPath);
            //}

            ////common resource files path 
            //string resourceFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\Resources";

            //string zipFileVirtualPath = "\\Statements" + "\\" + batchId + "\\statement" + DateTime.Now.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".zip";
            // string zipPath = outputLocation + zipFileVirtualPath;
            string zipPath = string.Empty;
            try
            {
                //create folder to store the html statement files for current batch customers

                if (!Directory.Exists(Path.GetPathRoot(outputLocation)))
                {
                    return string.Empty;
                }

                string path = outputLocation + "\\Statements" + "\\" + scheduleName + "\\" + batchName;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //create media folder for common images and videos files of asset library
                string mediaPath = path + "\\common\\media\\";
                if (!Directory.Exists(mediaPath))
                {
                    Directory.CreateDirectory(mediaPath);
                }

                //common resource files path 
                string resourceFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\Resources";

                //string zipFileVirtualPath = "\\Statements" + "\\" + scheduleName + "\\" + batchName + "\\statement" + DateTime.Now.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".zip";
                //zipPath = outputLocation + zipFileVirtualPath;

                ////create temp folder for common html statement
                //string temppath = path + "\\temp\\";
                //if (!Directory.Exists(temppath))
                //{
                //    Directory.CreateDirectory(temppath);
                //}

                ////create temp media folder for common images and videos files of asset library
                //string tempmediaPath = temppath + "\\common\\media\\";
                //if (!Directory.Exists(tempmediaPath))
                //{
                //    Directory.CreateDirectory(tempmediaPath);
                //}

                ////folder to save actual common html statement file
                //string spath = temppath + "\\statement\\";
                //if (!Directory.Exists(spath))
                //{
                //    Directory.CreateDirectory(spath);
                //}

                ////to delete any common html statement file, if exist 
                //string filepath = spath + fileName;
                //if (File.Exists(filepath))
                //{
                //    File.Delete(filepath);
                //}

                //// Create a html file to write to common html statement
                //using (StreamWriter sw = File.CreateText(filepath))
                //{
                //    sw.WriteLine(htmlstr);
                //}

                //asset (images and videos) files as well as json files
                if (filesDictionary != null && filesDictionary?.Count > 0)
                {
                    //WebClient webClient = new WebClient();
                    foreach (KeyValuePair<string, string> file in filesDictionary)
                    {
                        if (File.Exists(file.Value))
                        {
                            if (file.Key.Contains(".json"))
                            {
                                //File.Copy(file.Value, Path.Combine(spath, file.Key));
                            }
                            else
                            {
                                if (File.Exists(Path.Combine(mediaPath, file.Key)))
                                {
                                    // If it exists, delete the destination file before copying the new one
                                    File.Delete(Path.Combine(mediaPath, file.Key));
                                }
                                File.Copy(file.Value, Path.Combine(mediaPath, file.Key));
                                //File.Copy(file.Value, Path.Combine(tempmediaPath, file.Key));
                            }
                        }
                        //webClient.DownloadFile(file.Value, (spath + file.Key));
                    }
                }

                //copy all common resource file to current batch statement folder as well as at common statement folder
                DirectoryCopy(resourceFilePath, (path + "\\common"), true);
                //DirectoryCopy(resourceFilePath, (temppath + "\\common"), true);

                ////create a zip file for common html statement and related resources and media files
                //if (!File.Exists(Path.Combine(temppath, zipPath)))
                //{
                //    ZipFile.CreateFromDirectory(temppath, zipPath);
                //}

                ////delete temp folder after zip file created
                //string deleteFile = path + "\\temp";
                //DirectoryInfo directoryInfo = new DirectoryInfo(deleteFile);
                //if (directoryInfo.Exists)
                //{
                //    directoryInfo.Delete(true);
                //}
            }
            catch (Exception ex)
            {
                throw ex;
               // _log.Error($"CreateAndWriteToZipFile method get exception which is {ex.Message}. Called by {callerName}.");
            }
            // //create temp folder for common html statement
            // string temppath = path + "\\temp\\";
            // if (!Directory.Exists(temppath))
            // {
            //     Directory.CreateDirectory(temppath);
            // }

            // //create temp media folder for common images and videos files of asset library
            // string tempmediaPath = temppath + "\\common\\media\\";
            // if (!Directory.Exists(tempmediaPath))
            // {
            //     Directory.CreateDirectory(tempmediaPath);
            // }

            // //folder to save actual common html statement file
            // string spath = temppath + "\\statement\\";
            // if (!Directory.Exists(spath))
            // {
            //     Directory.CreateDirectory(spath);
            // }

            // //to delete any common html statement file, if exist 
            // string filepath = spath + fileName;
            // if (File.Exists(filepath))
            // {
            //     File.Delete(filepath);
            // }

            // // Create a html file to write to common html statement
            // using (StreamWriter sw = File.CreateText(filepath))
            // {
            //     sw.WriteLine(htmlstr);
            // }

            // //asset (images and videos) files as well as json files
            // if (filesDictionary != null && filesDictionary?.Count > 0)
            // {
            //     //WebClient webClient = new WebClient();
            //     foreach (KeyValuePair<string, string> file in filesDictionary)
            //     {
            //         if (File.Exists(file.Value))
            //         {
            //             if (file.Key.Contains(".json"))
            //             {
            //                 File.Copy(file.Value, Path.Combine(spath, file.Key));
            //             }
            //             else
            //             {
            //                 File.Copy(file.Value, Path.Combine(mediaPath, file.Key));
            //                // File.Copy(file.Value, Path.Combine(tempmediaPath, file.Key));
            //             }
            //         }
            //         //webClient.DownloadFile(file.Value, (spath + file.Key));
            //     }
            // }

            // //copy all common resource file to current batch statement folder as well as at common statement folder
            // DirectoryCopy(resourceFilePath, (path + "\\common"), true);
            //// DirectoryCopy(resourceFilePath, (temppath + "\\common"), true);

            // //create a zip file for common html statement and related resources and media files
            // ZipFile.CreateFromDirectory(temppath, zipPath);

            // //delete temp folder after zip file created
            // string deleteFile = path + "\\temp";
            // DirectoryInfo directoryInfo = new DirectoryInfo(deleteFile);
            // if (directoryInfo.Exists)
            // {
            //     directoryInfo.Delete(true);
            // }

            return zipPath;
        }

        /// <summary>
        /// This method help to delete unwantedly added json files if html generation failed for customer
        /// </summary>
        /// <param name="batchId"> the batch identifier </param>
        /// <param name="customerId"> the customer identifier </param>
        /// <returns>true if deleted successfully, otherwise false.</returns>
        public bool DeleteUnwantedDirectory(long batchId, long? customerId, string baseURL)
        {
            string deleteDirPath = baseURL + "\\Statements" + "\\" + batchId + "\\" + customerId;
            DirectoryInfo directoryInfo = new DirectoryInfo(deleteDirPath);
            if (directoryInfo.Exists)
            {
                directoryInfo.Delete(true);
            }
            return true;
        }

        /// <summary>
        /// This method help to get string value of month
        /// </summary>
        /// <param name="m"> the numeric value of month </param>
        /// <returns>string value of month</returns>
        public string getMonth(int m)
        {
            string res;
            switch (m)
            {
                case 1:
                    res = "Jan";
                    break;
                case 2:
                    res = "Feb";
                    break;
                case 3:
                    res = "Mar";
                    break;
                case 4:
                    res = "Apr";
                    break;
                case 5:
                    res = "May";
                    break;
                case 6:
                    res = "Jun";
                    break;
                case 7:
                    res = "Jul";
                    break;
                case 8:
                    res = "Aug";
                    break;
                case 9:
                    res = "Sep";
                    break;
                case 10:
                    res = "Oct";
                    break;
                case 11:
                    res = "Nov";
                    break;
                case 12:
                    res = "Dec";
                    break;
                default:
                    res = "Jan";
                    break;
            }
            return res;
        }

        /// <summary>
        /// This method help to get month difference in between 2 dates
        /// </summary>
        /// <param name="endDate"> the end date value </param>
        /// <param name="startDate"> the start date value </param>
        /// <returns>difference between 2 dates in numeric</returns>
        public int MonthDifference(DateTime endDate, DateTime startDate)
        {
            return (endDate.Month - startDate.Month) + 12 * (endDate.Year - startDate.Year);
        }

        /// <summary>
        /// This method help to get day difference in between 2 dates
        /// </summary>
        /// <param name="endDate"> the end date value </param>
        /// <param name="startDate"> the start date value </param>
        /// <returns>difference between 2 dates in numeric</returns>
        public int DayDifference(DateTime endDate, DateTime startDate)
        {
            return Convert.ToInt32((endDate.Date - startDate.Date).TotalDays);
        }

        /// <summary>
        /// This method help to get year difference in between 2 dates
        /// </summary>
        /// <param name="endDate"> the end date value </param>
        /// <param name="startDate"> the start date value </param>
        /// <returns>difference between 2 dates in numeric</returns>
        public int YearDifference(DateTime startDate, DateTime endDate)
        {
            //Excel documentation says "COMPLETE calendar years in between dates"
            int years = endDate.Year - startDate.Year;

            if (startDate.Month == endDate.Month &&// if the start month and the end month are the same
                endDate.Day < startDate.Day// AND the end day is less than the start day
                || endDate.Month < startDate.Month)// OR if the end month is less than the start month
            {
                years--;
            }

            return years;
        }

        /// <summary>
        /// This method help to get numeric value of month
        /// </summary>
        /// <param name="m"> the string value of month </param>
        /// <returns>numeric value of month</returns>
        public int getNumericMonth(string m)
        {
            int res;
            if (m.ToLower() == "january" || m.ToLower() == "jan")
            {
                res = 1;
            }
            else if (m.ToLower() == "february" || m.ToLower() == "feb")
            {
                res = 2;
            }
            else if (m.ToLower() == "march" || m.ToLower() == "mar")
            {
                res = 3;
            }
            else if (m.ToLower() == "april" || m.ToLower() == "apr")
            {
                res = 4;
            }
            else if (m.ToLower() == "may")
            {
                res = 5;
            }
            else if (m.ToLower() == "june" || m.ToLower() == "jun")
            {
                res = 6;
            }
            else if (m.ToLower() == "july" || m.ToLower() == "jul")
            {
                res = 7;
            }
            else if (m.ToLower() == "august" || m.ToLower() == "aug")
            {
                res = 8;
            }
            else if (m.ToLower() == "september" || m.ToLower() == "sep")
            {
                res = 9;
            }
            else if (m.ToLower() == "october" || m.ToLower() == "oct")
            {
                res = 10;
            }
            else if (m.ToLower() == "november" || m.ToLower() == "nov")
            {
                res = 11;
            }
            else if (m.ToLower() == "december" || m.ToLower() == "dec")
            {
                res = 12;
            }
            else
            {
                res = 1;
            }
            return res;
        }

        /// <summary>
        /// This method help to save image file from url
        /// </summary>
        /// <param name="filePath"> the file path value </param>
        /// <param name="format"> the image format </param>
        /// <param name="imageUrl"> the image url </param>
        /// <returns>return true if download successfully.</returns>
        public bool SaveImage(string filePath, ImageFormat format, string imageUrl)
        {
            try
            {
                WebClient client = new WebClient();
                Stream stream = client.OpenRead(imageUrl);
                Bitmap bitmap;
                bitmap = new Bitmap(stream);

                if (bitmap != null)
                {
                    bitmap.Save(filePath, format);
                }

                stream.Flush();
                stream.Close();
                client.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
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
        public string ExecuteWebTenantRequest(string instanceURL, string controller, string action, string objectData, string tenantKey, string tenantCode, bool isThirdPartyEnabled = false, bool toBeSerailzied = false)
        {
            string responseFromServer = string.Empty;
            try
            {

                WebRequest request = WebRequest.Create(instanceURL + "/" + controller + "/" + action + "?isThirdPartyEnabled=" + isThirdPartyEnabled);
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
        /// This method executes pdf crowd tool web request to convert HTML file to PDF.
        /// </summary>
        /// <param name="htmlStatementPath">The statement statement path.</param>
        /// <param name="outPdfPath">The output pdf path.</param>
        /// <param name="password">The password to protect PDF.</param>
        /// <returns>
        /// Returns the true if pdf generated successfully, otherwise false
        /// </returns>
        public bool HtmlStatementToPdf(string htmlStatementPath, string outPdfPath, string password)
        {
            var isPdfSuccess = false;
            try
            {
                var userName = System.Configuration.ConfigurationManager.AppSettings["PdfCrowdUserName"];
                var apiKey = System.Configuration.ConfigurationManager.AppSettings["PdfCrowdApiKey"];
                var client = new pdfcrowd.HtmlToPdfClient(userName, apiKey);

                //Set the output page width. The safe maximum is 200in otherwise some PDF viewers may be unable to open the PDF.
                client.setPageWidth("12in");

                //Set the output page height. Use -1 for a single page PDF. The safe maximum is 200in otherwise some PDF viewers may be unable to open the PDF.
                client.setPageHeight("10in");

                //Set the output page top margin.
                client.setMarginTop("0.4in");

                //Set the output page right margin.
                client.setMarginRight("0.2in");

                //Set the output page bottom margin.
                client.setMarginBottom("0.4in");

                //Set the output page left margin.
                client.setMarginLeft("0.2in");

                //Set the output page header height.
                client.setHeaderHeight("0.4in");

                //Use the specified HTML as the output page footer. 
                client.setFooterHtml("Page <span class='pdfcrowd-page-number'></span> of <span class='pdfcrowd-page-count'></span> pages");

                //Set the output page footer height.
                client.setFooterHeight("0.4in");

                //The viewport width affects the @media min-width and max-width CSS properties. 
                //This mode can be used to choose a particular version (mobile, desktop, ..) of a responsive page
                client.setRenderingMode("viewport");

                //The HTML contents width fits the print area width.
                client.setSmartScalingMode("content-fit");

                //Set the quality of embedded JPEG images. A lower quality results in a smaller PDF file but can lead to compression artifacts.
                client.setJpegQuality(80);

                //Specify which image types will be converted to JPEG. 
                //Converting lossless compression image formats (PNG, GIF, ...) to JPEG may result in a smaller PDF file.
                client.setConvertImagesToJpeg("all");

                //Set the DPI of images in PDF. A lower DPI may result in a smaller PDF file.
                client.setImageDpi(300);

                //Set the title of the PDF.
                client.setTitle("PDF statement");

                //Set the author of the PDF.
                client.setAuthor("PPS");

                if (password != string.Empty)
                {
                    //Encrypt the PDF. This prevents search engines from indexing the contents.
                    client.setEncrypt(true);

                    //Protect the PDF with a user password. 
                    //When a PDF has a user password, it must be supplied in order to view the document and to perform operations allowed by the access permissions.
                    client.setUserPassword(password);
                }

                client.convertFileToFile(htmlStatementPath, outPdfPath);
                isPdfSuccess = true;
            }
            catch (pdfcrowd.Error why)
            {
                throw why;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isPdfSuccess;
        }

        ///// <summary>
        ///// This method helps to format nedbank tenant amount value
        ///// </summary>
        ///// <param name="amount">The value.</param>
        ///// <returns>
        ///// Returns the the formatted amount value string
        ///// </returns>
        //public string NedbankClientAmountFormatter(double amount)
        //{
        //    try
        //    {
        //        var totalAmtStr = Convert.ToString(amount).Split(new Char[] { '.', ',' });
        //        var wholeNo = totalAmtStr[0];
        //        var franctionNo = totalAmtStr.Length > 1 ? (new string(totalAmtStr[1].Take(2).ToArray())) : "0";

        //        char[] cArray = wholeNo.ToCharArray();
        //        Array.Reverse(cArray);

        //        var tempAmountVal = ".";
        //        int cnt = 0;
        //        while (cArray.Length != cnt)
        //        {
        //            tempAmountVal = (tempAmountVal.Length > 1 && tempAmountVal.Length % 4 == 0) ? tempAmountVal + " " + cArray[cnt].ToString() : tempAmountVal + cArray[cnt].ToString();
        //            cnt++;
        //        }

        //        cArray = tempAmountVal.ToCharArray();
        //        Array.Reverse(cArray);
        //        return (new string(cArray) + franctionNo);
        //    }
        //    catch (Exception)
        //    {
        //        return "0";
        //    }
        //}

        /// <summary>
        /// This method helps to format currency as per provided country currency details amount value
        /// </summary>
        /// <param name="CountryCultureInfoCode">The country currency cultureInfo code value.</param>
        /// <param name="CurrencyDecimalSeparator">Defines the string that separates integral and decimal digits.</param>
        /// <param name="currencyFormat">The currency format value.</param>
        /// <param name="amount">The amount value.</param>
        /// <returns>
        /// Returns the the formatted amount value in string
        /// </returns>
        public string CurrencyFormatting(string CountryCultureInfoCode, string CurrencyDecimalSeparator, string currencyFormat, decimal amount)
        {
            try
            {
                NumberFormatInfo myNumberFormatInfo = new CultureInfo(CountryCultureInfoCode, false).NumberFormat;
                myNumberFormatInfo.CurrencyDecimalSeparator = CurrencyDecimalSeparator;
                return amount.ToString(currencyFormat, myNumberFormatInfo);
            }
            catch (Exception)
            {
                return "0";
            }
        }

        public async Task<string>  GeneratePdf(string htmlPath, string outPdfPath, string segment, string language)
        {
            string pdfGenerationError = null;

            // Switch case based on the value of segment
            //switch (segment)
            //{
            //    case "FSP":
            //        // Handle the "pps" case
            //        // You can add your logic specific to "pps" segment here
            //        segment = "fsp";
            //        break;
            //    case "PPS":
            //        // Handle the "pps" case
            //        // You can add your logic specific to "pps" segment here
            //        segment = "pps";
            //        break;
            //    // Add more cases as needed
            //    // case "otherSegment":
            //    //     // Handle the "otherSegment" case
            //    //     break;
            //    default:
            //        // Handle the case when segment doesn't match any expected value
            //        break;
            //}





            SelectPdf.PdfDocument doc = new PdfDocument();
           // segment = "pps";
       //     try
        //    {
                string headerFooterFontFolderPath = System.Configuration.ConfigurationManager.AppSettings["HeaderFooterFontFolderPath"];

            if (string.IsNullOrWhiteSpace(headerFooterFontFolderPath))
            {
                // _log.Error($"HeaderFooterFontFolderPath appSetting key is missing in web.config.");
                pdfGenerationError = "HeaderFooterFontFolderPath appSetting key is missing in web.config.";
                //  return false;
                return pdfGenerationError;
            }

         //   var html = File.ReadAllText("./invoice.html");
             var headerContent = File.ReadAllText($@"{headerFooterFontFolderPath}\HeaderFooters\" + segment + "_header.html");
              var footerContent =  File.ReadAllText($@"{headerFooterFontFolderPath}\HeaderFooters\" + segment + "_footer.html");

           
            // Get the directory path without the file name
            string directoryPath = Path.GetDirectoryName(outPdfPath);
          // Get the parent directory path
            string parentDirectoryPath = Directory.GetParent(directoryPath).FullName;

            // Read the local image file as base64
            var logoImgPath = parentDirectoryPath+ @"\common\images\logo3.jpg";
            var logoImgPathBase64 = Convert.ToBase64String(File.ReadAllBytes(logoImgPath));

            var logoFbPath = parentDirectoryPath+ @"\common\images\fb_foot.png";
            var logoFbPathBase64 = Convert.ToBase64String(File.ReadAllBytes(logoFbPath));

            var logoImgInstaPath = parentDirectoryPath+ @"\common\images\insta_foot.png";
            var logoImgInstaPathBase64 = Convert.ToBase64String(File.ReadAllBytes(logoImgInstaPath));

            var logoImgTwitterPath = parentDirectoryPath+ @"\common\images\twitter_foot.png";
            var logoImgTwitterPathBase64 = Convert.ToBase64String(File.ReadAllBytes(logoImgTwitterPath));

            var logoImgInPath = parentDirectoryPath+ @"\common\images\in_foot.png";
            var logoImgInBase64 = Convert.ToBase64String(File.ReadAllBytes(logoImgInPath));

            var logoImgYouPath = parentDirectoryPath+ @"\common\images\you_foot.png";
            var logoImgYouPathBase64 = Convert.ToBase64String(File.ReadAllBytes(logoImgYouPath));

            var logoImgTiktokPath = parentDirectoryPath+ @"\common\images\ticktok_foot.png";
            var logoImgTiktokPathBase64 = Convert.ToBase64String(File.ReadAllBytes(logoImgTiktokPath));

            headerContent = headerContent.Replace("{{logoImgPath}}", logoImgPathBase64);
            footerContent= footerContent.Replace("{{logoImgFbPath}}", logoFbPathBase64);
            footerContent= footerContent.Replace("{{logoImgInstaPath}}", logoImgInstaPathBase64);
            footerContent=footerContent.Replace("{{logoImgTwitterPath}}", logoImgTwitterPathBase64);
            footerContent = footerContent.Replace("{{logoImgInPath}}", logoImgInBase64);
            footerContent = footerContent.Replace("{{logoImgYouPath}}", logoImgYouPathBase64);
            footerContent = footerContent.Replace("{{logoImgTiktokPath}}", logoImgTiktokPathBase64);

           //footerContent += "<span class='pageNumber'></span> of<span class='totalPages'></span>";
           footerContent = footerContent.Replace("{{PageNumber}}", "<span class='pageNumber'></span>/<span class='totalPages'></span>");


            //headerContent = @"<div style='width:100%; height: 50px ;font-weight: 400;margin: 10px auto;background-color: #fff;'><div style='display: flex; justify-content: space-between; padding: 1rem 0 1rem 0; flex-direction: column;'><div style='display:flex;width:100%;gap: 20px;align-items: center;'><div class='fsp-logo'><img  src ='data:image/jpeg;base64,/9j/4QAYRXhpZgAASUkqAAgAAAAAAAAAAAAAAP/sABFEdWNreQABAAQAAAAmAAD/4QMsaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLwA8P3hwYWNrZXQgYmVnaW49Iu+7vyIgaWQ9Ilc1TTBNcENlaGlIenJlU3pOVGN6a2M5ZCI/PiA8eDp4bXBtZXRhIHhtbG5zOng9ImFkb2JlOm5zOm1ldGEvIiB4OnhtcHRrPSJBZG9iZSBYTVAgQ29yZSA2LjAtYzAwMiA3OS4xNjQ0NjAsIDIwMjAvMDUvMTItMTY6MDQ6MTcgICAgICAgICI+IDxyZGY6UkRGIHhtbG5zOnJkZj0iaHR0cDovL3d3dy53My5vcmcvMTk5OS8wMi8yMi1yZGYtc3ludGF4LW5zIyI+IDxyZGY6RGVzY3JpcHRpb24gcmRmOmFib3V0PSIiIHhtbG5zOnhtcD0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLyIgeG1sbnM6eG1wTU09Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9tbS8iIHhtbG5zOnN0UmVmPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvc1R5cGUvUmVzb3VyY2VSZWYjIiB4bXA6Q3JlYXRvclRvb2w9IkFkb2JlIFBob3Rvc2hvcCAyMS4yIChXaW5kb3dzKSIgeG1wTU06SW5zdGFuY2VJRD0ieG1wLmlpZDo5OUFERjE0Q0EwQzkxMUVFQUI2NkY4QzlBMzYzRjM0MiIgeG1wTU06RG9jdW1lbnRJRD0ieG1wLmRpZDo5OUFERjE0REEwQzkxMUVFQUI2NkY4QzlBMzYzRjM0MiI+IDx4bXBNTTpEZXJpdmVkRnJvbSBzdFJlZjppbnN0YW5jZUlEPSJ4bXAuaWlkOjk5QURGMTRBQTBDOTExRUVBQjY2RjhDOUEzNjNGMzQyIiBzdFJlZjpkb2N1bWVudElEPSJ4bXAuZGlkOjk5QURGMTRCQTBDOTExRUVBQjY2RjhDOUEzNjNGMzQyIi8+IDwvcmRmOkRlc2NyaXB0aW9uPiA8L3JkZjpSREY+IDwveDp4bXBtZXRhPiA8P3hwYWNrZXQgZW5kPSJyIj8+/+4ADkFkb2JlAGTAAAAAAf/bAIQADQkJCQoJDQoKDRMMCwwTFhANDRAWGRQUFhQUGRgTFRUVFRMYGB0eIB4dGCYmKSkmJjg3Nzc4Pj4+Pj4+Pj4+PgEODAwODw4RDw8RFA4QDhQVERERERUfFRUXFRUfJxwYGBgYHCcjJSAgICUjKysnJysrNTUzNTU+Pj4+Pj4+Pj4+/8AAEQgATQBQAwEiAAIRAQMRAf/EAIQAAQACAwEBAAAAAAAAAAAAAAACBQMEBgcBAQEBAQEBAAAAAAAAAAAAAAAAAgEDBBAAAgEDAgQCBwcEAwAAAAAAAQIDABEEBQYhMRITQSJhcYGRIxQVUaGxQmKiB8EyklPRUjMRAAIBAwQDAQAAAAAAAAAAAAABESExAkFRcRJhgSIT/9oADAMBAAIRAxEAPwDi6UpXqOIpSr7b2z9V10CaO2NhXscmQGzW59tebfhWNpVYiShpXpabD2lhAJqGY0k1uIkmWL3ItjR9h7SzQU0/MaOa3ARzLL70a5qf0x8ldWeaUq+3Ds/VNCBmktk4V7DJjBst+XcX8v4VQ1SadUTEClKVoFKUoC92ht/67qoimB+TxwJckjhcXssd/wBR+69dluTc8Wn4ipjfDxjePEghPbaYR+Uv1rxjgU8AV8zeFhxrR2TD29sTtGSk+pZYxescwh6UJHqUuRVBrATVd5tgyEx4/wAwmBEI7fDjQiJekG458a5uuTm2JVlyVk+uarMxKznHUm/bx/gr+yzH1sSaQa5qsLAtO2QoN+3kfGX992HrUg1d4G08DOyJEhyphHj5rYE5ZUDHgxjlTn4rYqfXWvibaw83EizYcwxQySzYpGR0peeNeqJVcXVVk9I4emqnEyGdbtvdEWoYjJk/ExxaPLgmPcaEP5Q/W3GSBjwPV5l8bjjXG7w2/wDQtVMUIPyeQDLjE8bC/mjv+k/das2kQy6Nu7GwpUdRKVxsmGbp4pkL0sp6CVZbngRzq/3tD3NsQNIS8+mZZxes8ynmRSfWvQTUqmSi2Rt1wee0pSuhIpSlAd7sfMQ6N2SQDgahFM9/9c/wur2MTVdrmIukb7TKym7WJJkR5wlIJBTqDSABQSSGBFUuhar9LzTLIplxJ0MGZCObxPzt+peYr0aXF0zdGmxYGbKHnCl8HPS15VAt3Ev+bwlj8D7DXN/OU6MpVXBx2Drsce5VmnylTSosuXMukZUOXDqjsqJ1M9mA48qp5dWzgGx1lRsYdxREsaiJxIwZ2KFRctYcSL1b5/8AH248WQiCJM2L8skThSR6UkKkffTA/j7cmVIBPEmFF+aSVwxA9CRlifurZxvKFSO0sfM1rdOLPMxk+VKzyvawVIR0xIAOAF7ACrnfGYg0bsggnP1CWZLf64B2ur2sBV3Di6ZtfTZcDClCZBUPnZ72vEpFu69vHwij8T7TXnOu6t9UzRJGpixIEEGHCeaxJy6v1NzNYvrKdEHRQVtKUroSKUqcMrwypNHbrjYMtxcXBuOFAQuK3dP1fM066wuGhZgz48nmjLDkwAIKt9jKQajHqOQqogKAIkkQZlv5ZRZr+n1UhzJYI1jSVOhBKoBQn/2Xoc/287cvsrPQOpxP5GyYkCyCXh4HonH+Tdp/expl/wAi5MqFYxLx8B0QD/Je6/uYVzP1Ca6kSRDpEa/2HiIm7i34faK+NnSkEF4vMHU+VuUkgmP7hwqei2K7Pcahq+ZqFlncLCrF0gj8sYY83NySzH/sxJrS4Vvx6lPG0bK8N4y7LeMkfEv1XBFvGsLZsvSkdoysSCNGC2NlcSgk8CT1eJqvRJrUqcsjSyvK1g0jF2A4C7G5sKhWgUpSgFZFmdUCgCwJYEi542/4rHSgM/zcvWHstw3XYCwva3IcKxxzPEQUtcHqFxfjYj+tQpWAm0rMoUgABQvAW4KSR7eNQpStApSlAf/Z' style='width: 50px; height: 50px;' alt='image not found'></div><div><h1 style=' font-size: 28.8px; text-shadow: #c6ced9 1px 1px 1px; margin-bottom: 0;'>Financial Service Provider(FSP) Statement</h1><h6 syle='font-weight: bold;font-size:16px;color: #5b5a5a;margin-bottom: 0;'>{{FSPName}} T/A {{FSPTradingName}}</h6></div></div></div>";



            // string headerContent = @"<div style='width:100%; height: 50px ;font-weight: 400;margin: 10px auto;background-color: #fff;'><div style='display: flex; justify-content: space-between; padding: 1rem 0 1rem 0; flex-direction: column;'><div style='display:flex;width:100%;gap: 20px;align-items: center;'><div class='fsp-logo'><img  src ='../common/images/logo3.jpg' style='width: 50px; height: 50px;' alt='image not found'></div><div><h1 style=' font-size: 28.8px; text-shadow: #c6ced9 1px 1px 1px; margin-bottom: 0;'>Financial Service Provider(FSP) Statement</h1><h6 syle='font-weight: bold;font-size:16px;color: #5b5a5a;margin-bottom: 0;'>{{FSPName}} T/A {{FSPTradingName}}</h6></div></div></div>";





            //    var footerContent = @"<div class='container_area'><div class='fsp-footer-section dark-blue-bg py-1 px-2'><ul class='fsp-social-icons'><li><a href='#'><img src='../common/images/fb_foot.png'></a></li><li><a href='#'><img src=''../common/images/insta_foot.png'></a></li><li><a href='#'><img src='../common/images/twitter_foot.png'></a></li><li><a href='#'><img src='../common/images/in_foot.png'></a></li><li><a href='#'><img src='../common/images/you_foot.png'></a></li><li><a href='#'><img src='../common/images/ticktok_foot.png'></a></li></ul><div class='fsp-copyright mb-0'>{{FSPFooterDetails}}</div><div></div><div></div><div></div><div class='fsp-page mb-0'>{{FSPPage}}</div></div></div>";


            //string Header = @"<div style='text-align: center; font-size: 12px;'><span class='date'></span>|<span class='title'></span> |<span class='url'></span>|<span class='pageNumber'></span> of<span class='totalPages'></span></div>";
            //   string footerContent = "<div style='text-align: center; font-size: 12px;'>Page <span class='pageNumber'></span></div>";


            //var headerContent = "<span style='font-size: 30px; width: 200px; height: 50px; background - color: red; color: black; margin: 20px;'>Header<a href='#'><img src='../common/images/you_foot.png'></a></span>";
            //  var footerContent = "<span style='font-size: 30px;background-color: red; color:black;'>Footer</span>";

            var pdfOptions = new PuppeteerSharp.PdfOptions();

            pdfOptions.PrintBackground = true;
            pdfOptions.DisplayHeaderFooter = true;
            pdfOptions.HeaderTemplate = headerContent; 
            pdfOptions.Landscape = false; 
            pdfOptions.MarginOptions = new PuppeteerSharp.Media.MarginOptions() { Bottom = "4cm", Left = "1cm", Right = "1cm", Top = "4cm" };
            pdfOptions.Scale = 1m; 
            pdfOptions.FooterTemplate = footerContent; 
            pdfOptions.PreferCSSPageSize = true;
            pdfOptions.Format = PuppeteerSharp.Media.PaperFormat.A4;


            //var options = new LaunchOptions
            //{
            //    Headless = false,
            //    ExecutablePath = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
            //};

            //using (var browser = await Puppeteer.LaunchAsync(options))
            //{


            //}
            //var page = await browser.NewPageAsync();


            //await page.GoToAsync("https://www.google.com/");
            //await page.PdfAsync(outPdfPath, pdfOptions);

            var browserFetcher = new BrowserFetcher();
            // Download the necessary browser binaries
            await browserFetcher.DownloadAsync();

            // Launch Puppeteer with specific options
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });

            // Create a new page
            var page = await browser.NewPageAsync();
            // Navigate to the specified HTML path
            await page.GoToAsync(htmlPath);
           //Add script tags to the page
            await page.AddScriptTagAsync(new AddTagOptions { Path = parentDirectoryPath + @"\common\js\jquery.min.js" });
            await page.AddScriptTagAsync(new AddTagOptions { Path = parentDirectoryPath + @"\common\js\popper.min.js" });
            await page.AddScriptTagAsync(new AddTagOptions { Path = parentDirectoryPath + @"\common\js\bootstrap.min.js" });
            await page.AddScriptTagAsync(new AddTagOptions { Path = parentDirectoryPath + @"\common\js\highcharts.js" });
            await page.AddScriptTagAsync(new AddTagOptions { Path = parentDirectoryPath + @"\common\js\jquery.min.js" });
            await page.AddScriptTagAsync(new AddTagOptions { Path = parentDirectoryPath + @"\common\js\series-label.js" });
            await page.AddScriptTagAsync(new AddTagOptions { Path = parentDirectoryPath + @"\common\js\exporting.js" });
            await page.AddScriptTagAsync(new AddTagOptions { Path = parentDirectoryPath + @"\common\js\export-data.js" });
            await page.AddScriptTagAsync(new AddTagOptions { Path = parentDirectoryPath + @"\common\js\accessibility.js" });
            await page.AddScriptTagAsync(new AddTagOptions { Path = parentDirectoryPath + @"\common\js\script.js" });
            // Generate PDF from the page with specified options
            // Add style tags to the page
            await page.AddStyleTagAsync(new AddTagOptions { Path = parentDirectoryPath + @"\common\css\bootstrap.min.css" });
            await page.AddStyleTagAsync(new AddTagOptions { Path = parentDirectoryPath + @"\common\css\font-awesome.min.css" });
            await page.AddStyleTagAsync(new AddTagOptions { Path = parentDirectoryPath + @"\common\css\site.css" });
            await page.AddStyleTagAsync(new AddTagOptions { Path = parentDirectoryPath + @"\common\css\ltr.css" });
            await page.PdfAsync(outPdfPath, pdfOptions);




            
            return pdfGenerationError;
            }
           // catch (Exception ex)
           // {
                //_log.Error($"GeneratePdf method get exception which is {ex.Message}.");
              //  pdfGenerationError = $"GeneratePdf method get exception which is {ex.Message}";
              //  return pdfGenerationError;
           // }
            
            //try
            //{
            //    //htmlStatementPath = @"C:\UserFiles\Statements\1161\112233\Statement_112233_78_4_19_2022_7_44_29_PM.html";

            //    // read parameters from the webpage
            //    string url = htmlStatementPath;

            //    PdfPageSize pageSize = PdfPageSize.A4;

            //    PdfPageOrientation pdfOrientation = PdfPageOrientation.Portrait;

            //    // instantiate a html to pdf converter object
            //    HtmlToPdf converter = new HtmlToPdf();

            //    // set converter options
            //    converter.Options.PdfPageSize = pageSize;
            //    converter.Options.PdfPageOrientation = pdfOrientation;
            //    converter.Options.WebPageWidth = 1152;
            //    converter.Options.WebPageHeight = 960;

            //    converter.Options.MarginBottom = 0;
            //    converter.Options.MarginTop = 40;
            //    converter.Options.MarginLeft = 40;
            //    converter.Options.MarginRight = 40;

            //headerHtml = "<b>Header</b>";
            //footerHtml = "<b>Footer</b>";


            //    if (customerId == 8001586813601 || customerId == 8000487288901 || customerId == 8001552853901 || customerId == 8000923280901 || customerId == 8001294271701)
            //    {
            //        segment = "Home Loan For Other Segment African";
            //    }

            //    if (customerId == 8000459699201 || customerId == 981511000101 || customerId == 8000703912901)
            //    {
            //        segment = "Home Loan For Wealth Segment African";
            //    }

            //    if (customerId == 8001453741401)
            //    {
            //        segment = "Home Loan For Wealth Segment African";
            //    }
            //    if (customerId == 8000104791201)
            //    {
            //        segment = "Home Loan For Wealth Segment English";
            //    }

            //    if (customerId == 6468878000101)
            //    {
            //        segment = "Home Loan For PML Segment English";
            //    }

            //    if (customerId == 8382274900101)
            //    {
            //        segment = "Home Loan For PML Segment African";
            //    }

            //    if (customerId == 1588756700101 || customerId == 5126658600201 || customerId == 5870511200201 || customerId == 5955131300101 || customerId == 6414207700101 || customerId == 3152385300101 || customerId == 5682491700101 || customerId == 6372679800101 || customerId == 6000734400101 || customerId == 5445044200201 || customerId == 5844741100101 || customerId == 6395898300101 || customerId == 2566840700101 || customerId == 6168293500101 || customerId == 6285740700101 || customerId == 399272200101 || customerId == 505313900101 || customerId == 3640939700101 || customerId == 5973293400201 || customerId == 6259987900101 || customerId == 6329572700101 || customerId == 924640000101 || customerId == 4926004900101 || customerId == 5903081000101 || customerId == 5654457700101)
            //    {
            //        segment = "Home Loan For PML Segment African";
            //    }

            //    if (customerId == 7503010231)
            //    {
            //        segment = "Multi Currency For CIB";
            //    }

            //    if (customerId == 7526721177)
            //    {
            //        segment = "Multi Currency For Wealth";
            //    }
            //    if (customerId == 4)
            //    {
            //        segment = "PPS";
            //    }
            //    //PdfHtmlSection headHtml = new PdfHtmlSection(headerHtml, Path.GetDirectoryName(htmlStatementPath));
            //    PdfHtmlSection headHtml = new PdfHtmlSection(@"C:\UserFiles\HeaderFooters\" + segment + "_header.html");
            //    //PdfHtmlSection headHtml = new PdfHtmlSection(@"C:\UserFiles\Statements\1163\header.html");//Wealth
            //    converter.Header.Add(headHtml);

            //    if(segment.Contains("Corporate Saver"))
            //    {
            //        converter.Header.Height = 100;
            //    }
            //    else
            //    {
            //        converter.Header.Height = 80;
            //    }
            //    //PdfHtmlSection footHtml = new PdfHtmlSection(footerHtml, Path.GetDirectoryName(htmlStatementPath));
            //    PdfHtmlSection footHtml = new PdfHtmlSection(@"C:\UserFiles\HeaderFooters\" + segment + "_footer.html");
            //    converter.Footer.Add(footHtml);
            //    converter.Footer.Height = 80;
            //    if (segment == "Home Loan For Other Segment English" || segment == "Home Loan For Other Segment African" 
            //        || segment == "Home Loan For PML Segment English" || segment == "Home Loan For PML Segment African" 
            //        || segment == "Multi Currency For CIB"
            //        || segment == "Investment Other Segment For English" || segment == "Investment Other Segment For African")
            //    {
            //        converter.Footer.Height = 50;
            //    }

            //    converter.Options.DisplayFooter = true;
            //    converter.Options.DisplayHeader = true;

            //    headHtml.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;
            //    footHtml.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;

            //    // create a new pdf document converting an url
            //    PdfDocument doc = converter.ConvertUrl(url);
            //    doc.Fonts.Add(@"C:\UserFiles\Fonts\MarkPro-Regular.ttf");
            //    doc.Fonts.Add(@"C:\UserFiles\Fonts\Mark Pro.ttf");
            //    doc.Fonts.Add(@"C:\UserFiles\Fonts\Mark Pro Bold.ttf");

            //    if (segment.Contains("Corporate Saver"))
            //    {
            //        PdfFont font = doc.AddFont(PdfStandardFont.Helvetica);
            //        font.Size = 7;

            //        PdfTextElement text1 = new PdfTextElement(447, 75, "Page {page_number} of {total_pages}", font);
            //        if(language == "AFR")
            //        {
            //            text1 = new PdfTextElement(447, 75, "Bladsy {page_number} van {total_pages}", font);
            //        }
            //        text1.ForeColor = System.Drawing.Color.Black;

            //        doc.Header.Add(text1);
            //    }

            //    // save pdf document
            //    doc.Save(outPdfPath);

            //    // close pdf document
            //    doc.Close();
            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}
        }

        #endregion

  //  }
}