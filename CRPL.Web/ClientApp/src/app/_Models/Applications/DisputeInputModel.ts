import {ApplicationInputModel} from "./ApplicationInputModel";
import { ExpectedRecourse } from "./DisputeViewModel";

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
  ExpectedRecourse: ExpectedRecourse;
  ExpectedRecourseData: string;
  ContactAddress: string;
  LinkToInfraction?: string;

  DisputedWorkId?: string;
  AccuserId?: string;
}
