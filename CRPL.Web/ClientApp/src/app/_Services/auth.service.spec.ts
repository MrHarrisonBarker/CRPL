import {inject, TestBed} from '@angular/core/testing';

import {AuthService} from './auth.service';
import {HttpClientTestingModule} from "@angular/common/http/testing";
import {Router} from "@angular/router";

describe('AuthService', () =>
{
  let service: AuthService;

  beforeEach(() =>
  {
    let routerMock = jasmine.createSpyObj('router', ['navigate']);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        AuthService,
        {provide: Router, useValue: routerMock},
        {provide: 'BASE_URL', useValue: ''}]
    });
    service = TestBed.inject(AuthService);
  });

  it('should be created', () =>
  {
    expect(service).toBeTruthy();
  });
});
