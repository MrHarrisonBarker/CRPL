import {Component, Input, OnInit} from '@angular/core';
import {RegisteredWorkViewModel} from "../../../_Models/Works/RegisteredWork";
import {CopyrightService} from "../../../_Services/copyright.service";
import {AlertService} from "../../../_Services/alert.service";
import {Observable} from "rxjs";

@Component({
  selector: 'copyright-view-expired [CopyrightAsync]',
  templateUrl: './copyright-view-expired.component.html',
  styleUrls: ['./copyright-view-expired.component.css']
})
export class CopyrightViewExpiredComponent implements OnInit
{
  @Input() CopyrightAsync!: Observable<RegisteredWorkViewModel>;
  public Copyright!: RegisteredWorkViewModel | null;

  constructor (private copyrightService: CopyrightService, private alertService: AlertService)
  {
  }

  ngOnInit (): void
  {
    if (this.CopyrightAsync) this.CopyrightAsync.subscribe(copyright =>
    {
      console.log("[copyright-view-expired] got application", copyright);
      this.Copyright = copyright;
    });
  }

  public Sync (): void
  {
    if (this.Copyright)
    {
      this.copyrightService.Sync(this.Copyright.Id).subscribe(x => this.alertService.Alert({
        Type: "info",
        Message: "The work has been synced with the blockchain"
      }));
    }
  }
}
