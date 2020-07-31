

export class ScheduleLogDetail {
  "Identifier": number;
  "ScheduleId": number;
  "ScheduleLogId": number;
  "CustomerId": number;
  "CustomerName": string;
  "LogMessage": string;
  "NumberOfRetry": number;
  "CreateDate": Date;
  "Status": string;
  "IsChecked": boolean = false;
  "RenderEngineName": string;
  "RenderEngineURL": string;
}
