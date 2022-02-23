import {Component, OnInit} from '@angular/core';
import {QueryService} from "../_Services/query.service";
import {RegisteredWorkViewModel} from "../_Models/Works/RegisteredWork";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styles: ['#recent-copyrights {display: flex; flex-direction: row; flex-wrap: wrap; margin: 0 1rem;}']
})
export class HomeComponent implements OnInit
{
  public RecentCopyrights!: RegisteredWorkViewModel[];

  constructor (private queryService: QueryService)
  {
  }

  async ngOnInit (): Promise<any>
  {
    return this.queryService.GetRecent().subscribe(x => this.RecentCopyrights = x);
  }

}
