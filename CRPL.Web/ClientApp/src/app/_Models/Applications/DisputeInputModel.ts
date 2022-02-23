import {ApplicationInputModel} from "./ApplicationInputModel";

export enum DisputeType
{
  Ownership,
  Usage
}

export interface DisputeInputModel extends ApplicationInputModel
{
  DisputeType?: DisputeType;
  Reason?: string;
  Spotted?: Date;
  Infractions: number;
  ExpectedRecourse: string;
  ContactAddress: string;
  LinkToInfraction?: string;

  DisputedWorkId?: string;
  AccuserId?: string;
}
