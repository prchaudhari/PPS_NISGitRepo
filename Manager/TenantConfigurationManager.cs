// <copyright file="TenantConfigurationManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Unity;
    using Websym.Core.TenantManager;

    #endregion


    /// <summary>
    /// This class implements manager layer of asset library manager.
    /// </summary>
    public class TenantConfigurationManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The asset library repository.
        /// </summary>
        ITenantConfigurationRepository tenantConfigurationRepository = null;

        /// <summary>
        /// The utility object
        /// </summary>
        private IConfigurationUtility configurationUtility = null;

        /// <summary>
        /// The crypto manager
        /// </summary>
        private readonly ICryptoManager cryptoManager;

        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for asset library manager, which initialise
        /// asset library repository.
        /// </summary>
        /// <param name="container">IUnity container implementation object.</param>
        public TenantConfigurationManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.configurationUtility = new ConfigurationUtility(this.unityContainer);
                this.cryptoManager = this.unityContainer.Resolve<ICryptoManager>();
                this.tenantConfigurationRepository = this.unityContainer.Resolve<ITenantConfigurationRepository>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Public Methods

        #region Tenant Configuration Functions

        /// <summary>
        /// This method is used to get tenant configuration
        /// </summary>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public IList<TenantConfiguration> GetTenantConfigurations(string tenantCode)
        {
            try
            {
                return this.tenantConfigurationRepository.GetTenantConfigurations(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method is used to get tenant configuration
        /// </summary>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public IList<TenantSubscription> GetTenantSubscriptions(ClientSearchParameter clientSearchParameter, string tenantCode)
        {
            try
            {
                IList<TenantSubscription> tenantSubscriptions = new List<TenantSubscription>();

                tenantSubscriptions = this.tenantConfigurationRepository.GetTenantSubscriptions(tenantCode);
                TenantSearchParameter tenantSearchParameter = new TenantSearchParameter();
                tenantSearchParameter.SortingParameter = new Websym.Core.TenantManager.SortParameter();
                tenantSearchParameter.SortingParameter.SortColumn = "TenantCode";
                tenantSearchParameter.TenantType = "Tenant";
                tenantSearchParameter.IsActive = true;
                tenantSearchParameter.ParentTenantCode = clientSearchParameter.ParentTenantCode;
                var tenants = this.configurationUtility.GetTenant(tenantSearchParameter);
                IList<TenantSubscription> newTenantSubscriptions = new List<TenantSubscription>();
                tenants.ToList().ForEach(item =>
                {
                    TenantSubscription tenantSubscription = new TenantSubscription();
                    tenantSubscription.TenantCode = new Guid(item.TenantCode);
                    tenantSubscription.TenantName = item.TenantName;
                    var data = tenantSubscriptions.Where(i => i.TenantCode.ToString() == item.TenantCode).ToList();
                    if (data.Count > 0)
                    {
                        tenantSubscription.SubscriptionEndDate = data[0].SubscriptionEndDate;
                        tenantSubscription.SubscriptionStartDate = data[0].SubscriptionStartDate;
                        tenantSubscription.SubscriptionKey = data[0].SubscriptionKey;
                        tenantSubscription.LastModifiedOn = data[0].LastModifiedOn;
                        tenantSubscription.LastModifiedBy = data[0].LastModifiedBy;
                        tenantSubscription.LastModifiedName = data[0].LastModifiedName;

                    }
                    newTenantSubscriptions.Add(tenantSubscription);
                });

                return newTenantSubscriptions;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method is used to get tenant configuration
        /// </summary>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public TenantSubscription GetTenantSubscription(string tenantCode)
        {
            try
            {
                TenantSubscription tenantSubscriptions = new TenantSubscription();

                tenantSubscriptions = this.tenantConfigurationRepository.GetTenantSubscription(tenantCode);

                return tenantSubscriptions;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }


        /// <summary>
        /// This method will call add save tenant configuration method of repository.
        /// </summary>
        /// <param name="setting">tenant configuration are to be add.</param>
        /// <param name="tenantCode">Tenant code of asset library.</param>
        /// <returns>
        /// Returns true if entities added successfully, false otherwise.
        /// </returns>
        public bool Save(TenantConfiguration setting, string tenantCode)
        {
            try
            {
                this.IsValidTenantConfiguration(setting, tenantCode);
                return tenantConfigurationRepository.Save(setting, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool AddTenantSubscriptions(IList<TenantSubscription> tenantSubscriptions, string tenantCode)
        {
            try
            {
                this.IsValidTenantSubscription(tenantSubscriptions, tenantCode);
             
                tenantSubscriptions.ToList().ForEach(item =>
                {
                    //var bytes = BitConverter.GetBytes(item.SubscriptionEndDate.Ticks);
                    //Array.Resize(ref bytes, 16);
                    //guid = new Guid(bytes);
                    //item.SubscriptionKey = guid.ToString();

                    string encryptedText = this.cryptoManager.Encrypt(item.SubscriptionEndDate.ToUniversalTime().ToString());
                    item.SubscriptionKey = encryptedText;
                    //string decryptedText= this.cryptoManager.Decrypt(encryptedText);


                });
                ////var dateBytes = guid.ToByteArray();

                ////Array.Resize(ref dateBytes, 8);

                ////var date = new DateTime((long)BitConverter.ToUInt64(dateBytes, 0));


                return tenantConfigurationRepository.AddTenantSubscriptions(tenantSubscriptions, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion


        #endregion

        #region Private Methods


        #region Tenant Configuration


        /// <summary>
        /// This method is responsible for validate asset s.
        /// </summary>
        /// <param name="assets"></param>
        /// <param name="tenantCode"></param>
        private void IsValidTenantConfiguration(TenantConfiguration item, string tenantCode)
        {
            try
            {

                InvalidAssetException invalidAssetException = new InvalidAssetException(tenantCode);

                try
                {
                    item.IsValid();
                }
                catch (Exception ex)
                {
                    invalidAssetException.Data.Add(item.Identifier, ex.Data);
                }


                if (invalidAssetException.Data.Count > 0)
                {
                    throw invalidAssetException;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method is responsible for validate asset subscription.
        /// </summary>
        /// <param name="assets"></param>
        /// <param name="tenantCode"></param>
        private void IsValidTenantSubscription(IList<TenantSubscription> tenantSubscriptions, string tenantCode)
        {
            try
            {

                InvalidAssetException invalidAssetException = new InvalidAssetException(tenantCode);
                tenantSubscriptions.ToList().ForEach(item =>
                {
                    try
                    {
                        item.IsValid();
                    }
                    catch (Exception ex)
                    {
                        invalidAssetException.Data.Add(item.Identifier, ex.Data);
                    }
                    if (invalidAssetException.Data.Count > 0)
                    {
                        throw invalidAssetException;
                    }
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion


        #endregion
    }
}
