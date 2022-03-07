import {Component} from '@angular/core';
import {ClarityIcons, searchIcon} from "@cds/core/icon";
import {Router} from "@angular/router";
import {AuthService} from "../_Services/auth.service";

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent
{
  isExpanded = false;

  constructor (private router: Router, public authService: AuthService)
  {
    ClarityIcons.addIcons(searchIcon);
  }

  collapse ()
  {
    this.isExpanded = false;
  }

  toggle ()
  {
    this.isExpanded = !this.isExpanded;
  }

  public RouteActive (route: string): boolean
  {
    return this.router.url == route;
  }
}
