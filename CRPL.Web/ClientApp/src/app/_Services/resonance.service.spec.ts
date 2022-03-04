import {TestBed} from '@angular/core/testing';

import {ResonanceService} from './resonance.service';

describe('ResonanceService', () => {
  let service: ResonanceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ResonanceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
