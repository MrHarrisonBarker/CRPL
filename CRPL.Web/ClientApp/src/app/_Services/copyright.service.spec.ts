import {TestBed} from '@angular/core/testing';

import {CopyrightService} from './copyright.service';
import {HttpClientTestingModule} from "@angular/common/http/testing";
import {AuthService} from "./auth.service";
import {AlertService} from "./alert.service";
import {WarehouseService} from "./warehouse.service";

describe('CopyrightService', () =>
{
  let service: CopyrightService;
  let alertMock = jasmine.createSpyObj('alertMock', ['Alert']);
  let authMock = jasmine.createSpyObj('AuthService', [''], ['IsAuthenticated', 'UserAccount']);
  let warehouseMock = jasmine.createSpyObj('WarehouseService', [''])

  beforeEach(() =>
  {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        CopyrightService,
        {provide: AuthService, useValue: authMock},
        {provide: AlertService, useValue: alertMock},
        {provide: WarehouseService, useValue: warehouseMock},
        {provide: 'BASE_URL', useValue: ''}]
    });
    service = TestBed.inject(CopyrightService);
  });

  it('should be created', () =>
  {
    expect(service).toBeTruthy();
  });
});
