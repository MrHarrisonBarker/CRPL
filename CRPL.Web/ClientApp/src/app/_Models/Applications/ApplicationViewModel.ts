import {UserAccountMinimalViewModel} from "../Account/UserAccountMinimalViewModel";
import {ApplicationStatus} from "./ApplicationStatus";

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
}
