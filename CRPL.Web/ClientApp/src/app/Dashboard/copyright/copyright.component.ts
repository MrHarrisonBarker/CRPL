import {Component, Input, OnInit} from '@angular/core';
import {RegisteredWorkViewModel} from "../../_Models/Works/RegisteredWork";

@Component({
  selector: 'copyright-view [Copyright]',
  templateUrl: './copyright.component.html',
  styleUrls: ['./copyright.component.css']
})
export class CopyrightComponent implements OnInit
{
  @Input() Copyright!: RegisteredWorkViewModel;

  constructor ()
  {
  }

  ngOnInit (): void
  {
  }

}
