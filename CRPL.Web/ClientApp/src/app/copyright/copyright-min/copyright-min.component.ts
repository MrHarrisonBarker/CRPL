import {Component, Input, OnInit} from '@angular/core';
import {RegisteredWorkViewModel} from "../../_Models/Works/RegisteredWork";

@Component({
  selector: 'copyright-min [Copyright]',
  templateUrl: './copyright-min.component.html',
  styleUrls: ['./copyright-min.component.css']
})
export class CopyrightMinComponent implements OnInit
{

  @Input() Copyright!: RegisteredWorkViewModel;

  constructor ()
  {
  }

  ngOnInit (): void
  {
  }

}
