export interface StructuredQuery
{
  Keyword?: string;
  SortBy?: Sortable;
  WorkFilters?: Map<WorkFilter, string>;
}

export enum WorkFilter
{
  RegisteredAfter,
  RegisteredBefore,
}

export enum Sortable
{
  Created,
  Title,
}
