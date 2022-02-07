import {UserAccountMinimalViewModel} from "../Account/UserAccountMinimalViewModel";
import {ApplicationStatus} from "./ApplicationStatus";
import {RegisteredWorkViewModel} from "../Works/RegisteredWork";

export enum ApplicationType
{
  CopyrightRegistration,
  OwnershipRestructure,
  CopyrightTypeChange,
  Dispute
}

export interface ApplicationViewModel
{
  Id: string;
  ApplicationType: ApplicationType;
  Creates: Date;
  Modified: Date;
  Status: ApplicationStatus;
  AssociatedUsers: UserAccountMinimalViewModel[];
  AssociatedWork?: RegisteredWorkViewModel;
}
