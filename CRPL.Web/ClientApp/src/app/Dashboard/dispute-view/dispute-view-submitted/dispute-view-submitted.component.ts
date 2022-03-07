import {Component, Input, OnChanges, OnDestroy, OnInit, SimpleChanges} from '@angular/core';
import {DisputeViewModel, ResolvedStatus} from "../../../_Models/Applications/DisputeViewModel";
import {DisputeType} from "../../../_Models/Applications/DisputeInputModel";
import {WarehouseService} from "../../../_Services/warehouse.service";
import {BehaviorSubject, Observable, Subscription} from "rxjs";
import {FormsService} from "../../../_Services/forms.service";
import Web3 from "web3";
import {AlertService} from "../../../_Services/alert.service";
import {finalize} from "rxjs/operators";
import {ApplicationViewModel} from "../../../_Models/Applications/ApplicationViewModel";

@Component({
  selector: 'dispute-view-submitted [ApplicationAsync]',
  templateUrl: './dispute-view-submitted.component.html',
  styleUrls: ['./dispute-view-submitted.component.css']
})
export class DisputeViewSubmittedComponent implements OnInit, OnChanges, OnDestroy
{
  @Input() ApplicationAsync!: Observable<ApplicationViewModel>;
  public Application!: DisputeViewModel;
  private ApplicationSubscription!: Subscription;

  public Owner: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  public Locked: boolean = false;

  constructor (public warehouse: WarehouseService, private formsService: FormsService, private alertService: AlertService)
  {
  }

  private subscribeToApplication()
  {
    this.ApplicationSubscription = this.ApplicationAsync.subscribe(application => this.Application = application as DisputeViewModel);
  }

  ngOnInit (): void
  {
    this.subscribeToApplication();

    if (!this.warehouse.__MyWorks) this.Owner.next(false);
    if (!this.Application.AssociatedWork) this.Owner.next(false);

    this.Owner.next(this.warehouse._MyWorks.findIndex(w => w.Id == this.Application.AssociatedWork?.Id) != -1);
    // TODO: not working when piping from sub, doesn't subscribe for some reason!
    // this.IsOwner = this.warehouse.__MyWorks.pipe(map(x =>
    // {
    //   console.log("works loaded", x);
    //   return x.findIndex(w => w.Id == this.Application.AssociatedWork?.Id) != -1
    // }));
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

  get DisputeType ()
  {
    return DisputeType[this.Application.DisputeType];
  }

  public Refuse (): void
  {
    this.Locked = true;
    this.formsService.ResolveDispute({
      Accept: false,
      DisputeId: this.Application.Id,
      Message: "refused dispute recourse"
    })
        .pipe(finalize(() => this.Locked = false))
        .subscribe(x =>
        {
          this.Application.Status = x.Status;
          this.Application.ResolveResult.ResolvedStatus = x.ResolveResult.ResolvedStatus;
        });
  }

  public Accept (): void
  {
    console.log('accepting dispute')
    this.formsService.ResolveDispute({
      Accept: true,
      DisputeId: this.Application.Id,
      Message: "accepted dispute recourse"
    })
        .pipe(finalize(() => this.Locked = false))
        .subscribe(x =>
        {
          this.Application.Status = x.Status;
          this.Application.ResolveResult.ResolvedStatus = x.ResolveResult.ResolvedStatus;
        });
  }

  private Ethereum = (window as any).ethereum;

  public StartPayment (): void
  {
    this.Locked = true;
    let web3 = new Web3(Web3.givenProvider);

    (this.Ethereum.request({
      method: 'eth_sendTransaction',
      params: [{
        to: this.Application.AssociatedUsers[0].WalletPublicAddress,
        from: this.Ethereum.selectedAddress,
        value: web3.utils.toWei(this.Application.ExpectedRecourseData, "ether")
      }]
    }) as Promise<any>).then((transaction: string) =>
    {
      this.formsService.RecordPayment(this.Application.Id, transaction)
          .pipe(finalize(() => this.Locked = false))
          .subscribe(x =>
          {
            this.alertService.Alert({Type: 'success', Message: 'Recorded payment'});
            this.Locked = false;
            this.Application.ResolveResult.ResolvedStatus = ResolvedStatus.Processing;
          })
    }).catch((err: any) => {
      console.log('[dispute-view-submitted] got error', err);
    }).finally(() => this.Locked = false);
  }
}
