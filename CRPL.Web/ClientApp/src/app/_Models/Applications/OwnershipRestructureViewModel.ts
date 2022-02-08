import {OwnershipStake} from "../StructuredOwnership/OwnershipStake";
import {ApplicationViewModel} from "./ApplicationViewModel";

export interface OwnershipRestructureViewModel extends ApplicationViewModel
{
  CurrentStructure: OwnershipStake[];
  ProposedStructure: OwnershipStake[];
}
