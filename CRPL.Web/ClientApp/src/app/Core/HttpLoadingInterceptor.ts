import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from "@angular/common/http";
import {AlertService} from "../_Services/alert.service";
import {Observable} from "rxjs";
import {Injectable} from "@angular/core";
import {finalize} from "rxjs/operators";

@Injectable()
export class HttpLoadingInterceptor implements HttpInterceptor
{
  constructor (private alertService: AlertService)
  {
  }

  intercept (
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>>
  {
    this.alertService.StartLoading();
    return next.handle(request).pipe(finalize(() => this.alertService.StopLoading())) as Observable<HttpEvent<any>>;
  }
}
