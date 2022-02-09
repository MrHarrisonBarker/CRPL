import {OwnershipStake} from "../StructuredOwnership/OwnershipStake";

export interface OwnershipRestructureInputModel
{
  WorkId?: string;
  CurrentStructure?: OwnershipStake[];
  ProposedStructure: OwnershipStake[];
}
