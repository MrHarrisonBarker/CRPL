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
  public ColourMap: Record<string, string> = {}

  constructor ()
  {
  }

  ngOnInit (): void
  {
    this.TotalShares = this.OwnershipStructure.map(x => x.Share).reduce((previousValue, currentValue) => previousValue + currentValue);
    this.OwnershipStructure.forEach(share => this.ColourMap[share.Owner] = this.randomColor())
  }

  public ShareStyle(stake: OwnershipStake): number
  {
    return (stake.Share / this.TotalShares) * 100;
  }

  private randomColor(): string
  {
    let color = '#';
    let letters = '0123456789ABCDEF'
    for (let i = 0; i < 6; i++) {
      color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
  }

}
