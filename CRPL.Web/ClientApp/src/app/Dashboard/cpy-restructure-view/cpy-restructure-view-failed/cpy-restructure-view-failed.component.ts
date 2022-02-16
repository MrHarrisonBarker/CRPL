import {Component, Input, OnInit} from '@angular/core';
import {OwnershipRestructureViewModel} from "../../../_Models/Applications/OwnershipRestructureViewModel";

@Component({
  selector: 'cpy-restructure-view-failed [Application]',
  templateUrl: './cpy-restructure-view-failed.component.html',
  styleUrls: ['./cpy-restructure-view-failed.component.css']
})
export class CpyRestructureViewFailedComponent implements OnInit
{

  @Input() Application!: OwnershipRestructureViewModel

  constructor ()
  {
  }

  ngOnInit (): void
  {
  }

}
