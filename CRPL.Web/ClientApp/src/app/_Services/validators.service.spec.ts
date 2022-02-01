import { TestBed } from '@angular/core/testing';

import { ValidatorsService } from './validators.service';
import {UserService} from "./user.service";

describe('ValidatorsService', () => {
  let service: ValidatorsService;

  beforeEach(() => {
    let mockUserService = jasmine.createSpyObj('UserService', ['PhoneExists','EmailExists']);

    TestBed.configureTestingModule({
      providers: [
        {provide: UserService, useValue: mockUserService}
      ]
    });
    service = TestBed.inject(ValidatorsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
