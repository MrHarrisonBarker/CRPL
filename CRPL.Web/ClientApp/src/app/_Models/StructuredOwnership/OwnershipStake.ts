export interface OwnershipStake
{
  Owner: string;
  Share: number;
}

export interface OwnershipStakeInput extends OwnershipStake
{
  Locked: boolean;
}
