import {TestBed} from '@angular/core/testing';

import {ValidatorsService} from './validators.service';
import {UserService} from "./user.service";
import {isObservable, of} from "rxjs";
import {AbstractControl, FormControl} from "@angular/forms";

describe('ValidatorsService', () =>
{
  let service: ValidatorsService;
  let mockUserService = jasmine.createSpyObj('UserService', ['IsPhoneUnique', 'IsEmailUnique']);

  beforeEach(() =>
  {

    TestBed.configureTestingModule({
      providers: [
        {provide: UserService, useValue: mockUserService}
      ]
    });
    service = TestBed.inject(ValidatorsService);
  });

  it('should be created', () =>
  {
    expect(service).toBeTruthy();
  });

  it('should add error when validating phone', () =>
  {
    mockUserService.IsPhoneUnique.and.returnValue(of(false));

    let mockControl: AbstractControl = new FormControl('TEST INPUT');
    let sub = service.phoneValidate()(mockControl);

    if (isObservable(sub))  {
      sub.subscribe(x => expect(x).toEqual({phoneExists: true}))
    }
  });

  it('should add null when validating phone', () =>
  {
    mockUserService.IsPhoneUnique.and.returnValue(of(true));

    let mockControl: AbstractControl = new FormControl('TEST INPUT');
    let sub = service.phoneValidate()(mockControl);

    if (isObservable(sub))  {
      sub.subscribe(x => expect(x).toEqual(null))
    }
  });

  it('should add error when validating email', () =>
  {
    mockUserService.IsEmailUnique.and.returnValue(of(false));

    let mockControl: AbstractControl = new FormControl('TEST INPUT');
    let sub = service.emailValidate()(mockControl);

    if (isObservable(sub))  {
      sub.subscribe(x => expect(x).toEqual({emailExists: true}))
    }
  });

  it('should add null when validating email', () =>
  {
    mockUserService.IsEmailUnique.and.returnValue(of(true));

    let mockControl: AbstractControl = new FormControl('TEST INPUT');
    let sub = service.emailValidate()(mockControl);

    if (isObservable(sub))  {
      sub.subscribe(x => expect(x).toEqual(null))
    }
  });
});
