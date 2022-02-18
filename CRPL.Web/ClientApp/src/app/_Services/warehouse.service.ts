import {Injectable} from '@angular/core';
import {ApplicationViewModel} from "../_Models/Applications/ApplicationViewModel";
import {RegisteredWorkViewModel} from "../_Models/Works/RegisteredWork";
import {Subject} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class WarehouseService
{
  set MyWorks (value: RegisteredWorkViewModel[])
  {
    this.__MyWorks.next(value);
  }

  set MyApplications (value: ApplicationViewModel[])
  {
    this.__MyApplications.next(value);
  }

  public UpdateApplication (value: ApplicationViewModel)
  {
    let index = this._MyApplications.findIndex(x => x.Id == value.Id);
    if (index == -1)
    {
      this._MyApplications.push(value);
      this.__MyApplications.next(this._MyApplications);
    } else
    {
      this._MyApplications[index] = value;
      this.__MyApplications.next(this._MyApplications);
    }
  }

  private _MyApplications: ApplicationViewModel[] = [];
  private _MyWorks: RegisteredWorkViewModel[] = [];

  public __MyApplications: Subject<ApplicationViewModel[]> = new Subject<ApplicationViewModel[]>();
  public __MyWorks: Subject<RegisteredWorkViewModel[]> = new Subject<RegisteredWorkViewModel[]>();

  constructor ()
  {
    this.__MyApplications.subscribe(x =>
    {
      this._MyApplications = x;
      console.log("Current applications", x);
    });
    this.__MyWorks.subscribe(x => this._MyWorks = x);
  }
}
