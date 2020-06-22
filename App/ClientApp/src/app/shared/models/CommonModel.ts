
export class PagingParameter {
  PageIndex: number = 0;
  PageSize: number = 0;
}

export class SortParameter {
  SortOrder: SortOrder.Ascending;
  SortColumn: string;
}

export enum SortOrder {
  Ascending = 1,
  Descending = 2
}

export enum SearchMode {
  Contains = 1,
  Equals = 2
}
