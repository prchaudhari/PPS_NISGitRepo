import { PagingParameter, SortParameter, SearchMode } from "../../shared/models/commonmodel";
export class TenantConfiguration {
  Identifier: number;
  Name: string;
  OutputPDFPath: string;
  Description: string;
  OutputHTMLPath: string;
  InputDataSourcePath: string;
  AssetPath: string;
  ArchivalPath: string;
  IsAssetPathEditable: boolean;
  IsOutputHTMLPathEditable: boolean;
  ArchivalPeriodUnit: string;
  ArchivalPeriod: number;
  DateFormat: string;
  ApplicationTheme: string;
  WidgetThemeSetting: string;
}

export class TenantConfigurationSearchParameter {
  Identifier: number;
  TenantConfigurationName: string;
  TenantConfigurationURL: string;
  SearchMode: number;
  PagingParameter: PagingParameter;
  SortParameter: SortParameter;
}
