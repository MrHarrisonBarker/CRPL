import {Component, OnInit} from '@angular/core';
import {QueryService} from "../_Services/query.service";
import {RegisteredWorkViewModel} from "../_Models/Works/RegisteredWork";
import {DisputeViewModel} from "../_Models/Applications/DisputeViewModel";
import {Observable} from "rxjs";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html'
})
export class HomeComponent implements OnInit
{
  public Page: number = 0;
  public RecentCopyrights!: Observable<RegisteredWorkViewModel[]>;
  public OpenDisputes!: Observable<DisputeViewModel[]>;

  constructor (private queryService: QueryService)
  {
  }

  async ngOnInit (): Promise<any>
  {
    this.RecentCopyrights = this.queryService.GetRecent(this.Page)
    this.OpenDisputes = this.queryService.GetDisputes(this.Page);
  }

}
