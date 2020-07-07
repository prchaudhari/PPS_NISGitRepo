import { User } from '../users/user'
export class Statement {
  "Identifier": number;
  "Name": string;
  "Description": string;
  "PublishedBy": number;
  "Owner": number;
  "Version": string;
  "Status": string="New";
  "CreatedDate": Date;
  "PublishedOn": Date;
  "PublishedOnTick": number;
  "IsActive": boolean;
  "IsDeleted": boolean;
  "TenantCode": string;
  "LastUpdatedDate": null;
  "UpdateBy": number;
  "StatementOwnerName": string;
  "StatementPublishedByUserName": string;
  "StatementUpdatedByUserName": string;
  "StatementPages":  [];
}
export class StatementPage {
  "Identifier": number;
  "ReferencePageId": number;
  "StatementId": number;
  "SequenceNumber": number;
  "TenantCode": string;
  "PageName": string;
}
