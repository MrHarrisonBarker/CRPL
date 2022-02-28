import { TestBed } from '@angular/core/testing';

import { QueryService } from './query.service';
import {HttpClientTestingModule} from "@angular/common/http/testing";
import {AuthService} from "./auth.service";
import {AlertService} from "./alert.service";
import {WarehouseService} from "./warehouse.service";

describe('QueryService', () => {

  let service: QueryService;
  let alertMock = jasmine.createSpyObj('alertMock', ['Alert']);
  let authMock = jasmine.createSpyObj('AuthService', [''], ['IsAuthenticated', 'UserAccount'])

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        QueryService,
        {provide: AuthService, useValue: authMock},
        {provide: AlertService, useValue: alertMock},
        WarehouseService,
        {provide: 'BASE_URL', useValue: ''}
      ]
    });
    service = TestBed.inject(QueryService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
