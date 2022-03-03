import {Component, OnInit} from '@angular/core';
import {QueryService} from "../_Services/query.service";
import {RegisteredWorkViewModel} from "../_Models/Works/RegisteredWork";
import {DisputeViewModel} from "../_Models/Applications/DisputeViewModel";
import {forkJoin, Subscription} from "rxjs";
import {mergeMap} from "rxjs/operators";
import {AngularFireMessaging} from '@angular/fire/compat/messaging';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html'
})
export class HomeComponent implements OnInit
{
  public Page: number = 0;
  public RecentCopyrights!: RegisteredWorkViewModel[];
  public OpenDisputes!: DisputeViewModel[];

  public Token!: string;
  private msgSub!: Subscription;

  constructor (private queryService: QueryService, private afMessaging: AngularFireMessaging)
  {
  }

  async ngOnInit (): Promise<any>
  {
    return forkJoin([this.queryService.GetRecent(this.Page), this.queryService.GetDisputes(this.Page)]).subscribe(x =>
    {
      this.RecentCopyrights = x[0];
      this.OpenDisputes = x[1];
    });
  }

  requestPermission ()
  {
    this.afMessaging.requestToken
        .subscribe(
          (token) =>
          {
            if (token) {
              console.log('Permission granted! Save to the server!', token);
              this.Token = token;
            }
          },
          (error) =>
          {
            console.error(error);
          },
        );
  }

  listen ()
  {
    console.log("listening")
    if (this.msgSub) this.msgSub.unsubscribe();
    this.msgSub = this.afMessaging.messages
                      .subscribe((message) =>
        {
          console.log(message);
        });
  }

  deleteToken ()
  {
    if (this.Token != null)
    {
      this.afMessaging.getToken
          .pipe(mergeMap(token => this.afMessaging.deleteToken(this.Token)))
          .subscribe(
            (token) =>
            {
              console.log('Token deleted!');
            },
          );
    }
  }

}
