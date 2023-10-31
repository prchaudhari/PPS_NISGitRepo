using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NIS.Repository.Entities
{
    [Table("NIS.EtlSchedules")]
    public partial class EtlSchedules
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public long ProductBatchId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public long DayOfMonth { get; set; }

        public bool IsLastDate { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [MaxLength(50)]
        public string Status { get; set; }

        [Required]
        public int UpdateBy { get; set; }

        public DateTime? LastUpdatedDate { get; set; }

        public string TenantCode { get; set; }

        public long ScheduleId { get; set; }

        public int HourOfDay { get; set; }

        public int MinuteOfDay { get; set; }
    }
}
