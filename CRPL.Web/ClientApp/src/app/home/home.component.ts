import {Component, OnInit} from '@angular/core';
import {QueryService} from "../_Services/query.service";
import {RegisteredWorkViewModel} from "../_Models/Works/RegisteredWork";
import {DisputeViewModel} from "../_Models/Applications/DisputeViewModel";
import {forkJoin} from "rxjs";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styles: ['.card-list {display: flex; flex-direction: row; flex-wrap: wrap; margin: 0 1rem;}']
})
export class HomeComponent implements OnInit
{
  public Page: number = 0;
  public RecentCopyrights!: RegisteredWorkViewModel[];
  public OpenDisputes!: DisputeViewModel[];

  constructor (private queryService: QueryService)
  {
  }

  async ngOnInit (): Promise<any>
  {
    return forkJoin ([this.queryService.GetRecent(this.Page), this.queryService.GetDisputes(this.Page)]).subscribe(x =>
    {
      this.RecentCopyrights = x[0];
      this.OpenDisputes = x[1];
    });
  }

}
