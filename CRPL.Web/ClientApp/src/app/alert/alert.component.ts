import {Component, OnInit} from '@angular/core';
import {AlertMeta, AlertService} from "../_Services/alert.service";

// TODO: alert timeout. remove after a couple of seconds
@Component({
  selector: 'alert-container',
  templateUrl: './alert.component.html',
  styleUrls: ['./alert.component.css']
})
export class AlertComponent implements OnInit
{

  public loadingSub: boolean = false;
  public alertSub: AlertMeta = null as any;

  constructor (public alertService: AlertService)
  {
    alertService.alert.subscribe(a => this.alertSub = a);
    alertService.loading.subscribe(l => this.loadingSub = l);
  }

  ngOnInit (): void
  {
  }

  alertClosed ()
  {
    console.log("the alert has been closed");
  }
}
