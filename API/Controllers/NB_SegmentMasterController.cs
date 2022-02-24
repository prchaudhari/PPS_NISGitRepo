using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using NedbankRepository;

namespace nIS.Controllers
{
    public class NB_SegmentMasterController : ApiController
    {
        private NedbankDbContext db = new NedbankDbContext();

        // GET: api/NB_SegmentMaster
        public IQueryable<Segment> GetNB_SegmentMaster()
        {
            var result = db.NB_SegmentMaster.ToList();

            return db.NB_SegmentMaster.OrderBy(m => m.Name).Select(n => new Segment()
            {
                Id = n.Id,
                Name = n.Name,
                Code = n.Code,
                SegmentTypeId = n.SegmentTypeId
            });
        }

        // GET: api/NB_SegmentMaster/5
        [ResponseType(typeof(Segment))]
        public async Task<IHttpActionResult> GetNB_SegmentMaster(long id)
        {
            NB_SegmentMaster nB_SegmentMaster = await db.NB_SegmentMaster.FindAsync(id);
            if (nB_SegmentMaster == null)
            {
                return NotFound();
            }

            return Ok(new Segment() { Id = nB_SegmentMaster.Id, Code = nB_SegmentMaster.Code, Name = nB_SegmentMaster.Name, SegmentTypeId = nB_SegmentMaster.SegmentTypeId });
        }

        // PUT: api/NB_SegmentMaster/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutNB_SegmentMaster(long id, Segment segment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != segment.Id)
            {
                return BadRequest();
            }
            NB_SegmentMaster nbB_SegmentMaster = new NB_SegmentMaster()
            {
                Id = segment.Id,
                Code = segment.Code,
                Name = segment.Name,
                SegmentTypeId = segment.SegmentTypeId
            };

            db.Entry(nbB_SegmentMaster).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NB_SegmentMasterExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/NB_SegmentMaster
        [ResponseType(typeof(Segment))]
        public async Task<IHttpActionResult> PostNB_SegmentMaster(Segment segment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.NB_SegmentMaster.Add(new NB_SegmentMaster()
            {
                Id = segment.Id,
                Code = segment.Code,
                Name = segment.Name,
                SegmentTypeId = segment.SegmentTypeId
            });
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = segment.Id }, segment);
        }

        // DELETE: api/NB_SegmentMaster/5
        [ResponseType(typeof(Segment))]
        public async Task<IHttpActionResult> DeleteNB_SegmentMaster(long id)
        {
            NB_SegmentMaster nB_SegmentMaster = await db.NB_SegmentMaster.FindAsync(id);
            if (nB_SegmentMaster == null)
            {
                return NotFound();
            }

            db.NB_SegmentMaster.Remove(nB_SegmentMaster);
            await db.SaveChangesAsync();

            return Ok(new Segment() {
                Id = nB_SegmentMaster.Id,
                Name = nB_SegmentMaster.Name,
                Code = nB_SegmentMaster.Code,
                SegmentTypeId = nB_SegmentMaster.SegmentTypeId
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool NB_SegmentMasterExists(long id)
        {
            return db.NB_SegmentMaster.Count(e => e.Id == id) > 0;
        }
    }
}