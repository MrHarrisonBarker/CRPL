import {Inject, Injectable} from '@angular/core';
import {WarehouseService} from "./warehouse.service";
import * as signalR from '@microsoft/signalr';
import {HubConnection, HubConnectionState, LogLevel} from '@microsoft/signalr';
import {ApplicationViewModel} from "../_Models/Applications/ApplicationViewModel";
import {RegisteredWorkViewModel} from "../_Models/Works/RegisteredWork";
import {AuthService} from "./auth.service";

@Injectable({
  providedIn: 'root'
})
export class ResonanceService
{
  private readonly BaseUrl: string;
  private ResonanceConnection!: HubConnection;

  private ListenedWorks: string[] = [];
  private ListenedApplications: string[] = [];

  private ApplicationsToListen: string[] = [];
  private WorksToListen: string[] = [];

  constructor (@Inject('BASE_URL') baseUrl: string, private warehouseService: WarehouseService, private authService: AuthService)
  {
    this.BaseUrl = baseUrl;
    this.ResonanceConnection = new signalR.HubConnectionBuilder()
      .configureLogging(LogLevel.Information)
      .withUrl(this.BaseUrl + 'hubs/resonance')
      .withAutomaticReconnect()
      .build();

    this.warehouseService.__MyWorks.subscribe(works =>
    {
      console.log("[resonance-service] The works in the warehouse have been updated");
      for (let work of works)
      {
        if (!this.ListenedWorks.includes(work.Id)) this.ListenToWork(work.Id);
      }
    });

    this.warehouseService.__MyApplications.subscribe(applications =>
    {
      console.log("[resonance-service] The applications in the warehouse have been updated", applications, this.ListenedApplications);
      for (let application of applications)
      {
        console.log("[resonance-service] checking if application is listened", application);
        if (!this.ListenedApplications.includes(application.Id))
        {
          console.log("[resonance-service] the application is not being listened too");
          this.ListenToApplication(application.Id);
        }
      }
    });
  }

  public ConnectSockets ()
  {
    console.log('Connecting to sockets');

    this.ResonanceConnection.start().then(() =>
    {
      console.log("[resonance-service] Connected to RESONANCE websocket");
      this.authService.IsAuthenticated.subscribe(authed => {
        if (authed) this.ResonanceConnection.invoke("RegisterUser", this.authService.UserAccount.getValue().Id).then(r => null);
      })

      for (let application of this.ApplicationsToListen)
      {
        this.ListenToApplication(application);
      }
      this.ApplicationsToListen = [];

      for (let work of this.WorksToListen)
      {
        this.ListenToWork(work);
      }
      this.WorksToListen = [];

      this.ResonanceConnection.on("PushApplication", (data: ApplicationViewModel) => this.warehouseService.UpdateApplication(data));
      this.ResonanceConnection.on("PushWork", (data: RegisteredWorkViewModel) => this.warehouseService.UpdateWork(data));

    }).catch(function (err)
    {
      return console.error(err.toString());
    });
  }

  public ListenToApplication (id: string)
  {
    console.log("[resonance-service] Trying to listen for application " + id);

    if (this.ResonanceConnection.state != HubConnectionState.Connected) {
      console.log("[resonance-service] The hub is still connecting will listen later to " + id);
      this.ApplicationsToListen.push(id);
    }

    if (this.ResonanceConnection.state == HubConnectionState.Connected)
    {
      this.ResonanceConnection.invoke("ListenToApplication", id).then(value =>
      {
        console.log(`[resonance-service] the client is now listening to the application ${id}`);
        this.ListenedApplications.push(id);
      });
    }
  }

  public ListenToWork (id: string)
  {
    console.log("[resonance-service] Trying to listen for work " + id);

    if (this.ResonanceConnection.state != HubConnectionState.Connected) {
      console.log("[resonance-service] The hub is still connecting will listen later to " + id);
      this.WorksToListen.push(id);
    }

    if (this.ResonanceConnection.state == HubConnectionState.Connected)
    {
      this.ResonanceConnection.invoke("ListenToWork", id).then(value =>
      {
        console.log(`[resonance-service] the client is now listening to the work ${id}`);
        this.ListenedWorks.push(id);
      });
    }
  }
}
