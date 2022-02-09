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
  public TotalShares!: number;

  constructor ()
  {
  }

  ngOnInit (): void
  {
    this.TotalShares = this.OwnershipStructure.map(x => x.Share).reduce((previousValue, currentValue) => previousValue + currentValue);
  }

  public ShareStyle(stake: OwnershipStake): number
  {
    return (stake.Share / this.TotalShares) * 100;
  }

}
