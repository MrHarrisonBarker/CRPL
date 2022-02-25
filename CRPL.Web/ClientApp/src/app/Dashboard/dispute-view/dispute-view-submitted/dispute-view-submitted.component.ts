import {Component, Input, OnInit} from '@angular/core';
import {DisputeViewModel} from "../../../_Models/Applications/DisputeViewModel";
import {DisputeType} from "../../../_Models/Applications/DisputeInputModel";
import {WarehouseService} from "../../../_Services/warehouse.service";
import {BehaviorSubject} from "rxjs";

@Component({
  selector: 'dispute-view-submitted [Application]',
  templateUrl: './dispute-view-submitted.component.html',
  styleUrls: ['./dispute-view-submitted.component.css']
})
export class DisputeViewSubmittedComponent implements OnInit
{

  @Input() Application!: DisputeViewModel;
  public Owner: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  constructor (public warehouse: WarehouseService)
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
}
