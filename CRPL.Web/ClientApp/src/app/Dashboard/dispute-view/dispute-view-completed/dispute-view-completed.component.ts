import {Component, Input, OnInit} from '@angular/core';
import { DisputeType } from 'src/app/_Models/Applications/DisputeInputModel';
import {DisputeViewModel, ExpectedRecourse} from "../../../_Models/Applications/DisputeViewModel";

@Component({
  selector: 'dispute-view-completed [Application]',
  templateUrl: './dispute-view-completed.component.html',
  styleUrls: ['./dispute-view-completed.component.css']
})
export class DisputeViewCompletedComponent implements OnInit
{
  @Input() Application!: DisputeViewModel;

  constructor ()
  {
  }

  ngOnInit (): void
  {
  }

  get DisputeType ()
  {
    return DisputeType[this.Application.DisputeType];
  }

  get ExpectedRecourse()
  {
    return ExpectedRecourse[this.Application.ExpectedRecourse];
  }
}
