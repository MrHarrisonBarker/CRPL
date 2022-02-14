import {ErrorHandler, Injectable, NgZone} from "@angular/core";
import {AlertService} from "../_Services/alert.service";
import {HttpErrorResponse} from "@angular/common/http";

@Injectable()
export class GlobalErrorHandler implements ErrorHandler
{
  constructor (
    private alertService: AlertService,
    private zone: NgZone)
  {
  }

  handleError (error: any): void
  {
    if (!(error instanceof HttpErrorResponse)) error = (error as HttpErrorResponse).error

    this.zone.run(() =>
    {
      this.alertService.Alert({Type: 'danger', Message: error?.message || "Something went wrong :("})
    });

    console.error('Error from global error handler', error);
  }
}
