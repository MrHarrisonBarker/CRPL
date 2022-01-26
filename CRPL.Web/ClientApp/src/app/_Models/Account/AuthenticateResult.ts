import {UserAccountViewModel} from "./UserAccountViewModel";

export interface AuthenticateResult
{
  Token: string;
  Log: string;
  Account: UserAccountViewModel;
}
