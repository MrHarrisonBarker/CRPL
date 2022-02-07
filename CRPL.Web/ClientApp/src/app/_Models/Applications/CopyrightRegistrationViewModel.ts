import {ApplicationViewModel} from "./ApplicationViewModel";
import {OwnershipStake} from "../StructuredOwnership/OwnershipStake";
import { CopyrightType } from "../CopyrightType";
import {WorkType} from "../WorkType";

export interface CopyrightRegistrationViewModel extends ApplicationViewModel
{
  WorkType: WorkType;
  Title: string;
  WorkHash: string;
  WorkUri: string;
  Legal: string;
  CopyrightType: CopyrightType;
  OwnershipStakes: OwnershipStake[];
  YearsExpire?: number;
}
