import {Component, Input, OnInit} from "@angular/core";
import {DisputeType} from "../../_Models/Applications/DisputeInputModel";

@Component({
  selector: 'dispute-type [type]',
  template: '<span [ngClass]="map[Type]">{{Type}}</span>',
  styles: ['']
})
export class DisputeTypeComponent implements OnInit
{
  @Input() type!: number;
  map: Record<string, string> = {
    Ownership: "label label-blue",
    Usage: "label label-light-blue",
  }

  constructor ()
  {
  }

  ngOnInit (): void
  {

  }

  get Type (): string
  {
    return DisputeType[this.type];
  }
}
