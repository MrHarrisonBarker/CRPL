import {Component, Input, OnChanges, OnDestroy, OnInit, SimpleChanges} from '@angular/core';
import {ApplicationViewModel} from "../../_Models/Applications/ApplicationViewModel";
import {RegisteredWorkViewModel} from "../../_Models/Works/RegisteredWork";
import {WarehouseService} from "../../_Services/warehouse.service";
import {AlertService} from "../../_Services/alert.service";
import {FormsService} from "../../_Services/forms.service";
import {ClarityIcons, trashIcon} from "@cds/core/icon";
import {finalize} from "rxjs/operators";
import {Observable, Subscription} from "rxjs";

@Component({
  selector: 'applications-view [ApplicationAsync] [ShowForms]',
  templateUrl: './applications-view.component.html',
  styleUrls: ['./applications-view.component.css']
})
export class ApplicationsViewComponent implements OnInit, OnDestroy, OnChanges
{
  @Input() ApplicationAsync!: Observable<ApplicationViewModel>;
  public Application!: ApplicationViewModel | null;
  private ApplicationSubscription!: Subscription;

  @Input() ShowForms: boolean = false;
  @Input() Cancelable: boolean = true;
  public Locked: boolean = false;

  constructor (private warehouse: WarehouseService, private alertService: AlertService, private formsService: FormsService)
  {
    console.log("[applications-view] CONSTRUCT");
    // this.ApplicationAsync = this.warehouse.__MyApplications.pipe(map(x => x.find(a => a.Id == this.ApplicationId) as ApplicationViewModel));
    ClarityIcons.addIcons(trashIcon);
  }

  private subscribeToApplication()
  {
    this.ApplicationSubscription = this.ApplicationAsync.subscribe(x => this.Application = x);
  }

  ngOnInit (): void
  {
    this.subscribeToApplication();
  }

  get ExistingWork (): RegisteredWorkViewModel
  {
    return <RegisteredWorkViewModel>this.warehouse.MyWorks.find(x => x.Id == this.Application?.AssociatedWork?.Id);
  }

  public Cancel (): void
  {
    this.Locked = true;
    if (this.Application)
    {
      this.formsService.Cancel(this.Application?.Id)
          .pipe(finalize(() => this.Locked = false))
          .subscribe(x =>
          {
            this.alertService.Alert({Type: 'success', Message: 'Canceled application'});
            this.Application = null as any;
            this.alertService.TriggerChange.emit();
          }, error => this.alertService.Alert({
            Type: 'danger',
            Message: 'There was a problem when canceling this application'
          }));
    }
  }

  public ngOnDestroy (): void
  {
    this.ApplicationSubscription.unsubscribe();
  }

  public ngOnChanges (changes: SimpleChanges): void
  {
    if (this.ApplicationSubscription)
    {
      console.log("has sub");
      this.ApplicationSubscription.unsubscribe();

    }
  }
}
