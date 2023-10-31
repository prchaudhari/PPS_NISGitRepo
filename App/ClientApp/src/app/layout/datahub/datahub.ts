import { LongDateFormatKey } from "moment";

export class ETLSchedule {
  "Identifier": number;
  "Product": string;
  "Name": string;
  "ScheduleDate": Date;
  "StartDate": Date;
  "EndDate": Date;
  "DayOfMonth": number;
  "IsActive": boolean;
  "RecurrancePattern":string;
  "ProductBatchId": number;
  "IsApproved": boolean;
  "Status": string;
}

export class ETLScheduleBatch {
  "Identifier": number;
  "BatchName": string;
  "ETLScheduleId": number;
  "IsExecuted": boolean;
  "DataExtractionDate": Date;
  "BatchExecutionDate": Date;
  "Status": string;
  "ETLScheduleName": string;
}

export class ETLScheduleBatchLog {
  "Identifier": number;
  "ETLSchedule": string;
  "Batch": string;
  "ProcessingTime": string;
  "Status": string;
  "ExecutionDate": Date;
  "EtlScheduleId": number;
  "LogMessage": string;
  "SequenceNo": number;
}

export class ETLScheduleBatchLogDetail {
  "Identifier": number;
  "Schedule": string;
  "Status": string;
  "ExecutionDate": Date;
  "Segment": string;
  "Language": string;
  "BatchName": string;
  "LogMessage": string;
  "ReferenceRecordId": number;
}
