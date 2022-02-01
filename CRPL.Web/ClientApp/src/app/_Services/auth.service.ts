import {Inject, Injectable} from '@angular/core';
import {HttpClient, HttpParams} from "@angular/common/http";
import {UserPaths} from "../api.conts";
import {BehaviorSubject, from, Observable, of} from "rxjs";
import {AuthenticateResult} from "../_Models/Account/AuthenticateResult";
import {AuthenticateSignatureInputModel} from "../_Models/Account/AuthenticateSignatureInputModel";
import {finalize, map, switchMap, tap} from "rxjs/operators";
import jwtDecode, {JwtPayload} from "jwt-decode";
import {UserAccountViewModel} from "../_Models/Account/UserAccountViewModel";
import {Router} from "@angular/router";
import {AlertService} from "./alert.service";

@Injectable({
  providedIn: 'root'
})
export class AuthService
{
  private Ethereum = (window as any).ethereum;
  private readonly BaseUrl;

  private AuthenticationToken?: string = "";
  private Address?: string = "";

  public UserAccount: BehaviorSubject<UserAccountViewModel> = new BehaviorSubject<UserAccountViewModel>(null as any);
  public IsAuthenticated: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  constructor (
    private http: HttpClient,
    @Inject('BASE_URL') baseUrl: string,
    private router: Router,
    private alertService: AlertService)
  {
    this.BaseUrl = baseUrl;
    this.checkForToken();
  }

  private checkForToken (): void
  {
    let token = localStorage.getItem("authentication_token");
    let expires = localStorage.getItem("expires_at");

    if (token && expires && new Date(JSON.parse(expires)) > new Date())
    {
      console.log("in date token found in local storage!");

      this.AuthenticationToken = token;
    }
  }

  public getToken (): string | null
  {
    if (this.AuthenticationToken) return this.AuthenticationToken
    return null;
  }

  public LoginWithMetaMask (): Observable<AuthenticateResult>
  {
    console.log("Logging in with Meta Mask");
    this.alertService.StartLoading();

    return this.getWalletAddress().pipe(switchMap(walletAddress =>
    {
      console.log("address found", walletAddress);

      if (this.Address)
      {
        return this.fetchNonce().pipe(switchMap(nonce =>
        {
          console.log("Got nonce", nonce);

          if (nonce)
          {
            return this.signMessage(nonce).pipe(switchMap(signature =>
            {
              console.log("Signed message", signature);

              if (signature)
              {
                if (this.Address) return this.authenticateSignature({
                  Signature: signature,
                  WalletAddress: this.Address
                });
              }

              return of({Log: "There was a problem signing the message!"});
            }));
          }

          return of({Log: "Your account's nonce was not found!"});
        }));
      }

      return of({Log: "No wallet address was found! Make sure you have MetaMask setup."});
    }));
  }

  private getWalletAddress (): Observable<string>
  {
    console.log("Getting users wallets from MM");
    return from<string>(this.Ethereum.request({
      method: 'eth_requestAccounts'
    })).pipe(tap(accounts =>
    {
      if (accounts == null) throw new Error("NO ACCOUNTS FOUND");
      this.Address = accounts[0];
    })).pipe(map(s => s[0]));
  }

  public fetchNonce (): Observable<string>
  {
    console.log("Fetching nonce from server");
    if (this.Address) return this.http.post<string>(this.BaseUrl + UserPaths.FetchNonce, {}, {params: new HttpParams().set('walletAddress', this.Address)});
    throw new Error("Can't find public wallet address");
  }

  private signMessage (nonce: string): Observable<string>
  {
    let msg = "Signing a unique nonce " + nonce;
    console.log("Signing message using MM", msg);
    return from<string>(this.Ethereum.request({
      method: 'personal_sign',
      params: [this.Address, msg]
    }))
  }

  private authenticateSignature (input: AuthenticateSignatureInputModel): Observable<AuthenticateResult>
  {
    console.log("Authenticating signature with server");
    return this.http.post<AuthenticateResult>(this.BaseUrl + UserPaths.AuthenticateSignature, input).pipe(tap((authResult: AuthenticateResult) =>
    {
      if (authResult.Token)
      {
        this.AuthenticationToken = authResult.Token;
        if (authResult.Account)
        {
          this.UserAccount.next(authResult.Account);
          this.IsAuthenticated.next(true);
        }

        let payload: JwtPayload = jwtDecode(this.AuthenticationToken);

        if (payload != null && payload.exp)
        {
          console.log("saving jwt token");
          localStorage.setItem('authentication_token', this.AuthenticationToken);
          localStorage.setItem("expires_at", JSON.stringify(new Date(payload.exp * 1000).valueOf()));
        }
      }
    }));
  }

  public Authenticate (token: string): Observable<UserAccountViewModel>
  {
    if (this.UserAccount.getValue() != null) return of(this.UserAccount.getValue());
    return this.http.get<UserAccountViewModel>(this.BaseUrl + UserPaths.Auth, {
      params: new HttpParams().set('token', token)
    }).pipe(tap(account =>
    {
      console.log("got user account from auth", account);
      this.UserAccount.next(account);
    })).pipe(finalize(() => this.IsAuthenticated.next(true)));
  }

  public Logout (): void
  {
    localStorage.removeItem("authentication_token");
    localStorage.removeItem("expires_at");

    this.IsAuthenticated.next(false);
    this.UserAccount.next(null as any);
    this.Address = undefined;
    this.AuthenticationToken = undefined;

    this.alertService.Alert({Message: "Successfully logged out! See you soon", Type: "success"})
    this.router.navigate(["/"]).then(r => null);
  }
}
