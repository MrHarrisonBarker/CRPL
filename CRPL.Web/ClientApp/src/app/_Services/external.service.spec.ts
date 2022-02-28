import { TestBed } from '@angular/core/testing';

import { ExternalService } from './external.service';
import {HttpClientTestingModule} from "@angular/common/http/testing";

describe('ExternalService', () => {
  let service: ExternalService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        ExternalService
      ]
    });
    service = TestBed.inject(ExternalService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
