import {Component} from '@angular/core';
import {ResonanceService} from "./_Services/resonance.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'app';
  constructor (private resonanceService: ResonanceService)
  {
    resonanceService.ConnectSockets();
  }
}
