import {TestBed} from '@angular/core/testing';

import {FormsService} from './forms.service';
import {HttpClientTestingModule} from "@angular/common/http/testing";
import {UserService} from "./user.service";
import {AuthService} from "./auth.service";
import {AlertService} from "./alert.service";
import {Router} from "@angular/router";

describe('FormsService', () =>
{

  let service: FormsService;
  let alertMock = jasmine.createSpyObj('alertMock', ['Alert']);
  let authMock = jasmine.createSpyObj('AuthService', [''], ['IsAuthenticated', 'UserAccount'])

  beforeEach(() =>
  {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        FormsService,
        {provide: AuthService, useValue: authMock},
        {provide: AlertService, useValue: alertMock},
        {provide: 'BASE_URL', useValue: ''}
      ]
    });
    service = TestBed.inject(FormsService);
  });

  it('should be created', () =>
  {
    expect(service).toBeTruthy();
  });
});
