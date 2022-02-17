import {inject, TestBed} from '@angular/core/testing';

import {WorksService} from './works.service';
import {HttpClientTestingModule, HttpTestingController} from "@angular/common/http/testing";
import {AuthService} from "./auth.service";
import {AlertService} from "./alert.service";
import {Router} from "@angular/router";
import {UserService} from "./user.service";

describe('WorksService', () =>
{
  let service: WorksService;
  let routerMock = jasmine.createSpyObj('router', ['navigate']);
  let alertMock = jasmine.createSpyObj('alertMock', ['Alert'])
  let authMock = jasmine.createSpyObj('AuthService', [''], ['IsAuthenticated', 'UserAccount'])

  beforeEach(() =>
  {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        WorksService,
        {provide: AuthService, useValue: authMock},
        {provide: AlertService, useValue: alertMock},
        {provide: Router, useValue: routerMock},
        {provide: 'BASE_URL', useValue: ''}]
    });
    service = TestBed.inject(WorksService);
  });

  it('should be created', () =>
  {
    expect(service).toBeTruthy();
  });

  // it('should upload', inject(
  //   [HttpTestingController, UserService],
  //   (httpMock: HttpTestingController, worksService: WorksService) =>
  //   {
  //     let mockFile: File = new File([''], 'test-file.pdf');
  //
  //     service.UploadWork(mockFile).subscribe(result => {
  //       expect(result).toEqual({type: 0});
  //     });
  //
  //     let request = httpMock.expectOne('works');
  //
  //     request.flush(null);
  //   }));

  it('should get signed work', inject(
    [HttpTestingController, WorksService],
    (httpMock: HttpTestingController, worksService: WorksService) =>
    {
      let mockResponse = new Blob(['1','1','1']);
      let mockId = 'ID';

      worksService.GetSignedWork(mockId).subscribe(response => {
        expect(response).toEqual(mockResponse);
      });

      let request = httpMock.expectOne('works/' + encodeURI(mockId));

      request.flush(mockResponse);
    }));
});
