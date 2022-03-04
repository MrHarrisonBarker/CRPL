import {Component, Input, OnInit} from '@angular/core';
import {OwnershipRestructureViewModel} from "../../../_Models/Applications/OwnershipRestructureViewModel";
import {CopyrightService} from "../../../_Services/copyright.service";
import {AlertService} from "../../../_Services/alert.service";
import {FormsService} from "../../../_Services/forms.service";
import {finalize} from "rxjs/operators";

@Component({
  selector: 'cpy-restructure-view-submitted [Application]',
  templateUrl: './cpy-restructure-view-submitted.component.html',
  styleUrls: ['./cpy-restructure-view-submitted.component.css']
})
export class CpyRestructureViewSubmittedComponent implements OnInit
{
  @Input() Application!: OwnershipRestructureViewModel;
  public Locked: boolean = false;

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

  private send (accepted: boolean): void
  {
    this.Locked = true;
    this.copyrightService.BindProposal({ApplicationId: this.Application.Id, Accepted: accepted})
        .pipe(finalize(() => this.Locked = false))
        .subscribe(x => this.alertService.Alert({Type: 'success', Message: 'Sent transaction'}),
          error => this.alertService.Alert({Type: 'danger', Message: error.error})
        );
  }
}
