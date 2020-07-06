
import { User } from '../users/user'

export class Schedule {
  "Identifier": number;
  "Name": string;
  "Description": string;
  "DayOfMonth": number;
  "HourOfDay": number;
  "MinuteOfDay": number;
  "StartDate": Date;
  "EndDate": Date;
  "Status": string;
  "IsActive": boolean;
  "IsDeleted": boolean;
  "TenantCode": string;
  "LastUpdatedDate": string;
  "UpdateBy": User
}
