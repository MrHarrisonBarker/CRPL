import {TestBed} from '@angular/core/testing';

import {ResonanceService} from './resonance.service';
import {HttpClientTestingModule} from "@angular/common/http/testing";
import {WarehouseService} from "./warehouse.service";
import {Router} from "@angular/router";

describe('ResonanceService', () => {

  let service: ResonanceService;
  let routerMock = jasmine.createSpyObj('router', ['navigate']);

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        ResonanceService,
        WarehouseService,
        {provide: 'BASE_URL', useValue: ''},
        {provide: Router, useValue: routerMock},
      ]
    });
    service = TestBed.inject(ResonanceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
