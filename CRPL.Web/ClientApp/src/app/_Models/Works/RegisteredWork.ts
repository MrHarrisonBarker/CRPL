import {ApplicationViewModel} from "../Applications/ApplicationViewModel";
import {OwnershipStake} from "../StructuredOwnership/OwnershipStake";
import {ProposalVote} from "../StructuredOwnership/ProposalVote";

export enum RegisteredWorkStatus
{
  Created,
  ProcessingVerification,
  Verified,
  SentToChain,
  Registered,
  Rejected,
  Expired
}

export interface CopyrightMeta
{
  Title: string;
  Expires: number;
  Registered: number;
  WorkHash: string;
  WorkUri: string;
  LegalMeta: string;
}

interface VerificationResult
{
  IsAuthentic: boolean;
  Collision: string;
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
  RegisteredTransactionUri?: string;
  OwnershipStructure?: OwnershipStake[],
  AssociatedApplication?: ApplicationViewModel[],
  CurrentVotes?: ProposalVote[],
  HasProposal?: boolean,
  Meta?: CopyrightMeta,
  VerificationResult?: VerificationResult;
  Title?: string;
}
