import {TestBed} from '@angular/core/testing';

import {UserService} from './user.service';
import {HttpClientTestingModule} from "@angular/common/http/testing";
import {AuthService} from "./auth.service";
import {Router} from "@angular/router";
import {AlertService} from "./alert.service";

describe('UserService', () =>
{
  let service: UserService;

  beforeEach(() =>
  {
    let routerMock = jasmine.createSpyObj('router', ['navigate']);
    let alertMock = jasmine.createSpyObj('alertMock', ['Alert'])

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        AuthService,
        {provide: AlertService, useValue: alertMock},
        {provide: Router, useValue: routerMock},
        {provide: 'BASE_URL', useValue: ''}]
    });
    service = TestBed.inject(UserService);
  });

  it('should be created', () =>
  {
    expect(service).toBeTruthy();
  });
});
