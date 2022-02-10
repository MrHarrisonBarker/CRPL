import {Injectable} from '@angular/core';
import {ApplicationViewModel} from "../_Models/Applications/ApplicationViewModel";
import {RegisteredWorkViewModel} from "../_Models/Works/RegisteredWork";

@Injectable({
  providedIn: 'root'
})
export class WarehouseService
{
  get MyWorks (): RegisteredWorkViewModel[]
  {
    return this._MyWorks;
  }

  set MyWorks (value: RegisteredWorkViewModel[])
  {
    this._MyWorks = value;
  }

  get MyApplications (): ApplicationViewModel[]
  {
    return this._MyApplications;
  }

  set MyApplications (value: ApplicationViewModel[])
  {
    this._MyApplications = value;
  }

  private _MyApplications!: ApplicationViewModel[];
  private _MyWorks!: RegisteredWorkViewModel[];

  constructor ()
  {
  }
}
