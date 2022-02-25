import {Component, Input, OnInit} from '@angular/core';
import { DisputeType } from 'src/app/_Models/Applications/DisputeInputModel';
import {DisputeViewModel, ExpectedRecourse} from "../../_Models/Applications/DisputeViewModel";

@Component({
  selector: 'dispute-min [Dispute]',
  templateUrl: './dispute-min.component.html',
  styleUrls: ['./dispute-min.component.css']
})
export class DisputeMinComponent implements OnInit
{
  @Input() Dispute!: DisputeViewModel;

  constructor ()
  {
  }

  ngOnInit (): void
  {
    console.log(this.Dispute);
  }

  get DisputeType ()
  {
    return DisputeType[this.Dispute.DisputeType];
  }

  get ExpectedRecourse()
  {
    return ExpectedRecourse[this.Dispute.ExpectedRecourse];
  }

}
