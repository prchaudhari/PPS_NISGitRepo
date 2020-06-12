

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System.Net.Mail;
    using System.Net.Http;
    //using Websym.Core.ConfigurationManager;
    //using Websym.Core.ResourceManager;
    //using Websym.Core.EventManager;
    //using Websym.Core.NotificationEngine;
    using System.Configuration;
    using System.Reflection;
    using System.Net.Http.Headers;
    using Websym.Core.ConfigurationManager;
    using Unity;
    using Websym.Core.EventManager;

    #endregion

    public class ConfigurationUtility : IConfigurationUtility
    {
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        #region Constructore
        public ConfigurationUtility(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
        }
        #endregion

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
                #region DLL related changes

                //container = unityresolver.GetDependency();
                Websym.Core.ConfigurationManager.ConfigurationManager configmanager = null;
                //configmanager = new Websym.Core.ConfigurationManager.ConfigurationManager(this.unityContainer);
                configurationSectionList = configmanager.GetConfigurations(configurationSearchParameter, tenantCode.ToString())?.ToList();

                #endregion

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
                //sqlConnectionString = "metadata=res://*/nVidYoDataContext.csdl|res://*/nVidYoDataContext.ssdl|res://*/nVidYoDataContext.msl;provider=System.Data.SqlClient;provider connection string=';Data Source=192.168.100.7;Initial Catalog=nvidyo;User ID=sa;Password=Admin@123;multipleactiveresultsets=True;application name=EntityFramework';";
                //return sqlConnectionString;
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

        #region other component integration api

        //public IList<Websym.Core.EventManager.Event> AddUserNotificationSubscription(EventSearchParameter eventSearchParameter, SubscrptionDeliveryMode deliveryMode, string userIdentifier, string contactNumber, string emailAddress, string tenantCode)
        //{
        //    try
        //    {
        //        IList<Websym.Core.EventManager.Event> eventList = null;

        //        #region DLL related changes

        //        #region Add & Get Entity 

        //        Websym.Core.EventManager.EventManager eventmanager = null;
        //        eventmanager = new Websym.Core.EventManager.EventManager(this.unityContainer);
        //        eventList = eventmanager.GetEvents(eventSearchParameter, tenantCode.ToString())?.ToList();

        //        //if event is not exist add events for specific client(While added client )
        //        if (eventList?.Count <= 0 || eventList == null)
        //        {
        //            IList<Event> events = new List<Event>();
        //            events.Add(new Event()
        //            {
        //                EventCode = 1,
        //                EventName = "UserAdd",
        //                EntityName = "User",
        //                ComponentCode = "nVidYo",
        //                IsNotificationEnable = true,
        //                NotifyAllSubscribers = false

        //            });
        //            events.Add(new Event()
        //            {
        //                EventCode = 2,
        //                EventName = "ForgotPassword",
        //                EntityName = "User",
        //                ComponentCode = "nVidYo",
        //                IsNotificationEnable = true,
        //                NotifyAllSubscribers = false

        //            });
        //            bool eventres = eventmanager.AddEvents(events, tenantCode);
        //            if (eventres == true)
        //            {
        //                eventList = eventmanager.GetEvents(eventSearchParameter, tenantCode.ToString())?.ToList();

        //                #region Add Template

        //                Websym.Core.NotificationEngine.TemplateManager templateManager = new TemplateManager(this.unityContainer);
        //                IList<Template> templates = new List<Template>();
        //                IList<string> list = new List<string>();
        //                list.Add("FIRSTNAME");
        //                list.Add("HTMLLink");
        //                templates.Add(new Template()
        //                {
        //                    TemplateCode = "1",
        //                    ComponentCode = "nVidYo",
        //                    EventCode = eventList.FirstOrDefault().EventCode,
        //                    EntityName = "User",
        //                    DeliveryMode = Websym.Core.NotificationEngine.TemplateDeliveryMode.HTMLEmail,
        //                    TemplateSubject = "User Added",
        //                    Attributes = list,
        //                    TemplateBody = "Hello |FIRSTNAME|,<br/><br/>Please <a href='|HTMLLink|'>click here</a>  to activate your account. <br/><br/><br/>Regards,<br/>nVidYo Team",
        //                    IsActive = true,
        //                    EmailProvider = EmailProvider.SMTP
        //                });

        //                templates.Add(new Template()
        //                {
        //                    TemplateCode = "2",
        //                    ComponentCode = "nVidYo",
        //                    EventCode = eventList.LastOrDefault().EventCode,
        //                    EntityName = "User",
        //                    DeliveryMode = Websym.Core.NotificationEngine.TemplateDeliveryMode.HTMLEmail,
        //                    TemplateSubject = "Reset Password",
        //                    Attributes = list,
        //                    TemplateBody = "Hello |FIRSTNAME|,<br/><br/>Please <a href='|HTMLLink|'>click here</a>  to reactivate your account. <br/><br/><br/>Regards,<br/>nVidYo Team",
        //                    IsActive = true,
        //                    EmailProvider = EmailProvider.SMTP
        //                });

        //                bool temres = templateManager.AddTemplates(templates, tenantCode);

        //                if (temres == false)
        //                {
        //                    throw new InvalidTemplateException(tenantCode);
        //                }

        //                #endregion
        //            }
        //            else
        //            {
        //                throw new InvalidEventException(tenantCode);
        //            }
        //        }

        //        #endregion

        //        #region Add Subscription

        //        IList<Subscription> subscriptions = eventList?.Select(eventDetail => new Subscription()
        //        {
        //            ComponentCode = eventDetail.ComponentCode,
        //            EntityName = eventDetail.EntityName,
        //            EventCode = eventDetail.EventCode,
        //            UserIdentifier = userIdentifier,
        //            MobileNumber = contactNumber,
        //            EmailAddress = emailAddress,
        //            DeliveryMode = deliveryMode,
        //            IsActive = true
        //        })
        //            .ToList();

        //        // Websym.Core.EventManager.EventManager eventmanager = null;
        //        Websym.Core.NotificationEngine.SubscriptionManager subscriptionManager = null;
        //        subscriptionManager = new Websym.Core.NotificationEngine.SubscriptionManager(this.unityContainer);
        //        bool res = subscriptionManager.AddSubscriptions(subscriptions, tenantCode.ToString());

        //        #endregion

        //        if (res == true)
        //        {
        //            return eventList;
        //        }

        //        #endregion

        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        //public bool SendNotification(EventContext eventContext, SubscrptionDeliveryMode deliveryMode, string tenantCode)
        //{
        //    try
        //    {
        //        Websym.Core.NotificationEngine.NotificationManager noificationManager = null;
        //        SubscrptionDeliveryMode delivery = (SubscrptionDeliveryMode)Enum.Parse(typeof(SubscrptionDeliveryMode), deliveryMode.ToString());

        //        noificationManager = new Websym.Core.NotificationEngine.NotificationManager(unityContainer);
        //        bool res = noificationManager.ProcessNotification(eventContext, delivery);
        //        return res;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        #endregion
    }
}
