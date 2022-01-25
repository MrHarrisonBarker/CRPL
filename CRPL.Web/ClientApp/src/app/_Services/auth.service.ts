import {Inject, Injectable} from '@angular/core';
import {HttpClient, HttpParams} from "@angular/common/http";
import {UserPaths} from "../api.conts";
import {from, Observable} from "rxjs";
import {AuthenticateResult} from "../_Models/Account/AuthenticateResult";
import {AuthenticateSignatureInputModel} from "../_Models/Account/AuthenticateSignatureInputModel";
import {map, tap} from "rxjs/operators";

@Injectable({
  providedIn: 'root'
})
export class AuthService
{
  private Ethereum = (window as any).ethereum;
  private readonly BaseUrl;

  Address: string = "";

  constructor (private http: HttpClient, @Inject('BASE_URL') baseUrl: string)
  {
    this.BaseUrl = baseUrl;
  }

  public LoginWithMetaMask ()
  {
    console.log("Logging in with Meta Mask");

    this.getWalletAddress().subscribe(a => {
      console.log("address found", a);
      if (a != null) this.fetchNonce().subscribe(nonce => {
        console.log("Got nonce", nonce);
        this.signMessage(nonce).subscribe(signature => {
          console.log("Signed message", signature);
          this.authenticateSignature({
            Signature: signature,
            WalletAddress: this.Address
          }).subscribe(auth => {
            console.log("Authentication result", auth);
          });
        });
      });
    });

  }

  private getWalletAddress (): Observable<string>
  {
    console.log("Getting users wallets from MM");
    return from<string>(this.Ethereum.request({
      method: 'eth_requestAccounts'
    })).pipe(tap(accounts => {
      if (accounts == null) throw new Error("NO ACCOUNTS FOUND");
      this.Address = accounts[0];
    })).pipe(map(s => s[0]));
  }

  private fetchNonce (): Observable<string>
  {
    console.log("Fetching nonce from server");
    return this.http.post<string>(this.BaseUrl + UserPaths.FetchNonce, {}, {params: new HttpParams().set('walletAddress', this.Address)});
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
    return this.http.post<AuthenticateResult>(this.BaseUrl + UserPaths.AuthenticateSignature, input);
  }
}
