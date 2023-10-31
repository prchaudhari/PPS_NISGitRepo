using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NIS.Repository.Entities
{
    [Table("NIS.EtlBatches")]
    public partial class EtlBatches
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string BatchName { get; set; }

        public long EtlScheduleId { get; set; }

        public long ProductBatchId { get; set; }

        public bool IsExecuted { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        [Required]
        public bool IsApproved { get; set; }

        public DateTime? DataExtractionDateTime { get; set; }
        public DateTime? ActualRunDate { get; set; }

        [MaxLength(50)]
        public string Status { get; set; }

        public string TenantCode { get; set; }
    }
}
