import {Component, OnInit} from '@angular/core';
import {AuthService} from "../../_Services/auth.service";

@Component({
  selector: 'login-button',
  templateUrl: './login-button.component.html',
  styleUrls: ['./login-button.component.css']
})
export class LoginButtonComponent implements OnInit
{

  constructor (private authService: AuthService)
  {
  }

  ngOnInit (): void
  {
  }

  public Login ()
  {
    // TODO: Button should just be disabled
    if (!(window as any).ethereum) {
      window.alert('Please install MetaMask first.');
      return;
    }

    this.authService.LoginWithMetaMask();
  }
}
