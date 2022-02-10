import {ApplicationViewModel} from "../Applications/ApplicationViewModel";
import {OwnershipStake} from "../StructuredOwnership/OwnershipStake";
import {ProposalVote} from "../StructuredOwnership/ProposalVote";

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
  OwnershipStructure?: OwnershipStake[],
  AssociatedApplication?: ApplicationViewModel[],
  CurrentVotes?: ProposalVote[],
  HasProposal?: boolean
}
