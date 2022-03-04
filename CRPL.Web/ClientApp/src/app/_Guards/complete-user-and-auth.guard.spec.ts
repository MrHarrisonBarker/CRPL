// describe('CompleteUserAndAuthGuard', () =>
// {
//   let guard: CompleteUserAndAuthGuard;
//
//   let routerMock = jasmine.createSpyObj('router', ['navigate']);
//   let alertMock = jasmine.createSpyObj('alertMock', ['Alert']);
//   let authMock = jasmine.createSpyObj('AuthService', ['getToken', 'Authenticate', 'Logout'], ['IsAuthenticated']);
//
//   beforeEach(() =>
//   {
//     TestBed.configureTestingModule({
//       imports: [HttpClientTestingModule],
//       providers: [
//         AuthGuard,
//         {provide: AuthService, useValue: authMock},
//         {provide: AlertService, useValue: alertMock},
//         {provide: Router, useValue: routerMock},
//         {provide: 'BASE_URL', useValue: ''}]
//     });
//     guard = TestBed.inject(CompleteUserAndAuthGuard);
//   });
//
//   it('should be created', () =>
//   {
//     expect(guard).toBeTruthy();
//   });
//
//   it('should return true if already authenticated', () =>
//   {
//     setProp<BehaviorSubject<boolean>>(authMock, 'IsAuthenticated', new BehaviorSubject<boolean>(true));
//
//     guard.canActivate({} as ActivatedRouteSnapshot, <RouterStateSnapshot>{url: 'testUrl'}).subscribe(res =>
//     {
//       expect(res).toBeTruthy();
//     });
//   });
//
//   it('should route away if no token', () =>
//   {
//     setProp<BehaviorSubject<boolean>>(authMock, 'IsAuthenticated', new BehaviorSubject<boolean>(false));
//     authMock.getToken.and.returnValue(null);
//     routerMock.navigate.and.returnValue(new Promise<boolean>(() => true));
//
//     guard.canActivate({} as ActivatedRouteSnapshot, <RouterStateSnapshot>{url: 'testUrl'}).subscribe(res =>
//     {
//       // expect(routerMock.navigate).toHaveBeenCalled(); TODO: router tests not consistent
//       expect(res).toBeFalsy();
//     });
//
//   });
//
//   // it('should return false and route away if no user', () =>
//   // {
//   //   setProp<BehaviorSubject<boolean>>(authMock, 'IsAuthenticated', new BehaviorSubject<boolean>(false));
//   //   authMock.getToken.and.returnValue("TEST TOKEN");
//   //   routerMock.navigate.and.returnValue(new Promise<boolean>(() => true));
//   //   authMock.Authenticate.and.returnValue(of(null));
//   //
//   //   guard.canActivate({} as ActivatedRouteSnapshot, <RouterStateSnapshot>{url: 'testUrl'}).subscribe(res =>
//   //   {
//   //     // expect(routerMock.navigate).toHaveBeenCalled(); TODO: router tests not consistent
//   //     expect(res).toBeFalsy();
//   //   });
//   // });
//
//   it('should route to info wizard if user not complete', () =>
//   {
//     let mockUser: UserAccountViewModel = {
//       WalletAddressUri: "",
//       DateOfBirth: {Day: 0, Month: 0, Year: 0},
//       DialCode: "",
//       Email: "",
//       FirstName: "",
//       Id: "",
//       LastName: "",
//       PhoneNumber: "",
//       RegisteredJurisdiction: "",
//       Status: AccountStatus.Incomplete,
//       WalletPublicAddress: ""
//     };
//
//     setProp<BehaviorSubject<boolean>>(authMock, 'IsAuthenticated', new BehaviorSubject<boolean>(false));
//     authMock.getToken.and.returnValue("TEST TOKEN");
//     // routerMock.navigate.and.returnValue(new Promise<boolean>(() => true));
//     authMock.Authenticate.and.returnValue(of(mockUser));
//
//     guard.canActivate({} as ActivatedRouteSnapshot, <RouterStateSnapshot>{url: 'testUrl'}).subscribe(res =>
//     {
//       // expect(routerMock.navigate).toHaveBeenCalled(); TODO: router tests not consistent
//       expect(res).toBeFalsy();
//     });
//   });
//
//   it('should catch authentication error', () =>
//   {
//     setProp<BehaviorSubject<boolean>>(authMock, 'IsAuthenticated', new BehaviorSubject<boolean>(false));
//     authMock.getToken.and.returnValue("TEST TOKEN");
//     routerMock.navigate.and.returnValue(new Promise<boolean>(() => true));
//     authMock.Authenticate.and.returnValue(throwError(new Error("This is an error")));
//     authMock.Authenticate.and.returnValue(of(null));
//
//     guard.canActivate({} as ActivatedRouteSnapshot, <RouterStateSnapshot>{url: 'testUrl'}).subscribe(res => {
//       console.log(res);
//       expect(res).toBeFalsy();
//       // expect(routerMock).toHaveBeenCalled(); TODO: router tests not consistent
//       expect(alertMock).toHaveBeenCalledWith({Message: "There was a problem when authenticating your account", Type: "danger"});
//     });
//
//   });
// });
