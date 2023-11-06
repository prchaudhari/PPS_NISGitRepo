using System;

namespace nIS.NedBank
{
    public class ScheduleModel
    {
        public long Identifier { get; set; }
        public string Name { get; set; }
        public long StatementId { get; set; }
        public string Description { get; set; }
        public long DayOfMonth { get; set; }
        public long HourOfDay { get; set; }
        public long MinuteOfDay { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string TenantCode { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public long UpdateBy { get; set; }
        public bool IsExportToPDF { get; set; }
        public string StatementName { get; set; }
        public string RecurrancePattern { get; set; }
        public Nullable<long> RepeatEveryDayMonWeekYear { get; set; }
        public string WeekDays { get; set; }
        public Nullable<bool> IsEveryWeekDay { get; set; }
        public string MonthOfYear { get; set; }
        public Nullable<bool> IsEndsAfterNoOfOccurrences { get; set; }
        public Nullable<long> NoOfOccurrences { get; set; }
        public Nullable<int> ExecutedBatchCount { get; set; }
        public string Languages { get; set; }
        public string ProductBatchName { get; set; }
        public string ProductName { get; set; }
        public int ProductId { get; set; }
        public string ScheduleNameByUser { get; set; }
        public bool IsDataReady { get; set; }
        public Nullable<DateTime> EndDateForDisplay { get; set; }
        public Nullable<long> NoOfOccuranceForDisplay { get; set; }
        public Nullable<long> TotalBatches { get; set; }
        public bool IsDeleteButtonVisible { get; set; }
    }
}
