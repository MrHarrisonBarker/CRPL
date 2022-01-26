import {Component, OnInit} from '@angular/core';
import {AuthService} from "../../_Services/auth.service";

@Component({
  selector: 'logout-button',
  templateUrl: './logout-button.component.html',
  styleUrls: ['./logout-button.component.css']
})
export class LogoutButtonComponent implements OnInit
{

  constructor (public authService: AuthService)
  {
  }

  ngOnInit (): void
  {
  }

  public Logout (): void
  {
    this.authService.Logout();
  }

}
