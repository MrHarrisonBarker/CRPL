import {Injectable} from '@angular/core';
import {ApplicationViewModel} from "../_Models/Applications/ApplicationViewModel";
import {RegisteredWorkViewModel} from "../_Models/Works/RegisteredWork";
import {BehaviorSubject, Subject} from "rxjs";
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

  public SetMyApplications (value: ApplicationViewModel[])
  {
    this.__MyApplications.next(value);
  }

  set MyDisputed (value: DisputeViewModel[])
  {
    this.__MyDisputed.next(value);
  }

  public UpdateWork (value: RegisteredWorkViewModel)
  {
    console.log("[warehouse] updating work", value.Id);
    let index = this._MyWorks.findIndex(x => x.Id == value.Id);
    if (index == -1)
    {
      this._MyWorks.push(value);
      this.__MyWorks.next(this._MyWorks);
    } else
    {
      this._MyWorks[index] = value;
      this.__MyWorks.next(this._MyWorks);
    }
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
    console.log("[warehouse] updating application", value.Id);
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

  public __MyApplications: BehaviorSubject<ApplicationViewModel[]> = new BehaviorSubject<ApplicationViewModel[]>(this._MyApplications);
  public __MyWorks: BehaviorSubject<RegisteredWorkViewModel[]> = new BehaviorSubject<RegisteredWorkViewModel[]>(this._MyWorks);
  public __MyDisputed: Subject<DisputeViewModel[]> = new Subject<DisputeViewModel[]>();

  // public MyApplications: Observable<ApplicationViewModel[]> = this.__MyApplications.asObservable().pipe(startWith(this._MyApplications));

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
