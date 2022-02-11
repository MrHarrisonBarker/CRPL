import {ApplicationViewModel} from "./ApplicationViewModel";
import {OwnershipStake} from "../StructuredOwnership/OwnershipStake";
import { CopyrightType } from "../CopyrightType";
import {WorkType} from "../WorkType";
import {Protections} from "./Protections";

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
  Protections: Protections;
}
