import {ApplicationViewModel} from "../Applications/ApplicationViewModel";
import {OwnershipStake} from "../StructuredOwnership/OwnershipStake";

export enum RegisteredWorkStatus
{
  Created,
  Verified,
  Registered
}

export interface ProposalVote
{
  Voter: string;
  Accepted: boolean;
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
  OwnershipStructure?: OwnershipStake[],
  AssociatedApplication?: ApplicationViewModel[]
  CurrentVotes?: ProposalVote[]
}
