import { environment } from '../../../environments/environment.prod'
//import { environment } from '../../../environments/environment'

export const ConfigConstants = {
  BaseURL: environment.baseURL,
  ResourceUrl: "https://localhost:44347/LocaleResource/Get",
  TenantCode: "00000000-0000-0000-0000-000000000000",

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
