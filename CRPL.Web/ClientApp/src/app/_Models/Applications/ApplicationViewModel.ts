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
  Created: Date;
  Modified: Date;
  TransactionId: string;
  Status: ApplicationStatus;
  AssociatedUsers: UserAccountMinimalViewModel[];
  AssociatedWork?: RegisteredWorkViewModel;
}
