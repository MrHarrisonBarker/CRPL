import {Injectable} from '@angular/core';
import {BehaviorSubject, Subject} from "rxjs";

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
  get loading (): BehaviorSubject<boolean>
  {
    return this._loading;
  }

  get alert (): Subject<AlertMeta>
  {
    return this._alert;
  }

  private _alert: Subject<AlertMeta> = new Subject<AlertMeta>();
  private _loading: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  constructor ()
  {
  }

  public Alert (meta: AlertMeta): void
  {
    this._alert.next(meta);
  }

  public StartLoading (): void
  {
    this._loading.next(true);
  }

  public StopLoading (): void
  {
    this._loading.next(false);
  }
}
