import {Injectable} from '@angular/core';
import {ApplicationViewModel} from "../_Models/Applications/ApplicationViewModel";
import {RegisteredWorkViewModel} from "../_Models/Works/RegisteredWork";
import {Subject} from "rxjs";
import {DisputeViewModel} from "../_Models/Applications/DisputeViewModel";

@Injectable({
  providedIn: 'root'
})
export class WarehouseService
{
  public FireSale()
  {
    this.__MyDisputed.next([]);
    this.__MyApplications.next([]);
    this.__MyWorks.next([]);
  }

  set MyWorks (value: RegisteredWorkViewModel[])
  {
    this.__MyWorks.next(value);
  }

  set MyApplications (value: ApplicationViewModel[])
  {
    this.__MyApplications.next(value);
  }

  set MyDisputed (value: DisputeViewModel[])
  {
    this.__MyDisputed.next(value);
  }

  public UpdateWork (value: RegisteredWorkViewModel)
  {

  }

  public UpdateDispute (value: DisputeViewModel)
  {
    let index = this._MyDisputed.findIndex(x => x.Id == value.Id);
    if (index == -1)
    {
      this._MyDisputed.push(value);
      this.__MyDisputed.next(this._MyDisputed);
    } else
    {
      this._MyDisputed[index] = value;
      this.__MyDisputed.next(this._MyDisputed);
    }
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

  public RemoveApplication (id: string)
  {
    this.__MyApplications.next(this._MyApplications.filter(x => x.Id != id))
  }

  public _MyApplications: ApplicationViewModel[] = [];
  public _MyWorks: RegisteredWorkViewModel[] = [];
  public _MyDisputed: DisputeViewModel[] = [];

  public __MyApplications: Subject<ApplicationViewModel[]> = new Subject<ApplicationViewModel[]>();
  public __MyWorks: Subject<RegisteredWorkViewModel[]> = new Subject<RegisteredWorkViewModel[]>();
  public __MyDisputed: Subject<DisputeViewModel[]> = new Subject<DisputeViewModel[]>();

  constructor ()
  {
    this.__MyApplications.subscribe(x =>
    {
      this._MyApplications = x;
      console.log("Current applications", x);
    });
    this.__MyWorks.subscribe(x => this._MyWorks = x);
    this.__MyDisputed.subscribe(x => this._MyDisputed = x);
  }
}
