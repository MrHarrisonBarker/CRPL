import {OwnershipStake} from "../StructuredOwnership/OwnershipStake";

export interface OwnershipRestructureInputModel
{
  CurrentStructure?: OwnershipStake[];
  ProposedStructure: OwnershipStake[];
}
