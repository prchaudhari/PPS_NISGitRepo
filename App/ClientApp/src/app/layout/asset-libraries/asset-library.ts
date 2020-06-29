
import { PagingParameter, SortParameter, SearchMode } from "../../shared/models/commonmodel";
import { DateTimeAdapter } from "ng-pick-datetime";
import { User } from "../users/user";

export class AssetLibrary {
  Identifier: number;
  Name: string;
  Description: string = "";
  Assets: Asset[] = null;
}

export class Asset {
  AssetLibraryIdentifier: string;
  Identifier: number;
  Name: string;
  FilePath: string;
  FileContent: string;
  IsChecked: boolean = false;
  LastUpdatedDate;
  LastUpdatedBy: User;
}

export class AssetLibrarySearchParameter {
  Identifier: string;
  Name: string;
  IsAssetDataRequired: boolean;
  PagingParameter: PagingParameter = new PagingParameter();
  SortParameter: SortParameter = new SortParameter();
  SearchMode: SearchMode = SearchMode.Equals;
  SceneIdentifier: string;
}

export class AssetSearchParameter {
  Identifier: string;
  AssetLibraryIdentifier: string;
  Name: string;
  Extension: string;
  SceneIdentifier: string;
  PagingParameter: PagingParameter = new PagingParameter();
  SortParameter: SortParameter = new SortParameter();
  SearchMode: SearchMode = SearchMode.Equals;
  IsDeleted: boolean;
}

export class SSMLGenerator {
  Identifier: number;
  AssetLibraryIdentifier: string;
  Name: string;
  Gender: string;
  Language: string;
  Description: string = "";
  AudioURL: string = "";
  AssetLibraries: AssetLibrary[] = null;
}
