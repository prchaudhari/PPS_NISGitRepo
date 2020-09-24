export class Tenant {
  "Identifier": number;
  "TenantCode": string;
  "TenantName": string;
  "TenantDomainName": string;
  "TenantType": string;
  "PrimaryPinCode": string;
  "PrimaryAddressLine1": string;
  "PrimaryAddressLine2": string;
  "StorageAccount": string;
  "AccessToken": string;
  "StartDate": Date;
  "EndDate": Date;
  "ManageType": string;
  "PanNumber": string;
  "ServiceTax": string;
  "TenantCity": string;
  "TenantCountry": string;
  "Country": Country;
  "TenantState": string;
  "IsActive": string;
  "TenantDescription": string;
  "TenantLogo": string;
  "TenantContacts": TenantContact[];
  "User": any;
}
export class Country {
  "Identifier": number;
  "CountryName": string;
}
export class City {
  "Identifier": number;
  "CountryName": string;
}
export class State {
  "Identifier": number;
  "CountryName": string;
}
export class TenantContact {
  "Identifier": number;
  "FirstName": string;
  "LastName": string;
  "EmailAddress": string;
  "ContactNumber": string;
  "ProfileImage": string;
  "IsActive": boolean;
  "CountryId": string;
  "CountryCode": string;
  "ContactType": string;
  "ContactTypeId": string;
  "TenantCode": string;
  "IsActivationLinkSent": boolean = false;
}
