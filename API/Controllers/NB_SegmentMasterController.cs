using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Unity;
using Websym.Core.TenantManager;

namespace nIS.Controllers
{
    public class NB_SegmentMasterController : ApiController
    {
        /// <summary>
        /// The connection string
        /// </summary>
        private string connectionString = string.Empty;

        /// <summary>
        /// The validation engine object
        /// </summary>
        IValidationEngine validationEngine = null;

        /// <summary>
        /// The utility object
        /// </summary>
        private IConfigurationUtility configurationutility = null;

        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        public NB_SegmentMasterController(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
        }

        // GET: api/NB_SegmentMaster
        public List<Segment> GetNB_SegmentMaster()
        {
            string tenantCode = Helper.CheckTenantCode(Request.Headers);
            this.SetAndValidateConnectionString(tenantCode);
            List<NB_SegmentMaster> segments = new List<NB_SegmentMaster>();
            using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
            {
                segments = nISEntitiesDataContext.NB_SegmentMaster.ToList();
            }

            return segments.OrderBy(m => m.Name).Select(n => new Segment()
            {
                Id = n.Id,
                Name = n.Name,
                Code = n.Code,
                SegmentTypeId = n.SegmentTypeId
            }).ToList();
        }

        /// <summary>
        /// This method help to set and validate connection string
        /// </summary>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        private void SetAndValidateConnectionString(string tenantCode)
        {
            try
            {
                this.connectionString = validationEngine.IsValidText(this.connectionString) ? this.connectionString : this.configurationutility.GetConnectionString(ModelConstant.COMMON_SECTION, ModelConstant.NIS_CONNECTION_STRING, ModelConstant.CONFIGURATON_BASE_URL, ModelConstant.TENANT_CODE_KEY, tenantCode);
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

        // GET: api/NB_SegmentMaster/5
        [ResponseType(typeof(Segment))]
        public async Task<IHttpActionResult> GetNB_SegmentMaster(long id, string tenantCode)
        {
            this.SetAndValidateConnectionString(tenantCode);
            NB_SegmentMaster nB_SegmentMaster = new NB_SegmentMaster();
            using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
            {
                nB_SegmentMaster = await nISEntitiesDataContext.NB_SegmentMaster.FirstOrDefaultAsync(m=> m.Id == id);
            }

            if (nB_SegmentMaster == null)
            {
                return NotFound();
            }

            return Ok(new Segment() { Id = nB_SegmentMaster.Id, Code = nB_SegmentMaster.Code, Name = nB_SegmentMaster.Name, SegmentTypeId = nB_SegmentMaster.SegmentTypeId });
        }
    }
}