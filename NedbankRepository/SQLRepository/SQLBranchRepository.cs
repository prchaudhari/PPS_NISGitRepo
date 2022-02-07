// <copyright file="SQLAnalyticsDataRepository.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2017 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace NedbankRepository
{
    #region References
    using NedBankException;
    using NedbankModel;
    using NedbankUtility;
    using NedBankValidationEngine;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Text;
    using Unity;

    #endregion

    /// <summary>
    /// This class represents repository layer of accet library for crud operation.
    /// </summary>
    /// <seealso cref="NedbankRepository.IBranchRepository" />
    public class SQLBranchRepository : IBranchRepository
    {

        #region Private Members

        NedbankEntities db;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializing instance of class.
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        public SQLBranchRepository(IUnityContainer unityContainer)
        {
            db = new NedbankEntities();
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Get branch by id
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public BranchInformation GetBranchById(long branchId, string tenantCode)
        {
            var branchRecord = db.NB_BranchMaster.Where(m => m.Id == branchId && m.TenantCode == tenantCode).FirstOrDefault();

            return new BranchInformation()
            {
                AddressLine0 = branchRecord.AddressLine0,
                BranchId = branchRecord.BranchId,
                AddressLine1 = branchRecord.AddressLine1,
                AddressLine2 = branchRecord.AddressLine2,
                AddressLine3 = branchRecord.AddressLine3,
                AddressLine4 = branchRecord.AddressLine4,
                ContactNo = branchRecord.ContactNo,
                Id = branchRecord.Id,
                Name = branchRecord.Name,
                TenantCode = tenantCode,
                VatRegNo = branchRecord.VatRegNo
            };
        }


        #endregion
    }
}