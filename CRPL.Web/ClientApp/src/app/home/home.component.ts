import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  public openWizard: boolean = false;

  openWizardClick()
  {
    this.openWizard = !this.openWizard;
  }
}
