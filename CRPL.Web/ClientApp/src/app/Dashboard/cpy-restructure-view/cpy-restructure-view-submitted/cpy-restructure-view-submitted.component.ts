import {Component, Input, OnChanges, OnDestroy, OnInit, SimpleChanges} from '@angular/core';
import {OwnershipRestructureViewModel} from "../../../_Models/Applications/OwnershipRestructureViewModel";
import {CopyrightService} from "../../../_Services/copyright.service";
import {AlertService} from "../../../_Services/alert.service";
import {FormsService} from "../../../_Services/forms.service";
import {finalize} from "rxjs/operators";
import {Observable, Subscription} from "rxjs";
import {ApplicationViewModel} from "../../../_Models/Applications/ApplicationViewModel";

@Component({
  selector: 'cpy-restructure-view-submitted [ApplicationAsync]',
  templateUrl: './cpy-restructure-view-submitted.component.html',
  styleUrls: ['./cpy-restructure-view-submitted.component.css']
})
export class CpyRestructureViewSubmittedComponent implements OnInit, OnChanges, OnDestroy
{
  @Input() ApplicationAsync!: Observable<ApplicationViewModel>;
  public Application!: OwnershipRestructureViewModel;
  private ApplicationSubscription!: Subscription;

  public Locked: boolean = false;

  constructor (
    private copyrightService: CopyrightService,
    private formsService: FormsService,
    private alertService: AlertService)
  {
  }

  private subscribeToApplication()
  {
    this.ApplicationSubscription = this.ApplicationAsync.subscribe(application => {
      console.log("[cpy-registration-view] got registration application", application);
      this.Application = application as OwnershipRestructureViewModel;
    });
  }

  ngOnInit (): void
  {
    this.subscribeToApplication();
  }

  ngOnChanges (changes: SimpleChanges): void
  {
    if (this.ApplicationSubscription)
    {
      this.ApplicationSubscription.unsubscribe();
      this.subscribeToApplication();
    }
  }

  ngOnDestroy (): void
  {
    if (this.ApplicationSubscription) this.ApplicationSubscription.unsubscribe();
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
