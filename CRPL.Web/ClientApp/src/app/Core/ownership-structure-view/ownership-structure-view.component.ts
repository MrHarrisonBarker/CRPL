import {Component, Input, OnInit} from '@angular/core';
import {OwnershipStake} from "../../_Models/StructuredOwnership/OwnershipStake";

@Component({
  selector: 'ownership-structure-view [OwnershipStructure]',
  templateUrl: './ownership-structure-view.component.html',
  styleUrls: ['./ownership-structure-view.component.css']
})
export class OwnershipStructureViewComponent implements OnInit
{
  @Input() OwnershipStructure!: OwnershipStake[];

  constructor ()
  {
  }

  ngOnInit (): void
  {
  }

}
