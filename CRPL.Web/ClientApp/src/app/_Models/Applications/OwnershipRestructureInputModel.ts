import {OwnershipStake} from "../StructuredOwnership/OwnershipStake";
import {ApplicationInputModel} from "./ApplicationInputModel";

export interface OwnershipRestructureInputModel extends ApplicationInputModel
{
  WorkId?: string;
  CurrentStructure?: OwnershipStake[];
  ProposedStructure: OwnershipStake[];
}
