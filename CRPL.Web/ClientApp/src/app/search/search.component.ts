import {Component, OnInit} from '@angular/core';
import {QueryService} from "../_Services/query.service";
import {Sortable, StructuredQuery, WorkFilter} from "../_Models/StructuredQuery/StructuredQuery";
import {WorkType} from "../_Models/WorkType";

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent implements OnInit
{
  public Page: number = 0;

  public Sortables: string[] = Object.values(Sortable).filter(value => typeof value != 'number') as string[];
  public WorkTypes: string[] = Object.values(WorkType).filter(value => typeof value != 'number') as string[];

  public SearchKeyword!: string;
  public RegisteredAfter: any;
  public RegisteredBefore: any;
  public SortBy: keyof  typeof Sortable = "Registered";
  public TypeOfWork: keyof  typeof WorkType = "" as any;

  constructor (private queryService: QueryService)
  {
  }

  ngOnInit (): void
  {
  }

  public Search ()
  {
    let query: StructuredQuery = {
      Keyword: this.SearchKeyword,
      WorkFilters: {"0": "", "1": "", "2": ""},
      SortBy: Sortable[this.SortBy]
    }

    if (query.WorkFilters)
    {
      if (this.RegisteredAfter) query.WorkFilters[WorkFilter.RegisteredAfter] = new Date(this.RegisteredAfter).toDateString();
      if (this.RegisteredBefore) query.WorkFilters[WorkFilter.RegisteredBefore] = new Date(this.RegisteredBefore).toDateString();
      if (this.TypeOfWork) query.WorkFilters[WorkFilter.WorkType] = this.TypeOfWork;
    }

    console.log(query);

    this.queryService.Search(this.Page, query).subscribe(x => console.log(x));
  }

}
