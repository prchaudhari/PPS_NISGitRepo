using System;

namespace nIS.PPS
{
    public class ETLScheduleModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Product { get; set; }

        public int ProductId { get; set; }

        public long ProductBatchId { get; set; }

        public DateTime? ScheduleDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public long DayOfMonth { get; set; }

        public bool IsLastDate { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        public string RecurrancePattern { get; set; }

        public string ScheduleNameByUser { get; set; }

        public long HourOfDay { get; set; }

        public long MinuteOfDay { get; set; }

        public Nullable<long> RepeatEveryDayMonWeekYear { get; set; }

        public Nullable<DateTime> EndDateForDisplay { get; set; }

        public Nullable<long> NoOfOccuranceForDisplay { get; set; }

        public string WeekDays { get; set; }

        public string ScheduleStatements { get; set; }

        public string Status { get; set; }

        public string TenantCode { get; set; }

        public int TotalBacthes { get; set; }

        public int ExecutedBatches { get; set; }

    }
}
