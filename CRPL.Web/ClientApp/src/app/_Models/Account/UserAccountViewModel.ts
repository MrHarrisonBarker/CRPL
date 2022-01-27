import {DOB} from "./DOB";

export enum AccountStatus
{
  Created,
  Incomplete,
  Complete
}

export interface UserAccountViewModel
{
  Id: string;
  FirstName: string;
  LastName: string;
  DateOfBirth: DOB;
  RegisteredJurisdiction: string;
  Email: string;
  DialCode: string;
  PhoneNumber: string;
  Status: AccountStatus;
  WalletPublicAddress: string;
}
