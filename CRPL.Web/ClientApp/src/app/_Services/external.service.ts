import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {environment} from "../../environments/environment";
import {BehaviorSubject} from "rxjs";

interface ethPriceResult
{
  ethbtc: number,
  ethbtc_timestamp: number,
  ethusd: number,
  ethusd_timestamp: number
}

export interface EtherScanResult
{
  status: number,
  message: string,
  result: any | ethPriceResult
}

@Injectable({
  providedIn: 'root'
})
export class ExternalService
{
  public CurrentPrice: BehaviorSubject<number> = new BehaviorSubject<number>(2455.12);

  constructor (private http: HttpClient)
  {
    this.updatePrice();
  }

  private updatePrice (): void
  {
    if (environment.production)
    {
      this.http.get<EtherScanResult>('https://api.etherscan.io/api?module=stats&action=ethprice&apikey=' + environment.etherScanKey).subscribe(x =>
      {
        console.log(x);
        this.CurrentPrice.next(x.result.ethusd)
      });
    }
  }
}
