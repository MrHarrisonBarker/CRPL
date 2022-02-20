import {Component, Input, OnInit} from '@angular/core';
import {RegisteredWorkViewModel} from "../../../_Models/Works/RegisteredWork";
import {CopyrightService} from "../../../_Services/copyright.service";
import {AlertService} from "../../../_Services/alert.service";

@Component({
  selector: 'copyright-view-expired',
  templateUrl: './copyright-view-expired.component.html',
  styleUrls: ['./copyright-view-expired.component.css']
})
export class CopyrightViewExpiredComponent implements OnInit
{

  @Input() Copyright!: RegisteredWorkViewModel;

  constructor (private copyrightService: CopyrightService, private alertService: AlertService)
  {
  }

  ngOnInit (): void
  {
  }

  public Sync (): void
  {
    this.copyrightService.Sync(this.Copyright.Id).subscribe(x => this.alertService.Alert({
      Type: "info",
      Message: "The work has been synced with the blockchain"
    }));
  }
}
