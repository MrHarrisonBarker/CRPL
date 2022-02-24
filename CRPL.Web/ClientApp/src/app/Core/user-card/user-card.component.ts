import {Component, Input, OnInit} from '@angular/core';
import {UserAccountMinimalViewModel} from "../../_Models/Account/UserAccountMinimalViewModel";

@Component({
  selector: 'user-card [User]',
  templateUrl: './user-card.component.html',
  styleUrls: ['./user-card.component.css']
})
export class UserCardComponent implements OnInit
{
  @Input() User!: UserAccountMinimalViewModel;

  constructor ()
  {
  }

  ngOnInit (): void
  {
  }

}
