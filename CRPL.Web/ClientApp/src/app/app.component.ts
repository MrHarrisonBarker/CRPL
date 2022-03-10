import {Component} from '@angular/core';
import {ResonanceService} from "./_Services/resonance.service";
import {environment} from "../environments/environment";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'app';
  buildNumber = environment.buildNumber;
  constructor (private resonanceService: ResonanceService)
  {
    console.log(environment);
    resonanceService.ConnectSockets();
  }
}
