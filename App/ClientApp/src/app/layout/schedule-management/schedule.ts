
import { User } from '../users/user'
import { Statement } from '../statement-defination/statement';

export class Schedule {
  "Identifier": number;
  "Name": string="";
  "Description": string="";
  "DayOfMonth": number;
  "HourOfDay": number;
  "MinuteOfDay": number;
  "StartDate": Date;
  "EndDate": Date;
  "Status": string="New";
  "IsActive": boolean;
  "IsDeleted": boolean;
  "TenantCode": string;
  "LastUpdatedDate": string;
  "UpdateBy": User;
  "Statement": Statement;
  "IsExportToPDF":boolean;
  "RecurrancePattern":string;
  "RepeatEveryDayMonWeekYear":number;
  "WeekDays":string;
  "IsEveryWeekDay":boolean;
  "MonthOfYear":string;
  "IsEndsAfterNoOfOccurrences":boolean;
  "NoOfOccurrences":number;
  "ExecutedBatchCount": number;
  "ProductId": number;
}
