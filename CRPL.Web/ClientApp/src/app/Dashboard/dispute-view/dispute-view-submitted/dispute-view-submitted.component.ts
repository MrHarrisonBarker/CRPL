import {Component, Input, OnInit} from '@angular/core';
import {DisputeViewModel} from "../../../_Models/Applications/DisputeViewModel";
import {DisputeType} from "../../../_Models/Applications/DisputeInputModel";

@Component({
  selector: 'dispute-view-submitted [Application]',
  templateUrl: './dispute-view-submitted.component.html',
  styleUrls: ['./dispute-view-submitted.component.css']
})
export class DisputeViewSubmittedComponent implements OnInit
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

}
