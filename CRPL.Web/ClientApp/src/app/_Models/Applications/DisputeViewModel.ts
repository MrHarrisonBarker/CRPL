import {ApplicationViewModel} from "./ApplicationViewModel";
import {DisputeType} from "./DisputeInputModel";
import {RegisteredWorkViewModel} from "../Works/RegisteredWork";
import {UserAccountMinimalViewModel} from "../Account/UserAccountMinimalViewModel";

export enum ExpectedRecourse
{
  ChangeOfOwnership,
  Payment
}

export interface DisputeViewModel extends ApplicationViewModel
{
  DisputeType: DisputeType;
  Reason:string;
  Spotted: Date;
  Infractions: number;
  ExpectedRecourse: ExpectedRecourse;
  ExpectedRecourseData: string;
  ContactAddress: string;
  LinkToInfraction: string;

  DisputedWork: RegisteredWorkViewModel;
  Accuser: UserAccountMinimalViewModel;
}
