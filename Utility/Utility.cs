namespace nIS
{
    #region References

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
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
        public string WriteToFile(string Message, string fileName, long batchId, long customerId, string baseURL, string outputLocation)
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
            //string destPath = baseURL + "\\Statements";
            string path = outputLocation + "\\Statements" + "\\" + batchId + "\\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string resourceFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\Resources";
            //string resourceFilePath = destPath + "\\" + batchId + "\\common";
            string zipFileVirtualPath = "\\Statements" + "\\" + batchId + "\\statement" + DateTime.Now.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".zip";
            string zipfilepath = baseURL + zipFileVirtualPath;
            string zipPath = outputLocation + zipFileVirtualPath;
            string temppath = path + "\\temp\\";
            if (!Directory.Exists(temppath))
            {
                Directory.CreateDirectory(temppath);
            }

            string spath = temppath + "\\statement\\";
            if (!Directory.Exists(spath))
            {
                Directory.CreateDirectory(spath);
            }
            string filepath = spath + fileName;
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }

            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(filepath))
            {
                sw.WriteLine(htmlstr);
            }

            if (filesDictionary != null && filesDictionary?.Count > 0)
            {
                //WebClient webClient = new WebClient();
                foreach (KeyValuePair<string, string> file in filesDictionary)
                {
                    if (File.Exists(file.Value))
                    {
                        File.Copy(file.Value, Path.Combine(spath, file.Key));
                    }
                    //webClient.DownloadFile(file.Value, (spath + file.Key));
                }
            }

            DirectoryCopy(resourceFilePath, (path + "\\common"), true);
            DirectoryCopy(resourceFilePath, (temppath + "\\common"), true);
            ZipFile.CreateFromDirectory(temppath, zipPath);

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
        public bool DeleteUnwantedDirectory(long batchId, long customerId, string baseURL)
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
        /// <returns>
        /// Returns the true if pdf generated successfully, otherwise false
        /// </returns>
        public bool HtmlStatementToPdf(string htmlStatementPath, string outPdfPath)
        {
            var isPdfSuccess = false;
            try
            {
                var client = new pdfcrowd.HtmlToPdfClient("demo", "ce544b6ea52a5621fb9d55f8b542d14d");
                client.setPageWidth("12in");
                client.setPageHeight("10in");
                client.setRenderingMode("viewport");
                client.setSmartScalingMode("content-fit");
                client.setJpegQuality(80);
                client.setConvertImagesToJpeg("all");
                client.setImageDpi(340);
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


        #endregion

    }
}