import {Component, Input, OnInit} from '@angular/core';
import {RegisteredWorkViewModel} from "../../_Models/Works/RegisteredWork";

@Component({
  selector: 'copyright-view [Copyright]',
  templateUrl: './copyright-view.component.html',
  styleUrls: ['./copyright-view.component.css']
})
export class CopyrightViewComponent implements OnInit
{
  @Input() Copyright!: RegisteredWorkViewModel;
  public RestructureOpen: boolean = false;

  constructor ()
  {
  }

  ngOnInit (): void
  {
  }

}
