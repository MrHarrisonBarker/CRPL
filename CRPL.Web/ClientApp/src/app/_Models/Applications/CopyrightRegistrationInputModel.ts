import {ApplicationInputModel} from "./ApplicationInputModel";
import {OwnershipStake} from "../StructuredOwnership/OwnershipStake";

export interface CopyrightRegistrationInputModel extends ApplicationInputModel
{
  Title?: string;
  WorkHash?: string;
  WorkUri?: string;
  Legal?: string;
  OwnershipStakes?: OwnershipStake[];
}
