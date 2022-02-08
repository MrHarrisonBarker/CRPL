import {ApplicationViewModel} from "../Applications/ApplicationViewModel";

export enum RegisteredWorkStatus
{
  Created,
  Verified,
  Registered
}

export interface RegisteredWorkViewModel
{
  Id: string,
  Created: Date,
  Registered?: Date,
  Status: RegisteredWorkStatus,
  RightId?: string,
  Hash?: string,
  RegisteredTransactionId: string,
  AssociatedApplication: ApplicationViewModel[]
}
