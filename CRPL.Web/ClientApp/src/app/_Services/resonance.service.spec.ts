import {TestBed} from '@angular/core/testing';

import {ResonanceService} from './resonance.service';
import {HttpClientTestingModule} from "@angular/common/http/testing";
import {WarehouseService} from "./warehouse.service";

describe('ResonanceService', () => {
  let service: ResonanceService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        ResonanceService,
        WarehouseService,
        {provide: 'BASE_URL', useValue: ''}
      ]
    });
    service = TestBed.inject(ResonanceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
