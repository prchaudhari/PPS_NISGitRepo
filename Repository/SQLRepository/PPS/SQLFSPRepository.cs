namespace nIS
{

    #region References
    using Dapper;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Linq.Dynamic;
    using Unity;
    #endregion
    public class SQLPPSRepository : IPPSRepository
    {
        #region Private Members

        /// <summary>
        /// The validation engine object
        /// </summary>
        IValidationEngine validationEngine = null;

        /// <summary>
        /// The connection string
        /// </summary>
        private string connectionString = string.Empty;

        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The configurationutility
        /// </summary>
        private IConfigurationUtility configurationutility = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializing instance of class.
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        public SQLPPSRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
        }

        #endregion

        #region Public Functions
             
        public List<spIAA_PaymentDetail> spIAA_PaymentDetail_fspstatement(string tenantCode)
        {
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (var connection = new SqlConnection(this.connectionString))
                {                   
                    connection.Open();
                    
                    //   @START_ID bigint,
                    //   @END_ID     bigint, 
                    //@FSP_REF varchar(50),
                    //@FSP_Kind   int
                    var param1 = new SqlParameter("@START_ID", 19296386638);
                    var param2 = new SqlParameter("@END_ID", 192967705831);
                    var param3 = new SqlParameter("@FSP_REF", "124560909");
                    var param4 = new SqlParameter("@FSP_Kind", 1030);
               
                    // Using Dapper Query method for a stored procedure with parameters
                    var result = connection.Query<spIAA_PaymentDetail>(
                        "sysdba.spIAA_PaymentDetail",
                        new { START_ID = param1.Value, END_ID = param2.Value, FSP_REF = param3.Value, FSP_Kind = param4.Value },
                        commandType: CommandType.StoredProcedure, commandTimeout: 4000
                    ).ToList();
                    //var result = connection.Query("select Top 10 * from FSP_Input");

                    return result;
                }            
            }
            catch (Exception ex)
            {
                throw;
            }
           
        }

        public List<spIAA_Commission_Detail> spIAA_Commission_Detail_ppsStatement(string tenantCode)
        {
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (var connection = new SqlConnection(this.connectionString))
                {

                    connection.Open();
                 //   (@INT_EXT_REF VARCHAR(MAX)
	                //,@MEASURE_TYPE VARCHAR(MAX)
	                //,@CREDIT_ORGANISATION VARCHAR(MAX)
	                //,@DATE_FROM DATETIME
                 //   , @DATE_TO DATETIME
	                //,@AGR_KIND VARCHAR(MAX))
                    var param1 = new SqlParameter("@INT_EXT_REF", "");
                    var param2 = new SqlParameter("@MEASURE_TYPE", "");
                    var param3 = new SqlParameter("@CREDIT_ORGANISATION", "124560909");
                    var param4 = new SqlParameter("@DATE_FROM", DateTime.Now);
                    var param5 = new SqlParameter("@DATE_TO", DateTime.Now);
                    var param6 = new SqlParameter("@AGR_KIND", "");
                
                    // Using Dapper Query method for a stored procedure with parameters
                    var result = connection.Query<spIAA_Commission_Detail>(
                        "sysdba.spIAA_Commission_Detail",
                        new { INT_EXT_REF = param1.Value, MEASURE_TYPE = param2.Value, CREDIT_ORGANISATION = param3.Value, DATE_FROM = param4.Value,
                            DATE_TO = param4.Value,AGR_KIND = param4.Value },
                        commandType: CommandType.StoredProcedure, commandTimeout: 4000
                    ).ToList();
                    //var result = connection.Query("select Top 10 * from FSP_Input");

                    return result;
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IList<ProductViewModel> Get_Products(string tenantCode)
        {
            IList<ProductViewModel> Records = new List<ProductViewModel>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    return nISEntitiesDataContext.Products.Select(x => new ProductViewModel()
                    {
                        Id = x.Id,
                        Name = x.Name
                    }).OrderBy(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

              
        #endregion

        #region Private Method

        /// <summary>
        /// This method help to set and validate connection string
        /// </summary>
        /// <param name="tenantCode">The tenant code</param>
        /// <exception cref="NedBankException.ConnectionStringNotFoundException"></exception>
        private void SetAndValidateConnectionString(string tenantCode)
        {
            try
            {
                this.connectionString = CommonVariable.ConnectionString; ;// this.configurationutility.GetConnectionString(ModelConstant.COMMON_SECTION, ModelConstant.NIS_CONNECTION_STRING, ModelConstant.CONFIGURATON_BASE_URL, ModelConstant.TENANT_CODE_KEY, tenantCode);

                if (!this.validationEngine.IsValidText(this.connectionString))
                {
                    throw new ConnectionStringNotFoundException(tenantCode);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        #endregion
     
    }
}
