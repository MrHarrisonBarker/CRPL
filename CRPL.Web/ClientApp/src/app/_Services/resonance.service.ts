import {Inject, Injectable} from '@angular/core';
import {WarehouseService} from "./warehouse.service";
import * as signalR from '@microsoft/signalr';
import {HubConnection, HubConnectionState, LogLevel} from '@microsoft/signalr';
import {ApplicationViewModel} from "../_Models/Applications/ApplicationViewModel";
import {RegisteredWorkViewModel} from "../_Models/Works/RegisteredWork";

@Injectable({
  providedIn: 'root'
})
export class ResonanceService
{
  private readonly BaseUrl: string;
  private ApplicationsConnection!: HubConnection;
  private WorksConnection!: HubConnection;

  private ListenedWorks: string[] = [];
  private ListenedApplications: string[] = [];

  constructor (@Inject('BASE_URL') baseUrl: string, private warehouseService: WarehouseService)
  {
    this.BaseUrl = baseUrl;
    this.ApplicationsConnection = new signalR.HubConnectionBuilder()
      .configureLogging(LogLevel.Information)
      .withUrl(this.BaseUrl + 'hubs/applications')
      .withAutomaticReconnect()
      .build();

    this.WorksConnection = new signalR.HubConnectionBuilder()
      .configureLogging(LogLevel.Information)
      .withUrl(this.BaseUrl + 'hubs/works')
      .withAutomaticReconnect()
      .build();

    this.warehouseService.__MyWorks.subscribe(works =>
    {
      console.log("The works in the warehouse have been updated");
      for (let work of works)
      {
        if (!this.ListenedWorks.includes(work.Id)) this.ListenToWork(work.Id);
      }
    });

    this.warehouseService.__MyApplications.subscribe(applications =>
    {
      console.log("The applications in the warehouse have been updated");
      for (let application of applications)
      {
        if (!this.ListenedApplications.includes(application.Id)) this.ListenToApplication(application.Id);
      }
    });
  }

  public ConnectSockets ()
  {
    console.log('Connecting to sockets');

    this.ApplicationsConnection.start().then(() =>
    {
      console.log("Connected to APPLICATIONS websocket");

      this.ApplicationsConnection.on("PushApplication", (data: ApplicationViewModel) => this.warehouseService.UpdateApplication(data));

    }).catch(function (err)
    {
      return console.error(err.toString());
    });

    this.WorksConnection.start().then(() =>
    {
      console.log("Connected to WORKS websocket");

      this.ApplicationsConnection.on("PushWork", (data: RegisteredWorkViewModel) => this.warehouseService.UpdateWork(data));

    }).catch(function (err)
    {
      return console.error(err.toString());
    });
  }

  public ListenToApplication (id: string)
  {
    console.log("Listening to " + id);
    if (this.ApplicationsConnection.state == HubConnectionState.Connected)
    {
      this.ApplicationsConnection.invoke("ListenToApplication", id).then(value => console.log(`the client is now listening to the application ${id}`));
    }
  }

  public ListenToWork (id: string)
  {
    console.log("Listening to " + id);
    if (this.WorksConnection.state == HubConnectionState.Connected)
    {
      this.WorksConnection.invoke("ListenToWork", id).then(value => console.log(`the client is now listening to the work ${id}`));
    }
  }
}
