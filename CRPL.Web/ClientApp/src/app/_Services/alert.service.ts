import {Injectable} from '@angular/core';
import {Subject} from "rxjs";

export interface AlertMeta
{
  Message: string;
  Type: string;
}

@Injectable({
  providedIn: 'root'
})
export class AlertService
{

  public alert: Subject<AlertMeta> = new Subject<AlertMeta>();

  constructor ()
  {
  }

  public Alert(meta: AlertMeta): void
  {
    this.alert.next(meta);
  }
}
