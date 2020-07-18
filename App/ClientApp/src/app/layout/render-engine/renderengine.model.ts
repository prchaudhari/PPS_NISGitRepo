import { PagingParameter, SortParameter, SearchMode } from "../../shared/models/commonmodel";

export class RenderEngine {
  Identifier: number;
  RenderEngineName: string;
  URL: string;
  PriorityLevel: number;
  NumberOfThread: number;
  IsActive: boolean;
  IsDeleted: boolean;
  InUse: boolean;
}

export class RenderEngineSearchParameter {
  Identifier: number;
  RenderEngineName: string;
  RenderEngineURL: string;
  SearchMode: number;
  PagingParameter: PagingParameter;
  SortParameter: SortParameter;
}
