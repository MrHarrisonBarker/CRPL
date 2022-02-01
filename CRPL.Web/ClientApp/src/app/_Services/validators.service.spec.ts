import {TestBed} from '@angular/core/testing';

import {ValidatorsService} from './validators.service';
import {UserService} from "./user.service";
import {isObservable, of} from "rxjs";
import {AbstractControl, FormControl, FormGroup} from "@angular/forms";

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

  it('should add error if non existent', () =>
  {
    let mockFormGroup: FormGroup = new FormGroup({
      Email: new FormControl(''),
      PhoneNumber: new FormControl('')
    })

    expect(service.hasOneContactInfo()(mockFormGroup.controls.Email)).toEqual({hasNoContact: true});
  });

  it('should add null if existent', () =>
  {
    let mockFormGroup: FormGroup = new FormGroup({
      Email: new FormControl('EMAIL'),
      PhoneNumber: new FormControl('')
    })

    expect(service.hasOneContactInfo()(mockFormGroup.controls.Email)).toEqual(null);
  });

  it('should return add null if no parent', () =>
  {
    expect(service.hasOneContactInfo()(new FormControl())).toEqual(null);
  });
});
