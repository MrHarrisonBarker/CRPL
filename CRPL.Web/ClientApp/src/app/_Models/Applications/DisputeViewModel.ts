import {ApplicationViewModel} from "./ApplicationViewModel";
import {DisputeType} from "./DisputeInputModel";
import {RegisteredWorkViewModel} from "../Works/RegisteredWork";
import {UserAccountMinimalViewModel} from "../Account/UserAccountMinimalViewModel";

export enum ExpectedRecourse
{
  ChangeOfOwnership,
  Payment
}

export enum ResolvedStatus
{
  Created,
  NeedsOnChainAction,
  Processing,
  Resolved,
  Failed
}

export interface ResolveResult
{
  Rejected: boolean;
  ResolvedStatus: ResolvedStatus;
  Transaction?: string;
  TransactionUri?: string;
  Message?: string;
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
  ResolveResult: ResolveResult;

  DisputedWork: RegisteredWorkViewModel;
  Accuser: UserAccountMinimalViewModel;
}
