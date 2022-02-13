import {Component, Input, OnInit} from '@angular/core';
import {OwnershipRestructureViewModel} from "../../../_Models/Applications/OwnershipRestructureViewModel";
import {CopyrightService} from "../../../_Services/copyright.service";
import {AlertService} from "../../../_Services/alert.service";
import {FormsService} from "../../../_Services/forms.service";

@Component({
  selector: 'cpy-restructure-view-submitted [Application]',
  templateUrl: './cpy-restructure-view-submitted.component.html',
  styleUrls: ['./cpy-restructure-view-submitted.component.css']
})
export class CpyRestructureViewSubmittedComponent implements OnInit
{

  @Input() Application!: OwnershipRestructureViewModel;

  constructor (
    private copyrightService: CopyrightService,
    private formsService: FormsService,
    private alertService: AlertService)
  {
  }

  ngOnInit (): void
  {
  }

  public Bind (): void
  {
    this.send(true);
  }

  public Reject (): void
  {
    this.send(false);
  }

  private send(accepted: boolean) : void
  {
    this.alertService.StartLoading();
    this.copyrightService.BindProposal({
      ApplicationId: this.Application.Id,
      Accepted: accepted
    }).subscribe(x =>
    {
      this.alertService.Alert({Type: 'success', Message: 'Sent transaction'})
      this.alertService.StopLoading();
    }, error =>
    {
      this.alertService.Alert({Type: 'danger', Message: error.error});
      this.alertService.StopLoading();
    });
  }
}
