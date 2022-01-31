import {Component, OnInit} from '@angular/core';
import {AuthService} from "../../_Services/auth.service";

@Component({
  selector: 'login-button',
  templateUrl: './login-button.component.html',
  styleUrls: ['./login-button.component.css']
})
export class LoginButtonComponent implements OnInit
{
  public HasMetaMask: boolean = false;

  constructor (private authService: AuthService)
  {
    this.HasMetaMask = (window as any).ethereum;
  }

  ngOnInit (): void
  {
  }

  public Login ()
  {
    if (!(window as any).ethereum) {
      window.alert('Please install MetaMask first.');
      return;
    }

    this.authService.LoginWithMetaMask();
  }
}
