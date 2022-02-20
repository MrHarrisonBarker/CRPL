import {Component, Input, OnInit} from '@angular/core';
import {OwnershipRestructureViewModel} from "../../_Models/Applications/OwnershipRestructureViewModel";

@Component({
  selector: 'cpy-restructure-view [Application]',
  templateUrl: './cpy-restructure-view.component.html',
  styleUrls: ['./cpy-restructure-view.component.css']
})
export class CpyRestructureViewComponent implements OnInit
{
  @Input() Application!: OwnershipRestructureViewModel

  constructor ()
  {
  }

  ngOnInit (): void
  {
  }
}
