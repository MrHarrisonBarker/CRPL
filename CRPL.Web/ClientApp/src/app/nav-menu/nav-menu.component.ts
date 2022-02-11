import {Component} from '@angular/core';
import {ClarityIcons, homeIcon, searchIcon} from "@cds/core/icon";

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent
{
  isExpanded = false;

  constructor ()
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
}
