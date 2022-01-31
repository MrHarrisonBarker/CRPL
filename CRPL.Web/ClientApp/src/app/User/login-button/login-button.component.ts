import {Component, OnInit} from '@angular/core';
import {AuthService} from "../../_Services/auth.service";
import {AlertService} from "../../_Services/alert.service";

@Component({
  selector: 'login-button',
  templateUrl: './login-button.component.html',
  styleUrls: ['./login-button.component.css']
})
export class LoginButtonComponent implements OnInit
{
  public HasMetaMask: boolean = false;

  constructor (
    public authService: AuthService,
    private alertService: AlertService)
  {
    this.HasMetaMask = (window as any).ethereum;
  }

  ngOnInit (): void
  {
  }

  public Login ()
  {
    if (!(window as any).ethereum)
    {
      window.alert('Please install MetaMask first.');
      return;
    }

    this.authService.LoginWithMetaMask().subscribe(res =>
    {
      if (res.Account) this.alertService.Alert({Message: "Successfully logged in!", Type: "success"});
      if (!res.Account) this.alertService.Alert({Message: res.Log, Type: "danger"});
    }, error => this.alertService.Alert({Message: error.error, Type: "danger"}), () => this.alertService.StopLoading());
  }
}
