import { RolePrivilege } from 'src/app/shared/models/rolePrivilege';

export interface Role {
    "Identifier":number;
    "Name":string;
    "Description":string;
    "Status":string;
    "Privileges":[];
    "RolePrivileges": [];
    "RolePrivilegesIdentifiers":[]
}

export interface RoleprivilegeMapping {
  "EntityName": string,
  "Operation": string,
  "RelatedOperation": string[] ,
  "OtherDependentEntity": string[],
}
