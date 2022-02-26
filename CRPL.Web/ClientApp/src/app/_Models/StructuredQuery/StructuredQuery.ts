export interface StructuredQuery
{
  Keyword?: string;
  SortBy?: Sortable;
  WorkFilters?: Record<WorkFilter, string>;
}

export enum WorkFilter
{
  RegisteredAfter,
  RegisteredBefore,
  WorkType
}

export enum Sortable
{
  Created,
  Title,
  Registered
}
