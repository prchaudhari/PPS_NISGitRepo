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

