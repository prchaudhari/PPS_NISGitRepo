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
        public string WriteToFile(string Message, string fileName, long batchId, long customerId, string baseURL, string outputLocation, bool printPdf = false, string headerHtml = "", string footerHtml = "", string segment = "")
        {
            //string resourceFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\Resources";
            string statementDestPath = outputLocation + "\\Statements" + "\\" + batchId;
            string statementPath = baseURL + "\\Statements" + "\\" + batchId + "\\" + customerId + "\\" + fileName;
            if (!Directory.Exists(statementDestPath))
            {
                Directory.CreateDirectory(statementDestPath);
            }
            string path = statementDestPath + "\\" + customerId + "\\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = path + fileName;
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
            //DirectoryCopy(resourceFilePath, (statementDestPath + "\\common"), true);

            //Printing PDF
            if (printPdf)
            {
                var outputPdfPath = Path.Combine(path, Path.GetFileNameWithoutExtension(fileName) + ".pdf");
                if (GeneratePdf(filepath, outputPdfPath, headerHtml, footerHtml, "", customerId, segment))
                {
                    File.Delete(filepath);
                }
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
        public string CreateAndWriteToZipFile(string htmlstr, string fileName, long batchId, string baseURL, string outputLocation, IDictionary<string, string> filesDictionary = null)
        {
            //create folder to store the html statement files for current batch customers
            string path = outputLocation + "\\Statements" + "\\" + batchId + "\\";
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

            string zipFileVirtualPath = "\\Statements" + "\\" + batchId + "\\statement" + DateTime.Now.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".zip";
            string zipPath = outputLocation + zipFileVirtualPath;

            //create temp folder for common html statement
            string temppath = path + "\\temp\\";
            if (!Directory.Exists(temppath))
            {
                Directory.CreateDirectory(temppath);
            }

            //create temp media folder for common images and videos files of asset library
            string tempmediaPath = temppath + "\\common\\media\\";
            if (!Directory.Exists(tempmediaPath))
            {
                Directory.CreateDirectory(tempmediaPath);
            }

            //folder to save actual common html statement file
            string spath = temppath + "\\statement\\";
            if (!Directory.Exists(spath))
            {
                Directory.CreateDirectory(spath);
            }

            //to delete any common html statement file, if exist 
            string filepath = spath + fileName;
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }

            // Create a html file to write to common html statement
            using (StreamWriter sw = File.CreateText(filepath))
            {
                sw.WriteLine(htmlstr);
            }

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
                            File.Copy(file.Value, Path.Combine(spath, file.Key));
                        }
                        else
                        {
                            File.Copy(file.Value, Path.Combine(mediaPath, file.Key));
                            File.Copy(file.Value, Path.Combine(tempmediaPath, file.Key));
                        }
                    }
                    //webClient.DownloadFile(file.Value, (spath + file.Key));
                }
            }

            //copy all common resource file to current batch statement folder as well as at common statement folder
            DirectoryCopy(resourceFilePath, (path + "\\common"), true);
            DirectoryCopy(resourceFilePath, (temppath + "\\common"), true);

            //create a zip file for common html statement and related resources and media files
            ZipFile.CreateFromDirectory(temppath, zipPath);

            //delete temp folder after zip file created
            string deleteFile = path + "\\temp";
            DirectoryInfo directoryInfo = new DirectoryInfo(deleteFile);
            if (directoryInfo.Exists)
            {
                directoryInfo.Delete(true);
            }

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
                client.setAuthor("NedBank");

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

        /// <summary>
        /// This method helps to format nedbank tenant amount value
        /// </summary>
        /// <param name="amount">The value.</param>
        /// <returns>
        /// Returns the the formatted amount value string
        /// </returns>
        public string NedbankClientAmountFormatter(double amount)
        {
            try
            {
                var totalAmtStr = Convert.ToString(amount).Split(new Char[] { '.', ',' });
                var wholeNo = totalAmtStr[0];
                var franctionNo = totalAmtStr.Length > 1 ? (new string(totalAmtStr[1].Take(2).ToArray())) : "0";

                char[] cArray = wholeNo.ToCharArray();
                Array.Reverse(cArray);

                var tempAmountVal = ".";
                int cnt = 0;
                while (cArray.Length != cnt)
                {
                    tempAmountVal = (tempAmountVal.Length > 1 && tempAmountVal.Length % 4 == 0) ? tempAmountVal + " " + cArray[cnt].ToString() : tempAmountVal + cArray[cnt].ToString();
                    cnt++;
                }

                cArray = tempAmountVal.ToCharArray();
                Array.Reverse(cArray);
                return (new string(cArray) + franctionNo);
            }
            catch (Exception)
            {
                return "0";
            }
        }

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

        public bool GeneratePdf(string htmlStatementPath, string outPdfPath, string headerHtml, string footerHtml, string password, long customerId, string segment)
        {
            try
            {
                //htmlStatementPath = @"C:\UserFiles\Statements\1161\112233\Statement_112233_78_4_19_2022_7_44_29_PM.html";

                // read parameters from the webpage
                string url = htmlStatementPath;

                PdfPageSize pageSize = PdfPageSize.A4;

                PdfPageOrientation pdfOrientation = PdfPageOrientation.Portrait;

                // instantiate a html to pdf converter object
                HtmlToPdf converter = new HtmlToPdf();

                // set converter options
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = 1152;
                converter.Options.WebPageHeight = 960;

                converter.Options.MarginBottom = 0;
                converter.Options.MarginTop = 40;
                converter.Options.MarginLeft = 40;
                converter.Options.MarginRight = 40;

                //headerHtml = "<b>Header</b>";
                //footerHtml = "<b>Footer</b>";


                if (customerId == 8001586813601 || customerId == 8000487288901 || customerId == 8001552853901 || customerId == 8000923280901 || customerId == 8001294271701)
                {
                    segment = "Home Loan For Other Segment African";
                }

                //                if(customerId == 864792200201
                //|| customerId == 1304188500101
                //|| customerId == 8000102211301
                //|| customerId == 8000106182901
                //|| customerId == 5843150200101
                //|| customerId == 5945522700301
                //|| customerId == 8000007005201
                //|| customerId == 8000124438201
                //|| customerId == 8000394683301
                //|| customerId == 8000412014401
                //|| customerId == 8000427087101
                //|| customerId == 8000434269301
                //|| customerId == 8000464315001
                //|| customerId == 8000662518801
                //|| customerId == 4741513000101
                //|| customerId == 5927499900101
                //|| customerId == 6130824100301
                //|| customerId == 8000124462701
                //|| customerId == 8000348752801
                //|| customerId == 8000415282501
                //|| customerId == 8000541267101
                //|| customerId == 8000568063301
                //|| customerId == 8000574542801
                //|| customerId == 8000638861201
                //|| customerId == 8000664423201
                //|| customerId == 8000665703201
                //|| customerId == 8000678298201
                //|| customerId == 403928800101
                //|| customerId == 4958468800101
                //|| customerId == 5937666500101
                //|| customerId == 6021481100101
                //|| customerId == 6435137700101
                //|| customerId == 8000003163301
                //|| customerId == 8000012842901
                //|| customerId == 8000109326701
                //|| customerId == 8000112629501
                //|| customerId == 8000137739501
                //|| customerId == 8000142702001
                //|| customerId == 8000377075301
                //|| customerId == 8000443947401
                //|| customerId == 8000448595801
                //|| customerId == 8000482681401
                //|| customerId == 8000506416701
                //|| customerId == 8000533351501
                //|| customerId == 8000579130001
                //|| customerId == 8000601681801
                //|| customerId == 8000646769001
                //|| customerId == 8000703978301
                //|| customerId == 818043900101
                //|| customerId == 1852632400101
                //|| customerId == 4060074600101
                //|| customerId == 5526246100101
                //|| customerId == 5555837800101
                //|| customerId == 8000121250101
                //|| customerId == 8000130959001
                //|| customerId == 8000369106001
                //|| customerId == 8000511196001
                //|| customerId == 8000519925001
                //|| customerId == 8000530194301
                //|| customerId == 8000551670601
                //|| customerId == 8000568009001
                //|| customerId == 8000631766301
                //|| customerId == 8000657818401
                //|| customerId == 8000662598601
                //|| customerId == 8000715729701
                //|| customerId == 1664709600101
                //|| customerId == 6034097200101
                //|| customerId == 8000008823001
                //|| customerId == 8000115883401
                //|| customerId == 8000122296401
                //|| customerId == 1008987400101
                //|| customerId == 1487629600101
                //|| customerId == 3310654800201
                //|| customerId == 4277031200301
                //|| customerId == 8000437476401
                //|| customerId == 8000449218001
                //|| customerId == 8000460412601
                //|| customerId == 8000549086001
                //|| customerId == 8000641091801
                //|| customerId == 8000667116801
                //|| customerId == 8000682210801
                //|| customerId == 8000736151801
                //|| customerId == 8000758364301
                //|| customerId == 425449800101
                //|| customerId == 6303127700101
                //|| customerId == 8000000049201
                //|| customerId == 8000114023701
                //|| customerId == 8000387474201
                //|| customerId == 8000392138001
                //|| customerId == 8000422435601
                //|| customerId == 8000441440401
                //|| customerId == 8000477911701
                //|| customerId == 8000496968401
                //|| customerId == 8000506412301
                //|| customerId == 8000528606701
                //|| customerId == 8000625235901
                //|| customerId == 8000695835201
                //|| customerId == 8000743384401
                //|| customerId == 8000692662801
                //|| customerId == 8000739389301
                //|| customerId == 8000748880401
                //|| customerId == 8000772716701
                //|| customerId == 8000106192401
                //|| customerId == 8000121269101
                //|| customerId == 8000131615901
                //|| customerId == 8000133057401
                //|| customerId == 8000407371601
                //|| customerId == 8000438129201
                //|| customerId == 8000470883901
                //|| customerId == 8000477909501
                //|| customerId == 8000480136701
                //|| customerId == 8000551627601
                //|| customerId == 8000633158701
                //|| customerId == 8000713291101
                //|| customerId == 8000733364201
                //|| customerId == 8000144147401
                //|| customerId == 8000403413201
                //|| customerId == 8000485888901
                //|| customerId == 8000509658601
                //|| customerId == 8000624527401
                //|| customerId == 8000652119901
                //|| customerId == 8000676897801
                //|| customerId == 8000702129601
                //|| customerId == 8000705323301
                //|| customerId == 8000728234501
                //|| customerId == 8000771120601
                //|| customerId == 8000016726501
                //|| customerId == 8000123704901
                //|| customerId == 8000128322101
                //|| customerId == 8000129037401
                //|| customerId == 8000354112501
                //|| customerId == 8000415272301
                //|| customerId == 8000425605201
                //|| customerId == 8000425628101
                //|| customerId == 8000425699101
                //|| customerId == 8000448526001
                //|| customerId == 8000463689201
                //|| customerId == 8000473389601
                //|| customerId == 8000561122001
                //|| customerId == 8000566649001
                //|| customerId == 8000570610401
                //|| customerId == 8000583177001
                //|| customerId == 8000617330201
                //|| customerId == 8000630040101
                //|| customerId == 5011282600101
                //|| customerId == 5575547200101
                //|| customerId == 8000112665301
                //|| customerId == 8000120591701
                //|| customerId == 8000397872101
                //|| customerId == 8000485887301
                //|| customerId == 8000504405901
                //|| customerId == 8000504492101
                //|| customerId == 8000543740501
                //|| customerId == 8000655361801
                //|| customerId == 8000662568001
                //|| customerId == 8000665773201
                //|| customerId == 8000725079901
                //|| customerId == 8000745841801
                //|| customerId == 1116693700101
                //|| customerId == 8000131607301
                //|| customerId == 8000132311301
                //|| customerId == 8000137713401
                //|| customerId == 8000137798501
                //|| customerId == 8000348715201
                //|| customerId == 8000429558601
                //|| customerId == 8000474793801
                //|| customerId == 8000508901101
                //|| customerId == 8000578404401
                //|| customerId == 8000755192701
                //|| customerId == 1125776800101
                //|| customerId == 4744135300101
                //|| customerId == 6075773900101
                //|| customerId == 8000007043401
                //|| customerId == 8000136207201
                //|| customerId == 8000136240701
                //|| customerId == 8000444634701
                //|| customerId == 8000484064301
                //|| customerId == 8000494499301
                //|| customerId == 8000503264501
                //|| customerId == 8000517538001
                //|| customerId == 8000551651401
                //|| customerId == 8000617358101
                //|| customerId == 8000620699001
                //|| customerId == 8000691991201
                //|| customerId == 8000705346501
                //|| customerId == 49863100101
                //|| customerId == 188639300101
                //|| customerId == 195250100101
                //|| customerId == 272895100201
                //|| customerId == 295892900201
                //|| customerId == 519205200201
                //|| customerId == 536211900101
                //|| customerId == 553598500101
                //|| customerId == 686353900201
                //|| customerId == 757958800201
                //|| customerId == 813662500101
                //|| customerId == 5344661300101
                //|| customerId == 8000000060901
                //|| customerId == 8000153843901
                //|| customerId == 8000500091001
                //|| customerId == 8000690189601
                //|| customerId == 8000716448201
                //|| customerId == 8000729910701
                //|| customerId == 8000739323701
                //|| customerId == 8000773490701
                //|| customerId == 92631100101
                //|| customerId == 692995100101
                //|| customerId == 6484321800101
                //|| customerId == 8000003144001
                //|| customerId == 8000010335401
                //|| customerId == 8000123704101
                //|| customerId == 8000342354101
                //|| customerId == 8000366684501
                //|| customerId == 8000396076901
                //|| customerId == 8000465041001
                //|| customerId == 8000548387701
                //|| customerId == 8000657832801
                //|| customerId == 8000721161801
                //|| customerId == 8000747228401
                //|| customerId == 522734800101
                //|| customerId == 562501100101
                //|| customerId == 565435500101
                //|| customerId == 614664600201
                //|| customerId == 846774300201
                //|| customerId == 911291600301
                //|| customerId == 935070900101
                //|| customerId == 984192800101
                //|| customerId == 1011808500101
                //|| customerId == 1025969300101
                //|| customerId == 1035606800401
                //|| customerId == 1071444400201
                //|| customerId == 1135979900101
                //|| customerId == 1171905700101
                //|| customerId == 1402349400101
                //|| customerId == 1722346500301
                //|| customerId == 5827760800201
                //|| customerId == 8000005678201
                //|| customerId == 8000384229601
                //|| customerId == 8000388179301
                //|| customerId == 8000446066301
                //|| customerId == 8000485832001
                //|| customerId == 4978184900101
                //|| customerId == 5254609300101
                //|| customerId == 8000002451401
                //|| customerId == 8000003136401
                //|| customerId == 8000127619801
                //|| customerId == 8000353428801
                //|| customerId == 8000506406001
                //|| customerId == 8000518223901
                //|| customerId == 8000620673201
                //|| customerId == 8000641014201
                //|| customerId == 8000697275301
                //|| customerId == 8000718936701
                //|| customerId == 8000356694201
                //|| customerId == 8000384217701
                //|| customerId == 8000397838201
                //|| customerId == 8000406653801
                //|| customerId == 8000429579801
                //|| customerId == 8000467584801
                //|| customerId == 8000741934301
                //|| customerId == 8000479365101
                //|| customerId == 8000485887201
                //|| customerId == 8000601668901
                //|| customerId == 8000713203101
                //|| customerId == 8000713244401
                //|| customerId == 8000764831501
                //|| customerId == 8000769452101
                //|| customerId == 168880700101
                //|| customerId == 360248300201
                //|| customerId == 598626700101
                //|| customerId == 621515600101
                //|| customerId == 698450700101
                //|| customerId == 700101100101
                //|| customerId == 719247700101
                //|| customerId == 731104400101
                //|| customerId == 837244300101
                //|| customerId == 904899400101
                //|| customerId == 1076550000101
                //|| customerId == 1206318600101
                //|| customerId == 1417130400401
                //|| customerId == 8000656003101
                //|| customerId == 8000699740901
                //|| customerId == 8000701434501
                //|| customerId == 74926200101
                //|| customerId == 286053800101
                //|| customerId == 341656400101
                //|| customerId == 366087100101
                //|| customerId == 500476800101
                //|| customerId == 611942500301
                //|| customerId == 657550000101
                //|| customerId == 928053400201
                //|| customerId == 1002444400101
                //|| customerId == 1085597300101
                //|| customerId == 1108129000201
                //|| customerId == 1152502100101
                //|| customerId == 1164999900101
                //|| customerId == 1307547700101
                //|| customerId == 1440603200101
                //|| customerId == 1445902200101
                //|| customerId == 1506847800101
                //|| customerId == 1595047200101
                //|| customerId == 1622285500101
                //|| customerId == 1693023600101
                //|| customerId == 1928951700301
                //|| customerId == 1935316900301
                //|| customerId == 2305775700201
                //|| customerId == 2310484900101
                //|| customerId == 2372845500201
                //|| customerId == 2548653300201
                //|| customerId == 2617233600301
                //|| customerId == 2676076800101
                //|| customerId == 3284672900101
                //|| customerId == 72814700101
                //|| customerId == 143180100201
                //|| customerId == 165274400101
                //|| customerId == 258014100201
                //|| customerId == 569902100101
                //|| customerId == 694380100101
                //|| customerId == 1001647300101
                //|| customerId == 1090778800101
                //|| customerId == 1144020000101
                //|| customerId == 1194733600101
                //|| customerId == 1204780700401
                //|| customerId == 1217706800201
                //|| customerId == 1273922000101
                //|| customerId == 1302947700101
                //|| customerId == 1310311100101
                //|| customerId == 1355271700101
                //|| customerId == 1689224400401
                //|| customerId == 1797193700101
                //|| customerId == 1896105300101
                //|| customerId == 2079384000201
                //|| customerId == 2220180900101
                //|| customerId == 2359540500101
                //|| customerId == 2518056400101
                //|| customerId == 2572810000101
                //|| customerId == 2624124000101
                //|| customerId == 2675199000101
                //|| customerId == 2700994700101
                //|| customerId == 2713374700101
                //|| customerId == 2783765200101
                //|| customerId == 2792784000301
                //|| customerId == 2911368100101
                //|| customerId == 2934986200101
                //|| customerId == 3009214100101
                //|| customerId == 3205668100101
                //|| customerId == 3577272400101
                //|| customerId == 3598797900201
                //|| customerId == 3609958700201
                //|| customerId == 3644504500101
                //|| customerId == 1456974400101
                //|| customerId == 1535323400101
                //|| customerId == 2052669700101
                //|| customerId == 2229577700101
                //|| customerId == 2289896000101
                //|| customerId == 2491085900301
                //|| customerId == 2491972700101
                //|| customerId == 2599949200101
                //|| customerId == 2737352900101
                //|| customerId == 2854982200101
                //|| customerId == 2868218100101
                //|| customerId == 2913810500101
                //|| customerId == 3003187300201
                //|| customerId == 3007405200101
                //|| customerId == 3183712100101
                //|| customerId == 3247457400101
                //|| customerId == 3274725800101
                //|| customerId == 3595507600101
                //|| customerId == 1732491800101
                //|| customerId == 1894748800101
                //|| customerId == 1898450900101
                //|| customerId == 2078791000201
                //|| customerId == 2079384000401
                //|| customerId == 2096480100701
                //|| customerId == 2382325400301
                //|| customerId == 2474160100101
                //|| customerId == 2987364900101
                //|| customerId == 2993089200101
                //|| customerId == 3153210800101
                //|| customerId == 3238532000101
                //|| customerId == 3261781400101
                //|| customerId == 3333514400101
                //|| customerId == 3534268800101
                //|| customerId == 3612717700101
                //|| customerId == 3644397300201
                //|| customerId == 3769182400101
                //|| customerId == 3892599700101
                //|| customerId == 3895850500101
                //|| customerId == 4442452700201
                //|| customerId == 4489722300101
                //|| customerId == 4625682000301
                //|| customerId == 4815725400101
                //|| customerId == 4865451500101
                //|| customerId == 4908606600401
                //|| customerId == 4911750000201
                //|| customerId == 4914131400401
                //|| customerId == 5017005500101
                //|| customerId == 5206498800101
                //|| customerId == 3744418900101
                //|| customerId == 3799684800101
                //|| customerId == 4066875600101
                //|| customerId == 4075800200101
                //|| customerId == 4181375700101
                //|| customerId == 4383109100101
                //|| customerId == 4574436500101
                //|| customerId == 4702150400101
                //|| customerId == 4853549600201
                //|| customerId == 4914131400201
                //|| customerId == 4954655200101
                //|| customerId == 5013547100301
                //|| customerId == 5038319700101
                //|| customerId == 5062530600401
                //|| customerId == 5062999600101
                //|| customerId == 5083589900101
                //|| customerId == 5222042000101
                //|| customerId == 5250604900201
                //|| customerId == 5272747800101
                //|| customerId == 5379141600101
                //|| customerId == 5422010300101
                //|| customerId == 5445947800101
                //|| customerId == 5449062300101
                //|| customerId == 5449296600101
                //|| customerId == 5472286200101
                //|| customerId == 5481108800101
                //|| customerId == 5482072600101
                //|| customerId == 5553524900101
                //|| customerId == 5581657900101
                //|| customerId == 5594460100101
                //|| customerId == 5451763400201
                //|| customerId == 5477400900301
                //|| customerId == 5532997900101
                //|| customerId == 5555475500101
                //|| customerId == 5589950900101
                //|| customerId == 5591013900101
                //|| customerId == 5626853100101
                //|| customerId == 5636800400101
                //|| customerId == 5665856600101
                //|| customerId == 5672550000101
                //|| customerId == 5726373700101
                //|| customerId == 5732896100101
                //|| customerId == 5746085600101
                //|| customerId == 5748395300101
                //|| customerId == 5749212900101
                //|| customerId == 5804665700201
                //|| customerId == 5852338800101
                //|| customerId == 5878951600101
                //|| customerId == 5888374900101
                //|| customerId == 5898256000101
                //|| customerId == 5941997300101
                //|| customerId == 5955216000101
                //|| customerId == 5981486200101
                //|| customerId == 6040521900101
                //|| customerId == 132978400101
                //|| customerId == 495707200301
                //|| customerId == 529161100101
                //|| customerId == 536211900501
                //|| customerId == 571064000101
                //|| customerId == 698966500101
                //|| customerId == 885537000101
                //|| customerId == 1101184400101
                //|| customerId == 1106946200101
                //|| customerId == 1201948300301
                //|| customerId == 1204230800101
                //|| customerId == 1378263700101
                //|| customerId == 1421812400101
                //|| customerId == 1452114400101
                //|| customerId == 1787725200101
                //|| customerId == 1819136300101
                //|| customerId == 1943103000101
                //|| customerId == 2131991300201
                //|| customerId == 2317881100101
                //|| customerId == 2793575200101
                //|| customerId == 2866422900201
                //|| customerId == 3080652500101
                //|| customerId == 3103060600101
                //|| customerId == 3284593500201
                //|| customerId == 3567056700101
                //|| customerId == 3583276800201
                //|| customerId == 3713659400101
                //|| customerId == 3726590100101
                //|| customerId == 3758767500101
                //|| customerId == 3791817400101
                //|| customerId == 4009590000101
                //|| customerId == 4050174100101
                //|| customerId == 4064319300101
                //|| customerId == 4104697500101
                //|| customerId == 4174109500101
                //|| customerId == 4238624300101
                //|| customerId == 4452364400101
                //|| customerId == 4978154800201
                //|| customerId == 5112591000101
                //|| customerId == 5425458200101
                //|| customerId == 5478335200101
                //|| customerId == 5503267700101
                //|| customerId == 5524369900201
                //|| customerId == 5579085500101
                //|| customerId == 5626736800101
                //|| customerId == 5639084000101
                //|| customerId == 5656696600101
                //|| customerId == 5671965900101
                //|| customerId == 5727200600101
                //|| customerId == 5759306500101
                //|| customerId == 5761844200101
                //|| customerId == 5808265200101
                //|| customerId == 5810121700101
                //|| customerId == 5819301700101
                //|| customerId == 5827686000101
                //|| customerId == 5839375700101
                //|| customerId == 5852655800301
                //|| customerId == 5873537700101
                //|| customerId == 5918057900101
                //|| customerId == 5923032500201
                //|| customerId == 5981216000101
                //|| customerId == 5998854400101
                //|| customerId == 6022339800101
                //|| customerId == 6027117200201
                //|| customerId == 6027487200201
                //|| customerId == 6028214300101
                //|| customerId == 6036560000101
                //|| customerId == 6042903800101
                //|| customerId == 6047429000101
                //|| customerId == 6049353900101
                //|| customerId == 6060136100101
                //|| customerId == 6062652200301
                //|| customerId == 6063395600101
                //|| customerId == 6072838500201
                //|| customerId == 6094103500101
                //|| customerId == 6096031200101
                //|| customerId == 6113471300101
                //|| customerId == 708681600201
                //|| customerId == 719826400101
                //|| customerId == 813148000101
                //|| customerId == 953690100101
                //|| customerId == 995267900201
                //|| customerId == 1079711200301
                //|| customerId == 1153899400101
                //|| customerId == 1348688500101
                //|| customerId == 1526534300101
                //|| customerId == 1632055000101
                //|| customerId == 1934333300101
                //|| customerId == 1955017700101
                //|| customerId == 1957757800101
                //|| customerId == 2772580000101
                //|| customerId == 3171367100101
                //|| customerId == 3294493300301
                //|| customerId == 3299514900101
                //|| customerId == 3463033000101
                //|| customerId == 3492614300101
                //|| customerId == 3646137400101
                //|| customerId == 3660419400101
                //|| customerId == 3815117200101
                //|| customerId == 3900526500101
                //|| customerId == 4078376400101
                //|| customerId == 4099045100101
                //|| customerId == 4257950000101
                //|| customerId == 4394560800101
                //|| customerId == 4531724700201
                //|| customerId == 4587666900101
                //|| customerId == 4757210400101
                //|| customerId == 4760092200101
                //|| customerId == 4780954100101
                //|| customerId == 4797078900101
                //|| customerId == 5020768100201
                //|| customerId == 5177207600101
                //|| customerId == 5266967700101
                //|| customerId == 5267645500101
                //|| customerId == 5333148900101
                //|| customerId == 5380777100101
                //|| customerId == 5408346400101
                //|| customerId == 5502469300101
                //|| customerId == 5504904500101
                //|| customerId == 5518119000201
                //|| customerId == 5521589900201
                //|| customerId == 5604666600101
                //|| customerId == 5651544600101
                //|| customerId == 5662541100101
                //|| customerId == 5665988300101
                //|| customerId == 5680912700101
                //|| customerId == 5686700000101
                //|| customerId == 5727592800201
                //|| customerId == 5806924900101
                //|| customerId == 5818519000301
                //|| customerId == 5884324200101
                //|| customerId == 5925585500101
                //|| customerId == 5936397400101
                //|| customerId == 5982248000101
                //|| customerId == 6001707500101
                //|| customerId == 6023623300101
                //|| customerId == 6043403100101
                //|| customerId == 6045960100101
                //|| customerId == 6052515600201
                //|| customerId == 6083798600101
                //|| customerId == 6086076000101
                //|| customerId == 6115221800101
                //|| customerId == 6123523000101
                //|| customerId == 6126105900201
                //|| customerId == 6133461500101
                //|| customerId == 6138998000201
                //|| customerId == 6198782300101
                //|| customerId == 6210713600101
                //|| customerId == 6238130400101
                //|| customerId == 3309716400101
                //|| customerId == 3335235300101
                //|| customerId == 3571803800101
                //|| customerId == 3794240900201
                //|| customerId == 3841003100101
                //|| customerId == 3876659300101
                //|| customerId == 4079343600101
                //|| customerId == 4141454400101
                //|| customerId == 4155358100101
                //|| customerId == 4276431000101
                //|| customerId == 4348576600101
                //|| customerId == 4534388900201
                //|| customerId == 4741474100101
                //|| customerId == 4803009700101
                //|| customerId == 4963201000101
                //|| customerId == 5036243700101
                //|| customerId == 5218885700101
                //|| customerId == 5425195000201
                //|| customerId == 5543766100101
                //|| customerId == 5612930000101
                //|| customerId == 5621094500101
                //|| customerId == 5621739700101
                //|| customerId == 5626523600101
                //|| customerId == 5645356300101
                //|| customerId == 5645576700101
                //|| customerId == 5679121600101
                //|| customerId == 5698263400101
                //|| customerId == 5710564700101
                //|| customerId == 5748134800101
                //|| customerId == 5748299400201
                //|| customerId == 5824643000101
                //|| customerId == 5825470800101
                //|| customerId == 5859122600101
                //|| customerId == 5861730000101
                //|| customerId == 5875824500101
                //|| customerId == 5883530900101
                //|| customerId == 5912276800101
                //|| customerId == 5985752500101
                //|| customerId == 5995430100101
                //|| customerId == 6043464000101
                //|| customerId == 6059631400101
                //|| customerId == 6069481900101
                //|| customerId == 6071310400101
                //|| customerId == 6169084700101
                //|| customerId == 6184907600301
                //|| customerId == 6184973200101
                //|| customerId == 6275481100101
                //|| customerId == 6275983700201
                //|| customerId == 5638406400101
                //|| customerId == 5651627900201
                //|| customerId == 5683943900101
                //|| customerId == 5687252600101
                //|| customerId == 5816948800101
                //|| customerId == 5833852100101
                //|| customerId == 5856337900101
                //|| customerId == 5884187700101
                //|| customerId == 5884375800101
                //|| customerId == 5892500600101
                //|| customerId == 5905581000101
                //|| customerId == 5924003600101
                //|| customerId == 5939486500101
                //|| customerId == 5946199500101
                //|| customerId == 5950002900101
                //|| customerId == 5962766900101
                //|| customerId == 6018994700201
                //|| customerId == 6026063900101
                //|| customerId == 6054969500101
                //|| customerId == 6112466100201
                //|| customerId == 6125096600101
                //|| customerId == 6149179200101
                //|| customerId == 6207330800201
                //|| customerId == 6274212200101
                //|| customerId == 941380100101
                //|| customerId == 941772500401
                //|| customerId == 1031815200401
                //|| customerId == 1090053900201
                //|| customerId == 1100250800101
                //|| customerId == 1106689400201
                //|| customerId == 1532927900101
                //|| customerId == 1658259900101
                //|| customerId == 1746678900201
                //|| customerId == 2142994200201
                //|| customerId == 2269974100101
                //|| customerId == 2322338300101
                //|| customerId == 2476510500101
                //|| customerId == 2661507500201
                //|| customerId == 2682852100101
                //|| customerId == 2808139100101
                //|| customerId == 3001740800101
                //|| customerId == 3021550000101
                //|| customerId == 3048380200101
                //|| customerId == 3180277500101
                //|| customerId == 3207818900101
                //|| customerId == 3297016300201
                //|| customerId == 3582579900101
                //|| customerId == 3768030900101
                //|| customerId == 3917265200101
                //|| customerId == 3964293900101
                //|| customerId == 3965263200101
                //|| customerId == 4410778500101
                //|| customerId == 4572027700101
                //|| customerId == 4797078900201
                //|| customerId == 4844459300101
                //|| customerId == 4957833000101
                //|| customerId == 5115567700101
                //|| customerId == 5281768000101
                //|| customerId == 5450980100101
                //|| customerId == 5452018800101
                //|| customerId == 5486336300101
                //|| customerId == 5544130200201
                //|| customerId == 5728023300101
                //|| customerId == 5733924400101
                //|| customerId == 5828414900101
                //|| customerId == 5917172300101
                //|| customerId == 6009532200101
                //|| customerId == 6010607500101
                //|| customerId == 6014261900101
                //|| customerId == 6098302200101
                //|| customerId == 6104021400101
                //|| customerId == 6129207500101
                //|| customerId == 6132328900101
                //|| customerId == 6167123700101
                //|| customerId == 6167123700201
                //|| customerId == 6217902300101
                //|| customerId == 6219559900101
                //|| customerId == 6260969800101
                //|| customerId == 4132577900101
                //|| customerId == 4145267700301
                //|| customerId == 4243676600101
                //|| customerId == 4337761900101
                //|| customerId == 4492076400101
                //|| customerId == 4626090500101
                //|| customerId == 4811878400101
                //|| customerId == 4828764800101
                //|| customerId == 4864362300101
                //|| customerId == 5007276600101
                //|| customerId == 5053744700101
                //|| customerId == 5091141800101
                //|| customerId == 5300206600101
                //|| customerId == 5307926200101
                //|| customerId == 5329767200101
                //|| customerId == 5334981200101
                //|| customerId == 5386068500101
                //|| customerId == 5427352200101
                //|| customerId == 5474432000301
                //|| customerId == 5524172600101
                //|| customerId == 5571343900101
                //|| customerId == 5618400500101
                //|| customerId == 5657909800101
                //|| customerId == 5660373100101
                //|| customerId == 5663202400101
                //|| customerId == 5666355600101
                //|| customerId == 5748299400101
                //|| customerId == 5760768600101
                //|| customerId == 5859067900101
                //|| customerId == 5866810400101
                //|| customerId == 5878630700101
                //|| customerId == 5891020800101
                //|| customerId == 5907363000101
                //|| customerId == 5938823300101
                //|| customerId == 5949753300101
                //|| customerId == 5971075600101
                //|| customerId == 5975781600101
                //|| customerId == 6046116600101
                //|| customerId == 6205096600101
                //|| customerId == 6256913000101
                //|| customerId == 6273079400101
                //|| customerId == 6275390000101
                //|| customerId == 6284109400101
                //|| customerId == 6326231700101
                //|| customerId == 6330343900101
                //|| customerId == 6345829100101
                //|| customerId == 6368147600101
                //|| customerId == 6467998100101
                //|| customerId == 6470460200101
                //|| customerId == 6470536300101
                //|| customerId == 6482779800101
                //|| customerId == 6486724100101
                //|| customerId == 6501085800101
                //|| customerId == 6515584200101
                //|| customerId == 6177200101
                //|| customerId == 6048961200201
                //|| customerId == 6065398700101
                //|| customerId == 6080435400101
                //|| customerId == 6105293500101
                //|| customerId == 6114858400101
                //|| customerId == 6117466400101
                //|| customerId == 6134421500101
                //|| customerId == 6139746600101
                //|| customerId == 6151418400101
                //|| customerId == 6170665800101
                //|| customerId == 6172872500201
                //|| customerId == 6182071500101
                //|| customerId == 6223064000101
                //|| customerId == 6237702200101
                //|| customerId == 6251446600101
                //|| customerId == 6337455700101
                //|| customerId == 6369330200101
                //|| customerId == 6373753300101
                //|| customerId == 6403787700101
                //|| customerId == 6416395200201
                //|| customerId == 6421900200101
                //|| customerId == 6435984100101
                //|| customerId == 6466840200101
                //|| customerId == 6519459800101
                //|| customerId == 6546665300101
                //|| customerId == 8000001721901
                //|| customerId == 8000002447101
                //|| customerId == 8000003171201
                //|| customerId == 8000003197601
                //|| customerId == 8000005646801
                //|| customerId == 8000010366401
                //|| customerId == 8000100898201
                //|| customerId == 8000104756901
                //|| customerId == 8000104760201
                //|| customerId == 8000106105401
                //|| customerId == 8000106185601
                //|| customerId == 8000108686001
                //|| customerId == 8000109355901
                //|| customerId == 8000113302401
                //|| customerId == 8000114007401
                //|| customerId == 8000121266001
                //|| customerId == 8000123782101
                //|| customerId == 8000128326701
                //|| customerId == 8000129047501
                //|| customerId == 8000134857901
                //|| customerId == 8000138770701
                //|| customerId == 8000139431901
                //|| customerId == 8000146618901
                //|| customerId == 8000147352301
                //|| customerId == 8000151354001
                //|| customerId == 8000156625401
                //|| customerId == 8000346285301
                //|| customerId == 8000352766301
                //|| customerId == 8000354175401
                //|| customerId == 6353540200101
                //|| customerId == 6364710500101
                //|| customerId == 6375931300101
                //|| customerId == 6384364500101
                //|| customerId == 6436344200101
                //|| customerId == 6438621700101
                //|| customerId == 6439002200101
                //|| customerId == 6494412200101
                //|| customerId == 8000004903101
                //|| customerId == 8000007081601
                //|| customerId == 8000014270301
                //|| customerId == 8000014280801
                //|| customerId == 8000017460001
                //|| customerId == 8000019986701
                //|| customerId == 8000099898201
                //|| customerId == 8000105460501
                //|| customerId == 8000110145201
                //|| customerId == 8000112687101
                //|| customerId == 8000123763301
                //|| customerId == 8000126906301
                //|| customerId == 8000130901601
                //|| customerId == 8000130980901
                //|| customerId == 8000132334801
                //|| customerId == 8000143431401
                //|| customerId == 8000144143501
                //|| customerId == 8000150640901
                //|| customerId == 8000152016501
                //|| customerId == 8000156646901
                //|| customerId == 8000157707501
                //|| customerId == 8000341660901
                //|| customerId == 8000343001401
                //|| customerId == 8000345506601
                //|| customerId == 8000354142201
                //|| customerId == 8000355988901
                //|| customerId == 8000356686701
                //|| customerId == 8000358063401
                //|| customerId == 8000361308801
                //|| customerId == 8000364559201
                //|| customerId == 8000369195801
                //|| customerId == 8000371768301
                //|| customerId == 8000372401101
                //|| customerId == 8000374920201
                //|| customerId == 8000374945201
                //|| customerId == 8000374991901
                //|| customerId == 8000377003601
                //|| customerId == 8000381029801
                //|| customerId == 8000386712101
                //|| customerId == 8000387427001
                //|| customerId == 6265710000101
                //|| customerId == 6286727500101
                //|| customerId == 6330367500101
                //|| customerId == 6335730900101
                //|| customerId == 6382481100101
                //|| customerId == 6411220000101
                //|| customerId == 6438102600101
                //|| customerId == 6444052000101
                //|| customerId == 6462374300101
                //|| customerId == 6541378100101
                //|| customerId == 8000000096701
                //|| customerId == 8000001784401
                //|| customerId == 8000002436401
                //|| customerId == 8000004965701
                //|| customerId == 8000004988501
                //|| customerId == 8000010328501
                //|| customerId == 8000011008401
                //|| customerId == 8000011065601
                //|| customerId == 8000012862801
                //|| customerId == 8000012874201
                //|| customerId == 8000013556301
                //|| customerId == 8000102201501
                //|| customerId == 8000102248401
                //|| customerId == 8000111945201
                //|| customerId == 8000114083701
                //|| customerId == 8000117289201
                //|| customerId == 8000124423401
                //|| customerId == 8000124465001
                //|| customerId == 8000127617401
                //|| customerId == 8000139405401
                //|| customerId == 8000140235301
                //|| customerId == 8000141154301
                //|| customerId == 8000146643901
                //|| customerId == 8000150695601
                //|| customerId == 8000154525901
                //|| customerId == 8000156641601
                //|| customerId == 8000348764701
                //|| customerId == 8000350218201
                //|| customerId == 8000350220701
                //|| customerId == 8000356666401
                //|| customerId == 8000364544901
                //|| customerId == 8000367755901
                //|| customerId == 8000367791701
                //|| customerId == 8000369186001
                //|| customerId == 8000373124201
                //|| customerId == 8000376332901
                //|| customerId == 8000384240101
                //|| customerId == 8000388151101
                //|| customerId == 553784900201
                //|| customerId == 596438800101
                //|| customerId == 616609900501
                //|| customerId == 676606400101
                //|| customerId == 712233500301
                //|| customerId == 748828200201
                //|| customerId == 6156767200101
                //|| customerId == 6188392100101
                //|| customerId == 6241458500201
                //|| customerId == 6309299500201
                //|| customerId == 6343841100201
                //|| customerId == 6350448900101
                //|| customerId == 844495400101
                //|| customerId == 963271100201
                //|| customerId == 982799900201
                //|| customerId == 991708700101
                //|| customerId == 1009936300101
                //|| customerId == 1056172300101
                //|| customerId == 6362674900101
                //|| customerId == 6370788300101
                //|| customerId == 6374690900101
                //|| customerId == 6375861600101
                //|| customerId == 6388695600101
                //|| customerId == 6402382600101
                //|| customerId == 6403662800101
                //|| customerId == 6409382800101
                //|| customerId == 6433333800101
                //|| customerId == 6434525500101
                //|| customerId == 6438561300101
                //|| customerId == 6466023100101
                //|| customerId == 6511913400101
                //|| customerId == 8000004952501
                //|| customerId == 8000008825801
                //|| customerId == 8000008880601
                //|| customerId == 8000013595401
                //|| customerId == 8000014273601
                //|| customerId == 8000018127501
                //|| customerId == 8000019927701
                //|| customerId == 8000019987301
                //|| customerId == 8000099892201)
                //                {
                //                    segment = "Home Loan For Other Segment African";
                //                }

                if (customerId == 8000459699201 || customerId == 981511000101 || customerId == 8000703912901)
                {
                    segment = "Home Loan For Wealth Segment African";
                }

                if (customerId == 8001453741401)
                {
                    segment = "Home Loan For Wealth Segment African";
                }
                if (customerId == 8000104791201)
                {
                    segment = "Home Loan For Wealth Segment English";
                }

                if (customerId == 6468878000101)
                {
                    segment = "Home Loan For PML Segment English";
                }

                if (customerId == 8382274900101)
                {
                    segment = "Home Loan For PML Segment African";
                }

                if (customerId == 1588756700101 || customerId == 5126658600201 || customerId == 5870511200201 || customerId == 5955131300101 || customerId == 6414207700101 || customerId == 3152385300101 || customerId == 5682491700101 || customerId == 6372679800101 || customerId == 6000734400101 || customerId == 5445044200201 || customerId == 5844741100101 || customerId == 6395898300101 || customerId == 2566840700101 || customerId == 6168293500101 || customerId == 6285740700101 || customerId == 399272200101 || customerId == 505313900101 || customerId == 3640939700101 || customerId == 5973293400201 || customerId == 6259987900101 || customerId == 6329572700101 || customerId == 924640000101 || customerId == 4926004900101 || customerId == 5903081000101 || customerId == 5654457700101)
                {
                    segment = "Home Loan For PML Segment African";
                }

                if (customerId == 7503010231)
                {
                    segment = "Multi Currency For CIB";
                }

                if (customerId == 7526721177)
                {
                    segment = "Multi Currency For Wealth";
                }

                //PdfHtmlSection headHtml = new PdfHtmlSection(headerHtml, Path.GetDirectoryName(htmlStatementPath));
                PdfHtmlSection headHtml = new PdfHtmlSection(@"C:\UserFiles\HeaderFooters\" + segment + "_header.html");
                //PdfHtmlSection headHtml = new PdfHtmlSection(@"C:\UserFiles\Statements\1163\header.html");//Wealth
                converter.Header.Add(headHtml);
                
                if(segment.Contains("Corporate Saver"))
                {
                    converter.Header.Height = 100;
                }
                else
                {
                    converter.Header.Height = 80;
                }
                //PdfHtmlSection footHtml = new PdfHtmlSection(footerHtml, Path.GetDirectoryName(htmlStatementPath));
                PdfHtmlSection footHtml = new PdfHtmlSection(@"C:\UserFiles\HeaderFooters\" + segment + "_footer.html");
                converter.Footer.Add(footHtml);
                converter.Footer.Height = 80;
                if (segment == "Home Loan For Other Segment English" || segment == "Home Loan For Other Segment African" || segment == "Home Loan For PML Segment English"
                    || segment == "Home Loan For PML Segment African" || segment == "Multi Currency For CIB")
                {
                    converter.Footer.Height = 50;
                }

                converter.Options.DisplayFooter = true;
                converter.Options.DisplayHeader = true;

                headHtml.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;
                footHtml.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;

                // create a new pdf document converting an url
                PdfDocument doc = converter.ConvertUrl(url);
                doc.Fonts.Add(@"C:\UserFiles\Fonts\MarkPro-Regular.ttf");
                doc.Fonts.Add(@"C:\UserFiles\Fonts\Mark Pro.ttf");
                doc.Fonts.Add(@"C:\UserFiles\Fonts\Mark Pro Bold.ttf");

                // save pdf document
                doc.Save(outPdfPath);

                // close pdf document
                doc.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

    }
}