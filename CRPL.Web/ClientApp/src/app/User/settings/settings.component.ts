import {Component, OnInit} from '@angular/core';
import {AuthService} from "../../_Services/auth.service";
import {UserAccountViewModel} from "../../_Models/Account/UserAccountViewModel";
import {Router} from "@angular/router";

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css']
})
export class SettingsComponent implements OnInit
{
  public UserAccount!: UserAccountViewModel;

  constructor (private authService: AuthService, private router: Router)
  {
  }

  ngOnInit (): void
  {
    this.UserAccount = this.authService.UserAccount.getValue();
  }

  public EditAccount (): void
  {
    this.router.navigate(['/u/info']);
  }

  public Delete (): void
  {

  }
}
