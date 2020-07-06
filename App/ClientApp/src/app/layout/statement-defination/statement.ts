import { User } from '../users/user'
export class Statement {
  "Identifier": number;
  "Name": string;
  "Description": string;
  "PublishedBy": User;
  "Owner": User;
  "Version": string;
  "Status": string;
  "CreatedDate": Date;
  "PublishedOn": Date;
  "IsActive": boolean;
  "IsDeleted": boolean;
  "TenantCode": string;
  "LastUpdatedDate": null;
  "UpdateBy": User
}
