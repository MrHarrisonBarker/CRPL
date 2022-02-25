import {Component, Input, OnInit} from '@angular/core';
import {DisputeViewModel} from "../../../_Models/Applications/DisputeViewModel";
import {DisputeType} from "../../../_Models/Applications/DisputeInputModel";
import {WarehouseService} from "../../../_Services/warehouse.service";
import {BehaviorSubject} from "rxjs";
import {FormsService} from "../../../_Services/forms.service";
import Web3 from "web3";
import {AlertService} from "../../../_Services/alert.service";

@Component({
  selector: 'dispute-view-submitted [Application]',
  templateUrl: './dispute-view-submitted.component.html',
  styleUrls: ['./dispute-view-submitted.component.css']
})
export class DisputeViewSubmittedComponent implements OnInit
{
  @Input() Application!: DisputeViewModel;
  public Owner: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  ProcessingTransaction: boolean = false;

  constructor (public warehouse: WarehouseService, private formsService: FormsService, private alertService: AlertService)
  {
  }

  ngOnInit (): void
  {
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

  get DisputeType ()
  {
    return DisputeType[this.Application.DisputeType];
  }

  public Refuse () : void
  {
    this.formsService.ResolveDispute({
      Accept: false,
      DisputeId: this.Application.Id,
      Message: "refused dispute recourse"
    }).subscribe(x =>
    {
      this.Application.Status = x.Status;
      this.Application.ResolveResult.ResolvedStatus = x.ResolveResult.ResolvedStatus;
    });
  }

  public Accept (): void
  {
    this.formsService.ResolveDispute({
      Accept: true,
      DisputeId: this.Application.Id,
      Message: "accepted dispute recourse"
    }).subscribe(x => {
      this.Application.Status = x.Status;
      this.Application.ResolveResult.ResolvedStatus = x.ResolveResult.ResolvedStatus;
    });
  }

  private Ethereum = (window as any).ethereum;

  public StartPayment (): void
  {
    this.ProcessingTransaction = true;
    let web3 = new Web3(Web3.givenProvider);

    this.Ethereum.request({
      method: 'eth_sendTransaction',
      params: [{
        to: this.Application.AssociatedUsers[0].WalletPublicAddress,
        from: this.Ethereum.selectedAddress,
        value: web3.utils.toWei(this.Application.ExpectedRecourseData, "ether")
      }]
    }).then((transaction: string) => {
      this.formsService.RecordPayment(this.Application.Id, transaction).subscribe(x => {
        console.log(x);
        this.alertService.Alert({Type:'success', Message: 'Recorded payment'});
        this.ProcessingTransaction = false;
      })
    });
  }
}
