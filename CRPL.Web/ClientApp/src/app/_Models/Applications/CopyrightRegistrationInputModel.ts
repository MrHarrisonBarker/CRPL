import {ApplicationInputModel} from "./ApplicationInputModel";
import {OwnershipStake} from "../StructuredOwnership/OwnershipStake";
import { CopyrightType } from "../CopyrightType";
import { WorkType } from "../WorkType";

export interface CopyrightRegistrationInputModel extends ApplicationInputModel
{
  Title?: string;
  WorkHash?: string;
  WorkUri?: string;
  Legal?: string;
  OwnershipStakes?: OwnershipStake[];
  YearsExpire?: number;
  CopyrightType?: CopyrightType;
  WorkType?: WorkType;
}
