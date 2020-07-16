"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.ConfigConstants = void 0;
//import { environment } from '../../../environments/environment.prod'
var environment_1 = require("../../../environments/environment");
exports.ConfigConstants = {
    BaseURL: environment_1.environment.baseURL,
    ResourceUrl: "https://localhost:44347/LocaleResource/Get",
    TenantCode: "00000000-0000-0000-0000-000000000000",
    ClientCustomerName: "Texfab India",
    UserListUISection: 'UserList',
    UserViewUISection: 'UserView',
    UserAddEditUISection: 'UserAddEdit',
    HeaderUISection: 'Header',
    RoleListUISection: 'RoleList',
    RoleViewUISection: 'RoleView',
    RoleAddEditUISection: 'RoleAddEdit',
    SideBarUISection: 'SideBar',
    LoginUISection: 'Login',
};
//# sourceMappingURL=configConstants.js.map